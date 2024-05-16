using AudioGeraImagemAPI.Domain.Utility;

namespace AudioGeraImagemAPI.Domain.Interfaces.Utility
{
    public interface IHttpHelper
    {
        Task<byte[]> GetBytes(string url);
        Task<Response> Send<T>(string url, VerboHttp verboHttp, T body, Dictionary<string, string> headers = null);
    }
}