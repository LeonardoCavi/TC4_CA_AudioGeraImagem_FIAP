using System.Diagnostics.CodeAnalysis;

namespace AudioGeraImagemAPI.Domain.Utility.DTO
{
    [ExcludeFromCodeCoverage]
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