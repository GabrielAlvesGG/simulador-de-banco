using simulador_de_banco.Domain.Entidade;

namespace simulador_de_banco.Application.Interface.IInfrastructure.Repository
{
    public interface ITransacoesServices
    {

        public Task<ContaCorrente?> BuscarContaAsync(
            int contaId,
            CancellationToken cancellationToken);


        public Task AtualizarSaldoAsync(
            ContaCorrente contaCorrente,
            CancellationToken cancellationToken);

        public Task RegistrarMovimentacaoAsync(Movimentacao movimentacao, CancellationToken cancellationToken);

    }
}
