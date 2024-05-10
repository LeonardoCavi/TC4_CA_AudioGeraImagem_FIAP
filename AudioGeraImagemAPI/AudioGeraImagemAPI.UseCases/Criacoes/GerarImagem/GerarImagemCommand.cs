using AudioGeraImagemAPI.Domain.Utility.DTO;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace AudioGeraImagemAPI.UseCases.Criacoes.GerarImagem
{
    public class GerarImagemCommand: IRequest<ResultadoOperacao<Guid>>
    {
        public GerarImagemCommand(string descricao, IFormFile arquivo)
        {
            Descricao = descricao;
            Arquivo = arquivo;
        }

        public string Descricao { get; set; }
        public IFormFile Arquivo { get; set; }

        public bool Valido()
        {
            if (Descricao.Length > 256)
                return false;

            if (!Arquivo.ContentType.Contains("audio/mpeg"))
                return false;

            return true;
        }

        public override string ToString() => $"{Descricao} - {Arquivo.FileName}";
    }
}
