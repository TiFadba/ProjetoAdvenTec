    using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoAdvenTec.Models
{
    [Table("avaliacoes")]
    public class Avaliacao
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Nome do Cliente")]
        public int idCliente { get; set; }

        [Display(Name = "ID da Avaliação")]
        public int idAvaliacao { get; set; }

        [Display(Name = "Mes/Ano da Avaliação")]
        [Required(ErrorMessage = "Campo obrigatório!")]
        public DateTime dataReferencia { get; set; }

        [Display(Name = "Nota Atribuída")]
        [Required(ErrorMessage = "Campo obrigatório!")]
        public int nota { get; set; }

        
        [Remote(action: "ChecarRazao", controller: "Avaliacoes")]
        [Display(Name = "Motivo da Nota")]
        [Required(ErrorMessage = "Campo obrigatório!")]
        public string razaoNota { get; set; }
    }
}
