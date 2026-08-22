1 - Primeiro ponto a classe que está na /services/ContaCorrenteService ela contem uma séries de defices técnicos e preciso implementar.

passo 1 - Migrar as responsabilidades com relação com o banco para infrastrutura da aplicação.

		* A primeira coisa a ser feito foi pegar todos os métodos que persistem no banco de dados e passar eles para infrastrutura. *

		* Consegui passar toda a lógica para infrastructure e também criar repository e interface. Implementei as assinaturas dentro da classe*
		* Fiz o mesmo para o ContaCorrenteServices criei uma interface para ele e conseguir conectar todo o fluxo de desenvolvimento. *

		$ Porém agora estou começando a ter uma excesão estourando a todo momento que tento rodar a aplicação, quando eu faço dotnet build ele não dá problema porém quando tento rodar o projeto com dotnet run ele quebra e retorna essa exception 
		"System.AggregateException: 'Some services are not able to be constructed (Error while validating the service descriptor 'ServiceType: simulador_de_banco.Application.Interface.IContaCorrenteService Lifetime: Scoped ImplementationType: simulador_de_banco.Application.Services.ContaCorrenteService': Unable to resolve service for type 'System.String' while attempting to activate 'simulador_de_banco.Infrastructure.Repostory.TransacoesRepository'.) (Error while validating the service descriptor 'ServiceType: simulador_de_banco.Infrastructure.Interface.ITransacoesRepository Lifetime: Scoped ImplementationType: simulador_de_banco.Infrastructure.Repostory.TransacoesRepository': Unable to resolve service for type 'System.String' while attempting to activate 'simulador_de_banco.Infrastructure.Repostory.TransacoesRepository'.)'"

		$ Resolução do problema "System.AggreagateException" era que estava tentando passar uma string no construtor da classe TransacaoRepository.cs isso faz com que gere uma excessão.