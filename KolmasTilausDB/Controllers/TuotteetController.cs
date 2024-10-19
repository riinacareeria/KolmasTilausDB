using KolmasTilausDB.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace KolmasTilausDB.Controllers
{
    public class TuotteetController : Controller
    {
        // GET: Tuotteet
        TilausDBEntities3 db = new TilausDBEntities3(); // Olio


        public ActionResult Index(string sortOrder, string currentFilter1, string searchString1, int? page, int? pagesize)
        {
            if (Session["UserName"] == null) // Toimii vain kirjautuneena
            {
                return RedirectToAction("login", "home");
            }
            else
            {
                //Lajittelu:
                ViewBag.CurrentSort = sortOrder;
                ViewBag.TuoteNimiSortParm = String.IsNullOrEmpty(sortOrder) ? "tuotenimi_desc" : "";
                ViewBag.TuoteHintaSortParm = sortOrder == "Ahinta" ? "Ahinta_desc" : "Ahinta";
                
                //Hakusuodatus muistiin:
                if (searchString1 != null)
                {
                    page = 1;
                }

                else
                {
                    searchString1 = currentFilter1;
                }

                ViewBag.CurrentFilter1 = searchString1;
                
                //Olion luonti:
                var tuotelista = from t in db.Tuotteet select t;

                //Hakusuodatus & lajittelu:
                if (!String.IsNullOrEmpty(searchString1))
                {
                    switch (sortOrder)
                    {
                        case "tuotenimi_desc":
                            tuotelista = tuotelista.Where(t => t.Nimi.Contains(searchString1)).OrderByDescending(t => t.Nimi);
                            break;
                        case "Ahinta":
                            tuotelista = tuotelista.Where(t => t.Nimi.Contains(searchString1)).OrderBy(t => t.Ahinta);
                            break;
                        case "Ahinta_desc":
                            tuotelista = tuotelista.Where(t => t.Nimi.Contains(searchString1)).OrderByDescending(t => t.Ahinta);
                            break;
                        default:
                            tuotelista = tuotelista.Where(t => t.Nimi.Contains(searchString1)).OrderBy(t => t.Nimi);
                            break;
                    }
                }

                //Lajittelu ilman hakusuodatusta
                else
                {
                    switch (sortOrder)
                    {
                        case "tuotenimi_desc":
                            tuotelista = tuotelista.OrderByDescending(t => t.Nimi);
                            break;
                        case "Ahinta":
                            tuotelista = tuotelista.OrderBy(t => t.Ahinta);
                            break;
                        case "Ahinta_desc":
                            tuotelista = tuotelista.OrderByDescending(t => t.Ahinta);
                            break;
                        default:
                            tuotelista = tuotelista.OrderBy(t => t.Nimi);
                            break;
                    }
                }

                // Sivukoon ja sivunumeron arvojen asetus
                int pageSize = (pagesize ?? 5); // Palauttaa sivukoon, jos null  -> 5 riviä/sivu
                int pageNumber = (page ?? 1); // Palauttaa sivunumeron, jos null -> 1
                return View(tuotelista.ToPagedList(pageNumber, pageSize));
            }
        }
        #region Vanha versio Indexistä
    
        public ActionResult Index1()
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

        #endregion

        #region "Ostoskori" index
        public ActionResult Index2()
        {
            List<Tuotteet> tuotelista = db.Tuotteet.ToList(); // Listan luominen
            db.Dispose();
            return View(tuotelista);
        }
        #endregion

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