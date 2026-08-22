using simulador_de_banco.Application.Services;

namespace simulador_de_banco.Infrastructure.Interface
{
    public interface ITransacoesRepository
    {
        public Task<ResultadoTransferencia> TransferirAsync(
          int contaOrigemId,
          int contaDestinoId,
          decimal valor,
          CancellationToken cancellationToken = default);

    }
}
