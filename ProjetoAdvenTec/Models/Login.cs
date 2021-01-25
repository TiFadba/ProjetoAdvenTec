using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoAdvenTec.Models
{
    public class Login
    {
        [Display(Name = "Login")]
        [Required(ErrorMessage = "Campo obrigatório!")]
        public string loginUsuario { get; set; }

        [Display(Name = "Senha")]
        [Required(ErrorMessage = "Campo obrigatório!")]
        public string senha { get; set; }

    }
}
