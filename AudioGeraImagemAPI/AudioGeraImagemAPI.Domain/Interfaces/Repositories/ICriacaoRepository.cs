using AudioGeraImagemAPI.Domain.Entities;
using System.Linq.Expressions;

namespace AudioGeraImagemAPI.Domain.Interfaces.Repositories
{
    public interface ICriacaoRepository: IEntidadeBaseRepository<Criacao>
    {
        Task<Criacao> ObterCriacaoProcessamentos(string id);
        Task<ICollection<Criacao>> ObterCriacoesProcessamentos();
        Task<ICollection<Criacao>> Buscar(Expression<Func<Criacao, bool>> predicate);
    }
}
