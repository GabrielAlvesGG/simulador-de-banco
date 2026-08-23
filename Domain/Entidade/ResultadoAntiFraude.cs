namespace simulador_de_banco.Domain.Entidade
{
    public class ResultadoAntiFraude
    {
        public bool Aprovado { get; set; }
        public string? Motivo { get; set; }
    }
}
