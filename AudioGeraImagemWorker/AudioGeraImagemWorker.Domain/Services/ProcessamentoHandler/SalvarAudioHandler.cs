using AudioGeraImagemWorker.Domain.DTOs;
using AudioGeraImagemWorker.Domain.Entities;
using AudioGeraImagemWorker.Domain.Enums;
using AudioGeraImagemWorker.Domain.Interfaces.Repositories;
using AudioGeraImagemWorker.Domain.Interfaces.Vendor;
using Microsoft.Extensions.Logging;

namespace AudioGeraImagemWorker.Domain.Services.ProcessamentoHandler
{
    public class SalvarAudioHandler : ProcessamentoHandlerBase
    {
        private readonly string _className = typeof(SalvarAudioHandler).Name;
        private readonly ILogger<SalvarAudioHandler> _logger;
        private readonly IBucketManager _bucketManager;

        public SalvarAudioHandler(ILogger<SalvarAudioHandler> logger, IBucketManager bucketManager, ICriacaoRepository criacaoRepository) : base(criacaoRepository)
        {
            _logger = logger;
            _bucketManager = bucketManager;
        }

        protected override async Task Executar(Comando comando)
        {
            try
            {
                var criacao = comando.Criacao;

                var fileName = string.Concat("audios/", criacao.Id.ToString(), ".mp3");
                criacao.UrlAudio = await _bucketManager.ArmazenarObjeto(comando.Payload, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{_className}] - [Executar] => Exception.: {ex.Message}");
                throw;
            }
        }
        protected override bool PodeExecutar(ProcessamentoCriacao processamento)
        {
            return processamento.Estado is EstadoProcessamento.SalvandoAudio;
        }
    }
}
