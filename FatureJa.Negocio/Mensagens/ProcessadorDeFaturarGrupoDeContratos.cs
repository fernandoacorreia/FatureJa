using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Diagnostics;
using System.Linq;
using FatureJa.Negocio.Armazenamento;
using FatureJa.Negocio.Entidades;
using Microsoft.WindowsAzure.StorageClient;

namespace FatureJa.Negocio.Mensagens
{
    public class ProcessadorDeFaturarGrupoDeContratos
    {
        private readonly CloudTableClient _clienteContratos = TabelaDeContratos.GetCloudTableClient();
        private readonly CloudTableClient _clienteFaturas = TabelaDeFaturas.GetCloudTableClient();
        private readonly CloudTableClient _clienteItensDeContratos = TabelaDeItensDeContratos.GetCloudTableClient();
        private readonly CloudTableClient _clienteItensDeFaturas = TabelaDeItensDeFaturas.GetCloudTableClient();
        private readonly CloudTableClient _clienteMovimento = TabelaDeMovimento.GetCloudTableClient();

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

            int grupo = mensagem.Grupo;
            if (grupo < 0)
            {
                throw new ArgumentException("O grupo deve ser maior ou igual a 0.", "mensagem");
            }
            if (grupo != Contrato.ObterGrupo(inicio) || grupo != Contrato.ObterGrupo(fim))
            {
                throw new ArgumentException("Os contratos devem ser de um mesmo grupo.", "mensagem");
            }

            FaturarGrupoDeContratos(ano, mes, inicio, fim, grupo);
        }

        private void FaturarGrupoDeContratos(int ano, int mes, int inicio, int fim, int grupo)
        {
            Trace.WriteLine(
                String.Format("Gerando faturamento para {0}/{1} do contrato {2} até {3} do grupo {4}.", mes, ano, inicio,
                              fim, grupo), "Information");

            for (int atual = inicio; atual <= fim; atual++)
            {
                FaturarContrato(ano, mes, atual);
            }
        }

        private void FaturarContrato(int ano, int mes, int atual)
        {
            Contrato contrato = ObterContrato(atual);
            if (contrato == null)
            {
                Trace.WriteLine(String.Format("O contrato {0} não foi encontrado.", atual), "Error");
                return;
            }

            IEnumerable<ItemDeContrato> itensDoContrato = ObterItensDoContrato(atual);
            IEnumerable<Movimento> movimentoDoContrato = ObterMovimentoDoContrato(ano, mes, atual);
            GerarFaturamento(ano, mes, contrato, itensDoContrato, movimentoDoContrato);
        }

        private Contrato ObterContrato(int atual)
        {
            TableServiceContext serviceContext = _clienteContratos.GetDataServiceContext();
            Contrato contrato =
                (from e in serviceContext.CreateQuery<Contrato>(TabelaDeContratos.Nome)
                 where e.PartitionKey == Contrato.ObterPartitionKey(atual) && e.RowKey == Contrato.ObterRowKey(atual)
                 select e).FirstOrDefault();
            return contrato;
        }

        private IEnumerable<ItemDeContrato> ObterItensDoContrato(int atual)
        {
            TableServiceContext serviceContext = _clienteItensDeContratos.GetDataServiceContext();

            string partitionKey = Contrato.ObterPartitionKey(atual);
            string rowKeyInicial = ItemDeContrato.ObterRowKey(atual, 0);
            string rowKeyFinal = ItemDeContrato.ObterRowKey(atual, int.MaxValue);

            CloudTableQuery<ItemDeContrato> query =
                (from e in serviceContext.CreateQuery<ItemDeContrato>(TabelaDeItensDeContratos.Nome)
                 where
                     e.PartitionKey == partitionKey &&
                     e.RowKey.CompareTo(rowKeyInicial) >= 0 &&
                     e.RowKey.CompareTo(rowKeyFinal) <= 0
                 select e).AsTableServiceQuery<ItemDeContrato>();

            return query.ToList();
        }

