namespace Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Aplicacao;

public record GeracaoProvaDto(
    string Nome,
    string Disciplina,
    List<string> Conteudos,
    int NumeroQuestoes);
