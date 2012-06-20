using Microsoft.WindowsAzure;

namespace FatureJa.Negocio.Infraestrutura
{
    public static class CloudStorageAccountFactory
    {
        public static CloudStorageAccount ObterCloudStorageAccount()
        {
            string stringDeConexao = AmbienteDaNuvem.StringDeConexaoAStorage;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(stringDeConexao);
            return storageAccount;
        }
    }
}