using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Aplicacao;
using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Dominio;

namespace Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Testes;

/// <summary>
/// Troca o sorteio por uma ordem previsível (a lista ao contrário), para o teste saber
/// exatamente quais questões entram na prova e em que ordem. Só é possível porque o
/// serviço depende da abstração <see cref="IEmbaralhador"/> (DIP).
/// </summary>
internal class EmbaralhadorInvertido : IEmbaralhador
{
    public List<Questao> Embaralhar(IReadOnlyList<Questao> questoes) =>
        Enumerable.Reverse(questoes).ToList();
}
