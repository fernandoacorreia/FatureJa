using System.Diagnostics;

namespace FatureJa.Negocio.Administracao
{
    public class GeradorDeContratos
    {
        public void SolicitarGeracao(int quantidade)
        {
            Debug.WriteLine("Solicitar a geração de contratos: " + quantidade);
        }
    }
}