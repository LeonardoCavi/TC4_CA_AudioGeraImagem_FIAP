using AudioGeraImagemAPI.Domain.Entities;
using AudioGeraImagemAPI.Domain.Enums;
using AudioGeraImagemAPI.Domain.Interfaces.Repositories;
using AudioGeraImagemAPI.UseCases.Criacoes.Obter;
using NSubstitute;

namespace AudioGeraImagemAPI.Test.Unitario.AudioGeraImagemAPI.UseCases.Test.Criacoes.Obter
{
    public class ObterCriacaoQueryHandlerTests
    {
        private readonly ICriacaoRepository _repository = Substitute.For<ICriacaoRepository>();

        [Fact]
        public async Task Handler_Teste_Sucesso()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var id = guid.ToString();
            var busca = new ObterCriacaoQuery(id);

            var criacoesMocks = new Criacao
            {
                Id = Guid.NewGuid(),
                Descricao = "teste 1",
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
            var handler = new ObterCriacaoQueryHandler(_repository);

            // Act
            var resultado = await handler.Handle(busca, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.True(resultado.Sucesso);
        }

        [Fact]
        public async Task Handler_Teste_Falha()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var id = guid.ToString();
            var busca = new ObterCriacaoQuery(id);

            Criacao criacoesMocks = null;

            _repository.ObterCriacaoProcessamentos(Arg.Any<string>()).Returns(criacoesMocks);
            var handler = new ObterCriacaoQueryHandler(_repository);

            // Act
            var resultado = await handler.Handle(busca, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.False(resultado.Sucesso);
        }
    }
}