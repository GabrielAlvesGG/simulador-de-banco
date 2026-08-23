using Microsoft.Data.SqlClient;
using simulador_de_banco.Application.Interface.IInfrastructure.Repository;
using simulador_de_banco.Domain.Entidade;
using simulador_de_banco.Infrastructure.Persistence.Interface;


namespace simulador_de_banco.Infrastructure.Repostory
{
    public class TransacoesRepository : ITransacoesServices
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransacoesRepository(IUnitOfWork sqlUnitOfWork)
        {
            _unitOfWork = sqlUnitOfWork;
        }
        public async Task<ContaCorrente?> BuscarContaAsync(
            int contaId,
            CancellationToken cancellationToken)
        {
            string sql = """
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

            var connection = _unitOfWork.Connection;

            var transaction = _unitOfWork.Transaction ?? throw new InvalidOperationException("Nenhuma transação foi iniciada.");

            await using var command = new SqlCommand(sql, connection, transaction);

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


        public async Task AtualizarSaldoAsync(
            int contaId,
            decimal novoSaldo,
            CancellationToken cancellationToken)
        {
            const string sql = """
            UPDATE ContasCorrentes
            SET Saldo = @Saldo
            WHERE Id = @Id
            """;


            var connection = _unitOfWork.Connection;

            var transaction = _unitOfWork.Transaction;

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

        public async Task<Guid> RegistrarMovimentacaoAsync(Movimentacao movimentacao, CancellationToken cancellationToken)
        {

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


            var connection = _unitOfWork.Connection;
            var transaction = _unitOfWork.Transaction;

            await using var commandMovimentacao =
                new SqlCommand(
                    insertMovimentacao,
                    connection,
                    transaction);

            commandMovimentacao.Parameters.AddWithValue(
                "@Id",
                movimentacao.Id);

            commandMovimentacao.Parameters.AddWithValue(
                "@ContaOrigemId",
                movimentacao.ContaOrigemId);

            commandMovimentacao.Parameters.AddWithValue(
                "@ContaDestinoId",
                movimentacao.ContaDestinoId);

            commandMovimentacao.Parameters.AddWithValue(
                "@Valor",
                movimentacao.Valor);

            commandMovimentacao.Parameters.AddWithValue(
                "@DataMovimentacao",
                movimentacao.DataMovimentacao);

            commandMovimentacao.Parameters.AddWithValue(
                "@Tipo",
                movimentacao.Tipo);

            commandMovimentacao.Parameters.AddWithValue(
                "@Status",
                movimentacao.Status);

            await commandMovimentacao.ExecuteNonQueryAsync(
                cancellationToken);

            return movimentacao.Id;

        }

    }
}
