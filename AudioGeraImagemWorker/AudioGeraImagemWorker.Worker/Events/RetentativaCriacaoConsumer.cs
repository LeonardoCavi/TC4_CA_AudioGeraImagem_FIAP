using AudioGeraImagem.Domain.Messages;
using AudioGeraImagemWorker.UseCases.Criacoes.Processar;
using MassTransit;
using MediatR;

namespace AudioGeraImagemWorker.Worker.Events
{
    public class RetentativaCriacaoConsumer : IConsumer<RetentativaCriacaoMessage>
    {
        private readonly IMediator _mediator;

        public RetentativaCriacaoConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<RetentativaCriacaoMessage> context)
        {
            var mensagem = context.Message;
            await _mediator.Send(new ProcessarCriacaoCommand(mensagem.CriacaoId, mensagem.Payload, mensagem.UltimoEstado));
        }
    }
}
