using FatureJa.Negocio.Armazenamento;
using Microsoft.WindowsAzure.StorageClient;
using Newtonsoft.Json;

namespace FatureJa.Negocio.Servicos
{
    public class GeradorDeMovimento
    {
        public void SolicitarGeracao(int ano, int mes)
        {
            dynamic mensagem = new
                                   {
                                       Comando = "GerarMovimento",
                                       Ano = ano,
                                       Mes = mes
                                   };
            var message = new CloudQueueMessage(JsonConvert.SerializeObject(mensagem));
            CloudQueue cloudQueue = FilaDeMensagens.GetCloudQueue();
            cloudQueue.AddMessage(message);
        }
    }
}