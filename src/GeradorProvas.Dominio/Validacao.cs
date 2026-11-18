namespace Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Dominio;

/// <summary>
/// Verificações compartilhadas pelas entidades, para que a regra de "campo obrigatório"
/// e sua mensagem fiquem em um único lugar.
/// </summary>
internal static class Validacao
{
    public static string TextoObrigatorio(string? valor, string nomeCampo)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException($"O campo '{nomeCampo}' é obrigatório.");

        return valor;
    }
}
