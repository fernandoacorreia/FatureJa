using Microsoft.WindowsAzure.StorageClient;

namespace FatureJa.Negocio.Entidades
{
    public class ItemDeContrato : TableServiceEntity
    {
        public string Produto { get; set; }

        public double Valor { get; set; }
    }
}