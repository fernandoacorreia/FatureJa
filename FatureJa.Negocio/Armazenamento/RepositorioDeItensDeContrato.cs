using System;
using System.Collections.Generic;
using System.Linq;
using FatureJa.Negocio.Entidades;
using Microsoft.WindowsAzure.StorageClient;

namespace FatureJa.Negocio.Armazenamento
{
    public class RepositorioDeItensDeContrato : RepositorioCloudTable<ItemDeContrato>
    {
        public RepositorioDeItensDeContrato()
        {
            Nome = "ItensDeContrato";
        }

        public IEnumerable<ItemDeContrato> ObterItensDoContrato(int atual)
        {
            try
            {
                string partitionKey = Contrato.ObterPartitionKey(atual);
                string rowKeyInicial = ItemDeContrato.ObterRowKey(atual, 0);
                string rowKeyFinal = ItemDeContrato.ObterRowKey(atual, int.MaxValue);

                CloudTableQuery<ItemDeContrato> query =
                    (from e in ServiceContext.CreateQuery<ItemDeContrato>(Nome)
                     where
                         e.PartitionKey == partitionKey &&
                         e.RowKey.CompareTo(rowKeyInicial) >= 0 &&
                         e.RowKey.CompareTo(rowKeyFinal) <= 0
                     select e).AsTableServiceQuery<ItemDeContrato>();

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new RepositorioCloudTableException(
                    String.Format("Erro obtendo itens do contrato '{0}'.", atual), ex);
            }
        }
    }
}