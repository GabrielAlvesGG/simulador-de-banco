namespace simulador_de_banco.Application.Antifraude.Models
{
    public class AntifraudeConsulta
    {
        public int IdContaOrigem { get; set; }
        public int IdContaDestino { get; set; }
        public decimal Valor { get; set; }
    }
}
