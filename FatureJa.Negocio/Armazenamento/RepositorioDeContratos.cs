using System;
using System.Linq;
using FatureJa.Negocio.Entidades;

namespace FatureJa.Negocio.Armazenamento
{
    public class RepositorioDeContratos : RepositorioCloudTable<Contrato>
    {
        public RepositorioDeContratos()
        {
            Nome = "Contratos";
        }

        public Contrato ObterContrato(int atual)
        {
            try
            {
                Contrato contrato =
                    (from e in ServiceContext.CreateQuery<Contrato>(Nome)
                     where
                         e.PartitionKey == Contrato.ObterPartitionKey(atual) && e.RowKey == Contrato.ObterRowKey(atual)
                     select e).FirstOrDefault();
                return contrato;
            }
            catch (Exception ex)
            {
                throw new RepositorioCloudTableException(
                    String.Format("Erro obtendo contrato '{0}'.", atual), ex);
            }
        }

        public int ObterNumeroDoUltimoContrato()
        {
            try
            {
                Contrato contrato =
                    (from e in ServiceContext.CreateQuery<Contrato>(Nome)
                     select e).FirstOrDefault();
                if (contrato == null)
                {
                    return 0;
                }
                return contrato.Numero;
            }
            catch (Exception ex)
            {
                throw new RepositorioCloudTableException(
                    String.Format("Erro obtendo número do último contrato."), ex);
            }
        }
    }
}