using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BAL;
using ClosedXML.Excel;
using Db.Model;
using Newtonsoft.Json;
using ReconUi.UiHelpers;

namespace ReconUi.Controllers
{
    public class BaseController : Controller
    {
        protected static void SaveActivityLogData(string tableName, string action, object logData)
        {
            try
            {
                var thisIdentity = System.Web.HttpContext.Current.User.Identity;


                var log = new ActivityLog()
                {
                    TableName = tableName,
                    LogDescription = action + " Action performed in " + tableName + " table",
                    LogData = JsonConvert.SerializeObject(logData, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }),
                    ActivityUser = thisIdentity.Name.Length == 0 ? "Guest User" : thisIdentity.Name,
                };
                if (thisIdentity.IsAuthenticated)
                    log.ActivityUserId = ExtensionProfile.GetUserGuid(thisIdentity).ToString();
                ReconBAL.SaveActivityLog(log);
            }
            catch
            {

                
            }
        }
        public virtual void BaseExportExcel(DataTable exportDataTable, string FileName)
        {
            using (var wb = new XLWorkbook())
            {
                exportDataTable.TableName = "Recon";
                var ws = wb.Worksheets.Add(exportDataTable);
                wb.Author = "Elite";
                wb.ShowRowColHeaders = true;

                ws.Tables.FirstOrDefault().ShowAutoFilter = false;
                ws.ConditionalFormats.RemoveAll();
                ws.Tables.FirstOrDefault().ShowColumnStripes = true;
                ws.Tables.FirstOrDefault().ShowRowStripes = true;
                ws.Rows(1, 3).Style.Font.Bold = true;
                var totalRows = ws.RowsUsed().Count();
                ws.Row(totalRows).Style.Font.Bold = true;

                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition",
                    "attachment;filename=" + FileName + ".xlsx");
                using (var MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
        }
    }
}