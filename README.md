# API de cadastro de questões e geração de provas

Projeto final de **Programação Orientada a Objeto 2** (FSG, 2026/2): uma API em .NET 10 que
cadastra questões de múltipla escolha, gera provas sorteando questões por disciplina e conteúdo,
e permite consultar questões e provas.

## Como executar

Pré-requisito: [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
dotnet run --project src/GeradorProvas.Apresentacao
```

A API sobe em `http://localhost:5000`. Abra
`src/GeradorProvas.Apresentacao/GeradorProvas.http` no Visual Studio, VS Code (extensão REST Client)
ou Rider e execute as requisições de cima para baixo: o arquivo cadastra questões, lista, gera
provas, consulta e, no final, mostra os casos de erro.

Os dados ficam em memória: ao reiniciar a API, tudo volta ao zero e os ids recomeçam em 1.

## Estrutura em camadas

```
GeradorProvas.Apresentacao  ──►  GeradorProvas.Aplicacao  ──►  GeradorProvas.Dominio
            │                              ▲
            └──►  GeradorProvas.Infraestrutura ┘
```

| Projeto | Responsabilidade | Principais classes |
|---|---|---|
| `GeradorProvas.Dominio` | Entidades que validam a si mesmas. Não referencia nenhum projeto. | `Alternativa`, `Questao`, `Prova`, `RegraNegocioException` |
| `GeradorProvas.Aplicacao` | Casos de uso, contratos (interfaces), DTOs e conversões. | `ServicoCadastroQuestao`, `ServicoConsultaQuestao`, `ServicoGeradorProva`, `ServicoConsultaProva`, `MapeadorProva`, `MapeadorQuestao` |
| `GeradorProvas.Infraestrutura` | Implementações concretas dos contratos. | `RepositorioQuestoesEmMemoria`, `RepositorioProvasEmMemoria`, `EmbaralhadorAleatorio` |
| `GeradorProvas.Apresentacao` | HTTP: controllers, tradução de exceções e composição da injeção de dependência. | `QuestoesController`, `ProvasController`, `TratadorDeExcecoes`, `Program.cs` |

Configurações comuns (`net10.0`, C# 14, nullable, namespace raiz) ficam em
`Directory.Build.props`, para não serem repetidas nos quatro `.csproj`.

## Endpoints

| Requisito | Endpoint | Sucesso | Erros |
|---|---|---|---|
| RF01 — Cadastrar questão | `POST /questoes` | `201` com `{ "id": n }` | `400` (RN01) |
| RF02 — Consultar questão | `GET /questoes/{id}` | `200` | `404` (RN06) |
| RF03 — Listar questões | `GET /questoes?disciplina=POO%20II&conteudo=SOLID` | `200` (lista, pode ser vazia) | `400` sem disciplina |
| RF04 — Gerar prova | `POST /provas` | `201` com a prova | `400` dados inválidos, `422` (RN02) |
| RF05 — Consultar prova | `GET /provas/{id}` | `200` | `404` (RN06) |

Os erros seguem o formato padrão `ProblemDetails` (`application/problem+json`), com a mensagem
em português no campo `detail`. Os filtros de disciplina e conteúdo ignoram maiúsculas e minúsculas.

## O caminho do `POST /provas` pelas camadas

1. **Apresentação** — `ProvasController.Gerar` recebe o `GeracaoProvaDto` e só chama
   `ServicoGeradorProva.GerarProva`. Não há regra no controller.
2. **Aplicação** — `ServicoGeradorProva.GerarProva`:
   1. valida o que não pertence à `Prova` (pelo menos um conteúdo, número de questões > 0);
   2. pede ao `IRepositorioQuestoes` as questões da disciplina com um dos conteúdos (`Filtrar`);
   3. se vieram menos questões do que o pedido, lança `RegraNegocioException` (**RN02** → `422`);
   4. sorteia: o `IEmbaralhador` embaralha e o serviço pega as `numeroQuestoes` primeiras (**RN03**);
   5. cria a `Prova` com `IRepositorioProvas.ObterProximoId()` (**RN05**) e a adiciona ao repositório;
   6. devolve `MapeadorProva.ParaDto(prova)`, que numera as questões a partir de 1 e dá as letras
      A, B, C... às alternativas (**RN04**).
3. **Domínio** — o construtor de `Prova` garante nome, disciplina, pelo menos uma questão e
   nenhuma questão repetida.
4. **Infraestrutura** — `RepositorioQuestoesEmMemoria` filtra a lista, `EmbaralhadorAleatorio`
   embaralha com o `Random` recebido no construtor e `RepositorioProvasEmMemoria` guarda a prova.
5. **Apresentação** — o controller responde `201 Created`, com o header `Location: /provas/{id}`
   e o JSON da prova. Se alguma camada lançar exceção, o `TratadorDeExcecoes` a converte no
   status certo.

Como a prova fica guardada já com as questões na ordem do sorteio, `GET /provas/{id}` devolve
sempre a mesma prova (**RF05**): consultar não sorteia de novo.

## Conteúdos da disciplina no projeto

| Conteúdo | Onde aparece |
|---|---|
| Encapsulamento | `Questao` e `Prova` validam a si mesmas no construtor e expõem suas listas como `IReadOnlyList`. As propriedades não têm `set`. |
| SRP | Um serviço por responsabilidade; `MapeadorProva` e `MapeadorQuestao` concentram as conversões; `TratadorDeExcecoes` concentra o mapeamento exceção → HTTP. |
| DIP e injeção de dependência | Os serviços recebem repositórios e embaralhador por interface, no construtor; o `Random` também é injetado (não há `new Random()` em serviço). Tudo é composto no `Program.cs`. |
| Repository e Service Layer | `IRepositorioQuestoes`, `IRepositorioProvas` e os quatro serviços. |
| DTO | `record`s de entrada e saída; as classes de domínio não aparecem na API. |
| Camadas | Quatro projetos, com as dependências apontando para o domínio. |

## Decisões que valem explicar

- **Tratador de exceções único**: em vez de repetir `try/catch` em cada action, o `TratadorDeExcecoes`
  (`IExceptionHandler`) traduz `ArgumentException` → `400`, `RecursoNaoEncontradoException` → `404`
  e `RegraNegocioException` → `422`.
- **Campos ausentes no JSON**: como as propriedades dos DTOs não são anuláveis, o `[ApiController]`
  já responde `400` quando um campo obrigatório nem é enviado. As regras restantes (2 a 5
  alternativas, exatamente uma correta, texto em branco) são verificadas pelo domínio.
- **Repositórios Singleton com trava**: a mesma lista atende requisições simultâneas, então
  `lock` protege a lista e o contador de ids.
- **Ids sequenciais**: seguindo o fluxo do enunciado, o serviço pede o próximo id antes de criar a
  entidade. Se a criação falhar (dados inválidos), aquele número não é reaproveitado.
