using System;
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

        public IEnumerable<Processamento> ObterUltimosProcessamentos(int take)
        {
            TableServiceContext serviceContext = CloudTableClient.GetDataServiceContext();
            CloudTableQuery<Processamento> query =
                (from p in serviceContext.CreateQuery<Processamento>(Nome)
                 select p).Take(take).AsTableServiceQuery();
            return query.ToList();
        }

        public Processamento ObterPorProcessamentoId(Guid processamentoId)
        {
            TableServiceContext serviceContext = CloudTableClient.GetDataServiceContext();
            return
                (from p in serviceContext.CreateQuery<Processamento>(Nome)
                 where p.ProcessamentoId == processamentoId
                 select p).FirstOrDefault();
        }
    }
}