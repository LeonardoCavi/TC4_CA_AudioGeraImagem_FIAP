using AudioGeraImagem.Domain.Entities;
using AudioGeraImagemWorker.Domain.DTOs;
using AudioGeraImagemWorker.Domain.Enums;
using AudioGeraImagemWorker.Domain.Interfaces.Repositories;
using AudioGeraImagemWorker.Domain.Interfaces.Services;
using AudioGeraImagemWorker.Domain.Interfaces.Utility;
using AudioGeraImagemWorker.Domain.Interfaces.Vendor;
using AudioGeraImagemWorker.Domain.Services.ProcessamentoHandler;
using AutoFixture;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Text;

namespace AudioGeraImagemWorker.Test.Unitario.AudioGeraImagemWorker.Domain.Teste.Services.ProcessamentoHandler
{
    public class GerarTextoHandlerTest
    {
        private readonly IFixture _fixture;
        private readonly ILogger<GerarTextoHandler> _loggerMock;
        private readonly IOpenAIVendor _openAIVendorMock;
        private readonly IHttpHelper _httpHelperMock;
        private readonly ICriacaoRepository _criacaoRepositoryMock;

        public GerarTextoHandlerTest()
        {
            _fixture = new Fixture();

            _loggerMock = Substitute.For<ILogger<GerarTextoHandler>>();
            _openAIVendorMock = Substitute.For<IOpenAIVendor>();
            _httpHelperMock = Substitute.For<IHttpHelper>();
            _criacaoRepositoryMock = Substitute.For<ICriacaoRepository>();

            _fixture.Register(() => _loggerMock);
            _fixture.Register(() => _openAIVendorMock);
            _fixture.Register(() => _httpHelperMock);
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
                }
            };

            var bytesMock = Encoding.UTF8.GetBytes("buffer teste");

            _httpHelperMock.GetBytes(Arg.Is(comando.Criacao.UrlAudio))
                .Returns(bytesMock);

            _openAIVendorMock.GerarTranscricao(Arg.Is(bytesMock))
                .Returns("texto_transcricao");

            var handler = _fixture.Create<GerarTextoHandler>();

            //Act
            var resultado = await handler.ExecutarEtapa(comando);

            //Assert
            Assert.Equal("texto_transcricao", resultado.Criacao.Transcricao);
            Assert.Equal(EstadoProcessamento.GerandoImagem, resultado.Criacao.ProcessamentosCriacao.Last().Estado);
        }

        [Fact]
        public async Task ExecutarEtapa_GetBytes_Exception()
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

            _httpHelperMock.GetBytes(Arg.Is(comando.Criacao.UrlAudio))
                .Throws(new Exception("erro http"));

            var handler = _fixture.Create<GerarTextoHandler>();

            //Act
            var exception = await Assert.ThrowsAsync<Exception>(() => handler.ExecutarEtapa(comando));

            //Assert
            Assert.Equal("erro http", exception.Message);
        }

        [Fact]
        public async Task ExecutarEtapa_GerarTranscricao_Exception()
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

            var bytesMock = Encoding.UTF8.GetBytes("buffer teste");

            _httpHelperMock.GetBytes(Arg.Is(comando.Criacao.UrlAudio))
                .Returns(bytesMock);

            _openAIVendorMock.GerarTranscricao(Arg.Is(bytesMock))
                .Throws(new Exception("erro transcricao"));

            var handler = _fixture.Create<GerarTextoHandler>();

            //Act
            var exception = await Assert.ThrowsAsync<Exception>(() => handler.ExecutarEtapa(comando));

            //Assert
            Assert.Equal("erro transcricao", exception.Message);
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
                }
            };

            var payload = Encoding.UTF8.GetBytes("buffer teste");

            var comando = new Comando(criacao, payload);

            var handler = _fixture.Create<GerarTextoHandler>();

            //Act
            await handler.ExecutarEtapa(comando);

            //Assert
            await _httpHelperMock.DidNotReceive().GetBytes(Arg.Any<string>());
            await _openAIVendorMock.DidNotReceive().GerarTranscricao(Arg.Any<byte[]>());
        }

        [Fact]
        public void ProximaEtapa()
        {
            //Arrange
            var processamentoHandler = Substitute.For<IProcessamentoHandler>();
            var handler = _fixture.Create<GerarTextoHandler>();

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
                UrlImagem = "url_imagem",
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
                }
            };

            var bytesMock = Encoding.UTF8.GetBytes("buffer teste");

            _httpHelperMock.GetBytes(Arg.Is(comando.Criacao.UrlAudio))
                .Returns(bytesMock);

            _openAIVendorMock.GerarTranscricao(Arg.Is(bytesMock))
                .Returns("texto_transcricao");

            var processamentoHandler = Substitute.For<IProcessamentoHandler>();
            processamentoHandler.ExecutarEtapa(Arg.Any<Comando>())
                .Returns(comandoMock);

            var handler = _fixture.Create<GerarTextoHandler>();

            //Act
            handler.ProximaEtapa(processamentoHandler);
            var resultado = await handler.ExecutarEtapa(comando);

            //Assert
            Assert.Equal("url_imagem", resultado.Criacao.UrlImagem);
        }
    }
}
