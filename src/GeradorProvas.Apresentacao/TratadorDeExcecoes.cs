using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Aplicacao;
using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Dominio;
using Microsoft.AspNetCore.Diagnostics;

namespace Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Apresentacao;

/// <summary>
/// Traduz as exceções das camadas internas em respostas HTTP (ProblemDetails).
/// Fica em um único lugar para que os controllers não repitam try/catch em cada ação.
/// </summary>
public class TratadorDeExcecoes : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetails;

    public TratadorDeExcecoes(IProblemDetailsService problemDetails)
    {
        _problemDetails = problemDetails;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext contexto,
        Exception excecao,
        CancellationToken cancelamento)
    {
        var status = StatusCorrespondente(excecao);
        if (status is null)
            return false;

        contexto.Response.StatusCode = status.Value;
        return await _problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = contexto,
            Exception = excecao,
            ProblemDetails = { Status = status, Detail = excecao.Message },
        });
    }

    private static int? StatusCorrespondente(Exception excecao) => excecao switch
    {
        ArgumentException => StatusCodes.Status400BadRequest,                    // RN01 e dados inválidos
        RecursoNaoEncontradoException => StatusCodes.Status404NotFound,          // RN06
        RegraNegocioException => StatusCodes.Status422UnprocessableEntity,       // RN02
        _ => null,
    };
}
