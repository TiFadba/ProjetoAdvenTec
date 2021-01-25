using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoAdvenTec.Models.Modelos_de_Validacao
{
    public class ClienteAvaliacao
    {
        public int idCliente {get;set;}
        public int idAvaliacao {get;set;}
        public string nomeCliente {get;set;}
        public int nota {get;set;}
        public string razaoNota {get;set;}
    }
}
