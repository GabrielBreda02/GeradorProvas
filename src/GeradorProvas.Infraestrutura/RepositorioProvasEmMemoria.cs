using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Aplicacao;
using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Dominio;

namespace Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Infraestrutura;

public class RepositorioProvasEmMemoria : IRepositorioProvas
{
    private readonly List<Prova> _provas = [];

    // O repositório é Singleton e atende requisições simultâneas: a trava protege a lista e o contador.
    private readonly Lock _trava = new();

    private int _ultimoId;

    public int ObterProximoId()
    {
        lock (_trava)
            return ++_ultimoId;
    }

    public void Adicionar(Prova prova)
    {
        lock (_trava)
            _provas.Add(prova);
    }

    public Prova? ObterPorId(int id)
    {
        lock (_trava)
            return _provas.Find(prova => prova.Id == id);
    }
}
