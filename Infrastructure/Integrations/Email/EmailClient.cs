using simulador_de_banco.Application.Notification.Interface;
using simulador_de_banco.Application.Notification.Models;
using System.Net.Mail;

namespace simulador_de_banco.Infrastructure.Integrations.email
{
    public class EmailClient :IEmailServices
    {

        public async Task EmailTransacaoContaOrigem(NotificationTransferencia transacaoEmailEnviarRequest, CancellationToken cancellationToken)
        {
            using var smtpClient = new SmtpClient(
                    "smtp.exemplo.com",
                    587);

            smtpClient.EnableSsl = true;

            using var mensagem = new MailMessage(
                "banco@exemplo.com",
                transacaoEmailEnviarRequest.Email);

            mensagem.Subject = "Transferência realizada";

            mensagem.Body = $"""
                    Olá, {transacaoEmailEnviarRequest.Nome}.

                    Sua transferência foi realizada com sucesso.

                    Conta de destino: {transacaoEmailEnviarRequest.ContaDestinoId}
                    Valor: {transacaoEmailEnviarRequest.Valor:C}
                    Saldo atual: {transacaoEmailEnviarRequest.Saldo:C}
                    Código: {transacaoEmailEnviarRequest.TransferenciaId}
                    """;

            await smtpClient.SendMailAsync(
                mensagem,
                cancellationToken);

        }
    }
}
