using System;

namespace FatureJa.Negocio.Armazenamento
{
    internal class RepositorioCloudTableException : ApplicationException
    {
        public RepositorioCloudTableException(string mensagem, Exception innerException)
            : base(mensagem, innerException)
        {
        }
    }
}