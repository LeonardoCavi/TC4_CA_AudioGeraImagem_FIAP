using AudioGeraImagemAPI.Domain.Entities;
using AudioGeraImagemAPI.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AudioGeraImagemAPI.Infra.Repositories
{
    public class ComandoRepository : EntidadeBaseRepository<Comando>, IComandoRepository
    {
        public ComandoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Comando> ObterComandoProcessamentos(string id)
        {
            return await _dbSet
                .Include(x => x.ProcessamentosComandos)
                .Where(x => x.Id.ToString() == id)
                .FirstOrDefaultAsync();
        }

        public async Task<ICollection<Comando>> ObterComandosProcessamentos()
        {
            return await _dbSet
                .Include(x => x.ProcessamentosComandos)
                .Take(50)
                .ToListAsync();
        }

        public async Task<ICollection<Comando>> Buscar(Expression<Func<Comando, bool>> predicate)
        {
            return await _dbSet
                .Include(x => x.ProcessamentosComandos)
                .Where(predicate)
                .Take(50)
                .ToListAsync();
        }
    }
}
