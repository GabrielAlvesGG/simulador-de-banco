namespace simulador_de_banco.Application.Interface.IInfrastructure.Integration.Antifraude
{
    public interface IAntifraudeServices
    {
        public Task AntifraudeVerificaTransacao(int contaOrigemId, int contaDestinoId, decimal valor, CancellationToken cancellationToken);
    }
}
