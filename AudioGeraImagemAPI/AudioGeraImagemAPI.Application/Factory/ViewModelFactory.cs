using AudioGeraImagemAPI.Application.ViewModels;
using AudioGeraImagemAPI.Domain.Entities;

namespace AudioGeraImagemAPI.Application.Factory
{
    public static class ViewModelFactory
    {
        public static ObterCriacaoViewModel CriarObterCriacaoViewModel(Comando comando)
        {
            return new()
            {
                Id = comando.Id,
                Descricao = comando.Descricao,
                UrlAudio = comando.UrlAudio,
                Transcricao = comando.Transcricao,
                UrlImagem = comando.UrlImagem,
                InstanteCriacao = comando.InstanteCriacao,
                InstanteAtualizacao = comando.InstanteAtualizacao,
                ProcessamentosComandos = comando.ProcessamentosComandos.OrderBy(x => x.InstanteCriacao).ToList()
            };
        }
        public static ListarCriacaoViewModel CriarListarCriacoesViewModel(Comando comando)
        {
            return new()
            {
                Id = comando.Id,
                Descricao = comando.Descricao,
                Transcricao = comando.Transcricao,
                InstanteCriacao = comando.InstanteCriacao,
                InstanteAtualizacao = comando.InstanteAtualizacao,
                UltimoEstado = comando.ProcessamentosComandos.OrderBy(x => x.InstanteCriacao).Last().Estado
            };
        }
    }
}
