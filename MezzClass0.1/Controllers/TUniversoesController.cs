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
using System.Web.Routing;
using MezzClass0._1.Models;

namespace MezzClass0._1.Controllers
{
    public class TUniversoesController : Controller
    {
        private MezClassEntities db = new MezClassEntities();

        // GET: TUniversoes
        public ActionResult Index(int? p)
            
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            if (p == 1)
            {
               
                const int x = 1;
                ViewBag.bandera = x;
                return View(db.TUniverso.ToList());
            }
            if (p == 2)
            {
                const int x = 2;
                ViewBag.bandera = x;
                return View(db.TUniverso.ToList());
            }
            return View(db.TUniverso.ToList());
        }

        // GET: TUniversoes/Details/5
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
            TUniverso tUniverso = db.TUniverso.Find(id);
            if (tUniverso == null)
            {
                return HttpNotFound();
            }
            return View(tUniverso);
        }

        // GET: TUniversoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TUniversoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdUniverso,NombUniverso,CantNiveles")] TUniverso tUniverso)
        {
            if (ModelState.IsValid)
            {
                var u = db.TUniverso.Where(p => p.NombUniverso == tUniverso.NombUniverso).ToList();
                if (u.Count > 0)
                {
                    ViewBag.error = "Ya existe un Universo con ese Nombre";
                    return View(tUniverso);
                }
                db.TUniverso.Add(tUniverso);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tUniverso);
        }

        // GET: TUniversoes/Edit/5
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
            TUniverso tUniverso = db.TUniverso.Find(id);
            if (tUniverso == null)
            {
                return HttpNotFound();
            }
            return View(tUniverso);
        }

        // POST: TUniversoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdUniverso,NombUniverso,CantNiveles")] TUniverso tUniverso)
        {
            if (ModelState.IsValid)
            {
                var u =
                    db.TUniverso.Where(
                        p => p.IdUniverso != tUniverso.IdUniverso && p.NombUniverso == tUniverso.NombUniverso).ToList();
                if (u.Count > 0)
                {
                    ViewBag.error = "Ya existe un Universo con ese Nombre";
                    return View(tUniverso);
                }
                db.Entry(tUniverso).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tUniverso);
        }

        // GET: TUniversoes/Delete/5
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
            TUniverso tUniverso = db.TUniverso.Find(id);
            if (tUniverso == null)
            {
                return HttpNotFound();
            }
            return View(tUniverso);
        }

        // POST: TUniversoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TUniverso tUniverso = db.TUniverso.Find(id);
            
            db.TProblema.RemoveRange(tUniverso.TProblema);
            db.TParametro.RemoveRange(tUniverso.TParametro);
          
            db.TUniverso.Remove(tUniverso);
            db.SaveChanges();
            return RedirectToAction("Index");
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
