namespace jamescms.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CodeSnippit1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Comments", "Text_Id", "dbo.Texts");
            DropForeignKey("dbo.Texts_CodeSnippits", "TextId", "dbo.Texts");
            DropForeignKey("dbo.Texts_CodeSnippits", "CodeSnippitId", "dbo.CodeSnippits");
            DropForeignKey("dbo.ChildCommentRelationships", "CommentParentId", "dbo.Comments");
            DropForeignKey("dbo.ChildCommentRelationships", "CommentId", "dbo.Comments");
            DropForeignKey("dbo.Texts_Tags", "TextId", "dbo.Texts");
            DropForeignKey("dbo.Texts_Tags", "TagId", "dbo.Tags");
            DropIndex("dbo.Comments", new[] { "Text_Id" });
            DropIndex("dbo.Texts_CodeSnippits", new[] { "TextId" });
            DropIndex("dbo.Texts_CodeSnippits", new[] { "CodeSnippitId" });
            DropIndex("dbo.ChildCommentRelationships", new[] { "CommentParentId" });
            DropIndex("dbo.ChildCommentRelationships", new[] { "CommentId" });
            DropIndex("dbo.Texts_Tags", new[] { "TextId" });
            DropIndex("dbo.Texts_Tags", new[] { "TagId" });
            CreateTable(
                "dbo.UserProfile",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
            DropTable("dbo.TextWalls");
            DropTable("dbo.Texts");
            DropTable("dbo.CodeSnippits");
            DropTable("dbo.Comments");
            DropTable("dbo.Tags");
            DropTable("dbo.Texts_CodeSnippits");
            DropTable("dbo.ChildCommentRelationships");
            DropTable("dbo.Texts_Tags");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Texts_Tags",
                c => new
                    {
                        TextId = c.Int(nullable: false),
                        TagId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TextId, t.TagId });
            
            CreateTable(
                "dbo.ChildCommentRelationships",
                c => new
                    {
                        CommentParentId = c.Int(nullable: false),
                        CommentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.CommentParentId, t.CommentId });
            
            CreateTable(
                "dbo.Texts_CodeSnippits",
                c => new
                    {
                        TextId = c.Int(nullable: false),
                        CodeSnippitId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TextId, t.CodeSnippitId });
            
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
                        Text_Id = c.Int(),
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
                "dbo.TextWalls",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Description = c.String(nullable: false, maxLength: 500),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.UserProfile");
            CreateIndex("dbo.Texts_Tags", "TagId");
            CreateIndex("dbo.Texts_Tags", "TextId");
            CreateIndex("dbo.ChildCommentRelationships", "CommentId");
            CreateIndex("dbo.ChildCommentRelationships", "CommentParentId");
            CreateIndex("dbo.Texts_CodeSnippits", "CodeSnippitId");
            CreateIndex("dbo.Texts_CodeSnippits", "TextId");
            CreateIndex("dbo.Comments", "Text_Id");
            AddForeignKey("dbo.Texts_Tags", "TagId", "dbo.Tags", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Texts_Tags", "TextId", "dbo.Texts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ChildCommentRelationships", "CommentId", "dbo.Comments", "Id");
            AddForeignKey("dbo.ChildCommentRelationships", "CommentParentId", "dbo.Comments", "Id");
            AddForeignKey("dbo.Texts_CodeSnippits", "CodeSnippitId", "dbo.CodeSnippits", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Texts_CodeSnippits", "TextId", "dbo.Texts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Comments", "Text_Id", "dbo.Texts", "Id");
        }
    }
}
