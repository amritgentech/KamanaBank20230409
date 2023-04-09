using ReconUi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using BAL;
using Db;
using Filter = Db.Filter;

namespace ReconUi.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        public HomeController()
        {
            ViewBag.DrpReconTypesData = ReconBAL.GetReconList();
            ViewBag.DrpTerminalListData = ReconBAL.GetTerminalList();
            ViewBag.ReasonTypesData = ReconBAL.GetReasonList();
        }
        public ActionResult Index()
        {
            return View(InitialVal(new Filter()));
        }
        public ActionResult cardRecon()
        {
            return View(InitialVal(new Filter()));
        }

        [HttpPost]
        public ActionResult Filter(Filter _Filter)
        {
            if (ModelState.IsValid)
            {
                var result = Dashboard.DashboardFromFilter(_Filter);
                result.Filter = _Filter;
                return View("CardRecon", result);
            }
            return View("CardRecon", InitialVal(_Filter));
        }
        private Dashboard InitialVal(Filter _Filter)
        {
            Summary _Summary = new Summary();
            _Summary.Amount = 0;
            _Summary.Matched = 0;
            _Summary.UnMatched = 0;
            _Summary.Invalid = 0;
            _Summary.DateDiff = 0;
            _Summary.Exception = 0;
            _Summary.Suspected = 0;
            _Summary.Transactions = 0;

            return new Dashboard(_Summary, _Summary, _Summary, _Filter);
        }
    }
}