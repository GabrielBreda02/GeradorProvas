# Roteiro de estudo: onde está cada coisa no código

Na apresentação, o professor passa de classe em classe e pergunta "onde está tal coisa?".
Este roteiro existe para treinar isso. Para cada pergunta provável, ele diz **qual arquivo abrir
e quais linhas mostrar**.

**Como estudar:**
1. Leia o [mapa da solução](#1-mapa-da-solução) para saber o que fica em cada projeto.
2. Faça o [tour classe por classe](#2-tour-classe-por-classe) com o código aberto ao lado.
3. Treine com o [índice rápido](#4-índice-rápido-onde-está): tape a coluna da direita e
   tente achar sozinho.
4. Estude a [seção do embaralhador](#3-o-embaralhador-interface-e-aberto-fechado-ocp), que é
   a pergunta que o professor já fez em aula.

> Os números de linha valem para a versão atual do código. Se alguém mudar um arquivo, procure
> pelo **nome do método**, que não muda.

### Atalhos para achar rápido

| O que você quer | Visual Studio | VS Code |
|---|---|---|
| Abrir qualquer classe ou método pelo nome | `Ctrl+T` | `Ctrl+T` (símbolo) / `Ctrl+P` (arquivo) |
| Ir para a definição do que está sob o cursor | `F12` | `F12` |
| Ir da interface para a classe que a implementa | `Ctrl+F12` | `Ctrl+F12` |
| Ver todo lugar que usa um método ou classe | `Shift+F12` | `Shift+F12` |
| Ir para uma linha | `Ctrl+G` | `Ctrl+G` |

`Ctrl+F12` em cima de `IEmbaralhador` leva direto ao `EmbaralhadorAleatorio`, e
`Shift+F12` em cima de `Embaralhar` mostra onde o serviço o usa. Isso impressiona na hora.

---

## 1. Mapa da solução

```
GeradorProvas.sln
├── src/
│   ├── GeradorProvas.Dominio          → entidades e regras (não depende de ninguém)
│   ├── GeradorProvas.Aplicacao        → serviços, interfaces (contratos), DTOs, mapeadores
│   ├── GeradorProvas.Infraestrutura   → repositórios em memória e embaralhador
│   └── GeradorProvas.Apresentacao     → controllers, Program.cs, tratador de exceções, .http
└── tests/
    └── GeradorProvas.Testes           → testes automatizados (não é camada)
```

**"Mostre que o Domínio não depende de ninguém"**: abra
[`GeradorProvas.Dominio.csproj`](../src/GeradorProvas.Dominio/GeradorProvas.Dominio.csproj).
Ele não tem nenhum `<ProjectReference>`.

**"Mostre as dependências entre as camadas"**: as referências ficam nos `.csproj`:

| Projeto | Referencia |
|---|---|
| [Dominio](../src/GeradorProvas.Dominio/GeradorProvas.Dominio.csproj) | ninguém |
| [Aplicacao](../src/GeradorProvas.Aplicacao/GeradorProvas.Aplicacao.csproj) | Dominio |
| [Infraestrutura](../src/GeradorProvas.Infraestrutura/GeradorProvas.Infraestrutura.csproj) | Aplicacao (para implementar as interfaces) e Dominio |
| [Apresentacao](../src/GeradorProvas.Apresentacao/GeradorProvas.Apresentacao.csproj) | Aplicacao e Infraestrutura (só para registrar as implementações no `Program.cs`) |

Todas as setas apontam para o Domínio. A Aplicação **não** conhece a Infraestrutura: ela só
conhece as interfaces, e quem liga uma coisa à outra é o `Program.cs`.

**"Onde está o .NET 10 / C# 14 / o namespace padrão?"**: em
[`Directory.Build.props`](../Directory.Build.props), que vale para todos os projetos.

---

## 2. Tour classe por classe

### Domínio (`src/GeradorProvas.Dominio`)

#### `Alternativa`, [Alternativa.cs](../src/GeradorProvas.Dominio/Alternativa.cs)
Um texto e se é a correta.
- *Onde valida o texto?* Linha [7](../src/GeradorProvas.Dominio/Alternativa.cs#L7) (`Validacao.TextoObrigatorio`).
- *Dá para mudar o texto depois?* Não: as propriedades só têm `get` ([11-13](../src/GeradorProvas.Dominio/Alternativa.cs#L11-L13)).

#### `Questao`, [Questao.cs](../src/GeradorProvas.Dominio/Questao.cs)
Questão de múltipla escolha. Valida a si mesma no construtor (RN01).
- *Onde está a RN01?* No construtor ([10-20](../src/GeradorProvas.Dominio/Questao.cs#L10-L20)):
  - campos obrigatórios nas linhas [13-15](../src/GeradorProvas.Dominio/Questao.cs#L13-L15);
  - 2 a 5 alternativas em [32-37](../src/GeradorProvas.Dominio/Questao.cs#L32-L37), com os limites nas constantes das linhas [5-6](../src/GeradorProvas.Dominio/Questao.cs#L5-L6);
  - exatamente uma correta em [39-43](../src/GeradorProvas.Dominio/Questao.cs#L39-L43).
- *Onde está o encapsulamento?* A lista é um campo privado ([8](../src/GeradorProvas.Dominio/Questao.cs#L8)) e
  sai para fora só como leitura ([30](../src/GeradorProvas.Dominio/Questao.cs#L30), `AsReadOnly()`).
  O construtor **copia** a lista recebida ([16](../src/GeradorProvas.Dominio/Questao.cs#L16)), então
  quem criou a questão não consegue mexer nela depois.
- *Que exceção lança?* `ArgumentException`, que vira **400**.

#### `Prova`, [Prova.cs](../src/GeradorProvas.Dominio/Prova.cs)
A prova gerada, com as questões na ordem do sorteio.
- *Onde exige nome e disciplina?* Linhas [10-11](../src/GeradorProvas.Dominio/Prova.cs#L10-L11).
- *Onde exige pelo menos uma questão?* [26-30](../src/GeradorProvas.Dominio/Prova.cs#L26-L30).
- *Onde impede questão repetida (RN03)?* [32-38](../src/GeradorProvas.Dominio/Prova.cs#L32-L38). Compara
  quantos ids distintos existem com o total. Lança `RegraNegocioException`.
- *Encapsulamento?* Igual à `Questao`: campo privado ([5](../src/GeradorProvas.Dominio/Prova.cs#L5)) e
  `IReadOnlyList` ([24](../src/GeradorProvas.Dominio/Prova.cs#L24)).

#### `RegraNegocioException`, [RegraNegocioException.cs](../src/GeradorProvas.Dominio/RegraNegocioException.cs)
Exceção de regra de negócio. Vira **422**. É lançada na RN02 (gerador de prova) e na
repetição de questões (`Prova`).

#### `Validacao`, [Validacao.cs](../src/GeradorProvas.Dominio/Validacao.cs)
Classe `internal static` que guarda a regra de "campo obrigatório" e a mensagem dela em um só
lugar ([9-15](../src/GeradorProvas.Dominio/Validacao.cs#L9-L15)). Usada por `Alternativa`, `Questao` e
`Prova`. É `internal`, então só o Domínio enxerga.

### Aplicação: contratos (`src/GeradorProvas.Aplicacao/Contratos`)

#### `IRepositorioQuestoes`, [IRepositorioQuestoes.cs](../src/GeradorProvas.Aplicacao/Contratos/IRepositorioQuestoes.cs)
O que um repositório de questões precisa saber fazer. `Listar` (RF03, linha
[14](../src/GeradorProvas.Aplicacao/Contratos/IRepositorioQuestoes.cs#L14)) recebe **um** conteúdo opcional;
`Filtrar` (RF04, linha [17](../src/GeradorProvas.Aplicacao/Contratos/IRepositorioQuestoes.cs#L17))
recebe **uma lista** de conteúdos.

#### `IRepositorioProvas`, [IRepositorioProvas.cs](../src/GeradorProvas.Aplicacao/Contratos/IRepositorioProvas.cs)
Próximo id, adicionar e buscar por id.

#### `IEmbaralhador`, [IEmbaralhador.cs](../src/GeradorProvas.Aplicacao/Contratos/IEmbaralhador.cs)
Um método só: `Embaralhar` ([7](../src/GeradorProvas.Aplicacao/Contratos/IEmbaralhador.cs#L7)). É o centro
da pergunta sobre aberto/fechado. Veja a [seção 3](#3-o-embaralhador-interface-e-aberto-fechado-ocp).

*Por que as interfaces ficam na Aplicação e não na Infraestrutura?* Porque quem **usa** é que
define o contrato (DIP). O serviço diz "preciso de algo que embaralhe", e a Infraestrutura se adapta a isso.

### Aplicação: DTOs (`src/GeradorProvas.Aplicacao/Dtos`)
São `record`s de entrada e saída da API. A entidade de domínio nunca sai pela API.

| Entrada | Saída |
|---|---|
| [`CadastroQuestaoDto`](../src/GeradorProvas.Aplicacao/Dtos/CadastroQuestaoDto.cs) + [`AlternativaCadastroDto`](../src/GeradorProvas.Aplicacao/Dtos/AlternativaCadastroDto.cs) (RF01) | [`QuestaoDto`](../src/GeradorProvas.Aplicacao/Dtos/QuestaoDto.cs) + [`AlternativaDto`](../src/GeradorProvas.Aplicacao/Dtos/AlternativaDto.cs) (RF02/RF03) |
| [`GeracaoProvaDto`](../src/GeradorProvas.Aplicacao/Dtos/GeracaoProvaDto.cs) (RF04) | [`ProvaDto`](../src/GeradorProvas.Aplicacao/Dtos/ProvaDto.cs) + [`QuestaoProvaDto`](../src/GeradorProvas.Aplicacao/Dtos/QuestaoProvaDto.cs) (RF04/RF05) |

- *Onde está a `letraCorreta`?* No `QuestaoDto` ([9](../src/GeradorProvas.Aplicacao/Dtos/QuestaoDto.cs#L9)).
  O `QuestaoProvaDto` **não** tem, porque a prova não entrega o gabarito.
- *Por que `record`?* São só dados, imutáveis, e o C# gera construtor e propriedades em uma linha.

### Aplicação: mapeadores (`src/GeradorProvas.Aplicacao/Mapeadores`)

#### `MapeadorQuestao`, [MapeadorQuestao.cs](../src/GeradorProvas.Aplicacao/Mapeadores/MapeadorQuestao.cs)
Converte `Questao` em `QuestaoDto` e concentra a **RN04** (letras).
- *Onde as alternativas ganham A, B, C?* [21-24](../src/GeradorProvas.Aplicacao/Mapeadores/MapeadorQuestao.cs#L21-L24),
  e o cálculo da letra fica em [29](../src/GeradorProvas.Aplicacao/Mapeadores/MapeadorQuestao.cs#L29)
  (`'A' + posição`).
- *Onde descobre a letra correta?* [26-27](../src/GeradorProvas.Aplicacao/Mapeadores/MapeadorQuestao.cs#L26-L27).

#### `MapeadorProva`, [MapeadorProva.cs](../src/GeradorProvas.Aplicacao/Mapeadores/MapeadorProva.cs)
Converte `Prova` em `ProvaDto`. É usado pelos **dois** serviços de prova (gerar e consultar), para
não repetir a conversão.
- *Onde numera as questões a partir de 1?* Linha [12](../src/GeradorProvas.Aplicacao/Mapeadores/MapeadorProva.cs#L12) (`posicao + 1`).
- *Onde as alternativas da prova ganham letras?* Linha [18](../src/GeradorProvas.Aplicacao/Mapeadores/MapeadorProva.cs#L18),
  que reaproveita o `MapeadorQuestao`.

### Aplicação: exceção

#### `RecursoNaoEncontradoException`, [RecursoNaoEncontradoException.cs](../src/GeradorProvas.Aplicacao/Excecoes/RecursoNaoEncontradoException.cs)
Lançada quando o id não existe. Vira **404** (RN06).

### Aplicação: serviços (`src/GeradorProvas.Aplicacao/Servicos`)
Um serviço por responsabilidade (SRP). Todos recebem as dependências **pelo construtor**,
como interface.

#### `ServicoCadastroQuestao`, [ServicoCadastroQuestao.cs](../src/GeradorProvas.Aplicacao/Servicos/ServicoCadastroQuestao.cs)
RF01. O método `Cadastrar` ([14-28](../src/GeradorProvas.Aplicacao/Servicos/ServicoCadastroQuestao.cs#L14-L28)) faz:
1. converte as alternativas do DTO em `Alternativa` ([16-17](../src/GeradorProvas.Aplicacao/Servicos/ServicoCadastroQuestao.cs#L16-L17));
2. pega o próximo id ([20](../src/GeradorProvas.Aplicacao/Servicos/ServicoCadastroQuestao.cs#L20)) e cria a `Questao`, que se valida sozinha;
3. guarda no repositório ([26](../src/GeradorProvas.Aplicacao/Servicos/ServicoCadastroQuestao.cs#L26)) e devolve o id.

#### `ServicoConsultaQuestao`, [ServicoConsultaQuestao.cs](../src/GeradorProvas.Aplicacao/Servicos/ServicoConsultaQuestao.cs)
- RF02, `ObterPorId` ([12-18](../src/GeradorProvas.Aplicacao/Servicos/ServicoConsultaQuestao.cs#L12-L18)): lança 404 em [14-15](../src/GeradorProvas.Aplicacao/Servicos/ServicoConsultaQuestao.cs#L14-L15).
- RF03, `Listar` ([20-28](../src/GeradorProvas.Aplicacao/Servicos/ServicoConsultaQuestao.cs#L20-L28)): a disciplina é obrigatória ([22-23](../src/GeradorProvas.Aplicacao/Servicos/ServicoConsultaQuestao.cs#L22-L23)).

#### `ServicoGeradorProva`, [ServicoGeradorProva.cs](../src/GeradorProvas.Aplicacao/Servicos/ServicoGeradorProva.cs)
RF04. **É a classe mais importante da apresentação.**
- *Do que ele depende?* De três interfaces ([7-9](../src/GeradorProvas.Aplicacao/Servicos/ServicoGeradorProva.cs#L7-L9)), recebidas no construtor ([11-19](../src/GeradorProvas.Aplicacao/Servicos/ServicoGeradorProva.cs#L11-L19)). Não tem nenhum `new` de repositório ou de `Random`.
- *Passo a passo do `GerarProva`* ([21-36](../src/GeradorProvas.Aplicacao/Servicos/ServicoGeradorProva.cs#L21-L36)):
  1. valida conteúdos e número de questões ([23](../src/GeradorProvas.Aplicacao/Servicos/ServicoGeradorProva.cs#L23); regra em [39-46](../src/GeradorProvas.Aplicacao/Servicos/ServicoGeradorProva.cs#L39-L46));
  2. filtra as questões ([25](../src/GeradorProvas.Aplicacao/Servicos/ServicoGeradorProva.cs#L25));
  3. verifica a RN02 ([26](../src/GeradorProvas.Aplicacao/Servicos/ServicoGeradorProva.cs#L26); regra em [48-55](../src/GeradorProvas.Aplicacao/Servicos/ServicoGeradorProva.cs#L48-L55));
  4. sorteia ([32](../src/GeradorProvas.Aplicacao/Servicos/ServicoGeradorProva.cs#L32); `Sortear` em [57-59](../src/GeradorProvas.Aplicacao/Servicos/ServicoGeradorProva.cs#L57-L59): embaralha e pega as N primeiras com `Take`);
  5. cria a `Prova` com o próximo id ([28-32](../src/GeradorProvas.Aplicacao/Servicos/ServicoGeradorProva.cs#L28-L32));
  6. guarda ([34](../src/GeradorProvas.Aplicacao/Servicos/ServicoGeradorProva.cs#L34)) e devolve o DTO ([35](../src/GeradorProvas.Aplicacao/Servicos/ServicoGeradorProva.cs#L35)).

#### `ServicoConsultaProva`, [ServicoConsultaProva.cs](../src/GeradorProvas.Aplicacao/Servicos/ServicoConsultaProva.cs)
RF05. Busca a prova guardada ([12-18](../src/GeradorProvas.Aplicacao/Servicos/ServicoConsultaProva.cs#L12-L18)).
**Não sorteia de novo**: ele nem tem embaralhador. Só devolve o que foi guardado na geração.

### Infraestrutura (`src/GeradorProvas.Infraestrutura`)

#### `RepositorioQuestoesEmMemoria`, [RepositorioQuestoesEmMemoria.cs](../src/GeradorProvas.Infraestrutura/RepositorioQuestoesEmMemoria.cs)
Implementa `IRepositorioQuestoes` ([6](../src/GeradorProvas.Infraestrutura/RepositorioQuestoesEmMemoria.cs#L6)).
- *Onde ficam os dados?* Na lista privada ([8](../src/GeradorProvas.Infraestrutura/RepositorioQuestoesEmMemoria.cs#L8)), sem banco de dados.
- *Onde gera o id (RN05)?* [15-19](../src/GeradorProvas.Infraestrutura/RepositorioQuestoesEmMemoria.cs#L15-L19), com o contador `_ultimoId` ([13](../src/GeradorProvas.Infraestrutura/RepositorioQuestoesEmMemoria.cs#L13)).
- *Onde filtra por disciplina e conteúdos?* `Filtrar` ([38-39](../src/GeradorProvas.Infraestrutura/RepositorioQuestoesEmMemoria.cs#L38-L39)), com os critérios em [47-51](../src/GeradorProvas.Infraestrutura/RepositorioQuestoesEmMemoria.cs#L47-L51).
- *Onde ignora maiúsculas e minúsculas?* [53-55](../src/GeradorProvas.Infraestrutura/RepositorioQuestoesEmMemoria.cs#L53-L55).
- *Para que serve o `lock`?* O repositório é Singleton: as mesmas listas atendem várias
  requisições ao mesmo tempo. A trava ([10-11](../src/GeradorProvas.Infraestrutura/RepositorioQuestoesEmMemoria.cs#L10-L11)) impede
  que duas questões recebam o mesmo id.

#### `RepositorioProvasEmMemoria`, [RepositorioProvasEmMemoria.cs](../src/GeradorProvas.Infraestrutura/RepositorioProvasEmMemoria.cs)
Mesma ideia, para provas: lista ([8](../src/GeradorProvas.Infraestrutura/RepositorioProvasEmMemoria.cs#L8)), id ([15-19](../src/GeradorProvas.Infraestrutura/RepositorioProvasEmMemoria.cs#L15-L19)) e busca por id ([27-31](../src/GeradorProvas.Infraestrutura/RepositorioProvasEmMemoria.cs#L27-L31)).

#### `EmbaralhadorAleatorio`, [EmbaralhadorAleatorio.cs](../src/GeradorProvas.Infraestrutura/EmbaralhadorAleatorio.cs)
Implementa `IEmbaralhador` ([6](../src/GeradorProvas.Infraestrutura/EmbaralhadorAleatorio.cs#L6)).
- *De onde vem o `Random`?* Do construtor ([10-13](../src/GeradorProvas.Infraestrutura/EmbaralhadorAleatorio.cs#L10-L13)), injetado pelo `Program.cs`. Ninguém dá `new Random()` aqui dentro.
- *Onde embaralha?* [15-20](../src/GeradorProvas.Infraestrutura/EmbaralhadorAleatorio.cs#L15-L20). Copia para um array e usa `Shuffle` ([18](../src/GeradorProvas.Infraestrutura/EmbaralhadorAleatorio.cs#L18)), que troca posições sem repetir nenhuma questão.

### Apresentação (`src/GeradorProvas.Apresentacao`)

#### `QuestoesController`, [QuestoesController.cs](../src/GeradorProvas.Apresentacao/Controllers/QuestoesController.cs)
- *Onde está a rota `/questoes`?* [7](../src/GeradorProvas.Apresentacao/Controllers/QuestoesController.cs#L7).
- RF01 `POST`: [19-24](../src/GeradorProvas.Apresentacao/Controllers/QuestoesController.cs#L19-L24). O `201 Created` com o id sai da linha [23](../src/GeradorProvas.Apresentacao/Controllers/QuestoesController.cs#L23).
- RF02 `GET /questoes/{id}`: [26-28](../src/GeradorProvas.Apresentacao/Controllers/QuestoesController.cs#L26-L28).
- RF03 `GET /questoes?disciplina=...`: [30-32](../src/GeradorProvas.Apresentacao/Controllers/QuestoesController.cs#L30-L32).
- *Tem regra de negócio aqui?* Não. Cada ação só recebe o DTO e chama o serviço.

#### `ProvasController`, [ProvasController.cs](../src/GeradorProvas.Apresentacao/Controllers/ProvasController.cs)
- RF04 `POST /provas`: [19-24](../src/GeradorProvas.Apresentacao/Controllers/ProvasController.cs#L19-L24).
- RF05 `GET /provas/{id}`: [26-28](../src/GeradorProvas.Apresentacao/Controllers/ProvasController.cs#L26-L28).

#### `TratadorDeExcecoes`, [TratadorDeExcecoes.cs](../src/GeradorProvas.Apresentacao/TratadorDeExcecoes.cs)
- *Onde o 404 acontece?* Aqui, e não no controller: [38-44](../src/GeradorProvas.Apresentacao/TratadorDeExcecoes.cs#L38-L44) traduz
  `ArgumentException` em 400 ([40](../src/GeradorProvas.Apresentacao/TratadorDeExcecoes.cs#L40)),
  `RecursoNaoEncontradoException` em 404 ([41](../src/GeradorProvas.Apresentacao/TratadorDeExcecoes.cs#L41)) e
  `RegraNegocioException` em 422 ([42](../src/GeradorProvas.Apresentacao/TratadorDeExcecoes.cs#L42)).
- *Por que não `try/catch` no controller?* Porque seriam quatro `try/catch` iguais. Assim a
  tradução fica num lugar só (SRP e sem duplicação).

#### `Program.cs`, [Program.cs](../src/GeradorProvas.Apresentacao/Program.cs)
É a **composição**: o único lugar que sabe quais classes concretas existem.
- Repositórios `Singleton` ([12-13](../src/GeradorProvas.Apresentacao/Program.cs#L12-L13)): as listas sobrevivem entre requisições.
- O `Random` compartilhado ([14](../src/GeradorProvas.Apresentacao/Program.cs#L14)).
- Qual embaralhador a API usa ([15](../src/GeradorProvas.Apresentacao/Program.cs#L15)).
- Os quatro serviços `Scoped` ([16-19](../src/GeradorProvas.Apresentacao/Program.cs#L16-L19)).
- O tratador de exceções ([9](../src/GeradorProvas.Apresentacao/Program.cs#L9) e [23](../src/GeradorProvas.Apresentacao/Program.cs#L23)).

#### `GeradorProvas.http`, [GeradorProvas.http](../src/GeradorProvas.Apresentacao/GeradorProvas.http)
O roteiro da demonstração: cadastra questões, consulta, lista, gera provas e mostra os erros.

---

## 3. O embaralhador: interface e aberto/fechado (OCP)

A pergunta do professor foi: *"o embaralhador está como interface? fechado para modificação e
aberto para extensão?"* **Sim.** O desenho do quadro bate com o código:

| No quadro | No código |
|---|---|
| `I Embaralhador` com `Embaralhar(questões)` | [`IEmbaralhador`](../src/GeradorProvas.Aplicacao/Contratos/IEmbaralhador.cs#L5-L8) |
| `Emb Aleatório` (seta para a interface) | [`EmbaralhadorAleatorio : IEmbaralhador`](../src/GeradorProvas.Infraestrutura/EmbaralhadorAleatorio.cs#L6) |
| `Emb Por Assunto` | ainda não existe, mas pode ser criado **sem mexer no serviço** (veja abaixo) |

**O que mostrar, nesta ordem:**
1. O serviço só conhece a interface: campo ([ServicoGeradorProva.cs:9](../src/GeradorProvas.Aplicacao/Servicos/ServicoGeradorProva.cs#L9)),
   construtor ([14](../src/GeradorProvas.Aplicacao/Servicos/ServicoGeradorProva.cs#L14)) e uso ([59](../src/GeradorProvas.Aplicacao/Servicos/ServicoGeradorProva.cs#L59)).
   Em nenhum lugar ele cita `EmbaralhadorAleatorio`.
2. A implementação fica em outra camada ([EmbaralhadorAleatorio.cs](../src/GeradorProvas.Infraestrutura/EmbaralhadorAleatorio.cs)).
3. Quem escolhe a implementação é uma linha só: [Program.cs:15](../src/GeradorProvas.Apresentacao/Program.cs#L15).

**Os princípios envolvidos, em uma frase cada:**
- **OCP (aberto/fechado):** para ter um jeito novo de embaralhar, eu **crio** uma classe nova
  (aberto para extensão) e **não altero** o `ServicoGeradorProva` (fechado para modificação).
- **DIP (inversão de dependência):** o serviço depende da abstração `IEmbaralhador`, não da
  classe concreta.
- **Strategy (padrão de projeto):** cada embaralhador é uma estratégia intercambiável do mesmo
  algoritmo. É a resposta da questão 4 do nosso próprio `.http`.
- **LSP (substituição):** qualquer `IEmbaralhador` pode entrar no lugar do outro e o serviço
  continua funcionando.

**A prova de que funciona já está nos testes:** os testes usam um segundo embaralhador,
[`EmbaralhadorInvertido`](../tests/GeradorProvas.Testes/Apoio/EmbaralhadorInvertido.cs), que só
inverte a lista. O `ServicoGeradorProva` roda com ele sem mudar uma linha
([ServicoGeradorProvaTestes.cs](../tests/GeradorProvas.Testes/Aplicacao/ServicoGeradorProvaTestes.cs)).

### "E se eu quiser embaralhar por assunto?"

Seriam só dois passos, e o serviço não muda:

**1.** Criar `src/GeradorProvas.Infraestrutura/EmbaralhadorPorAssunto.cs`. Este exemplo embaralha
dentro de cada conteúdo e depois intercala os conteúdos, para a prova sair equilibrada entre os
assuntos (com 3 de SOLID, 2 de Design Patterns e 1 de Encapsulamento, as três primeiras são uma
de cada):

```csharp
public class EmbaralhadorPorAssunto : IEmbaralhador
{
    private readonly Random _aleatorio;

    public EmbaralhadorPorAssunto(Random aleatorio)
    {
        _aleatorio = aleatorio;
    }

    // Embaralha dentro de cada assunto (conteúdo) e depois intercala os assuntos em rodízio,
    // para que as primeiras questões da lista já venham equilibradas entre os conteúdos.
    public List<Questao> Embaralhar(IReadOnlyList<Questao> questoes)
    {
        var filasPorAssunto = questoes
            .GroupBy(questao => questao.Conteudo)
            .Select(EmbaralharAssunto)
            .ToList();

        var intercaladas = new List<Questao>();
        for (var rodada = 0; intercaladas.Count < questoes.Count; rodada++)
            intercaladas.AddRange(filasPorAssunto.Where(fila => rodada < fila.Length).Select(fila => fila[rodada]));

        return intercaladas;
    }

    private Questao[] EmbaralharAssunto(IEnumerable<Questao> questoesDoAssunto)
    {
        var embaralhadas = questoesDoAssunto.ToArray();
        _aleatorio.Shuffle(embaralhadas);
        return embaralhadas;
    }
}
```

**2.** Trocar uma linha no [Program.cs:15](../src/GeradorProvas.Apresentacao/Program.cs#L15):

```csharp
builder.Services.AddScoped<IEmbaralhador, EmbaralhadorPorAssunto>();
```

O `ServicoGeradorProva`, a interface, os controllers e os testes do serviço continuam iguais.
É isso que "fechado para modificação, aberto para extensão" quer dizer na prática.

O mesmo vale para os repositórios: se um dia tiver banco de dados, cria-se um
`RepositorioQuestoesSql : IRepositorioQuestoes` e troca-se a linha [12](../src/GeradorProvas.Apresentacao/Program.cs#L12)
do `Program.cs`. Nenhum serviço muda.

---

## 4. Índice rápido: "onde está…?"

Para treinar, tape a coluna da direita.

### Requisitos e regras do enunciado

| Pergunta | Onde mostrar |
|---|---|
| RF01: cadastrar questão | [QuestoesController `Cadastrar`](../src/GeradorProvas.Apresentacao/Controllers/QuestoesController.cs#L19-L24) → [ServicoCadastroQuestao `Cadastrar`](../src/GeradorProvas.Aplicacao/Servicos/ServicoCadastroQuestao.cs#L14-L28) |
| RF02: consultar questão | [QuestoesController `Obter`](../src/GeradorProvas.Apresentacao/Controllers/QuestoesController.cs#L26-L28) → [ServicoConsultaQuestao `ObterPorId`](../src/GeradorProvas.Aplicacao/Servicos/ServicoConsultaQuestao.cs#L12-L18) |
| RF03: listar questões | [QuestoesController `Listar`](../src/GeradorProvas.Apresentacao/Controllers/QuestoesController.cs#L30-L32) → [ServicoConsultaQuestao `Listar`](../src/GeradorProvas.Aplicacao/Servicos/ServicoConsultaQuestao.cs#L20-L28) → [Repositório `Listar`](../src/GeradorProvas.Infraestrutura/RepositorioQuestoesEmMemoria.cs#L33-L36) |
| RF04: gerar prova | [ProvasController `Gerar`](../src/GeradorProvas.Apresentacao/Controllers/ProvasController.cs#L19-L24) → [ServicoGeradorProva `GerarProva`](../src/GeradorProvas.Aplicacao/Servicos/ServicoGeradorProva.cs#L21-L36) |
| RF05: consultar prova | [ProvasController `Obter`](../src/GeradorProvas.Apresentacao/Controllers/ProvasController.cs#L26-L28) → [ServicoConsultaProva `ObterPorId`](../src/GeradorProvas.Aplicacao/Servicos/ServicoConsultaProva.cs#L12-L18) |
| RN01: questão válida | [Questao, construtor](../src/GeradorProvas.Dominio/Questao.cs#L10-L20) (+ [Alternativa](../src/GeradorProvas.Dominio/Alternativa.cs#L7)) |
| RN02: poucas questões → 422 | [ServicoGeradorProva `ValidarQuestoesSuficientes`](../src/GeradorProvas.Aplicacao/Servicos/ServicoGeradorProva.cs#L48-L55) + [Tratador](../src/GeradorProvas.Apresentacao/TratadorDeExcecoes.cs#L42) |
| RN03: sorteio sem repetir | [ServicoGeradorProva `Sortear`](../src/GeradorProvas.Aplicacao/Servicos/ServicoGeradorProva.cs#L57-L59) + [Embaralhador](../src/GeradorProvas.Infraestrutura/EmbaralhadorAleatorio.cs#L15-L20) + [Prova `ValidarQuestoesSemRepeticao`](../src/GeradorProvas.Dominio/Prova.cs#L32-L38) |
| RN04: ordem e letras A, B, C | [Questao copia a lista](../src/GeradorProvas.Dominio/Questao.cs#L16) + [MapeadorQuestao](../src/GeradorProvas.Aplicacao/Mapeadores/MapeadorQuestao.cs#L21-L29) |
| RN05: ids sequenciais | [RepositorioQuestoes `ObterProximoId`](../src/GeradorProvas.Infraestrutura/RepositorioQuestoesEmMemoria.cs#L15-L19) e [RepositorioProvas `ObterProximoId`](../src/GeradorProvas.Infraestrutura/RepositorioProvasEmMemoria.cs#L15-L19) |
| RN06: inexistente → 404 | [ServicoConsultaQuestao](../src/GeradorProvas.Aplicacao/Servicos/ServicoConsultaQuestao.cs#L14-L15), [ServicoConsultaProva](../src/GeradorProvas.Aplicacao/Servicos/ServicoConsultaProva.cs#L14-L15) + [Tratador](../src/GeradorProvas.Apresentacao/TratadorDeExcecoes.cs#L41) |

### Detalhes que costumam cair

| Pergunta | Onde mostrar |
|---|---|
| Onde sai a letra correta? | [MapeadorQuestao:26-27](../src/GeradorProvas.Aplicacao/Mapeadores/MapeadorQuestao.cs#L26-L27) |
| Onde numera a prova a partir de 1? | [MapeadorProva:12](../src/GeradorProvas.Aplicacao/Mapeadores/MapeadorProva.cs#L12) |
| Onde pega as N primeiras embaralhadas? | [ServicoGeradorProva:59](../src/GeradorProvas.Aplicacao/Servicos/ServicoGeradorProva.cs#L59) (`Take`) |
| Onde valida conteúdos e número de questões? | [ServicoGeradorProva:39-46](../src/GeradorProvas.Aplicacao/Servicos/ServicoGeradorProva.cs#L39-L46) |
| Onde a prova é guardada? | [ServicoGeradorProva:34](../src/GeradorProvas.Aplicacao/Servicos/ServicoGeradorProva.cs#L34) → [RepositorioProvas `Adicionar`](../src/GeradorProvas.Infraestrutura/RepositorioProvasEmMemoria.cs#L21-L25) |
| Onde filtra por disciplina e conteúdos? | [RepositorioQuestoes `Filtrar`](../src/GeradorProvas.Infraestrutura/RepositorioQuestoesEmMemoria.cs#L38-L39) |
| Onde ficam guardados os dados? | listas privadas: [questões](../src/GeradorProvas.Infraestrutura/RepositorioQuestoesEmMemoria.cs#L8), [provas](../src/GeradorProvas.Infraestrutura/RepositorioProvasEmMemoria.cs#L8) |
| Onde estão as rotas? | `[Route]` em [QuestoesController:7](../src/GeradorProvas.Apresentacao/Controllers/QuestoesController.cs#L7) e [ProvasController:7](../src/GeradorProvas.Apresentacao/Controllers/ProvasController.cs#L7) |
| Onde sai o `201 Created`? | `CreatedAtAction` em [QuestoesController:23](../src/GeradorProvas.Apresentacao/Controllers/QuestoesController.cs#L23) e [ProvasController:23](../src/GeradorProvas.Apresentacao/Controllers/ProvasController.cs#L23) |
| Onde as dependências são registradas? | [Program.cs:11-19](../src/GeradorProvas.Apresentacao/Program.cs#L11-L19) |

### Conteúdos da disciplina

| Conceito | Onde mostrar | Em uma frase |
|---|---|---|
| Encapsulamento | [Questao:8 e 30](../src/GeradorProvas.Dominio/Questao.cs#L8-L30), [Prova:5 e 24](../src/GeradorProvas.Dominio/Prova.cs#L5-L24) | Lista privada, exposta só para leitura; propriedades só com `get`. |
| SRP | pasta [`Servicos`](../src/GeradorProvas.Aplicacao/Servicos), [mapeadores](../src/GeradorProvas.Aplicacao/Mapeadores), [TratadorDeExcecoes](../src/GeradorProvas.Apresentacao/TratadorDeExcecoes.cs) | Cada classe tem um único motivo para mudar. |
| OCP | [IEmbaralhador](../src/GeradorProvas.Aplicacao/Contratos/IEmbaralhador.cs) + [Program.cs:15](../src/GeradorProvas.Apresentacao/Program.cs#L15) | Comportamento novo = classe nova, sem alterar o serviço. |
| LSP | [EmbaralhadorAleatorio](../src/GeradorProvas.Infraestrutura/EmbaralhadorAleatorio.cs) e [EmbaralhadorInvertido](../tests/GeradorProvas.Testes/Apoio/EmbaralhadorInvertido.cs) | Qualquer implementação substitui a outra. |
| ISP | [IEmbaralhador](../src/GeradorProvas.Aplicacao/Contratos/IEmbaralhador.cs) (1 método), [IRepositorioProvas](../src/GeradorProvas.Aplicacao/Contratos/IRepositorioProvas.cs) (3) | Interfaces pequenas, uma por necessidade. |
| DIP | [ServicoGeradorProva:7-19](../src/GeradorProvas.Aplicacao/Servicos/ServicoGeradorProva.cs#L7-L19) | O serviço depende de interfaces, não de classes concretas. |
| Injeção de dependência | construtores + [Program.cs:11-19](../src/GeradorProvas.Apresentacao/Program.cs#L11-L19) | As dependências chegam prontas pelo construtor. |
| Repository | [Contratos](../src/GeradorProvas.Aplicacao/Contratos) + [RepositorioQuestoesEmMemoria](../src/GeradorProvas.Infraestrutura/RepositorioQuestoesEmMemoria.cs) | Esconde onde e como os dados ficam guardados. |
| Service Layer | pasta [`Servicos`](../src/GeradorProvas.Aplicacao/Servicos) | Os casos de uso ficam fora do controller. |
| Strategy | [IEmbaralhador](../src/GeradorProvas.Aplicacao/Contratos/IEmbaralhador.cs) | Algoritmos de embaralhar intercambiáveis. |
| DTO | pasta [`Dtos`](../src/GeradorProvas.Aplicacao/Dtos) | Entrada e saída da API sem expor o domínio. |
| Camadas | os quatro `.csproj` ([seção 1](#1-mapa-da-solução)) | Dependências apontando para o domínio. |

---

## 5. O caminho do `POST /provas` (para falar na apresentação)

1. A requisição chega em [ProvasController.Gerar](../src/GeradorProvas.Apresentacao/Controllers/ProvasController.cs#L19-L24). O JSON já vira um `GeracaoProvaDto`.
2. O controller chama [ServicoGeradorProva.GerarProva](../src/GeradorProvas.Aplicacao/Servicos/ServicoGeradorProva.cs#L21-L36).
3. O serviço valida o pedido ([39-46](../src/GeradorProvas.Aplicacao/Servicos/ServicoGeradorProva.cs#L39-L46)).
4. Pede ao repositório as questões do filtro ([RepositorioQuestoesEmMemoria.Filtrar](../src/GeradorProvas.Infraestrutura/RepositorioQuestoesEmMemoria.cs#L38-L39)).
5. Confere a RN02 ([48-55](../src/GeradorProvas.Aplicacao/Servicos/ServicoGeradorProva.cs#L48-L55)). Se faltar questão, a `RegraNegocioException` vai até o [TratadorDeExcecoes](../src/GeradorProvas.Apresentacao/TratadorDeExcecoes.cs#L42), que responde 422.
6. Sorteia: [EmbaralhadorAleatorio.Embaralhar](../src/GeradorProvas.Infraestrutura/EmbaralhadorAleatorio.cs#L15-L20) e depois `Take` ([59](../src/GeradorProvas.Aplicacao/Servicos/ServicoGeradorProva.cs#L59)).
7. Cria a [Prova](../src/GeradorProvas.Dominio/Prova.cs#L7-L16) com o [próximo id](../src/GeradorProvas.Infraestrutura/RepositorioProvasEmMemoria.cs#L15-L19). Ela confere nome, disciplina e repetição.
8. Guarda no [RepositorioProvasEmMemoria](../src/GeradorProvas.Infraestrutura/RepositorioProvasEmMemoria.cs#L21-L25).
9. Converte com o [MapeadorProva](../src/GeradorProvas.Aplicacao/Mapeadores/MapeadorProva.cs#L7-L13) (numera a partir de 1 e põe as letras).
10. O controller responde `201 Created` com `Location: /provas/{id}` ([ProvasController:23](../src/GeradorProvas.Apresentacao/Controllers/ProvasController.cs#L23)).

---

## 6. Perguntas de "por quê"

**Por que os repositórios são Singleton e os serviços Scoped?**
O repositório guarda os dados numa lista. Se fosse criado de novo a cada requisição, os dados
sumiriam. O serviço não guarda nada, então pode nascer e morrer a cada requisição
([Program.cs:11-19](../src/GeradorProvas.Apresentacao/Program.cs#L11-L19)).

**Por que o `Random` é injetado em vez de `new Random()` dentro do embaralhador?**
Porque é uma dependência (DIP). Nos testes, isso permite passar um `Random` com semente fixa e
ter um resultado previsível ([EmbaralhadorAleatorioTestes](../tests/GeradorProvas.Testes/Infraestrutura/EmbaralhadorAleatorioTestes.cs)).

**Por que o Domínio não referencia nenhum projeto?**
Ele é o centro: as regras não podem depender de banco, de HTTP ou de biblioteca. Se a API
virar um app de console, o Domínio continua igual.

**Por que existe DTO se já existe a entidade?**
Para a API não expor o domínio. Por exemplo, a prova (`QuestaoProvaDto`) não mostra qual
alternativa é a correta, e a entrada (`CadastroQuestaoDto`) não tem id, porque quem gera o id é o repositório.

**Por que `IReadOnlyList` e `AsReadOnly()`?**
Para ninguém de fora conseguir fazer `questao.Alternativas.Add(...)` e quebrar a regra das
2 a 5 alternativas depois que a questão foi validada.

**Por que os mapeadores são `static`?**
Eles não guardam estado: só convertem um objeto em outro. O próprio diagrama do enunciado marca
o `MapeadorProva` como «static».

**O que acontece se o id não existe?**
O serviço lança `RecursoNaoEncontradoException`, o `TratadorDeExcecoes` pega e responde 404.
O controller nem fica sabendo.

**Qual a diferença entre o 400 e o 422?**
400 é dado inválido, como campo vazio ou duas corretas (`ArgumentException`). 422 é um pedido
válido que a regra de negócio não deixa atender, como pedir 10 questões quando só existem 3
(`RegraNegocioException`).

**Se um cadastro falha, o id é perdido?**
É. O enunciado manda pegar o próximo id antes de criar a questão, então um cadastro recusado
deixa aquele número sem uso. A trava garante que dois cadastros nunca recebam o mesmo id.

---

## 7. Onde estão os testes de cada parte

`dotnet test` roda os 57 testes. A pasta de cada camada está em [`tests/GeradorProvas.Testes`](../tests/GeradorProvas.Testes):

| Se perguntarem do teste de... | Arquivo |
|---|---|
| RN01 | [QuestaoTestes](../tests/GeradorProvas.Testes/Dominio/QuestaoTestes.cs), [AlternativaTestes](../tests/GeradorProvas.Testes/Dominio/AlternativaTestes.cs) |
| RN03 e a prova | [ProvaTestes](../tests/GeradorProvas.Testes/Dominio/ProvaTestes.cs) |
| Ids, `lock` e filtros | [RepositorioQuestoesEmMemoriaTestes](../tests/GeradorProvas.Testes/Infraestrutura/RepositorioQuestoesEmMemoriaTestes.cs) |
| Embaralhador | [EmbaralhadorAleatorioTestes](../tests/GeradorProvas.Testes/Infraestrutura/EmbaralhadorAleatorioTestes.cs) |
| Gerar prova, RN02 | [ServicoGeradorProvaTestes](../tests/GeradorProvas.Testes/Aplicacao/ServicoGeradorProvaTestes.cs) |
| Letras (RN04) | [ServicoConsultaQuestaoTestes](../tests/GeradorProvas.Testes/Aplicacao/ServicoConsultaQuestaoTestes.cs) |
| RF05 | [ServicoConsultaProvaTestes](../tests/GeradorProvas.Testes/Aplicacao/ServicoConsultaProvaTestes.cs), [ProvasApiTestes](../tests/GeradorProvas.Testes/Api/ProvasApiTestes.cs) |
| Contrato HTTP | [QuestoesApiTestes](../tests/GeradorProvas.Testes/Api/QuestoesApiTestes.cs), [ProvasApiTestes](../tests/GeradorProvas.Testes/Api/ProvasApiTestes.cs) |
