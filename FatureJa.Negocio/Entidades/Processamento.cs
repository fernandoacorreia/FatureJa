using System;
using Microsoft.WindowsAzure.StorageClient;

namespace FatureJa.Negocio.Entidades
{
    public class Processamento : TableServiceEntity
    {
        public DateTime Inicio { get; set; }

        public string Comando { get; set; }

        public string Parametros { get; set; }

        public static string ObterPartitionKey(Guid processamentoId)
        {
            return processamentoId.ToString();
        }

        public static string ObterRowKey()
        {
            return String.Empty;
        }
    }
}