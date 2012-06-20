using System;
using Microsoft.WindowsAzure.StorageClient;

namespace FatureJa.Negocio.Entidades
{
    public class Processamento : TableServiceEntity
    {
        public DateTime Inicio { get; set; }

        public string Comando { get; set; }

        public string Parametros { get; set; }

        public Guid ProcessamentoId { get; set; }

        public static string ObterPartitionKey()
        {
            return DateTime.UtcNow.ToString("yyyyMM");
        }

        public static string ObterRowKey(Guid processamentoId)
        {
            long ticksInicio = DateTime.UtcNow.Ticks;
            long ticksInvertidos = long.MaxValue - ticksInicio;
            string ticksFormatados = ticksInvertidos.ToString().PadLeft(20, '0');
            return String.Format("{0}-{1}", ticksFormatados, processamentoId);
        }
    }
}