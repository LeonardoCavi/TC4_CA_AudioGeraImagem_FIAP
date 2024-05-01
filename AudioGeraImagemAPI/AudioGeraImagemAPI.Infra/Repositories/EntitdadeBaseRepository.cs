using AudioGeraImagemAPI.Domain.Entities;
using AudioGeraImagemAPI.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AudioGeraImagemAPI.Infra.Repositories
{
    public class EntidadeBaseRepository<TEntidade> : IEntidadeBaseRepository<TEntidade> where TEntidade : EntidadeBase
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<TEntidade> _dbSet;

        public EntidadeBaseRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntidade>();
        }

        public async Task Atualizar(TEntidade entidade)
        {
            _context.Update(entidade);
            await _context.SaveChangesAsync();
        }

        public async Task Excluir(TEntidade entidade)
        {
            _context.Remove(entidade);
            await _context.SaveChangesAsync();
        }

        public async Task Inserir(TEntidade entidade)
        {
            await _context.AddAsync(entidade);
            await _context.SaveChangesAsync();
        }

        public async Task<TEntidade> Obter(string id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<ICollection<TEntidade>> ObterTodos()
        {
            return await _dbSet.ToListAsync();
        }
    }
}
