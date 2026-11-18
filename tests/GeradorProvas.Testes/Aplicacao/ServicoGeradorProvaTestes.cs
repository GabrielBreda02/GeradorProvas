using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Aplicacao;
using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Dominio;
using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Infraestrutura;

namespace Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Testes;

public class ServicoGeradorProvaTestes
{
    private readonly RepositorioQuestoesEmMemoria _repositorioQuestoes = new();
    private readonly RepositorioProvasEmMemoria _repositorioProvas = new();
    private readonly ServicoGeradorProva _servico;

    public ServicoGeradorProvaTestes()
    {
        _servico = new ServicoGeradorProva(_repositorioQuestoes, _repositorioProvas, new EmbaralhadorInvertido());
    }

    [Fact]
    public void GerarProva_PegaAsPrimeirasQuestoesDoEmbaralhamento()
    {
        AdicionarQuestoes(Exemplos.Questao(1), Exemplos.Questao(2), Exemplos.Questao(3), Exemplos.Questao(4));

        var prova = _servico.GerarProva(Pedido(numeroQuestoes: 3));

        // O embaralhador de teste inverte a lista (4, 3, 2, 1); a prova fica com as três primeiras.
        Assert.Equal(new[] { "Enunciado 4", "Enunciado 3", "Enunciado 2" }, Enunciados(prova));
    }

    [Fact]
    public void GerarProva_NumeraAsQuestoesAPartirDe1()
    {
        AdicionarQuestoes(Exemplos.Questao(1), Exemplos.Questao(2), Exemplos.Questao(3));

        var prova = _servico.GerarProva(Pedido(numeroQuestoes: 3));

        Assert.Equal(new[] { 1, 2, 3 }, prova.Questoes.Select(questao => questao.Numero));
    }

    [Fact]
    public void GerarProva_UsaSoQuestoesDaDisciplinaEDosConteudosPedidos()
    {
        AdicionarQuestoes(
            Exemplos.Questao(1, "POO II", "SOLID"),
            Exemplos.Questao(2, "POO II", "Encapsulamento"),
            Exemplos.Questao(3, "Matemática", "SOLID"),
            Exemplos.Questao(4, "POO II", "Design Patterns"));

        var prova = _servico.GerarProva(new GeracaoProvaDto("Avaliação A1", "POO II", ["SOLID", "Design Patterns"], 2));

        Assert.Equal(new[] { "Enunciado 4", "Enunciado 1" }, Enunciados(prova));
    }

    [Fact]
    public void GerarProva_ComMenosQuestoesQueOPedido_LancaRegraNegocioInformandoQuantasExistem()
    {
        AdicionarQuestoes(Exemplos.Questao(1), Exemplos.Questao(2));

        var excecao = Assert.Throws<RegraNegocioException>(() => _servico.GerarProva(Pedido(numeroQuestoes: 3)));

        Assert.Contains("Disponíveis: 2", excecao.Message);
    }

    [Fact]
    public void GerarProva_ComMenosQuestoesQueOPedido_NaoGuardaProva()
    {
        AdicionarQuestoes(Exemplos.Questao(1));

        Assert.Throws<RegraNegocioException>(() => _servico.GerarProva(Pedido(numeroQuestoes: 2)));

        Assert.Null(_repositorioProvas.ObterPorId(1));
    }

    [Fact]
    public void GerarProva_SemConteudos_LancaArgumentException()
    {
        AdicionarQuestoes(Exemplos.Questao(1));

        Assert.Throws<ArgumentException>(
            () => _servico.GerarProva(new GeracaoProvaDto("Avaliação A1", "POO II", [], 1)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void GerarProva_ComNumeroDeQuestoesMenorQue1_RecusaSemGastarIdDeProva(int numeroQuestoes)
    {
        AdicionarQuestoes(Exemplos.Questao(1), Exemplos.Questao(2));

        Assert.Throws<ArgumentException>(() => _servico.GerarProva(Pedido(numeroQuestoes)));

        Assert.Equal(1, _servico.GerarProva(Pedido(numeroQuestoes: 1)).Id);
    }

    private void AdicionarQuestoes(params Questao[] questoes)
    {
        foreach (var questao in questoes)
            _repositorioQuestoes.Adicionar(questao);
    }

    private static GeracaoProvaDto Pedido(int numeroQuestoes) =>
        new("Avaliação A1", "POO II", ["SOLID"], numeroQuestoes);

    private static IEnumerable<string> Enunciados(ProvaDto prova) =>
        prova.Questoes.Select(questao => questao.Enunciado);
}
