using FatureJa.Negocio.Entidades;

namespace FatureJa.Negocio.Armazenamento
{
    internal class RepositorioDeEventosDeProcessamento : RepositorioCloudTable<EventoDeProcessamento>
    {
        public RepositorioDeEventosDeProcessamento()
        {
            Nome = "EventosDeProcessamento";
        }
    }
}