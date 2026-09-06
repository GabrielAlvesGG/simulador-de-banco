using simulador_de_banco.Domain.Entidade;
using Xunit;


namespace SimuladorBanco.Tests;

public class ContaCorrenteTests
{
    [Fact]
    public void Debitar_DeveDiminuirSaldo_QuandoValorForValido()
    {
        // Preparar: criar uma conta com saldo conhecido.
        var conta = new ContaCorrente(
            id: 1,
            numero: "12345",
            nome: "Gabriel",
            email: "gabriel@example.com",
            saldo: 1000m,
            ativa: true);

        // Executar: chamar o método que queremos testar.
        conta.Debitar(300m);

        // Verificar: comparar o esperado com o resultado real.
        Assert.Equal(700m, conta.Saldo);
    }
    [Fact]
    public void Creditar_Aumenta_Valor_Atribuido_Para_Conta()
    {
        var conta = new ContaCorrente(
         id: 2,
         numero: "123456",
         nome: "Joao",
         email: "joao@example.com",
         saldo: 1000m,
         ativa: true);

        conta.Creditar(500m);

        Assert.Equal(1500m, conta.Saldo);
    }
}