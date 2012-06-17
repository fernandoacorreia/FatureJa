using Microsoft.WindowsAzure.StorageClient;

namespace FatureJa.Negocio.Entidades
{
    public class ItemDeContrato : TableServiceEntity
    {
        public string Produto { get; set; }

        public double Valor { get; set; }

        public static string ObterPartitionKey(int numeroContrato)
        {
            return Contrato.ObterPartitionKey(numeroContrato);
        }

        public static string ObterRowKey(int numeroContrato, int numeroItem)
        {
            return Contrato.ObterRowKey(numeroContrato) + "-" + numeroItem.ToString().PadLeft(10, '0');
        }
    }
}