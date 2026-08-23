using Microsoft.Data.SqlClient;
using simulador_de_banco.Application.Interface.IInfrastructure.Persistence;
using simulador_de_banco.Application.Interface.IInfrastructure.Repository;
using simulador_de_banco.Application.Interface.IServices;
using simulador_de_banco.Domain.Entidade;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using System.Net.Mail;
using System.Runtime.CompilerServices;

namespace simulador_de_banco.Application.Services
{

    public class ContaCorrenteService : IContaCorrenteService
    {
        private readonly ITransacoesRepository _transacoesRepository;

        private readonly ISqlUnitOfWork _unitOfWork;
        public ContaCorrenteService(ITransacoesRepository transacoesRepository, ISqlUnitOfWork sqlUnitOfWork)
        {
            _transacoesRepository = transacoesRepository;
            _unitOfWork = sqlUnitOfWork;
        }

        public async Task TransferirAsync(int idContaOrigem, int idContaDestino, decimal valor, CancellationToken cancellationToken)
        {
            ValidateTransacao(idContaOrigem, idContaDestino, valor);

            await _unitOfWork.BeginAsync(cancellationToken);
            try
            {
          
                ContaCorrente contaOrigem = await _transacoesRepository.BuscarContaAsync(idContaOrigem,cancellationToken);

                if (contaOrigem is null)
                    throw new InvalidOperationException(
                        "Conta de origem não encontrada.");

                if (contaOrigem.Saldo < valor)
                    throw new InvalidOperationException("Saldo insuficiente.");

                if (!contaOrigem.Ativa)
                    throw new InvalidOperationException(
                        "A conta de origem está bloqueada.");

                ContaCorrente contaDestino = await _transacoesRepository.BuscarContaAsync(idContaDestino, cancellationToken);

                if (contaDestino is null)
                    throw new InvalidOperationException(
                        "Conta de destino não encontrada.");  

                if (!contaDestino.Ativa)
                    throw new InvalidOperationException(
                        "A conta de destino está bloqueada.");

                // Colocar algo para conseguir validar o antifraude. Próximo passo .

                contaOrigem.Saldo = contaOrigem.Saldo - valor;
                contaDestino.Saldo = contaDestino.Saldo + valor;

                _transacoesRepository.AtualizarSaldoAsync(contaOrigem.Id, contaOrigem.Saldo, cancellationToken);
                _transacoesRepository.AtualizarSaldoAsync(contaDestino.Id, contaDestino.Saldo, cancellationToken);

                _transacoesRepository.RegistrarMovimentacaoAsync(contaOrigem.Id, contaDestino.Id, valor, cancellationToken);

                _unitOfWork.CommitAsync(cancellationToken);

                // Colocar a questão da notificação para ser feita. Próximo passo.
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }

        private void ValidateTransacao(int idContaOrigem, int idContaDestino, decimal valor)
        {

            var limiteDiario = 10_000m;

            if (valor > limiteDiario)
                throw new InvalidOperationException(
                    "O valor ultrapassa o limite diário permitido.");

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
        }

    }
}
