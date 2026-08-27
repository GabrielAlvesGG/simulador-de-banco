using simulador_de_banco.Application.Antifraude.Models;

namespace simulador_de_banco.Application.Antifraude.Interface
{
    public interface IAntifraudeServices
    {
        public Task<ResultadoAntiFraude> AntifraudeVerificaTransacao(AntifraudeConsulta antifraudeRequest, CancellationToken cancellationToken);
    }
}
