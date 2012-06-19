using FatureJa.Negocio.Infraestrutura;
using Microsoft.WindowsAzure.StorageClient;

namespace FatureJa.Negocio.Armazenamento
{
    public class TabelaDeFaturas
    {
        public const string Nome = "Faturas";

        public static void Inicializar()
        {
            // assegura que a tabela seja criada
            CloudTableClient tableClient = GetCloudTableClient();
            tableClient.CreateTableIfNotExist(Nome);
        }

        public static CloudTableClient GetCloudTableClient()
        {
            return CloudTableClientFactory.GetCloudTableClient();
        }
    }
}