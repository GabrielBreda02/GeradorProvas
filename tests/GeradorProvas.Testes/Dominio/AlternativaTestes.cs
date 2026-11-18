using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Dominio;

namespace Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Testes;

public class AlternativaTestes
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Construtor_ComTextoEmBranco_LancaArgumentException(string texto)
    {
        Assert.Throws<ArgumentException>(() => new Alternativa(texto, correta: true));
    }
}
