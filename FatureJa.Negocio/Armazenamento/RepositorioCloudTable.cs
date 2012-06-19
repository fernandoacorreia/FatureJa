using FatureJa.Negocio.Infraestrutura;
using Microsoft.WindowsAzure.StorageClient;

namespace FatureJa.Negocio.Armazenamento
{
    public class RepositorioCloudTable<T> where T : TableServiceEntity
    {
        public RepositorioCloudTable()
        {
            CloudTableClient = CloudTableClientFactory.GetCloudTableClient();
            TableServiceContext = CloudTableClient.GetDataServiceContext();
        }

        public string Nome { get; set; }

        public CloudTableClient CloudTableClient { get; protected set; }

        public TableServiceContext TableServiceContext { get; protected set; }

        public void Inicializar()
        {
            CloudTableClient.CreateTableIfNotExist(Nome);
        }

        public void Incluir(T objeto)
        {
            TableServiceContext.AddObject(Nome, objeto);
            TableServiceContext.SaveChangesWithRetries();
        }
    }
}