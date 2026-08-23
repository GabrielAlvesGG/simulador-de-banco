using simulador_de_banco.Application.Interface.IInfrastructure.Storage;

namespace simulador_de_banco.Infrastructure.Storage
{
    public class ExtratoFile : IExtratoServices
    {
        public async Task<string> GerarExtratoTransacao(
            Guid transferenciaId,
            int contaOrigemId,
            int contaDestinoId,
            decimal valor, 
            decimal saldoAnteriorContaOrigem,
            decimal saldoNovoContaOrigem,
            CancellationToken cancellationToken)
        {
            var extrato = $"""
                    BANCO EXEMPLO
                    ----------------------------------
                    COMPROVANTE DE TRANSFERÊNCIA

                    Código: {transferenciaId}
                    Data: {DateTime.Now:dd/MM/yyyy HH:mm:ss}
                    Origem: {contaOrigemId}
                    Destino: {contaDestinoId}
                    Valor: {valor:C}
                    Saldo anterior: {saldoAnteriorContaOrigem:C}
                    Saldo atual: {saldoNovoContaOrigem:C}
                    Status: Concluída
                    ----------------------------------
                    """;

            // Responsabilidade 12: gravação de arquivo
            var diretorioExtratos = Path.Combine(
                Directory.GetCurrentDirectory(),
                "extratos");

            Directory.CreateDirectory(diretorioExtratos);

            var caminhoExtrato = Path.Combine(
                diretorioExtratos,
                $"{transferenciaId}.txt");

            await File.WriteAllTextAsync(
                caminhoExtrato,
                extrato,
                cancellationToken);

            return caminhoExtrato;
        }
    }
}
