using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace FatureJa.Negocio.Infraestrutura
{
    internal class CloudTableClientFactory
    {
        public static CloudTableClient GetCloudTableClient()
        {
            // Retrieve storage account from connection-string
            CloudStorageAccount storageAccount = CloudStorageAccountFactory.ObterCloudStorageAccount();

            // Create the table client
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            return tableClient;
        }
    }
}