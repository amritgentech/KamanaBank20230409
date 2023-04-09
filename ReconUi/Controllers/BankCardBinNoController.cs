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
    public class BankCardBinNoController : BaseController
    {
        private ReconContext db = new ReconContext();

        // GET: BankCardBinNo
        public ActionResult Index()
        {
            return View(db.BankCardBinNos.ToList());
        }

        // GET: BankCardBinNo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankCardBinNo bankCardBinNo = db.BankCardBinNos.Find(id);
            if (bankCardBinNo == null)
            {
                return HttpNotFound();
            }
            return View(bankCardBinNo);
        }

        // GET: BankCardBinNo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BankCardBinNo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BankCardBinNoId,BinNo,NetworkType,CreatedAt,UpdatedAt")] BankCardBinNo bankCardBinNo)
        {
            if (ModelState.IsValid)
            {
                db.BankCardBinNos.Add(bankCardBinNo);
                db.SaveChanges();
                try
                {
                    SaveActivityLogData("BankCardBinNo", "Create", bankCardBinNo);
                }
                catch { }
                return RedirectToAction("Index");
            }

            return View(bankCardBinNo);
        }

        // GET: BankCardBinNo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankCardBinNo bankCardBinNo = db.BankCardBinNos.Find(id);
            if (bankCardBinNo == null)
            {
                return HttpNotFound();
            }
            return View(bankCardBinNo);
        }

        // POST: BankCardBinNo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BankCardBinNoId,BinNo,NetworkType,CreatedAt,UpdatedAt")] BankCardBinNo bankCardBinNo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bankCardBinNo).State = EntityState.Modified;
                db.SaveChanges();
                try
                {
                    SaveActivityLogData("BankCardBinNo", "Update", bankCardBinNo);
                }
                catch { }
                return RedirectToAction("Index");
            }
            return View(bankCardBinNo);
        }

        // GET: BankCardBinNo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankCardBinNo bankCardBinNo = db.BankCardBinNos.Find(id);
            if (bankCardBinNo == null)
            {
                return HttpNotFound();
            }
            return View(bankCardBinNo);
        }

        // POST: BankCardBinNo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BankCardBinNo bankCardBinNo = db.BankCardBinNos.Find(id);
            db.BankCardBinNos.Remove(bankCardBinNo);
            db.SaveChanges();
            try
            {
                SaveActivityLogData("BankCardBinNo", "Delete", id);
            }
            catch { }
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
