using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Dominio;

namespace Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Aplicacao;

public interface IRepositorioQuestoes
{
    int ObterProximoId();

    void Adicionar(Questao questao);

    Questao? ObterPorId(int id);

    /// <summary>Questões da disciplina; quando <paramref name="conteudo"/> é informado, só as desse conteúdo.</summary>
    IReadOnlyList<Questao> Listar(string disciplina, string? conteudo);

    /// <summary>Questões da disciplina cujo conteúdo está em <paramref name="conteudos"/>.</summary>
    IReadOnlyList<Questao> Filtrar(string disciplina, IReadOnlyList<string> conteudos);
}
