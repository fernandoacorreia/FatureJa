using System.Diagnostics;

namespace FatureJa.Negocio.Mensagens
{
    public class ProcessadorDeMensagens
    {
        public void ProcessarMensagensNaFila()
        {
            Trace.WriteLine("Processando mensagens na fila.", "Information");
        }
    }
}