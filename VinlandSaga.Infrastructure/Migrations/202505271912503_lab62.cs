namespace VinlandSaga.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class lab62 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.News",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(),
                        Content = c.String(),
                        Summary = c.String(),
                        ImageUrl = c.String(),
                        PublishedDate = c.DateTime(nullable: false),
                        AuthorId = c.Guid(nullable: false),
                        IsPublished = c.Boolean(nullable: false),
                        ViewCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.AuthorId)
                .Index(t => t.AuthorId);
            
            AddColumn("dbo.Comment", "NewsId", c => c.Guid());
            CreateIndex("dbo.Comment", "NewsId");
            AddForeignKey("dbo.Comment", "NewsId", "dbo.News", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Comment", "NewsId", "dbo.News");
            DropForeignKey("dbo.News", "AuthorId", "dbo.User");
            DropIndex("dbo.News", new[] { "AuthorId" });
            DropIndex("dbo.Comment", new[] { "NewsId" });
            DropColumn("dbo.Comment", "NewsId");
            DropTable("dbo.News");
        }
    }
}
