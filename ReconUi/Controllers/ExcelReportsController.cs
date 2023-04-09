
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Db;
using Db.Model;

namespace ReconUi.Controllers
{
    public class ExcelReportsController : Controller
    {
        private ReconContext db = new ReconContext();

        // GET: ExcelReportes
        public ActionResult Index()
        {
            return View(db.ExcelReport.ToList());
        }

        // GET: ExcelReportes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var excelReport = db.ExcelReport.Find(id);
            if (excelReport == null)
            {
                return HttpNotFound();
            }
            return View(excelReport);
        }

        // GET: ExcelReportes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ExcelReportes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ExcelReport excelReport)
        {
            if (ModelState.IsValid)
            {
                db.ExcelReport.Add(excelReport);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(excelReport);
        }

        // GET: ExcelReportes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var excelReport = db.ExcelReport.Find(id);
            if (excelReport == null)
            {
                return HttpNotFound();
            }
            return View(excelReport);
        }

        // POST: ExcelReportes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ExcelReportId,Name,ExcelReportCode,Address,Contact,CreatedAt,UpdatedAt")] ExcelReport excelReport)
        {
            if (ModelState.IsValid)
            {
                db.Entry(excelReport).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(excelReport);
        }

        // GET: ExcelReportes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExcelReport excelReport = db.ExcelReport.Find(id);
            if (excelReport == null)
            {
                return HttpNotFound();
            }
            return View(excelReport);
        }

        // POST: ExcelReportes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ExcelReport excelReport = db.ExcelReport.Find(id);
            db.ExcelReport.Remove(excelReport);
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
