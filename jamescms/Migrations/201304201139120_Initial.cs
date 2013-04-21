namespace jamescms.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserGameProfiles", "Attempts", c => c.Long(nullable: false));
            AddColumn("dbo.UserGameProfiles", "CorrectAnswers", c => c.Long(nullable: false));
            DropColumn("dbo.UserGameProfiles", "GameAccountCreated");
            DropColumn("dbo.UserGameProfiles", "Nickname");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserGameProfiles", "Nickname", c => c.String());
            AddColumn("dbo.UserGameProfiles", "GameAccountCreated", c => c.DateTime(nullable: false));
            DropColumn("dbo.UserGameProfiles", "CorrectAnswers");
            DropColumn("dbo.UserGameProfiles", "Attempts");
        }
    }
}
