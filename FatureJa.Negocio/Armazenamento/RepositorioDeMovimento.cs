using System;
using System.Collections.Generic;
using System.Linq;
using FatureJa.Negocio.Entidades;
using Microsoft.WindowsAzure.StorageClient;

namespace FatureJa.Negocio.Armazenamento
{
    public class RepositorioDeMovimento : RepositorioCloudTable<Movimento>
    {
        public RepositorioDeMovimento()
        {
            Nome = "Movimento";
        }

        public IEnumerable<Movimento> ObterMovimentoDoContrato(int ano, int mes, int atual)
        {
            try
            {
                string partitionKey = Movimento.ObterPartitionKey(ano, mes);
                string rowKeyInicial = Movimento.ObterRowKey(atual, 0);
                string rowKeyFinal = Movimento.ObterRowKey(atual, int.MaxValue);

                CloudTableQuery<Movimento> query =
                    (from e in ServiceContext.CreateQuery<Movimento>(Nome)
                     where
                         e.PartitionKey == partitionKey &&
                         e.RowKey.CompareTo(rowKeyInicial) >= 0 &&
                         e.RowKey.CompareTo(rowKeyFinal) <= 0
                     select e).AsTableServiceQuery<Movimento>();

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new RepositorioCloudTableException(
                    String.Format("Erro obtendo movimento para {0}/{1} do contrato '{2}'.", mes, ano, atual), ex);
            }
        }
    }
}