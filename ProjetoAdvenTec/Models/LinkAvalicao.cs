using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoAdvenTec.Models
{
    [Table("linkAvaliacoes")]
    public class LinkAvalicao
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "ID do Cliente")]
        public int idCliente { get; set; }

        [Display(Name = "ID da Avaliação")]
        public int idAvaliacao { get; set; }

        [Display(Name = "Link de Acesso")]
        public string linkAvaliacaoEmail { get; set; }
    }
}
