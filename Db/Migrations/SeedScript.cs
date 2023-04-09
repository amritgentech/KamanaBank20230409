using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Db.Enum;
using Db.IdentityLibrary.DataModel;
using Db.Model;
using Helper.GlobalHelpers;
using Microsoft.AspNet.Identity;

namespace Db.Migrations
{
    public static class SeedScript
    {
        public static String PublishFolder
        {
            get
            {
                return ConfigurationManager.AppSettings["PublishFolder"];
            }
        }
        public static void SeedScripts(ReconContext context)
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;

            string publishFolder = string.IsNullOrEmpty(PublishFolder) ? "\\Sql" : "\\" + PublishFolder + "\\Sql";

            baseDir = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin", string.Empty)
                          .Replace("\\ReconParserExecutable", string.Empty).Replace("\\Debug", string.Empty)
                          .Replace("\\Db", string.Empty).Replace("\\ReconUi", string.Empty) + publishFolder;

            try
            {
                context.Database.ExecuteSqlCommand(File.ReadAllText(baseDir + "\\vw_transaction_details.sql"));
                context.Database.ExecuteSqlCommand(File.ReadAllText(baseDir + "\\vw_transaction_details_StrictDate.sql"));
                context.Database.ExecuteSqlCommand(File.ReadAllText(baseDir + "\\sp_TwoWayRecon.sql"));
                context.Database.ExecuteSqlCommand(File.ReadAllText(baseDir + "\\sp_ThreeWayRecon.sql"));
                context.Database.ExecuteSqlCommand(File.ReadAllText(baseDir + "\\sp_ThreeWayRecon_Visa.sql"));
            }
            catch (Exception e)
            {
                GlobalHelper.StepLogging(baseDir);
                GlobalHelper.ErrorLogging(e);
            }
        }
    }
}
