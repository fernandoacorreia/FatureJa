using System.ComponentModel.DataAnnotations;

namespace FatureJa.Web.Models
{
    public class GeracaoModel
    {
        [Required]
        [Display(Name = "Quantidade de contratos a gerar")]
        [Range(1,100000000)]
        public int QuantidadeContratos { get; set; }
    }
}