using simulador_de_banco.Domain.Entidade;

namespace simulador_de_banco.Application.Interface.IInfrastructure.Integration.IAntifraude
{
    public interface IAntifraudeServices
    {
        public Task<ResultadoAntiFraude> AntifraudeVerificaTransacao(Antifraude antifraudeRequest, CancellationToken cancellationToken);
    }
}
