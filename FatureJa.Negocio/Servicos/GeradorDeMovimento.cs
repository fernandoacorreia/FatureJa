using System.Diagnostics;
using FatureJa.Negocio.Armazenamento;
using Microsoft.WindowsAzure.StorageClient;
using Newtonsoft.Json;

namespace FatureJa.Negocio.Servicos
{
    public class GeradorDeMovimento
    {
        public void SolicitarGeracao(int ano, int mes)
        {
            int ultimoContrato = TabelaDeContratos.ObterNumeroDoUltimoContrato();
            if (ultimoContrato == 0)
            {
                Trace.TraceInformation("Não foi encontrado nenhum contrato.");
                return;
            }
            SolicitarGeracao(ano, mes, 1, ultimoContrato);
        }

        public void SolicitarGeracao(int ano, int mes, int primeiro, int ultimo)
        {
            Trace.TraceInformation(string.Format("Solicitando geração de movimento para {0}/{1} para os contratos {2} a {3}.", mes, ano, primeiro, ultimo));
            dynamic mensagem = new
                                   {
                                       Comando = "GerarMovimento",
                                       Ano = ano,
                                       Mes = mes,
                                       Primeiro = primeiro,
                                       Ultimo = ultimo
                                   };
            var message = new CloudQueueMessage(JsonConvert.SerializeObject(mensagem));
            CloudQueue cloudQueue = FilaDeMensagens.GetCloudQueue();
            cloudQueue.AddMessage(message);
        }
    }
}