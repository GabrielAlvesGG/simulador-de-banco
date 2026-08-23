using simulador_de_banco.Application.DTO;
using simulador_de_banco.Application.Interface.IInfrastructure.Integration.Antifraude;
using simulador_de_banco.Application.Interface.IInfrastructure.Integration.Email;
using simulador_de_banco.Application.Interface.IInfrastructure.Persistence;
using simulador_de_banco.Application.Interface.IInfrastructure.Repository;
using simulador_de_banco.Application.Interface.IServices;
using simulador_de_banco.Domain.Entidade;

namespace simulador_de_banco.Application.Services
{

    public class ContaCorrenteService : IContaCorrenteService
    {
        private readonly ITransacoesServices _transacoesRepository;

        private readonly ISqlUnitOfWorkServices _unitOfWork;
        private readonly IAntifraudeServices _antifraudeService;
        private readonly IEmailServices _emailServices;
        public ContaCorrenteService(ITransacoesServices transacoesRepository, ISqlUnitOfWorkServices sqlUnitOfWork, IAntifraudeServices antifraudeService,
            IEmailServices emailServices)
        {
            _transacoesRepository = transacoesRepository;
            _unitOfWork = sqlUnitOfWork;
            _antifraudeService = antifraudeService;
            _emailServices = emailServices;
        }

        public async Task<ResultadoTransferenciaDto> TransferirAsync(int idContaOrigem, int idContaDestino, decimal valor, CancellationToken cancellationToken)
        {
            ValidateTransacao(idContaOrigem, idContaDestino, valor);

            await _unitOfWork.BeginAsync(cancellationToken);
            try
            {
          
                ContaCorrente? contaOrigem = await _transacoesRepository.BuscarContaAsync(idContaOrigem,cancellationToken);

                if (contaOrigem is null)
                    throw new InvalidOperationException(
                        "Conta de origem não encontrada.");

                if (contaOrigem.Saldo < valor)
                    throw new InvalidOperationException("Saldo insuficiente.");

                if (!contaOrigem.Ativa)
                    throw new InvalidOperationException(
                        "A conta de origem está bloqueada.");

                ContaCorrente? contaDestino = await _transacoesRepository.BuscarContaAsync(idContaDestino, cancellationToken);

                if (contaDestino is null)
                    throw new InvalidOperationException(
                        "Conta de destino não encontrada.");  

                if (!contaDestino.Ativa)
                    throw new InvalidOperationException(
                        "A conta de destino está bloqueada.");

                await _antifraudeService.AntifraudeVerificaTransacao(contaOrigem.Id, contaDestino.Id, valor, cancellationToken);


                // Abstrair para um método privado 
                decimal saldoAntigoContaOrigem = contaOrigem.Saldo;

                contaOrigem.Saldo = contaOrigem.Saldo - valor; 

                contaDestino.Saldo = contaDestino.Saldo + valor;

                await _transacoesRepository.AtualizarSaldoAsync(contaOrigem.Id, contaOrigem.Saldo, cancellationToken);

                await _transacoesRepository.AtualizarSaldoAsync(contaDestino.Id, contaDestino.Saldo, cancellationToken);

               Guid transferenciaId =  await _transacoesRepository.RegistrarMovimentacaoAsync(contaOrigem.Id, contaDestino.Id, valor, cancellationToken);

                await _unitOfWork.CommitAsync(cancellationToken);

                await _emailServices.EmailTransacao(
                    contaOrigem.Email,
                    contaOrigem.Nome,
                    contaDestino.Id,
                    valor,
                    contaOrigem.Saldo,
                    transferenciaId, 
                    cancellationToken);

                return new ResultadoTransferenciaDto
                {
                    TransferenciaId = transferenciaId,
                    ContaOrigemId = contaOrigem.Id,
                    ContaDestinoId = contaDestino.Id,
                    Valor = valor,
                    SaldoAnterior = saldoAntigoContaOrigem,
                    SaldoAtual = contaOrigem.Saldo,
                    DataTransferencia = DateTime.UtcNow,
                    //CaminhoExtrato = caminhoExtrato,
                    Status = "Concluída"
                };
            }
            catch
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
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
