using AudioGeraImagemAPI.API.Models;
using AudioGeraImagemAPI.UseCases.Comandos.Create;
using AudioGeraImagemAPI.UseCases.Comandos.Get;
using AudioGeraImagemAPI.UseCases.Comandos.List;
using AudioGeraImagemAPI.UseCases.Imagens.Get;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AudioGeraImagemAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComandoController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ComandoController> _logger;
        private readonly string ClassName = typeof(ComandoController).Name;

        public ComandoController(IMediator mediator, ILogger<ComandoController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("gerar-imagem")]
        public async Task<IActionResult> GerarImagem([FromForm] GerarImagemRequest gerarImagem)
        {
            try
            {
                var command = new CriarComandoCommand(gerarImagem.Descricao, gerarImagem.Arquivo);
                _logger.LogInformation($"[{ClassName}] - [GerarImagem] => Request.: {command}");
                var resultado = await _mediator.Send(command);

                if(resultado.Sucesso)
                    return Accepted(string.Empty, resultado.Objeto);
                
                return BadRequest(resultado.MensagemErro);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ClassName}] - [GerarImagem] => Exception.: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("buscar-criacoes")]
        public async Task<IActionResult> BuscarCriacoes(string busca = "")
        {
            try
            {
                var query = new ListarComandosQuery(busca);
                _logger.LogInformation($"[{ClassName}] - [BuscarCriacoes] => Request.: {query}");
                var resultado = await _mediator.Send(query);

                if (resultado.Sucesso)
                    return Ok(resultado.Objeto);

                return NotFound(resultado.MensagemErro);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ClassName}] - [ListarCriacoes] => Exception.: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("obter-criacao/{id}")]
        public async Task<IActionResult> ObterCriacao(string id)
        {
            try
            {
                var query = new ObterComandoQuery(id);
                _logger.LogInformation($"[{ClassName}] - [ObterCriacao] => Request.: {query}");
                var resultado = await _mediator.Send(query);

                if (resultado.Sucesso)
                    return Ok(resultado.Objeto);

                return NotFound(resultado.MensagemErro);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ClassName}] - [ObterCriacao] => Exception.: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("obter-imagem/{id}")]
        public async Task<IActionResult> ObterImagem(string id)
        {
            try
            {
                var query = new ObterImagemQuery(id);
                _logger.LogInformation($"[{ClassName}] - [ObterImagem] => Request.: {query}");
                var resultado = await _mediator.Send(query);

                if (resultado.Sucesso)
                    return File(resultado.Objeto, "image/jpeg");

                return BadRequest(resultado.MensagemErro);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ClassName}] - [ObterImagem] => Exception.: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
