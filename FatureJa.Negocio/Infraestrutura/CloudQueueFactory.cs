using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace FatureJa.Negocio.Infraestrutura
{
    public static class CloudQueueFactory
    {
        private const string _queueName = "fatureja";

        public static CloudQueue Create()
        {
            // Retrieve storage account from connection-string
            string connectionString = "UseDevelopmentStorage=true"; // TODO
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            // Create the queue client
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue
            CloudQueue queue = queueClient.GetQueueReference(_queueName);

            return queue;
        }
    }
}