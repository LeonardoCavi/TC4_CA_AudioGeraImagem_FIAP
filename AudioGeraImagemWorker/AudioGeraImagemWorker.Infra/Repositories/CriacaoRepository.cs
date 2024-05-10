using AudioGeraImagem.Domain.Entities;
using AudioGeraImagemWorker.Domain.Entities;
using AudioGeraImagemWorker.Domain.Interfaces.Repositories;

namespace AudioGeraImagemWorker.Infra.Repositories
{
    public class CriacaoRepository : EntidadeBaseRepository<Criacao>, ICriacaoRepository
    {
        public CriacaoRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}