using System;
using System.Web.Mvc;
using VinlandSaga.Application.BussinessLogic;
using VinlandSaga.Application.BussinessLogic.Interfaces;
using VinlandSaga.Web.Models;

namespace VinlandSaga.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserBL _userBL;

        public AccountController()
        {
            var factory = BusinessLogicFactory.Instance;
            _userBL = factory.GetUserBL();
        }

        [HttpGet]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "VinlandSaga");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = _userBL.Login(model.Email, model.Password);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.ErrorMessage);
                return View(model);
            }

            // Устанавливаем cookie аутентификации
            if (result.AuthCookie != null)
            {
                Response.Cookies.Add(result.AuthCookie);
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "VinlandSaga");
        }

        [HttpGet]
        public ActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "VinlandSaga");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var result = _userBL.Register(model.Username, model.Email, model.Password, model.Username);
                
                if (!result.IsSuccess)
                {
                    ModelState.AddModelError("", result.ErrorMessage);
                    return View(model);
                }

                TempData["SuccessMessage"] = "Регистрация прошла успешно! Теперь вы можете войти в систему.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ошибка регистрации. Попробуйте позже.");
                System.Diagnostics.Debug.WriteLine($"Registration Error: {ex.Message}");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Logout()
        {
            var result = _userBL.SignOut();
            
            if (result.IsSuccess && result.ExpiredCookie != null)
            {
                Response.Cookies.Add(result.ExpiredCookie);
            }

            return RedirectToAction("Index", "VinlandSaga");
        }

        [HttpGet]
        [Authorize]
        public ActionResult Profile()
        {
            var userProfile = _userBL.GetUserProfile(User.Identity.Name);
            if (userProfile == null)
            {
                return RedirectToAction("Login");
            }

            return View(userProfile);
        }
    }
} 