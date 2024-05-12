using AudioGeraImagem.Domain.Entities;
using AudioGeraImagemWorker.Domain.DTOs;
using AudioGeraImagemWorker.Domain.Enums;
using AudioGeraImagemWorker.Domain.Interfaces.Repositories;
using AudioGeraImagemWorker.Domain.Interfaces.Utility;
using AudioGeraImagemWorker.Domain.Interfaces.Vendor;
using AudioGeraImagemWorker.Domain.Services.ProcessamentoHandler;
using AudioGeraImagemWorker.Infra;
using AudioGeraImagemWorker.Infra.Repositories;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Text;

namespace AudioGeraImagemWorker.Test.Unitario.AudioGeraImagemWorker.Domain.Teste.Services.ProcessamentoHandler
{
    public class ProcessamentoHandlerBaseTest
    {
        private readonly IFixture _fixture;
        private readonly ICriacaoRepository _criacaoRepositoryMock;
        private readonly IHttpHelper _httpHelperMock;
        private readonly IOpenAIVendor _openAIVendorMock;
        private readonly IBucketManager _bucketManagerMock;
        private Comando comando;

        public ProcessamentoHandlerBaseTest()
        {
            _fixture = new Fixture();

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

            comando = new(criacao, payload);

            _criacaoRepositoryMock = Substitute.For<ICriacaoRepository>();
            _httpHelperMock = Substitute.For<IHttpHelper>();
            _openAIVendorMock = Substitute.For<IOpenAIVendor>();
            _bucketManagerMock = Substitute.For<IBucketManager>();
        }

        [Fact]
        public async Task ExecutarEtapa_Sucesso()
        {
            var salvarAudioHandler = new SalvarAudioHandler(
                Substitute.For<ILogger<SalvarAudioHandler>>(),
                _bucketManagerMock,
                _criacaoRepositoryMock);

            var gerarTextoHandler = new GerarTextoHandler(
                Substitute.For<ILogger<GerarTextoHandler>>(),
                _openAIVendorMock,
                _httpHelperMock,
                _criacaoRepositoryMock);

            var gerarImagemHandler = new GerarImagemHandler(
                Substitute.For<ILogger<GerarImagemHandler>>(),
                _openAIVendorMock,
                _criacaoRepositoryMock);

            var salvarImagemHandler = new SalvarImagemHandler(
                Substitute.For<ILogger<SalvarImagemHandler>>(),
                _bucketManagerMock,
                _httpHelperMock,
                _criacaoRepositoryMock
                );

            // Set the chain of responsibility
            salvarAudioHandler
                .ProximaEtapa(gerarTextoHandler)
                .ProximaEtapa(gerarImagemHandler)
                .ProximaEtapa(salvarImagemHandler);

            var resultado = await salvarAudioHandler.ExecutarEtapa(comando);
            var ultimoProcessamento = resultado.Criacao.ProcessamentosCriacao.Last();

            Assert.Equal(EstadoProcessamento.Finalizado, ultimoProcessamento.Estado);
        }

        [Fact]
        public async Task ExecutarEtapa_SalvarAudio_Exception()
        {
            var salvarAudioHandler = new SalvarAudioHandler(
                Substitute.For<ILogger<SalvarAudioHandler>>(),
                _bucketManagerMock,
                _criacaoRepositoryMock);

            var gerarTextoHandler = new GerarTextoHandler(
                Substitute.For<ILogger<GerarTextoHandler>>(),
                _openAIVendorMock,
                _httpHelperMock,
                _criacaoRepositoryMock);

            var gerarImagemHandler = new GerarImagemHandler(
                Substitute.For<ILogger<GerarImagemHandler>>(),
                _openAIVendorMock,
                _criacaoRepositoryMock);

            var salvarImagemHandler = new SalvarImagemHandler(
                Substitute.For<ILogger<SalvarImagemHandler>>(),
                _bucketManagerMock,
                _httpHelperMock,
                _criacaoRepositoryMock
                );

            _bucketManagerMock.ArmazenarObjeto(Arg.Is(comando.Payload), Arg.Any<string>())
                .Throws(new Exception("erro bucket"));

            // Set the chain of responsibility
            salvarAudioHandler
                .ProximaEtapa(gerarTextoHandler)
                .ProximaEtapa(gerarImagemHandler)
                .ProximaEtapa(salvarImagemHandler);

            //Act
            var exception = await Assert.ThrowsAsync<Exception>(() => salvarAudioHandler.ExecutarEtapa(comando));

            //Assert
            Assert.Equal("erro bucket", exception.Message);
        }

