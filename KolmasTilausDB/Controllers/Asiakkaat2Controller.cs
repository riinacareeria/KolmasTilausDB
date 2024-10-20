﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using KolmasTilausDB.Models;
using PagedList;

namespace KolmasTilausDB.Controllers
{
    public class Asiakkaat2Controller : Controller
    {
        private TilausDBEntities3 db = new TilausDBEntities3();

        public ActionResult Index(string sortOrder, string currentFilter2, string searchString2, string postitoimipaikkaX, string currentPostitoimipaikka,  int? page, int? pagesize)
        {
            if (Session["UserName"] == null) // Toimii vain kirjautuneena
            {
                return RedirectToAction("login", "home");
            }
            else
            {
                //Lajittelu:
                ViewBag.CurrentSort = sortOrder;
                ViewBag.AsiakasNimiSortParm = String.IsNullOrEmpty(sortOrder) ? "asiakasnimi_desc" : "";

                //Hakusuodatus muistiin:
                if (searchString2 != null)
                {
                    page = 1;
                }

                else
                {
                    searchString2 = currentFilter2;
                }

                ViewBag.CurrentFilter1 = searchString2;

                //Pudotusvalikko muistiin:
                if ((postitoimipaikkaX != null) && (postitoimipaikkaX != "0"))
                {
                    page = 1;
                }

                else
                {
                    postitoimipaikkaX = currentPostitoimipaikka;
                }

                ViewBag.currentPostitoimipaikka = postitoimipaikkaX;


                //Olioiden luonti:
                var asiakaslista = from a in db.Asiakkaat select a;

                var postitmpList = from pt in db.Postitoimipaikat select pt;

                //Asiakashaku tekstikentällä

                if (!String.IsNullOrEmpty(searchString2))
                {
                    asiakaslista = asiakaslista.Where(a => a.Nimi.Contains(searchString2));
                }

                //Asiakashaku postitoimipaikan mukaan (pudotusvalikko)

                if (!String.IsNullOrEmpty(postitoimipaikkaX) && (postitoimipaikkaX !=""))
                {
                    string para = postitoimipaikkaX;
                    asiakaslista = asiakaslista.Where(pt => pt.Postinumero == para);
                }

                //Jos tekstihaku & lajittelu käytössä:
                if (!String.IsNullOrEmpty(searchString2))
                {
                    switch (sortOrder)
                    {
                        case "asiakasnimi_desc":
                            asiakaslista = asiakaslista.Where(a => a.Nimi.Contains(searchString2)).OrderByDescending(a => a.Nimi);
                            break;
                        default:
                            asiakaslista = asiakaslista.Where(a => a.Nimi.Contains(searchString2)).OrderBy(a => a.Nimi);
                            break;
                    }
                }
                
                //Muussa tapauksessa pudotusvalikko & lajittelu käytössä
                else if(!String.IsNullOrEmpty(postitoimipaikkaX) && (postitoimipaikkaX != ""))
                {
                    string para = postitoimipaikkaX;
                    switch (sortOrder)
                    {
                        case "asiakasnimi_desc":
                            asiakaslista = asiakaslista.Where(pt => pt.Postinumero == para).OrderByDescending(pt => pt.Postinumero);
                            break;
                        default:
                            asiakaslista = asiakaslista.Where(pt => pt.Postinumero == para).OrderBy(pt => pt.Postinumero);
                            break;
                    }
                }

                //Tai jos niistä ei kumpaakaan, niin sitten lajittelu ilman hakusuodatusta
                else
                {
                    switch (sortOrder)
                    {
                        case "asiakasnimi_desc":
                            asiakaslista = asiakaslista.OrderByDescending(a => a.Nimi);
                            break;
                        default:
                            asiakaslista = asiakaslista.OrderBy(a => a.Nimi);
                            break;
                    }
                }

                //Haku pudotusvalikolla:
                List<Postitoimipaikat> lstPosTmp = new List<Postitoimipaikat>();

                Postitoimipaikat tyhjaPosTmp = new Postitoimipaikat();
                tyhjaPosTmp.Postinumero = "0";
                tyhjaPosTmp.Postitoimipaikka = "";
                tyhjaPosTmp.PosNroPosTmp = "";
                lstPosTmp.Add(tyhjaPosTmp);

                foreach (Postitoimipaikat paikka in postitmpList)
                {
                    Postitoimipaikat yksiPaikka = new Postitoimipaikat();
                    yksiPaikka.Postinumero = paikka.Postinumero;
                    yksiPaikka.Postitoimipaikka = paikka.Postitoimipaikka;
                    yksiPaikka.PosNroPosTmp = paikka.Postinumero.ToString() + " - " + paikka.Postitoimipaikka; //Pudotusvalikossa näkyvä
                    lstPosTmp.Add(yksiPaikka);
                }
                ViewBag.Postinumero = new SelectList(lstPosTmp, "Postinumero", "Postitoimipaikka", postitoimipaikkaX);


                // Sivukoon ja sivunumeron arvojen asetus
                int pageSize = (pagesize ?? 5); // Palauttaa sivukoon, jos null  -> 5 riviä/sivu
                int pageNumber = (page ?? 1); // Palauttaa sivunumeron, jos null -> 1
                return View(asiakaslista.ToPagedList(pageNumber, pageSize));
            }
        }
        #region Vanha versio
        //public ActionResult Index()
        //{
        //    if (Session["UserName"] == null) // Toimii vain kirjautuneena
        //    {
        //        return RedirectToAction("login", "home");
        //    }
        //    else
        //    {
        //        var asiakkaat = db.Asiakkaat.Include(a => a.Postitoimipaikat);


