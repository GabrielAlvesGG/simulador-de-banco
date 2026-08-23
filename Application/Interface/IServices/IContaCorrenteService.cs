using simulador_de_banco.Application.DTO;

namespace simulador_de_banco.Application.Interface.IServices
{
    public interface IContaCorrenteService
    {
        public Task<ResultadoTransferenciaResponseDto> TransferirAsync(TransacaoRequestDto transacaoRequestDto, CancellationToken cancellationToken);

    }
}
