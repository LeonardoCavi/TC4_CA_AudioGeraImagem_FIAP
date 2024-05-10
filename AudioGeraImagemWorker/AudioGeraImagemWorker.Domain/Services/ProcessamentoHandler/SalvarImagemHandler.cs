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
    public class SalvarImagemHandler : ProcessamentoHandlerBase
    {
        private readonly string _className = typeof(SalvarImagemHandler).Name;
        private readonly ILogger<SalvarImagemHandler> _logger;
        private readonly IBucketManager _bucketManager;
        private readonly IHttpHelper _httpHelper;

        public SalvarImagemHandler(ILogger<SalvarImagemHandler> logger, IBucketManager bucketManager, IHttpHelper httpHelper, ICriacaoRepository criacaoRepository) : base(criacaoRepository)
        {
            _logger = logger;
            _bucketManager = bucketManager;
            _httpHelper = httpHelper;
        }

        protected override async Task Executar(Comando comando)
        {
            try
            {
                var criacao = comando.Criacao;

                var bytes = await _httpHelper.GetBytes(criacao.UrlImagem);
                var fileName = string.Concat("imagens/", criacao.Id.ToString(), ".jpeg");
                var urlImagem = await _bucketManager.ArmazenarObjeto(bytes, fileName);
                criacao.UrlImagem = urlImagem;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{_className}] - [Executar] => Exception.: {ex.Message}");
                throw;
            }
        }

        protected override bool PodeExecutar(ProcessamentoCriacao processamentoCriacao)
        {
            return processamentoCriacao.Estado is EstadoProcessamento.SalvadoImagem;
        }
    }
}
