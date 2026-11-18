using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Dominio;

namespace Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Testes;

public class QuestaoTestes
{
    [Theory]
    [InlineData("", "SOLID", "Enunciado")]
    [InlineData("POO II", "   ", "Enunciado")]
    [InlineData("POO II", "SOLID", "")]
    public void Construtor_ComCampoObrigatorioEmBranco_LancaArgumentException(
        string disciplina, string conteudo, string enunciado)
    {
        Assert.Throws<ArgumentException>(
            () => new Questao(1, disciplina, conteudo, enunciado, Exemplos.Alternativas()));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(6)]
    public void Construtor_ComQuantidadeDeAlternativasForaDe2A5_LancaArgumentException(int quantidade)
    {
        Assert.Throws<ArgumentException>(() => CriarQuestao(Exemplos.Alternativas(quantidade)));
    }

    [Theory]
    [InlineData(2)]
    [InlineData(5)]
    public void Construtor_ComQuantidadeDeAlternativasNosLimites_CriaQuestao(int quantidade)
    {
        var questao = CriarQuestao(Exemplos.Alternativas(quantidade));

        Assert.Equal(quantidade, questao.Alternativas.Count);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    public void Construtor_SemExatamenteUmaAlternativaCorreta_LancaArgumentException(int quantidadeCorretas)
    {
        Assert.Throws<ArgumentException>(
            () => CriarQuestao(Exemplos.Alternativas(quantidade: 4, quantidadeCorretas)));
    }

    [Fact]
    public void Alternativas_MudarAListaUsadaNaCriacao_NaoAfetaAQuestao()
    {
        var alternativas = Exemplos.Alternativas(quantidade: 3);
        var questao = CriarQuestao(alternativas);

        alternativas.Add(new Alternativa("Intrusa", correta: false));

        Assert.Equal(3, questao.Alternativas.Count);
    }

    [Fact]
    public void Alternativas_TentarAdicionarPorFora_LancaNotSupportedException()
    {
        var questao = CriarQuestao(Exemplos.Alternativas());
        var comoColecao = (ICollection<Alternativa>)questao.Alternativas;

        Assert.Throws<NotSupportedException>(() => comoColecao.Add(new Alternativa("Intrusa", correta: false)));
    }

    private static Questao CriarQuestao(IEnumerable<Alternativa> alternativas) =>
        new(1, "POO II", "SOLID", "Enunciado", alternativas);
}
