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
    public class TerminalController : Controller
    {
        private ReconContext db = new ReconContext();

        // GET: Terminal
        public ActionResult Index(string terminalId)
        {
            List<Db.Model.Terminal> terminalList = new List<Terminal>();
            if (!string.IsNullOrEmpty(terminalId))
                terminalList = db.Terminals.Include(t => t.Branch).Where(x => x.TerminalId == terminalId).ToList();
            else
            {
                terminalList = db.Terminals.Include(t => t.Branch).ToList();
            }
            return View(terminalList.ToList());
        }

        // GET: Terminal/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Terminal terminal = db.Terminals.Find(id);
            if (terminal == null)
            {
                return HttpNotFound();
            }
            return View(terminal);
        }

        // GET: Terminal/Create
        public ActionResult Create()
        {
            ViewBag.BranchId = new SelectList(db.Branchs, "BranchId", "Name");
            return View();
        }

        // POST: Terminal/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TerminalId,Name,Address,TerminalType,TerminalBrand,TerminalIP,TerminalUserName,TerminalPassword,BranchId,Cbs_terminal_ac,CreatedAt,UpdatedAt")] Terminal terminal)
        {
            if (ModelState.IsValid)
            {
                db.Terminals.Add(terminal);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BranchId = new SelectList(db.Branchs, "BranchId", "Name", terminal.BranchId);
            return View(terminal);
        }

        // GET: Terminal/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Terminal terminal = db.Terminals.Find(id);
            if (terminal == null)
            {
                return HttpNotFound();
            }
            ViewBag.BranchId = new SelectList(db.Branchs, "BranchId", "Name", terminal.BranchId);
            return View(terminal);
        }

        // POST: Terminal/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TerminalId,Name,Address,TerminalType,TerminalBrand,TerminalIP,TerminalUserName,TerminalPassword,BranchId,Cbs_terminal_ac,CreatedAt,UpdatedAt")] Terminal terminal)
        {
            if (ModelState.IsValid)
            {
                db.Entry(terminal).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BranchId = new SelectList(db.Branchs, "BranchId", "Name", terminal.BranchId);
            return View(terminal);
        }

        // GET: Terminal/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Terminal terminal = db.Terminals.Find(id);
            if (terminal == null)
            {
                return HttpNotFound();
            }
            return View(terminal);
        }

        // POST: Terminal/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Terminal terminal = db.Terminals.Find(id);
            db.Terminals.Remove(terminal);
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
