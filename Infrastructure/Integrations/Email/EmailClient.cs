using simulador_de_banco.Application.Interface.IInfrastructure.Integration.Email;
using System.Net.Mail;

namespace simulador_de_banco.Infrastructure.Integrations.email
{
    public class EmailClient :IEmailServices
    {

        public async Task EmailTransacao(string emailOrigem,string nomeOrigem,int contaDestinoId,decimal valor,decimal novoSaldoOrigem, Guid transferenciaId, CancellationToken cancellationToken)
        {
            using var smtpClient = new SmtpClient(
                    "smtp.exemplo.com",
                    587);

            smtpClient.EnableSsl = true;

            using var mensagem = new MailMessage(
                "banco@exemplo.com",
                emailOrigem);

            mensagem.Subject = "Transferência realizada";

            mensagem.Body = $"""
                    Olá, {nomeOrigem}.

                    Sua transferência foi realizada com sucesso.

                    Conta de destino: {contaDestinoId}
                    Valor: {valor:C}
                    Saldo atual: {novoSaldoOrigem:C}
                    Código: {transferenciaId}
                    """;

            await smtpClient.SendMailAsync(
                mensagem,
                cancellationToken);

        }
    }
}
