namespace Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Dominio;

public class Alternativa
{
    public Alternativa(string texto, bool correta)
    {
        Texto = Validacao.TextoObrigatorio(texto, nameof(texto));
        Correta = correta;
    }

    public string Texto { get; }

    public bool Correta { get; }
}
