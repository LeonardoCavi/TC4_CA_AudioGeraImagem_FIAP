using AudioGeraImagemAPI.Domain.Entities;
using AudioGeraImagemAPI.Domain.Enums;
using AudioGeraImagemAPI.Domain.Interfaces.Repositories;
using AudioGeraImagemAPI.Domain.Interfaces.Utility;
using AudioGeraImagemAPI.Domain.Utility;
using AudioGeraImagemAPI.UseCases.Criacoes.ObterImagem;
using NSubstitute;
using System.Text;

namespace AudioGeraImagemAPI.Test.Unitario.AudioGeraImagemAPI.UseCases.Test.Criacoes.ObterImagem
{
    public class ObterImagemQueryHandlerTests
    {
        private readonly ICriacaoRepository _repository = Substitute.For<ICriacaoRepository>();
        private readonly IHttpHelper _httpHelper = Substitute.For<IHttpHelper>();

        [Fact]
        public async Task Handle_Teste_Sucesso()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var id = guid.ToString();
            var imagemQuery = new ObterImagemQuery(id);
            var bytes = Encoding.UTF8.GetBytes("This is a dummy file");

            var criacoesMocks = new Criacao
            {
                Id = guid,
                Descricao = "teste 1",
                UrlImagem = "https://s3.amazonaws.com/bucket/imagem/teste.jpg",
                ProcessamentosCriacao = new List<ProcessamentoCriacao>
                    {
                        new ProcessamentoCriacao
                        {
                            Estado = EstadoProcessamento.Finalizado,
                            InstanteCriacao = DateTime.Now
                        }
                    }
            };

            _repository.ObterCriacaoProcessamentos(Arg.Any<string>()).Returns(criacoesMocks);
            _httpHelper.GetBytes(criacoesMocks.UrlImagem).Returns(bytes);

            var handler = new ObterImagemQueryHandler(_repository, _httpHelper);

            // Act
            var resultado = await handler.Handle(imagemQuery, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.True(resultado.Sucesso);
        }

        [Fact]
        public async Task Handle_Teste_EmAndamento()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var id = guid.ToString();
            var imagemQuery = new ObterImagemQuery(id);
            var bytes = Encoding.UTF8.GetBytes("This is a dummy file");

            var criacoesMocks = new Criacao
            {
                Id = guid,
                Descricao = "teste 1",
                UrlImagem = "https://s3.amazonaws.com/bucket/imagem/teste.jpg",
                ProcessamentosCriacao = new List<ProcessamentoCriacao>
                    {
                        new ProcessamentoCriacao
                        {
                            Estado = EstadoProcessamento.GerandoImagem,
                            InstanteCriacao = DateTime.Now
                        }
                    }
            };

            _repository.ObterCriacaoProcessamentos(Arg.Any<string>()).Returns(criacoesMocks);
            _httpHelper.GetBytes(criacoesMocks.UrlImagem).Returns(bytes);

            var handler = new ObterImagemQueryHandler(_repository, _httpHelper);

            // Act
            var resultado = await handler.Handle(imagemQuery, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.False(resultado.Sucesso);
        }

        [Fact]
        public async Task Handle_Teste_NaoEncontrado()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var id = guid.ToString();
            var imagemQuery = new ObterImagemQuery(id);
            var bytes = Encoding.UTF8.GetBytes("This is a dummy file");

            var criacao = new Criacao();
            criacao = null;

            _repository.ObterCriacaoProcessamentos(Arg.Any<string>()).Returns(criacao);

            var handler = new ObterImagemQueryHandler(_repository, _httpHelper);

            // Act
            var resultado = await handler.Handle(imagemQuery, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.False(resultado.Sucesso);
        }
    }
}