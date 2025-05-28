namespace VinlandSaga.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuditLog",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserId = c.Guid(),
                        UserName = c.String(),
                        Action = c.String(),
                        EntityName = c.String(),
                        EntityId = c.Guid(),
                        OldValues = c.String(),
                        NewValues = c.String(),
                        Timestamp = c.DateTime(nullable: false),
                        IpAddress = c.String(),
                        IsSuccess = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Username = c.String(),
                        Email = c.String(),
                        PasswordHash = c.String(),
                        Salt = c.String(),
                        AvatarUrl = c.String(),
                        DisplayName = c.String(),
                        About = c.String(),
                        RegistrationDate = c.DateTime(nullable: false),
                        LastLoginDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsEmailConfirmed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Comment",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Content = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        LikesCount = c.Int(nullable: false),
                        UserId = c.Guid(nullable: false),
                        ParentCommentId = c.Guid(),
                        FanartId = c.Guid(),
                        ForumPostId = c.Guid(),
                        CharacterId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Character", t => t.CharacterId)
                .ForeignKey("dbo.ForumPost", t => t.ForumPostId)
                .ForeignKey("dbo.Fanart", t => t.FanartId)
                .ForeignKey("dbo.Comment", t => t.ParentCommentId)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.ParentCommentId)
                .Index(t => t.FanartId)
                .Index(t => t.ForumPostId)
                .Index(t => t.CharacterId);
            
            CreateTable(
                "dbo.Character",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        JapaneseName = c.String(),
                        Description = c.String(),
                        Biography = c.String(),
                        ImageUrl = c.String(),
                        Alias = c.String(),
                        Age = c.String(),
                        Gender = c.String(),
                        Occupation = c.String(),
                        Affiliation = c.String(),
                        FirstAppearance = c.String(),
                        Status = c.String(),
                        PopularityRank = c.Int(nullable: false),
                        IsFeatured = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CharacterRelationship",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SourceCharacterId = c.Guid(nullable: false),
                        TargetCharacterId = c.Guid(nullable: false),
                        RelationType = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Character", t => t.SourceCharacterId)
                .ForeignKey("dbo.Character", t => t.TargetCharacterId)
                .Index(t => t.SourceCharacterId)
                .Index(t => t.TargetCharacterId);
            
            CreateTable(
                "dbo.Tag",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Slug = c.String(),
                        UsageCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Fanart",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                        ImageUrl = c.String(),
                        ThumbnailUrl = c.String(),
                        UploadDate = c.DateTime(nullable: false),
                        LikesCount = c.Int(nullable: false),
                        IsApproved = c.Boolean(nullable: false),
                        IsFeatured = c.Boolean(nullable: false),
                        UserId = c.Guid(nullable: false),
                        CategoryId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Category", t => t.CategoryId)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.Category",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        ImageUrl = c.String(),
                        Slug = c.String(),
                        SortOrder = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        ParentCategoryId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Category", t => t.ParentCategoryId)
                .Index(t => t.ParentCategoryId);
            
            CreateTable(
                "dbo.ForumTopic",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastActivityDate = c.DateTime(),
                        ViewsCount = c.Int(nullable: false),
                        IsPinned = c.Boolean(nullable: false),
                        IsClosed = c.Boolean(nullable: false),
                        UserId = c.Guid(nullable: false),
                        CategoryId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Category", t => t.CategoryId)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.ForumPost",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Content = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        LikesCount = c.Int(nullable: false),
                        UserId = c.Guid(nullable: false),
                        ForumTopicId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ForumTopic", t => t.ForumTopicId)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.ForumTopicId);
            
            CreateTable(
                "dbo.UserRole",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserId = c.Guid(nullable: false),
                        RoleId = c.Guid(nullable: false),
                        AssignedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Role", t => t.RoleId)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Role",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RolePermission",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        RoleId = c.Guid(nullable: false),
                        PermissionId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Permission", t => t.PermissionId)
                .ForeignKey("dbo.Role", t => t.RoleId)
                .Index(t => t.RoleId)
                .Index(t => t.PermissionId);
            
            CreateTable(
                "dbo.Permission",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        SystemName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MediaCharacter",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        MediaId = c.Guid(nullable: false),
                        CharacterId = c.Guid(nullable: false),
                        Role = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Character", t => t.CharacterId)
                .ForeignKey("dbo.Media", t => t.MediaId)
                .Index(t => t.MediaId)
                .Index(t => t.CharacterId);
            
            CreateTable(
                "dbo.Media",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                        Type = c.String(),
                        Season = c.String(),
                        Episodes = c.String(),
                        Chapters = c.String(),
                        Status = c.String(),
                        ReleaseDate = c.DateTime(),
                        Studio = c.String(),
                        Director = c.String(),
                        Author = c.String(),
                        ImageUrl = c.String(),
                        ExternalLink = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TagCharacter",
                c => new
                    {
                        Tag_Id = c.Guid(nullable: false),
                        Character_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Tag_Id, t.Character_Id })
                .ForeignKey("dbo.Tag", t => t.Tag_Id, cascadeDelete: true)
                .ForeignKey("dbo.Character", t => t.Character_Id, cascadeDelete: true)
                .Index(t => t.Tag_Id)
                .Index(t => t.Character_Id);
            
            CreateTable(
                "dbo.ForumTopicTag",
                c => new
                    {
                        ForumTopic_Id = c.Guid(nullable: false),
                        Tag_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.ForumTopic_Id, t.Tag_Id })
                .ForeignKey("dbo.ForumTopic", t => t.ForumTopic_Id, cascadeDelete: true)
                .ForeignKey("dbo.Tag", t => t.Tag_Id, cascadeDelete: true)
                .Index(t => t.ForumTopic_Id)
                .Index(t => t.Tag_Id);
            
            CreateTable(
                "dbo.FanartTag",
                c => new
                    {
                        Fanart_Id = c.Guid(nullable: false),
                        Tag_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Fanart_Id, t.Tag_Id })
                .ForeignKey("dbo.Fanart", t => t.Fanart_Id, cascadeDelete: true)
                .ForeignKey("dbo.Tag", t => t.Tag_Id, cascadeDelete: true)
                .Index(t => t.Fanart_Id)
                .Index(t => t.Tag_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MediaCharacter", "MediaId", "dbo.Media");
            DropForeignKey("dbo.MediaCharacter", "CharacterId", "dbo.Character");
            DropForeignKey("dbo.AuditLog", "UserId", "dbo.User");
            DropForeignKey("dbo.UserRole", "UserId", "dbo.User");
            DropForeignKey("dbo.UserRole", "RoleId", "dbo.Role");
            DropForeignKey("dbo.RolePermission", "RoleId", "dbo.Role");
            DropForeignKey("dbo.RolePermission", "PermissionId", "dbo.Permission");
            DropForeignKey("dbo.ForumPost", "UserId", "dbo.User");
            DropForeignKey("dbo.Fanart", "UserId", "dbo.User");
            DropForeignKey("dbo.Comment", "UserId", "dbo.User");
            DropForeignKey("dbo.Comment", "ParentCommentId", "dbo.Comment");
            DropForeignKey("dbo.FanartTag", "Tag_Id", "dbo.Tag");
            DropForeignKey("dbo.FanartTag", "Fanart_Id", "dbo.Fanart");
            DropForeignKey("dbo.Comment", "FanartId", "dbo.Fanart");
            DropForeignKey("dbo.Category", "ParentCategoryId", "dbo.Category");
            DropForeignKey("dbo.ForumTopic", "UserId", "dbo.User");
            DropForeignKey("dbo.ForumTopicTag", "Tag_Id", "dbo.Tag");
            DropForeignKey("dbo.ForumTopicTag", "ForumTopic_Id", "dbo.ForumTopic");
            DropForeignKey("dbo.ForumPost", "ForumTopicId", "dbo.ForumTopic");
            DropForeignKey("dbo.Comment", "ForumPostId", "dbo.ForumPost");
            DropForeignKey("dbo.ForumTopic", "CategoryId", "dbo.Category");
            DropForeignKey("dbo.Fanart", "CategoryId", "dbo.Category");
            DropForeignKey("dbo.TagCharacter", "Character_Id", "dbo.Character");
            DropForeignKey("dbo.TagCharacter", "Tag_Id", "dbo.Tag");
            DropForeignKey("dbo.CharacterRelationship", "TargetCharacterId", "dbo.Character");
            DropForeignKey("dbo.CharacterRelationship", "SourceCharacterId", "dbo.Character");
            DropForeignKey("dbo.Comment", "CharacterId", "dbo.Character");
            DropIndex("dbo.FanartTag", new[] { "Tag_Id" });
            DropIndex("dbo.FanartTag", new[] { "Fanart_Id" });
            DropIndex("dbo.ForumTopicTag", new[] { "Tag_Id" });
            DropIndex("dbo.ForumTopicTag", new[] { "ForumTopic_Id" });
            DropIndex("dbo.TagCharacter", new[] { "Character_Id" });
            DropIndex("dbo.TagCharacter", new[] { "Tag_Id" });
            DropIndex("dbo.MediaCharacter", new[] { "CharacterId" });
            DropIndex("dbo.MediaCharacter", new[] { "MediaId" });
            DropIndex("dbo.RolePermission", new[] { "PermissionId" });
            DropIndex("dbo.RolePermission", new[] { "RoleId" });
            DropIndex("dbo.UserRole", new[] { "RoleId" });
            DropIndex("dbo.UserRole", new[] { "UserId" });
            DropIndex("dbo.ForumPost", new[] { "ForumTopicId" });
            DropIndex("dbo.ForumPost", new[] { "UserId" });
            DropIndex("dbo.ForumTopic", new[] { "CategoryId" });
            DropIndex("dbo.ForumTopic", new[] { "UserId" });
            DropIndex("dbo.Category", new[] { "ParentCategoryId" });
            DropIndex("dbo.Fanart", new[] { "CategoryId" });
            DropIndex("dbo.Fanart", new[] { "UserId" });
            DropIndex("dbo.CharacterRelationship", new[] { "TargetCharacterId" });
            DropIndex("dbo.CharacterRelationship", new[] { "SourceCharacterId" });
            DropIndex("dbo.Comment", new[] { "CharacterId" });
            DropIndex("dbo.Comment", new[] { "ForumPostId" });
            DropIndex("dbo.Comment", new[] { "FanartId" });
            DropIndex("dbo.Comment", new[] { "ParentCommentId" });
            DropIndex("dbo.Comment", new[] { "UserId" });
            DropIndex("dbo.AuditLog", new[] { "UserId" });
            DropTable("dbo.FanartTag");
            DropTable("dbo.ForumTopicTag");
            DropTable("dbo.TagCharacter");
            DropTable("dbo.Media");
            DropTable("dbo.MediaCharacter");
            DropTable("dbo.Permission");
            DropTable("dbo.RolePermission");
            DropTable("dbo.Role");
            DropTable("dbo.UserRole");
            DropTable("dbo.ForumPost");
            DropTable("dbo.ForumTopic");
            DropTable("dbo.Category");
            DropTable("dbo.Fanart");
            DropTable("dbo.Tag");
            DropTable("dbo.CharacterRelationship");
            DropTable("dbo.Character");
            DropTable("dbo.Comment");
            DropTable("dbo.User");
            DropTable("dbo.AuditLog");
        }
    }
}
