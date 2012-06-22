using FatureJa.Negocio.Entidades;

namespace FatureJa.Negocio.Armazenamento
{
    public class RepositorioDeItensDeFatura : RepositorioCloudTable<ItemDeFatura>
    {
        public RepositorioDeItensDeFatura()
        {
            Nome = "ItensDeFatura";
        }
    }
}