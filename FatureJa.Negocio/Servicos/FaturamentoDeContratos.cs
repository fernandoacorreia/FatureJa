using System.Diagnostics;
using FatureJa.Negocio.Armazenamento;
using Microsoft.WindowsAzure.StorageClient;
using Newtonsoft.Json;

namespace FatureJa.Negocio.Servicos
{
    public class FaturamentoDeContratos
    {
        public void SolicitarFaturamento(int ano, int mes)
        {
            int ultimoContrato = TabelaDeContratos.ObterNumeroDoUltimoContrato();
            if (ultimoContrato == 0)
            {
                Trace.TraceInformation("Não foi encontrado nenhum contrato.");
                return;
            }
            SolicitarFaturamento(ano, mes, 1, ultimoContrato);
        }

        public void SolicitarFaturamento(int ano, int mes, int primeiro, int ultimo)
        {
            Trace.TraceInformation(string.Format("Solicitando faturamento para {0}/{1} dos contratos {2} a {3}.", mes, ano, primeiro, ultimo));
            dynamic mensagem = new
                                   {
                                       Comando = "Faturar",
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