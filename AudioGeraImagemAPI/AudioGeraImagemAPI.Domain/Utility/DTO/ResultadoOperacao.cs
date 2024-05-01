using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioGeraImagemAPI.Domain.Utility.DTO
{
    public class ResultadoOperacao<T>
    {
        public ResultadoOperacao(bool sucesso, string mensagemErro, T objeto)
        {
            Sucesso = sucesso;
            MensagemErro = mensagemErro;
            Objeto = objeto;
        }

        public bool Sucesso { get; set; }
        public string MensagemErro { get; set; }
        public T Objeto { get; set; }


    }
}
