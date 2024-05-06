using AudioGeraImagemAPI.Domain.Entities;
using AudioGeraImagemAPI.Domain.Interfaces.Repositories;
using AudioGeraImagemAPI.Domain.Utility.DTO;
using AudioGeraImagemAPI.Domain.Utility.Factory;
using MediatR;

namespace AudioGeraImagemAPI.UseCases.Comandos.Get
{
    public class ObterComandoQueryHandler : IRequestHandler<ObterComandoQuery, ResultadoOperacao<ComandoDTO>>
    {
        private readonly IComandoRepository _repository;

        public ObterComandoQueryHandler(IComandoRepository repository)
        {
            _repository = repository;
        }

        public async Task<ResultadoOperacao<ComandoDTO>> Handle(ObterComandoQuery request, CancellationToken cancellationToken)
        {
            var comando = await _repository.ObterComandoProcessamentos(request.Id);

            if (comando == null)
                return ResultadoOperacaoFactory.Criar(false, "Criação não encontrada.", new ComandoDTO());

            return ResultadoOperacaoFactory.Criar(true, string.Empty, CriarComandoDTO(comando));
        }

        private ComandoDTO CriarComandoDTO(Comando comando)
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
    }
}
