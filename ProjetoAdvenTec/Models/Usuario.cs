using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoAdvenTec.Models
{
    [Table("usuarios")]
    public class Usuario
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public string nome { get; set; }

        //[Remote("", "")]
        [Display(Name = "Login")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public string nomeLogin { get; set; }

        [Display(Name = "Senha")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public string senha { get; set; }

        [Display(Name = "Usuário Ativo")]
        public bool primeiroAcesso { get; set; }

        [Display(Name = "Administrador")]
        public bool administrador { get; set; }

        [Display(Name = "Adicionar Usuários")]
        public bool adicionarUsuario { get; set; }

        [Display(Name = "Administrar Usuários")]
        public bool adminUsuarios { get; set; }

        [Display(Name = "Adicionar Clientes")]
        public bool adicionarCliente { get; set; }

        [Display(Name = "Administrar Clientes")]
        public bool adminClientes { get; set; }

        [Display(Name = "Adicionar Avalição Mensal")]
        public bool adicionarAvaliacao { get; set; }

        [Display(Name = "Administrar Avaliações Mensais")]
        public bool adminAvaliacoes { get; set; }

        [Display(Name = "Conceder Permissões")]
        public bool concederPermissoes { get; set; }

        

    }
}
