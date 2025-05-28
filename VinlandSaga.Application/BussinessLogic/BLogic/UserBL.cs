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
                var allUsers = GetAll<User>();
                var user = allUsers.FirstOrDefault(u => u.Email == email && u.IsActive);
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
                Update(user);

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
                var allUsers = GetAll<User>();
                
                // Проверка существования пользователя
                if (allUsers.Any(u => u.Email == email))
                {
                    return new AuthResultDto
                    {
                        IsSuccess = false,
                        ErrorMessage = "Пользователь с таким email уже существует"
                    };
                }

                if (allUsers.Any(u => u.Username == username))
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

                var userCreated = Create(user);
                if (!userCreated)
                {
                    return new AuthResultDto
                    {
                        IsSuccess = false,
                        ErrorMessage = "Ошибка создания пользователя"
                    };
                }

                // Назначение роли по умолчанию
                var allRoles = GetAll<Role>();
                var userRole = allRoles.FirstOrDefault(r => r.Name == "User");
                if (userRole != null)
                {
                    Create(new UserRole
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        RoleId = userRole.Id
                    });
                }

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
            var user = GetById<User>(userId);
            if (user == null) return null;

            try
            {
                var roles = GetUserRoles(userId);
                var allPosts = GetAll<ForumPost>();
                var allTopics = GetAll<ForumTopic>();
                
                var postsCount = allPosts.Count(p => p.AuthorId == userId);
                var topicsCount = allTopics.Count(t => t.AuthorId == userId);

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
                var allUsers = GetAll<User>();
                var user = allUsers.FirstOrDefault(u => u.Username == username);
                return user != null ? GetUserProfile(user.Id) : null;
            }
            catch
            {
                return null;
            }
        }

        public bool UpdateUserProfile(UserProfileDto userDto)
        {
            var user = GetById<User>(userDto.Id);
            if (user == null) return false;

            try
            {
                user.DisplayName = userDto.DisplayName;
                user.About = userDto.About;
                user.AvatarUrl = userDto.AvatarUrl;

                return Update(user);
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteUser(Guid userId)
        {
            var user = GetById<User>(userId);
            if (user == null) return false;

            try
            {
                user.IsActive = false;
                return Update(user);
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
                var allUserRoles = GetAll<UserRole>();
                var allRoles = GetAll<Role>();
                
                var userRoleIds = allUserRoles
                    .Where(ur => ur.UserId == userId)
                    .Select(ur => ur.RoleId)
                    .ToList();
                
                return allRoles
                    .Where(r => userRoleIds.Contains(r.Id))
                    .Select(r => r.Name)
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

                var userRole = new UserRole
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    RoleId = role.Id
                };

                return Create(userRole);
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
                var allRoles = GetAll<Role>();
                var role = allRoles.FirstOrDefault(r => r.Name == roleName);
                if (role == null) return false;

                var allUserRoles = GetAll<UserRole>();
                var userRole = allUserRoles.FirstOrDefault(ur => ur.UserId == userId && ur.RoleId == role.Id);
                if (userRole == null) return true; // Уже нет

                return Delete<UserRole>(userRole.Id);
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
                var allUsers = GetAll<User>();
                return allUsers
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
                var allUsers = GetAll<User>();
                return allUsers
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
            return Count<User>();
        }

        public List<UserProfileDto> SearchUsers(string searchTerm, int page = 1, int pageSize = 20)
        {
            try
            {
                var allUsers = GetAll<User>();
                var skip = (page - 1) * pageSize;
                return allUsers
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