        //        return View(asiakkaat.ToList());
        //    }
        //}
        #endregion

        // GET: Asiakkaat2/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Asiakkaat asiakkaat = db.Asiakkaat.Find(id);
            if (asiakkaat == null)
            {
                return HttpNotFound();
            }
            return View(asiakkaat);
        }

        // GET: Asiakkaat2/Create
        public ActionResult Create()
        {
            //ViewBag.Postinumero = new SelectList(db.Postitoimipaikat, "Postinumero", "Postitoimipaikka");
            var posti = db.Postitoimipaikat;
            IEnumerable<SelectListItem> selectPostiList = from p in posti
                                                          select new SelectListItem
                                                          {
                                                              Value = p.Postinumero,
                                                              Text = p.Postinumero + " " + p.Postitoimipaikka.ToString()
                                                          };

            ViewBag.Postinumero = new SelectList(selectPostiList, "Value", "Text");
            return View();
        }

        // POST: Asiakkaat2/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AsiakasID,Nimi,Osoite,Postinumero")] Asiakkaat asiakkaat)
        {
            if (ModelState.IsValid)
            {
                db.Asiakkaat.Add(asiakkaat);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Postinumero = new SelectList(db.Postitoimipaikat, "Postinumero", "Postitoimipaikka", asiakkaat.Postinumero);
            return View(asiakkaat);
        }

        // GET: Asiakkaat2/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Asiakkaat asiakkaat = db.Asiakkaat.Find(id);
            if (asiakkaat == null)
            {
                return HttpNotFound();
            }
            //ViewBag.Postinumero = new SelectList(db.Postitoimipaikat, "Postinumero", "Postitoimipaikka", asiakkaat.Postinumero);
            var posti = db.Postitoimipaikat;
            IEnumerable<SelectListItem> selectPostiList = from p in posti
                                                          select new SelectListItem
                                                          {
                                                              Value = p.Postinumero,
                                                              Text = p.Postinumero + " " + p.Postitoimipaikka
                                                          };

            ViewBag.Postinumero = new SelectList(selectPostiList, "Value", "Text", asiakkaat.Postinumero);

            return View(asiakkaat);
        }

        // POST: Asiakkaat2/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AsiakasID,Nimi,Osoite,Postinumero")] Asiakkaat asiakkaat)
        {
            if (ModelState.IsValid)
            {
                db.Entry(asiakkaat).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.Postinumero = new SelectList(db.Postitoimipaikat, "Postinumero", "Postitoimipaikka", asiakkaat.Postinumero);
            var posti = db.Postitoimipaikat;
            IEnumerable<SelectListItem> selectPostiList = from p in posti
                                                          select new SelectListItem
                                                          {
                                                              Value = p.Postinumero,
                                                              Text = p.Postinumero + " " + p.Postitoimipaikka
                                                          };

            ViewBag.Postinumero = new SelectList(selectPostiList, "Value", "Text", asiakkaat.Postinumero);
            return View(asiakkaat);
        }

        // GET: Asiakkaat2/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Asiakkaat asiakkaat = db.Asiakkaat.Find(id);
            if (asiakkaat == null)
            {
                return HttpNotFound();
            }
            return View(asiakkaat);
        }

        // POST: Asiakkaat2/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Asiakkaat asiakkaat = db.Asiakkaat.Find(id);
            db.Asiakkaat.Remove(asiakkaat);
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
