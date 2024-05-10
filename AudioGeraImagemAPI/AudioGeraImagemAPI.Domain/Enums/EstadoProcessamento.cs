namespace AudioGeraImagemAPI.Domain.Enums
{
    public enum EstadoProcessamento
    {
        Recebido,
        SalvandoAudio,
        GerandoTexto,
        SalvandoTexto,
        GerandoImagem,
        SalvadoImagem,
        Finalizado,
        Falha
    }
}
