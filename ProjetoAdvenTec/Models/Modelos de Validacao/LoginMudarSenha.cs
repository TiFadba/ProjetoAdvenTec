using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoAdvenTec.Models.Modelos_de_Validacao
{
    public class LoginMudarSenha
    {
        public int idUsuario { get; set; }

        [Remote(action: "ChecarNomeEmpresa", controller: "Clientes")]
        [Display(Name = "Nova Senha")]
        [Required(ErrorMessage = "Campo obrigatório!")]
        public string senha { get; set; }

        [Display(Name = "Confirmar Senha")]
        [Required(ErrorMessage = "Campo obrigatório!")]
        public string confirmarSenha { get; set; }
    }
}
