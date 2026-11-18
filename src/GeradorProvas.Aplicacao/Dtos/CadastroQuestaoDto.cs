namespace Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Aplicacao;

public record CadastroQuestaoDto(
    string Disciplina,
    string Conteudo,
    string Enunciado,
    List<AlternativaCadastroDto> Alternativas);
