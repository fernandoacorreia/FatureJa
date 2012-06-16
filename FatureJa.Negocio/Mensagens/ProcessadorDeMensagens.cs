using System;
using System.Diagnostics;
using FatureJa.Negocio.Infraestrutura;
using Microsoft.WindowsAzure.StorageClient;
using Newtonsoft.Json;

namespace FatureJa.Negocio.Mensagens
{
    public class ProcessadorDeMensagens
    {
        public void ProcessarMensagensNaFila()
        {
            TimeSpan visibilityTimeout = TimeSpan.FromSeconds(60);
            const int limiteDeTentativas = 3; // número máximo de tentativas de processamento da mesma mensagem

            Trace.WriteLine("Processando mensagens na fila.", "Information");
            CloudQueue cloudQueue = CloudQueueFactory.Create();
            while (true) // repetir enquanto houver mensagens na fila
            {
                CloudQueueMessage mensagem = cloudQueue.GetMessage(visibilityTimeout);
                if (mensagem == null)
                {
                    return; // a fila está vazia
                }
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
                            ProcessarMensagem(mensagemDeserializada);
                        }
                    }
                    //cloudQueue.DeleteMessage(mensagem);
                    // TODO
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(
                        String.Format("Erro processando mensagem '{0}': '{1}'.", mensagem.AsString, ex.Message), "Error");
                }
            }
        }

        private void ProcessarMensagem(dynamic mensagem)
        {
            string comando = mensagem.Comando;
            if (comando == "GerarContratos")
            {
                Trace.WriteLine("Gerando contratos.", "Information");
            }
            else
            {
                Trace.WriteLine(String.Format("O comando '{0}' não foi reconhecido.", comando), "Error");
            }
        }
    }
}