        [Fact]
        public async Task ExecutarEtapa_GerarTexto_GetBytes_Exception()
        {
            var salvarAudioHandler = new SalvarAudioHandler(
                Substitute.For<ILogger<SalvarAudioHandler>>(),
                _bucketManagerMock,
                _criacaoRepositoryMock);

            var gerarTextoHandler = new GerarTextoHandler(
                Substitute.For<ILogger<GerarTextoHandler>>(),
                _openAIVendorMock,
                _httpHelperMock,
                _criacaoRepositoryMock);

            var gerarImagemHandler = new GerarImagemHandler(
                Substitute.For<ILogger<GerarImagemHandler>>(),
                _openAIVendorMock,
                _criacaoRepositoryMock);

            var salvarImagemHandler = new SalvarImagemHandler(
                Substitute.For<ILogger<SalvarImagemHandler>>(),
                _bucketManagerMock,
                _httpHelperMock,
                _criacaoRepositoryMock
                );

            _bucketManagerMock.ArmazenarObjeto(Arg.Is(comando.Payload), Arg.Any<string>())
                .Returns("url_bucket_audio");

            _httpHelperMock.GetBytes(Arg.Is("url_bucket_audio"))
                .Throws(new Exception("erro http"));

            // Set the chain of responsibility
            salvarAudioHandler
                .ProximaEtapa(gerarTextoHandler)
                .ProximaEtapa(gerarImagemHandler)
                .ProximaEtapa(salvarImagemHandler);

            //Act
            var exception = await Assert.ThrowsAsync<Exception>(() => salvarAudioHandler.ExecutarEtapa(comando));

            //Assert
            Assert.Equal("erro http", exception.Message);
        }

        [Fact]
        public async Task ExecutarEtapa_GerarTexto_GerarTranscricao_Exception()
        {
            var salvarAudioHandler = new SalvarAudioHandler(
                Substitute.For<ILogger<SalvarAudioHandler>>(),
                _bucketManagerMock,
                _criacaoRepositoryMock);

            var gerarTextoHandler = new GerarTextoHandler(
                Substitute.For<ILogger<GerarTextoHandler>>(),
                _openAIVendorMock,
                _httpHelperMock,
                _criacaoRepositoryMock);

            var gerarImagemHandler = new GerarImagemHandler(
                Substitute.For<ILogger<GerarImagemHandler>>(),
                _openAIVendorMock,
                _criacaoRepositoryMock);

            var salvarImagemHandler = new SalvarImagemHandler(
                Substitute.For<ILogger<SalvarImagemHandler>>(),
                _bucketManagerMock,
                _httpHelperMock,
                _criacaoRepositoryMock
                );

            _bucketManagerMock.ArmazenarObjeto(Arg.Is(comando.Payload), Arg.Any<string>())
                .Returns("url_bucket_audio");

            var audioBytesMock = Encoding.UTF8.GetBytes("buffer teste");

            _httpHelperMock.GetBytes(Arg.Is("url_bucket_audio"))
                .Returns(audioBytesMock);

            _openAIVendorMock.GerarTranscricao(Arg.Is(audioBytesMock))
                .Throws(new Exception("erro transcricao"));

            // Set the chain of responsibility
            salvarAudioHandler
                .ProximaEtapa(gerarTextoHandler)
                .ProximaEtapa(gerarImagemHandler)
                .ProximaEtapa(salvarImagemHandler);

            //Act
            var exception = await Assert.ThrowsAsync<Exception>(() => salvarAudioHandler.ExecutarEtapa(comando));

            //Assert
            Assert.Equal("erro transcricao", exception.Message);
        }

