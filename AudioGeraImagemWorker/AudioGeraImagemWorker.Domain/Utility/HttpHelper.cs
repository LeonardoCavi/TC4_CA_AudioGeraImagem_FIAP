﻿using AudioGeraImagemWorker.Domain.Interfaces.Utility;
using Microsoft.Extensions.Logging;
using Polly;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace AudioGeraImagemWorker.Domain.Utility
{
    public enum VerboHttp
    {
        Get,
        Post,
        Put,
        Delete
    }

    public enum CodeHttp
    {
        Success,
        BadRequest,
        ServerError,
        Others
    }

    public class Response
    {
        public CodeHttp Code { get; set; }
        public string Received { get; set; }
    }

    public class HttpHelper : IHttpHelper
    {
        private readonly string className = typeof(HttpHelper).Name;
        private readonly ILogger<HttpHelper> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IAsyncPolicy _resiliencePolicy;

        public HttpHelper(ILogger<HttpHelper> logger,
                        IHttpClientFactory httpClientFactory,
                        IAsyncPolicy resiliencePolicy)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _resiliencePolicy = resiliencePolicy;
        }

        public async Task<byte[]> GetBytes(string url)
        {
            using var result = await _resiliencePolicy.ExecuteAsync(async () =>
            {
                return await ExternalIntegration(url, VerboHttp.Get, string.Empty);
            });

            if (!result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                throw new Exception($"{result.StatusCode} - {content}");
            }

            return await result.Content.ReadAsByteArrayAsync();
        }

        public async Task<Response> Send<T>(string url,
                                         VerboHttp verboHttp,
                                         T body,
                                         Dictionary<string, string> headers = null)
        {
            try
            {
                using var result = await _resiliencePolicy.ExecuteAsync(async () =>
                {
                    return await ExternalIntegration(url, verboHttp, body, headers);
                });

                var response = await ProcessAndAnalyzeResponse(result, url);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{className} - url:{url} Send Ex:{ex}");

                Response rtd = new Response
                {
                    Received = null,
                    Code = CodeHttp.ServerError
                };

                return rtd;
            }
        }
        private async Task<HttpResponseMessage> ExternalIntegration<T>(string url,
                                                                    VerboHttp verboHttp,
                                                                    T body,
                                                                    Dictionary<string, string> headers = null)
        {
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                HttpResponseMessage result = null;
                var jsonRequest = string.Empty;

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                switch (verboHttp)
                {
                    case VerboHttp.Get:
                        result = await httpClient.GetAsync(url);
                        break;

                    case VerboHttp.Post:
                        if (body is MultipartContent multipartContent)
                            result = await httpClient.PostAsync(url, multipartContent);
                        else
                        {
                            jsonRequest = JsonSerializer.Serialize(body);
                            StringContent contentPost = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            result = await httpClient.PostAsync(url, contentPost);
                            contentPost.Dispose();
                        }
                        break;

                    case VerboHttp.Put:
                        jsonRequest = JsonSerializer.Serialize(body);
                        StringContent contentPut = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                        result = await httpClient.PutAsync(url, contentPut);
                        contentPut.Dispose();
                        break;

                    case VerboHttp.Delete:
                        result = await httpClient.DeleteAsync(url);
                        break;
                }

                return result;
            }
        }

        private async Task<Response> ProcessAndAnalyzeResponse(HttpResponseMessage result, string uri)
        {
            Response response = new Response();
            string content = await result.Content.ReadAsStringAsync();

            switch (result.StatusCode)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.Created:
                case HttpStatusCode.Accepted:
                case HttpStatusCode.NoContent:
                case HttpStatusCode.ResetContent:
                    response.Code = CodeHttp.Success;
                    response.Received = content;
                    return response;

                case HttpStatusCode.BadRequest:
                    response.Code = CodeHttp.BadRequest;
                    response.Received = content;
                    return response;

                case HttpStatusCode.InternalServerError:
                    response.Code = CodeHttp.ServerError;
                    response.Received = content;
                    return response;

                default:
                    response.Code = CodeHttp.Others;
                    response.Received = content;
                    return response;
            }
        }
    }
}
