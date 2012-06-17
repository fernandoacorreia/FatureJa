using System;

namespace FatureJa.Negocio.Util
{
    public static class GeradorDeNomesDeProdutos
    {
        #region Produtos Contratados

        private static readonly string[] _nomeProdutosContratados = new[]
                                                                        {
                                                                            "Licença de Uso do Sistema Contábil",
                                                                            "Manutenção do Sistema Contábil",
                                                                            "Licença de Uso do Sistema Financeiro",
                                                                            "Manutenção do Sistema Financeiro",
                                                                            "Licença de Uso do Sistema de RH",
                                                                            "Manutenção do Sistema de de RH",
                                                                            "Licença de Uso do Sistema de Laboratórios",
                                                                            "Manutenção do Sistema de Laboratórios",
                                                                            "Licença de Uso do Sistema Educacional",
                                                                            "Manutenção do Sistema Educacional",
                                                                            "Licença de Uso do Sistema Comercial",
                                                                            "Manutenção do Sistema Comercial",
                                                                            "Licença de Uso do Sistema de Produção",
                                                                            "Manutenção do Sistema de Produção",
                                                                            "Licença de Uso do Sistema de Atendimento",
                                                                            "Manutenção do Sistema de Atendimento",
                                                                            "Licença de Uso do Sistema Jurídico",
                                                                            "Manutenção do Sistema Jurídico",
                                                                            "Suporte prioritário",
                                                                            "Suporte 24x7"
                                                                        };

        #endregion

        #region Produtos de Movimento

        private static readonly string[] _nomeProdutosMovimento = new[]
                                                                      {
                                                                          "Horas de consultoria",
                                                                          "Horas de desenvolvimento",
                                                                          "Horas de treinamento",
                                                                          "Horas de implantação",
                                                                          "Evento de suporte",
                                                                          "Licença de uso complementar"
                                                                      };

        #endregion

        private static readonly Random _random = new Random();

        public static string GerarNomeContratado()
        {
            string nome = _nomeProdutosContratados[_random.Next(0, _nomeProdutosContratados.Length)];
            return nome;
        }

        internal static string GerarNomeMovimento()
        {
            string nome = _nomeProdutosMovimento[_random.Next(0, _nomeProdutosMovimento.Length)];
            return nome;
        }
    }
}