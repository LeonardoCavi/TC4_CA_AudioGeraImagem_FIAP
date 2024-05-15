using AudioGeraImagem.Domain.Entities;
using AudioGeraImagemWorker.Domain.Enums;
using AudioGeraImagemWorker.Infra;
using AudioGeraImagemWorker.Infra.Repositories;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Polly;

namespace AudioGeraImagemWorker.Test.Unitario.AudioGeraImagemWorker.Infra.Teste.Repositories
{
    public class CriacaoRepositoryTest
    {
        private readonly IFixture _fixture;
        protected readonly ApplicationDbContext _context;

        public CriacaoRepositoryTest()
        {
            _fixture = new Fixture();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "db")
            .Options;

            _context = new ApplicationDbContext(options);
            _context.Criacoes.AddRange(new List<Criacao>()
            {
                _fixture.Create<Criacao>(),
                _fixture.Create<Criacao>()
            });
            _context.SaveChanges();
            _fixture.Register(() => _context);
        }

        [Fact]
        public async Task Inserir_Sucesso()
        {
            var criacao = _fixture.Create<Criacao>();

            var repository = _fixture.Create<CriacaoRepository>();

            await repository.Inserir(criacao);

            var criacaoDb = await _context.Criacoes.FindAsync(criacao.Id);

            Assert.NotNull(criacaoDb);
        }

        [Fact]
        public async Task Atualizar_Sucesso()
        {
            var criacao = _fixture.Create<Criacao>();

            var repository = _fixture.Create<CriacaoRepository>();

            await repository.Inserir(criacao);

            criacao.Descricao = "teste 123";

            await repository.Atualizar(criacao);

            var criacaoDb = await _context.Criacoes.FindAsync(criacao.Id);

            Assert.NotNull(criacaoDb);
            Assert.Equal("teste 123", criacaoDb.Descricao);
        }

        [Fact]
        public async Task Excluir_Sucesso()
        {
            var criacao = _fixture.Create<Criacao>();

            var repository = _fixture.Create<CriacaoRepository>();

            await repository.Inserir(criacao);

            criacao.Descricao = "teste 123";

            await repository.Excluir(criacao);

            var criacaoDb = await _context.Criacoes.FindAsync(criacao.Id);

            Assert.Null(criacaoDb);
        }

        [Fact]
        public async Task Obter_Sucesso()
        {
            var primeiraCriacao = _context.Criacoes.First();

            var repository = _fixture.Create<CriacaoRepository>();

            var criacaoDb = await repository.Obter(primeiraCriacao.Id);

            Assert.NotNull(criacaoDb);
        }

        [Fact]
        public async Task ObterTodos_Sucesso()
        {
            var repository = _fixture.Create<CriacaoRepository>();

            var criacoesDB = await repository.ObterTodos();

            Assert.Equal(2, criacoesDB.Count);
        }
    }
}
