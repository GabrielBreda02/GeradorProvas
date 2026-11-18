namespace Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Dominio;

public class Prova
{
    private readonly List<Questao> _questoes;

    public Prova(int id, string nome, string disciplina, IEnumerable<Questao> questoes)
    {
        Id = id;
        Nome = Validacao.TextoObrigatorio(nome, nameof(nome));
        Disciplina = Validacao.TextoObrigatorio(disciplina, nameof(disciplina));
        _questoes = [.. questoes];

        ValidarPeloMenosUmaQuestao();
        ValidarQuestoesSemRepeticao();
    }

    public int Id { get; }

    public string Nome { get; }

    public string Disciplina { get; }

    public IReadOnlyList<Questao> Questoes => _questoes.AsReadOnly();

    private void ValidarPeloMenosUmaQuestao()
    {
        if (_questoes.Count == 0)
            throw new ArgumentException("A prova deve ter pelo menos uma questão.");
    }

    private void ValidarQuestoesSemRepeticao()
    {
        var quantidadeDeQuestoesDistintas = _questoes.DistinctBy(questao => questao.Id).Count();

        if (quantidadeDeQuestoesDistintas != _questoes.Count)
            throw new RegraNegocioException("A prova não pode ter questões repetidas.");
    }
}
