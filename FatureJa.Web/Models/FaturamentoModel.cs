using System.ComponentModel.DataAnnotations;

namespace FatureJa.Web.Models
{
    public class SolicitacaoDeFaturamentoModel
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