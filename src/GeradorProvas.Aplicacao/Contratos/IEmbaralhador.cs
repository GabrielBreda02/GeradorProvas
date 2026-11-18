using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Dominio;

namespace Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Aplicacao;

public interface IEmbaralhador
{
    List<Questao> Embaralhar(IReadOnlyList<Questao> questoes);
}
