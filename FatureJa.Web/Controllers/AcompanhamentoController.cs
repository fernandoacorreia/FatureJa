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
            ViewBag.ProcessamentoId = processamentoId;

            var processamento = new RepositorioDeProcessamentos().ObterPorProcessamentoId(processamentoId);
            ViewBag.Message = String.Format("{0} {1}", processamento.Comando, processamento.Parametros);

            var repositorio = new RepositorioDeEventosDeProcessamento();
            List<EventoDeProcessamento> eventos = repositorio.ObterUltimosEventos(processamentoId, 20);

            if (eventos.Count == 0)
            {
                ViewBag.Mensagem = "Não há nenhum evento registrado para este processamento.";
                return View();
            }

            ViewBag.Eventos = eventos;
            return View();
        }

        public ActionResult VisualizacaoPorPeriodo(Guid processamentoId)
        {
            ViewBag.ProcessamentoId = processamentoId;

            var processamento = new RepositorioDeProcessamentos().ObterPorProcessamentoId(processamentoId);
            ViewBag.Message = String.Format("{0} {1}", processamento.Comando, processamento.Parametros);

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

            long quantidadeAcumulada = 0;
            var seriePorPeriodo = new StringBuilder();
            seriePorPeriodo.AppendLine("\"Horario,Quantidade\\n\" +");
            foreach (var item in quantidadePorFaixa)
            {
                quantidadeAcumulada += item.Quantidade;
                seriePorPeriodo.AppendLine(String.Format("\"{0},{1}\\n\" +", item.Periodo.ToString("yyyy/MM/dd HH:mm:ss"),
                                             item.Quantidade));
            }
            seriePorPeriodo.AppendLine("\"\"");
            ViewBag.Serie = seriePorPeriodo.ToString();

            ViewBag.Quantidade = quantidadeAcumulada;

            var minDate = (from e in eventos select e.Inicio).Min();
            var maxDate = (from e in eventos select e.Termino).Max();
            TimeSpan duracao = maxDate - minDate;
            double velocidade = duracao.TotalSeconds == 0 ? 0 : Math.Round(quantidadeAcumulada/duracao.TotalSeconds, 1);
            ViewBag.Velocidade = velocidade;

            return View();
        }

        public ActionResult VisualizacaoAcumulada(Guid processamentoId)
        {
            ViewBag.ProcessamentoId = processamentoId;

            var processamento = new RepositorioDeProcessamentos().ObterPorProcessamentoId(processamentoId);
            ViewBag.Message = String.Format("{0} {1}", processamento.Comando, processamento.Parametros);

            var repositorio = new RepositorioDeEventosDeProcessamento();
            List<EventoDeProcessamento> eventos = repositorio.ObterUltimosEventos(processamentoId, int.MaxValue);

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
                     select new { Periodo = g.Key, Quantidade = g.Sum(o => o.Quantidade) })
                    .ToList();

            long quantidadeAcumulada = 0;
            var serieAcumulada = new StringBuilder();
            serieAcumulada.AppendLine("\"Horario,Quantidade\\n\" +");
            foreach (var item in quantidadePorFaixa)
            {
                quantidadeAcumulada += item.Quantidade;
                serieAcumulada.AppendLine(String.Format("\"{0},{1}\\n\" +", item.Periodo.ToString("yyyy/MM/dd HH:mm:ss"),
                                             quantidadeAcumulada));
            }
            serieAcumulada.AppendLine("\"\"");
            ViewBag.Serie = serieAcumulada.ToString();
            ViewBag.Quantidade = quantidadeAcumulada;

            var minDate = (from e in eventos select e.Inicio).Min();
            var maxDate = (from e in eventos select e.Termino).Max();
            TimeSpan duracao = maxDate - minDate;
            double velocidade = duracao.TotalSeconds == 0 ? 0 : Math.Round(quantidadeAcumulada / duracao.TotalSeconds, 1);
            ViewBag.Velocidade = velocidade;

            return View();
        }
    }
}