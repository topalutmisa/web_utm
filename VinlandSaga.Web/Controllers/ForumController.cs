using System;
using System.Linq;
using System.Web.Mvc;
using VinlandSaga.Application.BussinessLogic;
using VinlandSaga.Application.BussinessLogic.Interfaces;
using VinlandSaga.Domain.DTOs;
using VinlandSaga.Web.Models;

namespace VinlandSaga.Web.Controllers
{
    public class ForumController : Controller
    {
        private readonly IForumBL _forumBL;
        private readonly IUserBL _userBL;

        public ForumController()
        {
            var factory = BusinessLogicFactory.Instance;
            _forumBL = factory.GetForumBL();
            _userBL = factory.GetUserBL();
        }

        public ActionResult Index(int page = 1, Guid? categoryId = null)
        {
            const int pageSize = 10;
            
            try
            {
                var topics = categoryId.HasValue 
                    ? _forumBL.GetTopicsByCategory(categoryId.Value, page, pageSize)
                    : _forumBL.GetRecentTopics(pageSize);
                
                var categories = _forumBL.GetAllCategories();
                var usersCount = _userBL.GetTotalUsersCount();
                
                var model = new ForumIndexViewModel
                {
                    Topics = topics.Select(t => new ForumTopicViewModel
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Description = t.Content,
                        CreatedDate = t.CreatedDate,
                        LastPostDate = t.LastPostDate,
                        PostsCount = t.PostsCount,
                        ViewsCount = t.ViewsCount,
                        IsLocked = t.IsLocked,
                        IsPinned = t.IsSticky,
                        UserId = t.AuthorId,
                        CategoryId = t.CategoryId,
                        AuthorName = t.AuthorName
                    }).ToList(),
                    Categories = categories.Select(c => new CategoryViewModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        TopicsCount = _forumBL.GetTopicsByCategory(c.Id, 1, 1000).Count
                    }).ToList(),
                    CurrentPage = page,
                    TotalPages = (int)Math.Ceiling((double)_forumBL.GetTopicsCount() / pageSize),
                    TotalTopics = _forumBL.GetTopicsCount(),
                    SelectedCategoryId = categoryId
                };
                
