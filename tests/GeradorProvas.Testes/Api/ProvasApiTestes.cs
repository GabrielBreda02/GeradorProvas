using System.Net;
using System.Net.Http.Json;
using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Aplicacao;

namespace Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Testes;

public class ProvasApiTestes : TestesDeApi
{
    [Fact]
    public async Task PostProvas_Responde201ComAProvaNumeradaELocation()
    {
        await CadastrarQuestoes(quantidade: 3);

        var resposta = await GerarProva(numeroQuestoes: 2);

        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        Assert.Equal("/provas/1", resposta.Headers.Location?.AbsolutePath);
        var prova = await resposta.Content.ReadFromJsonAsync<ProvaDto>();
        Assert.Equal(new[] { 1, 2 }, prova!.Questoes.Select(questao => questao.Numero));
    }

    [Fact]
    public async Task GetProva_DevolveExatamenteAProvaGerada()
    {
        await CadastrarQuestoes(quantidade: 5);
        var gerada = await (await GerarProva(numeroQuestoes: 5)).Content.ReadAsStringAsync();

        var consultada = await Cliente.GetStringAsync("/provas/1");

        Assert.Equal(gerada, consultada);
    }

    [Fact]
    public async Task PostProvas_SemQuestoesSuficientes_Responde422()
    {
        await CadastrarQuestoes(quantidade: 2);

        var resposta = await GerarProva(numeroQuestoes: 3);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, resposta.StatusCode);
    }

    [Fact]
    public async Task PostProvas_NumeroDeQuestoesZero_Responde400()
    {
        await CadastrarQuestoes(quantidade: 1);

        var resposta = await GerarProva(numeroQuestoes: 0);

        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task GetProva_Inexistente_Responde404()
    {
        var resposta = await Cliente.GetAsync("/provas/99");

        Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
    }

    private async Task CadastrarQuestoes(int quantidade)
    {
        for (var numero = 1; numero <= quantidade; numero++)
            await CadastrarQuestao(Exemplos.CadastroQuestao(enunciado: $"Questão {numero}"));
    }

    private Task<HttpResponseMessage> GerarProva(int numeroQuestoes) =>
        Cliente.PostAsJsonAsync("/provas", new GeracaoProvaDto("Avaliação A1", "POO II", ["SOLID"], numeroQuestoes));
}
