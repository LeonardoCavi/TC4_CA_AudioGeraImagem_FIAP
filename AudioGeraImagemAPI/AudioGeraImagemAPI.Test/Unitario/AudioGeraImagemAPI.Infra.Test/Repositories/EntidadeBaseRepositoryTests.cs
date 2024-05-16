using AudioGeraImagemAPI.Domain.Entities;
using AudioGeraImagemAPI.Domain.Enums;
using AudioGeraImagemAPI.Infra;
using AudioGeraImagemAPI.Infra.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AudioGeraImagemAPI.Test.Unitario.AudioGeraImagemAPI.Infra.Test.Repositories
{
    public class EntidadeBaseRepositoryTests
    {
        private readonly ApplicationDbContext _context;
        private Criacao criacaoMock;

        public EntidadeBaseRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "AudioGeraImagemTestsCriacoes_1")
                .Options;

            _context = new ApplicationDbContext(options);

            criacaoMock = new Criacao
            {
                Id = Guid.NewGuid(),
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
        }

        [Fact]
        public async Task Inserir_Teste_Sucesso()
        {
            // Arrange
            var repository = new EntidadeBaseRepository<Criacao>(_context);

            // Act
            await repository.Inserir(criacaoMock);

            // Assert
            var entidadeInserida = await _context.Criacoes.FirstOrDefaultAsync();
            Assert.NotNull(entidadeInserida);
        }

        [Fact]
        public async Task Atualizar_Teste_Sucesso()
        {
            // Arrange
            var repository = new EntidadeBaseRepository<Criacao>(_context);

            // Act 1
            await repository.Inserir(criacaoMock);
            var entidadeInserida = await _context.Criacoes.FirstOrDefaultAsync();

            entidadeInserida.Descricao = "Descricao alterada";

            // Act 2
            await repository.Atualizar(entidadeInserida);
            var entidadeAtualizada = await _context.Criacoes.FirstOrDefaultAsync();

            // Assert
            Assert.NotNull(entidadeAtualizada);
            Assert.Equal("Descricao alterada", entidadeAtualizada.Descricao);
        }

        [Fact]
        public async Task Excluir_Teste_Sucesso()
        {
            // Arrange
            var repository = new EntidadeBaseRepository<Criacao>(_context);
            criacaoMock.Descricao = "teste de exclusao";

            // Act 1
            await repository.Inserir(criacaoMock);
            var entidadeInserida = await _context.Criacoes.FirstOrDefaultAsync(x => x.Descricao == criacaoMock.Descricao);

            // Act 2
            await repository.Excluir(entidadeInserida);
            var entidadeExcluida = await _context.Criacoes.FirstOrDefaultAsync(x => x.Descricao == criacaoMock.Descricao);

            // Assert
            Assert.Null(entidadeExcluida);
        }
    }
}