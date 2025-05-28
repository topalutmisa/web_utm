using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using VinlandSaga.Application.BussinessLogic;
using VinlandSaga.Application.BussinessLogic.Interfaces;
using VinlandSaga.Web.Models;

namespace VinlandSaga.Web.Controllers
{
    [Authorize(Roles = "Administrator,Moderator")]
    public class UsersController : Controller
    {
        private readonly IUserBL _userBL;

        public UsersController()
        {
            var factory = BusinessLogicFactory.Instance;
            _userBL = factory.GetUserBL();
        }

        public ActionResult Index(int page = 1, string search = "")
        {
            const int pageSize = 10;
            
            try
            {
                var users = string.IsNullOrEmpty(search) 
                    ? _userBL.GetRecentUsers(pageSize * page)
                    : _userBL.SearchUsers(search, page, pageSize);
                
                var totalUsers = _userBL.GetTotalUsersCount();
                
                var model = new UsersListViewModel
                {
                    Users = users.Select(u => new UserViewModel
                    {
                        Id = u.Id,
                        Username = u.Username,
                        Email = u.Email,
                        DisplayName = u.DisplayName ?? u.Username,
                        RegistrationDate = u.RegistrationDate,
                        LastLoginDate = u.LastLoginDate,
                        IsActive = u.IsActive,
                        IsEmailConfirmed = u.IsEmailConfirmed
                    }).ToList(),
                    CurrentPage = page,
                    TotalPages = (int)Math.Ceiling((double)totalUsers / pageSize),
                    SearchTerm = search,
                    TotalUsers = totalUsers
                };
                
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Ошибка при загрузке пользователей: " + ex.Message;
                return View(new UsersListViewModel());
            }
        }

        public ActionResult Details(Guid id)
        {
            try
            {
                var user = _userBL.GetUserProfile(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                
                var model = new UserDetailsViewModel
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    DisplayName = user.DisplayName ?? user.Username,
                    About = user.About,
                    AvatarUrl = user.AvatarUrl,
                    RegistrationDate = user.RegistrationDate,
                    LastLoginDate = user.LastLoginDate,
                    IsActive = user.IsActive,
                    IsEmailConfirmed = user.IsEmailConfirmed,
                    Roles = user.Roles?.ToList() ?? new List<string>()
                };
                
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Ошибка при загрузке пользователя: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleActive(Guid id)
        {
            try
            {
                var user = _userBL.GetUserProfile(id);
                if (user != null)
                {
                    // Здесь нужно расширить UserBL для изменения статуса
                    // Пока что просто показываем сообщение
                    TempData["InfoMessage"] = "Функция изменения статуса будет реализована в UserBL";
                }
                else
                {
                    TempData["ErrorMessage"] = "Пользователь не найден";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ошибка при изменении статуса пользователя: " + ex.Message;
            }
            
            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmEmail(Guid id)
        {
            try
            {
                var user = _userBL.GetUserProfile(id);
                if (user != null)
                {
                    // Здесь нужно расширить UserBL для подтверждения email
                    // Пока что просто показываем сообщение
                    TempData["InfoMessage"] = "Функция подтверждения email будет реализована в UserBL";
                }
                else
                {
                    TempData["ErrorMessage"] = "Пользователь не найден";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ошибка при подтверждении email: " + ex.Message;
            }
            
            return RedirectToAction("Details", new { id });
        }
    }
} 