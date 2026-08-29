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
using simulador_de_banco.Application.Mapper;

namespace simulador_de_banco.Application.Services
{

    public class ContaCorrenteService : IContaCorrenteService
    {
        private readonly ITransacoesServices _transacoesRepository;

        private readonly IUnitOfWorkServices _unitOfWork;
        private readonly IAntifraudeServices _antifraudeService;
        private readonly IEmailServices _emailServices;
        private readonly IExtratoServices _extratorServices;
        private readonly IMapperRequests _mapperRequests;
        public ContaCorrenteService(ITransacoesServices transacoesRepository,
            IUnitOfWorkServices sqlUnitOfWork,
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
            ValidaTransacao(transacaoRequestDto);
            
            AntifraudeConsulta antifraudeRequest = _mapperRequests.MapeandoAntifraudeConsulta(transacaoRequestDto);

            await _antifraudeService.AntifraudeVerificaTransacao(antifraudeRequest, cancellationToken);

            await _unitOfWork.BeginAsync(cancellationToken);
            try
            {
          
                ContaCorrente? contaOrigem = await _transacoesRepository.BuscarContaAsync(transacaoRequestDto.IdContaOrigem,cancellationToken);

                ContaCorrente contaCorrenteOperacoes = new ContaCorrente();

                bool isContaOrigem = true;

                contaCorrenteOperacoes.ValidandoContaCorrente(contaOrigem, transacaoRequestDto.Valor, isContaOrigem);

                ContaCorrente? contaDestino = await _transacoesRepository.BuscarContaAsync(transacaoRequestDto.IdContaDestino, cancellationToken);

                bool isContaDestino = false;

                contaCorrenteOperacoes.ValidandoContaCorrente(contaDestino, transacaoRequestDto.Valor, isContaDestino);


                decimal saldoAntigoContaOrigem = contaOrigem.Saldo;

                contaCorrenteOperacoes.Debitar(contaOrigem, transacaoRequestDto.Valor);

                contaCorrenteOperacoes.Creditar(contaDestino, transacaoRequestDto.Valor);


                await _transacoesRepository.AtualizarSaldoAsync(contaOrigem.Id, contaOrigem.Saldo, cancellationToken);

                await _transacoesRepository.AtualizarSaldoAsync(contaDestino.Id, contaDestino.Saldo, cancellationToken);

                Movimentacao movimentacao = new Movimentacao(contaOrigem.Id, contaDestino.Id, transacaoRequestDto.Valor);
                await _transacoesRepository.RegistrarMovimentacaoAsync(movimentacao, cancellationToken);

                await _unitOfWork.CommitAsync(cancellationToken);

                NotificationTransferencia transacaoEmailEnviarRequest = new NotificationTransferencia()
                {
                    Email = contaOrigem.Email,
                    Nome = contaOrigem.Nome,
                    ContaDestinoId = contaDestino.Id,
                    Valor = transacaoRequestDto.Valor,
                    Saldo = contaOrigem.Saldo,
                    TransferenciaId = movimentacao.Id,

                };

                await _emailServices.EmailTransacaoContaOrigem(
                    transacaoEmailEnviarRequest, cancellationToken);

                ExtratoBancarioTransacao extratoBancarioTransacao = new ExtratoBancarioTransacao()
                {
                    TransferenciaId = movimentacao.Id,
                    ContaOrigemId = contaOrigem.Id,
                    ContaDestinoId = contaDestino.Id,
                    Valor = transacaoRequestDto.Valor,
                    SaldoAnteriorContaOrigem = saldoAntigoContaOrigem,
                    SaldoNovoContaOrigem = contaOrigem.Saldo
                };

                string caminhoExtrato = await _extratorServices.GerarExtratoTransacao(extratoBancarioTransacao, cancellationToken);

                return new ResultadoTransferenciaResponseDto
                {
                    TransferenciaId = movimentacao.Id,
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

        private void ValidaTransacao(TransacaoRequestDto transacaoRequestDto)
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
