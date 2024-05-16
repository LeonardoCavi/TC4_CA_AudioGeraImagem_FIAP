using System.Net;

namespace AudioGeraImagemAPI.Test.Fake
{
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly string _response;
        private readonly HttpStatusCode _statusCode;
        private readonly string _exceptionMessage;
        public MockHttpMessageHandler(string response, HttpStatusCode statusCode, string exceptionMessage = "")
        {
            _response = response;
            _statusCode = statusCode;
            _exceptionMessage = exceptionMessage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {

            if (!string.IsNullOrEmpty(_exceptionMessage))
                throw new Exception(_exceptionMessage);

            return await Task.FromResult(new HttpResponseMessage
            {
                StatusCode = _statusCode,
                Content = new StringContent(_response)
            });
        }
    }
}