        [Fact]
        public async Task ExecutarEtapa_GerarImagem_Exception()
        {
            var salvarAudioHandler = new SalvarAudioHandler(
                Substitute.For<ILogger<SalvarAudioHandler>>(),
                _bucketManagerMock,
                _criacaoRepositoryMock);

            var gerarTextoHandler = new GerarTextoHandler(
                Substitute.For<ILogger<GerarTextoHandler>>(),
                _openAIVendorMock,
                _httpHelperMock,
                _criacaoRepositoryMock);

            var gerarImagemHandler = new GerarImagemHandler(
                Substitute.For<ILogger<GerarImagemHandler>>(),
                _openAIVendorMock,
                _criacaoRepositoryMock);

            var salvarImagemHandler = new SalvarImagemHandler(
                Substitute.For<ILogger<SalvarImagemHandler>>(),
                _bucketManagerMock,
                _httpHelperMock,
                _criacaoRepositoryMock
                );

            _bucketManagerMock.ArmazenarObjeto(Arg.Is(comando.Payload), Arg.Any<string>())
                .Returns("url_bucket_audio");

            var audioBytesMock = Encoding.UTF8.GetBytes("buffer teste");

            _httpHelperMock.GetBytes(Arg.Is("url_bucket_audio"))
                .Returns(audioBytesMock);

            _openAIVendorMock.GerarTranscricao(Arg.Is(audioBytesMock))
                .Returns("texto_transcricao");

            _openAIVendorMock.GerarImagem(Arg.Is("texto_transcricao"))
                .Throws(new Exception("erro imagem"));

            // Set the chain of responsibility
            salvarAudioHandler
                .ProximaEtapa(gerarTextoHandler)
                .ProximaEtapa(gerarImagemHandler)
                .ProximaEtapa(salvarImagemHandler);

            //Act
            var exception = await Assert.ThrowsAsync<Exception>(() => salvarAudioHandler.ExecutarEtapa(comando));

            //Assert
            Assert.Equal("erro imagem", exception.Message);
        }

        [Fact]
        public async Task ExecutarEtapa_SalvarImagem_GetBytes_Exception()
        {
            var salvarAudioHandler = new SalvarAudioHandler(
                Substitute.For<ILogger<SalvarAudioHandler>>(),
                _bucketManagerMock,
                _criacaoRepositoryMock);

            var gerarTextoHandler = new GerarTextoHandler(
                Substitute.For<ILogger<GerarTextoHandler>>(),
                _openAIVendorMock,
                _httpHelperMock,
                _criacaoRepositoryMock);

            var gerarImagemHandler = new GerarImagemHandler(
                Substitute.For<ILogger<GerarImagemHandler>>(),
                _openAIVendorMock,
                _criacaoRepositoryMock);

            var salvarImagemHandler = new SalvarImagemHandler(
                Substitute.For<ILogger<SalvarImagemHandler>>(),
                _bucketManagerMock,
                _httpHelperMock,
                _criacaoRepositoryMock
                );

            _bucketManagerMock.ArmazenarObjeto(Arg.Is(comando.Payload), Arg.Any<string>())
                .Returns("url_bucket_audio");

            var audioBytesMock = Encoding.UTF8.GetBytes("buffer teste");

            _httpHelperMock.GetBytes(Arg.Is("url_bucket_audio"))
                .Returns(audioBytesMock);

            _openAIVendorMock.GerarTranscricao(Arg.Is(audioBytesMock))
                .Returns("texto_transcricao");

            _openAIVendorMock.GerarImagem(Arg.Is("texto_transcricao"))
                .Returns("url_imagem");

            _httpHelperMock.GetBytes(Arg.Is("url_imagem"))
                .Throws(new Exception("erro http"));

            // Set the chain of responsibility
            salvarAudioHandler
                .ProximaEtapa(gerarTextoHandler)
                .ProximaEtapa(gerarImagemHandler)
                .ProximaEtapa(salvarImagemHandler);

            //Act
            var exception = await Assert.ThrowsAsync<Exception>(() => salvarAudioHandler.ExecutarEtapa(comando));

            //Assert
            Assert.Equal("erro http", exception.Message);
        }

