using System;

namespace FatureJa.Negocio.Util
{
    public static class GeradorDeCnpjs
    {
        private static readonly Random _random = new Random();

        public static string GerarCnpj()
        {
            int raiz = _random.Next(1, 99999999);
            int digito = _random.Next(0, 99);
            string raizFormatada = raiz.ToString().PadLeft(8, '0');
            string digitoFormatado = digito.ToString().PadLeft(2, '0');
            string cnpj = raizFormatada + "0001" + digitoFormatado;
            return cnpj;
        }
    }
}