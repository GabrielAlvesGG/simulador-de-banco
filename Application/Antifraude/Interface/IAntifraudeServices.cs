using simulador_de_banco.Application.Antifraude.Models;
using simulador_de_banco.Domain.Entidade;

namespace simulador_de_banco.Application.Antifraude.Interface
{
    public interface IAntifraudeServices
    {
        public Task<ResultadoAntiFraude> AntifraudeVerificaTransacao(AntifraudeConsulta antifraudeRequest, CancellationToken cancellationToken);
    }
}
