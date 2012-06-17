using System.ComponentModel.DataAnnotations;

namespace FatureJa.Web.Models
{
    public class GeracaoDeContratosModel
    {
        [Required]
        [Display(Name = "Quantidade de contratos a gerar")]
        [Range(1,100000000)]
        public int QuantidadeContratos { get; set; }
    }

    public class GeracaoDeMovimentoModel
    {
        [Required]
        [Display(Name = "Ano")]
        [Range(2012, 2999)]
        public int Ano { get; set; }

        [Required]
        [Display(Name = "Mês")]
        [Range(1, 12)]
        public int Mes { get; set; }
    }
}