namespace simulador_de_banco.Application.DTO
{
    public class TransacaoRequestDto
    {
        public int IdContaOrigem { get; set; }
        public int IdContaDestino { get; set; }
        public decimal Valor { get; set; }
    }
}
