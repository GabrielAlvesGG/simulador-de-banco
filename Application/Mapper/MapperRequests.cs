using simulador_de_banco.Application.Antifraude.Models;
using simulador_de_banco.Application.DTO;

namespace simulador_de_banco.Application.Mapper
{
    public class MapperRequests : IMapperRequests
    {

        public AntifraudeConsulta MapeandoAntifraudeConsulta(TransacaoRequestDto transacaoRequestDto)
        {
            return new AntifraudeConsulta()
            {
                IdContaDestino = transacaoRequestDto.IdContaDestino,
                IdContaOrigem = transacaoRequestDto.IdContaOrigem,
                Valor = transacaoRequestDto.Valor
            };
        }
    }
}
