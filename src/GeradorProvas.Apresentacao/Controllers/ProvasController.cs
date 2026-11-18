using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Aplicacao;
using Microsoft.AspNetCore.Mvc;

namespace Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Apresentacao;

[ApiController]
[Route("provas")]
public class ProvasController : ControllerBase
{
    private readonly ServicoGeradorProva _servicoGerador;
    private readonly ServicoConsultaProva _servicoConsulta;

    public ProvasController(ServicoGeradorProva servicoGerador, ServicoConsultaProva servicoConsulta)
    {
        _servicoGerador = servicoGerador;
        _servicoConsulta = servicoConsulta;
    }

    [HttpPost]
    public ActionResult<ProvaDto> Gerar(GeracaoProvaDto dados)
    {
        var prova = _servicoGerador.GerarProva(dados);
        return CreatedAtAction(nameof(Obter), new { id = prova.Id }, prova);
    }

    [HttpGet("{id:int}")]
    public ActionResult<ProvaDto> Obter(int id) =>
        Ok(_servicoConsulta.ObterPorId(id));
}
