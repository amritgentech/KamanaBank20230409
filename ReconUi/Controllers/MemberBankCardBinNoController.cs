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
    public class MemberBankCardBinNoController : BaseController
    {
        private ReconContext db = new ReconContext();

        // GET: MemberBankCardBinNo
        public ActionResult Index()
        {
            var memberBankCardBinNos = db.MemberBankCardBinNos.Include(m => m.MemberBank);
            return View(memberBankCardBinNos.ToList());
        }

        // GET: MemberBankCardBinNo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MemberBankCardBinNo memberBankCardBinNo = db.MemberBankCardBinNos.Find(id);
            if (memberBankCardBinNo == null)
            {
                return HttpNotFound();
            }
            return View(memberBankCardBinNo);
        }

        // GET: MemberBankCardBinNo/Create
        public ActionResult Create()
        {
            ViewBag.MemberBankId = new SelectList(db.MemberBanks, "MemberBankId", "Name");
            return View();
        }

        // POST: MemberBankCardBinNo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MemberBankCardBinNoId,BinNo,NetworkType,MemberBankId,CreatedAt,UpdatedAt")] MemberBankCardBinNo memberBankCardBinNo)
        {
            if (ModelState.IsValid)
            {
                db.MemberBankCardBinNos.Add(memberBankCardBinNo);
                db.SaveChanges();
                try
                {
                    SaveActivityLogData("MemberBankCardBinNo", "Create", memberBankCardBinNo);
                }
                catch { }
                return RedirectToAction("Index");
            }

            ViewBag.MemberBankId = new SelectList(db.MemberBanks, "MemberBankId", "Name", memberBankCardBinNo.MemberBankId);
            return View(memberBankCardBinNo);
        }

        // GET: MemberBankCardBinNo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MemberBankCardBinNo memberBankCardBinNo = db.MemberBankCardBinNos.Find(id);
            if (memberBankCardBinNo == null)
            {
                return HttpNotFound();
            }
            ViewBag.MemberBankId = new SelectList(db.MemberBanks, "MemberBankId", "Name", memberBankCardBinNo.MemberBankId);
            return View(memberBankCardBinNo);
        }

        // POST: MemberBankCardBinNo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MemberBankCardBinNoId,BinNo,NetworkType,MemberBankId,CreatedAt,UpdatedAt")] MemberBankCardBinNo memberBankCardBinNo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(memberBankCardBinNo).State = EntityState.Modified;
                db.SaveChanges();
                try
                {
                    SaveActivityLogData("MemberBankCardBinNo", "Update", memberBankCardBinNo);
                }
                catch { }
                return RedirectToAction("Index");
            }
            ViewBag.MemberBankId = new SelectList(db.MemberBanks, "MemberBankId", "Name", memberBankCardBinNo.MemberBankId);
            return View(memberBankCardBinNo);
        }

        // GET: MemberBankCardBinNo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MemberBankCardBinNo memberBankCardBinNo = db.MemberBankCardBinNos.Find(id);
            if (memberBankCardBinNo == null)
            {
                return HttpNotFound();
            }
            return View(memberBankCardBinNo);
        }

        // POST: MemberBankCardBinNo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MemberBankCardBinNo memberBankCardBinNo = db.MemberBankCardBinNos.Find(id);
            db.MemberBankCardBinNos.Remove(memberBankCardBinNo);
            db.SaveChanges();
            try
            {
                SaveActivityLogData("MemberBankCardBinNo", "Delete", memberBankCardBinNo);
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
