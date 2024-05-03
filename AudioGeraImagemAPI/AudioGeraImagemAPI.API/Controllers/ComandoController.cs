using AudioGeraImagemAPI.Application.ViewModels;
using AudioGeraImagemAPI.UseCases.Comandos.Create;
using AudioGeraImagemAPI.UseCases.Comandos.Get;
using AudioGeraImagemAPI.UseCases.Comandos.List;
using AudioGeraImagemAPI.UseCases.Imagens.Get;
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
        public async Task<IActionResult> GerarImagem([FromForm] GerarImagemViewModel gerarImagem)
        {
            try
            {
                _logger.LogInformation($"[{ClassName}] - [GerarImagem] => Request.: {gerarImagem.Descricao} - {gerarImagem.Arquivo.FileName}");
                var resultado = await _mediator.Send(new CriarComandoCommand() { Descricao = gerarImagem.Descricao, Arquivo = gerarImagem.Arquivo });

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
                _logger.LogInformation($"[{ClassName}] - [BuscarCriacoes] => Request.: {new { Busca = busca }}");

                var resultado = await _mediator.Send(new ListarComandosQuery() { Busca = busca });

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
                _logger.LogInformation($"[{ClassName}] - [ObterCriacao] => Request.: {new { Id = id }}");

                var resultado = await _mediator.Send(new ObterComandoQuery() { Id = id });

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
                _logger.LogInformation($"[{ClassName}] - [ObterImagem] => Request.: {new { Id = id }}");

                var resultado = await _mediator.Send(new ObterImagemQuery() { Id = id });

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
