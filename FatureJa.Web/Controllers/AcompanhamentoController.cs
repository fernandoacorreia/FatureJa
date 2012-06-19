using System;
using System.Web.Mvc;
using FatureJa.Negocio.Armazenamento;

namespace FatureJa.Web.Controllers
{
    public class AcompanhamentoController : Controller
    {
        //
        // GET: /Acompanhamento/

        public ActionResult Index()
        {
            var repositorio = new RepositorioDeProcessamentos();
            var processamentos = repositorio.ObterUltimosProcessamentos();
            ViewBag.Processamentos = processamentos;
            return View();
        }

        public ActionResult Eventos(Guid processamentoId)
        {
            return View();
        }
    }
}