using Microsoft.Data.SqlClient;
using simulador_de_banco.Application.Interface.IInfrastructure.Persistence;
using System.Data;

namespace simulador_de_banco.Infrastructure.Persistence
{
    public sealed class SqlUnitOfWork : ISqlUnitOfWorkServices, IUnitOfWork
    {

        private readonly SqlConnection _connection;
        private SqlTransaction? _transaction;

        public SqlConnection Connection => _connection; //Qual o motivo disso ? 

        public SqlTransaction? Transaction => _transaction;

        public SqlUnitOfWork(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("A conexão com o banco não foi configurada.");

                _connection = new SqlConnection(connectionString);
        }

        public async Task BeginAsync(CancellationToken cancellationToken = default)
        {
            if(_transaction is not null)
            {
                throw new InvalidOperationException("Já existe uma transação em andamento.");
            }

            if(_connection.State != ConnectionState.Open)
            {
                await _connection.OpenAsync(cancellationToken);
            }

            _transaction = (SqlTransaction)await _connection.BeginTransactionAsync(cancellationToken);

        }

        public async Task CommitAsync(CancellationToken cancellationToken = default) { 
            if(_transaction is null)
            {
                throw new InvalidOperationException(
                    "Nenhuma transação foi iniciada.");
            }

            await _transaction.CommitAsync(cancellationToken);
            await EncerrarTransacaoAsync();
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default) { 
            if(_transaction is null)
                return;
            

            await _transaction.RollbackAsync(cancellationToken);
            await EncerrarTransacaoAsync();
        }

        private async Task EncerrarTransacaoAsync()
        {
            if (_transaction is null)
                return;

            await _transaction.DisposeAsync();

            _transaction = null;
        }
        public async ValueTask DisposeAsync()
        {
            if (_transaction is not null)
            {
                try
                {
                    await _transaction.RollbackAsync();
                }
                finally
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }

            await _connection.DisposeAsync();
        }
    }
}
