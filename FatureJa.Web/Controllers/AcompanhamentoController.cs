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
            var processamentos = repositorio.ObterUltimosProcessamentos(20);
            ViewBag.Processamentos = processamentos;
            return View();
        }

        public ActionResult Eventos(Guid processamentoId)
        {
            var repositorio = new RepositorioDeEventosDeProcessamento();
            var eventos = repositorio.ObterUltimosEventos(processamentoId, 20);
            ViewBag.Eventos = eventos;
            return View();
        }
    }
}