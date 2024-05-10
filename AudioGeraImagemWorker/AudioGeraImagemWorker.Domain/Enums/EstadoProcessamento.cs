namespace AudioGeraImagemWorker.Domain.Enums
{
    public enum EstadoProcessamento
    {
        Recebido,
        SalvandoAudio,
        GerandoTexto,
        GerandoImagem,
        SalvadoImagem,
        Finalizado,
        Falha
    }
}