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
using Microsoft.AspNetCore.Http;
using static ProjetoAdvenTec.Services.BloquearAcessoDireto;

namespace ProjetoAdvenTec.Controllers
{
    /*CRUD do usuario e validações*/

    public class UsuarioAcessoController : Controller
    {
        private readonly DataContext _context;

        public UsuarioAcessoController(DataContext context)
        {
            _context = context;
        }


        //Pagina Inicial do Usuario
        // GET: UsuarioAcesso
        public async Task<IActionResult> Index(Usuario user)
        {
            ViewBag.idUsuario = user.id;
            ViewBag.infoUsuario = user;

            //Traz a lista de usuarios
            //Consulta feita apartir de informacoes de permissão e hierarqui de acesso (ex. um usuario alterar dados do admin do sistema) 
            return View(await _context.Usuario.Where(c=> !c.id.Equals(user.id) && ((c.concederPermissoes.Equals(false) && c.administrador.Equals(true)) || c.administrador.Equals(false) )).ToListAsync());
        }

        //Traz a view do Login
        public IActionResult Logar()
        {
            return View(new Login());
        }

        //Recebe as credencias para validação do login e redirecionamento ao sistema 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logar([Bind("loginUsuario,senha")] Login login)
        {
            if (ModelState.IsValid)
            {
                //Verifica as credenciais conferem
                Usuario user = await _context.Usuario.FirstOrDefaultAsync(c => c.nomeLogin.Equals(login.loginUsuario) && c.senha.Equals(login.senha));

                if(user != null)
                {
                    //caso seja o primeiro acesso o usuario é redirecionado para a alteração da senha 
                    if (user.primeiroAcesso)
                    {
                        IdPass idUsuario = new IdPass();
                        idUsuario.id = user.id;
                        return RedirectToAction("AlterarSenha","UsuarioAcesso", idUsuario);
                    }
                    else
                    {
                        IdPass idUsuario = new IdPass()
                        {
                            id = user.id
                        };
                        

                        //Vai para a pagina principal do sistema
                        return RedirectToAction("PaginaPrincipal", "Home", idUsuario);
                    }

                }
                else
                {
                    //Avisa na view caso não haja usuario com as credenciais passadas
                    ViewBag.resultadoBusca = false;
                }

            }
            return View(login);
        }

        //Traz a view para a alteração da senha
        public IActionResult AlterarSenha(IdPass idUsuario)
        {
            ViewBag.senhasConferem = true;
            LoginMudarSenha novaSenha = new LoginMudarSenha();
            novaSenha.idUsuario = idUsuario.id;
            return View(novaSenha);
        }

        //Recebe os dados da alteração e valida-os e faz a mudança
        /*O sistema não valida de é a mesma senha anterior*/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AlterarSenha([Bind("idUsuario,senha,confirmarSenha")] LoginMudarSenha novoLogin)
        {
            if (ModelState.IsValid)
            {
                if (novoLogin.senha.Equals(novoLogin.confirmarSenha))
                {
                    Usuario user = await _context.Usuario.FirstOrDefaultAsync(u => u.id.Equals(novoLogin.idUsuario));

                    if (user != null)
                    {
                        user.senha = novoLogin.senha;
                        user.primeiroAcesso = false;

                        _context.Update(user);
                        await _context.SaveChangesAsync();

                        return RedirectToAction("PaginaPrincipal", "Home", user);


                    }
                }
                else
                {
                    //avisa que as senhas não são iguais
                    ViewBag.senhasConferem = false;
                    return View(novoLogin);
                }         

            }

            return View(novoLogin);
        }



        // GET: UsuarioAcesso/Details/5
        public async Task<IActionResult> Details(int? id, string idUsuario)
        {
            ViewBag.idUsuario = Convert.ToInt32(idUsuario);

            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(m => m.id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        //Traz a view de criação do usuario
        // GET: UsuarioAcesso/Create
        public IActionResult Create(string idUsuario)
        {
            ViewBag.idUsuario = Convert.ToInt32(idUsuario);
            Usuario user = _context.Usuario.Find(Convert.ToInt32(idUsuario));
            ViewBag.infoUsuario = user;
            return View();
        }

        //Salva usuario novo no banco
        // POST: UsuarioAcesso/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,nome,nomeLogin,senha,primeiroAcesso,administrador,adicionarUsuario,adminUsuarios,adicionarCliente,adminClientes,adicionarAvaliacao,adminAvaliacoes,concederPermissoes")] Usuario usuario, IFormCollection form)
        {
            if (ModelState.IsValid)
            {
                usuario.primeiroAcesso = true; 
                _context.Add(usuario);
                await _context.SaveChangesAsync();

                int idView = Convert.ToInt32(form["idUsuarioAcesso"]); ;

                IdPass idUsuario = new IdPass();
                idUsuario.id = idView;

                return RedirectToAction("PaginaPrincipal", "Home", idUsuario);

            }
            return View(usuario);
        }

        // GET: UsuarioAcesso/Edit/5
        public async Task<IActionResult> Edit(int? id, string idUsuario)
        {

            Usuario user = _context.Usuario.Find(Convert.ToInt32(idUsuario));
            ViewBag.infoUsuario = user;
            ViewBag.idUsuario = Convert.ToInt32(idUsuario);

            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // POST: UsuarioAcesso/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,nome,nomeLogin,senha,primeiroAcesso,administrador,adicionarUsuario,adminUsuarios,adicionarCliente,adminClientes,adicionarAvaliacao,adminAvaliacoes,concederPermissoes")] Usuario usuario, IFormCollection form)
        {
            if (id != usuario.id)
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
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.id))
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
            return View(usuario);
        }


        // GET: UsuarioAcesso/Delete/5
        public async Task<IActionResult> Delete(int? id, string idUsuario)
        {
            ViewBag.idUsuario = Convert.ToInt32(idUsuario);
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(m => m.id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: UsuarioAcesso/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, IFormCollection form)
        {
            int idView = Convert.ToInt32(form["idUsuarioAcesso"]); ;

            IdPass idUsuario = new IdPass();
            idUsuario.id = idView;

            var usuario = await _context.Usuario.FindAsync(id);
            _context.Usuario.Remove(usuario);
            await _context.SaveChangesAsync();

            return RedirectToAction("PaginaPrincipal", "Home", idUsuario);
        }

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

        private bool UsuarioExists(int id)
        {
            return _context.Usuario.Any(e => e.id == id);
        }
    }
}
