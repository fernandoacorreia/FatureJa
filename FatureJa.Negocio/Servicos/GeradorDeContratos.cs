using System;
using System.Diagnostics;
using FatureJa.Negocio.Armazenamento;
using FatureJa.Negocio.Entidades;
using Microsoft.WindowsAzure.StorageClient;
using Newtonsoft.Json;

namespace FatureJa.Negocio.Servicos
{
    public class GeradorDeContratos
    {
        public void SolicitarGeracao(int quantidade)
        {
            Guid processamentoId = Guid.NewGuid();
            RegistrarProcessamento(processamentoId, quantidade);
            SolicitarGeracao(processamentoId, 1, quantidade);
        }

        private void RegistrarProcessamento(Guid processamentoId, int quantidade)
        {
            var processamento = new Processamento
                                    {
                                        PartitionKey = Processamento.ObterPartitionKey(),
                                        RowKey = Processamento.ObterRowKey(),
                                        ProcessamentoId = processamentoId,
                                        Comando = "GerarContratos",
                                        Inicio = DateTime.UtcNow,
                                        Parametros = String.Format("Quantidade={0}", quantidade)
                                    };
            new RepositorioDeProcessamentos().Incluir(processamento);
        }

        public void SolicitarGeracao(Guid processamentoId, int primeiro, int ultimo)
        {
            Trace.TraceInformation(string.Format("Solicitando geração dos contratos {0} a {1}.", primeiro, ultimo));
            dynamic mensagem = new
                                   {
                                       Comando = "GerarContratos",
                                       ProcessamentoId = processamentoId,
                                       Primeiro = primeiro,
                                       Ultimo = ultimo
                                   };
            var message = new CloudQueueMessage(JsonConvert.SerializeObject(mensagem));
            CloudQueue cloudQueue = FilaDeMensagens.GetCloudQueue();
            cloudQueue.AddMessage(message);
        }
    }
}