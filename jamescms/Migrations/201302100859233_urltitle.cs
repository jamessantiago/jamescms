namespace jamescms.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class urltitle : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Texts", "UrlTitle", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Texts", "UrlTitle");
        }
    }
}
