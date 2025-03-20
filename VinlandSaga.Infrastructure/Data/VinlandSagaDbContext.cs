using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
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
            
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            
            
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            
            
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
                
            base.OnModelCreating(modelBuilder);
        }
    }
} 
