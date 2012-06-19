namespace FatureJa.Negocio.Armazenamento
{
    public static class InicializadorDeArmazenamento
    {
        public static void Inicializar()
        {
            FilaDeMensagens.Inicializar();
            TabelaDeContratos.Inicializar();
            TabelaDeItensDeContrato.Inicializar();
            TabelaDeMovimento.Inicializar();
            TabelaDeFaturas.Inicializar();
            TabelaDeItensDeFatura.Inicializar();
            new RepositorioDeProcessamentos().Inicializar();
            new RepositorioDeEventosDeProcessamento().Inicializar();
        }
    }
}