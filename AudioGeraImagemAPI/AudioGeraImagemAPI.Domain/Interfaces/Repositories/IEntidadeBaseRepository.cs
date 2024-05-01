using AudioGeraImagemAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioGeraImagemAPI.Domain.Interfaces.Repositories
{
    public interface IEntidadeBaseRepository<TEntidade> where TEntidade : EntidadeBase
    {
        Task Inserir(TEntidade entidade);
        Task<TEntidade> Obter(string id);
        Task<ICollection<TEntidade>> ObterTodos();
        Task Atualizar(TEntidade entidade);
        Task Excluir(TEntidade entidade);
    }
}
