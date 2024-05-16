using AudioGeraImagemAPI.API.Models;
using AudioGeraImagemAPI.UseCases.Criacoes.GerarImagem;
using AudioGeraImagemAPI.UseCases.Criacoes.Listar;
using AudioGeraImagemAPI.UseCases.Criacoes.Obter;
using AudioGeraImagemAPI.UseCases.Criacoes.ObterImagem;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AudioGeraImagemAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CriacaoController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CriacaoController> _logger;
        private readonly string ClassName = typeof(CriacaoController).Name;

        public CriacaoController(IMediator mediator, ILogger<CriacaoController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("gerar-imagem")]
        public async Task<IActionResult> GerarImagem([FromForm] GerarImagemRequest gerarImagem)
        {
            try
            {
                var command = new GerarImagemCommand(gerarImagem.Descricao, gerarImagem.Arquivo);
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
                var query = new ListarCriacoesQuery(busca);
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
                var query = new ObterCriacaoQuery(id);
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
