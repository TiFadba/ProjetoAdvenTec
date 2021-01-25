using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetoAdvenTec.Models;
using ProjetoAdvenTec.Models.Modelos_de_Validacao;
using ProjetoAdvenTec.Models.DataBase;
using ProjetoAdvenTec.Services;
using Microsoft.AspNetCore.Http;
using static ProjetoAdvenTec.Services.BloquearAcessoDireto;

namespace ProjetoAdvenTec.Controllers
{
    /*CRUD dos Clientes e outros metodos*
     * As ViewBags são usadas para persistir o id do Usuario entre outras informações na view
     */

    public class ClientesController : Controller
    {
        private readonly DataContext _context;

        public ClientesController(DataContext context)
        {
            _context = context;
        }

        [NoDirectAccess]
        // GET: Clientes
        //Pagina inicial de administração dos clientes
        public IActionResult Index(Usuario user)
        {
            IdPass idUsuario = new IdPass();
            ViewBag.infoUsuario = user;
            idUsuario.id = user.id;
            return View(idUsuario);
        }

        [NoDirectAccess]
        //Traz os clientes encontrados apartir da info de pesquisa passada
        public async Task<IActionResult> ListarClientes(string nomePesquisado, string idUsuario)
        {

            Usuario user = _context.Usuario.FirstOrDefault(u => u.id.Equals(Convert.ToInt32(idUsuario)));
            ViewBag.infoUsuario = user;

            ViewBag.idUsuario = Convert.ToInt32(idUsuario);

            if (!String.IsNullOrEmpty(nomePesquisado))
            {
                //Busca/consulta no banco de dados
                var filtroNome = _context.Cliente.AsQueryable();
                filtroNome = filtroNome.Where(e => e.nomeCliente.Contains(nomePesquisado));

                if (filtroNome.Count() < 1)
                {
                    return View(null);
                }

                //retororna a lista com os clientes encontrados
                return View(await filtroNome.ToListAsync());
            }

            return View(null);
        }

        [NoDirectAccess]
        // GET: Clientes/Create
        public IActionResult Create(IdPass idUsuario)
        {
            ViewBag.idUsuario = idUsuario.id;
            return View();
        }

        // POST: Clientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,nomeCliente,nomeResponsavel,cnpj,categoria,email")] Cliente cliente, IFormCollection form)
        {
            cliente.categoria = "Nenhum"; // Um cliente novo recebe 'nunhum', por nao ter participado de nunhuma avaliação

            if (ModelState.IsValid)
            {
                cliente.id = 0;
                _context.Add(cliente);
                await _context.SaveChangesAsync();

                int idView = Convert.ToInt32(form["idUsuarioAcesso"]); ;

                IdPass idUsuario = new IdPass();
                idUsuario.id = idView;

                //Retorna para a pagina inicial
                return RedirectToAction("PaginaPrincipal","Home", idUsuario);
            }
            return View(cliente);
        }

        /*
         * Os Proximos metodos são exclusivos para validação de inforações no form de cadastro do cliente
         */
        [NoDirectAccess]
        public bool NomeVerificacao(string nomeEmpresaVerificacao)
        {
            var verificarExistencia = _context.Cliente.AsQueryable();
            verificarExistencia = verificarExistencia.Where(n => n.nomeCliente.Equals(nomeEmpresaVerificacao));

            if (verificarExistencia.FirstOrDefault() != null)
            {
                return false;
            }

            return true;
        }

        [NoDirectAccess]
        public bool CnpjVerificacao(string cnpjVerificacao)
        {
            var verificarExistencia = _context.Cliente.AsQueryable();
            verificarExistencia = verificarExistencia.Where(n => n.cnpj.Equals(cnpjVerificacao));

            if(verificarExistencia.FirstOrDefault() != null)
            {
                return false;
            }

            return true;
        }

        [NoDirectAccess]
        public bool ValidarExistenciaEmail(string email)
        {
            var verificarExistencia = _context.Cliente.AsQueryable();
            verificarExistencia = verificarExistencia.Where(n => n.email.Equals(email));

            if (verificarExistencia.FirstOrDefault() == null)
            {
                return true;
            }

            return false;
        }

        [NoDirectAccess]
        public IActionResult ChecarNomeEmpresa(string nomeCliente)
        {

            if (NomeVerificacao(nomeCliente))
            {
                return Json(true);
            }

            return Json($"O nome '{nomeCliente}' já está em uso.");

        }

        [NoDirectAccess]
        public IActionResult ChecarCnpjEmpresa(string cnpj)
        {
            if(cnpj.Length == 14)
            {
                if (Validacao.IsCnpj(cnpj))
                {

                    if (CnpjVerificacao(cnpj))
                    {
                        return Json(true);
                    }

                    return Json($"O CNPJ '{cnpj}' já está em uso.");

                }

            }

            return Json($"O CNPJ '{cnpj}' não é valido.");

        }

        [NoDirectAccess]
        public IActionResult VerificarUsoEmail(string email)
        {

            if (ValidarExistenciaEmail(email))
            {
                return Json(true);
            }

            return Json($"O email '{email}' já está em uso.");

        }

        /*--------------------------------------------------------------------------*/

        [NoDirectAccess]
        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(int? id, string idUsuario)
        {

            ViewBag.idUsuario = Convert.ToInt32(idUsuario);

            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Cliente.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        // POST: Clientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,nomeCliente,nomeResponsavel,cnpj,categoria,email")] Cliente cliente, IFormCollection form)
        {
            if (id != cliente.id)
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
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.id))
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
            return View(cliente);
        }

        [NoDirectAccess]
        //Retorna para a pagina principal persistindo o usuario que esta utilizando o sistema
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

        [NoDirectAccess]
        // GET: Clientes/Delete/5
        public async Task<IActionResult> Delete(int? id, string idUsuario)
        {
            ViewBag.idUsuario = Convert.ToInt32(idUsuario);

            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Cliente
                .FirstOrDefaultAsync(m => m.id == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, IFormCollection form)
        {
            int idView = Convert.ToInt32(form["idUsuarioAcesso"]); ;

            IdPass idUsuario = new IdPass();
            idUsuario.id = idView;

            var cliente = await _context.Cliente.FindAsync(id);
            _context.Cliente.Remove(cliente);
            await _context.SaveChangesAsync();
            return RedirectToAction("PaginaPrincipal", "Home", idUsuario);

        }

        [NoDirectAccess]
        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int? id, string idUsuario)
        {
            ViewBag.idUsuario = Convert.ToInt32(idUsuario);

            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Cliente
                .FirstOrDefaultAsync(m => m.id == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        private bool ClienteExists(int id)
        {
            return _context.Cliente.Any(e => e.id == id);
        }
    }
}
