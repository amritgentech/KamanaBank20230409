using BAL;
using Db;
using Db.Enum;
using Db.Model;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using ReconUi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Filter = Db.Filter;

namespace ReconUi.Controllers
{
    public class SettlementCheckerController : Controller
    {
        ReconContext db = new ReconContext();
        public SettlementCheckerController()
        {
            ViewBag.DrpTerminalListData = ReconBAL.GetTerminalList();
        }
        public ActionResult Index()
        {
            var settlementViewModel = new SettlementViewModel();
            settlementViewModel._Filter = new Filter();
            settlementViewModel.ListSettlements = new List<SettlementModel>();
            return View(settlementViewModel);
        }
        public List<SettlementModel> GetNonSettlementData(Filter _Filter)
        {
            try
            {
                var settlementModelList = new List<SettlementModel>();
                DateTime fromDate = Convert.ToDateTime(_Filter.FromDate);
                DateTime toDate = Convert.ToDateTime(_Filter.ToDate);
                var terminalId = _Filter.TerminalId;

                var settlementList = new List<Settlement>();
                if (string.IsNullOrEmpty(terminalId))
                {
                    settlementList = db.Settlements.Where(x => x.TransactionDate >= fromDate && x.TransactionDate <= toDate
                                               && x.IsSettled == false).ToList();
                }
                else
                {
                    settlementList = db.Settlements.Where(x => x.TransactionDate >= fromDate && x.TransactionDate <= toDate && x.TerminalId == terminalId
                                              && x.IsSettled == false).ToList();
                }
                foreach (var item in settlementList)
                {
                    var settlementModel = new SettlementModel();
                    settlementModel.SettlementId = item.SettlementId;
                    settlementModel.VisaTransactionId = item.VisaTransactionId;
                    settlementModel.NpnTransactionId = item.NpnTransactionId;
                    settlementModel.CBSTransactionId = item.CBSTransactionId;
                    settlementModel.TransactionDate = item.TransactionDate;
                    settlementModel.TerminalId = item.TerminalId;
                    settlementModel.CardNo = item.CardNo;
                    settlementModel.TraceNo = item.TraceNo;
                    settlementModel.AuthCode = item.AuthCode;
                    settlementModel.VisaTransactionAmount = item.VisaTransactionAmount.ToString();
                    settlementModel.NpnTransactionAmount = item.NpnTransactionAmount.ToString();
                    settlementModel.CbsTransactionAmount = item.CbsTransactionAmount.ToString();
                    settlementModel.VisaResponseCode = item.VisaResponseCode;
                    settlementModel.NpnResponseCode = item.NpnResponseCode;
                    settlementModel.CbsResponseCode = item.CbsResponseCode;
                    settlementModel.TerminalType = item.TerminalType;
                    settlementModel.FT_Branch = item.FT_Branch;
                    settlementModel.IsOwnUsPayableReceivable = item.IsOwnUsPayableReceivable;
                    settlementModel.Status = item.Status;
                    settlementModel.IsChecked = false;
                    settlementModel.Reason = item.Reason;
                    settlementModelList.Add(settlementModel);
                }

                return settlementModelList;
            }
            catch (ArgumentException e)
            {

            }
            return new List<SettlementModel>();
        }
        [HttpPost]
        public ActionResult Filter(Filter _Filter)
        {
            if (ModelState.IsValid)
            {
                var result = GetNonSettlementData(_Filter);
                var settlementViewModel = new SettlementViewModel();
                settlementViewModel._Filter = _Filter;
                settlementViewModel.ListSettlements = result;
                ViewBag.Message = "Success";
                return View("Index", settlementViewModel);
            }
            ViewBag.Message = "Success";
            return View("Index", new SettlementViewModel() { _Filter = new Filter(), ListSettlements = new List<SettlementModel>() });
        }
        [HttpPost]
        public ActionResult CheckerUpdateTransactions(Filter _Filter, List<SettlementModel> SettlementModelList)
        {
            try
            {
                foreach (var item in SettlementModelList)
                {
                    if (item.IsChecked == true)
                    {
                        var settlement = db.Settlements.Where(x => x.SettlementId == item.SettlementId).FirstOrDefault();
                        settlement.IsSettled = true;
                        db.Entry(settlement).State = EntityState.Modified;
                        UpdateSettlementFlag(settlement);

                    }
                }
                db.SaveChanges();
                TempData["Message"] = "Success";
            }
            catch (Exception e)
            {
                TempData["Message"] = "Error";
            }
            return RedirectToAction("SettledTransactions");
        }
        public void UpdateSettlementFlag(Settlement settlement)
        {
            string[] Idarray = { settlement.VisaTransactionId, settlement.NpnTransactionId, settlement.CBSTransactionId };

            foreach (var item in Idarray)
            {
                if (item == null)
                    continue;

                int Id = Convert.ToInt32(item);
                var transaction = db.Transactions.Where(x => x.TransactionId == Id).SingleOrDefault();
                transaction.Recon_status = SettlementFlag.ManualSettled.ToString();
            }
        }
        public ActionResult SettledTransactions()
        {
            return View("Settlement");
        }
        [HttpPost]
        public string GetSettledTransactions(JQueryDataTableParamModel param)
        {
            var settledTransactions = new List<Settlement>();

            if (!string.IsNullOrEmpty(param.sSearch) && !string.IsNullOrWhiteSpace(param.sSearch))
            {
                var result = db.Settlements.Where(x => x.IsSettled == true).ToList();

                settledTransactions = result.Where(a => a.CardNo.ToLower().Contains(param.sSearch)
                    || a.TraceNo.ToLower().Contains(param.sSearch) || a.TerminalId.ToLower().Contains(param.sSearch)
                    ).OrderBy(a => a.SettlementId).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
            }
              
            else
                settledTransactions = db.Settlements.Where(x => x.IsSettled == true).OrderBy(a => a.SettlementId).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();

            int totalRecord = db.Settlements.Where(x => x.IsSettled == true).Count();

            StringBuilder sb = new StringBuilder();
            sb.Clear();
            sb.Append("{");
            sb.Append("\"sEcho\": "); 
            sb.Append(param.sEcho);
            sb.Append(",");
            sb.Append("\"iTotalRecords\": ");
            sb.Append(totalRecord);
            sb.Append(",");
            sb.Append("\"iTotalDisplayRecords\": ");
            sb.Append(totalRecord);
            sb.Append(",");
            sb.Append("\"aaData\": ");
            sb.Append(JsonConvert.SerializeObject(settledTransactions));
            sb.Append("}");
            return sb.ToString();
        }
    }
}