using AudioGeraImagemWorker.Domain.Entities;
using AudioGeraImagemWorker.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioGeraImagemWorker.Domain.Factories
{
    public static class ProcessamentoComandoFactory
    {
        public static ProcessamentoComando Novo(EstadoComando estado)
        {
            return new()
            {
                Estado = estado,
                InstanteCriacao = DateTime.Now
            };
        }
    }
}
