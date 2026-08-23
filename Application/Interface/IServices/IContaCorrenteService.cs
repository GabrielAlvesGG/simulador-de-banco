using simulador_de_banco.Application.DTO;

namespace simulador_de_banco.Application.Interface.IServices
{
    public interface IContaCorrenteService
    {
        public Task<ResultadoTransferenciaDto> TransferirAsync(int idContaOrigem, int idContaDestino, decimal valor, CancellationToken cancellationToken);

    }
}
