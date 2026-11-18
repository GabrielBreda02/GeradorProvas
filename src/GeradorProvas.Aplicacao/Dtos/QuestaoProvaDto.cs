namespace Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Aplicacao;

public record QuestaoProvaDto(
    int Numero,
    string Enunciado,
    List<AlternativaDto> Alternativas);
