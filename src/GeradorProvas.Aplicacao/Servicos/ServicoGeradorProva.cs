using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Dominio;

namespace Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Aplicacao;

public class ServicoGeradorProva
{
    private readonly IRepositorioQuestoes _repositorioQuestoes;
    private readonly IRepositorioProvas _repositorioProvas;
    private readonly IEmbaralhador _embaralhador;

    public ServicoGeradorProva(
        IRepositorioQuestoes repositorioQuestoes,
        IRepositorioProvas repositorioProvas,
        IEmbaralhador embaralhador)
    {
        _repositorioQuestoes = repositorioQuestoes;
        _repositorioProvas = repositorioProvas;
        _embaralhador = embaralhador;
    }

    public ProvaDto GerarProva(GeracaoProvaDto dados)
    {
        ValidarDados(dados);

        var questoesDoFiltro = _repositorioQuestoes.Filtrar(dados.Disciplina, dados.Conteudos);
        ValidarQuestoesSuficientes(questoesDoFiltro.Count, dados.NumeroQuestoes);

        var prova = new Prova(
            _repositorioProvas.ObterProximoId(),
            dados.Nome,
            dados.Disciplina,
            Sortear(questoesDoFiltro, dados.NumeroQuestoes));

        _repositorioProvas.Adicionar(prova);
        return MapeadorProva.ParaDto(prova);
    }

    // Nome e disciplina são validados pela própria Prova; aqui ficam só os campos que não viram estado dela.
    private static void ValidarDados(GeracaoProvaDto dados)
    {
        if (dados.Conteudos.Count == 0)
            throw new ArgumentException("Informe pelo menos um conteúdo.");

        if (dados.NumeroQuestoes <= 0)
            throw new ArgumentException("O número de questões deve ser maior que zero.");
    }

    // RN02
    private static void ValidarQuestoesSuficientes(int quantidadeDisponivel, int quantidadePedida)
    {
        if (quantidadeDisponivel < quantidadePedida)
            throw new RegraNegocioException(
                $"Não há questões suficientes para o filtro informado. " +
                $"Disponíveis: {quantidadeDisponivel}. Pedidas: {quantidadePedida}.");
    }

    // RN03: o embaralhamento não repete questões, então pegar as primeiras é um sorteio sem repetição.
    private IEnumerable<Questao> Sortear(IReadOnlyList<Questao> questoes, int quantidade) =>
        _embaralhador.Embaralhar(questoes).Take(quantidade);
}
