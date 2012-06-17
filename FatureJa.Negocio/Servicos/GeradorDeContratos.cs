using FatureJa.Negocio.Armazenamento;
using Microsoft.WindowsAzure.StorageClient;
using Newtonsoft.Json;

namespace FatureJa.Negocio.Servicos
{
    public class GeradorDeContratos
    {
        public void SolicitarGeracao(int quantidade)
        {
            dynamic mensagem = new
                                   {
                                       Comando = "GerarContratos",
                                       Quantidade = quantidade
                                   };
            var message = new CloudQueueMessage(JsonConvert.SerializeObject(mensagem));
            CloudQueue cloudQueue = FilaDeMensagens.GetCloudQueue();
            cloudQueue.AddMessage(message);
        }
    }
}