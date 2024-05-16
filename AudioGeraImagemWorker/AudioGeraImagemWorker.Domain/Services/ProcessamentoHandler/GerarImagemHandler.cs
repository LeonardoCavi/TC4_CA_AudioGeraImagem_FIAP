using AudioGeraImagemWorker.Domain.DTOs;
using AudioGeraImagemWorker.Domain.Entities;
using AudioGeraImagemWorker.Domain.Enums;
using AudioGeraImagemWorker.Domain.Interfaces.Repositories;
using AudioGeraImagemWorker.Domain.Interfaces.Vendor;
using Microsoft.Extensions.Logging;

namespace AudioGeraImagemWorker.Domain.Services.ProcessamentoHandler
{
    public class GerarImagemHandler : ProcessamentoHandlerBase
    {
        private readonly string _className = typeof(GerarImagemHandler).Name;
        private readonly ILogger<GerarImagemHandler> _logger;
        private readonly IOpenAIVendor _openAIVendor;

        public GerarImagemHandler(ILogger<GerarImagemHandler> logger, IOpenAIVendor openAIVendor, ICriacaoRepository criacaoRepository) : base(criacaoRepository)
        {
            _logger = logger;
            _openAIVendor = openAIVendor;
        }

        protected override async Task Executar(Comando comando)
        {
            try
            {
                var criacao = comando.Criacao;

                criacao.UrlImagem = await _openAIVendor.GerarImagem(criacao.Transcricao);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{_className}] - [Executar] => Exception.: {ex.Message}");
                throw;
            }
        }

        protected override bool PodeExecutar(ProcessamentoCriacao processamentoCriacao)
        {
            return processamentoCriacao.Estado is EstadoProcessamento.GerandoImagem;
        }
    }
}
