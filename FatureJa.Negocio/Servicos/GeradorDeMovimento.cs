using System;
using System.Diagnostics;
using FatureJa.Negocio.Armazenamento;
using FatureJa.Negocio.Entidades;
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
                Trace.TraceWarning("Não foi encontrado nenhum contrato.");
                return;
            }

            Guid processamentoId = Guid.NewGuid();
            RegistrarProcessamento(processamentoId, ano, mes);
            SolicitarGeracao(processamentoId, ano, mes, 1, ultimoContrato);
        }

        private void RegistrarProcessamento(Guid processamentoId, int ano, int mes)
        {
            var processamento = new Processamento
                                    {
                                        PartitionKey = Processamento.ObterPartitionKey(),
                                        RowKey = Processamento.ObterRowKey(processamentoId),
                                        ProcessamentoId = processamentoId,
                                        Comando = "GerarMovimento",
                                        Inicio = DateTime.UtcNow,
                                        Parametros = String.Format("Ano={0}; Mes={1}", ano, mes)
                                    };
            new RepositorioDeProcessamentos().Incluir(processamento);
        }

        public void SolicitarGeracao(Guid processamentoId, int ano, int mes, int primeiro, int ultimo)
        {
            Trace.WriteLine(string.Format("Solicitando geração de movimento para {0}/{1} para os contratos {2} a {3}.",
                                          mes, ano, primeiro, ultimo));
            dynamic mensagem = new
                                   {
                                       Comando = "GerarMovimento",
                                       ProcessamentoId = processamentoId,
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