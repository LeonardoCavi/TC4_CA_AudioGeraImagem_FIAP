using AudioGeraImagemAPI.Domain.Interfaces.Repositories;
using AudioGeraImagemAPI.Domain.Utility;
using AudioGeraImagemAPI.Domain.Utility.DTO;
using AudioGeraImagemAPI.Domain.Utility.Factory;
using MediatR;

namespace AudioGeraImagemAPI.UseCases.Criacoes.ObterImagem
{
    public class ObterImagemQueryHandler : IRequestHandler<ObterImagemQuery, ResultadoOperacao<Stream>>
    {
        private readonly ICriacaoRepository _repository;
        private readonly HttpHelper _httpHelper;

        public ObterImagemQueryHandler(ICriacaoRepository repository, HttpHelper httpHelper)
        {
            _repository = repository;
            _httpHelper = httpHelper;
        }

        public async Task<ResultadoOperacao<Stream>> Handle(ObterImagemQuery request, CancellationToken cancellationToken)
        {
            var criacao = await _repository.ObterCriacaoProcessamentos(request.Id);

            if (criacao == null)
                return ResultadoOperacaoFactory.Criar(false, "Criação não encontrada.", Stream.Null);

            if (!criacao.ProcessamentosCriacao.Any(x => x.Estado == Domain.Enums.EstadoProcessamento.Finalizado))
                return ResultadoOperacaoFactory.Criar(false, "Criação ainda está em processamento ou finalizou com falha.", Stream.Null);

            var bytes = await _httpHelper.GetBytes(criacao.UrlImagem);

            return ResultadoOperacaoFactory.Criar(true, string.Empty, ObterStream(bytes));
        }

        private Stream ObterStream(byte[] bytes)
        {
            return new MemoryStream(bytes);
        }
    }
}
