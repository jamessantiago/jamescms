namespace jamescms.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CodeSnippit : DbMigration
    {
        public override void Up()
        {
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
            
            AddColumn("dbo.Comments", "Text_Id", c => c.Int());
            AddForeignKey("dbo.Comments", "Text_Id", "dbo.Texts", "Id");
            CreateIndex("dbo.Comments", "Text_Id");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Texts_CodeSnippits", new[] { "CodeSnippitId" });
            DropIndex("dbo.Texts_CodeSnippits", new[] { "TextId" });
            DropIndex("dbo.Comments", new[] { "Text_Id" });
            DropForeignKey("dbo.Texts_CodeSnippits", "CodeSnippitId", "dbo.CodeSnippits");
            DropForeignKey("dbo.Texts_CodeSnippits", "TextId", "dbo.Texts");
            DropForeignKey("dbo.Comments", "Text_Id", "dbo.Texts");
            DropColumn("dbo.Comments", "Text_Id");
            DropTable("dbo.Texts_CodeSnippits");
            DropTable("dbo.CodeSnippits");
        }
    }
}
