1 - Primeiro ponto a classe que está na /services/ContaCorrenteService ela contem uma séries de defices técnicos e preciso implementar.

passo 1 - Migrar as responsabilidades com relação com o banco para infrastrutura da aplicação.

		* A primeira coisa a ser feito foi pegar todos os métodos que persistem no banco de dados e passar eles para infrastrutura. *

		* Consegui passar toda a lógica para infrastructure e também criar repository e interface. Implementei as assinaturas dentro da classe*
		* Fiz o mesmo para o ContaCorrenteServices criei uma interface para ele e conseguir conectar todo o fluxo de desenvolvimento. *

		$ Porém agora estou começando a ter uma excesão estourando a todo momento que tento rodar a aplicação, quando eu faço dotnet build ele não dá problema porém quando tento rodar o projeto com dotnet run ele quebra e retorna essa exception 
		"System.AggregateException: 'Some services are not able to be constructed (Error while validating the service descriptor 'ServiceType: simulador_de_banco.Application.Interface.IContaCorrenteService Lifetime: Scoped ImplementationType: simulador_de_banco.Application.Services.ContaCorrenteService': Unable to resolve service for type 'System.String' while attempting to activate 'simulador_de_banco.Infrastructure.Repostory.TransacoesRepository'.) (Error while validating the service descriptor 'ServiceType: simulador_de_banco.Infrastructure.Interface.ITransacoesRepository Lifetime: Scoped ImplementationType: simulador_de_banco.Infrastructure.Repostory.TransacoesRepository': Unable to resolve service for type 'System.String' while attempting to activate 'simulador_de_banco.Infrastructure.Repostory.TransacoesRepository'.)'"

		$ Resolução do problema "System.AggreagateException" era que estava tentando passar uma string no construtor da classe TransacaoRepository.cs isso faz com que gere uma excessão.

		$ Outra questão muito interessante que estava fazendo errado é que a interface que estava criando para fazer a injeção de dependência da infrastrutura estava implementada da forma errada. O motivo é que criei a interface na camada de respositório fazendo com que a application continua-se dependendo de algo que não está na camada dela tendo que ir buscar na camada dos diretórios de infrastrutura, assim para resolver essa depependência movi a interface que estava na infrastrutura para application e a classe TransacoesRepository que implementa a interface nela consegue buscar da camada de application. Com isso resolvendo o problema da application conhecer a camada de infra.

		* Começando uma nova etapa da refatoração voltado para as classes que contém propriedades, temos três classes nessa god class, aprimeira é uma entidade a classe ContaCorrente. Ela representa uma regra de negócio então vou criar uma pasta domain para direcionar a classe ContaCorrente até lá e assim conseguir desacoplar a regra de negócio com isso. 
		* Segundo ponto é conseguir verificar a classe que está sendo criada que contém ResultadoTransferencia ela é uma resposta então deve estar dentro do DTOs assim colocando ela no seu devido lugar 
		* A terceira refatoração vai ser da classe ResultadoAntifraude ela deve ir consultar um sistema externo de fraude então essa classe com suas propriedades iram consumir serviços externos. Dessa forma precisando ficar na infraestrutura dentro de algo voltado para Integration Model.

		*Outro ponto que não me atentei durante a minha refatoração dos métodos que utilizava persistência no banco de dados foi o caso de ter regras de validação que deveria ficar na application/services/ContaCorrenteServices.cs e acabei transferindo para infrastructure.
		*Com isso tendo mais pontos de ajuste, pois a cada consulta no banco de dados acaba trazendo mais informações a serem validadas, com isso tendo que ocorrer outras validações que volta para o application services.