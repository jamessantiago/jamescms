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
                    UrlTitle = "My_first_post",
                    Article = "This my very first post",
                    Posted = new DateTime(1970, 1, 1),
                    Updated = new DateTime(1970, 1, 1)
                });

            context.Texts.AddOrUpdate(
                d => d.Title,
                new Text
                {
                    Title = "My second post",
                    UrlTitle = "My_Second_post",
                    Article = "This my second post",
                    Posted = new DateTime(1970, 1, 1),
                    Updated = new DateTime(1970, 1, 1),
                    Tags = new List<Tag>() { new Tag() { Name = "Fister" } }
                });

            context.Texts.AddOrUpdate(
                d => d.Title,
                new Text
                {
                    Title = "My third post",
                    Article = "This my third post",
                    UrlTitle = "third_post",
                    Posted = new DateTime(1970, 1, 1),
                    Updated = new DateTime(1970, 1, 1),
                    Tags = new List<Tag>() { new Tag() { Name = "thirds" } },
                    CodeSnippit = new List<CodeSnippit>() { new CodeSnippit() { Name = "Test", Code = "10 GOTO 10" } }
                });

            context.Database.Initialize(true);
        }
    }
}
