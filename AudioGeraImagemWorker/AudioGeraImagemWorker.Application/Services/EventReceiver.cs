using AudioGeraImagem.Domain.Messages;
using AudioGeraImagemWorker.Application.Interfaces;
using AudioGeraImagemWorker.Domain.Interfaces;
using AudioGeraImagemWorker.Domain.Services;
using Microsoft.Extensions.Logging;

namespace AudioGeraImagemWorker.Application.Services
{
    public class EventReceiver : IEventReceiver
    {
        private readonly IComandoManager _comandoManager;
        private readonly ILogger<ComandoManager> _logger;
        private readonly string _className = typeof(ComandoManager).Name;

        public EventReceiver(IComandoManager comandoManager,
                             ILogger<ComandoManager> logger)
        {
            _comandoManager = comandoManager;
            _logger = logger;
        }

        public async Task ReceberMensagem(ComandoMessage mensagem)
        {
            try
            {
                await _comandoManager.ProcessarComando(mensagem);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{_className}] - [ReceberMensagem] => Exception.: {ex.Message}");
            }
        }

        public async Task ReceberRetentativa(RetentativaComandoMessage mensagem)
        {
            try
            {
                await _comandoManager.ReprocessarComando(mensagem);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{_className}] - [ReceberRetentativa] => Exception.: {ex.Message}");
            }
        }
    }
}