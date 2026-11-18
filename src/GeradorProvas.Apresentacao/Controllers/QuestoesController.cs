using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Aplicacao;
using Microsoft.AspNetCore.Mvc;

namespace Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Apresentacao;

[ApiController]
[Route("questoes")]
public class QuestoesController : ControllerBase
{
    private readonly ServicoCadastroQuestao _servicoCadastro;
    private readonly ServicoConsultaQuestao _servicoConsulta;

    public QuestoesController(ServicoCadastroQuestao servicoCadastro, ServicoConsultaQuestao servicoConsulta)
    {
        _servicoCadastro = servicoCadastro;
        _servicoConsulta = servicoConsulta;
    }

    [HttpPost]
    public ActionResult Cadastrar(CadastroQuestaoDto dados)
    {
        var id = _servicoCadastro.Cadastrar(dados);
        return CreatedAtAction(nameof(Obter), new { id }, new { id });
    }

    [HttpGet("{id:int}")]
    public ActionResult<QuestaoDto> Obter(int id) =>
        Ok(_servicoConsulta.ObterPorId(id));

    [HttpGet]
    public ActionResult<List<QuestaoDto>> Listar([FromQuery] string disciplina, [FromQuery] string? conteudo) =>
        Ok(_servicoConsulta.Listar(disciplina, conteudo));
}
