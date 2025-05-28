using System;
using System.Linq;
using System.Web.Mvc;
using VinlandSaga.Application.BussinessLogic;
using VinlandSaga.Application.BussinessLogic.Interfaces;
using VinlandSaga.Domain.DTOs;
using VinlandSaga.Web.Models;

namespace VinlandSaga.Web.Controllers
{
    public class NewsController : Controller
    {
        private readonly INewsBL _newsBL;
        private readonly IUserBL _userBL;

        public NewsController()
        {
            var factory = BusinessLogicFactory.Instance;
            _newsBL = factory.GetNewsBL();
            _userBL = factory.GetUserBL();
        }

        public ActionResult Index(int page = 1)
        {
            const int pageSize = 5;
            
            try
            {
                var news = _newsBL.GetPublishedNews(page, pageSize);
                var totalNews = _newsBL.GetPublishedNewsCount();
                
                var model = new NewsListViewModel
                {
                    News = news.Select(n => new NewsViewModel
                    {
                        Id = n.Id,
                        Title = n.Title,
                        Content = n.Content,
                        Summary = n.Summary,
                        ImageUrl = n.ImageUrl,
                        PublishedDate = n.PublishDate,
                        AuthorId = n.AuthorId,
                        AuthorName = n.AuthorName,
                        IsPublished = n.IsPublished,
                        ViewCount = n.ViewsCount
                    }).ToList(),
                    CurrentPage = page,
                    TotalPages = (int)Math.Ceiling((double)totalNews / pageSize),
                    TotalNews = totalNews
                };
                
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Ошибка при загрузке новостей: " + ex.Message;
                return View(new NewsListViewModel());
            }
        }

        public ActionResult Details(Guid id)
        {
            try
            {
                var news = _newsBL.GetNews(id);
                if (news == null || (!news.IsPublished && !User.IsInRole("Administrator") && !User.IsInRole("Moderator")))
                {
                    return HttpNotFound();
                }
                
                // Увеличиваем счетчик просмотров
                _newsBL.IncrementViewCount(id);
                
                var model = new NewsDetailsViewModel
                {
                    Id = news.Id,
                    Title = news.Title,
                    Content = news.Content,
                    Summary = news.Summary,
                    ImageUrl = news.ImageUrl,
                    PublishedDate = news.PublishDate,
                    AuthorId = news.AuthorId,
                    AuthorName = news.AuthorName,
                    IsPublished = news.IsPublished,
                    ViewCount = news.ViewsCount
                };
                
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Ошибка при загрузке новости: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "Administrator,Moderator")]
        public ActionResult Create()
        {
            return View(new CreateNewsViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Moderator")]
        public ActionResult Create(CreateNewsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var currentUser = _userBL.GetUserProfile(User.Identity.Name);
                if (currentUser == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                
                var newsDto = new NewsDto
                {
                    Title = model.Title,
                    Content = model.Content,
                    Summary = model.Summary,
                    ImageUrl = model.ImageUrl,
                    AuthorId = currentUser.Id,
                    IsPublished = model.IsPublished
                };
                
                var success = _newsBL.CreateNews(newsDto);
                
                if (success)
                {
                    TempData["SuccessMessage"] = "Новость успешно создана!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Ошибка при создании новости");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ошибка при создании новости: " + ex.Message);
                return View(model);
            }
        }

        [Authorize(Roles = "Administrator,Moderator")]
        public ActionResult Edit(Guid id)
        {
            try
            {
                var news = _newsBL.GetNews(id);
                if (news == null)
                {
                    return HttpNotFound();
                }
                
                var model = new EditNewsViewModel
                {
                    Id = news.Id,
                    Title = news.Title,
                    Content = news.Content,
                    Summary = news.Summary,
                    ImageUrl = news.ImageUrl,
                    IsPublished = news.IsPublished
                };
                
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Ошибка при загрузке новости: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Moderator")]
        public ActionResult Edit(EditNewsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var newsDto = new NewsDto
                {
                    Id = model.Id,
                    Title = model.Title,
                    Content = model.Content,
                    Summary = model.Summary,
                    ImageUrl = model.ImageUrl,
                    IsPublished = model.IsPublished
                };
                
                var success = _newsBL.UpdateNews(newsDto);
                
                if (success)
                {
                    TempData["SuccessMessage"] = "Новость успешно обновлена!";
                    return RedirectToAction("Details", new { id = model.Id });
                }
                else
                {
                    ModelState.AddModelError("", "Ошибка при обновлении новости");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ошибка при обновлении новости: " + ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(Guid id)
        {
            try
            {
                var success = _newsBL.DeleteNews(id);
                
                if (success)
                {
                    TempData["SuccessMessage"] = "Новость успешно удалена!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Ошибка при удалении новости";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ошибка при удалении новости: " + ex.Message;
            }
            
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Moderator")]
        public ActionResult TogglePublish(Guid id)
        {
            try
            {
                var news = _newsBL.GetNews(id);
                if (news == null)
                {
                    TempData["ErrorMessage"] = "Новость не найдена";
                    return RedirectToAction("Index");
                }
                
                bool success;
                if (news.IsPublished)
                {
                    success = _newsBL.UnpublishNews(id);
                    TempData["SuccessMessage"] = success ? "Новость снята с публикации" : "Ошибка при снятии с публикации";
                }
                else
                {
                    success = _newsBL.PublishNews(id);
                    TempData["SuccessMessage"] = success ? "Новость опубликована" : "Ошибка при публикации";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ошибка: " + ex.Message;
            }
            
            return RedirectToAction("Index");
        }
    }
} 