        private IEnumerable<Movimento> ObterMovimentoDoContrato(int ano, int mes, int atual)
        {
            TableServiceContext serviceContext = _clienteMovimento.GetDataServiceContext();

            string partitionKey = Movimento.ObterPartitionKey(ano, mes);
            string rowKeyInicial = Movimento.ObterRowKey(atual, 0);
            string rowKeyFinal = Movimento.ObterRowKey(atual, int.MaxValue);

            CloudTableQuery<Movimento> query =
                (from e in serviceContext.CreateQuery<Movimento>(TabelaDeMovimento.Nome)
                 where
                     e.PartitionKey == partitionKey &&
                     e.RowKey.CompareTo(rowKeyInicial) >= 0 &&
                     e.RowKey.CompareTo(rowKeyFinal) <= 0
                 select e).AsTableServiceQuery<Movimento>();

            return query.ToList();
        }

        private void GerarFaturamento(int ano, int mes, Contrato contrato, IEnumerable<ItemDeContrato> itensDoContrato,
                                      IEnumerable<Movimento> movimentoDoContrato)
        {
            int TODO_SERIE = 1;
            int TODO_NUMERO_FATURA = contrato.Numero;
            int numeroItemDeFatura = 0;
            double valorTotal = 0;
            TableServiceContext contextoItensDeFaturas = _clienteItensDeFaturas.GetDataServiceContext();
            TableServiceContext contextoDeFaturas = _clienteFaturas.GetDataServiceContext();

            // incluir itens de contrato
            foreach (ItemDeContrato itemDeContrato in itensDoContrato)
            {
                numeroItemDeFatura++;
                var itemDeFatura = new ItemDeFatura
                                       {
                                           PartitionKey = ItemDeFatura.ObterPartitionKey(ano, mes),
                                           RowKey =
                                               ItemDeFatura.ObterRowKey(TODO_SERIE, TODO_NUMERO_FATURA,
                                                                        numeroItemDeFatura),
                                           Produto = itemDeContrato.Produto,
                                           Quantidade = 1,
                                           ValorUnitario = itemDeContrato.Valor,
                                           ValorTotal = itemDeContrato.Valor
                                       };
                valorTotal += itemDeFatura.ValorTotal;
                contextoItensDeFaturas.AddObject(TabelaDeItensDeFaturas.Nome, itemDeFatura);
            }

            // incluir itens de movimento
            foreach (Movimento movimento in movimentoDoContrato)
            {
                numeroItemDeFatura++;
                var itemDeFatura = new ItemDeFatura
                                       {
                                           PartitionKey = ItemDeFatura.ObterPartitionKey(ano, mes),
                                           RowKey =
                                               ItemDeFatura.ObterRowKey(TODO_SERIE, TODO_NUMERO_FATURA,
                                                                        numeroItemDeFatura),
                                           Produto = movimento.Produto,
                                           Quantidade = movimento.Quantidade,
                                           ValorUnitario = movimento.ValorUnitario,
                                           ValorTotal = movimento.ValorTotal
                                       };
                valorTotal += itemDeFatura.ValorTotal;
                contextoItensDeFaturas.AddObject(TabelaDeItensDeFaturas.Nome, itemDeFatura);
            }

            // incluir registro mestre da fatura
            var fatura = new Fatura
                             {
                                 PartitionKey = Fatura.ObterPartitionKey(ano, mes),
                                 RowKey = Fatura.ObterRowKey(TODO_SERIE, TODO_NUMERO_FATURA),
                                 DataDeEmissao = DateTime.UtcNow,
                                 NumeroDoContrato = contrato.Numero,
                                 CnpjDoCliente = contrato.CnpjDoCliente,
                                 RazaoSocialDoCliente = contrato.RazaoSocialDoCliente,
                                 MunicipioDoCliente = contrato.MunicipioDoCliente,
                                 UfDoCliente = contrato.UfDoCliente,
                                 ValorTotal = valorTotal
                             };
            contextoDeFaturas.AddObject(TabelaDeFaturas.Nome, fatura);

            // salvar
            contextoItensDeFaturas.SaveChangesWithRetries(SaveChangesOptions.Batch);
            contextoDeFaturas.SaveChangesWithRetries(SaveChangesOptions.Batch);
        }
    }
}