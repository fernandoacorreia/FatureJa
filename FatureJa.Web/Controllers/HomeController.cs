using System.Web.Mvc;

namespace FatureJa.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Acelerando processamento com a nuvem.";

            return View();
        }
    }
}