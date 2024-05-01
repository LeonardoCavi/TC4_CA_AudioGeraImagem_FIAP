using AudioGeraImagem.Domain.Messages;
using AudioGeraImagemWorker.Application.Interfaces;
using MassTransit;

namespace AudioGeraImagemWorker.Worker.Events
{
    public class NovoComandoConsumer : IConsumer<ComandoMessage>
    {
        private readonly IEventReceiver _eventReceiver;

        public NovoComandoConsumer(IEventReceiver eventReceiver)
        {
            _eventReceiver = eventReceiver;
        }

        public async Task Consume(ConsumeContext<ComandoMessage> context)
        {
            var mensagem = context.Message;
            await _eventReceiver.ReceberMensagem(mensagem);
        }
    }
}