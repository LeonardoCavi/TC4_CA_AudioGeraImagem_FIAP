using AudioGeraImagemAPI.Application.Factory;
using AudioGeraImagemAPI.Application.Intefaces;
using AudioGeraImagemAPI.Application.ViewModels;
using AudioGeraImagemAPI.Domain.Entities;
using AudioGeraImagemAPI.Domain.Enums;
using AudioGeraImagemAPI.Domain.Interfaces;
using AudioGeraImagemAPI.Domain.Utility.DTO;
using AudioGeraImagemAPI.Domain.Utility.Factory;

namespace AudioGeraImagemAPI.Application.Services
{
    public class ComandoApplicationService : IComandoApplicationService
    {
        private readonly IComandoService _service;

        public ComandoApplicationService(IComandoService service)
        {
            _service = service;
        }

        public async Task<ResultadoOperacao<List<ListarCriacaoViewModel>>> BuscarCriacoes(string busca)
        {
            List<ListarCriacaoViewModel> listaCriacoes = new();

            var resultado = await _service.Buscar(busca);

            if (!resultado.Sucesso)
                return ResultadoOperacaoFactory.Criar(false, resultado.MensagemErro, listaCriacoes);

            listaCriacoes.AddRange(resultado.Objeto.Select(ViewModelFactory.CriarListarCriacoesViewModel));

            return ResultadoOperacaoFactory.Criar(true, string.Empty, listaCriacoes);
        }

        public async Task<ResultadoOperacao<ObterCriacaoViewModel>> ObterCriacao(string id)
        {
            var resultado = await _service.ObterComando(id);

            if(!resultado.Sucesso)
                return ResultadoOperacaoFactory.Criar(false, resultado.MensagemErro, new ObterCriacaoViewModel());

            return ResultadoOperacaoFactory.Criar(true, string.Empty, ViewModelFactory.CriarObterCriacaoViewModel(resultado.Objeto));
        }

        public async Task<ResultadoOperacao<Stream>> ObterImagem(string id)
        {
            return await _service.ObterImagem(id);
        }

        public async Task<ResultadoOperacao<Guid>> GerarImagem(GerarImagemViewModel gerarImagem)
        {
            if (!RequisicaoValida(gerarImagem))
                return ResultadoOperacaoFactory.Criar(false, "Escreva uma descrição com até 256 caracteres e o arquivo deve ser .mp3", Guid.Empty);

            var comando = CriarComando(gerarImagem);

            await _service.GerarImagem(comando, gerarImagem.Arquivo);

            return ResultadoOperacaoFactory.Criar(true, string.Empty, comando.Id);
        }

        private bool RequisicaoValida(GerarImagemViewModel gerarImagem)
        {
            if (gerarImagem.Descricao.Length > 256)
                return false;

            if (!gerarImagem.Arquivo.ContentType.Contains("audio/mpeg"))
                return false;

            return true;
        }

        private Comando CriarComando(GerarImagemViewModel gerarImagem)
        {
            var comando = new Comando()
            {
                Id = Guid.NewGuid(),
                Descricao = gerarImagem.Descricao,
                InstanteCriacao = DateTime.Now,
                ProcessamentosComandos = new()
            };

            comando.ProcessamentosComandos.Add(new()
            {
                Estado = EstadoComando.Recebido,
                InstanteCriacao = DateTime.Now
            });

            return comando;
        }
    }
}
