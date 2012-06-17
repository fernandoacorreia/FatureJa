﻿using FatureJa.Negocio.Infraestrutura;
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
    }
}