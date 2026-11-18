using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Aplicacao;
using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Dominio;

namespace Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Testes;

/// <summary>Objetos válidos prontos para uso: cada teste muda só o dado que importa para ele.</summary>
internal static class Exemplos
{
    /// <summary>Alternativas "Alternativa 1", "Alternativa 2"...; as primeiras <paramref name="quantidadeCorretas"/> são corretas.</summary>
    public static List<Alternativa> Alternativas(int quantidade = 3, int quantidadeCorretas = 1) =>
        Enumerable.Range(1, quantidade)
            .Select(numero => new Alternativa($"Alternativa {numero}", correta: numero <= quantidadeCorretas))
            .ToList();

    /// <summary>Questão válida com enunciado "Enunciado {id}".</summary>
    public static Questao Questao(int id, string disciplina = "POO II", string conteudo = "SOLID") =>
        new(id, disciplina, conteudo, $"Enunciado {id}", Alternativas());

    /// <summary>Pedido de cadastro válido, com a alternativa A correta.</summary>
    public static CadastroQuestaoDto CadastroQuestao(
        string disciplina = "POO II",
        string conteudo = "SOLID",
        string enunciado = "Enunciado") =>
        new(disciplina, conteudo, enunciado,
        [
            new AlternativaCadastroDto("Alternativa A", Correta: true),
            new AlternativaCadastroDto("Alternativa B", Correta: false),
        ]);
}
