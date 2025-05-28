using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using VinlandSaga.Application.BussinessLogic.Core;
using VinlandSaga.Application.BussinessLogic.Interfaces;
using VinlandSaga.Domain.DTOs;
using VinlandSaga.Domain.Models;

namespace VinlandSaga.Application.BussinessLogic.BLogic
{
    public class UserBL : BaseApi, IUserBL
    {
        public AuthResultDto Login(string email, string password)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == email && u.IsActive);
                if (user == null)
                {
                    return new AuthResultDto
                    {
                        IsSuccess = false,
                        ErrorMessage = "Пользователь не найден или неактивен"
                    };
                }

                // Проверка пароля (здесь должна быть проверка хеша)
                if (user.PasswordHash != password) // Временно для простоты
                {
                    return new AuthResultDto
                    {
                        IsSuccess = false,
                        ErrorMessage = "Неверный пароль"
                    };
                }

                // Получение ролей пользователя
                var roles = GetUserRoles(user.Id);
                var rolesString = string.Join(",", roles);

                // Создание cookie аутентификации
                var authTicket = new FormsAuthenticationTicket(
                    1,
                    user.Email,
                    DateTime.Now,
                    DateTime.Now.AddDays(30),
                    true,
                    rolesString
                );

                var encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
                {
                    HttpOnly = true,
                    Secure = false,
                    Expires = DateTime.Now.AddDays(30)
                };

                // Обновление даты последнего входа
                user.LastLoginDate = DateTime.Now;
                SaveChanges();

