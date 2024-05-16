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
    public class GerarImagemHandlerTest
    {
        private readonly IFixture _fixture;
        private readonly ILogger<GerarImagemHandler> _loggerMock;
        private readonly IOpenAIVendor _openAIVendorMock;
        private readonly ICriacaoRepository _criacaoRepositoryMock;

        public GerarImagemHandlerTest()
        {
            _fixture = new Fixture();

            _loggerMock = Substitute.For<ILogger<GerarImagemHandler>>();
            _openAIVendorMock = Substitute.For<IOpenAIVendor>();
            _criacaoRepositoryMock = Substitute.For<ICriacaoRepository>();

            _fixture.Register(() => _loggerMock);
            _fixture.Register(() => _openAIVendorMock);
            _fixture.Register(() => _criacaoRepositoryMock);
        }

        [Fact]
        public async Task ExecutarEtapa_Sucesso()
        {
            //Arrange
            var comando = _fixture.Create<Comando>();
            comando.Criacao.ProcessamentosCriacao = new()
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
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Estado = EstadoProcessamento.GerandoImagem,
                    InstanteCriacao = DateTime.Now.AddSeconds(30),
                    MensagemErro = string.Empty
                }
            };

            _openAIVendorMock.GerarImagem(Arg.Is(comando.Criacao.Transcricao))
                .Returns("url_imagem");

            var handler = _fixture.Create<GerarImagemHandler>();

            //Act
            var resultado = await handler.ExecutarEtapa(comando);

            //Assert
            Assert.Equal("url_imagem", resultado.Criacao.UrlImagem);
            Assert.Equal(EstadoProcessamento.SalvadoImagem, resultado.Criacao.ProcessamentosCriacao.Last().Estado);
        }

        [Fact]
        public async Task ExecutarEtapa_Exception()
        {
            //Arrange
            var comando = _fixture.Create<Comando>();
            comando.Criacao.ProcessamentosCriacao = new()
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
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Estado = EstadoProcessamento.GerandoImagem,
                    InstanteCriacao = DateTime.Now.AddSeconds(30),
                    MensagemErro = string.Empty
                }
            };

            _openAIVendorMock.GerarImagem(Arg.Is(comando.Criacao.Transcricao))
                .Throws(new Exception("erro imagem"));

            var handler = _fixture.Create<GerarImagemHandler>();

            //Act
            var exception = await Assert.ThrowsAsync<Exception>(() => handler.ExecutarEtapa(comando));

            //Assert
            Assert.Equal("erro imagem", exception.Message);
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

            var payload = Encoding.UTF8.GetBytes("buffer teste");

            var comando = new Comando(criacao, payload);

            var handler = _fixture.Create<GerarImagemHandler>();

            //Act
            await handler.ExecutarEtapa(comando);

            //Assert
            await _openAIVendorMock.DidNotReceive().GerarImagem(Arg.Any<string>());
        }

        [Fact]
        public void ProximaEtapa()
        {
            //Arrange
            var processamentoHandler = Substitute.For<IProcessamentoHandler>();
            var handler = _fixture.Create<GerarImagemHandler>();

            //Act
            var resultado = handler.ProximaEtapa(processamentoHandler);

            //Assert
            Assert.Equal(processamentoHandler, resultado);
        }

        [Fact]
        public async Task ProximaEtapa_ExecutarEtapa()
        {
            //Arrange
            var comando = _fixture.Create<Comando>();
            comando.Criacao.ProcessamentosCriacao = new()
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

            var comandoMock = new Comando(new Criacao()
            {
                Id = comando.Criacao.Id,
                Descricao = comando.Criacao.Descricao,
                UrlAudio = comando.Criacao.UrlAudio,
                Transcricao = comando.Criacao.Transcricao,
                UrlImagem = "url_bucket_imagem",
                InstanteCriacao = comando.Criacao.InstanteCriacao,
                InstanteAtualizacao = DateTime.Now
            }, comando.Payload);
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
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Estado = EstadoProcessamento.GerandoImagem,
                    InstanteCriacao = DateTime.Now.AddSeconds(30),
                    MensagemErro = string.Empty
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Estado = EstadoProcessamento.SalvadoImagem,
                    InstanteCriacao = DateTime.Now.AddSeconds(30),
                    MensagemErro = string.Empty
                }
            };
            _openAIVendorMock.GerarImagem(Arg.Is(comando.Criacao.Transcricao))
                .Returns("url_imagem");

            var processamentoHandler = Substitute.For<IProcessamentoHandler>();
            processamentoHandler.ExecutarEtapa(Arg.Any<Comando>())
                .Returns(comandoMock);

            var handler = _fixture.Create<GerarImagemHandler>();

            //Act
            handler.ProximaEtapa(processamentoHandler);
            var resultado = await handler.ExecutarEtapa(comando);

            //Assert
            Assert.Equal("url_bucket_imagem", resultado.Criacao.UrlImagem);
        }
    }
}
