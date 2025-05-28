namespace VinlandSaga.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class lab622 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ForumTopic", "CategoryId", "dbo.Category");
            RenameColumn(table: "dbo.ForumPost", name: "ForumTopicId", newName: "TopicId");
            RenameIndex(table: "dbo.ForumPost", name: "IX_ForumTopicId", newName: "IX_TopicId");
            AddColumn("dbo.ForumTopic", "LastPostDate", c => c.DateTime());
            AddColumn("dbo.ForumTopic", "PostsCount", c => c.Int(nullable: false));
            AddColumn("dbo.ForumTopic", "IsLocked", c => c.Boolean(nullable: false));
            AddColumn("dbo.ForumTopic", "Category_Id", c => c.Guid());
            CreateIndex("dbo.ForumTopic", "Category_Id");
            AddForeignKey("dbo.ForumTopic", "Category_Id", "dbo.Category", "Id");
            DropColumn("dbo.ForumTopic", "LastActivityDate");
            DropColumn("dbo.ForumTopic", "IsClosed");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ForumTopic", "IsClosed", c => c.Boolean(nullable: false));
            AddColumn("dbo.ForumTopic", "LastActivityDate", c => c.DateTime());
            DropForeignKey("dbo.ForumTopic", "Category_Id", "dbo.Category");
            DropIndex("dbo.ForumTopic", new[] { "Category_Id" });
            DropColumn("dbo.ForumTopic", "Category_Id");
            DropColumn("dbo.ForumTopic", "IsLocked");
            DropColumn("dbo.ForumTopic", "PostsCount");
            DropColumn("dbo.ForumTopic", "LastPostDate");
            RenameIndex(table: "dbo.ForumPost", name: "IX_TopicId", newName: "IX_ForumTopicId");
            RenameColumn(table: "dbo.ForumPost", name: "TopicId", newName: "ForumTopicId");
            AddForeignKey("dbo.ForumTopic", "CategoryId", "dbo.Category", "Id");
        }
    }
}
