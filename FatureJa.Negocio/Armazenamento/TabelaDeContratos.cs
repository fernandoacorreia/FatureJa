using System.Linq;
using FatureJa.Negocio.Entidades;
using FatureJa.Negocio.Infraestrutura;
using Microsoft.WindowsAzure.StorageClient;

namespace FatureJa.Negocio.Armazenamento
{
    public static class TabelaDeContratos
    {
        public const string Nome = "Contratos";

        public static void Inicializar()
        {
            // assegura que a tabela seja criada
            CloudTableClient tableClient = GetCloudTableClient();
            tableClient.CreateTableIfNotExist(Nome);
        }

        public static CloudTableClient GetCloudTableClient()
        {
            return CloudTableClientFactory.GetCloudTableClient();
        }

        public static int ObterNumeroDoUltimoContrato()
        {
            CloudTableClient tableClient = GetCloudTableClient();
            TableServiceContext serviceContext = tableClient.GetDataServiceContext();
            Contrato contrato =
                (from e in serviceContext.CreateQuery<Contrato>(Nome)
                 select e).FirstOrDefault();
            if (contrato == null)
            {
                return 0;
            }
            return contrato.Numero;
        }
    }
}