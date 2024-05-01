using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AudioGeraImagemAPI.Domain.Utility
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

    public class HttpHelper
    {
        private readonly ILogger<HttpHelper> _logger;
        private readonly string className = typeof(HttpHelper).Name;
        private readonly AsyncPolicy _resiliencePolicy;

        public HttpHelper(ILogger<HttpHelper> logger,
                        AsyncPolicy resiliencePolicy)
        {
            _logger = logger;
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
            using (var httpClient = new HttpClient())
            {
                HttpResponseMessage result = null;
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
                            var jsonRequest = JsonSerializer.Serialize(body);
                            StringContent contentPost = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            result = await httpClient.PostAsync(url, contentPost);
                            contentPost.Dispose();
                        }
                        break;

                    case VerboHttp.Put:
                        if (body is null)
                        {
                            result = await httpClient.PutAsync(url, null);
                        }
                        else
                        {
                            var jsonRequest = JsonSerializer.Serialize(body);
                            StringContent contentPut = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            result = await httpClient.PutAsync(url, contentPut);
                            contentPut.Dispose();
                        }
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
            string content = string.Empty;

            try
            {
                content = await result.Content.ReadAsStringAsync();

                switch (result.StatusCode)
                {
                    case HttpStatusCode.OK:
                        response.Code = CodeHttp.Success;
                        response.Received = content;
                        return response;

                    case HttpStatusCode.Created:
                        response.Code = CodeHttp.Success;
                        response.Received = content;
                        return response;

                    case HttpStatusCode.Accepted:
                        response.Code = CodeHttp.Success;
                        response.Received = content;
                        return response;

                    case HttpStatusCode.NoContent:
                        response.Code = CodeHttp.Success;
                        response.Received = content;
                        return response;

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
            catch (Exception ex)
            {
                response.Code = CodeHttp.ServerError;
                response.Received = ex.ToString();
                return response;
            }
            finally
            {
                _logger.LogInformation($"{className}: ProcessAndAnalyzeResponse{Environment.NewLine}URL:{uri}{Environment.NewLine}Http Status Code:{result.StatusCode}{Environment.NewLine}Content:{content}");
                result.Dispose();
            }
        }
    }
}
