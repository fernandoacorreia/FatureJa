using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.Diagnostics;

namespace FatureJa.Negocio.Infraestrutura
{
    public static class Diagnostico
    {
        public static void Configurar()
        {
            TimeSpan span = TimeSpan.FromMinutes(1);

            Trace.WriteLine("Configurando diagnósticos.");

            DiagnosticMonitorConfiguration config = DiagnosticMonitor.GetDefaultInitialConfiguration();

            config.DiagnosticInfrastructureLogs.ScheduledTransferLogLevelFilter = LogLevel.Information;
            config.DiagnosticInfrastructureLogs.ScheduledTransferPeriod = span;

            config.Logs.ScheduledTransferLogLevelFilter = LogLevel.Information;
            config.Logs.ScheduledTransferPeriod = span;

            config.WindowsEventLog.DataSources.Add("System!*");
            config.WindowsEventLog.DataSources.Add("Application!*");
            config.WindowsEventLog.ScheduledTransferLogLevelFilter = LogLevel.Error;
            config.WindowsEventLog.ScheduledTransferPeriod = span;

            DiagnosticMonitor.Start("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString", config);
        }
    }
}
