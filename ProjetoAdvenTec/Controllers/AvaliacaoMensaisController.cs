using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetoAdvenTec.Controllers.SistemaAutomatizado;
using ProjetoAdvenTec.Models;
using ProjetoAdvenTec.Models.DataBase;
using ProjetoAdvenTec.Models.Modelos_de_Validacao;
using static ProjetoAdvenTec.Services.BloquearAcessoDireto;

namespace ProjetoAdvenTec.Controllers
{
    //Classe para ações ligadas a entidade 'AvaliacaoMensal', havendo CRUD, validações e outros metodos.
    public class AvaliacaoMensaisController : Controller 
    {
        private readonly DataContext _context; // Banco de Dados

        public AvaliacaoMensaisController(DataContext context)
        {
            _context = context;
        }

        //Metodo que chama a pagina principal do controller
        // GET: AvaliacaoMensais
        [NoDirectAccess]
        public async Task<IActionResult> Index(Usuario user)
        {
            ViewBag.idUsuario = user.id; // Passar dados para a view de modo isolado para futuras requisições e ações
            ViewBag.infoUsuario = user; // Passar dados para a view de modo isolado para futuras requisições e ações

            return View(await _context.AvaliacaoMensal.ToListAsync()); // retorna todas as avaliações
        }


        [NoDirectAccess]
        //mostra os dados relacionados as avalições criadas
        public async Task<IActionResult> DetalhesGerais(int? id, string idUsuario)
        {
            ViewBag.idAvaliacao = Convert.ToInt32(id); // Passar dados para a view de modo isolado para futuras requisições e ações
            
            /*Atributo que é copartilhado entra todas as views para persistir 
             * o usuario que esta utilizando o sistema e sempre atualizando
             * suas condições.*/
            ViewBag.idUsuario = Convert.ToInt32(idUsuario); 

            if (id == null)
            {
                return NotFound();
            }

            var avaliacaoMensal = await _context.AvaliacaoMensal
                .FirstOrDefaultAsync(m => m.id == id);
            if (avaliacaoMensal == null)
            {
                return NotFound();
            }

            return View(avaliacaoMensal);

        }

        [NoDirectAccess]
        //Retorna os  que foram pesquisados para a view
        public async Task<IActionResult> ListaAvaliacoesClientes(string idAvaliacao, string idUsuario)
        {
            List<ClienteAvaliacao> listaClientesAvaliacao = new List<ClienteAvaliacao>(); 

            int idAvaliacaoBusca = Convert.ToInt32(idAvaliacao);

            //Persitir informações na view
            ViewBag.idAvaliacao = idAvaliacaoBusca;
            ViewBag.idUsuario = Convert.ToInt32(idUsuario);

            //busca as avalações dos clientes ligadas ao id da avaliação mensal
            var buscarAvaliacoes = _context.Avaliacao.AsQueryable();
            buscarAvaliacoes = buscarAvaliacoes.Where(a => a.idAvaliacao.Equals(idAvaliacaoBusca));

            //recebe as avaliações encontradas
            List<Avaliacao> avaliacoesEncontradas = await buscarAvaliacoes.ToListAsync();

            //laço para reorganizar os atributos do cliente e da avaliação em um novo objeto com apenas dados necessarios
            foreach (var avaliacao in avaliacoesEncontradas)
            {
                //busca o cliente que fez a avaliação que esta sendo analisada/buscada
                Cliente cliente = _context.Cliente.Find(avaliacao.idCliente);

                ClienteAvaliacao clienteAvaliacao = new ClienteAvaliacao()
                {
                    idAvaliacao = avaliacao.id,
                    idCliente = cliente.id,
                    nomeCliente = cliente.nomeCliente,
                    nota = avaliacao.nota,
                    razaoNota = avaliacao.razaoNota
                    
                };

                listaClientesAvaliacao.Add(clienteAvaliacao);

            }

            //retorna a lista com os dados necessarios para atender a requisição
            return View(listaClientesAvaliacao);
        }

        [NoDirectAccess]
        // GET: AvaliacaoMensais/Details/5
        //Traz os dados relacionados a uma avaliação pelo seu id
        public  IActionResult Details(int ? id, string idUsuario)
        {
            ViewBag.idAvaliacao = id;
            ViewBag.idUsuario = idUsuario;

            return View();
        }

        [NoDirectAccess]
        // GET: AvaliacaoMensais/Create
        //Cria uma Avaliação
        public IActionResult Create(string idUsuario)
        {
            ViewBag.idUsuario = Convert.ToInt32(idUsuario);
            return View();
        }

        // POST: AvaliacaoMensais/Create
        //Requisição para persistencia da criação de uma avaliação no dados no banco 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,dataReferencia,dataInicio,diasExpirar,encerrado")] AvaliacaoMensal avaliacaoMensal, IFormCollection form)
        {
            if (ModelState.IsValid)
            {
                _context.Add(avaliacaoMensal);
                await _context.SaveChangesAsync();


                //Verifica se o usuario requisitou o inicio imediato da avaliação ou agendou
                if (avaliacaoMensal.dataReferencia.Date.Equals(avaliacaoMensal.dataInicio.Date))
                {
                    //inicio instantaneo da avaliação 
                    AcoesSistema acoes = new AcoesSistema(_context);

                    //metodo que faz todo o processo necessario para se inciar uma avaliação.
                    //arquivo (Controllers > SistemaAutomatizado > *AcoesSistema.cs*)
                    acoes.AcioneManualAvalicao(avaliacaoMensal.dataReferencia);

                }

                //persiste o id do usuario para atualização da página
                int idView = Convert.ToInt32(form["idUsuarioAcesso"]); ;

                //Objeto para passar ids(inteiros) entre 'rotas/actions'
                IdPass idUsuario = new IdPass();
                idUsuario.id = idView;

                //Retorna para a pagina principal do sistema
                return RedirectToAction("PaginaPrincipal", "Home", idUsuario);
            }

            //Retorna informações de válidação
            return View(avaliacaoMensal);
        }

