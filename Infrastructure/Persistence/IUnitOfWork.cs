using Microsoft.Data.SqlClient;
using simulador_de_banco.Application.Interface.IInfrastructure.Persistence;

namespace simulador_de_banco.Infrastructure.Persistence
{
    public interface IUnitOfWork
    {
        public SqlConnection Connection { get; }
        public SqlTransaction Transaction { get; }
    }
}
