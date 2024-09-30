using KolmasTilausDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KolmasTilausDB.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Muokkaus: HomeController/About/ViewBagMessage.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Muokkaus: HomeController/Contact/ViewBagMessage.";

            return View();
        }

        public ActionResult Map()
        {
            ViewBag.Message = "Yhteystiedot";

            return View();
        }
        #region Login / Logout
        public ActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Authorize(Logins LoginModel)
        {
            TilausDBEntities3 db = new TilausDBEntities3();
            
            
            var LoggedUser = db.Logins.SingleOrDefault(x => x.UserName == LoginModel.UserName && x.PassWord == LoginModel.PassWord);
            if (LoggedUser != null)
            {
                ViewBag.LoginMessage = "Sisäänkirjautuminen onnistui";
                Session["LoggedStatus"] = "Olet kirjautuneena";
                Session["UserName"] = LoggedUser.UserName;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.LoginMessage = "Sisäänkirjautuminen epäonnistui";
                ViewBag.LoggedStatus = "Ei kirjautuneena";
                LoginModel.LoginErrorMessage = "Virheellinen käyttäjätunnus tai salasana.";
                return View("Login", LoginModel);
            }
        }

        public ActionResult LogOut()
        {
            Session.Abandon();
            Session["LoggedStatus"] = "Ei kirjautuneena";
            return RedirectToAction("Index", "Home"); //Uloskirjautumisen jälkeen pääsivulle

        }

        #endregion
    }
}