                ViewBag.UsersCount = usersCount;
                
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Ошибка при загрузке форума: " + ex.Message;
                return View(new ForumIndexViewModel());
            }
        }

        public ActionResult Topic(Guid id, int page = 1)
        {
            const int pageSize = 20;
            
            try
            {
                var topic = _forumBL.GetTopic(id);
                if (topic == null)
                {
                    return HttpNotFound();
                }
                
                var posts = _forumBL.GetPostsByTopic(id, page, pageSize);
                
                var model = new ForumTopicDetailsViewModel
                {
                    Topic = new ForumTopicViewModel
                    {
                        Id = topic.Id,
                        Title = topic.Title,
                        Description = topic.Content,
                        CreatedDate = topic.CreatedDate,
                        LastPostDate = topic.LastPostDate,
                        PostsCount = topic.PostsCount,
                        ViewsCount = topic.ViewsCount,
                        IsLocked = topic.IsLocked,
                        IsPinned = topic.IsSticky,
                        UserId = topic.AuthorId,
                        CategoryId = topic.CategoryId,
                        AuthorName = topic.AuthorName
                    },
                    Posts = posts.Select(p => new ForumPostViewModel
                    {
                        Id = p.Id,
                        Content = p.Content,
                        CreatedDate = p.CreatedDate,
                        ModifiedDate = p.ModifiedDate,
                        UserId = p.AuthorId,
                        TopicId = p.TopicId,
                        AuthorName = GetUserDisplayName(p.AuthorId),
                        CanEdit = CanEditPost(p.AuthorId)
                    }).ToList(),
                    CurrentPage = page,
                    TotalPages = (int)Math.Ceiling((double)topic.PostsCount / pageSize),
                    TotalPosts = topic.PostsCount,
                    CanReply = !topic.IsLocked && Request.IsAuthenticated
                };
                
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Ошибка при загрузке темы: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [Authorize]
        public ActionResult CreateTopic()
        {
            var categories = _forumBL.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(new CreateForumTopicViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CreateTopic(CreateForumTopicViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var categories = _forumBL.GetAllCategories();
                ViewBag.Categories = new SelectList(categories, "Id", "Name", model.CategoryId);
                return View(model);
            }

            try
            {
                var currentUser = _userBL.GetUserProfile(User.Identity.Name);
                if (currentUser == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                
                var actionDto = new ForumActionDto
                {
                    Title = model.Title,
                    Content = model.Content,
                    UserId = currentUser.Id,
                    CategoryId = model.CategoryId
                };
                
                var result = _forumBL.CreateTopic(actionDto);
                
                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Тема успешно создана!";
                    return RedirectToAction("Topic", new { id = result.CreatedTopicId });
                }
                else
                {
                    ModelState.AddModelError("", result.ErrorMessage);
                    var categories = _forumBL.GetAllCategories();
                    ViewBag.Categories = new SelectList(categories, "Id", "Name", model.CategoryId);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ошибка при создании темы: " + ex.Message);
                var categories = _forumBL.GetAllCategories();
                ViewBag.Categories = new SelectList(categories, "Id", "Name", model.CategoryId);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Reply(Guid topicId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["ErrorMessage"] = "Содержимое поста не может быть пустым";
                return RedirectToAction("Topic", new { id = topicId });
            }

            try
            {
                var currentUser = _userBL.GetUserProfile(User.Identity.Name);
                if (currentUser == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                
                var actionDto = new ForumActionDto
                {
                    Content = content,
                    UserId = currentUser.Id,
                    TopicId = topicId
                };
                
                var result = _forumBL.CreatePost(actionDto);
                
                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Ответ успешно добавлен!";
                }
                else
                {
                    TempData["ErrorMessage"] = result.ErrorMessage;
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ошибка при добавлении ответа: " + ex.Message;
            }
            
            return RedirectToAction("Topic", new { id = topicId });
        }

        [Authorize]
        public ActionResult EditPost(Guid id)
        {
            try
            {
                var posts = _forumBL.GetPostsByTopic(Guid.Empty, 1, 1000); // Получаем все посты для поиска
                var post = posts.FirstOrDefault(p => p.Id == id);
                
                if (post == null || !CanEditPost(post.AuthorId))
                {
                    return HttpNotFound();
                }
                
                var model = new EditForumPostViewModel
                {
                    Id = post.Id,
                    Content = post.Content,
                    TopicId = post.TopicId
                };
                
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Ошибка при загрузке поста: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult EditPost(EditForumPostViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var success = _forumBL.UpdatePost(model.Id, model.Content);
                
                if (success)
                {
                    TempData["SuccessMessage"] = "Пост успешно обновлен!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Ошибка при обновлении поста";
                }
                
                return RedirectToAction("Topic", new { id = model.TopicId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ошибка при обновлении поста: " + ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeletePost(Guid id)
        {
            try
            {
                var posts = _forumBL.GetPostsByTopic(Guid.Empty, 1, 1000); // Получаем все посты для поиска
                var post = posts.FirstOrDefault(p => p.Id == id);
                
                if (post == null || !CanEditPost(post.AuthorId))
                {
                    TempData["ErrorMessage"] = "У вас нет прав для удаления этого поста";
                    return RedirectToAction("Index");
                }
                
                var success = _forumBL.DeletePost(id);
                
                if (success)
                {
                    TempData["SuccessMessage"] = "Пост успешно удален!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Ошибка при удалении поста";
                }
                
                return RedirectToAction("Topic", new { id = post.TopicId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ошибка при удалении поста: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        private string GetUserDisplayName(Guid userId)
        {
            try
            {
                var user = _userBL.GetUserProfile(userId);
                return user?.DisplayName ?? user?.Username ?? "Неизвестный пользователь";
            }
            catch
            {
                return "Неизвестный пользователь";
            }
        }

        private bool CanEditPost(Guid postUserId)
        {
            if (!Request.IsAuthenticated)
                return false;
                
            if (User.IsInRole("Administrator") || User.IsInRole("Moderator"))
                return true;
                
            var currentUser = _userBL.GetUserProfile(User.Identity.Name);
            return currentUser != null && currentUser.Id == postUserId;
        }
    }
} 