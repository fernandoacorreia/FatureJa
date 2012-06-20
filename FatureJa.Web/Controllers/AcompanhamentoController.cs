using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using FatureJa.Negocio.Armazenamento;
using FatureJa.Negocio.Entidades;

namespace FatureJa.Web.Controllers
{
    public class AcompanhamentoController : Controller
    {
        //
        // GET: /Acompanhamento/

        public ActionResult Index()
        {
            var repositorio = new RepositorioDeProcessamentos();
            IEnumerable<Processamento> processamentos = repositorio.ObterUltimosProcessamentos(20);
            ViewBag.Processamentos = processamentos;
            return View();
        }

        public ActionResult Lista(Guid processamentoId)
        {
            var processamento = new RepositorioDeProcessamentos().ObterPorProcessamentoId(processamentoId);
            ViewBag.Message = String.Format("{0} {1}", processamento.Comando, processamento.Parametros);

            var repositorio = new RepositorioDeEventosDeProcessamento();
            List<EventoDeProcessamento> eventos = repositorio.ObterUltimosEventos(processamentoId, 20);

            if (eventos.Count == 0)
            {
                ViewBag.Mensagem = "Não há nenhum evento registrado para este processamento.";
                return View();
            }

            ViewBag.ProcessamentoId = processamentoId;
            ViewBag.Eventos = eventos;
            return View();
        }

        public ActionResult Visualizacao(Guid processamentoId)
        {
            var processamento = new RepositorioDeProcessamentos().ObterPorProcessamentoId(processamentoId);
            ViewBag.Message = String.Format("{0} {1}", processamento.Comando, processamento.Parametros);

            ViewBag.ProcessamentoId = processamentoId;

            var repositorio = new RepositorioDeEventosDeProcessamento();
            List<EventoDeProcessamento> eventos = repositorio.ObterUltimosEventos(processamentoId, 1000);

            if (eventos.Count == 0)
            {
                ViewBag.Mensagem = "Não há nenhum evento registrado para este processamento.";
                return View();
            }

            var quantidadePorFaixa =
                (from e in eventos
                 let periodo = e.ObterHorarioArredondado(5)
                 orderby periodo
                 group e by periodo
                 into g
                 select new {Periodo = g.Key, Quantidade = g.Sum(o => o.Quantidade)})
                    .ToList();

            var csv = new StringBuilder();
            csv.AppendLine("\"Horario,Quantidade\\n\" +");

            long quantidadeAcumulada = 0;
            foreach (var item in quantidadePorFaixa)
            {
                quantidadeAcumulada += item.Quantidade;
                csv.AppendLine(String.Format("\"{0},{1}\\n\" +", item.Periodo.ToString("yyyy/MM/dd HH:mm:ss"),
                                             quantidadeAcumulada));
            }
            ViewBag.Quantidade = quantidadeAcumulada;

            csv.AppendLine("\"\"");
            ViewBag.Csv = csv.ToString();

            var minDate = (from e in eventos select e.Inicio).Min();
            var maxDate = (from e in eventos select e.Termino).Max();
            TimeSpan duracao = maxDate - minDate;
            double velocidade = duracao.TotalSeconds == 0 ? 0 : Math.Round(quantidadeAcumulada/duracao.TotalSeconds, 1);
            ViewBag.Velocidade = velocidade;

            return View();
        }
    }
}