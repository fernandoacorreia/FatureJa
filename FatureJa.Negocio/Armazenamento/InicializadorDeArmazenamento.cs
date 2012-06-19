namespace FatureJa.Negocio.Armazenamento
{
    public static class InicializadorDeArmazenamento
    {
        public static void Inicializar()
        {
            FilaDeMensagens.Inicializar();
            TabelaDeContratos.Inicializar();
            TabelaDeItensDeContratos.Inicializar();
            TabelaDeMovimento.Inicializar();
            TabelaDeFaturas.Inicializar();
            TabelaDeItensDeFaturas.Inicializar();
            new RepositorioDeProcessamentos().Inicializar();
        }
    }
}