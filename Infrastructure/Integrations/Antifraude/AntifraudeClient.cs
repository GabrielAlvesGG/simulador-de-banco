using simulador_de_banco.Application.Interface.IInfrastructure.Integration.Antifraude;
using simulador_de_banco.Infrastructure.Integrations.Antifraude.Models;

namespace simulador_de_banco.Infrastructure.Integrations.Antifraude
{
    public class AntifraudeClient : IAntifraudeServices
    {
        private readonly HttpClient _httpClient;
        public AntifraudeClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task AntifraudeVerificaTransacao(int contaOrigemId,int contaDestinoId,decimal valor, CancellationToken cancellationToken)
        {
            var analiseAntifraude = new
            {
                ContaOrigem = contaOrigemId,
                ContaDestino = contaDestinoId,
                Valor = valor,
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
                    .ReadFromJsonAsync<ResultadoAntiFraudeResponse>(
                        cancellationToken: cancellationToken);

            if (resultadoAntifraude is null ||
                !resultadoAntifraude.Aprovado)
            {
                throw new InvalidOperationException(
                    "Transferência recusada pelo serviço antifraude.");
            }
        }
    }
}
