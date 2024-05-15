using System.Net;

namespace AudioGeraImagemWorker.Test.Fake
{
    internal class MockHttpMessageHandler: HttpMessageHandler
    {
        private readonly string _response;
        private readonly HttpStatusCode _statusCode;
        public MockHttpMessageHandler(string response, HttpStatusCode statusCode)
        {
            _response = response;
            _statusCode = statusCode;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return new HttpResponseMessage
            {
                StatusCode = _statusCode,
                Content = new StringContent(_response)
            };
        }
    }
}
