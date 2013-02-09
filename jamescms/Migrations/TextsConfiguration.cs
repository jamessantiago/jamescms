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
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(jamescms.Models.TextContext context)
        {
            context.Texts.AddOrUpdate(
                d => d.Title,
                new Text
                {
                    Title = "My first post",
                    Article = "This my very first post",
                    Posted = new DateTime(1970, 1, 1),
                    Updated = new DateTime(1970, 1, 1),
                    Tags = new List<Tag>() { new Tag() { Name = "First" } }
                });

            context.Texts.AddOrUpdate(
                d => d.Title,
                new Text
                {
                    Title = "My second post",
                    Article = "This my second post",
                    Posted = new DateTime(1970, 1, 1),
                    Updated = new DateTime(1970, 1, 1),
                    Tags = new List<Tag>() { new Tag() { Name = "Fister" } }
                });

            context.Database.Initialize(true);
        }
    }
}
