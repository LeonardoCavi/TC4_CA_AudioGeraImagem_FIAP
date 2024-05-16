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
    public class SalvarImagemHandlerTest
    {
        private readonly IFixture _fixture;
        private readonly ILogger<SalvarImagemHandler> _loggerMock;
        private readonly IBucketManager _bucketManagerMock;
        private readonly IHttpHelper _httpHelperMock;
        private readonly ICriacaoRepository _criacaoRepositoryMock;

        public SalvarImagemHandlerTest()
        {
            _fixture = new Fixture();

            _loggerMock = Substitute.For<ILogger<SalvarImagemHandler>>();
            _bucketManagerMock = Substitute.For<IBucketManager>();
            _httpHelperMock = Substitute.For<IHttpHelper>();
            _criacaoRepositoryMock = Substitute.For<ICriacaoRepository>();

            _fixture.Register(() => _loggerMock);
            _fixture.Register(() => _bucketManagerMock);
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

            var bytesMock = Encoding.UTF8.GetBytes("buffer teste");

            _httpHelperMock.GetBytes(Arg.Is(comando.Criacao.UrlImagem))
                .Returns(bytesMock);

            _bucketManagerMock.ArmazenarObjeto(Arg.Is(bytesMock), Arg.Any<string>())
                .Returns("url_bucket_imagem");

            var handler = _fixture.Create<SalvarImagemHandler>();

            //Act
            var resultado = await handler.ExecutarEtapa(comando);

            //Assert
            Assert.Equal("url_bucket_imagem", resultado.Criacao.UrlImagem);
            Assert.Equal(EstadoProcessamento.Finalizado, resultado.Criacao.ProcessamentosCriacao.Last().Estado);
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

            _httpHelperMock.GetBytes(Arg.Is(comando.Criacao.UrlImagem))
                .Throws(new Exception("erro http"));

            var handler = _fixture.Create<SalvarImagemHandler>();

            //Act
            var exception = await Assert.ThrowsAsync<Exception>(() => handler.ExecutarEtapa(comando));

            //Assert
            Assert.Equal("erro http", exception.Message);
        }

        [Fact]
        public async Task ExecutarEtapa_ArmazenarObjeto_Exception()
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
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Estado = EstadoProcessamento.SalvadoImagem,
                    InstanteCriacao = DateTime.Now.AddSeconds(30),
                    MensagemErro = string.Empty
                }
            };

            var bytesMock = Encoding.UTF8.GetBytes("buffer teste");

            _httpHelperMock.GetBytes(Arg.Is(comando.Criacao.UrlImagem))
                .Returns(bytesMock);

            _bucketManagerMock.ArmazenarObjeto(Arg.Is(bytesMock), Arg.Any<string>())
                .Throws(new Exception("erro bucket"));

            var handler = _fixture.Create<SalvarImagemHandler>();

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

            var payload = Encoding.UTF8.GetBytes("buffer teste");

            var comando = new Comando(criacao, payload);

            var handler = _fixture.Create<SalvarImagemHandler>();

            //Act
            await handler.ExecutarEtapa(comando);

            //Assert
            await _httpHelperMock.DidNotReceive().GetBytes(Arg.Any<string>());
            await _bucketManagerMock.DidNotReceive().ArmazenarObjeto(Arg.Any<byte[]>(), Arg.Any<string>());
        }

        [Fact]
        public void ProximaEtapa()
        {
            //Arrange
            var processamentoHandler = Substitute.For<IProcessamentoHandler>();
            var handler = _fixture.Create<SalvarImagemHandler>();

            //Act
            var resultado = handler.ProximaEtapa(processamentoHandler);

            //Assert
            Assert.Equal(processamentoHandler, resultado);
        }
    }
}
