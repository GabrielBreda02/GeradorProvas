using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Aplicacao;
using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Dominio;
using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Infraestrutura;

namespace Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Testes;

public class ServicoConsultaQuestaoTestes
{
    private readonly RepositorioQuestoesEmMemoria _repositorio = new();
    private readonly ServicoConsultaQuestao _servico;

    public ServicoConsultaQuestaoTestes()
    {
        _servico = new ServicoConsultaQuestao(_repositorio);
    }

    [Fact]
    public void ObterPorId_DaAsLetrasNaOrdemDoCadastro()
    {
        AdicionarQuestaoComATerceiraCorreta();

        var questao = _servico.ObterPorId(1);

        Assert.Equal(new[] { 'A', 'B', 'C' }, questao.Alternativas.Select(alternativa => alternativa.Letra));
        Assert.Equal(new[] { "Primeira", "Segunda", "Terceira" }, questao.Alternativas.Select(alternativa => alternativa.Texto));
    }

    [Fact]
    public void ObterPorId_InformaALetraDaAlternativaCorreta()
    {
        AdicionarQuestaoComATerceiraCorreta();

        var questao = _servico.ObterPorId(1);

        Assert.Equal('C', questao.LetraCorreta);
    }

    [Fact]
    public void ObterPorId_Inexistente_LancaRecursoNaoEncontradoException()
    {
        Assert.Throws<RecursoNaoEncontradoException>(() => _servico.ObterPorId(99));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Listar_SemDisciplina_LancaArgumentException(string disciplina)
    {
        Assert.Throws<ArgumentException>(() => _servico.Listar(disciplina, conteudo: null));
    }

    private void AdicionarQuestaoComATerceiraCorreta()
    {
        _repositorio.Adicionar(new Questao(1, "POO II", "SOLID", "Enunciado",
        [
            new Alternativa("Primeira", correta: false),
            new Alternativa("Segunda", correta: false),
            new Alternativa("Terceira", correta: true),
        ]));
    }
}
