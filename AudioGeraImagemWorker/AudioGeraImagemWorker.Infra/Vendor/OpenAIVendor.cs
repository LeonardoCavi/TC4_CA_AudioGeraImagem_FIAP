using AudioGeraImagemWorker.Domain.Interfaces.Vendor;
using AudioGeraImagemWorker.Domain.Utility;
using AudioGeraImagemWorker.Infra.Vendor.Entities.OpenAI;
using AudioGeraImagemWorker.Infra.Vendor.Entities.OpenAI.Response;
using System.Text.Json;

namespace AudioGeraImagemWorker.Infra.Vendor
{
    public class OpenAIVendor : IOpenAIVendor
    {
        private readonly HttpHelper _httpHelper;
        private readonly OpenAIParameters _parameters;

        public OpenAIVendor(HttpHelper httpHelper,
                            OpenAIParameters parameters)
        {
            _httpHelper = httpHelper;
            _parameters = parameters;
        }

        public async Task<string> GerarImagem(string prompt)
        {
            var body = new
            {
                model = _parameters.ImageGeneratorParameters.Model,
                prompt,
                n = 1,
                size = "1024x1024"
            };

            var result = await _httpHelper.Send(_parameters.ImageGeneratorParameters.Url, VerboHttp.Post, body, AddHeaders());

            if (result.Code != CodeHttp.Success)
                throw GerarException(JsonSerializer.Deserialize<ErrorResponse>(result.Received));

            var response = JsonSerializer.Deserialize<GerarImagemResponse>(result.Received);
            return response.data.First().url;
        }

        public async Task<string> GerarTranscricao(byte[] bytes)
        {
            MultipartFormDataContent body = new()
            {
                { new StringContent( _parameters.TranscriptionParameters.Model), "model" },
                { new ByteArrayContent(bytes), "file", "audio.mp3" }
            };

            var result = await _httpHelper.Send(_parameters.TranscriptionParameters.Url, VerboHttp.Post, body, AddHeaders());

            if (result.Code != CodeHttp.Success)
                throw GerarException(JsonSerializer.Deserialize<ErrorResponse>(result.Received));

            var response = JsonSerializer.Deserialize<GerarTranscricaoResponse>(result.Received);
            return response.text;
        }

        private Exception GerarException(ErrorResponse response) => new($"{response.error.code} - {response.error.type} - {response.error.message} - {response.error.param}");

        private Dictionary<string, string> AddHeaders() => new () { { "Authorization", $"Bearer {_parameters.SecretKey}" } };

    }
}