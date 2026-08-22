namespace simulador_de_banco.Application.Interface.IServices
{
    public interface IContaCorrenteService
    {
        public Task TransferirAsync(int idContaOrigem, int idContaDestino, decimal valor, CancellationToken cancellationToken);

    }
}
