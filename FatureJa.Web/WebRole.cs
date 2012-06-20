using System.Diagnostics;
using FatureJa.Negocio.Infraestrutura;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace FatureJa.Web
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            Diagnostico.Configurar();
            Trace.WriteLine("Executando base.OnStart() de WebRole.");
            return base.OnStart();
        }
    }
}