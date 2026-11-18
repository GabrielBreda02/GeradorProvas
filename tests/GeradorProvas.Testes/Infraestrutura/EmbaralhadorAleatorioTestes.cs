using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Dominio;
using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Infraestrutura;

namespace Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Testes;

public class EmbaralhadorAleatorioTestes
{
    // Semente fixa: o "aleatório" passa a ser reproduzível, então o teste nunca oscila.
    private readonly EmbaralhadorAleatorio _embaralhador = new(new Random(42));

    [Fact]
    public void Embaralhar_DevolveTodasAsQuestoesSemRepetir()
    {
        var embaralhadas = _embaralhador.Embaralhar(DezQuestoes());

        Assert.Equal(Enumerable.Range(1, 10), embaralhadas.Select(questao => questao.Id).Order());
    }

    [Fact]
    public void Embaralhar_MudaAOrdemDasQuestoes()
    {
        var embaralhadas = _embaralhador.Embaralhar(DezQuestoes());

        Assert.NotEqual(Enumerable.Range(1, 10), embaralhadas.Select(questao => questao.Id));
    }

    private static List<Questao> DezQuestoes() =>
        Enumerable.Range(1, 10).Select(id => Exemplos.Questao(id)).ToList();
}
