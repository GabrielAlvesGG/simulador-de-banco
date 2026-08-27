namespace simulador_de_banco.Application.Extrato.Models
{
    public class ExtratoBancarioTransacao
    {
        public Guid TransferenciaId { get; set; }
        public int ContaOrigemId { get; set; }
        public int ContaDestinoId { get; set; }
        public decimal Valor { get; set; }
        public decimal SaldoAnteriorContaOrigem { get; set; }
        public decimal SaldoNovoContaOrigem { get; set; }

    }
}
