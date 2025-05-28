using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using VinlandSaga.Infrastructure.Data;
using VinlandSaga.Application.BussinessLogic;
using VinlandSaga.Application.BussinessLogic.Interfaces;

namespace VinlandSaga.Web.Controllers
{
    public class TestController : Controller
    {
        public ActionResult DatabaseConnection()
        {
            try
            {
                using (var context = new VinlandSagaDbContext())
                {
                    // Попытка подключения к базе данных
                    var canConnect = context.Database.Exists();
                    ViewBag.CanConnect = canConnect;
                    
                    if (canConnect)
                    {
                        // Проверяем количество ролей
                        var rolesCount = context.Roles.Count();
                        ViewBag.RolesCount = rolesCount;
                        
                        // Проверяем количество пользователей
                        var usersCount = context.Users.Count();
                        ViewBag.UsersCount = usersCount;
                        
                        ViewBag.Message = "Подключение к базе данных успешно!";
                    }
                    else
                    {
                        ViewBag.Message = "База данных не существует.";
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.CanConnect = false;
                ViewBag.Message = $"Ошибка подключения: {ex.Message}";
                if (ex.InnerException != null)
                {
                    ViewBag.InnerError = ex.InnerException.Message;
                }
            }
            
            return View();
        }

        public ActionResult InitializeRoles()
        {
            try
            {
                using (var context = new VinlandSagaDbContext())
                {
                    // Создаем базовые роли, если их нет
                    var roles = new[]
                    {
                        new { Name = "Administrator", Description = "Полный доступ ко всем функциям сайта" },
                        new { Name = "Moderator", Description = "Управление контентом и пользователями" },
                        new { Name = "User", Description = "Стандартный доступ к сайту" },
                        new { Name = "Artist", Description = "Подтвержденный статус художника для загрузки фан-арта" }
                    };

                    int addedRoles = 0;
                    foreach (var roleInfo in roles)
                    {
                        var existingRole = context.Roles.FirstOrDefault(r => r.Name == roleInfo.Name);
                        if (existingRole == null)
                        {
                            context.Roles.Add(new VinlandSaga.Domain.Models.Role
                            {
                                Id = Guid.NewGuid(),
                                Name = roleInfo.Name,
                                Description = roleInfo.Description
                            });
                            addedRoles++;
                        }
                    }

                    context.SaveChanges();
                    
                    ViewBag.Success = true;
                    ViewBag.Message = $"Инициализация завершена! Добавлено ролей: {addedRoles}";
                    ViewBag.TotalRoles = context.Roles.Count();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Success = false;
                ViewBag.Message = $"Ошибка инициализации: {ex.Message}";
                if (ex.InnerException != null)
                {
                    ViewBag.InnerError = ex.InnerException.Message;
                }
            }
            
            return View();
        }

        public ActionResult TestRegistration()
        {
            try
            {
                var factory = BusinessLogicFactory.Instance;
                var userBL = factory.GetUserBL();
                
                // Генерируем уникальные данные для тестирования
                var testUsername = "testuser_" + DateTime.Now.Ticks;
                var testEmail = $"test_{DateTime.Now.Ticks}@example.com";
                var testPassword = "TestPassword123!";
                
                ViewBag.TestData = $"Username: {testUsername}, Email: {testEmail}";
                
                var result = userBL.Register(testUsername, testEmail, testPassword, testUsername);
                
                if (result.IsSuccess)
                {
                    ViewBag.Success = true;
                    ViewBag.Message = $"Тестовый пользователь успешно зарегистрирован! ID: {result.UserId}";
                    ViewBag.UserId = result.UserId;
                    ViewBag.Username = testUsername;
                    ViewBag.Email = testEmail;
                }
                else
                {
                    ViewBag.Success = false;
                    ViewBag.Message = $"Ошибка регистрации: {result.ErrorMessage}";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Success = false;
                ViewBag.Message = $"Ошибка регистрации: {ex.Message}";
                
                // Собираем все внутренние исключения
                var innerErrors = new List<string>();
                var currentEx = ex.InnerException;
                while (currentEx != null)
                {
                    innerErrors.Add(currentEx.Message);
                    currentEx = currentEx.InnerException;
                }
                
                if (innerErrors.Any())
                {
                    ViewBag.InnerError = string.Join(" -> ", innerErrors);
                }
                
                // Специальная обработка для DbEntityValidationException
                if (ex is System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    var validationErrors = new List<string>();
                    foreach (var validationError in dbEx.EntityValidationErrors)
                    {
                        foreach (var error in validationError.ValidationErrors)
                        {
                            validationErrors.Add($"{error.PropertyName}: {error.ErrorMessage}");
                        }
                    }
                    ViewBag.ValidationErrors = validationErrors;
                }
            }
            
            return View();
        }

        [HttpPost]
        public ActionResult InitializeAdmin()
        {
            try
            {
                using (var context = new VinlandSagaDbContext())
                {
                    // Проверяем, есть ли уже админ
                    var adminRole = context.Roles.FirstOrDefault(r => r.Name == "Administrator");
                    if (adminRole == null)
                    {
                        TempData["Error"] = "Сначала инициализируйте роли!";
                        return RedirectToAction("Index");
                    }

                    var existingAdmin = context.Users.FirstOrDefault(u => u.Username == "admin");
                    if (existingAdmin != null)
                    {
                        TempData["Warning"] = $"Администратор уже существует! Username: {existingAdmin.Username}, Email: {existingAdmin.Email}";
                        return RedirectToAction("Index");
                    }

                    // Создаем администратора через новую архитектуру
                    var factory = BusinessLogicFactory.Instance;
                    var userBL = factory.GetUserBL();
                    
                    var result = userBL.Register("admin", "admin@vinlandsaga.com", "Admin123!", "Administrator");
                    
                    if (result.IsSuccess)
                    {
                        // Назначаем роль администратора
                        var userRole = new VinlandSaga.Domain.Models.UserRole
                        {
                            Id = Guid.NewGuid(),
                            UserId = result.UserId.Value,
                            RoleId = adminRole.Id,
                            AssignedDate = DateTime.Now
                        };
                        context.UserRoles.Add(userRole);
                        context.SaveChanges();

                        TempData["Success"] = "Администратор успешно создан! Username: admin, Email: admin@vinlandsaga.com, Password: Admin123!";
                    }
                    else
                    {
                        TempData["Error"] = $"Ошибка создания администратора: {result.ErrorMessage}";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ошибка создания администратора: {ex.Message}";
                if (ex.InnerException != null)
                {
                    TempData["Error"] += $" Детали: {ex.InnerException.Message}";
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult InitializeCategories()
        {
            try
            {
                using (var context = new VinlandSagaDbContext())
                {
                    var categories = new[]
                    {
                        new { Name = "Manga Discussion", Description = "Обсуждение манги Vinland Saga", SortOrder = 1 },
                        new { Name = "Anime Discussion", Description = "Обсуждение аниме Vinland Saga", SortOrder = 2 },
                        new { Name = "Character Analysis", Description = "Анализ персонажей и их развития", SortOrder = 3 },
                        new { Name = "Historical Context", Description = "Исторический контекст эпохи викингов", SortOrder = 4 },
                        new { Name = "Fan Theories", Description = "Теории и предположения фанатов", SortOrder = 5 },
                        new { Name = "General Discussion", Description = "Общие обсуждения", SortOrder = 6 }
                    };

                    int addedCategories = 0;
                    foreach (var catInfo in categories)
                    {
                        var existingCategory = context.Categories.FirstOrDefault(c => c.Name == catInfo.Name);
                        if (existingCategory == null)
                        {
                            context.Categories.Add(new VinlandSaga.Domain.Models.Category
                            {
                                Id = Guid.NewGuid(),
                                Name = catInfo.Name,
                                Description = catInfo.Description,
                                Slug = catInfo.Name.ToLower().Replace(" ", "-"),
                                SortOrder = catInfo.SortOrder,
                                IsActive = true
                            });
                            addedCategories++;
                        }
                    }

                    context.SaveChanges();

                    var totalCategories = context.Categories.Count();
                    TempData["Success"] = $"Категории инициализированы! Добавлено: {addedCategories}, Всего: {totalCategories}";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ошибка создания категорий: {ex.Message}";
                if (ex.InnerException != null)
                {
                    TempData["Error"] += $" Детали: {ex.InnerException.Message}";
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult InitializeSampleData()
        {
            try
            {
                using (var context = new VinlandSagaDbContext())
                {
                    // Проверяем наличие админа
                    var admin = context.Users.FirstOrDefault(u => u.Username == "admin");
                    if (admin == null)
                    {
                        TempData["Error"] = "Сначала создайте администратора!";
                        return RedirectToAction("Index");
                    }

                    // Проверяем наличие категорий
                    var categories = context.Categories.ToList();
                    if (!categories.Any())
                    {
                        TempData["Error"] = "Сначала создайте категории!";
                        return RedirectToAction("Index");
                    }

                    // Создаем тестовые темы форума
                    var sampleTopics = new[]
                    {
                        new { Title = "Добро пожаловать на форум Vinland Saga!", Description = "Представьтесь и расскажите о себе", Content = "Добро пожаловать на официальный форум Vinland Saga! Здесь вы можете обсуждать мангу, аниме, персонажей и многое другое. Расскажите о себе в комментариях!" },
                        new { Title = "Обсуждение последней главы манги", Description = "Что вы думаете о развитии сюжета?", Content = "Давайте обсудим последние события в манге. Какие моменты вас больше всего впечатлили?" },
                        new { Title = "Лучшие моменты аниме", Description = "Делимся любимыми сценами из аниме", Content = "Какие сцены из аниме Vinland Saga произвели на вас наибольшее впечатление? Поделитесь своими мыслями!" },
                        new { Title = "Анализ персонажа: Торфинн", Description = "Развитие главного героя на протяжении истории", Content = "Торфинн прошел невероятный путь развития. Давайте проанализируем его характер и мотивацию." }
                    };

                    int addedTopics = 0;
                    foreach (var topicInfo in sampleTopics)
                    {
                        var existingTopic = context.ForumTopics.FirstOrDefault(t => t.Title == topicInfo.Title);
                        if (existingTopic == null)
                        {
                            var randomCategory = categories[new Random().Next(categories.Count)];
                            var topicId = Guid.NewGuid();

                            var topic = new VinlandSaga.Domain.Models.ForumTopic
                            {
                                Id = topicId,
                                Title = topicInfo.Title,
                                Description = topicInfo.Description,
                                CreatedDate = DateTime.Now.AddDays(-new Random().Next(1, 30)),
                                UserId = admin.Id,
                                CategoryId = randomCategory.Id,
                                PostsCount = 1,
                                ViewsCount = new Random().Next(10, 100),
                                IsLocked = false,
                                IsPinned = addedTopics == 0 // Первая тема закрепленная
                            };

                            var firstPost = new VinlandSaga.Domain.Models.ForumPost
                            {
                                Id = Guid.NewGuid(),
                                Content = topicInfo.Content,
                                CreatedDate = topic.CreatedDate,
                                UserId = admin.Id,
                                TopicId = topicId
                            };

                            context.ForumTopics.Add(topic);
                            context.ForumPosts.Add(firstPost);
                            addedTopics++;
                        }
                    }

                    context.SaveChanges();

                    var totalTopics = context.ForumTopics.Count();
                    TempData["Success"] = $"Тестовые данные созданы! Добавлено тем: {addedTopics}, Всего тем: {totalTopics}";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ошибка создания тестовых данных: {ex.Message}";
                if (ex.InnerException != null)
                {
                    TempData["Error"] += $" Детали: {ex.InnerException.Message}";
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult Index()
        {
            try
            {
                using (var context = new VinlandSagaDbContext())
                {
                    ViewBag.DatabaseExists = context.Database.Exists();
                    ViewBag.RolesCount = context.Roles.Count();
                    ViewBag.UsersCount = context.Users.Count();
                    ViewBag.CategoriesCount = context.Categories.Count();
                    ViewBag.TopicsCount = context.ForumTopics.Count();
                    ViewBag.PostsCount = context.ForumPosts.Count();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return View();
        }

        public ActionResult AuthCheck()
        {
            return View();
        }

        [HttpPost]
        public ActionResult MakeCurrentUserAdmin()
        {
            try
            {
                if (!Request.IsAuthenticated)
                {
                    TempData["Error"] = "Вы должны быть авторизованы для выполнения этого действия";
                    return RedirectToAction("AuthCheck");
                }

                using (var context = new VinlandSagaDbContext())
                {
                    var currentUserEmail = User.Identity.Name;
                    var user = context.Users.FirstOrDefault(u => u.Email == currentUserEmail);
                    
                    if (user == null)
                    {
                        TempData["Error"] = "Пользователь не найден в базе данных";
                        return RedirectToAction("AuthCheck");
                    }

                    var adminRole = context.Roles.FirstOrDefault(r => r.Name == "Administrator");
                    if (adminRole == null)
                    {
                        TempData["Error"] = "Роль Administrator не найдена. Сначала инициализируйте роли.";
                        return RedirectToAction("AuthCheck");
                    }

                    // Проверяем, есть ли уже эта роль у пользователя
                    var existingUserRole = context.UserRoles.FirstOrDefault(ur => ur.UserId == user.Id && ur.RoleId == adminRole.Id);
                    if (existingUserRole != null)
                    {
                        TempData["Warning"] = "У пользователя уже есть роль администратора";
                        return RedirectToAction("AuthCheck");
                    }

                    // Назначаем роль администратора
                    var userRole = new VinlandSaga.Domain.Models.UserRole
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        RoleId = adminRole.Id,
                        AssignedDate = DateTime.Now
                    };
                    context.UserRoles.Add(userRole);
                    context.SaveChanges();

                    TempData["Success"] = $"Роль администратора успешно назначена пользователю {user.Username}! Перелогиньтесь для применения изменений.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ошибка назначения роли: {ex.Message}";
            }

            return RedirectToAction("AuthCheck");
        }

        [HttpPost]
        public ActionResult RefreshAuthCookie()
        {
            try
            {
                if (!Request.IsAuthenticated)
                {
                    TempData["Error"] = "Вы должны быть авторизованы для выполнения этого действия";
                    return RedirectToAction("AuthCheck");
                }

                using (var context = new VinlandSagaDbContext())
                {
                    var currentUserEmail = User.Identity.Name;
                    var user = context.Users.FirstOrDefault(u => u.Email == currentUserEmail);
                    
                    if (user == null)
                    {
                        TempData["Error"] = "Пользователь не найден в базе данных";
                        return RedirectToAction("AuthCheck");
                    }

                    // Создаем cookie вручную с актуальными ролями
                    var roles = string.Join(",", context.UserRoles
                        .Where(ur => ur.UserId == user.Id)
                        .Select(ur => ur.Role.Name));
                        
                    var userData = $"{user.Id}|{user.Email}|{roles}";
                    
                    var ticket = new System.Web.Security.FormsAuthenticationTicket(
                        1,
                        user.Email,
                        DateTime.Now,
                        DateTime.Now.AddHours(12),
                        true, 
                        userData, 
                        System.Web.Security.FormsAuthentication.FormsCookiePath);

                    var encryptedTicket = System.Web.Security.FormsAuthentication.Encrypt(ticket);
                    var cookie = new System.Web.HttpCookie(System.Web.Security.FormsAuthentication.FormsCookieName, encryptedTicket)
                    {
                        HttpOnly = true,
                        Secure = System.Web.Security.FormsAuthentication.RequireSSL,
                        Path = System.Web.Security.FormsAuthentication.FormsCookiePath
                    };
                    
                    Response.Cookies.Add(cookie);

                    TempData["Success"] = "Cookie аутентификации обновлен! Обновите страницу для применения изменений.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ошибка обновления cookie: {ex.Message}";
            }

            return RedirectToAction("AuthCheck");
        }

        public ActionResult CheckUserData()
        {
            try
            {
                using (var context = new VinlandSagaDbContext())
                {
                    var users = context.Users.Take(10).ToList();
                    ViewBag.Users = users;
                    ViewBag.UsersCount = context.Users.Count();
                    
                    // Проверяем структуру первого пользователя
                    if (users.Any())
                    {
                        var firstUser = users.First();
                        ViewBag.FirstUserInfo = new
                        {
                            Id = firstUser.Id,
                            Username = firstUser.Username ?? "NULL",
                            Email = firstUser.Email ?? "NULL",
                            DisplayName = firstUser.DisplayName ?? "NULL",
                            RegistrationDate = firstUser.RegistrationDate,
                            IsActive = firstUser.IsActive
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            
            return View();
        }
    }
} 