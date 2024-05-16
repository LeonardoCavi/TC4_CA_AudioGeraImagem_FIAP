using AudioGeraImagemAPI.Domain.Entities;
using AudioGeraImagemAPI.Domain.Enums;
using AudioGeraImagemAPI.Infra;
using AudioGeraImagemAPI.Infra.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AudioGeraImagemAPI.Test.Unitario.AudioGeraImagemAPI.Infra.Test.Repositories
{
    public class CriacaoRepositoryTests
    {
        private readonly ApplicationDbContext _context;

        public CriacaoRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "AudioGeraImagemTestsCriacoes_2")
            .Options;

            _context = new ApplicationDbContext(options);
        }

        [Fact]
        public async Task ObterCriacaoProcessamentos_Teste_Sucesso()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var id = guid.ToString();
            var criacaoMock = new Criacao
            {
                Id = guid,
                Descricao = "Apenas um teste",
                ProcessamentosCriacao = new()
                {
                    new()
                    {
                        Estado = EstadoProcessamento.Finalizado,
                        InstanteCriacao = DateTime.Now
                    }
                }
            };

            _context.Criacoes.Add(criacaoMock);
            _context.SaveChanges();
            var repositorio = new CriacaoRepository(_context);

            // Act
            var resultado = await repositorio.ObterCriacaoProcessamentos(id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(criacaoMock, resultado);
        }

        [Fact]
        public async Task ObterCriacoesProcessamentos_Teste_Sucesso()
        {
            // Arrange
            var criacoesMocks = new List<Criacao>
            {
                new Criacao
                {
                    Id = Guid.NewGuid(),
                    Descricao = "Mock 1",
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
                    Descricao = "Mock 2",
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
                    Descricao = "Mock 3",
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
                    Descricao = "Mock 4",
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
                    Descricao = "Mock 5",
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

            foreach (var criacao in criacoesMocks)
            {
                _context.Criacoes.Add(criacao);
                _context.SaveChanges();
            }

            var repositorio = new CriacaoRepository(_context);

            // Act
            var resultado = await repositorio.ObterCriacoesProcessamentos();

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(criacoesMocks, resultado);
        }

        [Fact]
        public async Task Buscar_Teste_Sucesso()
        {
            // Arrange
            var busca = "teste";
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
                    Descricao = "Mock 5",
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
            foreach (var criacao in criacoesMocks)
            {
                _context.Criacoes.Add(criacao);
                _context.SaveChanges();
            }

            var repositorio = new CriacaoRepository(_context);

            // Act
            var resultado = await repositorio
                .Buscar(x =>
                x.Descricao.Contains(busca) ||
                x.Transcricao.Contains(busca));

            // Assert
            Assert.NotNull(resultado);
            foreach (var item in resultado)
            {
                Assert.Contains(busca, item.Descricao);
            }
        }
    }
}