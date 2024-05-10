using AudioGeraImagemAPI.Domain.Entities;
using AudioGeraImagemAPI.Domain.Interfaces.Repositories;
using AudioGeraImagemAPI.Domain.Utility.DTO;
using AudioGeraImagemAPI.Domain.Utility.Factory;
using MediatR;

namespace AudioGeraImagemAPI.UseCases.Criacoes.List
{
    public class ListarCriacoesQueryHandler : IRequestHandler<ListarCriacoesQuery, ResultadoOperacao<IEnumerable<ListarCriacoesDTO>>>
    {
        private readonly ICriacaoRepository _repository;

        public ListarCriacoesQueryHandler(ICriacaoRepository repository)
        {
            _repository = repository;
        }

        public async Task<ResultadoOperacao<IEnumerable<ListarCriacoesDTO>>> Handle(ListarCriacoesQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Criacao> criacaos;

            if (string.IsNullOrEmpty(request.Busca))
            {
                criacaos = await _repository.ObterCriacoesProcessamentos();

                if (criacaos == null || !criacaos.Any())
                    return ResultadoOperacaoFactory.Criar(false, "Não existem criações.", CriarListarCriacoesViewModel(criacaos));
            }
            else
            {
                criacaos = await _repository.Buscar(x => x.Descricao.Contains(request.Busca) || x.Transcricao.Contains(request.Busca));

                if (criacaos == null || !criacaos.Any())
                    return ResultadoOperacaoFactory.Criar(false, "Criações não encontradas.", CriarListarCriacoesViewModel(criacaos));
            }

            return ResultadoOperacaoFactory.Criar(true, string.Empty, CriarListarCriacoesViewModel(criacaos));
        }

        private IEnumerable<ListarCriacoesDTO> CriarListarCriacoesViewModel(IEnumerable<Criacao> criacaos)
        {
            return criacaos.Select(criacao => new ListarCriacoesDTO()
            {
                Id = criacao.Id,
                Descricao = criacao.Descricao,
                Transcricao = criacao.Transcricao,
                InstanteCriacao = criacao.InstanteCriacao,
                InstanteAtualizacao = criacao.InstanteAtualizacao,
                UltimoEstado = criacao.ProcessamentosCriacao.OrderBy(x => x.InstanteCriacao).Last().Estado
            });
        }
    }
}
