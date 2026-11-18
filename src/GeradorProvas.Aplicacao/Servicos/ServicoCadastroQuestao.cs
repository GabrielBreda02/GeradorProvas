using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Dominio;

namespace Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Aplicacao;

public class ServicoCadastroQuestao
{
    private readonly IRepositorioQuestoes _repositorio;

    public ServicoCadastroQuestao(IRepositorioQuestoes repositorio)
    {
        _repositorio = repositorio;
    }

    public int Cadastrar(CadastroQuestaoDto dados)
    {
        var alternativas = dados.Alternativas
            .Select(alternativa => new Alternativa(alternativa.Texto, alternativa.Correta));

        var questao = new Questao(
            _repositorio.ObterProximoId(),
            dados.Disciplina,
            dados.Conteudo,
            dados.Enunciado,
            alternativas);

        _repositorio.Adicionar(questao);
        return questao.Id;
    }
}
