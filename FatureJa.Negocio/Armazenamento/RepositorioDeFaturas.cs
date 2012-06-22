using FatureJa.Negocio.Entidades;

namespace FatureJa.Negocio.Armazenamento
{
    public class RepositorioDeFaturas : RepositorioCloudTable<Fatura>
    {
        public RepositorioDeFaturas()
        {
            Nome = "Faturas";
        }
    }
}