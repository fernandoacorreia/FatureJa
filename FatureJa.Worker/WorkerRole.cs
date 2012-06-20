using System;
using System.Diagnostics;
using System.Net;
using FatureJa.Negocio.Infraestrutura;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace FatureJa.Worker
{
    public class WorkerRole : RoleEntryPoint
    {
        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.WriteLine("$projectname$ entry point called");

            LoopPrincipal.Executar(TimeSpan.FromSeconds(10));
        }

        public override bool OnStart()
        {
            Diagnostico.Configurar();
            Trace.WriteLine("Executando base.OnStart() de WorkerRole.");

            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 24;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }
    }
}