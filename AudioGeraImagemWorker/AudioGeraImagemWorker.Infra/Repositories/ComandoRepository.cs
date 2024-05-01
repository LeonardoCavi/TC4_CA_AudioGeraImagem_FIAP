using AudioGeraImagem.Domain.Entities;
using AudioGeraImagemWorker.Domain.Entities;
using AudioGeraImagemWorker.Domain.Interfaces.Repositories;

namespace AudioGeraImagemWorker.Infra.Repositories
{
    public class ComandoRepository : EntidadeBaseRepository<Comando>, IComandoRepository
    {
        public ComandoRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}