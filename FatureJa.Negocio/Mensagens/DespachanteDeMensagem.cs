using System;
using System.Diagnostics;
using System.Threading;

namespace FatureJa.Negocio.Mensagens
{
    public class DespachanteDeMensagem
    {
        public void Despachar(dynamic mensagem)
        {
            string comando = mensagem.Comando;

            string threadId = Thread.CurrentThread.ManagedThreadId.ToString();
            Trace.TraceInformation(String.Format("Despachando mensagem '{0}' na thread '{1}'.", comando, threadId));

            if (comando == "GerarContratos")
            {
                new ProcessadorDeGerarContratos().Processar(mensagem);
            }
            else if (comando == "GerarLoteDeContratos")
            {
                new ProcessadorDeGerarLoteDeContratos().Processar(mensagem);
            }
            else if (comando == "GerarMovimento")
            {
                new ProcessadorDeGerarMovimento().Processar(mensagem);
            }
            else if (comando == "GerarMovimentoParaLoteDeContratos")
            {
                new ProcessadorDeGerarMovimentoParaLoteDeContratos().Processar(mensagem);
            }
            else if (comando == "Faturar")
            {
                new ProcessadorDeFaturar().Processar(mensagem);
            }
            else if (comando == "FaturarLoteDeContratos")
            {
                new ProcessadorDeFaturarLoteDeContratos().Processar(mensagem);
            }
            else
            {
                Trace.TraceError(String.Format("O comando '{0}' não foi reconhecido.", comando));
            }
        }
    }
}