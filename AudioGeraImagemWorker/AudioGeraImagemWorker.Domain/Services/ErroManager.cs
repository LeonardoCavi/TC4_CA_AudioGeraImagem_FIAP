using AudioGeraImagem.Domain.Entities;
using AudioGeraImagem.Domain.Messages;
using AudioGeraImagemWorker.Domain.Enums;
using AudioGeraImagemWorker.Domain.Factories;
using AudioGeraImagemWorker.Domain.Interfaces;
using MassTransit;

namespace AudioGeraImagemWorker.Domain.Services
{
    public class ErroManager : IErroManager
    {
        private readonly IMessageScheduler _messageScheduler;

        public ErroManager(IMessageScheduler messageScheduler)
        {
            _messageScheduler = messageScheduler;
        }

        public async Task TratarErro(Comando comando, EstadoComando ultimoEstado, byte[] payload)
        {
            var ultimosProcessamentos = comando.ProcessamentosComandos.Where(x => x.Estado == ultimoEstado);

            if (ultimosProcessamentos.Count() < 3)
            {
                var mensagem = CriarMensagem(comando, ultimoEstado, payload);
                await PublicarMensagem(mensagem);
            }
            else
            {
                var novoProcessamentoComando = ProcessamentoComandoFactory.Novo(EstadoComando.Falha);
                comando.ProcessamentosComandos.Add(novoProcessamentoComando);
                comando.InstanteAtualizacao = novoProcessamentoComando.InstanteCriacao;
            }
        }

        private RetentativaComandoMessage CriarMensagem(Comando comando, EstadoComando ultimoEstado, byte[] payload)
        {
            return new()
            {
                ComandoId = comando.Id,
                UltimoEstado = ultimoEstado,
                Payload = payload
            };
        }
        private async Task PublicarMensagem(RetentativaComandoMessage mensagem)
        {
            await _messageScheduler.SchedulePublish(DateTime.UtcNow + TimeSpan.FromSeconds(20), mensagem);
        }
    }
}