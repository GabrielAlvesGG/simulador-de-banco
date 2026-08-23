using simulador_de_banco.Application.Notification.Models;

namespace simulador_de_banco.Application.Notification.Interface
{
    public interface IEmailServices
    {
        public Task EmailTransacaoContaOrigem(NotificationTransferencia transacaoEmailEnviarRequest, CancellationToken cancellationToken);
    }
}
