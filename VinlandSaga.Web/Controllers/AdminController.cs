using System.Web.Mvc;

namespace VinlandSaga.Web.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        public ActionResult AdminPanel()
        {
            ViewBag.Title = "Панель администратора";
            return View();
        }
    }
} 