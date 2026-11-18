using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Dominio;

namespace Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Aplicacao;

/// <summary>
/// Converte <see cref="Questao"/> em DTO e concentra a regra das letras (RN04):
/// as alternativas mantêm a ordem do cadastro e recebem A, B, C...
/// </summary>
public static class MapeadorQuestao
{
    private const char PrimeiraLetra = 'A';

    public static QuestaoDto ParaDto(Questao questao) => new(
        questao.Id,
        questao.Disciplina,
        questao.Conteudo,
        questao.Enunciado,
        ParaAlternativasDto(questao.Alternativas),
        LetraDaAlternativaCorreta(questao));

    public static List<AlternativaDto> ParaAlternativasDto(IReadOnlyList<Alternativa> alternativas) =>
        alternativas
            .Select((alternativa, posicao) => new AlternativaDto(LetraDaPosicao(posicao), alternativa.Texto))
            .ToList();

    private static char LetraDaAlternativaCorreta(Questao questao) =>
        LetraDaPosicao(questao.Alternativas.ToList().FindIndex(alternativa => alternativa.Correta));

    private static char LetraDaPosicao(int posicao) => (char)(PrimeiraLetra + posicao);
}
