using System;
using System.Collections.Generic;
using System.Diagnostics;
using FatureJa.Negocio.Armazenamento;
using FatureJa.Negocio.Entidades;

namespace FatureJa.Negocio.Mensagens
{
    public class ProcessadorDeFaturarLoteDeContratos
    {
        private readonly RepositorioDeContratos _repositorioDeContratos = new RepositorioDeContratos();
        private readonly RepositorioDeItensDeContrato _repositorioDeItensDeContrato = new RepositorioDeItensDeContrato();
        private readonly RepositorioDeMovimento _repositorioDeMovimento = new RepositorioDeMovimento();

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

            Guid processamentoId = mensagem.ProcessamentoId;
            if (processamentoId == Guid.Empty)
            {
                throw new ArgumentException("O identificador do processamento não foi encontrado.", "mensagem");
            }

            DateTime dataHoraInicio = DateTime.UtcNow;

            FaturarLoteDeContratos(ano, mes, inicio, fim, grupo);

            RegistrarEvento(processamentoId, inicio, fim, dataHoraInicio);
        }

        private static void RegistrarEvento(Guid processamentoId, int inicio, int fim, DateTime dataHoraInicio)
        {
            var repositorio = new RepositorioDeEventosDeProcessamento();
            repositorio.Incluir(new EventoDeProcessamento
                                    {
                                        PartitionKey = EventoDeProcessamento.ObterPartitionKey(processamentoId),
                                        RowKey = EventoDeProcessamento.ObterRowKey(dataHoraInicio, Guid.NewGuid()),
                                        Comando = "FaturarLoteDeContratos",
                                        Inicio = dataHoraInicio,
                                        Termino = DateTime.UtcNow,
                                        Duracao = (DateTime.UtcNow - dataHoraInicio).TotalSeconds,
                                        Quantidade = fim - inicio + 1
                                    });
        }

        private void FaturarLoteDeContratos(int ano, int mes, int inicio, int fim, int grupo)
        {
            Trace.TraceInformation(
                String.Format("Gerando faturamento para {0}/{1} do contrato {2} até {3} do grupo {4}.", mes, ano, inicio,
                              fim, grupo));

            for (int atual = inicio; atual <= fim; atual++)
            {
                FaturarContrato(ano, mes, atual);
            }
        }

        private void FaturarContrato(int ano, int mes, int atual)
        {
            Contrato contrato = _repositorioDeContratos.ObterContrato(atual);
            if (contrato == null)
            {
                Trace.TraceError(String.Format("O contrato {0} não foi encontrado.", atual));
                return;
            }

            IEnumerable<ItemDeContrato> itensDoContrato = _repositorioDeItensDeContrato.ObterItensDoContrato(atual);
            IEnumerable<Movimento> movimentoDoContrato = _repositorioDeMovimento.ObterMovimentoDoContrato(ano, mes,
                                                                                                          atual);
            GerarFaturamento(ano, mes, contrato, itensDoContrato, movimentoDoContrato);
        }

        private void GerarFaturamento(int ano, int mes, Contrato contrato, IEnumerable<ItemDeContrato> itensDoContrato,
                                      IEnumerable<Movimento> movimentoDoContrato)
        {
            int TODO_SERIE = 1;
            int TODO_NUMERO_FATURA = contrato.Numero;
            int numeroItemDeFatura = 0;
            double valorTotal = 0;
            var repositorioDeFaturas = new RepositorioDeFaturas();
            var repositorioDeItensDeFatura = new RepositorioDeItensDeFatura();


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
                repositorioDeItensDeFatura.AdicionarObjeto(itemDeFatura);
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
                repositorioDeItensDeFatura.AdicionarObjeto(itemDeFatura);
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
            repositorioDeFaturas.AdicionarObjeto(fatura);

            // salvar
            repositorioDeItensDeFatura.SalvarLote();
            repositorioDeFaturas.SalvarLote();
        }
    }
}