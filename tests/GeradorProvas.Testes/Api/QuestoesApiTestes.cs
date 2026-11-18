using System.Net;
using System.Net.Http.Json;
using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Aplicacao;

namespace Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Testes;

public class QuestoesApiTestes : TestesDeApi
{
    [Fact]
    public async Task PostQuestoes_DadosValidos_Responde201ComIdELocation()
    {
        var resposta = await CadastrarQuestao(Exemplos.CadastroQuestao());

        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        Assert.Equal("/questoes/1", resposta.Headers.Location?.AbsolutePath);
        Assert.Equal(1, (await LerJson(resposta)).GetProperty("id").GetInt32());
    }

    [Fact]
    public async Task PostQuestoes_ComDuasAlternativasCorretas_Responde400()
    {
        var dados = Exemplos.CadastroQuestao() with
        {
            Alternativas = [new AlternativaCadastroDto("A", Correta: true), new AlternativaCadastroDto("B", Correta: true)],
        };

        var resposta = await CadastrarQuestao(dados);

        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task GetQuestao_Existente_Responde200ComLetrasELetraCorreta()
    {
        await CadastrarQuestao(Exemplos.CadastroQuestao());

        var resposta = await Cliente.GetAsync("/questoes/1");

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        var questao = await LerJson(resposta);
        Assert.Equal("B", questao.GetProperty("alternativas")[1].GetProperty("letra").GetString());
        Assert.Equal("A", questao.GetProperty("letraCorreta").GetString());
    }

    [Fact]
    public async Task GetQuestao_Inexistente_Responde404()
    {
        var resposta = await Cliente.GetAsync("/questoes/99");

        Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
    }

    [Fact]
    public async Task GetQuestoes_ComDisciplinaEConteudo_DevolveSoAsQuestoesDoFiltro()
    {
        await CadastrarQuestao(Exemplos.CadastroQuestao(conteudo: "SOLID", enunciado: "Sobre SOLID"));
        await CadastrarQuestao(Exemplos.CadastroQuestao(conteudo: "Encapsulamento", enunciado: "Sobre encapsulamento"));

        var questoes = await Cliente.GetFromJsonAsync<List<QuestaoDto>>("/questoes?disciplina=POO%20II&conteudo=SOLID");

        Assert.Equal(new[] { "Sobre SOLID" }, questoes!.Select(questao => questao.Enunciado));
    }

    [Fact]
    public async Task GetQuestoes_DisciplinaSemQuestoes_Responde200ComListaVazia()
    {
        var resposta = await Cliente.GetAsync("/questoes?disciplina=Hist%C3%B3ria");

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        var questoes = await resposta.Content.ReadFromJsonAsync<List<QuestaoDto>>();
        Assert.NotNull(questoes);
        Assert.Empty(questoes);
    }

    [Fact]
    public async Task GetQuestoes_SemDisciplina_Responde400()
    {
        var resposta = await Cliente.GetAsync("/questoes");

        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }
}
