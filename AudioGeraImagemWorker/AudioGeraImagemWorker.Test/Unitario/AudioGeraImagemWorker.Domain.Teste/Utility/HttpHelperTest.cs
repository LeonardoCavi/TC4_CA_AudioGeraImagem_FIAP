using AudioGeraImagemWorker.Domain.Interfaces.Utility;
using AudioGeraImagemWorker.Domain.Services.ProcessamentoHandler;
using AudioGeraImagemWorker.Domain.Utility;
using AutoFixture;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioGeraImagemWorker.Test.Unitario.AudioGeraImagemWorker.Domain.Teste.Utility
{
    public class HttpHelperTest
    {
        private readonly IFixture _fixture;
        private readonly ILogger<HttpHelper> _loggerMock;
        private readonly IAsyncPolicy _resiliencePolicy;

        public HttpHelperTest()
        {
            _fixture = new Fixture();

            _loggerMock = Substitute.For<ILogger<HttpHelper>>();
            _resiliencePolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(new[]
               {
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3)
               });

            _fixture.Register(() => _loggerMock);
            _fixture.Register(() => _resiliencePolicy);
        }

        [Fact]
        public async Task GetBytes_Sucesso()
        {
            //var helper = _fixture.Create<HttpHelper>();
            //var resultado = await helper.GetBytes("url");
        }
    }
}
