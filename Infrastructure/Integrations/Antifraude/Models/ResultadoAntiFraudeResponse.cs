namespace simulador_de_banco.Infrastructure.Integrations.Antifraude.Models
{
    public class ResultadoAntiFraudeResponse
    {
        public bool Aprovado { get; set; }
        public string? Motivo { get; set; }
    }
}
