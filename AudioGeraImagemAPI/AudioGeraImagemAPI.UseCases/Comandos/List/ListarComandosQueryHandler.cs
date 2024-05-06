using AudioGeraImagemAPI.Domain.Entities;
using AudioGeraImagemAPI.Domain.Interfaces.Repositories;
using AudioGeraImagemAPI.Domain.Utility.DTO;
using AudioGeraImagemAPI.Domain.Utility.Factory;
using MediatR;

namespace AudioGeraImagemAPI.UseCases.Comandos.List
{
    public class ListarComandosQueryHandler : IRequestHandler<ListarComandosQuery, ResultadoOperacao<IEnumerable<ListarComandosDTO>>>
    {
        private readonly IComandoRepository _repository;

        public ListarComandosQueryHandler(IComandoRepository repository)
        {
            _repository = repository;
        }

        public async Task<ResultadoOperacao<IEnumerable<ListarComandosDTO>>> Handle(ListarComandosQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Comando> comandos;

            if (string.IsNullOrEmpty(request.Busca))
            {
                comandos = await _repository.ObterComandosProcessamentos();

                if (comandos == null || !comandos.Any())
                    return ResultadoOperacaoFactory.Criar(false, "Não existem criações.", CriarListarCriacoesViewModel(comandos));
            }
            else
            {
                comandos = await _repository.Buscar(x => x.Descricao.Contains(request.Busca) || x.Transcricao.Contains(request.Busca));

                if (comandos == null || !comandos.Any())
                    return ResultadoOperacaoFactory.Criar(false, "Criações não encontradas.", CriarListarCriacoesViewModel(comandos));
            }

            return ResultadoOperacaoFactory.Criar(true, string.Empty, CriarListarCriacoesViewModel(comandos));
        }

        private IEnumerable<ListarComandosDTO> CriarListarCriacoesViewModel(IEnumerable<Comando> comandos)
        {
            return comandos.Select(comando => new ListarComandosDTO()
            {
                Id = comando.Id,
                Descricao = comando.Descricao,
                Transcricao = comando.Transcricao,
                InstanteCriacao = comando.InstanteCriacao,
                InstanteAtualizacao = comando.InstanteAtualizacao,
                UltimoEstado = comando.ProcessamentosComandos.OrderBy(x => x.InstanteCriacao).Last().Estado
            });
        }
    }
}
