using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Aplicacao;
using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Dominio;

namespace Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Infraestrutura;

public class RepositorioQuestoesEmMemoria : IRepositorioQuestoes
{
    private readonly List<Questao> _questoes = [];

    // O repositório é Singleton e atende requisições simultâneas: a trava protege a lista e o contador.
    private readonly Lock _trava = new();

    private int _ultimoId;

    public int ObterProximoId()
    {
        lock (_trava)
            return ++_ultimoId;
    }

    public void Adicionar(Questao questao)
    {
        lock (_trava)
            _questoes.Add(questao);
    }

    public Questao? ObterPorId(int id)
    {
        lock (_trava)
            return _questoes.Find(questao => questao.Id == id);
    }

    public IReadOnlyList<Questao> Listar(string disciplina, string? conteudo) =>
        string.IsNullOrWhiteSpace(conteudo)
            ? Buscar(questao => PertenceADisciplina(questao, disciplina))
            : Filtrar(disciplina, [conteudo]);

    public IReadOnlyList<Questao> Filtrar(string disciplina, IReadOnlyList<string> conteudos) =>
        Buscar(questao => PertenceADisciplina(questao, disciplina) && TemUmDosConteudos(questao, conteudos));

    private List<Questao> Buscar(Func<Questao, bool> criterio)
    {
        lock (_trava)
            return _questoes.Where(criterio).ToList();
    }

    private static bool PertenceADisciplina(Questao questao, string disciplina) =>
        MesmoTexto(questao.Disciplina, disciplina);

    private static bool TemUmDosConteudos(Questao questao, IReadOnlyList<string> conteudos) =>
        conteudos.Any(conteudo => MesmoTexto(questao.Conteudo, conteudo));

    // Filtros ignoram maiúsculas e minúsculas: "poo ii" encontra as questões de "POO II".
    private static bool MesmoTexto(string texto, string outroTexto) =>
        string.Equals(texto, outroTexto, StringComparison.OrdinalIgnoreCase);
}
