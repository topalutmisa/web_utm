using System;
using System.Collections.Generic;
using System.Data.Entity;
using VinlandSaga.Domain.Models;

namespace VinlandSaga.Infrastructure.Data
{
    
    
    
    public class VinlandSagaDbInitializer : DropCreateDatabaseIfModelChanges<VinlandSagaDbContext>
    {
        protected override void Seed(VinlandSagaDbContext context)
        {
            
            var roles = new List<Role>
            {
                new Role 
                { 
                    Id = Guid.NewGuid(),
                    Name = "ÐÐ´Ð¼Ð¸Ð½Ð¸ÑÑ‚Ñ€Ð°Ñ‚Ð¾Ñ€",
                    Description = "ÐŸÐ¾Ð»Ð½Ñ‹Ð¹ Ð´Ð¾ÑÑ‚ÑƒÐ¿ ÐºÐ¾ Ð²ÑÐµÐ¼ Ñ„ÑƒÐ½ÐºÑ†Ð¸ÑÐ¼ ÑÐ°Ð¹Ñ‚Ð°"
                },
                new Role 
                { 
                    Id = Guid.NewGuid(),
                    Name = "ÐœÐ¾Ð´ÐµÑ€Ð°Ñ‚Ð¾Ñ€",
                    Description = "Ð£Ð¿Ñ€Ð°Ð²Ð»ÐµÐ½Ð¸Ðµ ÐºÐ¾Ð½Ñ‚ÐµÐ½Ñ‚Ð¾Ð¼ Ð¸ Ð¿Ð¾Ð»ÑŒÐ·Ð¾Ð²Ð°Ñ‚ÐµÐ»ÑÐ¼Ð¸"
                },
                new Role 
                { 
                    Id = Guid.NewGuid(),
                    Name = "ÐŸÐ¾Ð»ÑŒÐ·Ð¾Ð²Ð°Ñ‚ÐµÐ»ÑŒ",
                    Description = "Ð¡Ñ‚Ð°Ð½Ð´Ð°Ñ€Ñ‚Ð½Ñ‹Ð¹ Ð´Ð¾ÑÑ‚ÑƒÐ¿ Ðº ÑÐ°Ð¹Ñ‚Ñƒ"
                },
                new Role 
                { 
                    Id = Guid.NewGuid(),
                    Name = "Ð¥ÑƒÐ´Ð¾Ð¶Ð½Ð¸Ðº",
                    Description = "ÐŸÐ¾Ð´Ñ‚Ð²ÐµÑ€Ð¶Ð´ÐµÐ½Ð½Ñ‹Ð¹ ÑÑ‚Ð°Ñ‚ÑƒÑ Ñ…ÑƒÐ´Ð¾Ð¶Ð½Ð¸ÐºÐ° Ð´Ð»Ñ Ð·Ð°Ð³Ñ€ÑƒÐ·ÐºÐ¸ Ñ„Ð°Ð½-Ð°Ñ€Ñ‚Ð°"
                }
            };
            
            context.Roles.AddRange(roles);
            
            
            var permissions = new List<Permission>
            {
                new Permission 
                { 
                    Id = Guid.NewGuid(),
                    Name = "Ð£Ð¿Ñ€Ð°Ð²Ð»ÐµÐ½Ð¸Ðµ Ð¿Ð¾Ð»ÑŒÐ·Ð¾Ð²Ð°Ñ‚ÐµÐ»ÑÐ¼Ð¸",
                    Description = "Ð¡Ð¾Ð·Ð´Ð°Ð½Ð¸Ðµ, Ñ€ÐµÐ´Ð°ÐºÑ‚Ð¸Ñ€Ð¾Ð²Ð°Ð½Ð¸Ðµ Ð¸ ÑƒÐ´Ð°Ð»ÐµÐ½Ð¸Ðµ Ð¿Ð¾Ð»ÑŒÐ·Ð¾Ð²Ð°Ñ‚ÐµÐ»ÐµÐ¹",
                    SystemName = "users_manage"
                },
                new Permission 
                { 
                    Id = Guid.NewGuid(),
                    Name = "Ð£Ð¿Ñ€Ð°Ð²Ð»ÐµÐ½Ð¸Ðµ Ñ€Ð¾Ð»ÑÐ¼Ð¸",
                    Description = "Ð¡Ð¾Ð·Ð´Ð°Ð½Ð¸Ðµ, Ñ€ÐµÐ´Ð°ÐºÑ‚Ð¸Ñ€Ð¾Ð²Ð°Ð½Ð¸Ðµ Ð¸ ÑƒÐ´Ð°Ð»ÐµÐ½Ð¸Ðµ Ñ€Ð¾Ð»ÐµÐ¹",
                    SystemName = "roles_manage"
                },
                new Permission 
                { 
                    Id = Guid.NewGuid(),
                    Name = "ÐœÐ¾Ð´ÐµÑ€Ð°Ñ†Ð¸Ñ Ñ„Ð¾Ñ€ÑƒÐ¼Ð°",
                    Description = "ÐœÐ¾Ð´ÐµÑ€Ð°Ñ†Ð¸Ñ ÑÐ¾Ð¾Ð±Ñ‰ÐµÐ½Ð¸Ð¹ Ñ„Ð¾Ñ€ÑƒÐ¼Ð°",
                    SystemName = "forum_moderate"
                },
                new Permission 
                { 
                    Id = Guid.NewGuid(),
                    Name = "ÐœÐ¾Ð´ÐµÑ€Ð°Ñ†Ð¸Ñ Ñ„Ð°Ð½-Ð°Ñ€Ñ‚Ð°",
                    Description = "ÐœÐ¾Ð´ÐµÑ€Ð°Ñ†Ð¸Ñ Ð·Ð°Ð³Ñ€ÑƒÐ·Ð¾Ðº Ñ„Ð°Ð½-Ð°Ñ€Ñ‚Ð°",
                    SystemName = "fanart_moderate"
                },
                new Permission 
                { 
                    Id = Guid.NewGuid(),
                    Name = "ÐŸÑƒÐ±Ð»Ð¸ÐºÐ°Ñ†Ð¸Ñ Ð½Ð¾Ð²Ð¾ÑÑ‚ÐµÐ¹",
                    Description = "Ð¡Ð¾Ð·Ð´Ð°Ð½Ð¸Ðµ Ð¸ Ñ€ÐµÐ´Ð°ÐºÑ‚Ð¸Ñ€Ð¾Ð²Ð°Ð½Ð¸Ðµ Ð½Ð¾Ð²Ð¾ÑÑ‚ÐµÐ¹",
                    SystemName = "news_publish"
                }
            };
            
            context.Permissions.AddRange(permissions);
            
            
            var adminRole = roles[0]; 
            var moderatorRole = roles[1]; 
            
            foreach (var permission in permissions)
            {
                
                context.RolePermissions.Add(new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = adminRole.Id,
                    PermissionId = permission.Id
                });
                
                
                if (permission.SystemName == "forum_moderate" || 
                    permission.SystemName == "fanart_moderate" ||
                    permission.SystemName == "news_publish")
                {
                    context.RolePermissions.Add(new RolePermission
                    {
                        Id = Guid.NewGuid(),
                        RoleId = moderatorRole.Id,
                        PermissionId = permission.Id
                    });
                }
            }
            
            
            var adminSalt = Guid.NewGuid().ToString();
            var adminPasswordHash = HashPassword("admin123", adminSalt);
            
