using System;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Security.Cryptography;
using System.Text;
using VinlandSaga.Domain.Models;
using VinlandSaga.Infrastructure.Data;

namespace VinlandSaga.Application.Services
{
    public class AuthService
    {
        private readonly VinlandSagaDbContext _dbContext;

        public AuthService(VinlandSagaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public User RegisterUser(string username, string email, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Username, email and password are required");
            }

            var existingUser = _dbContext.Users.FirstOrDefault(u => u.Email == email || u.Username == username);
            if (existingUser != null)
            {
                throw new InvalidOperationException("User with this email or username already exists");
            }

            var salt = GenerateSalt();
            var passwordHash = HashPassword(password, salt);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = username,
                Email = email,
                PasswordHash = passwordHash,
                Salt = salt,
                RegistrationDate = DateTime.UtcNow,
                LastLoginDate = DateTime.UtcNow,
                IsActive = true,
                IsEmailConfirmed = false
            };

            _dbContext.Users.Add(user);
            
            // Назначаем роль "User" по умолчанию
            var userRole = _dbContext.Roles.FirstOrDefault(r => r.Name == "User");
            if (userRole != null)
            {
                _dbContext.UserRoles.Add(new UserRole
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    RoleId = userRole.Id,
                    AssignedDate = DateTime.UtcNow
                });
            }
            
            _dbContext.SaveChanges();

            return user;
        }

        public User AuthenticateUser(string email, string password)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email && u.IsActive);
            if (user == null)
            {
                return null;
            }

            var passwordHash = HashPassword(password, user.Salt);
            if (passwordHash != user.PasswordHash)
            {
                return null;
            }

            user.LastLoginDate = DateTime.UtcNow;
            _dbContext.SaveChanges();

            SetAuthCookie(user);

            return user;
        }

        public void LogoutUser()
        {
            FormsAuthentication.SignOut();
        }

        private void SetAuthCookie(User user)
        {
            var roles = string.Join(",", _dbContext.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .Select(ur => ur.Role.Name));
                
            var userData = $"{user.Id}|{user.Email}|{roles}";
            
            var ticket = new FormsAuthenticationTicket(
                1,
                user.Email,
                DateTime.Now,
                DateTime.Now.AddHours(12),
                true, 
                userData, 
                FormsAuthentication.FormsCookiePath);

            var encryptedTicket = FormsAuthentication.Encrypt(ticket);
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
            {
                HttpOnly = true,
                Secure = FormsAuthentication.RequireSSL,
                Path = FormsAuthentication.FormsCookiePath
            };
            
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        private string GenerateSalt()
        {
            byte[] randomBytes = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }

        private string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var combinedBytes = Encoding.UTF8.GetBytes(password + salt);
                var hashBytes = sha256.ComputeHash(combinedBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        public User GetCurrentUser()
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return null;
            }

            var ticket = GetAuthTicket();
            if (ticket == null)
            {
                return null;
            }

            var userData = ticket.UserData.Split('|');
            if (userData.Length < 2)
            {
                return null;
            }

            var userId = Guid.Parse(userData[0]);
            return _dbContext.Users.FirstOrDefault(u => u.Id == userId);
        }

        private FormsAuthenticationTicket GetAuthTicket()
        {
            var cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie == null)
            {
                return null;
            }

            try
            {
                return FormsAuthentication.Decrypt(cookie.Value);
            }
            catch
            {
                return null;
            }
        }

        public bool ChangePassword(Guid userId, string currentPassword, string newPassword)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return false;
            }

            var currentPasswordHash = HashPassword(currentPassword, user.Salt);
            if (currentPasswordHash != user.PasswordHash)
            {
                return false;
            }

            var newSalt = GenerateSalt();
            var newPasswordHash = HashPassword(newPassword, newSalt);

            user.Salt = newSalt;
            user.PasswordHash = newPasswordHash;
            _dbContext.SaveChanges();

            return true;
        }
    }
} 