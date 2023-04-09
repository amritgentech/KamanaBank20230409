using Db;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ReconUi.Controllers
{
    public class ReportExportsController : Controller
    {
        // GET: ReportExports
        ReconContext db = new ReconContext();
        // GET: ReportExports
        public ActionResult Index()
        {

            var lst = new SelectList(db.ExcelReport, "ReportName", "ReportName");
            var a = db.ExcelReport.ToList();
            var terminalList = new SelectList(db.Terminals,"Name","Name");
            ViewBag.ReportName = lst;
            ViewBag.TerminalId = terminalList;
            return View();
        }
        public string ShowReports(string reportType, DateTime FromDate, DateTime ToDate, string terminalId)
        {
            var _ExcelReport = db.ExcelReport.Where(m => m.ReportName == reportType).FirstOrDefault();
            string data = "";
            if (FromDate != null && ToDate != null)
            {
                if(string.IsNullOrEmpty(terminalId))
                {
                    data = SqlToJson("select * from(" + _ExcelReport.Sql + ")a where Date between convert(date, '" + FromDate + "', 101) and convert(date, '" + ToDate + "', 101)");
                }
                else
                {
                    data = SqlToJson("select * from(" + _ExcelReport.Sql + ")a where Date between convert(date, '" + FromDate + "', 101) and convert(date, '" + ToDate + "', 101) and Terminal = '" + terminalId + "'");
                }
                
            }
            else
            {
                data = SqlToJson("Please Select Date");
            }

            return data;
        }
        public String SqlToJson(string sql)
        {
            var table = new DataTable();
            var cmd = db.Database.Connection.CreateCommand();
            cmd.CommandText = sql;

            cmd.Connection.Open();
            table.Load(cmd.ExecuteReader());
            var data = table;
            JsonSerializerSettings jss = new JsonSerializerSettings();
            jss.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            string resultData = JsonConvert.SerializeObject(data, jss);
            return resultData;
        }
    }
}