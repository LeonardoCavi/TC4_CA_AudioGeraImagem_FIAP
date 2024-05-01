using AudioGeraImagem.Domain.Messages;

namespace AudioGeraImagemWorker.Domain.Interfaces
{
    public interface IComandoManager
    {
        Task ProcessarComando(ComandoMessage mensagem);
        Task ReprocessarComando(RetentativaComandoMessage mensagem);
    }
}