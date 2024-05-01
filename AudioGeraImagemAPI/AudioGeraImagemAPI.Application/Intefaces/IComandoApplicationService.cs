using AudioGeraImagemAPI.Application.ViewModels;
using AudioGeraImagemAPI.Domain.Entities;
using AudioGeraImagemAPI.Domain.Utility.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioGeraImagemAPI.Application.Intefaces
{
    public interface IComandoApplicationService
    {
        Task<ResultadoOperacao<Guid>> GerarImagem(GerarImagemViewModel gerarImagem);
        Task<ResultadoOperacao<List<ListarCriacaoViewModel>>> BuscarCriacoes(string busca);
        Task<ResultadoOperacao<ObterCriacaoViewModel>> ObterCriacao(string id);
        Task<ResultadoOperacao<Stream>> ObterImagem(string id);
    }
}
