namespace FatureJa.Negocio.Armazenamento
{
    public static class InicializadorDeArmazenamento
    {
        public static void Inicializar()
        {
            FilaDeMensagens.Inicializar();
            new RepositorioDeContratos().Inicializar();
            new RepositorioDeEventosDeProcessamento().Inicializar();
            new RepositorioDeFaturas().Inicializar();
            new RepositorioDeItensDeContrato().Inicializar();
            new RepositorioDeItensDeFatura().Inicializar();
            new RepositorioDeMovimento().Inicializar();
            new RepositorioDeProcessamentos().Inicializar();
        }
    }
}