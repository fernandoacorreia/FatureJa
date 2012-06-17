using FatureJa.Negocio.Infraestrutura;
using Microsoft.WindowsAzure.StorageClient;

namespace FatureJa.Negocio.Armazenamento
{
    public static class FilaDeMensagensFactory
    {
        public const string Nome = "mensagens";

        public static void Inicializar()
        {
            // assegura que a fila seja criada
            CloudQueue cloudQueue = CloudQueueFactory.GetCloudQueue(Nome);
            cloudQueue.CreateIfNotExist();
        }

        public static CloudQueue GetCloudQueue()
        {
            return CloudQueueFactory.GetCloudQueue(Nome);
        }
    }
}