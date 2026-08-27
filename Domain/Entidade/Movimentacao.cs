namespace simulador_de_banco.Domain.Entidade
{
    public class Movimentacao
    {
        public Guid Id { get; private set; }
        public int ContaOrigemId { get; private set; }
        public int ContaDestinoId { get; private set; }
        public decimal Valor { get; private set; }
        public DateTime DataMovimentacao { get; private set; }
        public string Tipo { get; private set; }
        public string Status { get; private set; }

        public Movimentacao(
            int contaOrigemId,
            int contaDestinoId,
            decimal valor)
        {
            if (valor <= 0)
                throw new ArgumentException(
                    "O valor deve ser maior que zero.");

            Id = Guid.NewGuid();
            ContaOrigemId = contaOrigemId;
            ContaDestinoId = contaDestinoId;
            Valor = valor;
            DataMovimentacao = DateTime.UtcNow;
            Tipo = "TRANSFERENCIA";
            Status = "CONCLUIDA";
        }
    }
}