                return new AuthResultDto
                {
                    IsSuccess = true,
                    UserId = user.Id,
                    Email = user.Email,
                    UserRole = rolesString,
                    AuthCookie = authCookie
                };
            }
            catch (Exception ex)
            {
                return new AuthResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = $"Ошибка входа: {ex.Message}"
                };
            }
        }

        public AuthResultDto Register(string username, string email, string password, string displayName)
        {
            try
            {
                // Проверка существования пользователя
                if (_context.Users.Any(u => u.Email == email))
                {
                    return new AuthResultDto
                    {
                        IsSuccess = false,
                        ErrorMessage = "Пользователь с таким email уже существует"
                    };
                }

                if (_context.Users.Any(u => u.Username == username))
                {
                    return new AuthResultDto
                    {
                        IsSuccess = false,
                        ErrorMessage = "Пользователь с таким именем уже существует"
                    };
                }

                // Создание нового пользователя
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Username = username,
                    Email = email,
                    DisplayName = displayName,
                    PasswordHash = password, // Здесь должно быть хеширование
                    RegistrationDate = DateTime.Now,
                    LastLoginDate = DateTime.Now,
                    IsActive = true,
                    IsEmailConfirmed = false
                };

                _context.Users.Add(user);

                // Назначение роли по умолчанию
                var userRole = _context.Roles.FirstOrDefault(r => r.Name == "User");
                if (userRole != null)
                {
                    _context.UserRoles.Add(new UserRole
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        RoleId = userRole.Id
                    });
                }

                SaveChanges();

                return new AuthResultDto
                {
                    IsSuccess = true,
                    UserId = user.Id,
                    Email = user.Email,
                    UserRole = "User"
                };
            }
            catch (Exception ex)
            {
                return new AuthResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = $"Ошибка регистрации: {ex.Message}"
                };
            }
        }

        public SignOutResultDto SignOut()
        {
            try
            {
                var expiredCookie = new HttpCookie(FormsAuthentication.FormsCookieName, "")
                {
                    Expires = DateTime.Now.AddDays(-1),
                    HttpOnly = true
                };

                return new SignOutResultDto
                {
                    IsSuccess = true,
                    Message = "Вы успешно вышли из системы",
                    ExpiredCookie = expiredCookie,
                    RedirectUrl = "/"
                };
            }
            catch (Exception ex)
            {
                return new SignOutResultDto
                {
                    IsSuccess = false,
                    Message = $"Ошибка выхода: {ex.Message}"
                };
            }
        }

        public UserProfileDto GetUserProfile(Guid userId)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null) return null;

                var roles = GetUserRoles(userId);
                var postsCount = _context.ForumPosts.Count(p => p.AuthorId == userId);
                var topicsCount = _context.ForumTopics.Count(t => t.AuthorId == userId);

                return new UserProfileDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    DisplayName = user.DisplayName,
                    RegistrationDate = user.RegistrationDate,
                    LastLoginDate = user.LastLoginDate,
                    IsActive = user.IsActive,
                    IsEmailConfirmed = user.IsEmailConfirmed,
                    Roles = roles,
                    PostsCount = postsCount,
                    TopicsCount = topicsCount
                };
            }
            catch
            {
                return null;
            }
        }

        public UserProfileDto GetUserProfile(string username)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Username == username);
                return user != null ? GetUserProfile(user.Id) : null;
            }
            catch
            {
                return null;
            }
        }

        public bool UpdateUserProfile(UserProfileDto userDto)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == userDto.Id);
                if (user == null) return false;

                user.DisplayName = userDto.DisplayName;
                user.About = userDto.About;
                user.AvatarUrl = userDto.AvatarUrl;

                SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteUser(Guid userId)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null) return false;

                user.IsActive = false;
                SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string[] GetUserRoles(Guid userId)
        {
            try
            {
                return _context.UserRoles
                    .Where(ur => ur.UserId == userId)
                    .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                    .ToArray();
            }
            catch
            {
                return new string[0];
            }
        }

        public bool IsUserInRole(Guid userId, string roleName)
        {
            return GetUserRoles(userId).Contains(roleName);
        }

        public bool AddUserToRole(Guid userId, string roleName)
        {
            try
            {
                var role = _context.Roles.FirstOrDefault(r => r.Name == roleName);
                if (role == null) return false;

                if (_context.UserRoles.Any(ur => ur.UserId == userId && ur.RoleId == role.Id))
                    return true; // Уже есть

                _context.UserRoles.Add(new UserRole
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    RoleId = role.Id
                });

                SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool RemoveUserFromRole(Guid userId, string roleName)
        {
            try
            {
                var role = _context.Roles.FirstOrDefault(r => r.Name == roleName);
                if (role == null) return false;

                var userRole = _context.UserRoles.FirstOrDefault(ur => ur.UserId == userId && ur.RoleId == role.Id);
                if (userRole == null) return true; // Уже нет

                _context.UserRoles.Remove(userRole);
                SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<UserProfileDto> GetRecentUsers(int count = 10)
        {
            try
            {
                return _context.Users
                    .Where(u => u.IsActive)
                    .OrderByDescending(u => u.RegistrationDate)
                    .Take(count)
                    .Select(u => GetUserProfile(u.Id))
                    .Where(dto => dto != null)
                    .ToList();
            }
            catch
            {
                return new List<UserProfileDto>();
            }
        }

        public List<UserProfileDto> GetActiveUsers(int count = 10)
        {
            try
            {
                return _context.Users
                    .Where(u => u.IsActive)
                    .OrderByDescending(u => u.LastLoginDate)
                    .Take(count)
                    .Select(u => GetUserProfile(u.Id))
                    .Where(dto => dto != null)
                    .ToList();
            }
            catch
            {
                return new List<UserProfileDto>();
            }
        }

        public int GetTotalUsersCount()
        {
            try
            {
                return _context.Users.Count(u => u.IsActive);
            }
            catch
            {
                return 0;
            }
        }

        public List<UserProfileDto> SearchUsers(string searchTerm, int page = 1, int pageSize = 20)
        {
            try
            {
                var skip = (page - 1) * pageSize;
                return _context.Users
                    .Where(u => u.IsActive && 
                               (u.Username.Contains(searchTerm) || 
                                u.DisplayName.Contains(searchTerm) ||
                                u.Email.Contains(searchTerm)))
                    .OrderBy(u => u.Username)
                    .Skip(skip)
                    .Take(pageSize)
                    .Select(u => GetUserProfile(u.Id))
                    .Where(dto => dto != null)
                    .ToList();
            }
            catch
            {
                return new List<UserProfileDto>();
            }
        }
    }
} 