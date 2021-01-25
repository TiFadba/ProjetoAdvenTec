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

    /*CRUD da entidade que persiste os dados relacionados ao link de avaliação por email*/

    /*Não é utilizada em potencial, mas futuramente quem sabe..*/

    public class LinkAvalicoesController : Controller
    {
        private readonly DataContext _context;

        public LinkAvalicoesController(DataContext context)
        {
            _context = context;
        }

        // GET: LinkAvalicoes
        public async Task<IActionResult> Index()
        {
            return View(await _context.LinkAvalicao.ToListAsync());
        }

        // GET: LinkAvalicoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var linkAvalicao = await _context.LinkAvalicao
                .FirstOrDefaultAsync(m => m.id == id);
            if (linkAvalicao == null)
            {
                return NotFound();
            }

            return View(linkAvalicao);
        }

        // GET: LinkAvalicoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LinkAvalicoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,idCliente,idAvaliacao,linkAvaliacaoEmail")] LinkAvalicao linkAvalicao)
        {
            if (ModelState.IsValid)
            {
                _context.Add(linkAvalicao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(linkAvalicao);
        }

        // GET: LinkAvalicoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var linkAvalicao = await _context.LinkAvalicao.FindAsync(id);
            if (linkAvalicao == null)
            {
                return NotFound();
            }
            return View(linkAvalicao);
        }

        // POST: LinkAvalicoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,idCliente,idAvaliacao,linkAvaliacaoEmail")] LinkAvalicao linkAvalicao)
        {
            if (id != linkAvalicao.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(linkAvalicao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LinkAvalicaoExists(linkAvalicao.id))
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
            return View(linkAvalicao);
        }

        // GET: LinkAvalicoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var linkAvalicao = await _context.LinkAvalicao
                .FirstOrDefaultAsync(m => m.id == id);
            if (linkAvalicao == null)
            {
                return NotFound();
            }

            return View(linkAvalicao);
        }

        // POST: LinkAvalicoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var linkAvalicao = await _context.LinkAvalicao.FindAsync(id);
            _context.LinkAvalicao.Remove(linkAvalicao);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LinkAvalicaoExists(int id)
        {
            return _context.LinkAvalicao.Any(e => e.id == id);
        }
    }
}
