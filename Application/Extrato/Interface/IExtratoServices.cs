using simulador_de_banco.Application.Extrato.Models;

namespace simulador_de_banco.Application.Extrato.Interface
{
    public interface IExtratoServices
    {
        public Task<string> GerarExtratoTransacao(
            ExtratoBancarioTransacao extratoBancarioTransacao,
            CancellationToken cancellationToken);
    }
}
