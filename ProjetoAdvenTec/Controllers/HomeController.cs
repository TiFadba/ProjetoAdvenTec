using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjetoAdvenTec.Models;
using ProjetoAdvenTec.Models.DataBase;
using ProjetoAdvenTec.Models.Modelos_de_Validacao;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static ProjetoAdvenTec.Services.BloquearAcessoDireto;

namespace ProjetoAdvenTec.Controllers
{
    /*O metodo que incia o sistema está aqui*/

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext _context;
        public HomeController(ILogger<HomeController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [NoDirectAccess]
        public IActionResult Index()
        {
            return View();
        }

        //Pagina Inicial do Sistema

        [NoDirectAccess]
        public async Task<IActionResult> PaginaPrincipal(IdPass idUsuario)
        {
            Usuario user = await _context.Usuario.FindAsync(idUsuario.id);

            return View(user);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
