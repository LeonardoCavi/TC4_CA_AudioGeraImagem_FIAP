using AudioGeraImagem.Domain.Messages;
using AudioGeraImagemWorker.UseCases.Comandos.Processar;
using MassTransit;
using MediatR;

namespace AudioGeraImagemWorker.Worker.Events
{
    public class RetentativaComandoConsumer : IConsumer<RetentativaComandoMessage>
    {
        private readonly IMediator _mediator;

        public RetentativaComandoConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<RetentativaComandoMessage> context)
        {
            var mensagem = context.Message;
            await _mediator.Send(new ProcessarComandoCommand(mensagem.ComandoId, mensagem.Payload, mensagem.UltimoEstado));
        }
    }
}
