using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using FatureJa.Negocio.Armazenamento;
using FatureJa.Negocio.Entidades;
using Microsoft.WindowsAzure.StorageClient;

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

        public ActionResult Visualizacao(Guid processamentoId)
        {
            // mensagem informativa
            var processamento = new RepositorioDeProcessamentos().ObterPorProcessamentoId(processamentoId);
            ViewBag.Message = String.Format("{0} {1}", processamento.Comando, processamento.Parametros);

            // obter query de eventos
            var repositorio = new RepositorioDeEventosDeProcessamento();
            List<EventoDeProcessamento> eventos = repositorio.ObterEventos(processamentoId);

            // consolidar por período
            int segundosPorFaixa = 5;
            var quantidadePorFaixa =
                (from e in eventos
                 let periodo = e.ObterHorarioArredondado(segundosPorFaixa)
                 orderby periodo
                 group e by periodo
                     into g
                     select new Medicao { Periodo = g.Key, Quantidade = g.Sum(o => o.Quantidade) })
                    .ToList();
            if (quantidadePorFaixa.Count == 0)
            {
                ViewBag.Erro = "Não há nenhum evento registrado para este processamento.";
                return View();
            }

            // série de dados por período
            var seriePorPeriodo = new StringBuilder();
            seriePorPeriodo.AppendLine("\"Horário,Quantidade\\n\" +");
            foreach (var item in quantidadePorFaixa)
            {
                seriePorPeriodo.AppendLine(String.Format("\"{0},{1}\\n\" +", item.Periodo.ToString("yyyy/MM/dd HH:mm:ss"),
                                             item.Quantidade));
            }
            seriePorPeriodo.AppendLine("\"\"");
            ViewBag.SeriePorPeriodo = seriePorPeriodo.ToString();

            // série de dados de velocidade média
            DateTime horarioInicial = DateTime.MaxValue;
            long quantidadeTotal = 0;
            var itensUltimoMinuto = new List<Medicao>();
            var serieVelocidadeMedia = new StringBuilder();
            serieVelocidadeMedia.AppendLine("\"Horário,Geral,Um minuto\\n\" +");
            foreach (var item in quantidadePorFaixa)
            {
                quantidadeTotal += item.Quantidade;
                if (horarioInicial > item.Periodo)
                {
                    horarioInicial = item.Periodo;
                }
                var lapso = item.Periodo - horarioInicial;
                double segundos = lapso.TotalSeconds + 1;
                double mediaGeral = quantidadeTotal / segundos;

                itensUltimoMinuto.Add(item);
                itensUltimoMinuto.RemoveAll(m => (item.Periodo - m.Periodo).TotalSeconds > 60);
                double totalUltimoMinuto = (from i in itensUltimoMinuto select i.Quantidade).Sum();
                double mediaUltimoMinuto = totalUltimoMinuto / 60;

                serieVelocidadeMedia.AppendLine(String.Format("\"{0},{1},{2}\\n\" +", item.Periodo.ToString("yyyy/MM/dd HH:mm:ss"),
                                             Math.Round(mediaGeral, 0), Math.Round(mediaUltimoMinuto, 0)));
            }
            serieVelocidadeMedia.AppendLine("\"\"");
            ViewBag.SerieVelocidadeMedia = serieVelocidadeMedia.ToString();

            // série de dados acumulados
            long quantidadeAcumulada = 0;
            var serieAcumulada = new StringBuilder();
            serieAcumulada.AppendLine("\"Horário,Quantidade\\n\" +");
            foreach (var item in quantidadePorFaixa)
            {
                quantidadeAcumulada += item.Quantidade;
                serieAcumulada.AppendLine(String.Format("\"{0},{1}\\n\" +", item.Periodo.ToString("yyyy/MM/dd HH:mm:ss"),
                                             quantidadeAcumulada));
            }
            serieAcumulada.AppendLine("\"\"");
            ViewBag.SerieAcumulada = serieAcumulada.ToString();

            return View();
        }

        protected class Medicao
        {
            public DateTime Periodo { get; set; }

            public int Quantidade { get; set; }
        }
    }
}