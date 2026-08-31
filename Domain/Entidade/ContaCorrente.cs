namespace simulador_de_banco.Domain.Entidade;

public class ContaCorrente
{
    public int Id { get; }
    public string Numero { get; }
    public string Nome { get; }
    public string Email { get; }
    public decimal Saldo { get; private set; }
    public bool Ativa { get; private set; }

    public ContaCorrente(
        int id,
        string numero,
        string nome,
        string email,
        decimal saldo,
        bool ativa)
    {
        if (id <= 0)
            throw new ArgumentException("Identificador de conta inválido.");

        // Considerando que este simulador não permite cheque especial.
        if (saldo < 0)
            throw new ArgumentException("O saldo não pode ser negativo.");

        Id = id;
        Numero = numero;
        Nome = nome;
        Email = email;
        Saldo = saldo;
        Ativa = ativa;
    }

    public void Debitar(decimal valor)
    {
        ValidarValorPositivo(valor);
        GarantirContaAtiva();

        if (Saldo < valor)
            throw new InvalidOperationException("Saldo insuficiente.");

        Saldo -= valor;
    }

    public void Creditar(decimal valor)
    {
        ValidarValorPositivo(valor);
        GarantirContaAtiva();

        Saldo += valor;
    }

    private static void ValidarValorPositivo(decimal valor)
    {
        if (valor <= 0)
            throw new ArgumentException(
                "O valor deve ser maior que zero.");
    }

    private void GarantirContaAtiva()
    {
        if (!Ativa)
            throw new InvalidOperationException(
                "A conta está bloqueada.");
    }
}