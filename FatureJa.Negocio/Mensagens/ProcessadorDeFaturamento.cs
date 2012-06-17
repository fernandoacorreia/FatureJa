using System;
using System.Diagnostics;
using FatureJa.Negocio.Armazenamento;
using FatureJa.Negocio.Entidades;
using Microsoft.WindowsAzure.StorageClient;
using Newtonsoft.Json;

namespace FatureJa.Negocio.Mensagens
{
    public class ProcessadorDeFaturamento
    {
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

            int primeiroContrato = 1;
            int ultimoContrato = TabelaDeContratos.ObterNumeroDoUltimoContrato();
            if (ultimoContrato == 0)
            {
                Trace.WriteLine("Não há nenhum contrato.", "Error");
                return;
            }

            Faturar(ano, mes, primeiroContrato, ultimoContrato);
        }

        private void Faturar(int ano, int mes, int primeiroContrato, int ultimoContrato)
        {
            Trace.WriteLine(
                String.Format("Gerando faturamento para {0}/{1} do contrato {2} até {3}.", mes, ano, primeiroContrato,
                              ultimoContrato), "Information");

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
                SolicitarFaturamentoParaGrupo(ano, mes, inicio, fim, grupo);
                grupo += 1;
                inicio = fim + 1;
            }
        }

        private void SolicitarFaturamentoParaGrupo(int ano, int mes, int inicio, int fim, int grupo)
        {
            Trace.WriteLine(
                String.Format("Solicitando faturamento para {0}/{1} dos contratos de {2} a {3} no grupo {4}.",
                              mes, ano, inicio, fim, grupo), "Information");
            dynamic mensagem = new
                                   {
                                       Comando = "FaturarGrupoDeContratos",
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