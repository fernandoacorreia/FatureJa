using System;
using System.Data.Services.Client;
using System.Diagnostics;
using FatureJa.Negocio.Armazenamento;
using FatureJa.Negocio.Entidades;
using FatureJa.Negocio.Util;
using Microsoft.WindowsAzure.StorageClient;

namespace FatureJa.Negocio.Mensagens
{
    public class ProcessadorDeGerarGrupoDeContratos
    {
        public void GerarGrupoDeContratos(dynamic mensagem)
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
                throw new ArgumentException("O grupo deve ser no mínimo 0.", "mensagem");
            }

            Trace.WriteLine(String.Format("Gerando contratos de {0} a {1} no grupo {2}.", inicio, fim, grupo),
                            "Information");

            CloudTableClient tableClient = TabelaDeContratosFactory.GetCloudTableClient();
            TableServiceContext serviceContext = tableClient.GetDataServiceContext();

            int quantidadeNoLote = 0;
            for (int atual = inicio; atual <= fim; atual++)
            {
                var contrato = NovoContrato(atual);
                serviceContext.AddObject("contratos", contrato);
                quantidadeNoLote += 1;
                if (quantidadeNoLote == 100)
                {
                    serviceContext.SaveChangesWithRetries(SaveChangesOptions.Batch);
                    serviceContext = tableClient.GetDataServiceContext();
                    quantidadeNoLote = 0;
                }
            }
            serviceContext.SaveChangesWithRetries(SaveChangesOptions.Batch);
        }

        private static Contrato NovoContrato(int atual)
        {
            string municipio;
            string uf;
            GeradorDeMunicipios.GerarMunicipioEUf(out municipio, out uf);

            var contrato = new Contrato
                               {
                                   PartitionKey = Contrato.ObterPartitionKey(atual),
                                   RowKey = Contrato.ObterRowKey(atual),
                                   Numero = atual,
                                   RazaoSocialDoCliente = GeradorDeNomesDeEmpresas.GerarNome(),
                                   CnpjDoCliente = GeradorDeCnpjs.GerarCnpj(),
                                   MunicipioDoCliente = municipio,
                                   UfDoCliente = uf
                               };
            return contrato;
        }
    }
}