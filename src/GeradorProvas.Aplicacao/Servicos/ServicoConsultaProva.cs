namespace Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Aplicacao;

public class ServicoConsultaProva
{
    private readonly IRepositorioProvas _repositorio;

    public ServicoConsultaProva(IRepositorioProvas repositorio)
    {
        _repositorio = repositorio;
    }

    public ProvaDto ObterPorId(int id)
    {
        var prova = _repositorio.ObterPorId(id)
            ?? throw new RecursoNaoEncontradoException($"Prova {id} não encontrada.");

        return MapeadorProva.ParaDto(prova);
    }
}
