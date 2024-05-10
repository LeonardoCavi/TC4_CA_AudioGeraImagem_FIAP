using AudioGeraImagem.Domain.Entities;
using AudioGeraImagem.Domain.Messages;
using AudioGeraImagemWorker.Domain.Entities;
using AudioGeraImagemWorker.Domain.Enums;
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

        public async Task TratarErro(Criacao criacao, EstadoProcessamento ultimoEstado, byte[] payload)
        {
            var ultimosProcessamentos = criacao.ProcessamentosCriacao.Where(x => x.Estado == ultimoEstado);

            if (ultimosProcessamentos.Count() < 3)
            {
                var mensagem = CriarMensagem(criacao, ultimoEstado, payload);
                await PublicarMensagem(mensagem);
            }
            else
            {
                var novoProcessamentoCriacao = new ProcessamentoCriacao(EstadoProcessamento.Falha);
                criacao.ProcessamentosCriacao.Add(novoProcessamentoCriacao);
                criacao.InstanteAtualizacao = novoProcessamentoCriacao.InstanteCriacao;
            }
        }

        private RetentativaCriacaoMessage CriarMensagem(Criacao criacao, EstadoProcessamento ultimoEstado, byte[] payload)
        {
            return new()
            {
                CriacaoId = criacao.Id,
                UltimoEstado = ultimoEstado,
                Payload = payload
            };
        }
        private async Task PublicarMensagem(RetentativaCriacaoMessage mensagem)
        {
            await _messageScheduler.SchedulePublish(DateTime.UtcNow + TimeSpan.FromSeconds(20), mensagem);
        }
    }
}