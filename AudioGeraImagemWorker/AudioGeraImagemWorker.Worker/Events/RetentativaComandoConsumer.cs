using AudioGeraImagem.Domain.Messages;
using AudioGeraImagemWorker.Application.Interfaces;
using MassTransit;

namespace AudioGeraImagemWorker.Worker.Events
{
    public class RetentativaComandoConsumer : IConsumer<RetentativaComandoMessage>
    {
        private readonly IEventReceiver _eventReceiver;

        public RetentativaComandoConsumer(IEventReceiver eventReceiver)
        {
            _eventReceiver = eventReceiver;
        }

        public async Task Consume(ConsumeContext<RetentativaComandoMessage> context)
        {
            var mensagem = context.Message;

            await _eventReceiver.ReceberRetentativa(mensagem);
        }
    }
}
