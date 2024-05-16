using AudioGeraImagem.Domain.Messages;
using AudioGeraImagemAPI.Domain.Entities;
using AudioGeraImagemAPI.Domain.Interfaces.Repositories;
using AudioGeraImagemAPI.UseCases.Criacoes.GerarImagem;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using System.Text;

namespace AudioGeraImagemAPI.Test.Unitario.AudioGeraImagemAPI.UseCases.Test.Criacoes.GerarImagem
{
    public class GerarImagemCommandHandlerTests
    {
        private readonly ICriacaoRepository _repository = Substitute.For<ICriacaoRepository>();
        private readonly IBus _bus = Substitute.For<IBus>();
        private readonly IConfiguration _configuration = Substitute.For<IConfiguration>();
        private readonly ISendEndpoint _endpoint = Substitute.For<ISendEndpoint>();

        [Fact]
        public async Task Handler_Teste_Sucesso()
        {
            // Arrange
            var bytes = Encoding.UTF8.GetBytes("This is a dummy file");
            IFormFile file = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "dummy.mp3")
            {
                Headers = new HeaderDictionary()
                {
                    {"Content-Disposition", "form-data; name=\"file\"; filename=\"dummy.mp3\""},
                    {"Content-Type", "audio/mpeg"}
                }
            };

            var command = new GerarImagemCommand("apensa um teste", file);
            var handler = new GerarImagemCommandHandler(_repository, _bus, _configuration);
            _bus.GetSendEndpoint(Arg.Any<Uri>()).Returns(_endpoint);

            // Act
            var resultado = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.True(resultado.Sucesso);
            await _repository.Received().Inserir(Arg.Is<Criacao>(x => x.Descricao == command.Descricao));
            await _endpoint.Received().Send(Arg.Is<CriacaoMessage>(x => x.CriacaoId == resultado.Objeto));
        }
    }
}