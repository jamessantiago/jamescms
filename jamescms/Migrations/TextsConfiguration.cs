namespace jamescms.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using jamescms.Models;
    using System.Collections.Generic;

    internal sealed class TextsConfiguration : DbMigrationsConfiguration<jamescms.Models.TextContext>
    {
        public TextsConfiguration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(jamescms.Models.TextContext context)
        {            

            if (!context.Settings.Any())
                context.Settings.Add(new jamescms.Models.Setting() { HeadSha = "" });

            context.Database.Initialize(true);
        }
    }
}
