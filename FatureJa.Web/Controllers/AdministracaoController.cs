using System.Web.Mvc;

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
    }
}