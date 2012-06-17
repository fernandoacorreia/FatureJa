using Microsoft.WindowsAzure.StorageClient;

namespace FatureJa.Negocio.Entidades
{
    public class Contrato : TableServiceEntity
    {
        public const int NumeroMaximoDeContrato = 1000000000;

        public int Numero { get; set; }

        public string RazaoSocialDoCliente { get; set; }

        public string CnpjDoCliente { get; set; }

        public string MunicipioDoCliente { get; set; }

        public string UfDoCliente { get; set; }

        public static string ObterPartitionKey(int numero)
        {
            int grupo = ObterGrupo(numero);
            int partitionKeyInvertida = NumeroMaximoDeContrato - grupo;
            string partitionKey = partitionKeyInvertida.ToString().PadLeft(10, '0');
            return partitionKey;
        }

        public static string ObterRowKey(int numero)
        {
            int rowKeyInvertida = NumeroMaximoDeContrato - numero;
            string rowKey = rowKeyInvertida.ToString().PadLeft(10, '0');
            return rowKey;
        }

        public static int ObterGrupo(int numeroDoContrato)
        {
            int grupo = (numeroDoContrato - 1)/1000;
            return grupo;
        }
    }
}