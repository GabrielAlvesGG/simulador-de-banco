namespace simulador_de_banco.Application.Interface.IInfrastructure.Persistence
{
    public interface ISqlUnitOfWorkServices
    {
        public Task BeginAsync(CancellationToken cancellationToken = default);
        public Task CommitAsync(CancellationToken cancellationToken = default);

        public Task RollbackAsync(CancellationToken cancellationToken = default);

        public ValueTask DisposeAsync();
    }
}
