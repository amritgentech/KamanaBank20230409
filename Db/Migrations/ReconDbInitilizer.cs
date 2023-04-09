using Db.Enum;
using Db.IdentityLibrary.DataModel;
using Db.Model;
using Microsoft.AspNet.Identity;
using System;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using Helper.GlobalHelpers;

namespace Db.Migrations
{
    public class ReconDbInitializer : CreateDatabaseIfNotExists<ReconContext>
    {
        
        protected override void Seed(ReconContext context)
        {
            //            var baseDir = AppDomain.CurrentDomain.BaseDirectory.Replace("\\ReconUi", string.Empty) + "ReconUi\\Sql";

         //   SeedValue.Values(context);
         //   SeedScript.SeedScripts(context);
        }

        
    }
}
