using AudioGeraImagem.Domain.Messages;
using AudioGeraImagemWorker.UseCases.Criacoes.Processar;
using AudioGeraImagemWorker.Worker.Events;
using AutoFixture;
using MassTransit;
using MediatR;
using NSubstitute;
using System.Text;

namespace AudioGeraImagemWorker.Test.Unitario.AudioGeraImagemWorker.Worker.Teste.Events
{
    public class NovaCriacaoConsumerTest
    {
        private readonly IFixture _fixture;
        private readonly IMediator _mediatorMock;
        private readonly NovaCriacaoConsumer consumer;

        public NovaCriacaoConsumerTest()
        {
            _fixture = new Fixture();

            _mediatorMock = Substitute.For<IMediator>();

            _fixture.Register(() => _mediatorMock);

            consumer = _fixture.Create<NovaCriacaoConsumer>();
        }

        [Fact]
        public async Task Consume_Success()
        {
            //Arrange
            var criacaoMessageMock = _fixture.Create<CriacaoMessage>();

            var consumeContextMock = Substitute.For<ConsumeContext<CriacaoMessage>>();
            consumeContextMock.Message.Returns(criacaoMessageMock);

            //Act
            await consumer.Consume(consumeContextMock);

            //Assert
            await _mediatorMock.Received().Send(Arg.Is<ProcessarCriacaoCommand>(x =>
                x.CriacaoId == criacaoMessageMock.CriacaoId &&
                !x.Retentativa));
        }
    }
}
