using System;
using System.Collections.Generic;
using System.Diagnostics;
using FatureJa.Negocio.Armazenamento;
using FatureJa.Negocio.Entidades;
using FatureJa.Negocio.Util;

namespace FatureJa.Negocio.Mensagens
{
    public class ProcessadorDeGerarLoteDeContratos
    {
        public void Processar(dynamic mensagem)
        {
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

            Guid processamentoId = mensagem.ProcessamentoId;
            if (processamentoId == Guid.Empty)
            {
                throw new ArgumentException("O identificador do processamento não foi encontrado.", "mensagem");
            }

            DateTime dataHoraInicio = DateTime.UtcNow;

            GerarLoteDeContratos(inicio, fim, grupo);

            RegistrarEvento(processamentoId, inicio, fim, dataHoraInicio);
        }

        private static void RegistrarEvento(Guid processamentoId, int inicio, int fim, DateTime dataHoraInicio)
        {
            var repositorio = new RepositorioDeEventosDeProcessamento();
            repositorio.Incluir(new EventoDeProcessamento
                                    {
                                        PartitionKey = EventoDeProcessamento.ObterPartitionKey(processamentoId),
                                        RowKey = EventoDeProcessamento.ObterRowKey(dataHoraInicio, Guid.NewGuid()),
                                        Comando = "GerarLoteDeContratos",
                                        Inicio = dataHoraInicio,
                                        Termino = DateTime.UtcNow,
                                        Duracao = (DateTime.UtcNow - dataHoraInicio).TotalSeconds,
                                        Quantidade = fim - inicio + 1
                                    });
        }

        private static void GerarLoteDeContratos(int inicio, int fim, int grupo)
        {
            Trace.TraceInformation(String.Format("Gerando contratos de {0} a {1} no grupo {2}.", inicio, fim, grupo));

            var repositorioDeContratos = new RepositorioDeContratos();

            int quantidadeNoLote = 0;
            for (int atual = inicio; atual <= fim; atual++)
            {
                // incluir contratos
                Contrato contrato = NovoContrato(atual);
                repositorioDeContratos.AdicionarObjeto(contrato);
                quantidadeNoLote += 1;
                if (quantidadeNoLote == 100 || atual == fim)
                {
                    repositorioDeContratos.SalvarLote();
                    repositorioDeContratos.CriarNovoContexto();
                    quantidadeNoLote = 0;
                }

                // incluir itens de contrato
                var repositorioDeItensDeContrato = new RepositorioDeItensDeContrato();
                foreach (ItemDeContrato item in NovosItensDeContrato(atual))
                {
                    repositorioDeItensDeContrato.AdicionarObjeto(item);
                }
                repositorioDeItensDeContrato.SalvarLote();
            }
        }

        private static Contrato NovoContrato(int numero)
        {
            string municipio;
            string uf;
            GeradorDeMunicipios.GerarMunicipioEUf(out municipio, out uf);

            var contrato = new Contrato
                               {
                                   PartitionKey = Contrato.ObterPartitionKey(numero),
                                   RowKey = Contrato.ObterRowKey(numero),
                                   Numero = numero,
                                   RazaoSocialDoCliente = GeradorDeNomesDeEmpresas.GerarNome(),
                                   CnpjDoCliente = GeradorDeCnpjs.GerarCnpj(),
                                   MunicipioDoCliente = municipio,
                                   UfDoCliente = uf
                               };
            return contrato;
        }

        private static IEnumerable<ItemDeContrato> NovosItensDeContrato(int numero)
        {
            var random = new Random();
            var lista = new List<ItemDeContrato>();
            int quantidade = random.Next(1, 10 + 1);
            for (int atual = 1; atual <= quantidade; atual++)
            {
                var item = new ItemDeContrato
                               {
                                   PartitionKey = ItemDeContrato.ObterPartitionKey(numero),
                                   RowKey = ItemDeContrato.ObterRowKey(numero, atual),
                                   Produto = GeradorDeNomesDeProdutos.GerarNomeContratado(),
                                   Valor = random.Next(100, 10000)
                               };
                lista.Add(item);
            }
            return lista;
        }
    }
}