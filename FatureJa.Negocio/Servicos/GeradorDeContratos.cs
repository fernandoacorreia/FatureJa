using System.Diagnostics;
using FatureJa.Negocio.Armazenamento;
using Microsoft.WindowsAzure.StorageClient;
using Newtonsoft.Json;

namespace FatureJa.Negocio.Servicos
{
    public class GeradorDeContratos
    {
        public void SolicitarGeracao(int quantidade)
        {
            SolicitarGeracao(1, quantidade);
        }

        public void SolicitarGeracao(int primeiro, int ultimo)
        {
            Trace.TraceInformation(string.Format("Solicitando geração dos contratos {0} a {1}.", primeiro, ultimo));
            dynamic mensagem = new
                                   {
                                       Comando = "GerarContratos",
                                       Primeiro = primeiro,
                                       Ultimo = ultimo
                                   };
            var message = new CloudQueueMessage(JsonConvert.SerializeObject(mensagem));
            CloudQueue cloudQueue = FilaDeMensagens.GetCloudQueue();
            cloudQueue.AddMessage(message);
        }
    }
}