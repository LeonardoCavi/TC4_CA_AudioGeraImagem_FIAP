using AudioGeraImagemAPI.Domain.Interfaces.Repositories;
using AudioGeraImagemAPI.Domain.Utility;
using AudioGeraImagemAPI.Domain.Utility.DTO;
using AudioGeraImagemAPI.Domain.Utility.Factory;
using MediatR;

namespace AudioGeraImagemAPI.UseCases.Imagens.Get
{
    public class ObterImagemQueryHandler : IRequestHandler<ObterImagemQuery, ResultadoOperacao<Stream>>
    {
        private readonly IComandoRepository _repository;
        private readonly HttpHelper _httpHelper;

        public ObterImagemQueryHandler(IComandoRepository repository, HttpHelper httpHelper)
        {
            _repository = repository;
            _httpHelper = httpHelper;
        }

        public async Task<ResultadoOperacao<Stream>> Handle(ObterImagemQuery request, CancellationToken cancellationToken)
        {
            var comando = await _repository.ObterComandoProcessamentos(request.Id);

            if (comando == null)
                return ResultadoOperacaoFactory.Criar(false, "Criação não encontrada.", Stream.Null);

            if (!comando.ProcessamentosComandos.Any(x => x.Estado == Domain.Enums.EstadoComando.Finalizado))
                return ResultadoOperacaoFactory.Criar(false, "Comando ainda está em processamento ou finalizou com falha.", Stream.Null);

            var bytes = await _httpHelper.GetBytes(comando.UrlImagem);

            return ResultadoOperacaoFactory.Criar(true, string.Empty, ObterStream(bytes));
        }

        private Stream ObterStream(byte[] bytes)
        {
            return new MemoryStream(bytes);
        }
    }
}
