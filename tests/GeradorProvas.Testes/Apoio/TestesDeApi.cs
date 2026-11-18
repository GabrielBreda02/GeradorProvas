using System.Net.Http.Json;
using System.Text.Json;
using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Aplicacao;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Testes;

/// <summary>
/// Sobe a API inteira em memória para cada teste. Como o xUnit cria uma instância da classe
/// por teste, cada um começa com os repositórios vazios e os ids em 1.
/// </summary>
public abstract class TestesDeApi : IDisposable
{
    private readonly WebApplicationFactory<Program> _fabrica = new();

    protected TestesDeApi()
    {
        Cliente = _fabrica.CreateClient();
    }

    protected HttpClient Cliente { get; }

    protected Task<HttpResponseMessage> CadastrarQuestao(CadastroQuestaoDto dados) =>
        Cliente.PostAsJsonAsync("/questoes", dados);

    protected static Task<JsonElement> LerJson(HttpResponseMessage resposta) =>
        resposta.Content.ReadFromJsonAsync<JsonElement>();

    public void Dispose()
    {
        Cliente.Dispose();
        _fabrica.Dispose();
        GC.SuppressFinalize(this);
    }
}
