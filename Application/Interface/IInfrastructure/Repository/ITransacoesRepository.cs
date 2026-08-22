using simulador_de_banco.Application.DTO;

namespace simulador_de_banco.Application.Interface.IInfrastructure.Repository
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
