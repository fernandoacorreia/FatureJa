using System;
using System.Diagnostics;
using System.Threading;
using FatureJa.Negocio.Armazenamento;
using FatureJa.Negocio.Mensagens;

namespace FatureJa.Worker
{
    public static class LoopPrincipal
    {
        public static void Executar(TimeSpan intervalo)
        {
            InicializadorDeArmazenamento.Inicializar();

            var processador = new ProcessadorDeMensagens();
            while (true)
            {
                processador.ProcessarMensagensNaFila();
                Trace.WriteLine("Nenhuma mensagem na fila. Aguardando...", "Information");
                Thread.Sleep(intervalo);
            }
        }
    }
}