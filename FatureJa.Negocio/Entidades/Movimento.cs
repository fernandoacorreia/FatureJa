using Microsoft.WindowsAzure.StorageClient;

namespace FatureJa.Negocio.Entidades
{
    public class Movimento : TableServiceEntity
    {
        public string Produto { get; set; }

        public double ValorUnitario { get; set; }

        public double Quantidade { get; set; }

        public double ValorTotal { get; set; }

        public static string ObterPartitionKey(int ano, int mes)
        {
            return ano.ToString() + mes.ToString().PadLeft(2, '0');
        }

        public static string ObterRowKey(int numeroContrato, int numeroItemDeMovimento)
        {
            return Contrato.ObterRowKey(numeroContrato) + "-" + numeroItemDeMovimento.ToString().PadLeft(10, '0');
        }
    }
}