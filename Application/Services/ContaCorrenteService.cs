using simulador_de_banco.Application.DTO;
using simulador_de_banco.Application.Interface.IInfrastructure.Persistence;
using simulador_de_banco.Application.Interface.IInfrastructure.Repository;
using simulador_de_banco.Application.Interface.IServices;
using simulador_de_banco.Domain.Entidade;
using simulador_de_banco.Application.Notification.Interface;
using simulador_de_banco.Application.Notification.Models;
using simulador_de_banco.Application.Antifraude.Interface;
using simulador_de_banco.Application.Extrato.Interface;
using simulador_de_banco.Application.Extrato.Models;
using simulador_de_banco.Application.Antifraude.Models;

namespace simulador_de_banco.Application.Services
{

    public class ContaCorrenteService : IContaCorrenteService
    {
        private readonly ITransacoesServices _transacoesRepository;

        private readonly ISqlUnitOfWorkServices _unitOfWork;
        private readonly IAntifraudeServices _antifraudeService;
        private readonly IEmailServices _emailServices;
        private readonly IExtratoServices _extratorServices;
        public ContaCorrenteService(ITransacoesServices transacoesRepository,
            ISqlUnitOfWorkServices sqlUnitOfWork,
            IAntifraudeServices antifraudeService,
            IEmailServices emailServices,
            IExtratoServices extratoServices)
        {
            _transacoesRepository = transacoesRepository;
            _unitOfWork = sqlUnitOfWork;
            _antifraudeService = antifraudeService;
            _emailServices = emailServices;
            _extratorServices = extratoServices;
        }

        public async Task<ResultadoTransferenciaResponseDto> TransferirAsync(TransacaoRequestDto transacaoRequestDto, CancellationToken cancellationToken)
        {
            ValidateTransacao(transacaoRequestDto);

            AntifraudeConsulta antifraudeRequest = new AntifraudeConsulta() { 
            IdContaDestino = transacaoRequestDto.IdContaDestino,
            IdContaOrigem = transacaoRequestDto.IdContaOrigem,
            Valor = transacaoRequestDto.Valor            
            };

            await _antifraudeService.AntifraudeVerificaTransacao(antifraudeRequest, cancellationToken);

            await _unitOfWork.BeginAsync(cancellationToken);
            try
            {
          
                ContaCorrente? contaOrigem = await _transacoesRepository.BuscarContaAsync(transacaoRequestDto.IdContaOrigem,cancellationToken);

                if (contaOrigem is null)
                    throw new InvalidOperationException(
                        "Conta de origem não encontrada.");

                if (contaOrigem.Saldo < transacaoRequestDto.Valor)
                    throw new InvalidOperationException("Saldo insuficiente.");

                if (!contaOrigem.Ativa)
                    throw new InvalidOperationException(
                        "A conta de origem está bloqueada.");

                ContaCorrente? contaDestino = await _transacoesRepository.BuscarContaAsync(transacaoRequestDto.IdContaDestino, cancellationToken);

                if (contaDestino is null)
                    throw new InvalidOperationException(
                        "Conta de destino não encontrada.");  

                if (!contaDestino.Ativa)
                    throw new InvalidOperationException(
                        "A conta de destino está bloqueada.");



                decimal saldoAntigoContaOrigem = contaOrigem.Saldo;

                contaOrigem.Saldo = contaOrigem.Saldo - transacaoRequestDto.Valor; 

                contaDestino.Saldo = contaDestino.Saldo + transacaoRequestDto.Valor;

                await _transacoesRepository.AtualizarSaldoAsync(contaOrigem.Id, contaOrigem.Saldo, cancellationToken);

                await _transacoesRepository.AtualizarSaldoAsync(contaDestino.Id, contaDestino.Saldo, cancellationToken);

               Guid transferenciaId =  await _transacoesRepository.RegistrarMovimentacaoAsync(contaOrigem.Id, contaDestino.Id, transacaoRequestDto.Valor, cancellationToken);

                await _unitOfWork.CommitAsync(cancellationToken);

                NotificationTransferencia transacaoEmailEnviarRequest = new NotificationTransferencia()
                {
                    Email = contaOrigem.Email,
                    Nome = contaOrigem.Nome,
                    ContaDestinoId = contaDestino.Id,
                    Valor = transacaoRequestDto.Valor,
                    Saldo = contaOrigem.Saldo,
                    TransferenciaId = transferenciaId,

                };

                await _emailServices.EmailTransacaoContaOrigem(
                    transacaoEmailEnviarRequest, cancellationToken);

                ExtratoBancarioTransacao extratoBancarioTransacao = new ExtratoBancarioTransacao()
                {
                    TransferenciaId = transferenciaId,
                    ContaOrigemId = contaOrigem.Id,
                    ContaDestinoId = contaDestino.Id,
                    Valor = transacaoRequestDto.Valor,
                    SaldoAnteriorContaOrigem = saldoAntigoContaOrigem,
                    SaldoNovoContaOrigem = contaOrigem.Saldo
                };

                string caminhoExtrato = await _extratorServices.GerarExtratoTransacao(extratoBancarioTransacao, cancellationToken);

                return new ResultadoTransferenciaResponseDto
                {
                    TransferenciaId = transferenciaId,
                    ContaOrigemId = contaOrigem.Id,
                    ContaDestinoId = contaDestino.Id,
                    Valor = transacaoRequestDto.Valor,
                    SaldoAnterior = saldoAntigoContaOrigem,
                    SaldoAtual = contaOrigem.Saldo,
                    DataTransferencia = DateTime.UtcNow,
                    CaminhoExtrato = caminhoExtrato,
                    Status = "Concluída"
                };
            }
            catch
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }

        private void ValidateTransacao(TransacaoRequestDto transacaoRequestDto)
        {

            var limiteDiario = 10_000m;

            if (transacaoRequestDto.Valor > limiteDiario)
                throw new InvalidOperationException(
                    "O transacaoRequestDto.Valor ultrapassa o limite diário permitido.");

            if (transacaoRequestDto.IdContaOrigem <= 0)
                throw new ArgumentException("Conta de origem inválida.");

            if (transacaoRequestDto.IdContaDestino <= 0)
                throw new ArgumentException("Conta de destino inválida.");

            if (transacaoRequestDto.IdContaOrigem == transacaoRequestDto.IdContaDestino)
                throw new InvalidOperationException(
                    "As contas de origem e destino devem ser diferentes.");

            if (transacaoRequestDto.Valor <= 0)
                throw new ArgumentException(
                    "O transacaoRequestDto.Valor da transferência deve ser maior que zero.");
        }

    }
}
