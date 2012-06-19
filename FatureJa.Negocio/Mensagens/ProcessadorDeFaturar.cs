using System;
using System.Diagnostics;
using FatureJa.Negocio.Armazenamento;
using FatureJa.Negocio.Entidades;
using FatureJa.Negocio.Servicos;
using Microsoft.WindowsAzure.StorageClient;
using Newtonsoft.Json;

namespace FatureJa.Negocio.Mensagens
{
    public class ProcessadorDeFaturar
    {
        private const int _quantidadeMaximaPorLote = 100;

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
                    String.Format("Subdividindo solicitação de faturamento para {0}/{1} dos contratos {2} a {3}.", mes, ano, primeiro, ultimo),
                    "Information");
                int meio = (ultimo - primeiro) / 2 + primeiro;
                var gerador = new FaturamentoDeContratos();
                gerador.SolicitarFaturamento(processamentoId, ano, mes, primeiro, meio);
                gerador.SolicitarFaturamento(processamentoId, ano, mes, meio + 1, ultimo);
            }
            else
            {
                Faturar(ano, mes, primeiro, ultimo);
            }
        }

        private void Faturar(int ano, int mes, int primeiroContrato, int ultimoContrato)
        {
            Trace.WriteLine(
                String.Format("Solicitando faturamento para {0}/{1} do contrato {2} até {3}.", mes, ano, primeiroContrato,
                              ultimoContrato), "Information");

            int grupoDoPrimeiroContrato = Contrato.ObterGrupo(primeiroContrato);
            int grupoDoUltimoContrato = Contrato.ObterGrupo(ultimoContrato);

            int inicio = primeiroContrato;
            int grupo = grupoDoPrimeiroContrato;

            while (grupo <= grupoDoUltimoContrato)  // assegura que um lote tenha contratos de um único grupo
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
                SolicitarFaturamentoParaLote(ano, mes, inicio, fim, grupo);
                grupo += 1;
                inicio = fim + 1;
            }
        }

        private void SolicitarFaturamentoParaLote(int ano, int mes, int inicio, int fim, int grupo)
        {
            Trace.WriteLine(
                String.Format("Solicitando faturamento para {0}/{1} dos contratos de {2} a {3} no grupo {4}.",
                              mes, ano, inicio, fim, grupo), "Information");
            dynamic mensagem = new
                                   {
                                       Comando = "FaturarLoteDeContratos",
                                       Ano = ano,
                                       Mes = mes,
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