using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Security.Cryptography;
using System.Text;
using VinlandSaga.Domain.Models;

namespace VinlandSaga.Infrastructure.Data
{
    public class VinlandSagaDbContext : DbContext
    {
        public VinlandSagaDbContext()
            : base("name=VinlandSagaDb")
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }
        
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Fanart> Fanarts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<ForumTopic> ForumTopics { get; set; }
        public DbSet<ForumPost> ForumPosts { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<CharacterRelationship> CharacterRelationships { get; set; }
        public DbSet<Media> Medias { get; set; }
        public DbSet<MediaCharacter> MediaCharacters { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Удаление конвенций
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            
            // Настройка отношений
            modelBuilder.Entity<User>()
                .HasMany(u => u.UserRoles)
                .WithRequired(ur => ur.User)
                .HasForeignKey(ur => ur.UserId);
                
            modelBuilder.Entity<User>()
                .HasMany(u => u.Comments)
                .WithRequired(c => c.User)
                .HasForeignKey(c => c.UserId);
                
            modelBuilder.Entity<User>()
                .HasMany(u => u.Fanarts)
                .WithRequired(f => f.User)
                .HasForeignKey(f => f.UserId);
                
            modelBuilder.Entity<User>()
                .HasMany(u => u.ForumPosts)
                .WithRequired(fp => fp.User)
                .HasForeignKey(fp => fp.UserId);
            
            modelBuilder.Entity<Role>()
                .HasMany(r => r.UserRoles)
                .WithRequired(ur => ur.Role)
                .HasForeignKey(ur => ur.RoleId);
                
            modelBuilder.Entity<Role>()
                .HasMany(r => r.RolePermissions)
                .WithRequired(rp => rp.Role)
                .HasForeignKey(rp => rp.RoleId);
            
            modelBuilder.Entity<Permission>()
                .HasMany(p => p.RolePermissions)
                .WithRequired(rp => rp.Permission)
                .HasForeignKey(rp => rp.PermissionId);
            
            modelBuilder.Entity<Category>()
                .HasOptional(c => c.ParentCategory)
                .WithMany(c => c.ChildCategories)
                .HasForeignKey(c => c.ParentCategoryId);
            
            modelBuilder.Entity<Comment>()
                .HasOptional(c => c.ParentComment)
                .WithMany(c => c.ChildComments)
                .HasForeignKey(c => c.ParentCommentId);
            
            modelBuilder.Entity<CharacterRelationship>()
                .HasRequired(cr => cr.SourceCharacter)
                .WithMany(c => c.SourceRelationships)
                .HasForeignKey(cr => cr.SourceCharacterId)
                .WillCascadeOnDelete(false);
                
            modelBuilder.Entity<CharacterRelationship>()
                .HasRequired(cr => cr.TargetCharacter)
                .WithMany(c => c.TargetRelationships)
                .HasForeignKey(cr => cr.TargetCharacterId)
                .WillCascadeOnDelete(false);
            
            // Здесь добавление сид-данных с помощью Database.SetInitializer и DbInitializer
            Database.SetInitializer(new VinlandSagaDbInitializer());
            
            base.OnModelCreating(modelBuilder);
        }
        
        // Внутренний класс для инициализации данных
        private class VinlandSagaDbInitializer : CreateDatabaseIfNotExists<VinlandSagaDbContext>
        {
            protected override void Seed(VinlandSagaDbContext context)
            {
                // Роли
                var adminRoleId = Guid.NewGuid();
                var moderatorRoleId = Guid.NewGuid();
                var userRoleId = Guid.NewGuid();
                var artistRoleId = Guid.NewGuid();
                
                var roles = new List<Role>
                {
                    new Role 
                    { 
                        Id = adminRoleId,
                        Name = "Administrator",
                        Description = "Полный доступ ко всем функциям сайта"
                    },
                    new Role 
                    { 
                        Id = moderatorRoleId,
                        Name = "Moderator",
                        Description = "Управление контентом и пользователями"
                    },
                    new Role 
                    { 
                        Id = userRoleId,
                        Name = "User",
                        Description = "Стандартный доступ к сайту"
                    },
                    new Role 
                    { 
                        Id = artistRoleId,
                        Name = "Artist",
                        Description = "Подтвержденный статус художника для загрузки фан-арта"
                    }
                };
                
                context.Roles.AddRange(roles);
                
                // Разрешения
                var usersManageId = Guid.NewGuid();
                var rolesManageId = Guid.NewGuid();
                var forumModerateId = Guid.NewGuid();
                var fanartModerateId = Guid.NewGuid();
                var newsPublishId = Guid.NewGuid();
                
                var permissions = new List<Permission>
                {
                    new Permission 
                    { 
                        Id = usersManageId,
                        Name = "UsersManagement",
                        Description = "Создание, редактирование и удаление пользователей",
                        SystemName = "users_manage"
                    },
                    new Permission 
                    { 
                        Id = rolesManageId,
                        Name = "RolesManagement",
                        Description = "Создание, редактирование и удаление ролей",
                        SystemName = "roles_manage"
                    },
                    new Permission 
                    { 
                        Id = forumModerateId,
                        Name = "ForumModeration",
                        Description = "Модерация сообщений форума",
                        SystemName = "forum_moderate"
                    },
                    new Permission 
                    { 
                        Id = fanartModerateId,
                        Name = "FanartModeration",
                        Description = "Модерация загрузок фан-арта",
                        SystemName = "fanart_moderate"
                    },
                    new Permission 
                    { 
                        Id = newsPublishId,
                        Name = "NewsPublication",
                        Description = "Создание и редактирование новостей",
                        SystemName = "news_publish"
                    }
                };
                
                context.Permissions.AddRange(permissions);
                
                // Связи ролей и разрешений
                var rolePermissions = new List<RolePermission>();
                
                // Все разрешения для администратора
                foreach (var permission in permissions)
                {
                    rolePermissions.Add(new RolePermission
                    {
                        Id = Guid.NewGuid(),
                        RoleId = adminRoleId,
                        PermissionId = permission.Id
                    });
                }
                
                // Некоторые разрешения для модератора
                rolePermissions.Add(new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = moderatorRoleId,
                    PermissionId = forumModerateId
                });
                
                rolePermissions.Add(new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = moderatorRoleId,
                    PermissionId = fanartModerateId
                });
                
                rolePermissions.Add(new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = moderatorRoleId,
                    PermissionId = newsPublishId
                });
                
