using System;
using System.Diagnostics;
using FatureJa.Negocio.Armazenamento;
using FatureJa.Negocio.Entidades;
using FatureJa.Negocio.Servicos;
using Microsoft.WindowsAzure.StorageClient;
using Newtonsoft.Json;

namespace FatureJa.Negocio.Mensagens
{
    public class ProcessadorDeGerarMovimento
    {
        private const int _quantidadeMaximaPorLote = 20;

        public void Processar(dynamic mensagem)
        {
            int ano = mensagem.Ano;
            if (ano < 2012 || ano > 2999)
            {
                throw new ArgumentException("O ano está fora da faixa suportada.", "mensagem");
            }

            int mes = mensagem.Mes;
            if (mes < 1 || mes > 12)
            {
                throw new ArgumentException("O mês está fora da faixa suportada.", "mensagem");
            }

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

            Guid processamentoId = mensagem.ProcessamentoId;
            if (processamentoId == Guid.Empty)
            {
                throw new ArgumentException("O identificador do processamento não foi encontrado.", "mensagem");
            }

            int quantidade = ultimo - primeiro + 1;
            if (quantidade > _quantidadeMaximaPorLote)
            {
                Trace.WriteLine(
                    String.Format("Subdividindo solicitação de geração de movimento para {0}/{1} para os contratos {2} a {3}.", mes, ano, primeiro, ultimo));
                int meio = (ultimo - primeiro) / 2 + primeiro;
                var gerador = new GeradorDeMovimento();
                gerador.SolicitarGeracao(processamentoId, ano, mes, primeiro, meio);
                gerador.SolicitarGeracao(processamentoId, ano, mes, meio + 1, ultimo);
            }
            else
            {
                GerarMovimento(processamentoId, ano, mes, primeiro, ultimo);
            }
        }

        private void GerarMovimento(Guid processamentoId, int ano, int mes, int primeiroContrato, int ultimoContrato)
        {
            Trace.WriteLine(
                String.Format("Solicitando geração de movimento para {0}/{1} do contrato {2} até {3}.", mes, ano, primeiroContrato,
                              ultimoContrato));

            int grupoDoPrimeiroContrato = Contrato.ObterGrupo(primeiroContrato);
            int grupoDoUltimoContrato = Contrato.ObterGrupo(ultimoContrato);

            int inicio = primeiroContrato;
            int grupo = grupoDoPrimeiroContrato;

            while (grupo <= grupoDoUltimoContrato)
            {
                int fim = inicio + 1000;
                int maximoGrupo = (grupo + 1)*1000;
                if (fim > maximoGrupo)
                {
                    fim = maximoGrupo;
                }
                if (fim > ultimoContrato)
                {
                    fim = ultimoContrato;
                }
                SolicitarGeracaoDeMovimentoParaGrupo(processamentoId, ano, mes, inicio, fim, grupo);
                grupo += 1;
                inicio = fim + 1;
            }
        }

        private void SolicitarGeracaoDeMovimentoParaGrupo(Guid processamentoId, int ano, int mes, int inicio, int fim, int grupo)
        {
            Trace.WriteLine(
                String.Format("Solicitando geração de movimento para {0}/{1} dos contratos de {2} a {3} no grupo {4}.",
                              mes, ano, inicio, fim, grupo));
            dynamic mensagem = new
                                   {
                                       Comando = "GerarMovimentoParaLoteDeContratos",
                                       ProcessamentoId = processamentoId,
                                       Ano = ano,
                                       Mes = mes,
                                       Inicio = inicio,
                                       Fim = fim
                                   };
            var message = new CloudQueueMessage(JsonConvert.SerializeObject(mensagem));
            CloudQueue cloudQueue = FilaDeMensagens.GetCloudQueue();
            cloudQueue.AddMessage(message);
        }
    }
}