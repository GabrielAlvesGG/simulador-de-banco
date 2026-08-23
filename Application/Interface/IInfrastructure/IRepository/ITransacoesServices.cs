using simulador_de_banco.Domain.Entidade;

namespace simulador_de_banco.Application.Interface.IInfrastructure.Repository
{
    public interface ITransacoesServices
    {

        public Task<ContaCorrente?> BuscarContaAsync(
            int contaId,
            CancellationToken cancellationToken);


        public Task AtualizarSaldoAsync(
            int contaId,
            decimal novoSaldo,
            CancellationToken cancellationToken);

        public Task<Guid> RegistrarMovimentacaoAsync(int contaOrigemId, int contaDestinoId, decimal valor, CancellationToken cancellationToken);

    }
}
