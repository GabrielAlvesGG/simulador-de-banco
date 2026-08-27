namespace simulador_de_banco.Domain.Entidade
{
    public class ContaCorrente
    {
        public int Id { get; set; }
        public string Numero { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal Saldo { get; set; }
        public bool Ativa { get; set; }
    }
}
