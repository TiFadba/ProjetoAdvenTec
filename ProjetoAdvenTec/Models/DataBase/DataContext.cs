using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using ProjetoAdvenTec.Models;

namespace ProjetoAdvenTec.Models.DataBase
{
    /*Classe de conexão com o banco de dados*/

    public class DataContext : DbContext
    {

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<ProjetoAdvenTec.Models.Cliente> Cliente { get; set; }

        public DbSet<ProjetoAdvenTec.Models.AvaliacaoMensal> AvaliacaoMensal { get; set; }

        public DbSet<ProjetoAdvenTec.Models.Avaliacao> Avaliacao { get; set; }

        public DbSet<ProjetoAdvenTec.Models.LinkAvalicao> LinkAvalicao { get; set; }

        public DbSet<ProjetoAdvenTec.Models.Usuario> Usuario { get; set; }
    }
}
