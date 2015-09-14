using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MezzClass0._1.Models;
using Microsoft.Owin.Security.Google;

namespace MezzClass0._1.Controllers
{
    public class ClasificarController : Controller
    {
        // GET: Clasificar
        private MezClassEntities db = new MezClassEntities();

        public ActionResult Index(Int32 id)
        {
            TUniverso u = db.TUniverso.Find(id);
            ViewBag.universo = u.NombUniverso;
            ViewBag.problemas =
                db.TProblema.Include(t => t.TUniverso)                   
                    .Where(p => p.IdUniverso == id && p.ejemplo == false);
            return View();
        }

        public ActionResult Similares(Int32 id)
        {
            TProblema tProblema = db.TProblema.Find(id);
            ViewBag.titulo = tProblema.titulo;
            ViewBag.enunciado = tProblema.Enunciado;
            List<TProblema> probs = db.TProblema.Include(t=>t.TProblemaParametro).Where(p => p.ejemplo && p.IdUniverso==tProblema.IdUniverso).ToList();
            List<TProblema> fin = new List<TProblema>();
            for (int i = 0; i < probs.Count; i++)
            {
                if (CompararParams(tProblema.TProblemaParametro.ToList(),probs[i].TProblemaParametro.ToList()))
                {
                    fin.Add(probs[i]);
                }
            }
            if (fin.Count > 0)
            {
                ViewBag.probs = fin;
            }

            return View();
        }

        public bool CompararParams(List<TProblemaParametro> a, List<TProblemaParametro> b)
        {
            if (a.Count != b.Count)
                return false;
            List<TProblemaParametro>a1 = a.OrderBy(p => p.TParametro.NombParametro).ToList();
            List<TProblemaParametro> b1 = b.OrderBy(p => p.TParametro.NombParametro).ToList();
            for (int i = 0; i < a1.Count(); i++)
            {
                if (a1[i].TParametro.NombParametro != b1[i].TParametro.NombParametro)
                    return false;
            }
            return true;
        }
    }
}
