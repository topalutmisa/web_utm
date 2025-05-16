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
            ViewBag.Title = "Сага";
            return View();
        }

        public ActionResult Story()
        {
            ViewBag.Title = "История";
            return View();
        }

        public ActionResult Characters() 
        {
            ViewBag.Title = "Персонажи";
            return View();
        }

        public ActionResult Gallery()
        {
            ViewBag.Title = "Галерея";
            return View();
        }

        public ActionResult Author()
        {
            ViewBag.Title = "Автор";
            return View();
        }

        public ActionResult Media()
        {
            ViewBag.Title = "Медиа";
            return View();
        }

        public ActionResult Forum()
        {
            ViewBag.Title = "Форум";
            return View();
        }

        public ActionResult Fanart()
        {
            ViewBag.Title = "ФанАрт";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Title = "Контакты";
            return View();
        }

        public ActionResult Profile()
        {
            ViewBag.Title = "Профиль";
            return View();
        }
    }
} 
