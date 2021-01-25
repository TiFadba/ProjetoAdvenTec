using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoAdvenTec.Models
{
    [Table("avaliacoesMensais")]
    public class AvaliacaoMensal
    {
        [Key]
        public int id { get; set; }

        [Remote(action: "ValidarExistenciaAvaliacaoMensal", controller: "AvaliacaoMensais")]
        [Display(Name = "Mês a ser avaliado")]
        [Required(ErrorMessage = "Campo obrigatório!")]
        public DateTime dataReferencia { get; set; }

        [Remote(action: "ValidarDataInicioAvaliacaoMensal", controller: "AvaliacaoMensais", AdditionalFields = nameof(dataReferencia))]
        [Display(Name = "Data de inicio da avaliação")]
        [Required(ErrorMessage = "Campo obrigatório!")]
        public DateTime dataInicio { get; set; }

        [Display(Name = "Número de dias para expiar")]
        public int diasExpirar { get; set; }

        [Display(Name = "Status da Avaliação")]
        public bool encerrado { get; set; }

        [Display(Name = "Promotores")]
        public int promotores { get; set; }

        [Display(Name = "Neutros")]
        public int neutros { get; set; }

        [Display(Name = "Detratores")]
        public int detratores { get; set; }

        [Display(Name = "NPS")]
        public decimal nps { get; set; }
    }
}
