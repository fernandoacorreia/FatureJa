using FatureJa.Negocio.Entidades;
using FatureJa.Negocio.Infraestrutura;
using Microsoft.WindowsAzure.StorageClient;

namespace FatureJa.Negocio.Armazenamento
{
    public class RepositorioDeProcessamentos
    {
        public const string Nome = "Processamentos";

        public RepositorioDeProcessamentos()
        {
            CloudTableClient = CloudTableClientFactory.GetCloudTableClient();
            TableServiceContext = CloudTableClient.GetDataServiceContext();
        }

        public CloudTableClient CloudTableClient { get; private set; }

        public TableServiceContext TableServiceContext { get; private set; }

        public void Inicializar()
        {
            CloudTableClient.CreateTableIfNotExist(Nome);
        }

        public void Incluir(Processamento processamento)
        {
            TableServiceContext.AddObject(Nome, processamento);
            TableServiceContext.SaveChangesWithRetries();
        }
    }
}