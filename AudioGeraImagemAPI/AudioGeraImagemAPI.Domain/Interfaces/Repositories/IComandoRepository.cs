using AudioGeraImagemAPI.Domain.Entities;
using System.Linq.Expressions;

namespace AudioGeraImagemAPI.Domain.Interfaces.Repositories
{
    public interface IComandoRepository: IEntidadeBaseRepository<Comando>
    {
        Task<Comando> ObterComandoProcessamentos(string id);
        Task<ICollection<Comando>> ObterComandosProcessamentos();
        Task<ICollection<Comando>> Buscar(Expression<Func<Comando, bool>> predicate);
    }
}
