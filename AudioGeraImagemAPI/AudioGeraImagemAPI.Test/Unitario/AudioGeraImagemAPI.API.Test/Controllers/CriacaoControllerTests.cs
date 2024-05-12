using AudioGeraImagemAPI.API.Controllers;
using AudioGeraImagemAPI.API.Models;
using AudioGeraImagemAPI.Domain.Entities;
using AudioGeraImagemAPI.Domain.Enums;
using AudioGeraImagemAPI.Domain.Utility.DTO;
using AudioGeraImagemAPI.UseCases.Criacoes;
using AudioGeraImagemAPI.UseCases.Criacoes.GerarImagem;
using AudioGeraImagemAPI.UseCases.Criacoes.Listar;
using AudioGeraImagemAPI.UseCases.Criacoes.Obter;
using AudioGeraImagemAPI.UseCases.Criacoes.ObterImagem;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Net;
using System.Text;

namespace AudioGeraImagemAPI.Test.Unitario.AudioGeraImagemAPI.API.Test.Controllers
{
    public class CriacaoControllerTests
    {
        private readonly IMediator _mediator = Substitute.For<IMediator>();
        private readonly ILogger<CriacaoController> _logger = Substitute.For<ILogger<CriacaoController>>();

        public CriacaoControllerTests()
        { }

        #region [GerarImagem]
        
        [Fact]
        public async Task GerarImagem_Teste_Sucesso()
        {
            // Arrange
            var bytes = Encoding.UTF8.GetBytes("This is a dummy file");
            IFormFile file = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "dummy.mp3");
            var geraImagemMock = new GerarImagemRequest
            {
                Descricao = "Apenas um teste",
                Arquivo = file
            };

            var guid = Guid.NewGuid();
            var resultadoOperacao = new ResultadoOperacao<Guid>(true, string.Empty, guid);
            _mediator.Send(Arg.Any<GerarImagemCommand>()).Returns(resultadoOperacao);
            var controller = new CriacaoController(_mediator, _logger);

            // Act
            var resultado = await controller.GerarImagem(geraImagemMock);

            // Assert
            var acceptedResult = resultado as AcceptedResult;
            var resultadoGuid = (Guid)acceptedResult.Value;
            
            Assert.NotNull(acceptedResult);
            Assert.NotNull(resultadoGuid);
            Assert.Equal(guid, resultadoGuid);
            Assert.Equal((int)HttpStatusCode.Accepted, acceptedResult.StatusCode);
        }

        [Fact]
        public async Task GerarImagem_Teste_Falha()
        {
            // Arrange
            var bytes = Encoding.UTF8.GetBytes("This is a dummy file");
            IFormFile file = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "dummy.mp3");
            var geraImagemMock = new GerarImagemRequest
            {
                Descricao = "Apenas um teste",
                Arquivo = file
            };

            var guid = Guid.NewGuid();
            var resultadoOperacao = new ResultadoOperacao<Guid>(false, "Mock de falha", guid);
            _mediator.Send(Arg.Any<GerarImagemCommand>()).Returns(resultadoOperacao);
            var controller = new CriacaoController(_mediator, _logger);

            // Act
            var resultado = await controller.GerarImagem(geraImagemMock);
            var badRequestObjectResult = resultado as BadRequestObjectResult;

