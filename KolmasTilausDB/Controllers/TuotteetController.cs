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
    public class TuotteetController : Controller
    {
        // GET: Tuotteet
        TilausDBEntities3 db = new TilausDBEntities3(); // Olio

        public ActionResult Index()
        {
            if (Session["UserName"] == null) // Toimii vain kirjautuneena
            {
                return RedirectToAction("login", "home");
            }
            else
            {
                List<Tuotteet> tuotelista = db.Tuotteet.ToList(); // Listan luominen
                db.Dispose();
                return View(tuotelista);
            }
        }

        public ActionResult Index2()
        {
            List<Tuotteet> tuotelista = db.Tuotteet.ToList(); // Listan luominen
            db.Dispose();
            return View(tuotelista);
        }

        #region Edit
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Tuotteet tuotelista = db.Tuotteet.Find(id);
            if (tuotelista == null) return HttpNotFound();
            return View(tuotelista);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TuoteID, Nimi, Ahinta, ImageLink")] Tuotteet tuotelista)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tuotelista).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index2");
            }
            return View(tuotelista);
        }
        #endregion

        #region Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TuoteID, Nimi, Ahinta, ImageLink")] Tuotteet tuotelista)
        {
            if (ModelState.IsValid)
            {
                db.Tuotteet.Add(tuotelista);
                db.SaveChanges();
                return RedirectToAction("Index2");
            }
            return View(tuotelista);
        }
        #endregion

        #region Delete
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Tuotteet tuotelista = db.Tuotteet.Find(id);
            if (tuotelista == null) return HttpNotFound();
            return View(tuotelista);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tuotteet tuotelista = db.Tuotteet.Find(id);
            db.Tuotteet.Remove(tuotelista);
            db.SaveChanges();
            return RedirectToAction("Index2");
        }
        #endregion
    }
}