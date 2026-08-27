using simulador_de_banco.Application.Extrato.Interface;
using simulador_de_banco.Application.Extrato.Models;

namespace simulador_de_banco.Infrastructure.Storage
{
    public class ExtratoFile : IExtratoServices
    {
        public async Task<string> GerarExtratoTransacao(
            ExtratoBancarioTransacao extratoBancarioTransacao,
            CancellationToken cancellationToken)
        {
            var extrato = $"""
                    BANCO EXEMPLO
                    ----------------------------------
                    COMPROVANTE DE TRANSFERÊNCIA

                    Código: {extratoBancarioTransacao.TransferenciaId}
                    Data: {DateTime.Now:dd/MM/yyyy HH:mm:ss}
                    Origem: {extratoBancarioTransacao.ContaOrigemId}
                    Destino: {extratoBancarioTransacao.ContaDestinoId}
                    Valor: {extratoBancarioTransacao.Valor:C}
                    Saldo anterior: {extratoBancarioTransacao.SaldoAnteriorContaOrigem:C}
                    Saldo atual: {extratoBancarioTransacao.SaldoNovoContaOrigem:C}
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
                $"{extratoBancarioTransacao.TransferenciaId}.txt");

            await File.WriteAllTextAsync(
                caminhoExtrato,
                extrato,
                cancellationToken);

            return caminhoExtrato;
        }
    }
}
