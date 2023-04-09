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
    public class MemberBankController : BaseController
    {
        private ReconContext db = new ReconContext();

        // GET: MemberBank
        public ActionResult Index()
        {
            return View(db.MemberBanks.ToList());
        }

        // GET: MemberBank/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MemberBank memberBank = db.MemberBanks.Find(id);
            if (memberBank == null)
            {
                return HttpNotFound();
            }
            return View(memberBank);
        }

        // GET: MemberBank/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MemberBank/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MemberBankId,Name,BankCode,Address,Contact,CreatedAt,UpdatedAt")] MemberBank memberBank)
        {
            if (ModelState.IsValid)
            {
                db.MemberBanks.Add(memberBank);
                db.SaveChanges();
                try
                {
                    SaveActivityLogData("MemberBank", "Create", memberBank);
                }
                catch { }
                return RedirectToAction("Index");
            }

            return View(memberBank);
        }

        // GET: MemberBank/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MemberBank memberBank = db.MemberBanks.Find(id);
            if (memberBank == null)
            {
                return HttpNotFound();
            }
            return View(memberBank);
        }

        // POST: MemberBank/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MemberBankId,Name,BankCode,Address,Contact,CreatedAt,UpdatedAt")] MemberBank memberBank)
        {
            if (ModelState.IsValid)
            {
                db.Entry(memberBank).State = EntityState.Modified;
                db.SaveChanges();
                try
                {
                    SaveActivityLogData("MemberBank", "Update", memberBank);
                }
                catch { }
                return RedirectToAction("Index");
            }
            return View(memberBank);
        }

        // GET: MemberBank/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MemberBank memberBank = db.MemberBanks.Find(id);
            if (memberBank == null)
            {
                return HttpNotFound();
            }
            return View(memberBank);
        }

        // POST: MemberBank/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MemberBank memberBank = db.MemberBanks.Find(id);
            db.MemberBanks.Remove(memberBank);
            db.SaveChanges();
            try
            {
                SaveActivityLogData("MemberBank", "Delete", id);
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
