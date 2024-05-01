using AudioGeraImagemAPI.Application.Intefaces;
using AudioGeraImagemAPI.Application.ViewModels;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace AudioGeraImagemAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComandoController : ControllerBase
    {
        private readonly IComandoApplicationService _applicationService;
        private readonly ILogger<ComandoController> _logger;
        private readonly string ClassName = typeof(ComandoController).Name;

        public ComandoController(IComandoApplicationService applicationService, ILogger<ComandoController> logger)
        {
            _applicationService = applicationService;
            _logger = logger;
        }

        [HttpPost("gerar-imagem")]
        public async Task<IActionResult> GerarImagem([FromForm] GerarImagemViewModel gerarImagem)
        {
            try
            {
                _logger.LogInformation($"[{ClassName}] - [GerarImagem] => Request.: {gerarImagem.Descricao} - {gerarImagem.Arquivo.FileName}");

                var resultado = await _applicationService.GerarImagem(gerarImagem);

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

                var resultado = await _applicationService.BuscarCriacoes(busca);

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

                var resultado = await _applicationService.ObterCriacao(id);

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

                var resultado = await _applicationService.ObterImagem(id);

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
