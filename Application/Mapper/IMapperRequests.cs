using simulador_de_banco.Application.Antifraude.Models;
using simulador_de_banco.Application.DTO;

namespace simulador_de_banco.Application.Mapper
{
    public class IMapperRequests
    {
        public AntifraudeConsulta MapeandoAntifraudeConsulta(TransacaoRequestDto transacaoRequestDto);
    }
}
