using AudioGeraImagemWorker.Domain.DTOs;
using AudioGeraImagemWorker.Domain.Interfaces.Utility;
using AudioGeraImagemWorker.Domain.Utility;
using AudioGeraImagemWorker.Infra.Vendor;
using AudioGeraImagemWorker.Infra.Vendor.Entities.OpenAI;
using AudioGeraImagemWorker.Infra.Vendor.Entities.OpenAI.Response;
using AutoFixture;
using Microsoft.Identity.Client;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AudioGeraImagemWorker.Test.Unitario.AudioGeraImagemWorker.Infra.Teste.Vendor
{
    public class OpenAIVendorTest
    {
        private readonly IFixture _fixture;
        private readonly IHttpHelper _httpHelperMock;
        private readonly OpenAIParameters _parameters;

        public OpenAIVendorTest()
        {
            _fixture = new Fixture();
            _httpHelperMock = Substitute.For<IHttpHelper>();
            _parameters = new OpenAIParameters()
            {
                SecretKey = "secret",
                TranscriptionParameters = new()
                {
                    Url = "url_transcriptions",
                    Model = "whisper-1"
                },
                ImageGeneratorParameters = new()
                {
                    Url = "url_generations",
                    Model = "dall-e-3"
                }
            };

            _fixture.Register(() => _httpHelperMock);
            _fixture.Register(() => _parameters);
        }

        [Fact]
        public async Task GerarImagem_Sucesso()
        {
            //Arrange
            var prompt = "teste";

            var gerarImagemResponseMock = new GerarImagemResponse()
            {
                created = 1,
                data = new ()
                {
                    new()
                    {
                        url = "url_imagem"
                    }
                }
            };
            var responseMock = new Response()
            {
                Code = CodeHttp.Success,
                Received = JsonSerializer.Serialize(gerarImagemResponseMock)
            };

            _httpHelperMock.Send(Arg.Any<string>(), Arg.Is(VerboHttp.Post), Arg.Any<object>(), Arg.Any<Dictionary<string, string>>())
                .Returns(responseMock);

            var vendor = _fixture.Create<OpenAIVendor>();

            //Act
            var resultado = await vendor.GerarImagem(prompt);

            //Assert
            Assert.Equal("url_imagem", resultado);
        }

        [Fact]
        public async Task GerarImagem_Erro()
        {
            //Arrange
            var prompt = "teste";

            var errorResponseMock = new ErrorResponse()
            {
                error = new()
                {
                    code = "400",
                    type = "BadRequest",
                    message = "error",
                    param = "fatal"
                }
            };
            var responseMock = new Response()
            {
                Code = CodeHttp.BadRequest,
                Received = JsonSerializer.Serialize(errorResponseMock)
            };

            _httpHelperMock.Send(Arg.Any<string>(), Arg.Is(VerboHttp.Post), Arg.Any<object>(), Arg.Any<Dictionary<string, string>>())
                .Returns(responseMock);

            var vendor = _fixture.Create<OpenAIVendor>();

            //Act
            var exception = await Assert.ThrowsAsync<Exception>(() => vendor.GerarImagem(prompt));

            //Assert
            Assert.Equal("400 - BadRequest - error - fatal", exception.Message);
        }

        [Fact]
        public async Task GerarTrascricao_Sucesso()
        {
            //Arrange
            var bytes = Encoding.UTF8.GetBytes("buffer test");

            var gerarImagemResponseMock = new GerarTranscricaoResponse()
            {
                text = "texto_transcricao"
            };
            var responseMock = new Response()
            {
                Code = CodeHttp.Success,
                Received = JsonSerializer.Serialize(gerarImagemResponseMock)
            };

            _httpHelperMock.Send(Arg.Any<string>(), Arg.Is(VerboHttp.Post), Arg.Any<object>(), Arg.Any<Dictionary<string, string>>())
                .Returns(responseMock);

            var vendor = _fixture.Create<OpenAIVendor>();

            //Act
            var resultado = await vendor.GerarTranscricao(bytes);

            //Assert
            Assert.Equal("texto_transcricao", resultado);
        }

        [Fact]
        public async Task GerarTrascricao_Erro()
        {
            //Arrange
            var bytes = Encoding.UTF8.GetBytes("buffer test");

            var errorResponseMock = new ErrorResponse()
            {
                error = new()
                {
                    code = "400",
                    type = "BadRequest",
                    message = "error",
                    param = "fatal"
                }
            };
            var responseMock = new Response()
            {
                Code = CodeHttp.BadRequest,
                Received = JsonSerializer.Serialize(errorResponseMock)
            };

            _httpHelperMock.Send(Arg.Any<string>(), Arg.Is(VerboHttp.Post), Arg.Any<object>(), Arg.Any<Dictionary<string, string>>())
                .Returns(responseMock);

            var vendor = _fixture.Create<OpenAIVendor>();

            //Act
            var exception = await Assert.ThrowsAsync<Exception>(() => vendor.GerarTranscricao(bytes));

            //Assert
            Assert.Equal("400 - BadRequest - error - fatal", exception.Message);
        }
    }
}
