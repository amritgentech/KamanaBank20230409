using Db.Enum;
using Db.IdentityLibrary.DataModel;
using Db.Model;
using Microsoft.AspNet.Identity;
using System;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.IO;
using Helper.GlobalHelpers;


namespace Db.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<Db.ReconContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }
        protected override void Seed(Db.ReconContext context)
        {
            //            ReconDbInitializer dbInitializer = new ReconDbInitializer();
            //            dbInitializer.InitializeDatabase(context);
          // SeedValue.Values(context);
          //  SeedScript.SeedScripts(context);
        }
    }
}
