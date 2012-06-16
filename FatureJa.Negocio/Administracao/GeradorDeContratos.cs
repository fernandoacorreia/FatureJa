using System;
using System.Diagnostics;
using FatureJa.Negocio.Infraestrutura;
using Microsoft.WindowsAzure.StorageClient;
using Newtonsoft.Json;

namespace FatureJa.Negocio.Administracao
{
    public class GeradorDeContratos
    {
        public void SolicitarGeracao(int quantidade)
        {
            dynamic mensagem = new
                                   {
                                       Comando = "GerarContratos",
                                       Quantidade = quantidade
                                   };
            var message = new CloudQueueMessage(JsonConvert.SerializeObject(mensagem));
            CloudQueue cloudQueue = CloudQueueFactory.Create();
            cloudQueue.AddMessage(message);
        }

        public void GerarContratos(dynamic mensagem)
        {
            int quantidade = mensagem.Quantidade;
            if (quantidade < 1)
            {
                throw new ArgumentException("A quantidade deve ser no mínimo 1.", "mensagem");
            }
            Trace.WriteLine(string.Format("Gerando {0} contratos.", quantidade), "Information");

            int numeroDoUltimoContrato = ObterNumeroDoUltimoContrato();
            int numeroDoPrimeiroContratoAGerar = numeroDoUltimoContrato + 1;
            int numeroDoUltimoContratoAGerar = numeroDoPrimeiroContratoAGerar + quantidade - 1;
            int grupoDoPrimeiroContratoAGerar = ObterGrupo(numeroDoPrimeiroContratoAGerar);
            int grupoDoUltimoContratoAGerar = ObterGrupo(numeroDoUltimoContratoAGerar);

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

            int grupo = mensagem.Grupo;
            if (grupo < 0)
            {
                throw new ArgumentException("O grupo deve ser no mínimo 0.", "mensagem");
            }

            Trace.WriteLine(String.Format("Gerando contratos de {0} a {1} no grupo {2}.", inicio, fim, grupo),
                            "Information");
        }

        private int ObterNumeroDoUltimoContrato()
        {
            return 0; // TODO
        }

        private int ObterGrupo(int numeroDoContrato)
        {
            int grupo = (numeroDoContrato - 1)/1000;
            return grupo;
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
            CloudQueue cloudQueue = CloudQueueFactory.Create();
            cloudQueue.AddMessage(message);
        }
    }
}