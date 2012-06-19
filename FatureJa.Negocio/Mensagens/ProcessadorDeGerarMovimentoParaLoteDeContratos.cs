using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Diagnostics;
using FatureJa.Negocio.Armazenamento;
using FatureJa.Negocio.Entidades;
using FatureJa.Negocio.Util;
using Microsoft.WindowsAzure.StorageClient;

namespace FatureJa.Negocio.Mensagens
{
    public class ProcessadorDeGerarMovimentoParaLoteDeContratos
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

            int inicio = mensagem.Inicio;
            if (inicio < 1)
            {
                throw new ArgumentException("O início deve ser no mínimo 1.", "mensagem");
            }

            int fim = mensagem.Fim;
            if (fim < inicio)
            {
                throw new ArgumentException("O fim deve ser maior ou igual ao início.", "mensagem");
            }
            if (fim > Contrato.NumeroMaximoDeContrato)
            {
                throw new ArgumentException(
                    String.Format("O fim deve ser menor ou igual a {0}.", Contrato.NumeroMaximoDeContrato),
                    "mensagem");
            }

            Guid processamentoId = mensagem.ProcessamentoId;
            if (processamentoId == Guid.Empty)
            {
                throw new ArgumentException("O identificador do processamento não foi encontrado.", "mensagem");
            }

            DateTime dataHoraInicio = DateTime.UtcNow;

            GerarMovimentoParaLoteDeContratos(ano, mes, inicio, fim);

            RegistrarEvento(processamentoId, inicio, fim, dataHoraInicio);
        }

        private static void RegistrarEvento(Guid processamentoId, int inicio, int fim, DateTime dataHoraInicio)
        {
            var repositorio = new RepositorioDeEventosDeProcessamento();
            repositorio.Incluir(new EventoDeProcessamento
            {
                PartitionKey = EventoDeProcessamento.ObterPartitionKey(processamentoId),
                RowKey = EventoDeProcessamento.ObterRowKey(dataHoraInicio),
                Comando = "GerarMovimentoParaLoteDeContratos",
                Inicio = dataHoraInicio,
                Termino = DateTime.UtcNow,
                Operacoes = fim - inicio + 1
            });
        }

        private static void GerarMovimentoParaLoteDeContratos(int ano, int mes, int inicio, int fim)
        {
            Trace.WriteLine(
                String.Format("Gerando movimento para {0}/{1} dos contratos de {2} a {3}.", mes, ano,
                              inicio, fim), "Information");

            CloudTableClient clienteMovimento = TabelaDeMovimento.GetCloudTableClient();
            for (int contratoAtual = inicio; contratoAtual <= fim; contratoAtual++)
            {
                GerarMovimentoParaContrato(ano, mes, contratoAtual, clienteMovimento);
            }
        }

        private static void GerarMovimentoParaContrato(int ano, int mes, int contrato, CloudTableClient clienteMovimento)
        {
            TableServiceContext contextoMovimento = clienteMovimento.GetDataServiceContext();
            foreach (Movimento item in NovosItensDeMovimento(ano, mes, contrato))
            {
                contextoMovimento.AddObject(TabelaDeMovimento.Nome, item);
            }
            contextoMovimento.SaveChangesWithRetries(SaveChangesOptions.Batch);
        }

        private static IEnumerable<Movimento> NovosItensDeMovimento(int ano, int mes, int contrato)
        {
            var random = new Random();
            var lista = new List<Movimento>();
            int quantidadeItens = random.Next(1, 5 + 1);
            for (int atual = 1; atual <= quantidadeItens; atual++)
            {
                double valorUnitario = random.Next(50, 200);
                double quantidade = random.Next(1, 100);
                double valorTotal = valorUnitario*quantidade;
                var item = new Movimento
                               {
                                   PartitionKey = Movimento.ObterPartitionKey(ano, mes),
                                   RowKey = Movimento.ObterRowKey(contrato, atual),
                                   Produto = GeradorDeNomesDeProdutos.GerarNomeMovimento(),
                                   ValorUnitario = valorUnitario,
                                   Quantidade = quantidade,
                                   ValorTotal = valorTotal
                               };
                lista.Add(item);
            }
            return lista;
        }
    }
}