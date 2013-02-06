namespace jamescms.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Original : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TextWalls",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Description = c.String(nullable: false, maxLength: 500),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Texts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 100),
                        Article = c.String(nullable: false),
                        Posted = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CommentText = c.String(nullable: false, maxLength: 2000),
                        ParentCommentId = c.Int(),
                        UserId = c.Int(nullable: false),
                        Posted = c.DateTime(nullable: false),
                        Upvotes = c.Int(nullable: false),
                        Downvotes = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Texts_Tags",
                c => new
                    {
                        TextId = c.Int(nullable: false),
                        TagId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TextId, t.TagId })
                .ForeignKey("dbo.Texts", t => t.TextId, cascadeDelete: true)
                .ForeignKey("dbo.Tags", t => t.TagId, cascadeDelete: true)
                .Index(t => t.TextId)
                .Index(t => t.TagId);
            
            CreateTable(
                "dbo.ChildCommentRelationships",
                c => new
                    {
                        CommentParentId = c.Int(nullable: false),
                        CommentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.CommentParentId, t.CommentId })
                .ForeignKey("dbo.Comments", t => t.CommentParentId)
                .ForeignKey("dbo.Comments", t => t.CommentId)
                .Index(t => t.CommentParentId)
                .Index(t => t.CommentId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.ChildCommentRelationships", new[] { "CommentId" });
            DropIndex("dbo.ChildCommentRelationships", new[] { "CommentParentId" });
            DropIndex("dbo.Texts_Tags", new[] { "TagId" });
            DropIndex("dbo.Texts_Tags", new[] { "TextId" });
            DropForeignKey("dbo.ChildCommentRelationships", "CommentId", "dbo.Comments");
            DropForeignKey("dbo.ChildCommentRelationships", "CommentParentId", "dbo.Comments");
            DropForeignKey("dbo.Texts_Tags", "TagId", "dbo.Tags");
            DropForeignKey("dbo.Texts_Tags", "TextId", "dbo.Texts");
            DropTable("dbo.ChildCommentRelationships");
            DropTable("dbo.Texts_Tags");
            DropTable("dbo.Comments");
            DropTable("dbo.Tags");
            DropTable("dbo.Texts");
            DropTable("dbo.TextWalls");
        }
    }
}
