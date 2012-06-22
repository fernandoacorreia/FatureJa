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
            try
            {
                return (from e in ServiceContext.CreateQuery<EventoDeProcessamento>(Nome)
                        where e.PartitionKey == processamentoId.ToString()
                        select e).AsTableServiceQuery().ToList();
            }
            catch (Exception ex)
            {
                throw new RepositorioCloudTableException(
                    String.Format("Erro obtendo eventos do processamento '{0}'.", processamentoId), ex);
            }
        }
    }
}