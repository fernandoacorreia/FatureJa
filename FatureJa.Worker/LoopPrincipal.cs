﻿using System.Diagnostics;
using System.Threading;
using FatureJa.Negocio.Mensagens;

namespace FatureJa.Worker
{
    public static class LoopPrincipal
    {
        public static void Executar()
        {
            var processador = new ProcessadorDeMensagens();
            while (true)
            {
                processador.ProcessarMensagensNaFila();
                Trace.WriteLine("Nenhuma mensagem na fila. Aguardando...", "Information");
                Thread.Sleep(10000);
            }
        }
    }
}