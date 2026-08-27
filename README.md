# Simulador de Banco

Projeto desenvolvido em **C#/.NET** com o objetivo de praticar conceitos de desenvolvimento de software através da implementação progressiva de funcionalidades e desafios utilizando um domínio bancário.

A aplicação será utilizada como um ambiente de estudo para implementar, analisar e refatorar diferentes situações encontradas em sistemas reais.

## Objetivo

O objetivo deste projeto é evoluir um sistema bancário gradualmente, utilizando cada nova necessidade como oportunidade para estudar e aplicar conceitos de engenharia de software.

Durante o desenvolvimento serão trabalhados temas como:

* Orientação a Objetos;
* SOLID;
* Clean Code;
* Design Patterns;
* Dependency Injection;
* acesso a banco de dados;
* APIs REST;
* integrações externas;
* tratamento de transações;
* concorrência;
* segurança;
* testes;
* mensageria;
* cache;
* arquitetura de software;
* escalabilidade.

O projeto será dividido em diferentes desafios, permitindo acompanhar a evolução do código e compreender o motivo de cada decisão tomada durante o desenvolvimento.

---

## Desafios

### Desafio 01 — Refatoração de uma God Class

O projeto inicia com uma classe chamada `ContaCorrenteService`, responsável por praticamente todo o fluxo de uma transferência bancária.

Entre suas responsabilidades estão:

* validar os dados da transferência;
* consultar contas no banco de dados;
* executar regras de negócio;
* consultar um serviço antifraude;
* calcular novos saldos;
* atualizar os saldos das contas;
* registrar a movimentação;
* controlar a transação do banco;
* enviar e-mail;
* gerar comprovante;
* salvar o comprovante em arquivo.

Essa concentração de responsabilidades caracteriza um problema conhecido como **God Class**.

O objetivo deste primeiro desafio é analisar essa implementação e refatorá-la progressivamente utilizando boas práticas de desenvolvimento.

📚 Documentação do desafio:

`docs/desafios/01-god-class.md`

---

### Próximos desafios

Novos desafios serão adicionados conforme a evolução do projeto.

| Desafio | Tema                     | Status             |
| ------- | ------------------------ | ------------------ |
| 01      | Refatoração de God Class | Em desenvolvimento |
| 02      | A definir                | Não iniciado       |
| 03      | A definir                | Não iniciado       |

---

## Diário de desenvolvimento

Cada desafio possui sua própria documentação contendo:

* problema identificado;
* análise inicial;
* conceitos relacionados;
* decisões tomadas;
* alterações realizadas;
* comparação antes e depois;
* aprendizados obtidos.

A documentação está disponível no diretório:

```text
docs/desafios/
```

---

## Tecnologias

Inicialmente o projeto utiliza:

* C#
* .NET
* ASP.NET Core
* SQL Server
* ADO.NET
* HttpClient

Novas tecnologias poderão ser adicionadas conforme os desafios forem implementados.

---

## Estrutura do projeto

```text
simulador-de-banco/
│
├── README.md
│
├── docs/
│   └── desafios/
│       ├── 01-god-class.md
│       ├── 02-...
│       └── 03-...
│
├── Controller/
│
└── Services/
```

---

## Proposta de estudo

Este projeto não possui como objetivo apenas chegar a uma implementação final.

A proposta é registrar a evolução do código para compreender:

**qual era o problema → por que ele era um problema → qual conceito pode resolvê-lo → como a solução foi implementada → qual foi o resultado.**

Dessa forma, o próprio histórico de desenvolvimento do projeto funciona como material de estudo e revisão.
