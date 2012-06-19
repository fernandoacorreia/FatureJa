using System;
using Microsoft.WindowsAzure.StorageClient;

namespace FatureJa.Negocio.Entidades
{
    public class Fatura : TableServiceEntity
    {
        public DateTime DataDeEmissao { get; set; }

        public int NumeroDoContrato { get; set; }

        public string RazaoSocialDoCliente { get; set; }

        public string CnpjDoCliente { get; set; }

        public string MunicipioDoCliente { get; set; }

        public string UfDoCliente { get; set; }

        public double ValorTotal { get; set; }

        public static string ObterPartitionKey(int ano, int mes)
        {
            return ano.ToString() + mes.ToString().PadLeft(2, '0');
        }

        public static string ObterRowKey(int serie, int numeroFatura)
        {
            return serie.ToString().PadLeft(4, '0') + "-" + numeroFatura.ToString().PadLeft(10, '0');
        }
    }
}