            var adminUser = new User
            {
                Id = Guid.NewGuid(),
                Username = "admin",
                Email = "admin@vinlandsaga.com",
                PasswordHash = adminPasswordHash,
                Salt = adminSalt,
                DisplayName = "ÐÐ´Ð¼Ð¸Ð½Ð¸ÑÑ‚Ñ€Ð°Ñ‚Ð¾Ñ€",
                RegistrationDate = DateTime.Now,
                LastLoginDate = DateTime.Now,
                IsActive = true,
                IsEmailConfirmed = true
            };
            
            context.Users.Add(adminUser);
            
            
            context.UserRoles.Add(new UserRole
            {
                Id = Guid.NewGuid(),
                UserId = adminUser.Id,
                RoleId = adminRole.Id,
                AssignedDate = DateTime.Now
            });
            
            
            var categories = new List<Category>
            {
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Ð¦Ð¸Ñ„Ñ€Ð¾Ð²Ð¾Ð¹ Ð°Ñ€Ñ‚",
                    Description = "ÐÑ€Ñ‚, ÑÐ¾Ð·Ð´Ð°Ð½Ð½Ñ‹Ð¹ Ñ Ð¿Ð¾Ð¼Ð¾Ñ‰ÑŒÑŽ Ñ†Ð¸Ñ„Ñ€Ð¾Ð²Ñ‹Ñ… Ð¸Ð½ÑÑ‚Ñ€ÑƒÐ¼ÐµÐ½Ñ‚Ð¾Ð²",
                    Slug = "digital-art",
                    SortOrder = 1,
                    IsActive = true
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Ð¢Ñ€Ð°Ð´Ð¸Ñ†Ð¸Ð¾Ð½Ð½Ñ‹Ð¹ Ð°Ñ€Ñ‚",
                    Description = "ÐÑ€Ñ‚, ÑÐ¾Ð·Ð´Ð°Ð½Ð½Ñ‹Ð¹ Ñ‚Ñ€Ð°Ð´Ð¸Ñ†Ð¸Ð¾Ð½Ð½Ñ‹Ð¼Ð¸ Ð¼ÐµÑ‚Ð¾Ð´Ð°Ð¼Ð¸",
                    Slug = "traditional-art",
                    SortOrder = 2,
                    IsActive = true
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "ÐšÐ¾ÑÐ¿Ð»ÐµÐ¹",
                    Description = "ÐšÐ¾ÑÐ¿Ð»ÐµÐ¹ Ð¿Ð¾ Ð¼Ð¾Ñ‚Ð¸Ð²Ð°Ð¼ Vinland Saga",
                    Slug = "cosplay",
                    SortOrder = 3,
                    IsActive = true
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "ÐžÐ±ÑÑƒÐ¶Ð´ÐµÐ½Ð¸Ðµ Ð¼Ð°Ð½Ð³Ð¸",
                    Description = "ÐžÐ±ÑÑƒÐ¶Ð´ÐµÐ½Ð¸Ðµ Ð¼Ð°Ð½Ð³Ð¸ Vinland Saga",
                    Slug = "manga-discussion",
                    SortOrder = 1,
                    IsActive = true
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "ÐžÐ±ÑÑƒÐ¶Ð´ÐµÐ½Ð¸Ðµ Ð°Ð½Ð¸Ð¼Ðµ",
                    Description = "ÐžÐ±ÑÑƒÐ¶Ð´ÐµÐ½Ð¸Ðµ Ð°Ð½Ð¸Ð¼Ðµ Vinland Saga",
                    Slug = "anime-discussion",
                    SortOrder = 2,
                    IsActive = true
                }
            };
            
            context.Categories.AddRange(categories);
            
            
            context.SaveChanges();
        }
        
        private string HashPassword(string password, string salt)
        {
            
            
            return Convert.ToBase64String(
                System.Security.Cryptography.SHA256.Create()
                .ComputeHash(System.Text.Encoding.UTF8.GetBytes(password + salt))
            );
        }
    }
} 
