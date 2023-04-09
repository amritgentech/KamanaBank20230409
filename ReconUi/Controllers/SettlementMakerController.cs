using System;
using Db;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Filter = Db.Filter;
using BAL;
using System.Reflection;
using System.Text.RegularExpressions;
using Db.Enum;
using ReconUi.Models;
using DbOperations;
using Newtonsoft.Json;
using System.Data;
using System.Text;
using Db.Model;

namespace ReconUi.Controllers
{
    [Authorize]
    public class SettlementMakerController : Controller
    {
        ReconContext db = new ReconContext();
        public SettlementMakerController()
        {
            ViewBag.DrpTerminalListData = ReconBAL.GetTerminalList();
        }
        public DataTable SqlToJson(DateTime fromDate, DateTime toDate, string terminalId)
        {
            DbOperation dbOperation = new DbOperation("ReconContextConnectionString");
            var resultDataTable = new DataTable();
            ParameterCollection collection = new ParameterCollection();
            if (terminalId == null)
                terminalId = "";
            collection.Add(new Parameter("@fromDate", fromDate));
            collection.Add(new Parameter("@toDate", toDate));
            collection.Add(new Parameter("@terminalId", terminalId));
            resultDataTable = dbOperation.ExecuteDataTable("sp_NPNVsCbsVsVisa", string.Empty, collection, CommandType.StoredProcedure);
            return resultDataTable;
        }
        public ActionResult Index()
        {
            var settlementViewModel = new SettlementViewModel();
            settlementViewModel._Filter = new Filter();
            settlementViewModel.ListSettlements = new List<SettlementModel>();
            return View(settlementViewModel);
        }
        private List<SettlementModel> GetTransactionsNotinSettlements(Filter _Filter, List<SettlementModel> settlementModelList)
        {
            DateTime fromDate = Convert.ToDateTime(_Filter.FromDate);
            DateTime toDate = Convert.ToDateTime(_Filter.ToDate);
            var terminalId = _Filter.TerminalId;

            var settlementList = new List<Settlement>();
            if (string.IsNullOrEmpty(terminalId))
            {
                settlementList = db.Settlements.Where(x => x.TransactionDate >= fromDate && x.TransactionDate <= toDate && x.IsSettled == false).ToList();
            }
            else
            {
                settlementList = db.Settlements.Where(x => x.TransactionDate >= fromDate && x.TransactionDate <= toDate && x.TerminalId == terminalId && x.IsSettled == false).ToList();
            }
            if (settlementList.Count < 1)
                return settlementModelList;

            var result = (from t1 in settlementModelList
                          join t2 in settlementList
                               on new
                               {
                                   TerminalId = t1.TerminalId,
                                   TransactionDate = t1.TransactionDate,
                                   CardNo = t1.CardNo,
                                   TraceNo = t1.TraceNo
                               }
                              equals new
                              {
                                  TerminalId = t2.TerminalId,
                                  TransactionDate = t2.TransactionDate,
                                  CardNo = t2.CardNo,
                                  TraceNo = t2.TraceNo
                              }
                               into ps
                          from p in ps.DefaultIfEmpty()
                          select new SettlementModel()
                          {
                              TransactionDate = t1.TransactionDate,
                              TerminalId = t1.TerminalId,
                              CardNo = t1.CardNo,
                              TraceNo = t1.TraceNo,
                              Status = t1.Status,
                              TerminalType = t1.TerminalType,
                              IsOwnUsPayableReceivable = t1.IsOwnUsPayableReceivable,
                              VisaTransactionId = t1.VisaTransactionId,
                              NpnTransactionId = t1.NpnTransactionId,
                              CBSTransactionId = t1.CBSTransactionId,
                              VisaTransactionAmount = t1.VisaTransactionAmount,
                              NpnTransactionAmount = t1.NpnTransactionAmount,
                              CbsTransactionAmount = t1.CbsTransactionAmount,
                              VisaResponseCode = t1.VisaResponseCode,
                              NpnResponseCode = t1.NpnResponseCode,
                              CbsResponseCode = t1.CbsResponseCode,
                              AuthCode = t1.AuthCode,
                              FT_Branch = t1.FT_Branch,
                              //VisaTransactionId = p == null? null: p.VisaTransactionId,
                              //NpnTransactionId = p == null ? null : p.NpnTransactionId,
                              //CBSTransactionId = p == null ? null : p.CBSTransactionId,
                              //VisaTransactionAmount = p == null ? null : p.VisaTransactionAmount.ToString(),
                              //NpnTransactionAmount = p == null ? null : p.NpnTransactionAmount.ToString(),
                              //CbsTransactionAmount = p == null ? null : p.CbsTransactionAmount.ToString(),
                              //VisaResponseCode = p == null ? null : p.VisaResponseCode,
                              //NpnResponseCode = p == null ? null : p.NpnResponseCode,
                              //CbsResponseCode = p == null ? null : p.CbsResponseCode,
                              //AuthCode = p == null ? null : p.AuthCode,
                              //FT_Branch = p == null ? null : p.FT_Branch,
                          }).ToList();
            return result;
        }
        [HttpPost]
        public ActionResult Filter(Filter _Filter)
        {
            if (ModelState.IsValid)
            {
                List<SettlementModel> settlementModelList = new List<SettlementModel>();
                try
                {

                    DateTime fromDate = Convert.ToDateTime(_Filter.FromDate);
                    DateTime toDate = Convert.ToDateTime(_Filter.ToDate);
                    var terminalId = _Filter.TerminalId;
                    var data = new DataTable();
                    if (fromDate != null && toDate != null)
                    {
                        if (string.IsNullOrEmpty(terminalId))
                        {
                            data = SqlToJson(fromDate, toDate, terminalId);

                        }
                        else
                        {
                            data = SqlToJson(fromDate, toDate, terminalId);
                        }

                    }

                    var newdata = data.Select("Status <> 'Matched'");
                    foreach (DataRow dr in newdata)
                    {
                        SettlementModel settlementModel = new SettlementModel();
                        settlementModel.VisaTransactionId = dr["VisaId"].ToString();
                        settlementModel.NpnTransactionId = dr["NpnId"].ToString();
                        settlementModel.CBSTransactionId = dr["CBSId"].ToString();
                        settlementModel.TransactionDate = Convert.ToDateTime(dr["TranDate"]);
                        settlementModel.TerminalId = dr["Terminal"].ToString();
                        settlementModel.CardNo = dr["CardNo"].ToString();
                        settlementModel.TraceNo = dr["TraceNo"].ToString();
                        settlementModel.AuthCode = dr["AuthCode"].ToString();
                        settlementModel.Currency = dr["Currency"].ToString();
                        settlementModel.VisaTransactionAmount = dr["VisaAmount"].ToString();
                        settlementModel.NpnTransactionAmount = dr["NPNAmount"].ToString();
                        settlementModel.CbsTransactionAmount = dr["CBSAmount"].ToString();
                        settlementModel.VisaResponseCode = dr["VisaResCode"].ToString();
                        settlementModel.NpnResponseCode = dr["NPNResCode"].ToString();
                        settlementModel.CbsResponseCode = dr["CBSResCode"].ToString();
                        settlementModel.TerminalType = dr["TerminalType"].ToString();
                        settlementModel.FT_Branch = dr["FTBranch"].ToString();
                        settlementModel.IsOwnUsPayableReceivable = dr["ONUSPayableReceivable"].ToString();
                        settlementModel.Status = dr["Status"].ToString();
                        settlementModel.IsChecked = false;
                        settlementModelList.Add(settlementModel);
                    }
                }
                catch (ArgumentException e)
                {
                    ModelState.AddModelError("", "");
                }
              //  var result = GetTransactionsNotinSettlements(_Filter, settlementModelList);
                var SettlementViewModel = new SettlementViewModel();
                SettlementViewModel._Filter = _Filter;
                SettlementViewModel.ListSettlements = settlementModelList;
                ViewBag.Message = "Success";
                return View("Index", SettlementViewModel);
            }
            ViewBag.Message = "Success";
            return View("Index", new SettlementViewModel() { _Filter = new Filter(), ListSettlements = new List<SettlementModel>() });
        }
        [HttpPost]
        public ActionResult MakerUpdateTransactions(Filter _Filter, List<SettlementModel> settlementModelList)
        {
            try
            {
                foreach (var item in settlementModelList)
                {
                    if (item.IsChecked == true)
                    {
                        var settlement = new Settlement();
                        settlement.TransactionDate = item.TransactionDate;
                        settlement.TraceNo = item.TraceNo;
                        settlement.TerminalId = item.TerminalId;
                        settlement.CardNo = item.CardNo;
                        settlement.AuthCode = item.AuthCode;
                        settlement.VisaTransactionId = item.VisaTransactionId;
                        settlement.VisaTransactionAmount = Convert.ToDecimal(item.VisaTransactionAmount);
                        settlement.VisaResponseCode = item.VisaResponseCode;
                        settlement.NpnTransactionId = item.NpnTransactionId;
                        settlement.NpnTransactionAmount = Convert.ToDecimal(item.NpnTransactionAmount);
                        settlement.NpnResponseCode = item.NpnResponseCode;
                        settlement.CBSTransactionId = item.CBSTransactionId;
                        settlement.CbsTransactionAmount = Convert.ToDecimal(item.CbsTransactionAmount);
                        settlement.CbsResponseCode = item.CbsResponseCode;
                        settlement.TerminalType = item.TerminalType;
                        settlement.IsOwnUsPayableReceivable = item.IsOwnUsPayableReceivable;
                        settlement.Status = item.Status;
                        settlement.FT_Branch = item.FT_Branch;
                        settlement.Reason = item.Reason;
                        settlement.IsSettled = false;
                        db.Settlements.Add(settlement);
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

            return RedirectToAction("UnSettledTransactions");
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
                transaction.Recon_status = SettlementFlag.UnSettled.ToString();
            }
        }
        public ActionResult UnSettledTransactions()
        {
            return View("UnSettlement");
        }
        [HttpPost]
        public string GetUnSettledTransactions(JQueryDataTableParamModel param)
        {
            var unsettledTransactions = new List<Settlement>();

            if (!string.IsNullOrEmpty(param.sSearch) && !string.IsNullOrWhiteSpace(param.sSearch))
            {
                var result = db.Settlements.Where(x => x.IsSettled == false).ToList();

                unsettledTransactions = result.Where(a => a.CardNo.ToLower().Contains(param.sSearch)
               || a.TraceNo.ToLower().Contains(param.sSearch) || a.TerminalId.ToLower().Contains(param.sSearch)
               ).OrderBy(a => a.SettlementId).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
            }

            else
                unsettledTransactions = db.Settlements.Where(x => x.IsSettled == false).OrderBy(a => a.SettlementId).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();

            int totalRecord = db.Settlements.Where(x => x.IsSettled == false).Count();

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
            sb.Append(JsonConvert.SerializeObject(unsettledTransactions));
            sb.Append("}");
            return sb.ToString();
        }
    }
}
