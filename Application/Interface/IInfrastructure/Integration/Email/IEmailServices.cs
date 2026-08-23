namespace simulador_de_banco.Application.Interface.IInfrastructure.Integration.Email
{
    public interface IEmailServices
    {
        public Task EmailTransacao(string emailOrigem, string nomeOrigem, int contaDestinoId, decimal valor, decimal novoSaldoOrigem, Guid transferenciaId, CancellationToken cancellationToken);
    }
}
