using System;
using System.Diagnostics;
using FatureJa.Negocio.Armazenamento;
using FatureJa.Negocio.Entidades;
using Microsoft.WindowsAzure.StorageClient;
using Newtonsoft.Json;

namespace FatureJa.Negocio.Mensagens
{
    public class ProcessadorDeGerarContratos
    {
        public void Processar(dynamic mensagem)
        {
            int quantidade = mensagem.Quantidade;
            if (quantidade < 1)
            {
                throw new ArgumentException("A quantidade deve ser no mínimo 1.", "mensagem");
            }
            if (quantidade > Contrato.NumeroMaximoDeContrato)
            {
                throw new ArgumentException(
                    String.Format("A quantidade deve ser menor do que {0}.", Contrato.NumeroMaximoDeContrato),
                    "mensagem");
            }

            GerarContratos(quantidade);
        }

        private void GerarContratos(int quantidade)
        {
            Trace.WriteLine(string.Format("Gerando {0} contratos.", quantidade), "Information");

            int numeroDoUltimoContrato = TabelaDeContratos.ObterNumeroDoUltimoContrato();
            int numeroDoPrimeiroContratoAGerar = numeroDoUltimoContrato + 1;
            int numeroDoUltimoContratoAGerar = numeroDoPrimeiroContratoAGerar + quantidade - 1;

            if (numeroDoUltimoContratoAGerar > Contrato.NumeroMaximoDeContrato)
            {
                throw new ArgumentException(
                    String.Format("O número do último contrato a gerar deve ser menor do que {0}.",
                                  Contrato.NumeroMaximoDeContrato), "mensagem");
            }

            int grupoDoPrimeiroContratoAGerar = Contrato.ObterGrupo(numeroDoPrimeiroContratoAGerar);
            int grupoDoUltimoContratoAGerar = Contrato.ObterGrupo(numeroDoUltimoContratoAGerar);

            int inicio = numeroDoPrimeiroContratoAGerar;
            int grupo = grupoDoPrimeiroContratoAGerar;

            while (grupo <= grupoDoUltimoContratoAGerar)
            {
                int fim = inicio + 1000;
                int maximoGrupo = (grupo + 1)*1000;
                if (fim > maximoGrupo)
                {
                    fim = maximoGrupo;
                }
                if (fim > numeroDoUltimoContratoAGerar)
                {
                    fim = numeroDoUltimoContratoAGerar;
                }
                SolicitarGeracaoDeGrupo(inicio, fim, grupo);
                grupo += 1;
                inicio = fim + 1;
            }
        }

        private void SolicitarGeracaoDeGrupo(int inicio, int fim, int grupo)
        {
            Trace.WriteLine(
                String.Format("Solicitando geração de contratos de {0} a {1} no grupo {2}.", inicio, fim, grupo),
                "Information");
            dynamic mensagem = new
                                   {
                                       Comando = "GerarGrupoDeContratos",
                                       Inicio = inicio,
                                       Fim = fim,
                                       Grupo = grupo
                                   };
            var message = new CloudQueueMessage(JsonConvert.SerializeObject(mensagem));
            CloudQueue cloudQueue = FilaDeMensagens.GetCloudQueue();
            cloudQueue.AddMessage(message);
        }
    }
}