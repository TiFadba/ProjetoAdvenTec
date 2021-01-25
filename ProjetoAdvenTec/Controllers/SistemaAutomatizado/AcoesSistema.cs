using ProjetoAdvenTec.Models.DataBase;
using ProjetoAdvenTec.Models;
using ProjetoAdvenTec.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoAdvenTec.Controllers.SistemaAutomatizado
{

    public class AcoesSistema : IAcoesSistema // Classe excluiva para automatização de ações/rotinas do sistema
    {
        public readonly DataContext _context; // 'banco de dados'

        public AcoesSistema(DataContext context)
        {
            _context = context;
        }


        //Método utilizado para iniciar a avaliação automaticamente quando a data da avaliação e a data de inicio são iguais. 
        public bool AcioneManualAvalicao(DateTime referencia)
        {
            //Busca as avaliaçoes
            var checarAvaliacoes = _context.AvaliacaoMensal.AsQueryable();
            checarAvaliacoes = checarAvaliacoes.Where(a => a.dataReferencia.Date.Equals(referencia.Date));

            AvaliacaoMensal avaliacao = checarAvaliacoes.FirstOrDefault();

            if (avaliacao != null)
            {
                //Busca os clientes
                List<Cliente> listaClientes = _context.Cliente.ToList();

                //Inicia a criação dos links que serão enviados ao clientes
                foreach (var cliente in listaClientes)
                {
                    LinkAvalicao novoLink = new LinkAvalicao()
                    {
                        idAvaliacao = avaliacao.id,
                        idCliente = cliente.id,
                        linkAvaliacaoEmail = TokenService.GerarTokenLinkEmail() // Service para gerar um token para o link do email.
                    };

                    CriarLinkClienteAvaliacao(novoLink); // registra os links no banco

                    //Envia os emails para todos os clientes.
                    EnviarLinksEmail(cliente.email, novoLink.linkAvaliacaoEmail, avaliacao.dataReferencia.ToString("MM/yyyy"));
                }

                return true;
            }

            return false;
        }

        //Metodo que inicia uma avaliação que foi agendada
        public void VerificarNovasAvalicoes()
        {
            DateTime dataAtual = DateTime.Now.Date;

            //Busca as potenciais avaliações que vão iniciar
            var checarAvaliacoes = _context.AvaliacaoMensal.AsQueryable();

            checarAvaliacoes = checarAvaliacoes.Where(a => !a.encerrado && !(a.dataReferencia.Date.Equals(a.dataInicio.Date)));

            List<AvaliacaoMensal> todasAvaliacoesMensais = checarAvaliacoes.ToList();
            
            //Havendo avaliações, é iniciado um loop que cria o link, registra no banco e envia os links para os clientes.
            if(todasAvaliacoesMensais.Count > 0)
            {
                foreach (var avaliacao in todasAvaliacoesMensais)
                {
                    if(avaliacao.dataInicio.Date.Month.Equals(dataAtual.Date.Month) && avaliacao.dataInicio.Date.Year.Equals(dataAtual.Date.Year))
                    {
                        List<Cliente> listaClientes = _context.Cliente.ToList();

                        foreach(var cliente in listaClientes)
                        {
                            LinkAvalicao novoLink = new LinkAvalicao()
                            {
                                idAvaliacao = avaliacao.id,
                                idCliente = cliente.id,
                                linkAvaliacaoEmail = TokenService.GerarTokenLinkEmail() // Service para gerar um token para o link do email.
                            };

                            CriarLinkClienteAvaliacao(novoLink); // registra os links no banco

                            //Envia os emails para todos os clientes.
                            EnviarLinksEmail(cliente.email, novoLink.linkAvaliacaoEmail, avaliacao.dataReferencia.ToString("MM/yyyy"));
                        }
                    }
                }
            }
        }

        //Metodo que acompanha as avalições que estão sendo feita e encerra-as no periodo indicado
        public void VerificarAvalicoesProgresso()
        {
            DateTime dataAtual = DateTime.Now.Date;

            //dataAtual.AddDays(2);

            //Busca as avaliações que encerram no dia atual
            var checarAvaliacoes = _context.AvaliacaoMensal.AsQueryable();
            checarAvaliacoes = checarAvaliacoes.Where(a => !a.encerrado);

            List<AvaliacaoMensal> todasAvaliacoesMensais = checarAvaliacoes.ToList();

            //Havendo o sistema trata de encerra-las e gerar o relatótio com os resultados finais.
            if(todasAvaliacoesMensais.Count > 0)
            {
                foreach(var avaliacao in todasAvaliacoesMensais)
                {
                    //Checa o dia seguinte ao dia selecionado para o termino do periodo avaliativo
                    avaliacao.dataInicio.AddDays(avaliacao.diasExpirar + 1);

                    if (avaliacao.dataInicio.Date.Equals(dataAtual.Date))
                    {
                        var linkAvalicaoEncerrada = _context.LinkAvalicao.AsQueryable();
                        linkAvalicaoEncerrada = linkAvalicaoEncerrada.Where(l => l.idAvaliacao.Equals(avaliacao.id));

                        List<LinkAvalicao> listaLinksEncerrados = linkAvalicaoEncerrada.ToList();

                        //Excluí os links que davam acesso a avaliação agora finalizada
                        foreach(var link in listaLinksEncerrados)
                        {
                            _context.LinkAvalicao.Remove(link);
                        }

                        //encerra a avaliação
                        avaliacao.encerrado = true;

                        //Chamada ao metodo que faz os cálculos e gera os resultados das avaliações
                        RelatorioFinalAvaliacao(avaliacao);

                        //Atualiza o banco com as informações recentes
                        _context.AvaliacaoMensal.Update(avaliacao);
                        _context.SaveChanges();
                    }
                }
            }
        }

        //Metódo que gerar o resultado da avaliação
        public AvaliacaoMensal RelatorioFinalAvaliacao(AvaliacaoMensal avaliacaoFinalizada)
        {
            //busca os clientes que participaram da avaliação no mes que fez referencia
            var listaAvalicoesClientes = _context.Avaliacao.AsQueryable();
            listaAvalicoesClientes = listaAvalicoesClientes.Where(a => a.idAvaliacao.Equals(avaliacaoFinalizada.id));

            double promotores = 0;
            double neutros = 0;
            double detratores = 0;
            double NPS = 0;
            int totalParticioantes = 0;

            //Faz a classificação dos clientes apartir de suas avaliações/notas
            foreach(var resultado in listaAvalicoesClientes)
            {
                if (resultado.nota <= 6)
                {
                    detratores++;
                }
                else if (resultado.nota <= 8)
                {
                    neutros++;
                }
                else
                {
                    promotores++;
                }
            }

            //Equação que indica a porcentagem final de agrado referente ao mês
            totalParticioantes = listaAvalicoesClientes.Count();
            NPS = ((promotores -detratores) / totalParticioantes) * 100;

            //Dados sendo salvos para serem persistidos no banco
            avaliacaoFinalizada.detratores =(int) detratores;
            avaliacaoFinalizada.neutros = (int) neutros;
            avaliacaoFinalizada.promotores = (int) promotores;
            avaliacaoFinalizada.nps = (int) NPS;

            // retorna a 'atualização' dos campos
            return avaliacaoFinalizada;

        }

        //Metodo que persisti/registra os links no banco de dados
        private bool CriarLinkClienteAvaliacao(LinkAvalicao novoLinkAvalicao)
        {
            _context.Add(novoLinkAvalicao);
            _context.SaveChanges();

            return true;

        }

        //Metodo que monta(prepara as informações) o email a ser enviado
        private bool EnviarLinksEmail(string emailCliente, string linkCliente, string data)
        {

            int resultado;
            EmailModel emailCorporatiovo = new EmailModel();
            emailCorporatiovo.to = emailCliente;
            emailCorporatiovo.subject = "Link para avaliação de serviço.";
            emailCorporatiovo.body = "Avaliação referente o mês "+ data  +
                "\n\n" +
                "https://localhost:44362/Avaliacoes/AvaliacaoMensal?linkAcessoAvaliacao=" + linkCliente;

            resultado = EmailService.EnviarEmailAutomatico(emailCorporatiovo); // Service que faz o envio dos emails

            if (resultado == 1)
            {
                // kkk vou por alguma coisa aqui um dia
            }


            return true;
        }

    }
}
