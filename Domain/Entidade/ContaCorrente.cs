using simulador_de_banco.Application.DTO;

namespace simulador_de_banco.Domain.Entidade
{
    public class ContaCorrente
    {
        public int Id { get; set; }
        public string Numero { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal Saldo { get; set; }
        public bool Ativa { get; set; }


        public void ValidandoContaCorrente(ContaCorrente contaCorrente,decimal valor, bool isContaOrigem) {
            EssaContaExiste(contaCorrente);

            if(isContaOrigem)
                 EssaContaContemSaldo(contaCorrente, valor);


            ContaEstaAtiva(contaCorrente);

        }

        private void EssaContaExiste(ContaCorrente contaCorrente)
        {
            if (contaCorrente is null)
                throw new InvalidOperationException(
                    "Conta não encontrada.");
        }

        private void EssaContaContemSaldo(ContaCorrente contaCorrente,decimal valor)
        {

            if (contaCorrente.Saldo < valor)
                throw new InvalidOperationException("Saldo insuficiente.");
        }

        private void ContaEstaAtiva(ContaCorrente contaCorrente)
        {
            if (!contaCorrente.Ativa)
                throw new InvalidOperationException(
                    "A conta de origem está bloqueada.");
        }

        public void Debitar(ContaCorrente contaOrigem,decimal valorDebitar)
        {
            contaOrigem.Saldo = contaOrigem.Saldo - valorDebitar;
        }

        public void Creditar(ContaCorrente contaDestino, decimal valorDebitar)
        {
            contaDestino.Saldo = contaDestino.Saldo + valorDebitar;
        }
    }
}
