using System.Web.Mvc;

namespace FatureJa.Web.Controllers
{
    public class FaturamentoController : Controller
    {
        //
        // GET: /Faturamento/

        [Authorize(Roles = "Administrador")]
        public ActionResult Index()
        {
            return View();
        }
    }
}