namespace jamescms.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using jamescms.Models;
    using System.Collections.Generic;

    internal sealed class QuizGameConfiguration : DbMigrationsConfiguration<jamescms.Models.QuizGameContext>
    {
        public QuizGameConfiguration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(jamescms.Models.QuizGameContext context)
        {            
            context.Database.Initialize(true);
        }
    }
}
