namespace Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Dominio;

public class Questao
{
    private const int MinimoAlternativas = 2;
    private const int MaximoAlternativas = 5;

    private readonly List<Alternativa> _alternativas;

    public Questao(int id, string disciplina, string conteudo, string enunciado, IEnumerable<Alternativa> alternativas)
    {
        Id = id;
        Disciplina = Validacao.TextoObrigatorio(disciplina, nameof(disciplina));
        Conteudo = Validacao.TextoObrigatorio(conteudo, nameof(conteudo));
        Enunciado = Validacao.TextoObrigatorio(enunciado, nameof(enunciado));
        _alternativas = [.. alternativas];

        ValidarQuantidadeDeAlternativas();
        ValidarUnicaAlternativaCorreta();
    }

    public int Id { get; }

    public string Disciplina { get; }

    public string Conteudo { get; }

    public string Enunciado { get; }

    public IReadOnlyList<Alternativa> Alternativas => _alternativas.AsReadOnly();

    private void ValidarQuantidadeDeAlternativas()
    {
        if (_alternativas.Count is < MinimoAlternativas or > MaximoAlternativas)
            throw new ArgumentException(
                $"A questão deve ter de {MinimoAlternativas} a {MaximoAlternativas} alternativas.");
    }

    private void ValidarUnicaAlternativaCorreta()
    {
        if (_alternativas.Count(alternativa => alternativa.Correta) != 1)
            throw new ArgumentException("A questão deve ter exatamente uma alternativa correta.");
    }
}
