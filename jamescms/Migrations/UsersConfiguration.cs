namespace jamescms.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class UsersConfiguration : DbMigrationsConfiguration<jamescms.Models.UsersContext>
    {
        public UsersConfiguration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(jamescms.Models.UsersContext context)
        {

        }
    }
}
