namespace simulador_de_banco.Application.Interface.IInfrastructure.Storage
{
    public interface IExtratoServices
    {
        public Task<string> GerarExtratoTransacao(
            Guid transferenciaId,
            int contaOrigemId,
            int contaDestinoId,
            decimal valor,
            decimal saldoAnteriorContaOrigem,
            decimal saldoNovoContaOrigem,
            CancellationToken cancellationToken);
    }
}
