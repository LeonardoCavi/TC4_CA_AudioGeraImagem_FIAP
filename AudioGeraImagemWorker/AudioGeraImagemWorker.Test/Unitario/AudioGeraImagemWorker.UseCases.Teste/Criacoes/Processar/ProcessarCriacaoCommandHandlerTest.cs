using AudioGeraImagem.Domain.Entities;
using AudioGeraImagem.Domain.Messages;
using AudioGeraImagemWorker.Domain.DTOs;
using AudioGeraImagemWorker.Domain.Enums;
using AudioGeraImagemWorker.Domain.Interfaces.Repositories;
using AudioGeraImagemWorker.Domain.Interfaces.Services;
using AudioGeraImagemWorker.UseCases.Criacoes.Processar;
using AutoFixture;
using MassTransit;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioGeraImagemWorker.Test.Unitario.AudioGeraImagemWorker.UseCases.Teste.Criacoes.Processar
{
    public class ProcessarCriacaoCommandHandlerTest
    {
        private readonly IFixture _fixture;
        private readonly ILogger<ProcessarCriacaoCommandHandler> _loggerMock;
        private readonly ICriacaoRepository _criacaoRepositoryMock;
        private readonly IProcessamentoHandler _processamentoHandlerMock;
        private readonly IMessageScheduler _messageSchedulerMock;

        public ProcessarCriacaoCommandHandlerTest()
        {
            _fixture = new Fixture();

            _loggerMock = Substitute.For<ILogger<ProcessarCriacaoCommandHandler>>();
            _criacaoRepositoryMock = Substitute.For<ICriacaoRepository>();
            _processamentoHandlerMock = Substitute.For<IProcessamentoHandler>();
            _messageSchedulerMock = Substitute.For<IMessageScheduler>();

            _fixture.Register(() => _loggerMock);
            _fixture.Register(() => _criacaoRepositoryMock);
            _fixture.Register(() => _processamentoHandlerMock);
            _fixture.Register(() => _messageSchedulerMock);
        }

        [Fact]
        public async Task Handle_NovaCriacao_NaoEncontrado()
        {
            //Arrange
            var processarCriacaoCommand = _fixture.Create<ProcessarCriacaoCommand>();
            processarCriacaoCommand.UltimoEstado = EstadoProcessamento.Recebido;
            processarCriacaoCommand.Retentativa = false;

            var handler = _fixture.Create<ProcessarCriacaoCommandHandler>();

            //Act
            await handler.Handle(processarCriacaoCommand, default);

            //Assert
            await _criacaoRepositoryMock.DidNotReceive().Atualizar(Arg.Any<Criacao>());
        }

        [Fact]
        public async Task Handle_NovaCriacao_Successo()
        {
            //Arrange
            var processarCriacaoCommand = _fixture.Create<ProcessarCriacaoCommand>();
            processarCriacaoCommand.UltimoEstado = EstadoProcessamento.Recebido;
            processarCriacaoCommand.Retentativa = false;

            var criacaoMock = _fixture.Create<Criacao>();
            criacaoMock.Id = processarCriacaoCommand.CriacaoId;
            criacaoMock.ProcessamentosCriacao = new()
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Estado = EstadoProcessamento.Recebido,
                    InstanteCriacao = DateTime.Now,
                    MensagemErro = string.Empty
                }
            };

            _criacaoRepositoryMock.Obter(Arg.Is(processarCriacaoCommand.CriacaoId)).Returns(criacaoMock);

            var handler = _fixture.Create<ProcessarCriacaoCommandHandler>();

            //Act
            await handler.Handle(processarCriacaoCommand, default);

            //Assert
            await _processamentoHandlerMock.Received().ExecutarEtapa(Arg.Is<Comando>(x =>
                x.Criacao.Id == processarCriacaoCommand.CriacaoId &&
                x.Criacao.ProcessamentosCriacao.Count(x => x.Estado == EstadoProcessamento.SalvandoAudio) == 1 &&
                x.Criacao.ProcessamentosCriacao.Last().Estado == EstadoProcessamento.SalvandoAudio &&
                x.Payload == processarCriacaoCommand.Payload));
        }

        [Fact]
        public async Task Handle_NovaCriacao_TrataErro()
        {
            //Arrange
            var processarCriacaoCommand = _fixture.Create<ProcessarCriacaoCommand>();
            processarCriacaoCommand.UltimoEstado = EstadoProcessamento.Recebido;
            processarCriacaoCommand.Retentativa = false;

            var criacaoMock = _fixture.Create<Criacao>();
            criacaoMock.Id = processarCriacaoCommand.CriacaoId;
            criacaoMock.ProcessamentosCriacao = new()
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Estado = EstadoProcessamento.Recebido,
                    InstanteCriacao = DateTime.Now,
                    MensagemErro = string.Empty
                }
            };

            _criacaoRepositoryMock.Obter(Arg.Is(processarCriacaoCommand.CriacaoId)).Returns(criacaoMock);
            _processamentoHandlerMock.ExecutarEtapa(Arg.Any<Comando>()).Throws(new Exception("erro teste"));

            var handler = _fixture.Create<ProcessarCriacaoCommandHandler>();

            //Act
            await handler.Handle(processarCriacaoCommand, default);

            //Assert
            await _criacaoRepositoryMock.Received().Atualizar(Arg.Is<Criacao>(x =>
                x.Id == processarCriacaoCommand.CriacaoId &&
                x.ProcessamentosCriacao.Count(x => x.Estado == EstadoProcessamento.SalvandoAudio) == 1 &&
                x.ProcessamentosCriacao.Last().Estado == EstadoProcessamento.SalvandoAudio &&
                x.ProcessamentosCriacao.Last().MensagemErro == "erro teste"
                ));

            await _messageSchedulerMock.Received().SchedulePublish(Arg.Any<DateTime>(), Arg.Is<RetentativaCriacaoMessage>(x =>
                x.CriacaoId == processarCriacaoCommand.CriacaoId &&
                x.UltimoEstado == EstadoProcessamento.SalvandoAudio &&
                x.Payload == processarCriacaoCommand.Payload));
        }

        [Fact]
        public async Task Handle_RetentativaCriacao_Successo()
        {
            //Arrange
            var processarCriacaoCommand = _fixture.Create<ProcessarCriacaoCommand>();
            processarCriacaoCommand.UltimoEstado = EstadoProcessamento.SalvandoAudio;

            var criacaoMock = _fixture.Create<Criacao>();
            criacaoMock.Id = processarCriacaoCommand.CriacaoId;
            criacaoMock.ProcessamentosCriacao = new()
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Estado = EstadoProcessamento.Recebido,
                    InstanteCriacao = DateTime.Now,
                    MensagemErro = string.Empty
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Estado = EstadoProcessamento.SalvandoAudio,
                    InstanteCriacao = DateTime.Now,
                    MensagemErro = "erro teste"
                }
            };

            _criacaoRepositoryMock.Obter(Arg.Is(processarCriacaoCommand.CriacaoId)).Returns(criacaoMock);

            var handler = _fixture.Create<ProcessarCriacaoCommandHandler>();

            //Act
            await handler.Handle(processarCriacaoCommand, default);

            //Assert
            await _processamentoHandlerMock.Received().ExecutarEtapa(Arg.Is<Comando>(x =>
                x.Criacao.Id == processarCriacaoCommand.CriacaoId &&
                x.Criacao.ProcessamentosCriacao.Count(x => x.Estado == EstadoProcessamento.SalvandoAudio) == 2 &&
                x.Criacao.ProcessamentosCriacao.Last().Estado == EstadoProcessamento.SalvandoAudio &&
                x.Payload == processarCriacaoCommand.Payload));
        }

        [Fact]
        public async Task Handle_RetentativaCriacao_TrataErro()
        {
            //Arrange
            var processarCriacaoCommand = _fixture.Create<ProcessarCriacaoCommand>();
            processarCriacaoCommand.UltimoEstado = EstadoProcessamento.SalvandoAudio;

            var criacaoMock = _fixture.Create<Criacao>();
            criacaoMock.Id = processarCriacaoCommand.CriacaoId;
            criacaoMock.ProcessamentosCriacao = new()
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Estado = EstadoProcessamento.Recebido,
                    InstanteCriacao = DateTime.Now,
                    MensagemErro = string.Empty
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Estado = EstadoProcessamento.SalvandoAudio,
                    InstanteCriacao = DateTime.Now,
                    MensagemErro = "erro teste"
                }
            };

            _criacaoRepositoryMock.Obter(Arg.Is(processarCriacaoCommand.CriacaoId)).Returns(criacaoMock);
            _processamentoHandlerMock.ExecutarEtapa(Arg.Any<Comando>()).Throws(new Exception("erro teste 2"));

            var handler = _fixture.Create<ProcessarCriacaoCommandHandler>();

            //Act
            await handler.Handle(processarCriacaoCommand, default);

            //Assert
            await _criacaoRepositoryMock.Received().Atualizar(Arg.Is<Criacao>(x =>
                x.Id == processarCriacaoCommand.CriacaoId &&
                x.ProcessamentosCriacao.Count(x => x.Estado == EstadoProcessamento.SalvandoAudio) == 2 &&
                x.ProcessamentosCriacao.Last().Estado == EstadoProcessamento.SalvandoAudio &&
                x.ProcessamentosCriacao.Last().MensagemErro == "erro teste 2"
                ));

            await _messageSchedulerMock.Received().SchedulePublish(Arg.Any<DateTime>(), Arg.Is<RetentativaCriacaoMessage>(x =>
                x.CriacaoId == processarCriacaoCommand.CriacaoId &&
                x.UltimoEstado == EstadoProcessamento.SalvandoAudio &&
                x.Payload == processarCriacaoCommand.Payload));
        }

        [Fact]
        public async Task Handle_RetentativaCriacao_TrataErro_Excesso()
        {
            //Arrange
            var processarCriacaoCommand = _fixture.Create<ProcessarCriacaoCommand>();
            processarCriacaoCommand.UltimoEstado = EstadoProcessamento.SalvandoAudio;

            var criacaoMock = _fixture.Create<Criacao>();
            criacaoMock.Id = processarCriacaoCommand.CriacaoId;
            criacaoMock.ProcessamentosCriacao = new()
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Estado = EstadoProcessamento.Recebido,
                    InstanteCriacao = DateTime.Now,
                    MensagemErro = string.Empty
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Estado = EstadoProcessamento.SalvandoAudio,
                    InstanteCriacao = DateTime.Now,
                    MensagemErro = "erro teste"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Estado = EstadoProcessamento.SalvandoAudio,
                    InstanteCriacao = DateTime.Now,
                    MensagemErro = "erro teste 2"
                }
            };

            _criacaoRepositoryMock.Obter(Arg.Is(processarCriacaoCommand.CriacaoId)).Returns(criacaoMock);
            _processamentoHandlerMock.ExecutarEtapa(Arg.Any<Comando>()).Throws(new Exception("erro teste 3"));

            var handler = _fixture.Create<ProcessarCriacaoCommandHandler>();

            //Act
            await handler.Handle(processarCriacaoCommand, default);

            //Assert
            await _criacaoRepositoryMock.Received().Atualizar(Arg.Is<Criacao>(x =>
                x.Id == processarCriacaoCommand.CriacaoId &&
                x.ProcessamentosCriacao.Count(x => x.Estado == EstadoProcessamento.SalvandoAudio) == 3 &&
                x.ProcessamentosCriacao.Last(x => x.Estado == EstadoProcessamento.SalvandoAudio).Estado == EstadoProcessamento.SalvandoAudio &&
                x.ProcessamentosCriacao.Last(x => x.Estado == EstadoProcessamento.SalvandoAudio).MensagemErro == "erro teste 3" &&
                x.ProcessamentosCriacao.Last().Estado == EstadoProcessamento.Falha));

            await _messageSchedulerMock.DidNotReceive().SchedulePublish(Arg.Any<DateTime>(), Arg.Any<RetentativaCriacaoMessage>());
        }
    }
}
