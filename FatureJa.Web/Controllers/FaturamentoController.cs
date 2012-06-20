using System;
using System.Web.Mvc;
using FatureJa.Negocio.Servicos;
using FatureJa.Web.Models;

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

        //
        // POST: /Faturamento/

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public ActionResult Index(SolicitacaoDeFaturamentoModel model)
        {
            try
            {
                var faturamento = new FaturamentoDeContratos();
                faturamento.SolicitarFaturamento(model.Ano, model.Mes);
                return RedirectToAction("FaturamentoSolicitado");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        //
        // GET: /Faturamento/FaturamentoSolicitado/

        [Authorize(Roles = "Administrador")]
        public ActionResult FaturamentoSolicitado()
        {
            return View();
        }
    }
}