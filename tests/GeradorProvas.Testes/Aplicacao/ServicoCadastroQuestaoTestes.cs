using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Aplicacao;
using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Infraestrutura;

namespace Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Testes;

public class ServicoCadastroQuestaoTestes
{
    private readonly RepositorioQuestoesEmMemoria _repositorio = new();
    private readonly ServicoCadastroQuestao _servico;

    public ServicoCadastroQuestaoTestes()
    {
        _servico = new ServicoCadastroQuestao(_repositorio);
    }

    [Fact]
    public void Cadastrar_DadosValidos_GuardaAQuestaoComAsAlternativasNaOrdem()
    {
        var dados = new CadastroQuestaoDto("POO II", "SOLID", "Qual princípio fala de um único motivo para mudar?",
        [
            new AlternativaCadastroDto("Inversão de Dependência", Correta: false),
            new AlternativaCadastroDto("Responsabilidade Única", Correta: true),
            new AlternativaCadastroDto("Aberto-Fechado", Correta: false),
        ]);

        var id = _servico.Cadastrar(dados);

        var guardada = _repositorio.ObterPorId(id);
        Assert.NotNull(guardada);
        Assert.Equal(
            new[] { "Inversão de Dependência", "Responsabilidade Única", "Aberto-Fechado" },
            guardada.Alternativas.Select(alternativa => alternativa.Texto));
        Assert.Equal(
            new[] { false, true, false },
            guardada.Alternativas.Select(alternativa => alternativa.Correta));
    }

    [Fact]
    public void Cadastrar_DuasQuestoes_DevolveIdsSequenciais()
    {
        var primeiroId = _servico.Cadastrar(Exemplos.CadastroQuestao());
        var segundoId = _servico.Cadastrar(Exemplos.CadastroQuestao());

        Assert.Equal((1, 2), (primeiroId, segundoId));
    }

    [Fact]
    public void Cadastrar_DadosInvalidos_LancaArgumentExceptionSemGuardarNada()
    {
        var invalida = Exemplos.CadastroQuestao(enunciado: "");

        Assert.Throws<ArgumentException>(() => _servico.Cadastrar(invalida));
        Assert.Empty(_repositorio.Listar("POO II", conteudo: null));
    }
}
