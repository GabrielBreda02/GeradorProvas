using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Dominio;

namespace Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Testes;

public class ProvaTestes
{
    [Theory]
    [InlineData("", "POO II")]
    [InlineData("Avaliação A1", "   ")]
    public void Construtor_ComNomeOuDisciplinaEmBranco_LancaArgumentException(string nome, string disciplina)
    {
        Assert.Throws<ArgumentException>(() => new Prova(1, nome, disciplina, [Exemplos.Questao(1)]));
    }

    [Fact]
    public void Construtor_SemQuestoes_LancaArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Prova(1, "Avaliação A1", "POO II", []));
    }

    [Fact]
    public void Construtor_ComQuestaoRepetida_LancaRegraNegocioException()
    {
        var repetida = Exemplos.Questao(1);

        Assert.Throws<RegraNegocioException>(
            () => new Prova(1, "Avaliação A1", "POO II", [repetida, Exemplos.Questao(2), repetida]));
    }

    [Fact]
    public void Questoes_MantemAOrdemRecebida()
    {
        var prova = new Prova(1, "Avaliação A1", "POO II",
            [Exemplos.Questao(3), Exemplos.Questao(1), Exemplos.Questao(2)]);

        Assert.Equal(new[] { 3, 1, 2 }, prova.Questoes.Select(questao => questao.Id));
    }
}
