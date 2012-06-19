using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace FatureJa.Negocio.Infraestrutura
{
    internal static class CloudQueueFactory
    {
        public static CloudQueue GetCloudQueue(string nome)
        {
            // Retrieve storage account from connection-string
            CloudStorageAccount storageAccount = CloudStorageAccountFactory.ObterCloudStorageAccount();

            // Create the queue client
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue
            CloudQueue queue = queueClient.GetQueueReference(nome);

            return queue;
        }
    }
}