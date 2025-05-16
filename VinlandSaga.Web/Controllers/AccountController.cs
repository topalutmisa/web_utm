using System;
using System.Web.Mvc;
using System.Web.Security;
using VinlandSaga.Application.Services;
using VinlandSaga.Infrastructure.Data;
using VinlandSaga.Web.Models;

namespace VinlandSaga.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthService _authService;

        public AccountController()
        {
            var dbContext = new VinlandSagaDbContext(); // In a real application, it's better to use DI
            _authService = new AuthService(dbContext);
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

            var user = _authService.AuthenticateUser(model.Email, model.Password);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View(model);
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
                var user = _authService.RegisterUser(model.Username, model.Email, model.Password);
                return RedirectToAction("Login");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Registration error. Please try again later.");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Logout()
        {
            _authService.LogoutUser();
            return RedirectToAction("Index", "VinlandSaga");
        }

        [HttpGet]
        [Authorize]
        public ActionResult Profile()
        {
            var user = _authService.GetCurrentUser();
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            return View();
        }
    }
} 