using AudioGeraImagem.Domain.Messages;

namespace AudioGeraImagemWorker.Application.Interfaces
{
    public interface IEventReceiver
    {
        Task ReceberMensagem(ComandoMessage mensagem);
        Task ReceberRetentativa(RetentativaComandoMessage mensagem);
    }
}