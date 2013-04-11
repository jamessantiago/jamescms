namespace jamescms.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QuizStates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LastQuestion = c.Int(nullable: false),
                        LastTimeStarted = c.DateTime(nullable: false),
                        TotalTriviaQuestions = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TriviaQuestions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Question = c.String(),
                        Answer = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserGameProfiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountModelUserId = c.Int(nullable: false),
                        GameAccountCreated = c.DateTime(nullable: false),
                        LastTimeSeen = c.DateTime(nullable: false),
                        Nickname = c.String(),
                        Points = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
           
            DropTable("dbo.UserGameProfiles");
            DropTable("dbo.TriviaQuestions");
            DropTable("dbo.QuizStates");
        }
    }
}
