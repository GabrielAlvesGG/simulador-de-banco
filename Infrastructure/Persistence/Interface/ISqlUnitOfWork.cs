using Microsoft.Data.SqlClient;
using simulador_de_banco.Application.Interface.IInfrastructure.Persistence;

namespace simulador_de_banco.Infrastructure.Persistence.Interface
{
    public interface ISqlUnitOfWork
    {
        public SqlConnection Connection { get; }
        public SqlTransaction Transaction { get; }
    }
}
