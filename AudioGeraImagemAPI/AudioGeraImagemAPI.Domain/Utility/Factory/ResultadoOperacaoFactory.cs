using AudioGeraImagemAPI.Domain.Utility.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioGeraImagemAPI.Domain.Utility.Factory
{
    public static class ResultadoOperacaoFactory
    {
        public static ResultadoOperacao<T> Criar<T>(bool sucesso, string mensagemErro, T objeto)
        {
            return new(sucesso, mensagemErro, objeto);
        }
    }
}
