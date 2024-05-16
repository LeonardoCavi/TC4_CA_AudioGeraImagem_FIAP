using AudioGeraImagemWorker.Domain.DTOs;

namespace AudioGeraImagemWorker.Domain.Interfaces.Services
{
    public interface IProcessamentoHandler
    {
        IProcessamentoHandler ProximaEtapa(IProcessamentoHandler processamentoHandler);

        Task<Comando> ExecutarEtapa(Comando comando);
    }
}
