using AudioGeraImagemAPI.Domain.Entities;
using AudioGeraImagemAPI.Domain.Interfaces.Repositories;
using AudioGeraImagemAPI.Domain.Utility.DTO;
using AudioGeraImagemAPI.Domain.Utility.Factory;
using MediatR;

namespace AudioGeraImagemAPI.UseCases.Criacoes.Obter
{
    public class ObterCriacaoQueryHandler : IRequestHandler<ObterCriacaoQuery, ResultadoOperacao<CriacaoDTO>>
    {
        private readonly ICriacaoRepository _repository;

        public ObterCriacaoQueryHandler(ICriacaoRepository repository)
        {
            _repository = repository;
        }

        public async Task<ResultadoOperacao<CriacaoDTO>> Handle(ObterCriacaoQuery request, CancellationToken cancellationToken)
        {
            var criacao = await _repository.ObterCriacaoProcessamentos(request.Id);

            if (criacao == null)
                return ResultadoOperacaoFactory.Criar(false, "Criação não encontrada.", new CriacaoDTO());

            return ResultadoOperacaoFactory.Criar(true, string.Empty, CriarCriacaoDTO(criacao));
        }

        private CriacaoDTO CriarCriacaoDTO(Criacao criacao)
        {
            return new()
            {
                Id = criacao.Id,
                Descricao = criacao.Descricao,
                UrlAudio = criacao.UrlAudio,
                Transcricao = criacao.Transcricao,
                UrlImagem = criacao.UrlImagem,
                InstanteCriacao = criacao.InstanteCriacao,
                InstanteAtualizacao = criacao.InstanteAtualizacao,
                ProcessamentosCriacao = criacao.ProcessamentosCriacao.OrderBy(x => x.InstanteCriacao).ToList()
            };
        }
    }
}
