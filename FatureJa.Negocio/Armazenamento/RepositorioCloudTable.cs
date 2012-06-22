using System;
using System.Data.Services.Client;
using FatureJa.Negocio.Infraestrutura;
using Microsoft.WindowsAzure.StorageClient;

namespace FatureJa.Negocio.Armazenamento
{
    public class RepositorioCloudTable<T> where T : TableServiceEntity
    {
        public RepositorioCloudTable()
        {
            TableClient = CloudTableClientFactory.GetCloudTableClient();
            CriarNovoContexto();
        }

        public string Nome { get; set; }

        public CloudTableClient TableClient { get; protected set; }

        public TableServiceContext ServiceContext { get; protected set; }

        public void CriarNovoContexto()
        {
            ServiceContext = TableClient.GetDataServiceContext();
        }

        public void Inicializar()
        {
            TableClient.CreateTableIfNotExist(Nome);
        }

        public void Incluir(T objeto)
        {
            try
            {
                ServiceContext.AddObject(Nome, objeto);
                ServiceContext.SaveChangesWithRetries();
            }
            catch (Exception ex)
            {
                throw new RepositorioCloudTableException(
                    String.Format("Erro incluindo objeto em RepositorioCloudTable '{0}'.", Nome), ex);
            }
        }

        public void AdicionarObjeto(T contrato)
        {
            try
            {
                ServiceContext.AddObject(Nome, contrato);
            }
            catch (Exception ex)
            {
                throw new RepositorioCloudTableException(
                    String.Format("Erro adicionando objeto em RepositorioCloudTable '{0}'.", Nome), ex);
            }
        }

        public void SalvarLote()
        {
            try
            {
                ServiceContext.SaveChangesWithRetries(SaveChangesOptions.Batch);
            }
            catch (Exception ex)
            {
                throw new RepositorioCloudTableException(
                    String.Format("Erro salvando lote de objetos em RepositorioCloudTable '{0}'.", Nome), ex);
            }
        }
    }
}