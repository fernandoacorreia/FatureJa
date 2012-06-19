using System.Collections.Generic;
using System.Linq;
using FatureJa.Negocio.Entidades;
using Microsoft.WindowsAzure.StorageClient;

namespace FatureJa.Negocio.Armazenamento
{
    public class RepositorioDeProcessamentos : RepositorioCloudTable<Processamento>
    {
        public RepositorioDeProcessamentos()
        {
            Nome = "Processamentos";
        }

        public IEnumerable<Processamento> ObterUltimosProcessamentos()
        {
            TableServiceContext serviceContext = CloudTableClient.GetDataServiceContext();
            CloudTableQuery<Processamento> query =
                (from e in serviceContext.CreateQuery<Processamento>(Nome)
                 select e).Take(10).AsTableServiceQuery<Processamento>();
            return query.ToList();
        }
    }
}