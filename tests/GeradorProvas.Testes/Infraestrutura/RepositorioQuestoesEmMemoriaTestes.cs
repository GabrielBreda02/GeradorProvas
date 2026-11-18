using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Dominio;
using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Infraestrutura;

namespace Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Testes;

public class RepositorioQuestoesEmMemoriaTestes
{
    private readonly RepositorioQuestoesEmMemoria _repositorio = new();

    [Fact]
    public void ObterProximoId_ChamadasSeguidas_DevolveIdsSequenciaisAPartirDe1()
    {
        var ids = new[] { _repositorio.ObterProximoId(), _repositorio.ObterProximoId(), _repositorio.ObterProximoId() };

        Assert.Equal(new[] { 1, 2, 3 }, ids);
    }

    [Fact]
    public void ObterProximoId_ChamadasSimultaneas_NuncaRepeteId()
    {
        const int Chamadas = 10_000;
        var ids = new int[Chamadas];

        Parallel.For(0, Chamadas, indice => ids[indice] = _repositorio.ObterProximoId());

        Assert.Equal(Chamadas, ids.Distinct().Count());
    }

    [Fact]
    public void ObterPorId_Inexistente_DevolveNull()
    {
        _repositorio.Adicionar(Exemplos.Questao(1));

        Assert.Null(_repositorio.ObterPorId(2));
    }

    [Fact]
    public void Filtrar_DevolveSoAsQuestoesDaDisciplinaComUmDosConteudos()
    {
        AdicionarQuestoesVariadas();

        var questoes = _repositorio.Filtrar("POO II", ["SOLID", "Design Patterns"]);

        Assert.Equal(new[] { 1, 3 }, Ids(questoes));
    }

    [Fact]
    public void Filtrar_IgnoraMaiusculasEMinusculas()
    {
        AdicionarQuestoesVariadas();

        var questoes = _repositorio.Filtrar("poo ii", ["solid"]);

        Assert.Equal(new[] { 1 }, Ids(questoes));
    }

    [Fact]
    public void Listar_SemConteudo_DevolveTodasAsQuestoesDaDisciplina()
    {
        AdicionarQuestoesVariadas();

        var questoes = _repositorio.Listar("POO II", conteudo: null);

        Assert.Equal(new[] { 1, 2, 3 }, Ids(questoes));
    }

    [Fact]
    public void Listar_ComConteudo_DevolveSoAsQuestoesDesseConteudo()
    {
        AdicionarQuestoesVariadas();

        var questoes = _repositorio.Listar("POO II", "Encapsulamento");

        Assert.Equal(new[] { 2 }, Ids(questoes));
    }

    private void AdicionarQuestoesVariadas()
    {
        _repositorio.Adicionar(Exemplos.Questao(1, "POO II", "SOLID"));
        _repositorio.Adicionar(Exemplos.Questao(2, "POO II", "Encapsulamento"));
        _repositorio.Adicionar(Exemplos.Questao(3, "POO II", "Design Patterns"));
        _repositorio.Adicionar(Exemplos.Questao(4, "Matemática", "SOLID"));
    }

    private static int[] Ids(IEnumerable<Questao> questoes) =>
        questoes.Select(questao => questao.Id).ToArray();
}
