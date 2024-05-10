using AudioGeraImagemAPI.Domain.Entities;
using AudioGeraImagemAPI.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AudioGeraImagemAPI.Infra.Repositories
{
    public class CriacaoRepository : EntidadeBaseRepository<Criacao>, ICriacaoRepository
    {
        public CriacaoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Criacao> ObterCriacaoProcessamentos(string id)
        {
            return await _dbSet
                .Include(x => x.ProcessamentosCriacao)
                .Where(x => x.Id.ToString() == id)
                .FirstOrDefaultAsync();
        }

        public async Task<ICollection<Criacao>> ObterCriacoesProcessamentos()
        {
            return await _dbSet
                .Include(x => x.ProcessamentosCriacao)
                .Take(50)
                .ToListAsync();
        }

        public async Task<ICollection<Criacao>> Buscar(Expression<Func<Criacao, bool>> predicate)
        {
            return await _dbSet
                .Include(x => x.ProcessamentosCriacao)
                .Where(predicate)
                .Take(50)
                .ToListAsync();
        }
    }
}
