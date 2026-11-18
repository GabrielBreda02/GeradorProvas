using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Dominio;

namespace Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Aplicacao;

public static class MapeadorProva
{
    public static ProvaDto ParaDto(Prova prova) => new(
        prova.Id,
        prova.Nome,
        prova.Disciplina,
        prova.Questoes
            .Select((questao, posicao) => ParaQuestaoProvaDto(questao, numero: posicao + 1))
            .ToList());

    private static QuestaoProvaDto ParaQuestaoProvaDto(Questao questao, int numero) => new(
        numero,
        questao.Enunciado,
        MapeadorQuestao.ParaAlternativasDto(questao.Alternativas));
}