        [Fact]
        public async Task ExecutarEtapa_SalvarImagem_ArmazenarObjeto_Exception()
        {
            var salvarAudioHandler = new SalvarAudioHandler(
                Substitute.For<ILogger<SalvarAudioHandler>>(),
                _bucketManagerMock,
                _criacaoRepositoryMock);

            var gerarTextoHandler = new GerarTextoHandler(
                Substitute.For<ILogger<GerarTextoHandler>>(),
                _openAIVendorMock,
                _httpHelperMock,
                _criacaoRepositoryMock);

            var gerarImagemHandler = new GerarImagemHandler(
                Substitute.For<ILogger<GerarImagemHandler>>(),
                _openAIVendorMock,
                _criacaoRepositoryMock);

            var salvarImagemHandler = new SalvarImagemHandler(
                Substitute.For<ILogger<SalvarImagemHandler>>(),
                _bucketManagerMock,
                _httpHelperMock,
                _criacaoRepositoryMock
                );

            _bucketManagerMock.ArmazenarObjeto(Arg.Is(comando.Payload), Arg.Any<string>())
                .Returns("url_bucket_audio");

            var audioBytesMock = Encoding.UTF8.GetBytes("buffer teste");

            _httpHelperMock.GetBytes(Arg.Is("url_bucket_audio"))
                .Returns(audioBytesMock);

            _openAIVendorMock.GerarTranscricao(Arg.Is(audioBytesMock))
                .Returns("texto_transcricao");

            _openAIVendorMock.GerarImagem(Arg.Is("texto_transcricao"))
                .Returns("url_imagem");

            var imagembytesMock = Encoding.UTF8.GetBytes("buffer teste 2");

            _httpHelperMock.GetBytes(Arg.Is("url_imagem"))
                .Returns(imagembytesMock);

            _bucketManagerMock.ArmazenarObjeto(Arg.Is(imagembytesMock), Arg.Any<string>())
                .Throws(new Exception("erro bucket"));

            // Set the chain of responsibility
            salvarAudioHandler
                .ProximaEtapa(gerarTextoHandler)
                .ProximaEtapa(gerarImagemHandler)
                .ProximaEtapa(salvarImagemHandler);

            //Act
            var exception = await Assert.ThrowsAsync<Exception>(() => salvarAudioHandler.ExecutarEtapa(comando));

            //Assert
            Assert.Equal("erro bucket", exception.Message);
        }

        [Fact]
        public async Task ExecutarEtapa_Database_Sucesso()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "audioGeraImagem")
            .Options;

            using var context = new ApplicationDbContext(options);
            context.Criacoes.Add(comando.Criacao);
            context.SaveChanges();

            var criacaoRepository = new CriacaoRepository(context);

            var salvarAudioHandler = new SalvarAudioHandler(
                Substitute.For<ILogger<SalvarAudioHandler>>(),
                _bucketManagerMock,
                criacaoRepository);

            var gerarTextoHandler = new GerarTextoHandler(
                Substitute.For<ILogger<GerarTextoHandler>>(),
                _openAIVendorMock,
                _httpHelperMock,
                criacaoRepository);

            var gerarImagemHandler = new GerarImagemHandler(
                Substitute.For<ILogger<GerarImagemHandler>>(),
                _openAIVendorMock,
                criacaoRepository);

            var salvarImagemHandler = new SalvarImagemHandler(
                Substitute.For<ILogger<SalvarImagemHandler>>(),
                _bucketManagerMock,
                _httpHelperMock,
                criacaoRepository
                );

            // Set the chain of responsibility
            salvarAudioHandler
                .ProximaEtapa(gerarTextoHandler)
                .ProximaEtapa(gerarImagemHandler)
                .ProximaEtapa(salvarImagemHandler);

            var resultado = await salvarAudioHandler.ExecutarEtapa(comando);

            var resultadoDb = await criacaoRepository.Obter(resultado.Criacao.Id);
            var ultimoProcessamento = resultadoDb.ProcessamentosCriacao.Last();

            Assert.Equal(EstadoProcessamento.Finalizado, ultimoProcessamento.Estado);
        }
    }
}
