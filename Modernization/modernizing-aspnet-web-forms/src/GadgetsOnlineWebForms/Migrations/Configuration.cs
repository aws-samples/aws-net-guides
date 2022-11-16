using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;

namespace GadgetsOnlineWebForms.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<GadgetsOnline.Models.GadgetsOnlineEntities>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(GadgetsOnline.Models.GadgetsOnlineEntities context)
        {
            //  This method will be called after migrating to the latest version.
            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
}