using AudioGeraImagem.Domain.Entities;
using AudioGeraImagem.Domain.Messages;
using AudioGeraImagemWorker.Domain.DTOs;
using AudioGeraImagemWorker.Domain.Entities;
using AudioGeraImagemWorker.Domain.Enums;
using AudioGeraImagemWorker.Domain.Interfaces.Repositories;
using AudioGeraImagemWorker.Domain.Interfaces.Services;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AudioGeraImagemWorker.UseCases.Criacoes.Processar
{
    internal class ProcessarCriacaoCommandHandler : IRequestHandler<ProcessarCriacaoCommand>
    {
        private readonly string _className = typeof(ProcessarCriacaoCommandHandler).Name;
        private readonly ILogger<ProcessarCriacaoCommandHandler> _logger;
        private readonly ICriacaoRepository _criacaoRepository;
        private readonly IProcessamentoHandler _processamentoHandler;
        private readonly IMessageScheduler _messageScheduler;

        public ProcessarCriacaoCommandHandler(
            ILogger<ProcessarCriacaoCommandHandler> logger,
            ICriacaoRepository criacaoRepository,
            IProcessamentoHandler processamentoHandler,
            IMessageScheduler messageScheduler)
        {
            _processamentoHandler = processamentoHandler;
            _criacaoRepository = criacaoRepository;
            _messageScheduler = messageScheduler;
            _logger = logger;
        }

        public async Task Handle(ProcessarCriacaoCommand request, CancellationToken cancellationToken)
        {
            var criacao = await _criacaoRepository.Obter(request.CriacaoId);

            if (criacao is null)
            {
                _logger.LogWarning($"[{_className}] - [Handle] => Criacao descartada, pois a criacao de id '{request.CriacaoId}' não existe.");
            }
            else
            {
                EstadoProcessamento novoEstado = default;

                if (criacao.ProcessamentosCriacao.Last()?.Estado is EstadoProcessamento.Recebido)
                    novoEstado = EstadoProcessamento.SalvandoAudio;

                if (request.Retentativa)
                    novoEstado = request.UltimoEstado;

                await AtualizarCriacao(criacao, novoEstado);

                await ExecutarCriacao(criacao, request.Payload);
            }
        }

        private async Task ExecutarCriacao(Criacao criacao, byte[] payload)
        {
            try
            {
                var comando = new Comando(criacao, payload);
                await _processamentoHandler.ExecutarEtapa(comando);
            }
            catch (Exception ex)
            {
                var ultimoProcessamento = criacao.ProcessamentosCriacao.Last();
                ultimoProcessamento.MensagemErro = ex.Message;
                await TratarErro(criacao, ultimoProcessamento.Estado, payload);
            }
        }

        private async Task TratarErro(Criacao criacao, EstadoProcessamento ultimoEstado, byte[] payload)
        {
            var ultimosProcessamentos = criacao.ProcessamentosCriacao.Where(x => x.Estado == ultimoEstado);

            if (ultimosProcessamentos.Count() == 3)
                await AtualizarCriacao(criacao, EstadoProcessamento.Falha);
            else
            {
                var mensagem = new RetentativaCriacaoMessage(criacao.Id, ultimoEstado, payload);
                await _messageScheduler.SchedulePublish(DateTime.UtcNow + TimeSpan.FromSeconds(20), mensagem);
            }       
        }

        private async Task AtualizarCriacao(Criacao criacao, EstadoProcessamento novoEstado)
        {
            var novoProcessamentoCriacao = new ProcessamentoCriacao(novoEstado);
            criacao.ProcessamentosCriacao.Add(novoProcessamentoCriacao);
            criacao.InstanteAtualizacao = novoProcessamentoCriacao.InstanteCriacao;

            await _criacaoRepository.Atualizar(criacao);
        }
    }
}
