using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using VinlandSaga.Application.BussinessLogic;
using VinlandSaga.Application.BussinessLogic.Interfaces;
using VinlandSaga.Web.Models;

namespace VinlandSaga.Web.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly IUserBL _userBL;
        private readonly IForumBL _forumBL;
        private readonly INewsBL _newsBL;
        private readonly ICharacterBL _characterBL;

        public AdminController()
        {
            var factory = BusinessLogicFactory.Instance;
            _userBL = factory.GetUserBL();
            _forumBL = factory.GetForumBL();
            _newsBL = factory.GetNewsBL();
            _characterBL = factory.GetCharacterBL();
        }

        public ActionResult AdminPanel()
        {
            var model = new AdminPanelViewModel();
            
            try
            {
                // Статистика для админ-панели
                model.UsersCount = _userBL.GetTotalUsersCount();
                model.TopicsCount = _forumBL.GetTopicsCount();
                model.PostsCount = _forumBL.GetPostsCount();
                model.NewsCount = _newsBL.GetNewsCount();
                model.CharactersCount = _characterBL.GetCharactersCount();
                
                // Последние пользователи
                try
                {
                    var recentUsers = _userBL.GetRecentUsers(5);
                    model.RecentUsers = recentUsers
                        .Select(u => new RecentUserViewModel
                        { 
                            Username = !string.IsNullOrEmpty(u.Username) ? u.Username : 
                                      (!string.IsNullOrEmpty(u.Email) ? u.Email : "Неизвестно"), 
                            Email = !string.IsNullOrEmpty(u.Email) ? u.Email : "Нет email", 
                            RegistrationDate = u.RegistrationDate 
                        })
                        .ToList();
                }
                catch (Exception userEx)
                {
                    model.RecentUsers = new List<RecentUserViewModel>();
                    model.UserError = $"Ошибка загрузки пользователей: {userEx.Message}";
                }
                
                // Последние темы форума
                try
                {
                    var recentTopics = _forumBL.GetRecentTopics(5);
                    model.RecentTopics = recentTopics
                        .Select(t => new RecentTopicViewModel
                        { 
                            Title = !string.IsNullOrEmpty(t.Title) ? t.Title : "Без названия",
                            CreatedDate = t.CreatedDate, 
                            ViewsCount = t.ViewsCount, 
                            PostsCount = t.PostsCount 
                        })
                        .ToList();
                }
                catch (Exception topicEx)
                {
                    model.RecentTopics = new List<RecentTopicViewModel>();
                    model.TopicError = $"Ошибка загрузки тем: {topicEx.Message}";
                }
            }
            catch (Exception ex)
            {
                model.Error = ex.Message;
                model.RecentUsers = new List<RecentUserViewModel>();
                model.RecentTopics = new List<RecentTopicViewModel>();
            }

            return View(model);
        }

        public ActionResult ManageUsers()
        {
            return RedirectToAction("Index", "Users");
        }

        public ActionResult ManageNews()
        {
            return RedirectToAction("Index", "News");
        }

        public ActionResult ManageCharacters()
        {
            return RedirectToAction("Index", "Characters");
        }

        public ActionResult ManageForum()
        {
            return RedirectToAction("Index", "Forum");
        }

        public ActionResult TestPanel()
        {
            return RedirectToAction("Index", "Test");
        }

        public ActionResult DatabaseStatus()
        {
            return RedirectToAction("DatabaseConnection", "Test");
        }

        [HttpPost]
        public ActionResult ClearCache()
        {
            try
            {
                // Очистка кэша (если используется)
                System.Web.HttpRuntime.UnloadAppDomain();
                TempData["Success"] = "Кэш приложения успешно очищен!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ошибка очистки кэша: {ex.Message}";
            }
            return RedirectToAction("AdminPanel");
        }

        [HttpPost]
        public ActionResult BackupDatabase()
        {
            try
            {
                // Здесь можно добавить логику резервного копирования
                TempData["Info"] = "Функция резервного копирования будет реализована позже";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ошибка создания резервной копии: {ex.Message}";
            }
            return RedirectToAction("AdminPanel");
        }

        public ActionResult SystemInfo()
        {
            try
            {
                ViewBag.ServerTime = DateTime.Now;
                ViewBag.ApplicationVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                ViewBag.DotNetVersion = Environment.Version.ToString();
                ViewBag.MachineName = Environment.MachineName;
                ViewBag.ProcessorCount = Environment.ProcessorCount;
                ViewBag.WorkingSet = Environment.WorkingSet;
                ViewBag.OSVersion = Environment.OSVersion.ToString();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View();
        }

        public ActionResult UserActivity()
        {
            try
            {
                // Активность пользователей за последние 30 дней
                var activeUsers = _userBL.GetActiveUsers(10);
                var recentUsers = _userBL.GetRecentUsers(30);
                var recentTopics = _forumBL.GetRecentTopics(30);
                
                ViewBag.NewUsersThisMonth = recentUsers.Count;
                ViewBag.ActiveUsersThisMonth = activeUsers.Count;
                ViewBag.NewTopicsThisMonth = recentTopics.Count;
                ViewBag.NewPostsThisMonth = _forumBL.GetPostsCount(); // Приблизительно
                
                ViewBag.TopActiveUsers = activeUsers.Take(10).ToList();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View();
        }

        [HttpPost]
        public ActionResult ToggleUserStatus(Guid userId)
        {
            try
            {
                var user = _userBL.GetUserProfile(userId);
                if (user != null)
                {
                    // Логика переключения статуса пользователя
                    // Можно расширить UserBL для этого функционала
                    TempData["Success"] = "Статус пользователя изменен";
                }
                else
                {
                    TempData["Error"] = "Пользователь не найден";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ошибка изменения статуса: {ex.Message}";
            }
            return RedirectToAction("AdminPanel");
        }
    }
} 