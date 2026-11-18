namespace Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Aplicacao;

public class RecursoNaoEncontradoException : Exception
{
    public RecursoNaoEncontradoException(string mensagem)
        : base(mensagem)
    {
    }
}
