using System.Text.Json;
using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Aplicacao;
using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Infraestrutura;

namespace Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Testes;

public class ServicoConsultaProvaTestes
{
    private readonly RepositorioProvasEmMemoria _repositorioProvas = new();
    private readonly ServicoConsultaProva _servico;

    public ServicoConsultaProvaTestes()
    {
        _servico = new ServicoConsultaProva(_repositorioProvas);
    }

    [Fact]
    public void ObterPorId_DevolveAProvaExatamenteComoFoiGerada()
    {
        var gerador = CriarGeradorComCincoQuestoes();
        gerador.GerarProva(new GeracaoProvaDto("Prova 1", "POO II", ["SOLID"], 5));
        var gerada = gerador.GerarProva(new GeracaoProvaDto("Prova 2", "POO II", ["SOLID"], 5));

        var consultada = _servico.ObterPorId(gerada.Id);

        // Comparar o JSON confere todos os campos e a ordem das questões e alternativas.
        Assert.Equal(JsonSerializer.Serialize(gerada), JsonSerializer.Serialize(consultada));
    }

    [Fact]
    public void ObterPorId_Inexistente_LancaRecursoNaoEncontradoException()
    {
        Assert.Throws<RecursoNaoEncontradoException>(() => _servico.ObterPorId(99));
    }

    private ServicoGeradorProva CriarGeradorComCincoQuestoes()
    {
        var repositorioQuestoes = new RepositorioQuestoesEmMemoria();
        for (var id = 1; id <= 5; id++)
            repositorioQuestoes.Adicionar(Exemplos.Questao(id));

        return new ServicoGeradorProva(
            repositorioQuestoes, _repositorioProvas, new EmbaralhadorAleatorio(new Random(7)));
    }
}
