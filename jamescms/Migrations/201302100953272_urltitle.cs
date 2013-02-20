namespace jamescms.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class urltitle : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Texts", "UrlTitle", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.CodeSnippits", "Name", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.CodeSnippits", "Code", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CodeSnippits", "Code", c => c.String());
            AlterColumn("dbo.CodeSnippits", "Name", c => c.String());
            AlterColumn("dbo.Texts", "UrlTitle", c => c.String());
        }
    }
}
