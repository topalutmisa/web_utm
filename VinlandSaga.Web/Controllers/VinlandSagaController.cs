using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VinlandSaga.Application.BussinessLogic;
using VinlandSaga.Application.BussinessLogic.Interfaces;

namespace VinlandSaga.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly INewsBL _newsBL;
        private readonly ICharacterBL _characterBL;
        private readonly IForumBL _forumBL;

        public HomeController()
        {
            var factory = BusinessLogicFactory.Instance;
            _newsBL = factory.GetNewsBL();
            _characterBL = factory.GetCharacterBL();
            _forumBL = factory.GetForumBL();
        }
        
        public ActionResult Index()
        {
            ViewBag.Title = "Главная";
            
            // Получаем данные для главной страницы через Business Logic
            ViewBag.LatestNews = _newsBL.GetLatestNews(3);
            ViewBag.FeaturedCharacters = _characterBL.GetFeaturedCharacters(4);
            ViewBag.RecentTopics = _forumBL.GetRecentTopics(5);
            
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
