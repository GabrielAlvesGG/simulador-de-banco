namespace simulador_de_banco.Application.DTO
{
    public class ResultadoTransferencia
    {
        public Guid TransferenciaId { get; set; }
        public int ContaOrigemId { get; set; }
        public int ContaDestinoId { get; set; }
        public decimal Valor { get; set; }
        public decimal SaldoAnterior { get; set; }
        public decimal SaldoAtual { get; set; }
        public DateTime DataTransferencia { get; set; }
        public string CaminhoExtrato { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
