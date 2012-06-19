using System;
using Microsoft.WindowsAzure.StorageClient;

namespace FatureJa.Negocio.Entidades
{
    public class EventoDeProcessamento : TableServiceEntity
    {
        public DateTime Inicio { get; set; }

        public DateTime Termino { get; set; }

        public string Comando { get; set; }

        public int Quantidade { get; set; }

        public static string ObterPartitionKey(Guid processamentoId)
        {
            return processamentoId.ToString();
        }

        public static string ObterRowKey(DateTime inicio)
        {
            long ticksInicio = inicio.Ticks;
            long ticksInvertidos = long.MaxValue - ticksInicio;
            string ticksFormatados = ticksInvertidos.ToString().PadLeft(20, '0');
            return ticksFormatados;
        }
    }
}