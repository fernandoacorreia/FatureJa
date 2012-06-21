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
            // Desativar Nagle para melhorar a performance
            // http://blogs.msdn.com/b/windowsazurestorage/archive/2010/06/25/nagle-s-algorithm-is-not-friendly-towards-small-requests.aspx
            ServicePointManager.UseNagleAlgorithm = false;

            // Set the maximum number of concurrent connections
            // http://social.technet.microsoft.com/Forums/en-US/windowsazuredata/thread/d84ba34b-b0e0-4961-a167-bbe7618beb83
            ServicePointManager.DefaultConnectionLimit = 100;

            // Turn off 100-continue (saves 1 roundtrip)
            ServicePointManager.Expect100Continue = false;

            // Configurar diagnóstico
            Diagnostico.Configurar();
            Trace.WriteLine("Executando base.OnStart() de WorkerRole.");

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }
    }
}