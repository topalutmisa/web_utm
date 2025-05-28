using System;
using System.Linq;
using System.Web.Mvc;
using VinlandSaga.Application.BussinessLogic;
using VinlandSaga.Application.BussinessLogic.Interfaces;
using VinlandSaga.Web.Models;

namespace VinlandSaga.Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IUserBL _userBL;

        public ProfileController()
        {
            var factory = BusinessLogicFactory.Instance;
            _userBL = factory.GetUserBL();
        }

        public ActionResult Index()
        {
            try
            {
                var user = _userBL.GetUserProfile(User.Identity.Name);
                
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var model = new ProfileViewModel
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    DisplayName = user.DisplayName ?? user.Username,
                    About = user.About ?? "",
                    AvatarUrl = user.AvatarUrl ?? "",
                    RegistrationDate = user.RegistrationDate,
                    LastLoginDate = user.LastLoginDate,
                    IsEmailConfirmed = user.IsEmailConfirmed
                };

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Ошибка загрузки профиля: " + ex.Message;
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpGet]
        public ActionResult Edit()
        {
            try
            {
                var user = _userBL.GetUserProfile(User.Identity.Name);
                
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var model = new EditProfileViewModel
                {
                    DisplayName = user.DisplayName ?? user.Username,
                    About = user.About ?? "",
                    AvatarUrl = user.AvatarUrl ?? ""
                };

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Ошибка загрузки профиля: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = _userBL.GetUserProfile(User.Identity.Name);
                
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Обновляем профиль через DTO
                user.DisplayName = model.DisplayName;
                user.About = model.About;
                user.AvatarUrl = model.AvatarUrl;

                var success = _userBL.UpdateUserProfile(user);
                
                if (success)
                {
                    TempData["SuccessMessage"] = "Профиль успешно обновлен!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Ошибка при обновлении профиля");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ошибка при обновлении профиля: " + ex.Message);
                return View(model);
            }
        }
    }
} 