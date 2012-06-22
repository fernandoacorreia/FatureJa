using System;
using System.Collections.Generic;
using System.Linq;
using FatureJa.Negocio.Entidades;
using Microsoft.WindowsAzure.StorageClient;

namespace FatureJa.Negocio.Armazenamento
{
    public class RepositorioDeProcessamentos : RepositorioCloudTable<Processamento>
    {
        public RepositorioDeProcessamentos()
        {
            Nome = "Processamentos";
        }

        public IEnumerable<Processamento> ObterUltimosProcessamentos(int take)
        {
            try
            {
                return
                    (from p in ServiceContext.CreateQuery<Processamento>(Nome)
                     select p).Take(take).AsTableServiceQuery().ToList();
            }
            catch (Exception ex)
            {
                throw new RepositorioCloudTableException(
                    String.Format("Erro obtendo os {0} últimos processamentos.", take), ex);
            }
        }

        public Processamento ObterPorProcessamentoId(Guid processamentoId)
        {
            try
            {
                return
                    (from p in ServiceContext.CreateQuery<Processamento>(Nome)
                     where p.ProcessamentoId == processamentoId
                     select p).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new RepositorioCloudTableException(
                    String.Format("Erro obtendo o processamento '{0}'.", processamentoId), ex);
            }
        }
    }
}