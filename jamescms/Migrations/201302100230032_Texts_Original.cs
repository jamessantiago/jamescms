namespace jamescms.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Texts_Original : DbMigration
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
                "dbo.CodeSnippits",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Code = c.String(),
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
                        Text_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Texts", t => t.Text_Id)
                .Index(t => t.Text_Id);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Texts_CodeSnippits",
                c => new
                    {
                        TextId = c.Int(nullable: false),
                        CodeSnippitId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TextId, t.CodeSnippitId })
                .ForeignKey("dbo.Texts", t => t.TextId, cascadeDelete: true)
                .ForeignKey("dbo.CodeSnippits", t => t.CodeSnippitId, cascadeDelete: true)
                .Index(t => t.TextId)
                .Index(t => t.CodeSnippitId);
            
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
            
            DropTable("dbo.UserProfile");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.UserProfile",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
            DropIndex("dbo.Texts_Tags", new[] { "TagId" });
            DropIndex("dbo.Texts_Tags", new[] { "TextId" });
            DropIndex("dbo.ChildCommentRelationships", new[] { "CommentId" });
            DropIndex("dbo.ChildCommentRelationships", new[] { "CommentParentId" });
            DropIndex("dbo.Texts_CodeSnippits", new[] { "CodeSnippitId" });
            DropIndex("dbo.Texts_CodeSnippits", new[] { "TextId" });
            DropIndex("dbo.Comments", new[] { "Text_Id" });
            DropForeignKey("dbo.Texts_Tags", "TagId", "dbo.Tags");
            DropForeignKey("dbo.Texts_Tags", "TextId", "dbo.Texts");
            DropForeignKey("dbo.ChildCommentRelationships", "CommentId", "dbo.Comments");
            DropForeignKey("dbo.ChildCommentRelationships", "CommentParentId", "dbo.Comments");
            DropForeignKey("dbo.Texts_CodeSnippits", "CodeSnippitId", "dbo.CodeSnippits");
            DropForeignKey("dbo.Texts_CodeSnippits", "TextId", "dbo.Texts");
            DropForeignKey("dbo.Comments", "Text_Id", "dbo.Texts");
            DropTable("dbo.Texts_Tags");
            DropTable("dbo.ChildCommentRelationships");
            DropTable("dbo.Texts_CodeSnippits");
            DropTable("dbo.Tags");
            DropTable("dbo.Comments");
            DropTable("dbo.CodeSnippits");
            DropTable("dbo.Texts");
            DropTable("dbo.TextWalls");
        }
    }
}
