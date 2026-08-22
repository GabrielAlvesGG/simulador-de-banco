using Microsoft.Data.SqlClient;
using simulador_de_banco.Application.Interface.IInfrastructure.Repository;
using simulador_de_banco.Application.DTO;
using System.Net.Mail;
using simulador_de_banco.Domain.Entidade;
using simulador_de_banco.Infrastructure.IntegrationModel;


namespace simulador_de_banco.Infrastructure.Repostory
{
    public class TransacoesRepository : ITransacoesRepository
    {
        private readonly string _connectionString;
        private readonly HttpClient _httpClient;
        public TransacoesRepository(IConfiguration configuration, HttpClient httpClient)
        {
            _connectionString = configuration.GetConnectionString("ConnectionStrings") ?? throw new InvalidOperationException(
                    "A conexão com o banco não foi configurada.");
            _httpClient = httpClient;
        }

        public async Task<ResultadoTransferencia> TransferirAsync(
           int contaOrigemId,
           int contaDestinoId,
           decimal valor,
           CancellationToken cancellationToken = default)
        {
            if (contaOrigemId <= 0)
                throw new ArgumentException("Conta de origem inválida.");

            if (contaDestinoId <= 0)
                throw new ArgumentException("Conta de destino inválida.");

            if (contaOrigemId == contaDestinoId)
                throw new InvalidOperationException(
                    "As contas de origem e destino devem ser diferentes.");

            if (valor <= 0)
                throw new ArgumentException(
                    "O valor da transferência deve ser maior que zero.");

            await using var connection =
                new SqlConnection(_connectionString);

            await connection.OpenAsync(cancellationToken);

            await using var transaction =
                (SqlTransaction)await connection.BeginTransactionAsync(
                    cancellationToken);

            try
            {
                var contaOrigem = await BuscarContaAsync(
                    contaOrigemId,
                    connection,
                    transaction,
                    cancellationToken);

                var contaDestino = await BuscarContaAsync(
                    contaDestinoId,
                    connection,
                    transaction,
                    cancellationToken);

                if (contaOrigem is null)
                    throw new InvalidOperationException(
                        "Conta de origem não encontrada.");

                if (contaDestino is null)
                    throw new InvalidOperationException(
                        "Conta de destino não encontrada.");

                if (!contaOrigem.Ativa)
                    throw new InvalidOperationException(
                        "A conta de origem está bloqueada.");

                if (!contaDestino.Ativa)
                    throw new InvalidOperationException(
                        "A conta de destino está bloqueada.");

                if (contaOrigem.Saldo < valor)
                    throw new InvalidOperationException(
                        "Saldo insuficiente.");

                var limiteDiario = 10_000m;

                if (valor > limiteDiario)
                    throw new InvalidOperationException(
                        "O valor ultrapassa o limite diário permitido.");

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
                        .ReadFromJsonAsync<ResultadoAntifraude>(
                            cancellationToken: cancellationToken);

                if (resultadoAntifraude is null ||
                    !resultadoAntifraude.Aprovado)
                {
                    throw new InvalidOperationException(
                        "Transferência recusada pelo serviço antifraude.");
                }

                // Responsabilidade 7: cálculo dos novos saldos
                var novoSaldoOrigem = contaOrigem.Saldo - valor;
                var novoSaldoDestino = contaDestino.Saldo + valor;

                // Responsabilidade 8: atualização direta no banco
                await AtualizarSaldoAsync(
                    contaOrigemId,
                    novoSaldoOrigem,
                    connection,
                    transaction,
                    cancellationToken);

                await AtualizarSaldoAsync(
                    contaDestinoId,
                    novoSaldoDestino,
                    connection,
                    transaction,
                    cancellationToken);

                // Responsabilidade 9: gravação da movimentação
                var transferenciaId = Guid.NewGuid();

                const string insertMovimentacao = """
                INSERT INTO Movimentacoes
                (
                    Id,
                    ContaOrigemId,
                    ContaDestinoId,
                    Valor,
                    DataMovimentacao,
                    Tipo,
                    Status
                )
                VALUES
                (
                    @Id,
                    @ContaOrigemId,
                    @ContaDestinoId,
                    @Valor,
                    @DataMovimentacao,
                    @Tipo,
                    @Status
                )
                """;

                await using var commandMovimentacao =
                    new SqlCommand(
                        insertMovimentacao,
                        connection,
                        transaction);

                commandMovimentacao.Parameters.AddWithValue(
                    "@Id",
                    transferenciaId);

                commandMovimentacao.Parameters.AddWithValue(
                    "@ContaOrigemId",
                    contaOrigemId);

                commandMovimentacao.Parameters.AddWithValue(
                    "@ContaDestinoId",
                    contaDestinoId);

                commandMovimentacao.Parameters.AddWithValue(
                    "@Valor",
                    valor);

                commandMovimentacao.Parameters.AddWithValue(
                    "@DataMovimentacao",
                    DateTime.UtcNow);

                commandMovimentacao.Parameters.AddWithValue(
                    "@Tipo",
                    "TRANSFERENCIA");

                commandMovimentacao.Parameters.AddWithValue(
                    "@Status",
                    "CONCLUIDA");

                await commandMovimentacao.ExecuteNonQueryAsync(
                    cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                // Responsabilidade 10: envio de e-mail
                using var smtpClient = new SmtpClient(
                    "smtp.exemplo.com",
                    587);

                smtpClient.EnableSsl = true;

                using var mensagem = new MailMessage(
                    "banco@exemplo.com",
                    contaOrigem.Email);

                mensagem.Subject = "Transferência realizada";

                mensagem.Body = $"""
                Olá, {contaOrigem.Nome}.

                Sua transferência foi realizada com sucesso.

                Conta de destino: {contaDestino.Numero}
                Valor: {valor:C}
                Saldo atual: {novoSaldoOrigem:C}
                Código: {transferenciaId}
                """;

                await smtpClient.SendMailAsync(
                    mensagem,
                    cancellationToken);

                // Responsabilidade 11: geração de extrato
                var extrato = $"""
                BANCO EXEMPLO
                ----------------------------------
                COMPROVANTE DE TRANSFERÊNCIA

                Código: {transferenciaId}
                Data: {DateTime.Now:dd/MM/yyyy HH:mm:ss}
                Origem: {contaOrigem.Numero}
                Destino: {contaDestino.Numero}
                Valor: {valor:C}
                Saldo anterior: {contaOrigem.Saldo:C}
                Saldo atual: {novoSaldoOrigem:C}
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

                return new ResultadoTransferencia
                {
                    TransferenciaId = transferenciaId,
                    ContaOrigemId = contaOrigemId,
                    ContaDestinoId = contaDestinoId,
                    Valor = valor,
                    SaldoAnterior = contaOrigem.Saldo,
                    SaldoAtual = novoSaldoOrigem,
                    DataTransferencia = DateTime.UtcNow,
                    CaminhoExtrato = caminhoExtrato,
                    Status = "Concluída"
                };
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        // É voltado para parte de infra/repo
        private static async Task<ContaCorrente?> BuscarContaAsync(
            int contaId,
            SqlConnection connection,
            SqlTransaction transaction,
            CancellationToken cancellationToken)
        {
            const string sql = """
            SELECT
                Id,
                Numero,
                Nome,
                Email,
                Saldo,
                Ativa
            FROM ContasCorrentes
            WHERE Id = @Id
            """;

            await using var command =
                new SqlCommand(sql, connection, transaction);

            command.Parameters.AddWithValue("@Id", contaId);

            await using var reader =
                await command.ExecuteReaderAsync(cancellationToken);

            if (!await reader.ReadAsync(cancellationToken))
                return null;

            return new ContaCorrente
            {
                Id = reader.GetInt32(0),
                Numero = reader.GetString(1),
                Nome = reader.GetString(2),
                Email = reader.GetString(3),
                Saldo = reader.GetDecimal(4),
                Ativa = reader.GetBoolean(5)
            };
        }


        // É voltado para parte de infra/repo
        private static async Task AtualizarSaldoAsync(
            int contaId,
            decimal novoSaldo,
            SqlConnection connection,
            SqlTransaction transaction,
            CancellationToken cancellationToken)
        {
            const string sql = """
            UPDATE ContasCorrentes
            SET Saldo = @Saldo
            WHERE Id = @Id
            """;

            await using var command =
                new SqlCommand(sql, connection, transaction);

            command.Parameters.AddWithValue("@Saldo", novoSaldo);
            command.Parameters.AddWithValue("@Id", contaId);

            var registrosAlterados =
                await command.ExecuteNonQueryAsync(cancellationToken);

            if (registrosAlterados == 0)
                throw new InvalidOperationException(
                    "Não foi possível atualizar o saldo da conta.");
        }

    }
}