                context.RolePermissions.AddRange(rolePermissions);
                
                // Создание администратора
                var adminSalt = Guid.NewGuid().ToString();
                var adminPasswordHash = HashPassword("admin123", adminSalt);
                var adminId = Guid.NewGuid();
                
                var adminUser = new User
                {
                    Id = adminId,
                    Username = "admin",
                    Email = "admin@vinlandsaga.com",
                    PasswordHash = adminPasswordHash,
                    Salt = adminSalt,
                    DisplayName = "Administrator",
                    RegistrationDate = DateTime.Now,
                    LastLoginDate = DateTime.Now,
                    IsActive = true,
                    IsEmailConfirmed = true
                };
                
                context.Users.Add(adminUser);
                
                // Связь администратора с ролью
                context.UserRoles.Add(new UserRole
                {
                    Id = Guid.NewGuid(),
                    UserId = adminId,
                    RoleId = adminRoleId,
                    AssignedDate = DateTime.Now
                });
                
                // Категории
                var categories = new List<Category>
                {
                    new Category
                    {
                        Id = Guid.NewGuid(),
                        Name = "Digital Art",
                        Description = "Арт, созданный с помощью цифровых инструментов",
                        Slug = "digital-art",
                        SortOrder = 1,
                        IsActive = true
                    },
                    new Category
                    {
                        Id = Guid.NewGuid(),
                        Name = "Traditional Art",
                        Description = "Арт, созданный традиционными методами",
                        Slug = "traditional-art",
                        SortOrder = 2,
                        IsActive = true
                    },
                    new Category
                    {
                        Id = Guid.NewGuid(),
                        Name = "Cosplay",
                        Description = "Косплей по мотивам Vinland Saga",
                        Slug = "cosplay",
                        SortOrder = 3,
                        IsActive = true
                    },
                    new Category
                    {
                        Id = Guid.NewGuid(),
                        Name = "Manga Discussion",
                        Description = "Обсуждение манги Vinland Saga",
                        Slug = "manga-discussion",
                        SortOrder = 1,
                        IsActive = true
                    },
                    new Category
                    {
                        Id = Guid.NewGuid(),
                        Name = "Anime Discussion",
                        Description = "Обсуждение аниме Vinland Saga",
                        Slug = "anime-discussion",
                        SortOrder = 2,
                        IsActive = true
                    }
                };
                
                context.Categories.AddRange(categories);
                
                context.SaveChanges();
                
                base.Seed(context);
            }
        }
        
        private static string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var combinedBytes = Encoding.UTF8.GetBytes(password + salt);
                var hashBytes = sha256.ComputeHash(combinedBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
} 
