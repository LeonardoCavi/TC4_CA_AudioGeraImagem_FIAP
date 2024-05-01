using AudioGeraImagemAPI.Domain.Entities;
using AudioGeraImagemAPI.Domain.Utility.DTO;
using Microsoft.AspNetCore.Http;

namespace AudioGeraImagemAPI.Domain.Interfaces
{
    public interface IComandoService
    {
        Task GerarImagem(Comando comando, IFormFile arquivo);
        Task<ResultadoOperacao<Comando>> ObterComando(string id);
        Task<ResultadoOperacao<ICollection<Comando>>> Buscar(string busca);
        Task<ResultadoOperacao<Stream>> ObterImagem(string id);
    }
}
