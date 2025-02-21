using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VinlandSaga.Web.Controllers
{
    public class VinlandSagaController : Controller
    {
        
        public ActionResult Index()
        {
            ViewBag.Title = "Ð“Ð»Ð°Ð²Ð½Ð°Ñ";
            return View();
        }

        public ActionResult Story()
        {
            ViewBag.Title = "Ð¡ÑŽÐ¶ÐµÑ‚";
            return View();
        }

        public ActionResult Characters() 
        {
            ViewBag.Title = "ÐŸÐµÑ€ÑÐ¾Ð½Ð°Ð¶Ð¸";
            return View();
        }

        public ActionResult Gallery()
        {
            ViewBag.Title = "Ð“Ð°Ð»ÐµÑ€ÐµÑ";
            return View();
        }

        public ActionResult Author()
        {
            ViewBag.Title = "ÐÐ²Ñ‚Ð¾Ñ€";
            return View();
        }

        public ActionResult Media()
        {
            ViewBag.Title = "ÐœÐµÐ´Ð¸Ð°";
            return View();
        }

        public ActionResult Forum()
        {
            ViewBag.Title = "Ð¤Ð¾Ñ€ÑƒÐ¼";
            return View();
        }

        public ActionResult Fanart()
        {
            ViewBag.Title = "Ð¤Ð°Ð½-Ð°Ñ€Ñ‚";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Title = "ÐšÐ¾Ð½Ñ‚Ð°ÐºÑ‚Ñ‹";
            return View();
        }

        public ActionResult Profile()
        {
            ViewBag.Title = "ÐŸÑ€Ð¾Ñ„Ð¸Ð»ÑŒ";
            return View();
        }
    }
} 
