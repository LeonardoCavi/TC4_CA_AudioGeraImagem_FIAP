using AudioGeraImagemAPI.Domain.Utility.DTO;
using System.Diagnostics.CodeAnalysis;

namespace AudioGeraImagemAPI.Domain.Utility.Factory
{
    [ExcludeFromCodeCoverage]
    public static class ResultadoOperacaoFactory
    {
        public static ResultadoOperacao<T> Criar<T>(bool sucesso, string mensagemErro, T objeto)
        {
            return new(sucesso, mensagemErro, objeto);
        }
    }
}