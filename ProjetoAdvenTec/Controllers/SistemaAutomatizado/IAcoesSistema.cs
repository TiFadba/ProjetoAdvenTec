using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoAdvenTec.Controllers.SistemaAutomatizado
{
    //Interface que é usada para acessar os metodos da classe ao usar o HangFire no arquivo (Startup.cs)
    interface IAcoesSistema
    {
         void VerificarNovasAvalicoes();
         void VerificarAvalicoesProgresso();

    }
}
