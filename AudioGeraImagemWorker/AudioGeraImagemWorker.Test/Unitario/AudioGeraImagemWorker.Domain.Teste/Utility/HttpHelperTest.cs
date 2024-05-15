using AudioGeraImagemWorker.Domain.DTOs;
using AudioGeraImagemWorker.Domain.Utility;
using AudioGeraImagemWorker.Test.Fake;
using AutoFixture;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Polly;
using System.Net;
using System.Text;
using System.Text.Json;

namespace AudioGeraImagemWorker.Test.Unitario.AudioGeraImagemWorker.Domain.Teste.Utility
{
    public class HttpHelperTest
    {
        private readonly IFixture _fixture;
        private readonly ILogger<HttpHelper> _loggerMock;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IAsyncPolicy _resiliencePolicy;
        private string url = "http://url_teste";

        public HttpHelperTest()
        {
            _fixture = new Fixture();

            _loggerMock = Substitute.For<ILogger<HttpHelper>>();
            _httpClientFactory = Substitute.For<IHttpClientFactory>();
            _resiliencePolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(new[]
               {
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3)
               });

            _fixture.Register(() => _loggerMock);
            _fixture.Register(() => _httpClientFactory);
            _fixture.Register(() => _resiliencePolicy);
        }

        [Fact]
        public async Task GetBytes_Sucesso()
        {
            //Arrange
            var responseJson = JsonSerializer.Serialize(new { Teste = 1 });

            var messageHandler = new MockHttpMessageHandler(responseJson, HttpStatusCode.OK);

            var httpClientMock = new HttpClient(messageHandler);

            _httpClientFactory.CreateClient()
                .Returns(httpClientMock);

            var httpHelper = _fixture.Create<HttpHelper>();

            //Act
            var resultado = await httpHelper.GetBytes(url);

            //Assert
            Assert.Equal(Encoding.UTF8.GetBytes(responseJson), resultado);
        }

        [Fact]
        public async Task GetBytes_Falha()
        {
            //Arrange
            var url = "http://url_teste";
            var responseJson = JsonSerializer.Serialize("Teste");
            var messageHandler = new MockHttpMessageHandler(responseJson, HttpStatusCode.InternalServerError);

            var httpClientMock = new HttpClient(messageHandler);

            _httpClientFactory.CreateClient()
                .Returns(httpClientMock);

            var httpHelper = _fixture.Create<HttpHelper>();

            //Act
            var exception = await Assert.ThrowsAsync<Exception>(() => httpHelper.GetBytes(url));

            //Assert
            Assert.Equal("InternalServerError - \"Teste\"", exception.Message);
        }

        [Fact]
        public async Task Send_GetAsync_OK()
        {
            //Arrange
            var responseJson = JsonSerializer.Serialize(new { Teste = 1 });

            var messageHandler = new MockHttpMessageHandler(responseJson, HttpStatusCode.OK);

            var httpClientMock = new HttpClient(messageHandler);

            _httpClientFactory.CreateClient()
                .Returns(httpClientMock);

            var httpHelper = _fixture.Create<HttpHelper>();

            //Act
            var resultado = await httpHelper.Send(url, VerboHttp.Get, string.Empty);

            //Assert
            Assert.Equal(CodeHttp.Success, resultado.Code);
            Assert.Equal(responseJson, resultado.Received);
        }

        [Fact]
        public async Task Send_GetAsync_NotFound()
        {
            //Arrange
            var responseJson = JsonSerializer.Serialize(new { Teste = 1 });

            var messageHandler = new MockHttpMessageHandler(responseJson, HttpStatusCode.NotFound);

            var httpClientMock = new HttpClient(messageHandler);

            _httpClientFactory.CreateClient()
                .Returns(httpClientMock);

            var httpHelper = _fixture.Create<HttpHelper>();

            //Act
            var resultado = await httpHelper.Send(url, VerboHttp.Get, string.Empty);

            //Assert
            Assert.Equal(CodeHttp.Others, resultado.Code);
            Assert.Equal(responseJson, resultado.Received);
        }

        [Fact]
        public async Task Send_GetAsync_Exception()
        {
            //Arrange
            var responseJson = JsonSerializer.Serialize(new { Teste = 1 });

            var messageHandler = new MockHttpMessageHandler(responseJson, HttpStatusCode.ServiceUnavailable, "timeout exception");

            var httpClientMock = new HttpClient(messageHandler);

            _httpClientFactory.CreateClient()
                .Returns(httpClientMock);

            var httpHelper = _fixture.Create<HttpHelper>();

            //Act
            var resultado = await httpHelper.Send(url, VerboHttp.Get, string.Empty);

            //Assert
            Assert.Equal(CodeHttp.ServerError, resultado.Code);
            Assert.Null(resultado.Received);
        }

        [Fact]
        public async Task Send_PostAsync_Created()
        {
            //Arrange
            var headers = new Dictionary<string, string>
            {
                { "Authorization", "Token" }
            };
            var requestJson = new { Teste = 1 };
            var responseJson = JsonSerializer.Serialize(new { Teste = 1 });

            var messageHandler = new MockHttpMessageHandler(responseJson, HttpStatusCode.Created);

            var httpClientMock = new HttpClient(messageHandler);

            _httpClientFactory.CreateClient()
                .Returns(httpClientMock);

            var httpHelper = _fixture.Create<HttpHelper>();

            //Act
            var resultado = await httpHelper.Send(url, VerboHttp.Post, requestJson, headers);

            //Assert
            Assert.Equal(CodeHttp.Success, resultado.Code);
            Assert.Equal(responseJson, resultado.Received);
        }

