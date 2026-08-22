using System.Net.Http.Json;
using System.Net.Mail;
using Microsoft.Data.SqlClient;
using simulador_de_banco.Application.Interface.IInfrastructure.Repository;
using simulador_de_banco.Application.Interface.IServices;

namespace simulador_de_banco.Application.Services
{

    public class ContaCorrenteService : IContaCorrenteService
    {
        private readonly ITransacoesRepository _transacoesRepository;
        public ContaCorrenteService(ITransacoesRepository transacoesRepository)
        {
            _transacoesRepository = transacoesRepository;
        }

        public async Task TransferirAsync(int idContaOrigem, int idContaDestino, decimal valor, CancellationToken cancellationToken)
        {
            if (idContaOrigem <= 0)
                throw new ArgumentException("Conta de origem inválida.");

            if (idContaDestino <= 0)
                throw new ArgumentException("Conta de destino inválida.");

            if (idContaOrigem == idContaDestino)
                throw new InvalidOperationException(
                    "As contas de origem e destino devem ser diferentes.");

            if (valor <= 0)
                throw new ArgumentException(
                    "O valor da transferência deve ser maior que zero.");

            await _transacoesRepository.TransferirAsync(idContaOrigem, idContaDestino, valor, cancellationToken);
        }

    }
}
