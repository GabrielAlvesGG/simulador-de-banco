using simulador_de_banco.Application.Antifraude.Interface;
using simulador_de_banco.Application.Antifraude.Models;

namespace simulador_de_banco.Infrastructure.Integrations.AntifraudeClientIntegrations
{
    public class AntifraudeClientIntegrations : IAntifraudeServices
    {
        private readonly HttpClient _httpClient;
        public AntifraudeClientIntegrations(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<ResultadoAntiFraude> AntifraudeVerificaTransacao(AntifraudeConsulta antifraudeRequest, CancellationToken cancellationToken)
        {
            var analiseAntifraude = new
            {
                ContaOrigem = antifraudeRequest.IdContaOrigem,
                ContaDestino = antifraudeRequest.IdContaDestino,
                Valor = antifraudeRequest.Valor,
                DataOperacao = DateTime.UtcNow
            };

            var respostaAntifraude = await _httpClient.PostAsJsonAsync(
                "https://api-antifraude.exemplo.com/analises",
                analiseAntifraude,
                cancellationToken);

            if (!respostaAntifraude.IsSuccessStatusCode)
                throw new InvalidOperationException(
                    "Não foi possível consultar o serviço antifraude.");

            var resultadoAntifraude =
                await respostaAntifraude.Content
                    .ReadFromJsonAsync<ResultadoAntiFraude>(
                        cancellationToken: cancellationToken);

            if (resultadoAntifraude is null ||
                !resultadoAntifraude.Aprovado)
            {
                throw new InvalidOperationException(
                    "Transferência recusada pelo serviço antifraude.");
            }

            return resultadoAntifraude; 
        }
    }
}
