using AudioGeraImagem.Domain.Messages;
using AudioGeraImagemWorker.UseCases.Comandos.Processar;
using MassTransit;
using MediatR;

namespace AudioGeraImagemWorker.Worker.Events
{
    public class NovoComandoConsumer : IConsumer<ComandoMessage>
    {
        private readonly IMediator _mediator;

        public NovoComandoConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<ComandoMessage> context)
        {
            var mensagem = context.Message;
            await _mediator.Send(new ProcessarComandoCommand(mensagem.ComandoId, mensagem.Payload));
        }
    }
}