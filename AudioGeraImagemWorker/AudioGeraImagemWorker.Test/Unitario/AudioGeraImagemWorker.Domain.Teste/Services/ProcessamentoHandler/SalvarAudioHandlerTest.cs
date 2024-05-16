using AudioGeraImagem.Domain.Entities;
using AudioGeraImagemWorker.Domain.DTOs;
using AudioGeraImagemWorker.Domain.Enums;
using AudioGeraImagemWorker.Domain.Interfaces.Repositories;
using AudioGeraImagemWorker.Domain.Interfaces.Services;
using AudioGeraImagemWorker.Domain.Interfaces.Vendor;
using AudioGeraImagemWorker.Domain.Services.ProcessamentoHandler;
using AutoFixture;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Text;

namespace AudioGeraImagemWorker.Test.Unitario.AudioGeraImagemWorker.Domain.Teste.Services.ProcessamentoHandler
{
    public class SalvarAudioHandlerTest
    {
        private readonly IFixture _fixture;
        private readonly ILogger<SalvarAudioHandler> _loggerMock;
        private readonly IBucketManager _bucketManagerMock;
        private readonly ICriacaoRepository _criacaoRepositoryMock;

        public SalvarAudioHandlerTest()
        {
            _fixture = new Fixture();

            _loggerMock = Substitute.For<ILogger<SalvarAudioHandler>>();
            _bucketManagerMock = Substitute.For<IBucketManager>();
            _criacaoRepositoryMock = Substitute.For<ICriacaoRepository>();

            _fixture.Register(() => _loggerMock);
            _fixture.Register(() => _bucketManagerMock);
            _fixture.Register(() => _criacaoRepositoryMock);
        }

        [Fact]
        public async Task ExecutarEtapa_Sucesso()
        {
            //Arrange
            var criacao = _fixture.Create<Criacao>();
            criacao.ProcessamentosCriacao = new()
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
                    InstanteCriacao = DateTime.Now.AddSeconds(30),
                    MensagemErro = string.Empty
                }
            };

            var payload = Encoding.UTF8.GetBytes("buffer teste");

            var comando = new Comando(criacao, payload);

            _bucketManagerMock.ArmazenarObjeto(Arg.Is(payload), Arg.Any<string>())
                .Returns("url_bucket_audio");


            var handler = _fixture.Create<SalvarAudioHandler>();

            //Act
            var resultado = await handler.ExecutarEtapa(comando);

            //Assert
            Assert.Equal("url_bucket_audio", resultado.Criacao.UrlAudio);
            Assert.Equal(EstadoProcessamento.GerandoTexto, resultado.Criacao.ProcessamentosCriacao.Last().Estado);
        }

        [Fact]
        public async Task ExecutarEtapa_Exception()
        {
            //Arrange
            var criacao = _fixture.Create<Criacao>();
            criacao.ProcessamentosCriacao = new()
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
                    InstanteCriacao = DateTime.Now.AddSeconds(30),
                    MensagemErro = string.Empty
                }
            };

            var payload = Encoding.UTF8.GetBytes("buffer teste");

            var comando = new Comando(criacao, payload);

            _bucketManagerMock.ArmazenarObjeto(Arg.Is(payload), Arg.Any<string>())
                .Throws(new Exception("erro bucket"));

            var handler = _fixture.Create<SalvarAudioHandler>();

            //Act
            var exception = await Assert.ThrowsAsync<Exception>(() => handler.ExecutarEtapa(comando));

            //Assert
            Assert.Equal("erro bucket", exception.Message);
        }

        [Fact]
        public async Task ExecutarEtapa_NaoPodeExecutar()
        {
            //Arrange
            var criacao = _fixture.Create<Criacao>();
            criacao.ProcessamentosCriacao = new()
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Estado = EstadoProcessamento.Recebido,
                    InstanteCriacao = DateTime.Now,
                    MensagemErro = string.Empty
                }
            };

            var payload = Encoding.UTF8.GetBytes("buffer teste");

            var comando = new Comando(criacao, payload);

            var handler = _fixture.Create<SalvarAudioHandler>();

            //Act
            await handler.ExecutarEtapa(comando);

            //Assert
            await _bucketManagerMock.DidNotReceive().ArmazenarObjeto(Arg.Any<byte[]>(), Arg.Any<string>());
        }

        [Fact]
        public void ProximaEtapa()
        {
            //Arrange
            var processamentoHandler = Substitute.For<IProcessamentoHandler>();
            var handler = _fixture.Create<SalvarAudioHandler>();

            //Act
            var resultado = handler.ProximaEtapa(processamentoHandler);

            //Assert
            Assert.Equal(processamentoHandler, resultado);
        }

        [Fact]
        public async Task ProximaEtapa_ExecutarEtapa()
        {
            //Arrange
            var criacao = _fixture.Create<Criacao>();
            criacao.ProcessamentosCriacao = new()
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
                    InstanteCriacao = DateTime.Now.AddSeconds(30),
                    MensagemErro = string.Empty
                }
            };

            var payload = Encoding.UTF8.GetBytes("buffer teste");
            var comando = new Comando(criacao, payload);

            var comandoMock = new Comando(new Criacao()
            {
                Id = criacao.Id,
                Descricao = criacao.Descricao,
                UrlAudio = criacao.UrlAudio,
                Transcricao = "texto_transcricao",
                UrlImagem = criacao.UrlImagem,
                InstanteCriacao = criacao.InstanteCriacao,
                InstanteAtualizacao = DateTime.Now
            }, payload);
            comandoMock.Criacao.ProcessamentosCriacao = new()
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
                    InstanteCriacao = DateTime.Now.AddSeconds(30),
                    MensagemErro = string.Empty
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Estado = EstadoProcessamento.GerandoTexto,
                    InstanteCriacao = DateTime.Now.AddSeconds(30),
                    MensagemErro = string.Empty
                }
            };

            _bucketManagerMock.ArmazenarObjeto(Arg.Is(payload), Arg.Any<string>())
                .Returns("url_bucket_audio");

            var processamentoHandler = Substitute.For<IProcessamentoHandler>();
            processamentoHandler.ExecutarEtapa(Arg.Any<Comando>())
                .Returns(comandoMock);

            var handler = _fixture.Create<SalvarAudioHandler>();

            //Act
            handler.ProximaEtapa(processamentoHandler);
            var resultado = await handler.ExecutarEtapa(comando);

            //Assert
            Assert.Equal("texto_transcricao", resultado.Criacao.Transcricao);
        }
    }
}
