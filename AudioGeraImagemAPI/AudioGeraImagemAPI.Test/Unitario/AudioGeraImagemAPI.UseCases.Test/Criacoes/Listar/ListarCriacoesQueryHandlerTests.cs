using AudioGeraImagemAPI.Domain.Entities;
using AudioGeraImagemAPI.Domain.Enums;
using AudioGeraImagemAPI.Domain.Interfaces.Repositories;
using AudioGeraImagemAPI.Infra;
using AudioGeraImagemAPI.UseCases.Criacoes.Listar;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using System.Linq.Expressions;

namespace AudioGeraImagemAPI.Test.Unitario.AudioGeraImagemAPI.UseCases.Test.Criacoes.Listar
{
    public class ListarCriacoesQueryHandlerTests
    {
        private readonly ICriacaoRepository _repository = Substitute.For<ICriacaoRepository>();

        [Fact]
        public async Task Handler_Teste_Sucesso_Busca()
        {
            // Arrange
            var busca = new ListarCriacoesQuery("teste");
            var criacoesMocks = new List<Criacao>
            {
                new Criacao
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
                },
                new Criacao
                {
                    Id = Guid.NewGuid(),
                    Descricao = "teste 2",
                    ProcessamentosCriacao = new List<ProcessamentoCriacao>
                    {
                        new ProcessamentoCriacao
                        {
                            Estado = EstadoProcessamento.Falha,
                            InstanteCriacao = DateTime.Now
                        }
                    }
                },
                new Criacao
                {
                    Id = Guid.NewGuid(),
                    Descricao = "teste 3",
                    ProcessamentosCriacao = new List<ProcessamentoCriacao>
                    {
                        new ProcessamentoCriacao
                        {
                            Estado = EstadoProcessamento.SalvandoTexto,
                            InstanteCriacao = DateTime.Now
                        }
                    }
                },
                new Criacao
                {
                    Id = Guid.NewGuid(),
                    Descricao = "teste 4",
                    ProcessamentosCriacao = new List<ProcessamentoCriacao>
                    {
                        new ProcessamentoCriacao
                        {
                            Estado = EstadoProcessamento.SalvandoAudio,
                            InstanteCriacao = DateTime.Now
                        }
                    }
                },
                new Criacao
                {
                    Id = Guid.NewGuid(),
                    Descricao = "teste 5",
                    ProcessamentosCriacao = new List<ProcessamentoCriacao>
                    {
                        new ProcessamentoCriacao
                        {
                            Estado = EstadoProcessamento.GerandoImagem,
                            InstanteCriacao = DateTime.Now
                        }
                    }
                }
            };

            _repository.Buscar(Arg.Any<Expression<Func<Criacao, bool>>>()).Returns(criacoesMocks);
            var handler = new ListarCriacoesQueryHandler(_repository);

            // Act
            var resultado = await handler.Handle(busca, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.True(resultado.Sucesso);
        }

        [Fact]
        public async Task Handler_Teste_Sucesso_SemBusca()
        {
            // Arrange
            var busca = new ListarCriacoesQuery("");
            var criacoesMocks = new List<Criacao>
            {
                new Criacao
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
                },
                new Criacao
                {
                    Id = Guid.NewGuid(),
                    Descricao = "teste 2",
                    ProcessamentosCriacao = new List<ProcessamentoCriacao>
                    {
                        new ProcessamentoCriacao
                        {
                            Estado = EstadoProcessamento.Falha,
                            InstanteCriacao = DateTime.Now
                        }
                    }
                },
                new Criacao
                {
                    Id = Guid.NewGuid(),
                    Descricao = "teste 3",
                    ProcessamentosCriacao = new List<ProcessamentoCriacao>
                    {
                        new ProcessamentoCriacao
                        {
                            Estado = EstadoProcessamento.SalvandoTexto,
                            InstanteCriacao = DateTime.Now
                        }
                    }
                },
                new Criacao
                {
                    Id = Guid.NewGuid(),
                    Descricao = "teste 4",
                    ProcessamentosCriacao = new List<ProcessamentoCriacao>
                    {
                        new ProcessamentoCriacao
                        {
                            Estado = EstadoProcessamento.SalvandoAudio,
                            InstanteCriacao = DateTime.Now
                        }
                    }
                },
                new Criacao
                {
                    Id = Guid.NewGuid(),
                    Descricao = "teste 5",
                    ProcessamentosCriacao = new List<ProcessamentoCriacao>
                    {
                        new ProcessamentoCriacao
                        {
                            Estado = EstadoProcessamento.GerandoImagem,
                            InstanteCriacao = DateTime.Now
                        }
                    }
                }
            };

            _repository.ObterCriacoesProcessamentos().Returns(criacoesMocks);
            var handler = new ListarCriacoesQueryHandler(_repository);

            // Act
            var resultado = await handler.Handle(busca, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.True(resultado.Sucesso);
        }

        [Fact]
        public async Task Handler_Teste_Falha_Busca()
        {
            // Arrange
            var busca = new ListarCriacoesQuery("teste");
            var criacoesMocks = new List<Criacao>();

            _repository.ObterCriacoesProcessamentos().Returns(criacoesMocks);
            var handler = new ListarCriacoesQueryHandler(_repository);

            // Act
            var resultado = await handler.Handle(busca, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.False(resultado.Sucesso);
        }

        [Fact]
        public async Task Handler_Teste_Falha_SemBusca()
        {
            // Arrange
            var busca = new ListarCriacoesQuery("");
            var criacoesMocks = new List<Criacao>();

            _repository.ObterCriacoesProcessamentos().Returns(criacoesMocks);
            var handler = new ListarCriacoesQueryHandler(_repository);

            // Act
            var resultado = await handler.Handle(busca, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.False(resultado.Sucesso);
        }
    }
}