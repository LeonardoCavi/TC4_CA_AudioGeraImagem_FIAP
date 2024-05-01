using AudioGeraImagemWorker.Domain.Entities;

namespace AudioGeraImagemWorker.Domain.Interfaces.Repositories
{
    public interface IEntidadeBaseRepository<TEntidade> where TEntidade : EntidadeBase
    {
        Task Inserir(TEntidade entidade);

        Task<TEntidade> Obter(Guid id);

        Task<ICollection<TEntidade>> ObterTodos();

        Task Atualizar(TEntidade entidade);

        Task Excluir(TEntidade entidade);
    }
}