using System;
using System.Web.Mvc;
using FatureJa.Negocio.Servicos;
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
        // GET: /Administracao/GerarContratos/

        [Authorize(Roles = "Administrador")]
        public ActionResult GerarContratos()
        {
            return View();
        }

        //
        // POST: /Administracao/GerarContratos/

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public ActionResult GerarContratos(GeracaoDeContratosModel model)
        {
            try
            {
                var gerador = new GeradorDeContratos();
                gerador.SolicitarGeracao(model.QuantidadeContratos);
                return RedirectToAction("GeracaoSolicitada");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        //
        // GET: /Administracao/GerarMovimento/

        [Authorize(Roles = "Administrador")]
        public ActionResult GerarMovimento()
        {
            return View();
        }

        //
        // POST: /Administracao/GerarMovimento/

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public ActionResult GerarMovimento(GeracaoDeMovimentoModel model)
        {
            try
            {
                var gerador = new GeradorDeMovimento();
                gerador.SolicitarGeracao(model.Ano, model.Mes);
                return RedirectToAction("GeracaoSolicitada");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
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