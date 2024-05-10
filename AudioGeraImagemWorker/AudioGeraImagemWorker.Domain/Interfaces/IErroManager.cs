﻿using AudioGeraImagem.Domain.Entities;
using AudioGeraImagemWorker.Domain.Entities;
using AudioGeraImagemWorker.Domain.Enums;

namespace AudioGeraImagemWorker.Domain.Interfaces
{
    public interface IErroManager
    {
        Task TratarErro(Criacao criacao, EstadoProcessamento ultimoEstado, byte[] payload);
    }
}