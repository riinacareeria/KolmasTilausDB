using KolmasTilausDB.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace KolmasTilausDB.Controllers
{
    public class AsiakkaatController : Controller
    {
        // GET: Asiakkaat
        TilausDBEntities3 db = new TilausDBEntities3(); // Olio

        public ActionResult Index()
        {
            if (Session["UserName"] == null) // Toimii vain kirjautuneena
            {
                return RedirectToAction("login", "home");
            }
            else
            {
                List<Asiakkaat> asiakaslista = db.Asiakkaat.ToList(); // Listan luominen
                db.Dispose();
                return View(asiakaslista);
            }
        }

        #region Edit
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Asiakkaat asiakaslista = db.Asiakkaat.Find(id);
            if (asiakaslista == null) return HttpNotFound();
            return View(asiakaslista);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AsiakasID, Nimi, Osoite, Postitoimipaikat")] Asiakkaat asiakaslista)
        {
            if (ModelState.IsValid)
            {
                db.Entry(asiakaslista).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(asiakaslista);
        }
        #endregion

        #region Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AsiakasID, Nimi, Osoite, Postitoimipaikat")] Asiakkaat asiakaslista)
        {
            if (ModelState.IsValid)
            {
                db.Asiakkaat.Add(asiakaslista);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(asiakaslista);
        }
        #endregion

        #region Delete
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Asiakkaat asiakaslista = db.Asiakkaat.Find(id);
            if (asiakaslista == null) return HttpNotFound();
            return View(asiakaslista);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Asiakkaat asiakaslista = db.Asiakkaat.Find(id);
            db.Asiakkaat.Remove(asiakaslista);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        #endregion
    }
}