using Microsoft.WindowsAzure;

namespace FatureJa.Negocio.Infraestrutura
{
    public static class CloudStorageAccountFactory
    {
        public static CloudStorageAccount ObterCloudStorageAccount()
        {
            // TODO
            string connectionString1 = "UseDevelopmentStorage=true";
            string connectionString2 =
                "DefaultEndpointsProtocol=https;AccountName=...;AccountKey=...";


            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString1);
            return storageAccount;
        }
    }
}