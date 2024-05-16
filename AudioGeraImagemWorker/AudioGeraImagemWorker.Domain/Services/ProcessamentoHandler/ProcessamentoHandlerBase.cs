using AudioGeraImagem.Domain.Entities;
using AudioGeraImagemWorker.Domain.DTOs;
using AudioGeraImagemWorker.Domain.Entities;
using AudioGeraImagemWorker.Domain.Enums;
using AudioGeraImagemWorker.Domain.Interfaces.Repositories;
using AudioGeraImagemWorker.Domain.Interfaces.Services;

namespace AudioGeraImagemWorker.Domain.Services.ProcessamentoHandler
{
    public abstract class ProcessamentoHandlerBase : IProcessamentoHandler
    {
        private readonly ICriacaoRepository _criacaoRepository;
        private IProcessamentoHandler _proximoProcessamentoHandler;

        protected ProcessamentoHandlerBase(ICriacaoRepository criacaoRepository)
        {
            _criacaoRepository = criacaoRepository;
        }

        public IProcessamentoHandler ProximaEtapa(IProcessamentoHandler processamentoHandler)
        {
            _proximoProcessamentoHandler = processamentoHandler;

            return processamentoHandler;
        }

        public async Task<Comando> ExecutarEtapa(Comando comando)
        {
            if (VerificarPodeExecutar(comando))
            {
                await Executar(comando);
                await AtualizarProcessamentoCriacao(comando.Criacao);
            }

            if (_proximoProcessamentoHandler != null)
                return await _proximoProcessamentoHandler.ExecutarEtapa(comando);
            
            return comando;
        }

        private bool VerificarPodeExecutar(Comando comando)
        {
            var ultimoProcessamento = comando.Criacao.ProcessamentosCriacao.LastOrDefault();

            return PodeExecutar(ultimoProcessamento);
        }

        private async Task AtualizarProcessamentoCriacao(Criacao criacao)
        {
            var ultimoProcessamento = criacao.ProcessamentosCriacao.LastOrDefault();

            EstadoProcessamento novoEstado = default;

            switch (ultimoProcessamento.Estado)
            {
                case EstadoProcessamento.SalvandoAudio:
                    novoEstado = EstadoProcessamento.GerandoTexto;
                    break;

                case EstadoProcessamento.GerandoTexto:
                    novoEstado = EstadoProcessamento.GerandoImagem;
                    break;

                case EstadoProcessamento.GerandoImagem:
                    novoEstado = EstadoProcessamento.SalvadoImagem;
                    break;

                case EstadoProcessamento.SalvadoImagem:
                    novoEstado = EstadoProcessamento.Finalizado;
                    break;
            }

            await AtualizarCriacao(criacao, novoEstado);
        }

        private async Task AtualizarCriacao(Criacao criacao, EstadoProcessamento novoEstado)
        {
            var novoProcessamentoCriacao = new ProcessamentoCriacao(novoEstado);
            criacao.ProcessamentosCriacao.Add(novoProcessamentoCriacao);
            criacao.InstanteAtualizacao = novoProcessamentoCriacao.InstanteCriacao;

            await _criacaoRepository.Atualizar(criacao);
        }

        protected abstract bool PodeExecutar(ProcessamentoCriacao comando);
        protected abstract Task Executar(Comando comando);
    }
}
