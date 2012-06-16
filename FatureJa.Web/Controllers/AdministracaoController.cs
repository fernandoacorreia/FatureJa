using System.Web.Mvc;
using FatureJa.Negocio.Administracao;
using FatureJa.Web.Models;

namespace FatureJa.Web.Controllers
{
    public class AdministracaoController : Controller
    {
        //
        // GET: /Administracao/

        [Authorize(Roles = "Administrador")]
        public ActionResult Index()
        {
            return View();
        }

        //
        // POST: /Administracao/

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public ActionResult Index(GeracaoModel model)
        {
            try
            {
                var gerador = new GeradorDeContratos();
                gerador.SolicitarGeracao(model.QuantidadeContratos);
                return RedirectToAction("GeracaoSolicitada");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Administracao/GeracaoSolicitada/

        [Authorize(Roles = "Administrador")]
        public ActionResult GeracaoSolicitada()
        {
            return View();
        }
    }
}