using Db;
using Db.Enum;
using Db.Hepler;
using Db.Model;
using ReconUi.DTO;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ReconUi.Controllers
{
    public class ReportMastersController : Controller
    {
        private ReconContext db = new ReconContext();

        // GET: ReportMasters
        public ActionResult List()
        {
            var catList = db.Categories.ToList();
            return View(catList);
        }
        public ActionResult Index(int id = 0)
        {
            var reportMasters = new List<ReportMaster>();
            if (id == 0)
            {
                return Redirect("List");
            }
            else
            {
                reportMasters = db.ReportMasters.Include(r => r.Categories).Include(r => r.ReportTypes).Where(x => x.CategorieId == id).ToList();
            }

            return View(reportMasters);
        }
        public ActionResult SetParameters(int reportId)
        {
            var query = db.ReportMasters.Where(x => x.Id == reportId).FirstOrDefault();
            var oldParameters = db.ReportParameters.Where(x => x.ReportMasterId == reportId).ToList();
            List<ParametersDTO> parameters = new List<ParametersDTO>();
            foreach (var item in query.Query.Split(' '))
            {
                if (item.StartsWith("#"))
                {
                    ParametersDTO parameter = new ParametersDTO();
                    var oldParameter = oldParameters.Where(x => x.Label == item).FirstOrDefault();
                    if (oldParameter != null)
                    {
                        parameter.Label = oldParameter.Label;
                        parameter.ReportMasterId = oldParameter.ReportMasterId;
                        parameter.QueryOfMasterReport = query.Query;
                        parameter.ParameterDataType = oldParameter.ParameterDataType;
                        parameter.Query = oldParameter.Query;
                        parameter.DisplayName = oldParameter.DisplayName;
                    }
                    else
                    {
                        parameter.Label = item;
                        parameter.ReportMasterId = reportId;
                        parameter.QueryOfMasterReport = query.Query;
                    }
                    if (parameters.Where(x => x.Label == parameter.Label).Count() == 0)
                        parameters.Add(parameter);
                }
            }
            return View(parameters);
        }
        [HttpPost]
        public ActionResult SetParameters(List<ParametersDTO> listParameters)
        {
            var rptId = listParameters.FirstOrDefault().ReportMasterId;
            db.ReportParameters.RemoveRange(db.ReportParameters.Where(x => x.ReportMasterId == rptId).ToList());
            List<ReportParameter> parameters = new List<ReportParameter>();
            foreach (var item in listParameters)
            {
                var para = new ReportParameter();
                para.Label = item.Label;
                para.ParameterDataType = item.ParameterDataType;
                para.Query = item.Query;
                para.ReportMasterId = item.ReportMasterId;
                para.DisplayName = item.DisplayName;
                parameters.Add(para);
            }
            db.ReportParameters.AddRange(parameters);
            db.SaveChanges();
            return View(listParameters);
        }

        public ActionResult ViewReports(int? reportId)
        {
            var query = db.ReportMasters.Where(x => x.Id == reportId).FirstOrDefault();
            var oldParameters = db.ReportParameters.Where(x => x.ReportMasterId == reportId).ToList();
            List<ParametersDTO> parameters = new List<ParametersDTO>();
            foreach (var item in oldParameters)
            {
                ParametersDTO parameter = new ParametersDTO();
                var oldParameter = oldParameters.Where(x => x.Label == item.Label).FirstOrDefault();
                parameter.Label = oldParameter.Label;
                parameter.ReportMasterId = oldParameter.ReportMasterId;
                parameter.QueryOfMasterReport = query.Query;
                parameter.ParameterDataType = oldParameter.ParameterDataType;
                parameter.Query = oldParameter.Query;
                parameter.DisplayName = oldParameter.DisplayName;
                if (oldParameter.ParameterDataType == ParameterDataType.DropDown)
                {
                    var ddl = ExecuteSQL.GetSelectList(db, oldParameter.Query, null);
                    parameter.DDL = ddl;
                }
                if (parameters.Where(x => x.Label == parameter.Label).Count() == 0)
                    parameters.Add(parameter);
            }
            ViewBag.Table = "";
            ViewBag.RptName = query.Name;
            //backlist to index
            ViewBag.CategoryId = query.CategorieId;
            return View(parameters);
        }
        [HttpPost]
        public ActionResult ViewReports(List<ParametersDTO> listParameters)
        {
            var listOfQueryString = new List<string>();
            var reportMasterId = listParameters.FirstOrDefault().ReportMasterId;
            var rpt = db.ReportMasters.Where(x => x.Id == reportMasterId).FirstOrDefault();
            var databaseName = "use " + rpt.DatabaseName + "; ";
            var prevPreviousWord = ""; //2 step previous
            var previousWord = ""; //1 step previous
            foreach (var item in listParameters.FirstOrDefault().QueryOfMasterReport.Split(' '))
            {
                string word = item;
                if (item.StartsWith("#"))
                {
                    var searchval = listParameters.Where(x => x.Label.ToLower().Trim() == item.ToLower().Trim()).Select(x => x.SearchValue).FirstOrDefault();
                    if (string.IsNullOrEmpty(searchval))
                    {
                        word = prevPreviousWord;
                    }
                    else
                    {
                        if (previousWord.ToLower().Trim() == "like")
                            word = "'%" + searchval + "%'";
                        else
                            word = "'" + searchval + "'";
                    }
                }
                prevPreviousWord = previousWord;
                previousWord = item;
                listOfQueryString.Add(word);
            }
            var finalQuery = string.Join(" ", listOfQueryString);
            var datatable = ExecuteSQL.GetDatatable(db, databaseName + finalQuery, null);
            string htmltable = "";
            if (datatable.Rows.Count > 0)
            {
                var reportType = db.ReportMasters.Where(x => x.Id == reportMasterId).Select(x => x.ReportTypes);
                if (reportType.FirstOrDefault().Type == "Diagram")
                {
                    htmltable = "Diagram report comming soon"; //ExecuteSQL.GetDataForChart(datatable);
                }
                else
                    htmltable = ExecuteSQL.ConvertDataTableToHTML(datatable);
            }
            ViewBag.Table = htmltable;
            ViewBag.RptName = rpt.Name;
            //backlist to index
            ViewBag.CategoryId = rpt.CategorieId;
            foreach (var item in listParameters)
            {
                if (item.ParameterDataType == ParameterDataType.DropDown)
                {
                    var ddl = ExecuteSQL.GetSelectList(db, item.Query, null);
                    item.DDL = ddl;
                }
            }

            return View(listParameters);
        }


        // GET: ReportMasters/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReportMaster reportMaster = db.ReportMasters.Find(id);
            if (reportMaster == null)
            {
                return HttpNotFound();
            }
            return View(reportMaster);
        }

        // GET: ReportMasters/Create
        public ActionResult Create()
        {
            ViewBag.CategorieId = new SelectList(db.Categories, "Id", "Name");
            ViewBag.ReportTypeId = new SelectList(db.ReportTypes, "Id", "Type");
            return View();
        }

        // POST: ReportMasters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Query,DatabaseName,CategorieId,ReportTypeId")] ReportMaster reportMaster)
        {
            if (ModelState.IsValid)
            {
                db.ReportMasters.Add(reportMaster);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategorieId = new SelectList(db.Categories, "Id", "Name", reportMaster.CategorieId);
            ViewBag.ReportTypeId = new SelectList(db.ReportTypes, "Id", "Type", reportMaster.ReportTypeId);
            return View(reportMaster);
        }

        // GET: ReportMasters/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReportMaster reportMaster = db.ReportMasters.Find(id);
            if (reportMaster == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategorieId = new SelectList(db.Categories, "Id", "Name", reportMaster.CategorieId);
            ViewBag.ReportTypeId = new SelectList(db.ReportTypes, "Id", "Type", reportMaster.ReportTypeId);
            return View(reportMaster);
        }

        // POST: ReportMasters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Query,DatabaseName,CategorieId,ReportTypeId")] ReportMaster reportMaster)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reportMaster).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategorieId = new SelectList(db.Categories, "Id", "Name", reportMaster.CategorieId);
            ViewBag.ReportTypeId = new SelectList(db.ReportTypes, "Id", "Type", reportMaster.ReportTypeId);
            return View(reportMaster);
        }

        // GET: ReportMasters/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReportMaster reportMaster = db.ReportMasters.Find(id);
            if (reportMaster == null)
            {
                return HttpNotFound();
            }
            return View(reportMaster);
        }

        // POST: ReportMasters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ReportMaster reportMaster = db.ReportMasters.Find(id);
            db.ReportMasters.Remove(reportMaster);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}