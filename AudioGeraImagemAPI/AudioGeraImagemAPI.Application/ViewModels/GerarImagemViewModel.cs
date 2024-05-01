using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioGeraImagemAPI.Application.ViewModels
{
    public class GerarImagemViewModel
    {
        public string Descricao { get; set; }
        public IFormFile Arquivo { get; set; }
    }
}