        [NoDirectAccess]
        //Metodo de validação de campo na view/Model [Remote]
        public IActionResult ValidarExistenciaAvaliacaoMensal(DateTime dataReferencia) 
        {
            var verificarExistencia = _context.AvaliacaoMensal.AsQueryable();
            verificarExistencia = verificarExistencia.Where(n => n.dataReferencia.Date.Month.Equals(dataReferencia.Date.Month) && n.dataReferencia.Date.Year.Equals(dataReferencia.Date.Year));

            if (verificarExistencia.FirstOrDefault() == null)
            {
                return Json(true);
            }

            return Json($"Já existe uma avalição marcada para este mês.");

        }

        [NoDirectAccess]
        //Metodo de validação de campo na view/Model [Remote]
        public IActionResult ValidarDataInicioAvaliacaoMensal(DateTime dataInicio, DateTime dataReferencia)
        {
            
            if (dataInicio.Date.CompareTo(dataReferencia.Date) >= 0)
            {
                return Json(true);
            }

            return Json($"A data de inicio não poder ser menor que a da avaliação.");

        }

        [NoDirectAccess]
        // GET: AvaliacaoMensais/Edit/5
        public async Task<IActionResult> Edit(int? id, string idUsuario)
        {
            ViewBag.idUsuario = Convert.ToInt32(idUsuario);
            if (id == null)
            {
                return NotFound();
            }

            var avaliacaoMensal = await _context.AvaliacaoMensal.FindAsync(id);
            if (avaliacaoMensal == null)
            {
                return NotFound();
            }
            return View(avaliacaoMensal);
        }

        // POST: AvaliacaoMensais/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,dataReferencia,dataInicio,diasExpirar,encerrado")] AvaliacaoMensal avaliacaoMensal, IFormCollection form)
        {
            if (id != avaliacaoMensal.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                int idView = Convert.ToInt32(form["idUsuarioAcesso"]);

                IdPass idUsuario = new IdPass();
                idUsuario.id = idView;

                try
                {
                    _context.Update(avaliacaoMensal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AvaliacaoMensalExists(avaliacaoMensal.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("PaginaPrincipal", "Home", idUsuario);
            }
            return View(avaliacaoMensal);
        }

        [NoDirectAccess]
        // GET: AvaliacaoMensais/Delete/5
        //Traz view para exclusao de Avaliação
        public async Task<IActionResult> Delete(int? id, string idUsuario)
        {
            ViewBag.idUsuario = Convert.ToInt32(idUsuario);
            if (id == null)
            {
                return NotFound();
            }

            var avaliacaoMensal = await _context.AvaliacaoMensal
                .FirstOrDefaultAsync(m => m.id == id);
            if (avaliacaoMensal == null)
            {
                return NotFound();
            }

            return View(avaliacaoMensal);
        }

        // POST: AvaliacaoMensais/Delete/5
        //Excluir Avaliação
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, IFormCollection form)
        {
            int idView = Convert.ToInt32(form["idUsuarioAcesso"]); ;

            IdPass idUsuario = new IdPass();
            idUsuario.id = idView;

            var avaliacaoMensal = await _context.AvaliacaoMensal.FindAsync(id);

            List<Avaliacao> avaliacoes = await _context.Avaliacao.Where(a => a.idAvaliacao.Equals(avaliacaoMensal.id)).ToListAsync();
            List<LinkAvalicao> linksAvaliacao = await _context.LinkAvalicao.Where(a => a.idAvaliacao.Equals(avaliacaoMensal.id)).ToListAsync();

            if(avaliacoes.Count > 0)
            {
                foreach (var avaliacao in avaliacoes)
                {
                    _context.Avaliacao.Remove(avaliacao);
                }
            }

            if (linksAvaliacao.Count > 0)
            {
                foreach (var links in linksAvaliacao)
                {
                    _context.LinkAvalicao.Remove(links);
                }
            }

            _context.AvaliacaoMensal.Remove(avaliacaoMensal);

            await _context.SaveChangesAsync();

            return RedirectToAction("PaginaPrincipal", "Home", idUsuario);
        }

        [NoDirectAccess]
        //Metodo utilizado pelo Ajax, para trazer a pagina anterior persistindo o id do usuário
        public async Task<IActionResult> RetornarPaginaPrincipal(string idUsuario)
        {

            int id = Convert.ToInt32(idUsuario);

            var user = await _context.Usuario.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return RedirectToAction("Index", user);
        }

        private bool AvaliacaoMensalExists(int id)
        {
            return _context.AvaliacaoMensal.Any(e => e.id == id);
        }
    }
}
