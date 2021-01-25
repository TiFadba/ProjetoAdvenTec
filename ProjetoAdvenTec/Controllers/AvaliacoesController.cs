using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetoAdvenTec.Models;
using ProjetoAdvenTec.Models.DataBase;
using static ProjetoAdvenTec.Services.BloquearAcessoDireto;

namespace ProjetoAdvenTec.Controllers
{

    /*Class sem relevancia direta ao projeto, fazemos uso apenas de algumas chamadas para informações destinadas a view*/

    public class AvaliacoesController : Controller
    {
        private readonly DataContext _context;

        public AvaliacoesController(DataContext context)
        {
            _context = context;
        }

        [NoDirectAccess]
        // GET: Avaliacoes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Avaliacao.ToListAsync());
        }

        //Metodo que recebe e valida os links que sao enviados por email para a avaliação
        public ActionResult AvaliacaoMensal(string linkAcessoAvaliacao)
        {
            var validarLink = _context.LinkAvalicao.AsQueryable();
            validarLink = validarLink.Where(l => l.linkAvaliacaoEmail.Equals(linkAcessoAvaliacao));

            if(validarLink.FirstOrDefault() != null)
            {
                ViewBag.idCliente = validarLink.FirstOrDefault().idCliente;
                ViewBag.idAvaliacao = validarLink.FirstOrDefault().idAvaliacao;
                return View();
            }

            //Avisa que o link esta indisponivel
            return View("AvaliacaoConcluida");
        }

        [NoDirectAccess]
        //Metodo para agradescer os que avaliaram 
        public ActionResult Obrigado(string linkAcessoAvaliacao)
        {
          
            return View("Obrigado");
        }

        [NoDirectAccess]
        public ActionResult QuestionarioAvaliativo(int idCliente, int idAvaliacao)
        {
            ViewBag.idCliente = idCliente;
            ViewBag.idAvaliacao = idAvaliacao;
            return View();
        }

        [NoDirectAccess]
        //Valida o campo 'razaoNota na view' usando [Remote] no model
        public IActionResult ChecarRazao(string razaoNota)
        {

            if (!String.IsNullOrEmpty(razaoNota))
            {
                return Json(true);
            }

            return Json($"Este campo é obrigatório!");

        }

        // GET: Avaliacoes/Details/5

        [NoDirectAccess]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var avaliacao = await _context.Avaliacao
                .FirstOrDefaultAsync(m => m.id == id);
            if (avaliacao == null)
            {
                return NotFound();
            }

            return View(avaliacao);
        }

        [NoDirectAccess]
        // GET: Avaliacoes/Create
        public IActionResult Create(int idCliente, int idAvaliacao, int nota)
        {

            var dataReferencia = _context.AvaliacaoMensal.Find(idAvaliacao).dataReferencia;

            if(dataReferencia!= null)
            {
                ViewBag.dataFormatada = dataReferencia.ToString("MM/yyyy");
                ViewBag.dataReferencia = dataReferencia;
            }

            ViewBag.idCliente = idCliente;
            ViewBag.idAvaliacao = idAvaliacao;
            ViewBag.nota = nota;

            return View();
        }

        // POST: Avaliacoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,idCliente,idAvaliacao,dataReferencia,nota,razaoNota")] Avaliacao avaliacao)
        {
            if (ModelState.IsValid)
            {
                var linkUsado = await _context.LinkAvalicao.FirstOrDefaultAsync(l => l.idCliente.Equals(avaliacao.idCliente) && l.idAvaliacao.Equals(avaliacao.idAvaliacao));
                var verificarExistenciaAvaliacao = await _context.Avaliacao.FirstOrDefaultAsync(a => a.idCliente.Equals(avaliacao.idCliente) && a.idAvaliacao.Equals(avaliacao.idAvaliacao));
                
                if (linkUsado != null)
                {
                    _context.LinkAvalicao.Remove(linkUsado);
                }

                if (verificarExistenciaAvaliacao == null)
                {
                    _context.Add(avaliacao);
                    await AdicinarCategoriaCliente(avaliacao.idCliente, avaliacao.nota, avaliacao.dataReferencia); // Adiciona categoria ao avaliador
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Obrigado");
            }

            return View(avaliacao);
        }

        [NoDirectAccess]
        //metodo para categorizar os clientes apartir de sua ultima avaliação
        public async Task<bool> AdicinarCategoriaCliente(int idCliente, int nota, DateTime referencia)
        {
            var cliente = await _context.Cliente.FirstOrDefaultAsync(a => a.id.Equals(idCliente));

            //classificação
            if(cliente != null)
            {
                if (nota <= 6)
                {
                    cliente.categoria = "Detrator";
                }
                else if (nota <= 8)
                {
                    cliente.categoria = "Neutro";
                }
                else
                {
                    cliente.categoria = "Promotor";
                }

                //atualização
                 cliente.ultimaAvaliacao = referencia;
                _context.Cliente.Update(cliente);

                return true;
            }

            return false;

        }

        [NoDirectAccess]
        // GET: Avaliacoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var avaliacao = await _context.Avaliacao.FindAsync(id);
            if (avaliacao == null)
            {
                return NotFound();
            }
            return View(avaliacao);
        }

        // POST: Avaliacoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,idCliente,idAvaliacao,dataReferencia,nota,razaoNota")] Avaliacao avaliacao)
        {
            if (id != avaliacao.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(avaliacao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AvaliacaoExists(avaliacao.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(avaliacao);
        }

        [NoDirectAccess]
        // GET: Avaliacoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var avaliacao = await _context.Avaliacao
                .FirstOrDefaultAsync(m => m.id == id);
            if (avaliacao == null)
            {
                return NotFound();
            }

            return View(avaliacao);
        }

        // POST: Avaliacoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var avaliacao = await _context.Avaliacao.FindAsync(id);
            _context.Avaliacao.Remove(avaliacao);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AvaliacaoExists(int id)
        {
            return _context.Avaliacao.Any(e => e.id == id);
        }
    }
}
