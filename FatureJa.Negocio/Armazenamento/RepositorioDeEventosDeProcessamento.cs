using System;
using System.Collections.Generic;
using System.Linq;
using FatureJa.Negocio.Entidades;
using Microsoft.WindowsAzure.StorageClient;

namespace FatureJa.Negocio.Armazenamento
{
    public class RepositorioDeEventosDeProcessamento : RepositorioCloudTable<EventoDeProcessamento>
    {
        public RepositorioDeEventosDeProcessamento()
        {
            Nome = "EventosDeProcessamento";
        }

        public IEnumerable<EventoDeProcessamento> ObterUltimosEventos(Guid processamentoId, int take)
        {
            TableServiceContext serviceContext = CloudTableClient.GetDataServiceContext();
            CloudTableQuery<EventoDeProcessamento> query =
                (from e in serviceContext.CreateQuery<EventoDeProcessamento>(Nome)
                 where e.PartitionKey == processamentoId.ToString()
                 select e).Take(take).AsTableServiceQuery();
            return query.ToList();
        }
    }
}