        [Fact]
        public async Task Send_PostAsync_BadRequest()
        {
            //Arrange
            var headers = new Dictionary<string, string>
            {
                { "Authorization", "Token" }
            };
            var requestJson  = new MultipartFormDataContent()
            {
                { new StringContent( "field"), "field1" }
            };
            var responseJson = JsonSerializer.Serialize(new { Teste = 1 });

            var messageHandler = new MockHttpMessageHandler(responseJson, HttpStatusCode.BadRequest);

            var httpClientMock = new HttpClient(messageHandler);

            _httpClientFactory.CreateClient()
                .Returns(httpClientMock);

            var httpHelper = _fixture.Create<HttpHelper>();

            //Act
            var resultado = await httpHelper.Send(url, VerboHttp.Post, requestJson, headers);

            //Assert
            Assert.Equal(CodeHttp.BadRequest, resultado.Code);
            Assert.Equal(responseJson, resultado.Received);
        }

        [Fact]
        public async Task Send_PutAsync_Accepted()
        {
            //Arrange
            var headers = new Dictionary<string, string>
            {
                { "Authorization", "Token" }
            };
            var requestJson = new { Teste = 1 };
            var responseJson = JsonSerializer.Serialize(new { Teste = 1 });

            var messageHandler = new MockHttpMessageHandler(responseJson, HttpStatusCode.Accepted);

            var httpClientMock = new HttpClient(messageHandler);

            _httpClientFactory.CreateClient()
                .Returns(httpClientMock);

            var httpHelper = _fixture.Create<HttpHelper>();

            //Act
            var resultado = await httpHelper.Send(url, VerboHttp.Put, requestJson, headers);

            //Assert
            Assert.Equal(CodeHttp.Success, resultado.Code);
            Assert.Equal(responseJson, resultado.Received);
        }

        [Fact]
        public async Task Send_PutAsync_NoContent()
        {
            //Arrange
            var headers = new Dictionary<string, string>
            {
                { "Authorization", "Token" }
            };
            var requestJson = new { Teste = 1 };
            var responseJson = JsonSerializer.Serialize(new { Teste = 1 });

            var messageHandler = new MockHttpMessageHandler(responseJson, HttpStatusCode.NoContent);

            var httpClientMock = new HttpClient(messageHandler);

            _httpClientFactory.CreateClient()
                .Returns(httpClientMock);

            var httpHelper = _fixture.Create<HttpHelper>();

            //Act
            var resultado = await httpHelper.Send(url, VerboHttp.Put, requestJson, headers);

            //Assert
            Assert.Equal(CodeHttp.Success, resultado.Code);
            Assert.Equal(responseJson, resultado.Received);
        }

        [Fact]
        public async Task Send_PutAsync_ResetContent()
        {
            //Arrange
            var headers = new Dictionary<string, string>
            {
                { "Authorization", "Token" }
            };
            var requestJson = new { Teste = 1 };
            var responseJson = JsonSerializer.Serialize(new { Teste = 1 });

            var messageHandler = new MockHttpMessageHandler(responseJson, HttpStatusCode.ResetContent);

            var httpClientMock = new HttpClient(messageHandler);

            _httpClientFactory.CreateClient()
                .Returns(httpClientMock);

            var httpHelper = _fixture.Create<HttpHelper>();

            //Act
            var resultado = await httpHelper.Send(url, VerboHttp.Put, requestJson, headers);

            //Assert
            Assert.Equal(CodeHttp.Success, resultado.Code);
            Assert.Equal(responseJson, resultado.Received);
        }

        [Fact]
        public async Task Send_DeleteAsync_InternalServerError()
        {
            //Arrange
            var headers = new Dictionary<string, string>
            {
                { "Authorization", "Token" }
            };
            var responseJson = JsonSerializer.Serialize(new { Teste = 1 });

            var messageHandler = new MockHttpMessageHandler(responseJson, HttpStatusCode.InternalServerError);

            var httpClientMock = new HttpClient(messageHandler);

            _httpClientFactory.CreateClient()
                .Returns(httpClientMock);

            var httpHelper = _fixture.Create<HttpHelper>();

            //Act
            var resultado = await httpHelper.Send(url, VerboHttp.Delete, string.Empty, headers);

            //Assert
            Assert.Equal(CodeHttp.ServerError, resultado.Code);
            Assert.Equal(responseJson, resultado.Received);
        }

        [Fact]
        public async Task Send_DeleteAsync_Others()
        {
            //Arrange
            var headers = new Dictionary<string, string>
            {
                { "Authorization", "Token" }
            };
            var responseJson = JsonSerializer.Serialize(new { Teste = 1 });

            var messageHandler = new MockHttpMessageHandler(responseJson, HttpStatusCode.Conflict);

            var httpClientMock = new HttpClient(messageHandler);

            _httpClientFactory.CreateClient()
                .Returns(httpClientMock);

            var httpHelper = _fixture.Create<HttpHelper>();

            //Act
            var resultado = await httpHelper.Send(url, VerboHttp.Delete, string.Empty, headers);

            //Assert
            Assert.Equal(CodeHttp.Others, resultado.Code);
            Assert.Equal(responseJson, resultado.Received);
        }
    }
}
