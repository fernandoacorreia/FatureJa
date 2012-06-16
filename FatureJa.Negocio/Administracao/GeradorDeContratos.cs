using FatureJa.Negocio.Infraestrutura;
using Microsoft.WindowsAzure.StorageClient;
using Newtonsoft.Json;

namespace FatureJa.Negocio.Administracao
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
            CloudQueue cloudQueue = CloudQueueFactory.Create();
            cloudQueue.AddMessage(message);
        }
    }
}