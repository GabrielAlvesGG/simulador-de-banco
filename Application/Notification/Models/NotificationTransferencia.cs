namespace simulador_de_banco.Application.Notification.Models
{
    public class NotificationTransferencia
    {
        public string? Email { get; set; }
        public string? Nome { get; set; }
        public int ContaDestinoId { get; set; }
        public decimal Valor { get; set; }
        public decimal Saldo { get; set; }
        public Guid TransferenciaId { get; set; }
    }
}
