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

        public static string ObterRowKey(DateTime inicio, Guid eventoId)
        {
            long ticksInicio = inicio.Ticks;
            long ticksInvertidos = long.MaxValue - ticksInicio;
            string ticksFormatados = ticksInvertidos.ToString().PadLeft(20, '0');
            return String.Format("{0}-{1}", ticksFormatados, eventoId);
        }

        public DateTime ObterHorarioArredondado(int segundos)
        {
            TimeSpan intervalo = TimeSpan.FromSeconds(segundos);
            long ticks = intervalo.Ticks;
            var rounded = new DateTime(((Termino.Ticks + ticks/2)/ticks)*ticks);
            return rounded;
        }
    }
}