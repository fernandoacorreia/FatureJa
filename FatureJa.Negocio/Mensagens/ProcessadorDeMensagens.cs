using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using FatureJa.Negocio.Armazenamento;
using Microsoft.WindowsAzure.StorageClient;
using Newtonsoft.Json;

namespace FatureJa.Negocio.Mensagens
{
    public class ProcessadorDeMensagens
    {
        public void ProcessarMensagensNaFila()
        {
            Trace.TraceInformation("Processando mensagens da fila.");
            CloudQueue fila = FilaDeMensagens.GetCloudQueue();

            Parallel.ForEach<CloudQueueMessage>(ObterMensagens(fila), (mensagem) => ProcessarMensagem(fila, mensagem));
        }

        private static IEnumerable<CloudQueueMessage> ObterMensagens(CloudQueue fila)
        {
            TimeSpan tempoDeInvisibilidade = TimeSpan.FromSeconds(180);

            while (true)
            {
                var mensagem = fila.GetMessage(tempoDeInvisibilidade);
                if (mensagem != null)
                {
                    yield return mensagem;
                }
                else
                {
                    yield break;
                }
            }
        }

        private void ProcessarMensagem(CloudQueue fila, CloudQueueMessage mensagem)
        {
            const int limiteDeTentativas = 3; // número máximo de tentativas de processamento da mesma mensagem

            try
            {
                if (mensagem.DequeueCount > limiteDeTentativas)
                {
                    Trace.WriteLine(
                        String.Format("A mensagem não pôde ser processada após várias tentativas: '{0}'.",
                                      mensagem.AsString), "Error");
                }
                else
                {
                    var mensagemDeserializada = JsonConvert.DeserializeObject<dynamic>(mensagem.AsString);
                    if (mensagemDeserializada == null)
                    {
                        Trace.WriteLine(
                            String.Format(
                                "A mensagem não é válida; a deserialização resultou em um objeto nulo: '{0}'.",
                                mensagem.AsString), "Error");
                    }
                    else
                    {
                        new DespachanteDeMensagem().Despachar(mensagemDeserializada);
                    }
                }
                fila.DeleteMessage(mensagem);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(
                    String.Format("Erro processando mensagem '{0}': '{1}'.", mensagem.AsString, ex.Message), "Error");
            }
        }
    }
}