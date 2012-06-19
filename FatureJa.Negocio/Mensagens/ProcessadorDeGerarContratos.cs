using System;
using System.Diagnostics;
using FatureJa.Negocio.Armazenamento;
using FatureJa.Negocio.Entidades;
using FatureJa.Negocio.Servicos;
using Microsoft.WindowsAzure.StorageClient;
using Newtonsoft.Json;

namespace FatureJa.Negocio.Mensagens
{
    public class ProcessadorDeGerarContratos
    {
        private const int _quantidadeMaximaPorLote = 100;

        public void Processar(dynamic mensagem)
        {
            int primeiro = mensagem.Primeiro;
            if (primeiro < 1)
            {
                throw new ArgumentException("O número do primeiro contrato deve ser no mínimo 1.", "mensagem");
            }

            int ultimo = mensagem.Ultimo;
            if (ultimo < primeiro)
            {
                throw new ArgumentException("O número do último contrato deve ser maior ou igual ao primeiro.",
                                            "mensagem");
            }
            if (ultimo > Contrato.NumeroMaximoDeContrato)
            {
                throw new ArgumentException(
                    String.Format("O número do último contrato deve ser menor do que {0}.",
                                  Contrato.NumeroMaximoDeContrato),
                    "mensagem");
            }

            int quantidade = ultimo - primeiro + 1;
            if (quantidade > _quantidadeMaximaPorLote)
            {
                Trace.WriteLine(
                    String.Format("Subdividindo solicitação de geração dos contratos {0} a {1}.", primeiro, ultimo),
                    "Information");
                int meio = (ultimo - primeiro)/2 + primeiro;
                var gerador = new GeradorDeContratos();
                gerador.SolicitarGeracao(primeiro, meio);
                gerador.SolicitarGeracao(meio + 1, ultimo);
            }
            else
            {
                SolicitarGeracaoDeLote(primeiro, ultimo);
            }
        }

        private void SolicitarGeracaoDeLote(int primeiro, int ultimo)
        {
            Trace.WriteLine(string.Format("Gerando lote de contratos {0} a {1}.", primeiro, ultimo), "Information");

            int grupoDoPrimeiroContrato = Contrato.ObterGrupo(primeiro);
            int grupoDoUltimoContrato = Contrato.ObterGrupo(ultimo);

            int inicio = primeiro;
            int grupo = grupoDoPrimeiroContrato;

            while (grupo <= grupoDoUltimoContrato) // assegura que cada lote seja de um único grupo
            {
                int fim = inicio + 1000;
                int maximoGrupo = (grupo + 1)*1000;
                if (fim > maximoGrupo)
                {
                    fim = maximoGrupo;
                }
                if (fim > ultimo)
                {
                    fim = ultimo;
                }
                SolicitarGeracaoDeLote(inicio, fim, grupo);
                grupo += 1;
                inicio = fim + 1;
            }
        }

        private void SolicitarGeracaoDeLote(int inicio, int fim, int grupo)
        {
            Trace.WriteLine(
                String.Format("Solicitando geração de contratos de {0} a {1} no grupo {2}.", inicio, fim, grupo),
                "Information");
            dynamic mensagem = new
                                   {
                                       Comando = "GerarLoteDeContratos",
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