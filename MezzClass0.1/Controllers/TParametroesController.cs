using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MezzClass0._1.Models;

namespace MezzClass0._1.Controllers
{
    public class TParametroesController : Controller
    {
        private MezClassEntities db = new MezClassEntities();

        // GET: TParametroes
        public ActionResult Index(Int32? id,bool? error)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            if (error == true)
            {
                ViewBag.error = "Ya existe un parametro con el mismo nombre en este Universo";
            }
            var tParametro = db.TParametro.Include(t => t.TUniverso).Where(param=>param.IdUniverso==id);
            ViewBag.Id = id;
            return View(tParametro.ToList());
        }

        // GET: TParametroes/Details/5
        public ActionResult Details(int? id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TParametro tParametro = db.TParametro.Find(id);
            if (tParametro == null)
            {
                return HttpNotFound();
            }
            return View(tParametro);
        }

        // GET: TParametroes/Create
        public ActionResult Create(Int32? id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            
            ViewBag.IdUniverso = id;
            var u = db.TUniverso.Where(param => param.IdUniverso == id).ToList();
            ViewBag.nomb = u[0].NombUniverso;
           
            return View();
        }

        // POST: TParametroes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdParametro,NombParametro,descripcion,Peso,IdUniverso")] TParametro tParametro)
        {
            
            if (tParametro.descripcion == null || tParametro.NombParametro==null)
            {
                ViewBag.IdUniverso = tParametro.IdUniverso;
                ViewBag.error="El nombre de parametro y la descripcion son obligatorios";
                return View(tParametro);
            }

            if (ModelState.IsValid)
            {
                List<TParametro> prs = db.TParametro.Where(p => p.IdUniverso == tParametro.IdUniverso&&p.NombParametro==tParametro.NombParametro).ToList();
                if (prs.Count > 0)
                {
                   
                    return RedirectToAction("Index", new { id = tParametro.IdUniverso ,error=true});
                }
                db.TParametro.Add(tParametro);
               
                db.SaveChanges();
                return RedirectToAction("Index",new{id=tParametro.IdUniverso});
            }

            ViewBag.IdUniverso = new SelectList(db.TUniverso, "IdUniverso", "NombUniverso", tParametro.IdUniverso);
            return View(tParametro);
        }

        // GET: TParametroes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TParametro tParametro = db.TParametro.Find(id);
            if (tParametro == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdUniverso =tParametro.IdUniverso;
            return View(tParametro);
        }

        // POST: TParametroes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdParametro,NombParametro,descripcion,Peso,IdUniverso")] TParametro tParametro)
        {
            if (tParametro.descripcion == null || tParametro.NombParametro == null)
            {
                ViewBag.IdUniverso = tParametro.IdUniverso;
                ViewBag.error = "El nombre de parametro y la descripcion son obligatorios";
                return View(tParametro);
            }
            if (ModelState.IsValid)
            {
                var param =
                    db.TParametro.Where(
                        p => p.IdParametro != tParametro.IdParametro && p.IdUniverso == tParametro.IdUniverso && p.NombParametro==tParametro.NombParametro).ToList();
                if (param.Count > 0)
                {
                   
                    return RedirectToAction("Index", new { id = tParametro.IdUniverso,error=true });
                }

                db.Entry(tParametro).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = tParametro.IdUniverso });
            }
            ViewBag.IdUniverso = new SelectList(db.TUniverso, "IdUniverso", "NombUniverso", tParametro.IdUniverso);
            return View(tParametro);
        }

        // GET: TParametroes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TParametro tParametro = db.TParametro.Find(id);
            if (tParametro == null)
            {
                return HttpNotFound();
            }
            return View(tParametro);
        }

        // POST: TParametroes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TParametro tParametro = db.TParametro.Find(id);
            var x = db.TProblemaParametro.Where(p => p.IdParametro == id).ToList();
            if (x.Count > 0)
            {
                ViewBag.error = "Este parametro esta en uso y no se puede eliminar";
                return View(tParametro);
            }
            db.TParametro.Remove(tParametro);
            db.SaveChanges();
            return RedirectToAction("Index",new{id=tParametro.IdUniverso});
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
