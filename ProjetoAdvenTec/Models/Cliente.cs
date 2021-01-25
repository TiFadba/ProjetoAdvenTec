using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoAdvenTec.Models
{
    [Table("clientes")]
    public class Cliente
    {
        [Key]
        public int id { get; set; }

        [Remote(action: "ChecarNomeEmpresa", controller: "Clientes")]
        [Display (Name ="Nome da Empresa")]
        [Required(ErrorMessage = "Campo obrigatório!")]
        [MaxLength(130)]
        public string nomeCliente { get; set; }

        [Display(Name = "Nome do(a) Responável")]
        [Required(ErrorMessage = "Campo obrigatório!")]
        [MaxLength(130)]
        public string nomeResponsavel { get; set; }

        [Remote(action: "ChecarCnpjEmpresa", controller: "Clientes")]
        [Display(Name = "CNPJ")]
        [MaxLength(14, ErrorMessage = "Número máximo de caracteres atingido.")]
        public string cnpj { get; set; }

        [Display(Name = "Categoria")]
        public string categoria { get; set; }

        [Remote(action: "VerificarUsoEmail", controller: "Clientes")]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Campo obrigatório!")]
        [EmailAddress(ErrorMessage = "Email inválido!")]
        [MaxLength(130)]
        public string email { get; set; }

        [Display(Name = "Data de Entrada")]
        public DateTime dataIngressao { get; set; }

        [Display(Name = "Data da Última Avalicão")]
        public DateTime ultimaAvaliacao { get; set; }
    }
}
