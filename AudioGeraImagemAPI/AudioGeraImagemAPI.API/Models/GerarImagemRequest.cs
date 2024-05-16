namespace AudioGeraImagemAPI.API.Models
{
    public class GerarImagemRequest
    {
        public string Descricao { get; set; }
        public IFormFile Arquivo { get; set; }
    }
}
