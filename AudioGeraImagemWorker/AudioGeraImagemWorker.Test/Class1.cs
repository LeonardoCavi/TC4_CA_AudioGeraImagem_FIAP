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
using System.Text;

namespace AudioGeraImagemWorker.Test
{
    public class Class1
    {
        private readonly IFixture fixture;
        private readonly ICriacaoRepository criacaoRepositoryMock;
        private readonly IHttpHelper httpHelperMock;
        private readonly IOpenAIVendor openAIVendorMock;
        private readonly IBucketManager bucketManagerMock;
        private Comando comando;

        public Class1()
        {
            fixture = new Fixture();

            var criacao = fixture.Create<Criacao>();
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

            comando = new()
            {
                Criacao = criacao,
                Payload = payload
            };

            criacaoRepositoryMock = Substitute.For<ICriacaoRepository>();
            httpHelperMock = Substitute.For<IHttpHelper>();
            openAIVendorMock = Substitute.For<IOpenAIVendor>();
            bucketManagerMock = Substitute.For<IBucketManager>();
        }

        [Fact]
        public async Task PrimerioTeste()
        {
            var salvarAudioHandler = new SalvarAudioHandler(
                Substitute.For<ILogger<SalvarAudioHandler>>(),
                bucketManagerMock,
                criacaoRepositoryMock);

            var gerarTextoHandler = new GerarTextoHandler(
                Substitute.For<ILogger<GerarTextoHandler>>(),
                openAIVendorMock,
                httpHelperMock,
                criacaoRepositoryMock);

            var gerarImagemHandler = new GerarImagemHandler(
                Substitute.For<ILogger<GerarImagemHandler>>(),
                openAIVendorMock,
                criacaoRepositoryMock);

            var salvarImagemHandler = new SalvarImagemHandler(
                Substitute.For<ILogger<SalvarImagemHandler>>(),
                bucketManagerMock,
                httpHelperMock,
                criacaoRepositoryMock
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
        public async Task PrimerioTeste_Exception()
        {
            var salvarAudioHandler = new SalvarAudioHandler(
                Substitute.For<ILogger<SalvarAudioHandler>>(),
                bucketManagerMock,
                criacaoRepositoryMock);

            var gerarTextoHandler = new GerarTextoHandler(
                Substitute.For<ILogger<GerarTextoHandler>>(),
                openAIVendorMock,
                httpHelperMock,
                criacaoRepositoryMock);

            var gerarImagemHandler = new GerarImagemHandler(
                Substitute.For<ILogger<GerarImagemHandler>>(),
                openAIVendorMock,
                criacaoRepositoryMock);

            var salvarImagemHandler = new SalvarImagemHandler(
                Substitute.For<ILogger<SalvarImagemHandler>>(),
                bucketManagerMock,
                httpHelperMock,
                criacaoRepositoryMock
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
        public async Task SegundoTeste()
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
                bucketManagerMock,
                criacaoRepository);

            var gerarTextoHandler = new GerarTextoHandler(
                Substitute.For<ILogger<GerarTextoHandler>>(),
                openAIVendorMock,
                httpHelperMock,
                criacaoRepository);

            var gerarImagemHandler = new GerarImagemHandler(
                Substitute.For<ILogger<GerarImagemHandler>>(),
                openAIVendorMock,
                criacaoRepository);

            var salvarImagemHandler = new SalvarImagemHandler(
                Substitute.For<ILogger<SalvarImagemHandler>>(),
                bucketManagerMock,
                httpHelperMock,
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
