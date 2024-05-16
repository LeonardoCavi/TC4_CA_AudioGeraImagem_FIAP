using AudioGeraImagem.Domain.Messages;
using AudioGeraImagemWorker.UseCases.Criacoes.Processar;
using MassTransit;
using MediatR;

namespace AudioGeraImagemWorker.Worker.Events
{
    public class NovaCriacaoConsumer : IConsumer<CriacaoMessage>
    {
        private readonly IMediator _mediator;

        public NovaCriacaoConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<CriacaoMessage> context)
        {
            var mensagem = context.Message;
            await _mediator.Send(new ProcessarCriacaoCommand(mensagem.CriacaoId, mensagem.Payload));
        }
    }
}