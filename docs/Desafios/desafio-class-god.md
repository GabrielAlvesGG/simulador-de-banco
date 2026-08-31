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

		* Outro ponto que não me atentei durante a minha refatoração dos métodos que utilizava persistência no banco de dados foi o caso de ter regras de validação que deveria ficar na application/services/ContaCorrenteServices.cs e acabei transferindo para infrastructure.
		* Com isso tendo mais pontos de ajuste, pois a cada consulta no banco de dados acaba trazendo mais informações a serem validadas, com isso tendo que ocorrer outras validações que volta para o application services.
		* Um dos grandes pontos agora tá sendo fazer a implementação do unit of work para que a transação e a conexão não se perca durante a validação dos dados. A primeira coisa que vou fazer é conseguir implementar a unit of work para que tenhamos mais segurança na consistencia dos dados.Criando uma pasta dentro de infrastructure chamada persistência
		* Acabei de conseguir implementar a classe SqlUnitOfWork para conseguir fazer o desacoplamento do código e manter a instência correta da transação e também da conexão para conseguir manter o fluxo de todas as etapas caso de certo e remover caso de errado.
		* Agora que a classe está criada com todas as responsabilidades devida dela preciso conseguir criar a utilização dela dentro do repository. 
		* Foi jogado todas as validações para o services, depois tenho que dividir corretamente as lógicas que é responsabilidade do services e outras que são responsabilidade do domain.
		* Acabamos de notar o cenário do unit of work na questão de injeção de deendência sobre a questão de desacoplar a parte de conexão e transação sem fazer com que application acabe descobrindo o que está acontecendo nesses dois pontos a melhor solução foi implementar uma interface que está dentro da application para utilizar o begin, commit e rollback e para o repository dentro da infrastructure que necessita saber da transação e também conexão a parte da interface nessa parte do diretório.
		* Tive que fazer outro ajuste dentro da criação das injeções de dependências, pelo fato que as duas interfaces estavam apontando para instanciâncias diferentes. Outro problema também é que dentro do services os métodos que chamava para atualizar os saldos das contas e também o commit e rollback do unit of work estava sem o await para aguardar o fluxo dentro desses métodos ternimar para continuar.
		* Nesse trecho da refatoração, foi retirado toda a questão de validação do antifraude do repository e implementado no seu devido lugar dentro da infrastructure criado uma interface para que a application consiga requisita a validação da antifraude e não precise saber de seu contexto de validação da transação.
		* O trecho que foi refatorado agora está relacionado ao email, consegui refatorar o email e colocar ele nos diretório devido a ele para suas responsabilidades. 
		* Implementado refatoração também da questão de extrato de dados fazendo com que o estrato seja salvo dentro do servidor assim que é feito a transação.
		
		* Agora focando em refatoração dos parâmetros
		* na refatoração dos parâmetros tive que entender melhor como funciona cada parte de camadas e qual camada podia conhecer parâmetros e instancias de outra camda a interface da application não pode conhecer classe relacionadas a repository por exemplo. Então tive que fazer uma boa refatoração para conseguir encapsular corretamente as propriedades.
		* O ajuste atual está nas classes que foram criadas erradas dentro da domain. Vamos movelas para os lugares certos, começando com a notifications o real lugar dela é dentro de application.
		* Nesse momento estou começando a extrair os contextos em questão de responsabilidade de cada uma e criar um monolito modular.
		* O objetivo atual é conseguir pegar as regras do dominio e conseguir passar para ele de forma correta.
		* Tem muita coisa que era para ser feito dentro do dominio com entidade ou object value dentro do DDD, que estou afim de começar a implementar e também começar a entender como funciona.
		* Uma das primeiras atitudes que tive nessa refatoração foi de retirar a referência que a entidade ContaCorrente tinha da application, pois estava dentro da domain a domain não contém dependencia de ninguém. Primeiro motivo que ela não estava sendo usada.

		* Outro ponto dentro da refatoração que está sendo feita dentro da pasta services é sobre a validação da injeção de dependência IMapperRequests ela não estava sendo referência no construtor.
		* Ajustando a injeção de dependência que não tinha sido implementada dentro do controller do services relacionado ao MapperRequests.
		* Mais um ponto que agora vamos corrigir é a forma que atualizamos os dados dentro do nosso projeto, existe um problema na forma que temos a assinatura do método AtualizarSaldoAsync(). Os parâmetros de conta.Id e também poder passa o valor, faz com que isso seja uma liberdade que o sistema não pode ter, isso tem que fazer com que ele limite a atualizar o saldo dele de forma correta. Não podendo passar o valor mais sim fazendo da forma que o valor está dentro da entidade da instância.
