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

        public List<EventoDeProcessamento> ObterEventos(Guid processamentoId)
        {
            TableServiceContext serviceContext = CloudTableClient.GetDataServiceContext();
            return (from e in serviceContext.CreateQuery<EventoDeProcessamento>(Nome)
                    where e.PartitionKey == processamentoId.ToString()
                    select e).AsTableServiceQuery().ToList();
        }
    }
}