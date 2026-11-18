namespace Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Aplicacao;

public record QuestaoDto(
    int Id,
    string Disciplina,
    string Conteudo,
    string Enunciado,
    List<AlternativaDto> Alternativas,
    char LetraCorreta);
