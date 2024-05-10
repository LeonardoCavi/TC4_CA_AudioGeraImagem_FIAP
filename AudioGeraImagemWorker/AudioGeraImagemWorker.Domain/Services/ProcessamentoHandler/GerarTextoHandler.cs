using AudioGeraImagem.Domain.Entities;
using AudioGeraImagemWorker.Domain.DTOs;
using AudioGeraImagemWorker.Domain.Entities;
using AudioGeraImagemWorker.Domain.Enums;
using AudioGeraImagemWorker.Domain.Interfaces.Repositories;
using AudioGeraImagemWorker.Domain.Interfaces.Utility;
using AudioGeraImagemWorker.Domain.Interfaces.Vendor;
using Microsoft.Extensions.Logging;

namespace AudioGeraImagemWorker.Domain.Services.ProcessamentoHandler
{
    public class GerarTextoHandler : ProcessamentoHandlerBase
    {
        private readonly string _className = typeof(GerarTextoHandler).Name;
        private readonly ILogger<GerarTextoHandler> _logger;
        private readonly IOpenAIVendor _openAIVendor;
        private readonly IHttpHelper _httpHelper;

        public GerarTextoHandler(ILogger<GerarTextoHandler> logger, IOpenAIVendor openAIVendor, IHttpHelper httpHelper, ICriacaoRepository criacaoRepository) : base(criacaoRepository)
        {
            _logger = logger;
            _openAIVendor = openAIVendor;
            _httpHelper = httpHelper;
        }

        protected override async Task Executar(Comando comando)
        {
            try
            {
                var criacao = comando.Criacao;

                var bytes = await _httpHelper.GetBytes(criacao.UrlAudio);
                criacao.Transcricao = await _openAIVendor.GerarTranscricao(bytes);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{_className}] - [Executar] => Exception.: {ex.Message}");
                throw;
            }
        }

        protected override bool PodeExecutar(ProcessamentoCriacao processamentoCriacao)
        {
            return processamentoCriacao.Estado is EstadoProcessamento.GerandoTexto;
        }
    }
}
