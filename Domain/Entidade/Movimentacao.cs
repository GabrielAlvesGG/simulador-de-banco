namespace simulador_de_banco.Domain.Entidade
{
    public class Movimentacao
    {
            public int ContaOrigemId { get;set; }
            public int ContaDestinoId { get; set; }
            public decimal Valor { get; set; }
    }
}
