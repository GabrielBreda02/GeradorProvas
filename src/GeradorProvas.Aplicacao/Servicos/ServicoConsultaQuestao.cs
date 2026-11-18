namespace Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Aplicacao;

public class ServicoConsultaQuestao
{
    private readonly IRepositorioQuestoes _repositorio;

    public ServicoConsultaQuestao(IRepositorioQuestoes repositorio)
    {
        _repositorio = repositorio;
    }

    public QuestaoDto ObterPorId(int id)
    {
        var questao = _repositorio.ObterPorId(id)
            ?? throw new RecursoNaoEncontradoException($"Questão {id} não encontrada.");

        return MapeadorQuestao.ParaDto(questao);
    }

    public List<QuestaoDto> Listar(string disciplina, string? conteudo)
    {
        if (string.IsNullOrWhiteSpace(disciplina))
            throw new ArgumentException("Informe a disciplina para listar as questões.");

        return _repositorio.Listar(disciplina, conteudo)
            .Select(MapeadorQuestao.ParaDto)
            .ToList();
    }
}