            // Assert
            Assert.NotNull(badRequestObjectResult);
            Assert.Equal((int)HttpStatusCode.BadRequest, badRequestObjectResult.StatusCode);
        }
        
        [Fact]
        public async Task GerarImagem_Teste_Exception()
        {
            // Arrange
            var bytes = Encoding.UTF8.GetBytes("This is a dummy file");
            IFormFile file = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "dummy.mp3");
            var geraImagemMock = new GerarImagemRequest
            {
                Descricao = "Apenas um teste",
                Arquivo = file
            };

            var guid = Guid.NewGuid();
            var resultadoOperacao = new ResultadoOperacao<Guid>(true, string.Empty, guid);
            _mediator.Send(Arg.Any<GerarImagemCommand>()).Throws(new Exception("Exception do teste"));
            var controller = new CriacaoController(_mediator, _logger);

            // Act
            var resultado = await controller.GerarImagem(geraImagemMock);
            var objectResult = resultado as ObjectResult;
            
            // Assert
            Assert.NotNull(objectResult);
            Assert.Equal((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);
        }

        #endregion

        #region [BuscarCriacoes]

        [Fact]
        public async Task BuscarCriacoes_Teste_Sucesso()
        {
            // Arrange
            var busca = "teste";

            var buscaMock = new List<ListarCriacoesDTO>
            {
                new ListarCriacoesDTO
                {
                    Id = Guid.NewGuid(),
                    Descricao = "teste mock",
                    Transcricao = "Apenas um teste unitario com dados mockados",
                    InstanteCriacao = DateTime.Now,
                    InstanteAtualizacao = DateTime.Now,
                    UltimoEstado = EstadoProcessamento.Finalizado
                }
            };
            IEnumerable<ListarCriacoesDTO> buscaMockEnumerable = buscaMock.OfType<ListarCriacoesDTO>();

            var resultadoOperacao = new ResultadoOperacao<IEnumerable<ListarCriacoesDTO>>(true, string.Empty, buscaMockEnumerable);

            _mediator.Send(Arg.Any<ListarCriacoesQuery>()).Returns(resultadoOperacao);
            var controller = new CriacaoController(_mediator, _logger);

            // Act
            var resultado = await controller.BuscarCriacoes(busca);
            var okObjectResult = resultado as OkObjectResult;

            // Assert
            Assert.NotNull(okObjectResult);
            Assert.Equal(buscaMockEnumerable, okObjectResult.Value);
            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
        }

        [Fact]
        public async Task BuscarCriacoes_Teste_Falha()
        {
            // Arrange
            var busca = "";
            var resultadoOperacao = new ResultadoOperacao<IEnumerable<ListarCriacoesDTO>>(false, "Mock de falha", null);

            _mediator.Send(Arg.Any<ListarCriacoesQuery>()).Returns(resultadoOperacao);
            var controller = new CriacaoController(_mediator, _logger);

            // Act
            var resultado = await controller.BuscarCriacoes(busca);
            var notFoundObjectResult = resultado as NotFoundObjectResult;

            // Assert
            Assert.NotNull(notFoundObjectResult);
            Assert.Equal((int)HttpStatusCode.NotFound, notFoundObjectResult.StatusCode);
        }

        [Fact]
        public async Task TaskBuscarCriacoes_Teste_Exception()
        {
            // Arrange
            var busca = "teste";

            _mediator.Send(Arg.Any<ListarCriacoesQuery>()).Throws(new Exception("Exception do teste"));
            var controller = new CriacaoController(_mediator, _logger);

            // Act
            var resultado = await controller.BuscarCriacoes(busca);
            var objectResult = resultado as ObjectResult;

            // Assert
            Assert.NotNull(objectResult);
            Assert.Equal((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);
        }

        #endregion

        #region [ObterCriacao]

        [Fact]
        public async Task ObterCriacao_Teste_Sucesso()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var id = guid.ToString();
            var obterCriacaoMock = new CriacaoDTO
            {
                Id = guid,
                Descricao = "teste mock",
                UrlAudio = "https://minhaconta.blob.core.windows.net/audio/audio.mp3",
                Transcricao = "Apenas um teste unitario com dados mockados",
                UrlImagem = "https://minhaconta.blob.core.windows.net/imagem/imagem.jpg",
                InstanteCriacao = DateTime.Now,
                InstanteAtualizacao = DateTime.Now,
                ProcessamentosCriacao = new List<ProcessamentoCriacao>
                {
                    new()
                    {
                        Estado = EstadoProcessamento.Finalizado,
                        InstanteCriacao = DateTime.Now
                    }
                }
            };

            var resultadoOperacao = new ResultadoOperacao<CriacaoDTO>(true, string.Empty, obterCriacaoMock);
            _mediator.Send(Arg.Any<ObterCriacaoQuery>()).Returns(resultadoOperacao);
            var controller = new CriacaoController(_mediator, _logger);

            // Act
            var resultado = await controller.ObterCriacao(id);
            var okObjectResult = resultado as OkObjectResult;

            // Assert
            Assert.NotNull(okObjectResult);
            Assert.Equal(obterCriacaoMock, okObjectResult.Value);
            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
        }

        [Fact]
        public async Task ObterCriacao_Teste_Falha()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var id = guid.ToString();

            var resultadoOperacao = new ResultadoOperacao<CriacaoDTO>(false, "Mock de falha", null);
            _mediator.Send(Arg.Any<ObterCriacaoQuery>()).Returns(resultadoOperacao);
            var controller = new CriacaoController(_mediator, _logger);

            // Act
            var resultado = await controller.ObterCriacao(id);
            var notFoundObjectResult = resultado as NotFoundObjectResult;

            // Assert
            Assert.NotNull(notFoundObjectResult);
            Assert.Equal((int)HttpStatusCode.NotFound, notFoundObjectResult.StatusCode);
        }

        [Fact]
        public async Task ObterCriacao_Teste_Exception()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var id = guid.ToString();

            _mediator.Send(Arg.Any<ListarCriacoesQuery>()).Throws(new Exception("Exception do teste"));
            var controller = new CriacaoController(_mediator, _logger);

            // Act
            var resultado = await controller.ObterCriacao(id);
            var objectResult = resultado as ObjectResult;

            // Assert
            Assert.NotNull(objectResult);
            Assert.Equal((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);
        }

        #endregion

        #region [ObterImagem]

        [Fact]
        public async Task ObterImagem_Teste_Sucesso()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var id = guid.ToString();
            using (Stream stream = new MemoryStream()) 
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write("teste");
                    writer.Flush();

                    var resultadoOperacao = new ResultadoOperacao<Stream>(true, string.Empty, stream);
                    _mediator.Send(Arg.Any<ObterImagemQuery>()).Returns(resultadoOperacao);
                    var controller = new CriacaoController(_mediator, _logger);

                    // Act
                    var resultado = await controller.ObterImagem(id);
                    var fileStreamResult = resultado as FileStreamResult;

                    // Assert
                    Assert.NotNull(fileStreamResult);
                    Assert.Equal(stream, fileStreamResult.FileStream);
                    Assert.Equal("image/jpeg", fileStreamResult.ContentType);
                }
            }                           
        }

        [Fact]
        public async Task ObterImagem_Teste_Falha()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var id = guid.ToString();

            using (Stream stream = new MemoryStream())
            {
                var resultadoOperacao = new ResultadoOperacao<Stream>(false, "Mock de falha", stream);
                _mediator.Send(Arg.Any<ObterImagemQuery>()).Returns(resultadoOperacao);
                var controller = new CriacaoController(_mediator, _logger);
                
                // Act
                var resultado = await controller.ObterImagem(id);
                var badRequestObjectResult = resultado as BadRequestObjectResult;

                // Assert
                Assert.NotNull(badRequestObjectResult);
                Assert.Equal((int)HttpStatusCode.BadRequest, badRequestObjectResult.StatusCode);
            }
        }

        [Fact]
        public async Task ObterImagem_Teste_Exception()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var id = guid.ToString();

            using (Stream stream = new MemoryStream())
            {
                var resultadoOperacao = new ResultadoOperacao<Stream>(false, "Mock de falha", stream);
                _mediator.Send(Arg.Any<ObterImagemQuery>()).Throws(new Exception("Exception do teste"));
                var controller = new CriacaoController(_mediator, _logger);

                // Act
                var resultado = await controller.ObterImagem(id);
                var objectResult = resultado as ObjectResult;

                // Assert
                Assert.NotNull(objectResult);
                Assert.Equal((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);
            }
        }

        #endregion
    }
}