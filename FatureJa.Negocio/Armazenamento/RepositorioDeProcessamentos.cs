using FatureJa.Negocio.Entidades;

namespace FatureJa.Negocio.Armazenamento
{
    public class RepositorioDeProcessamentos : RepositorioCloudTable<Processamento>
    {
        public RepositorioDeProcessamentos()
        {
            Nome = "Processamentos";
        }
    }
}