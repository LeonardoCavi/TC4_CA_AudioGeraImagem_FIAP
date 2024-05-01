namespace AudioGeraImagemWorker.Domain.Enums
{
    public enum EstadoComando
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