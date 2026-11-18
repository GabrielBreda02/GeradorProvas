using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Aplicacao;
using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Dominio;

namespace Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Infraestrutura;

public class EmbaralhadorAleatorio : IEmbaralhador
{
    private readonly Random _aleatorio;

    public EmbaralhadorAleatorio(Random aleatorio)
    {
        _aleatorio = aleatorio;
    }

    public List<Questao> Embaralhar(IReadOnlyList<Questao> questoes)
    {
        var embaralhadas = questoes.ToArray();
        _aleatorio.Shuffle(embaralhadas);
        return [.. embaralhadas];
    }
}
