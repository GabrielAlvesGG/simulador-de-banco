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
            await _transacoesRepository.TransferirAsync(idContaOrigem, idContaDestino, valor, cancellationToken);
        }

    }

    public class ContaCorrente
    {
        public int Id { get; set; }
        public string Numero { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal Saldo { get; set; }
        public bool Ativa { get; set; }
    }

    public class ResultadoAntifraude
    {
        public bool Aprovado { get; set; }
        public string? Motivo { get; set; }
    }

    public class ResultadoTransferencia
    {
        public Guid TransferenciaId { get; set; }
        public int ContaOrigemId { get; set; }
        public int ContaDestinoId { get; set; }
        public decimal Valor { get; set; }
        public decimal SaldoAnterior { get; set; }
        public decimal SaldoAtual { get; set; }
        public DateTime DataTransferencia { get; set; }
        public string CaminhoExtrato { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
