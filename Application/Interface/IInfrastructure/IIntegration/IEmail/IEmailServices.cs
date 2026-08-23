
using simulador_de_banco.Domain.Entidade;

namespace simulador_de_banco.Application.Interface.IInfrastructure.Integration.Email
{
    public interface IEmailServices
    {
        public Task EmailTransacaoContaOrigem(TransacaoEmailEnviar transacaoEmailEnviarRequest, CancellationToken cancellationToken);
    }
}
