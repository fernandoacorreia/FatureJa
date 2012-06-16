using Microsoft.WindowsAzure.StorageClient;

namespace FatureJa.Negocio.Infraestrutura
{
    public class InicializadorDeStorage
    {
        public void Inicializar()
        {
            // assegura que a queue seja criada
            CloudQueue cloudQueue = CloudQueueFactory.Create();
            cloudQueue.CreateIfNotExist();
        }
    }
}