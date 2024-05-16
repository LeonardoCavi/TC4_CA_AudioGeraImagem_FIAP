using AudioGeraImagem.Domain.Messages;
using AudioGeraImagemWorker.Domain.Enums;
using AudioGeraImagemWorker.UseCases.Criacoes.Processar;
using AudioGeraImagemWorker.Worker.Events;
using AutoFixture;
using MassTransit;
using MediatR;
using NSubstitute;
using System.Text;

namespace AudioGeraImagemWorker.Test.Unitario.AudioGeraImagemWorker.Worker.Teste.Events
{
    public class RetentativaCriacaoConsumerTest
    {
        private readonly IFixture _fixture;
        private readonly IMediator _mediatorMock;
        private readonly RetentativaCriacaoConsumer consumer;

        public RetentativaCriacaoConsumerTest()
        {
            _fixture = new Fixture();

            _mediatorMock = Substitute.For<IMediator>();

            _fixture.Register(() => _mediatorMock);

            consumer = _fixture.Create<RetentativaCriacaoConsumer>();
        }

        [Fact]
        public async Task Consume_Success()
        {
            //Arrange
            var retentativaCriacaoMessageMock = _fixture.Create<RetentativaCriacaoMessage>();

            var consumeContextMock = Substitute.For<ConsumeContext<RetentativaCriacaoMessage>>();
            consumeContextMock.Message.Returns(retentativaCriacaoMessageMock);

            //Act
            await consumer.Consume(consumeContextMock);

            //Assert
            await _mediatorMock.Received().Send(Arg.Is<ProcessarCriacaoCommand>(x =>
                x.CriacaoId == retentativaCriacaoMessageMock.CriacaoId &&
                x.Payload == retentativaCriacaoMessageMock.Payload &&
                x.Retentativa));
        }
    }
}
