using AutoMapper;
using BAL;
using ClosedXML.Excel;
using Db;
using Db.Enum;
using Db.Model;
using DbOperations;
using Helper.GlobalHelpers;
using ReconUi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ReconUi.Model;
using System.Text;
using Newtonsoft.Json;

namespace ReconUi.Controllers
{
    [Authorize]
    public class ReportController : BaseController
    {
        public DbOperation _DbOperation { get; set; }
        #region Report View
        private Report _Report;
        private Recon recon;
        public ReportController()
        {
            ViewBag.DrpReconTypesData = ReconBAL.GetReconList();
            ViewBag.DrpReconTypeData = ReconBAL.ReconList();
            ViewBag.DrpTerminalListData = ReconBAL.GetTerminalList();
            ViewBag.ReasonTypesData = ReconBAL.GetReasonList();
            _DbOperation = new DbOperation("ReconContextConnectionString");
        }
        private Report InitialVal(Db.Filter _Filter)
        {
            _Report = new Report();
            //            Db.Filter _Filter = new Db.Filter();
            Summary _Summary = new Summary();
            _Summary.Amount = _Filter.amountAccordingToStatus;
            _Summary.Matched = _Filter.statusTotalTxnRecord;
            _Summary.Invalid = _Filter.statusTotalTxnRecord;
            _Summary.UnMatched = _Filter.statusTotalTxnRecord;
            _Summary.DateDiff = _Filter.statusTotalTxnRecord;
            _Summary.Exception = _Filter.statusTotalTxnRecord;
            _Summary.Suspected = _Filter.statusTotalTxnRecord;
            _Summary.Transactions = _Filter.statusTotalTxnRecord;
            _Report._Summary = _Summary;
            _Report._Filter = _Filter;

            return _Report;
        }
        // for mobilebanking
        //private Report InitialVal1(Db.Filter _Filter)
        //{
        //    _Report = new Report();
        //    Summary _Summary = new Summary();
        //    _Summary.Transactions = _Filter.statusTotalTxnRecord;
        //    _Summary.Amount = _Filter.amountAccordingToStatus;
        //    _Summary.Matched = _Filter.statusTotalTxnRecord;
        //    _Summary.UnMatched = _Filter.statusTotalTxnRecord;
        //    _Summary.Invalid = _Filter.statusTotalTxnRecord;
        //    _Summary.DateDiff = _Filter.statusTotalTxnRecord;

        //    _Report._Summary = _Summary;
        //    _Report._Filter = _Filter;

        //    return _Report;
        //}
        public ActionResult Index(Db.Filter _Filter)
        {
            //if (_Filter.ReconType == 4 || _Filter.ReconType == 5)
            //{
            //    _Report = InitialVal1(_Filter);
            //    ViewBag.Message = "Mobile";
            //}
            //else
            //{
            //    ViewBag.Message = "Atm";
            //    _Report = InitialVal(_Filter);
            //}
        _Report = InitialVal(_Filter);
            return View(_Report);
        }
        [HttpPost]
        public ActionResult Filter(Db.Filter _Filter)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _Report = Report.GetReportFromFilter(_Filter);
                    return View("Index", _Report);
                }
                catch (Exception ex)
                {
                }
            }
            return View("Index", InitialVal(_Filter));
        }
        public ActionResult AjaxHandler(JQueryDataTableParamModel param, Db.Filter _Filter)
        {
            recon = new Recon();
            List<VwTransactionDetailsModel> VwTransactionDetailsModels = null;
            string[] fieldSelector = null;
            object[] parametersArray = new object[] { _Filter };
            object results = new object();
            string ReconMappedMethodName = string.Empty;
            MethodInfo method;

            if (!String.IsNullOrEmpty(_Filter.ReconTypeName))
            {
                fieldSelector = Regex.Split(_Filter.ReconTypeName, "Vs");
            }
            try
            {
                if (_Filter.ReconType < 1)
                {
                    throw new ArgumentException("Recon type not selected");
                }
                using (var context = new Db.ReconContext())
                {
                    ReconMappedMethodName = context.ReconTypes.FirstOrDefault(x => x.ReconTypeId == _Filter.ReconType).MapReconMethod;
                }

                method = recon.GetType().GetMethod(ReconMappedMethodName);
                results = method.Invoke(recon, parametersArray);
             VwTransactionDetailsModels = (List<VwTransactionDetailsModel>)results;
                    var allTransactions = VwTransactionDetailsModels.Where(x => x != null);

                    if (_Filter.ReconType > 0)
                    {
                        if (fieldSelector.Count() == 2) //two way recon
                        {
                            var result = allTransactions.Select(x => new TwoWayModel()
                            {
                                TransactionDate = Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[0].Trim(), "TransactionDate"))).ToString("yyyyMMdd"),
                                TerminalId = x.TerminalId,
                                CardNo = x.CardNumber,
                                AccountNo = "'" + x.AccountNo,
                                TraceNo = "'" + x.TraceNo,
                                AuthCode = "'" + x.AuthCode,
                                TerminalType = UiHelpers.UIHelper.GetEnumValue<TerminalType>(x.TerminalType ?? 0).ToString(),
                                Currency = x.Currency != null ? x.Currency.ToString() : "0",
                                TransactionAmountSource = x.NpnAmount == null ? 0 : Convert.ToDouble(x.NpnAmount),
                                ResponseCodeSource = "'" + x.NpnResponseCode + "/" + x.NpnResponseCodeDescription,
                                StatusSource = x.Status,
                                TransactionAmountDestination = Convert.ToDouble(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[1].Trim(), "Amount"))),
                                ResponseCodeDestination = Convert.ToString(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[1].Trim(), "ResponseCode"))),
                                FT_Branch = x.CBSRefValue == null
                                    ? string.Empty
                                    : "'" + x.CBSRefValue.ToString()
                            }).ToList();

                            var dispalyResult = result.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();

                            return Json(new
                            {
                                sEcho = param.sEcho,
                                iTotalRecords = result.Count,
                                iTotalDisplayRecords = result.Count,
                                aaData = dispalyResult
                            },
                                JsonRequestBehavior.AllowGet);
                        }
                        else if (fieldSelector.Count() == 3) // three way recon..
                        {
                            var result = allTransactions.Select(x => new ThreeWayModel()
                            {
                                TransactionDate = fieldSelector[2].ToLower().Equals("visa") ? Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[0].Trim(), "TransactionGmtDate"))).ToString("yyyyMMdd") : Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[0].Trim(), "TransactionDate"))).ToString("yyyyMMdd"),
                                TerminalId = x.TerminalId,
                                CardNo = x.CardNumber,
                                AccountNo = "'" + x.AccountNo,
                                TraceNo = "'" + x.TraceNo,
                                AuthCode = "'" + x.AuthCode,
                                TerminalType = UiHelpers.UIHelper.GetEnumValue<TerminalType>(x.TerminalType ?? 0).ToString(),
                                StatusSource = x.Status,
                                Currency = x.Currency != null ? x.Currency.ToString() : "0",
                                TransactionAmountSource = x.NpnAmount == null ? 0.00 : Convert.ToDouble(x.NpnAmount),
                                ResponseCodeSource = "'" + x.NpnResponseCode + "/" + x.NpnResponseCodeDescription,
                                TransactionAmountDestination1 = Convert.ToDouble(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[1].Trim(), "Amount"))),
                                ResponseCodeDestination1 = Convert.ToString(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[1].Trim(), "ResponseCode"))),
                                TransactionAmountDestination2 = Convert.ToDouble(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[2].Trim(), "Amount"))),
                                ResponseCodeDestination2 = Convert.ToString(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[2].Trim(), "ResponseCode"))),
                                FT_Branch = x.CBSRefValue == null
                                    ? string.Empty
                                    : "'" + x.CBSRefValue.ToString()

                            }).ToList();
                            var dispalyResult = result.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();

                            return Json(new
                            {
                                sEcho = param.sEcho,
                                iTotalRecords = result.Count,
                                iTotalDisplayRecords = result.Count,
                                aaData = dispalyResult
                            },
                                JsonRequestBehavior.AllowGet);
                        }
                    }
            }
            catch (ArgumentException e)
            {
                ModelState.AddModelError("", "");
            }
            return null;
        }
        #endregion

        #region Excel Export for Mobile Banking
        //public ActionResult MobilebankingExportToExcel(Db.Filter filterModel)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        ViewBag.DrpReconTypeData = ReconBAL.ReconList();
        //        return View("Index", InitialVal(filterModel));
        //    }
        //    recon = new Recon();
        //    object[] parametersArray = new object[] { filterModel };
        //    object results = new object();
        //    string ReconMappedMethodName = string.Empty;
        //    DataTable exportDataTable = new DataTable();
        //    string[] exportHeading = null;

        //    Mapper.CreateMap(typeof(Db.Filter), typeof(Db.Filter));
        //    var summaryFilterModel = Mapper.Map<Db.Filter>(filterModel);
        //    string[] reconName = { "Header1", "Header2" };
        //    ViewBag.DrpReconTypeData = ReconBAL.ReconList();
        //    try
        //    {
        //        ReconMappedMethodName = ReconBAL.GetReconTypeMethodName(filterModel.ReconType);

        //        MethodInfo method = recon.GetType().GetMethod(ReconMappedMethodName);
        //        results = method.Invoke(recon, parametersArray);
        //        List<DigitalBankingTransactionViewModel> MobilebankingTransactionViewModel = (List<DigitalBankingTransactionViewModel>)results;
        //        var resultTwoway = MobilebankingTransactionViewModel.Select(x => new EsewaVsCbsViewModel()
        //        {
        //            TransactionDate = Convert.ToDateTime(x.TransactionDate).ToString("ddMMyyyy"),
        //            UniqueId = x.UniqueId,
        //            Account = x.Account,
        //            CbsCrAmount = x.CbsCrAmount == null ? 0.00 : Convert.ToDouble(x.CbsCrAmount),
        //            CbsDrAmount = x.CbsDrAmount == null ? 0.00 : Convert.ToDouble(x.CbsDrAmount),
        //            EsewaAmount = x.EsewaAmount == null ? 0.00 : Convert.ToDouble(x.EsewaAmount),
        //            EsewaId = x.EsewaId,
        //            EsewaStatus = x.EsewaStatus,
        //            EsewaSettlement = x.EsewaSettlement,
        //            Status = x.Status,
        //            NoOfAttempt = x.NoofAttempt,
        //            CbsCrTxnRemarks = x.CbsCrTxnRemarks,
        //            CbsDrTxnRemarks = x.CbsDrTxnRemarks,
        //            CbsCrRemarks = x.CbsCrRemarks,
        //            CbsDrRemarks = x.CbsDrRemarks,
        //            EsewaRemarks = x.EsewaRemarks,
        //            CbsCrOtherDet = x.CbsCrOtherDet,
        //            CbsDrOtherDet = x.CbsDrOtherDet

        //        }).ToList();
        //        exportDataTable = UiHelpers.UIHelper.ToDataTable<EsewaVsCbsViewModel>(resultTwoway);
        //        exportHeading = new[] { "Date", "UniqueTxnId", "Status", "Account", "Cbs_Cr Amount", "Cbs_Cr TxnRemarks", "Cbs_Cr Remarks", "Cbs_Cr OtherDet", "Cbs_Dr Amount", " Cbs_Dr TxnRemarks", "Cbs_Dr Remarks", "Cbs_Dr OtherDet", "EsewaId", "Esewa Amount", "Esewa Status", "Esewa Settlement", "Esewa Remarks", "No Of Attempt" };

        //        this.ExportEsewaExcel(exportDataTable, filterModel, exportHeading);
        //    }
        //    catch (Exception e)
        //    {
        //        return View("Index", Report.ReportFromFilter(summaryFilterModel));
        //    }
        //    try
        //    {
        //        SaveActivityLogData("Report", "MobilebankingExportToExcel", filterModel.ReconTypeName);
        //    }
        //    catch { }

        //    return View("Index", Report.ReportFromFilter(summaryFilterModel));
        //}
        public void ExportEsewaExcel(DataTable dt, Db.Filter filterModel, string[] headers)
        {
            try
            {
                string excelName = filterModel.ReconTypeName + "_" +  filterModel.Status + "_" + (filterModel.FromDate.Equals(filterModel.ToDate) ? filterModel.FromDate : filterModel.FromDate + " To " + filterModel.ToDate);
                var reconName = Regex.Split(filterModel.ReconTypeName, "Vs");


                Dictionary<int, string> dictionary = new Dictionary<int, string>();
                Dictionary<int, int> countDictionary = new Dictionary<int, int>();
                using (var wb = new XLWorkbook())
                {
                    var columns = dt.Columns;
                    var rows = dt.Rows.OfType<DataRow>();
                    DataRow datarwfooter;
                    datarwfooter = dt.NewRow();

                   // getting sum and total records to show in footer..
                    for (int i = 0; i < columns.Count; i++)
                    {
                        var columnName = columns[i].ColumnName;

                        if (columnName.ToLower().Contains("amount"))
                        {
                            var responseCodeColumnName = columns[i + 1].ColumnName; // to count the total txn rows..

                            var columnTotal = rows.Skip(0) // instead of dt_.Rows.IndexOf(row) !=0
                                .Sum(r => r[columnName] == DBNull.Value ? 0.00 : Convert.ToDouble(r[columnName]));

                            var countTotal = rows.Skip(0)
                                .Where(r => !string.IsNullOrEmpty(Convert.ToString(r[responseCodeColumnName])))
                                .ToList().Count;

                            dictionary.Add(i, String.Format("{0:0.00}", columnTotal));  //store in dictionay
                            countDictionary.Add(i++, countTotal);
                        }
                    }

                    // set values to footer array....
                    string[] footer = new string[dt.Columns.Count];
                    for (int i = 0; i < footer.Length; i++)
                    {
                        footer[i] = string.Empty;
                    }
                    var key = dictionary.Take(1).Select(d => d.Key).First() - 1;
                    footer[key] = "Total";
                    foreach (KeyValuePair<Int32, string> keyValuePair in dictionary)
                    {
                        footer[keyValuePair.Key] = keyValuePair.Value.ToString();
                    }
                    foreach (KeyValuePair<Int32, Int32> keyValuePair in countDictionary)
                    {
                        footer[keyValuePair.Key + 1] = keyValuePair.Value.ToString();
                    }

                   // insert footer..
                    for (int i = 0; i < footer.Length; i++)
                    {
                        datarwfooter[i] = footer[i];
                    }
                    dt.Rows.Add(datarwfooter);

                    //insert headers..
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dt.Columns[i].ColumnName = headers[i];
                    }
                    dt.TableName = "Recon";
                    var ws = wb.Worksheets.Add(dt);
                    ws.Row(1).InsertRowsAbove(2);
                    ws.Cell("A1").Value = excelName;
                    ws.Range("A1:R1").Row(1).Merge();
                    ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell("A2").Value = "";
                    ws.Range("A2:C2").Row(1).Merge();
                    ws.Cell("D2").Value = reconName[1] + "Cr";
                    ws.Range("D2:H2").Row(1).Merge();
                    ws.Cell("D2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell("I2").Value = reconName[1] + "Dr";
                    ws.Range("I2:L2").Row(1).Merge();
                    ws.Cell("I2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell("M2").Value = reconName[0];
                    ws.Range("M2:R2").Row(1).Merge();
                    ws.Cell("M2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    wb.Author = "Elite";
                    wb.ShowRowColHeaders = true;
                    ws.Tables.FirstOrDefault().ShowAutoFilter = false;
                    ws.ConditionalFormats.RemoveAll();
                    ws.Tables.FirstOrDefault().ShowColumnStripes = true;
                    ws.Tables.FirstOrDefault().ShowRowStripes = true;
                    ws.Rows(1, 3).Style.Font.Bold = true;
                    var totalRows = ws.RowsUsed().Count();
                    ws.Row(totalRows).Style.Font.Bold = true;

                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition",
                        "attachment;filename=" + excelName + ".xlsx");
                    using (var MyMemoryStream = new MemoryStream())
                    {
                        wb.SaveAs(MyMemoryStream);
                        MyMemoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }
            }
            catch (Exception e)
            {
            }
        }
        #endregion

        #region Excel Export
        public ActionResult ExportToExcel(Db.Filter filterModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.DrpReconTypesData = ReconBAL.GetReconList();
                return View("Index", InitialVal(filterModel));
            }
            recon = new Recon();
            object[] parametersArray = new object[] { filterModel };
            object results = new object();
            string ReconMappedMethodName = string.Empty;
            List<TwoWayModel> resultTwoway = new List<TwoWayModel>();
            List<TwoWayModelDateDiff> resultTwowayDateDiff = new List<TwoWayModelDateDiff>();
            List<ThreeWayModel> resultThreeway = new List<ThreeWayModel>();
            List<ThreeWayModelDateDiff> resultThreewayDateDiff = new List<ThreeWayModelDateDiff>();
            DataTable exportDataTable = new DataTable();
            string[] exportHeading = null;

            Mapper.CreateMap(typeof(Db.Filter), typeof(Db.Filter));
            var summaryFilterModel = Mapper.Map<Db.Filter>(filterModel);
            string[] reconName = { "Header1", "Header2" };

            if (!String.IsNullOrEmpty(filterModel.ReconTypeName))
            {
                reconName = Regex.Split(filterModel.ReconTypeName, "Vs");
            }
            ViewBag.DrpReconTypesData = ReconBAL.GetReconList();
            try
            {
                ReconMappedMethodName = ReconBAL.GetReconTypeMethodName(filterModel.ReconType);

                if (reconName.Count() == 2)
                {
                    if (filterModel.Status.ToLower().Equals("datediff"))
                    {
                        resultTwowayDateDiff = ExportExcelTwoWayDateDiffDataSource(ReconMappedMethodName, parametersArray, resultTwowayDateDiff, reconName);
                        exportDataTable = UiHelpers.UIHelper.ToDataTable<TwoWayModelDateDiff>(resultTwowayDateDiff);
                        exportHeading = new[] { "Terminal", "Card No.", "Account No.", "Trace", "Auth Code", "Terminal Type", "Currency", "Status", reconName[0] + "Date", reconName[0] + "Amount", reconName[0] + "Response Code", reconName[1] + " Date", reconName[1] + " Amount", reconName[1] + " Response Code", "FT\\Branch", "Remarks", "Main Code", "Reference No" };
                    }
                    else
                    {
                        resultTwoway = ExportExcelTwoWayDataSource(ReconMappedMethodName, parametersArray, resultTwoway, reconName);
                        exportDataTable = UiHelpers.UIHelper.ToDataTable<TwoWayModel>(resultTwoway);
                        exportHeading = new[] { "Date", "Terminal", "Card No.", "Account No.", "Trace", "Auth Code", "Terminal Type", "Currency", "Status", reconName[0] + "Amount", reconName[0] + "Response Code", reconName[1] + " Amount", reconName[1] + " Response Code", "FT\\Branch", "Remarks", "Main Code", "Reference No" };
                    }
                }
                else if (reconName.Count() == 3)
                {
                    if (filterModel.Status.ToLower().Equals("datediff"))
                    {
                        resultThreewayDateDiff = ExportExcelThreeWayDateDiffDataSource(ReconMappedMethodName, parametersArray, resultThreewayDateDiff, reconName);
                        exportHeading = new[] { "Terminal", "Card No.", "Account No", "Trace No", "Auth Code", "Terminal Type", "Currency", "Status", reconName[0] + "Date", reconName[0] + "Amount", reconName[0] + "Response Code", reconName[1] + "Date", reconName[1] + "Amount", reconName[1] + "Response Code", reconName[2] + " Date", reconName[2] + " Amount", reconName[2] + " Response Code", "CPD Date", "FT\\Branch", "Remarks", "Main Code", "Reference No", "Visa/Ej Currency" };
                        exportDataTable = UiHelpers.UIHelper.ToDataTable<ThreeWayModelDateDiff>(resultThreewayDateDiff);
                    }
                    else
                    {
                        resultThreeway = ExportExcelThreeWayDataSource(ReconMappedMethodName, parametersArray, resultThreeway, reconName);
                        exportHeading = new[] { "Date", "Terminal", "Card No.", "Account No", "Trace No", "Auth Code", "Terminal Type", "Currency", "Status", reconName[0] + "Amount", reconName[0] + "Response Code", reconName[1] + "Amount", reconName[1] + "Response Code", reconName[2] + " Amount", reconName[2] + " Response Code", "CPD Date", "FT\\Branch", "Remarks", "Main Code", "Reference No", "Visa/Ej Currency" };
                        exportDataTable = UiHelpers.UIHelper.ToDataTable<ThreeWayModel>(resultThreeway);
                    }
                }
                this.ExportCoreExcel(exportDataTable, filterModel, exportHeading);
            }
            catch (Exception e)
            {
                return View("Index", Report.GetReportFromFilter(summaryFilterModel));
            }
            try
            {
                SaveActivityLogData("Report", "ExportToExcel", filterModel.ReconTypeName);
            }
            catch { }

            return View("Index", Report.GetReportFromFilter(summaryFilterModel));
        }
        private List<TwoWayModel> ExportExcelTwoWayDataSource(string ReconMappedMethodName, object[] parametersArray, List<TwoWayModel> result, string[] fieldSelector)
        {
            object results = new object();
            MethodInfo method = recon.GetType().GetMethod(ReconMappedMethodName);
            results = method.Invoke(recon, parametersArray);
            var filtermodel = (Db.Filter)parametersArray[0];
            List<VwTransactionDetailsModel> VwTransactionDetailsModels = (List<VwTransactionDetailsModel>)results;

            foreach (var x in VwTransactionDetailsModels)
            {
                TwoWayModel twoWayModel = new TwoWayModel();

                var cbsResponseCodeDes = Convert.ToString(UiHelpers.UIHelper.GetPropertyValue(x,
                    string.Concat(fieldSelector[0].Trim(), "ResponseCodeDescription")));
                var NpnResponseCodeDes = Convert.ToString(UiHelpers.UIHelper.GetPropertyValue(x,
                    string.Concat(fieldSelector[1].Trim(), "ResponseCodeDescription")));


                twoWayModel.TransactionDate = Convert
                    .ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x,
                        string.Concat(fieldSelector[0].Trim(), "TransactionDate"))).ToString("yyyyMMdd");
                twoWayModel.TerminalId = x.TerminalId;
                twoWayModel.CardNo = x.CardNumber;
                twoWayModel.AccountNo = x.AccountNo;
                twoWayModel.TraceNo = x.TraceNo;
                twoWayModel.AuthCode = x.AuthCode;
                twoWayModel.TerminalType = UiHelpers.UIHelper.GetEnumValue<TerminalType>(x.TerminalType ?? 0).ToString();
                twoWayModel.Currency = x.Currency != null ? x.Currency.ToString() : "0";
                twoWayModel.StatusSource = x.Status;
                twoWayModel.TransactionAmountSource = x.NpnAmount == null ? 0.00 : Convert.ToDouble(x.NpnAmount);
                twoWayModel.ResponseCodeSource = "'" + x.NpnResponseCode + "/" + x.NpnResponseCodeDescription;
                twoWayModel.TransactionAmountDestination = Convert.ToDouble(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[1].Trim(), "Amount")));
                twoWayModel.ResponseCodeDestination =
                    //                                          Convert.ToString(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[1].Trim(), "ResponseCode")))
                    //                                          + "/" + 
                    NpnResponseCodeDes;
                twoWayModel.FT_Branch = x.CBSRefValue == null
                    ? string.Empty
                    : "'" + x.CBSRefValue.ToString();


                if (NpnResponseCodeDes.ToLower().Equals("reversal") && cbsResponseCodeDes.ToLower().Equals("reversal"))
                {
                    twoWayModel.Remarks = "Auto Reversal";
                }
                else
                {
                    twoWayModel.Remarks =
                        filtermodel.IsOwnUsPayableReceivable.ToLower().Equals("receivable") &&
                        x.Status.ToLower().Equals("invalid")
                            ? x.TerminalType == 1
                                ? "Atm Loro txn not auto posted"
                                : x.TerminalType == 2
                                    ? "Pos txns to be settled"
                                    : ""
                            : "";

                    if (x.CardType == (int)CardType.MasterCard)
                    {
                        twoWayModel.Remarks += twoWayModel.Remarks + " Master Card";
                    }
                    if (filtermodel.IsOwnUsPayableReceivable.ToLower().Equals("payable"))
                    {
                        if (x.Issuing_Bank > 0)
                        {
                            if (ReconBAL.IsMemberBankPayable(x.Issuing_Bank))
                            {
                                twoWayModel.Remarks += " (Member Bank)";
                            }
                        }
                    }
                    else
                    {
                        if (ReconBAL.IsMemberBank(x.CardNumber))
                        {
                            twoWayModel.Remarks += " (Member Bank)";
                        }
                    }

                }
                twoWayModel.CbsMainCode = x.CbsMainCode;
                twoWayModel.ReferenceNo = x.ReferenceNo;
                result.Add(twoWayModel);
            }
            return result;
        }
        private List<TwoWayModelDateDiff> ExportExcelTwoWayDateDiffDataSource(string ReconMappedMethodName, object[] parametersArray, List<TwoWayModelDateDiff> result, string[] fieldSelector)
        {
            object results = new object();
            MethodInfo method = recon.GetType().GetMethod(ReconMappedMethodName);
            results = method.Invoke(recon, parametersArray);
            var filtermodel = (Db.Filter)parametersArray[0];
            var DateFrom = GlobalHelper.ChangeDateFormat(filtermodel.FromDate);

            List<VwTransactionDetailsModel> VwTransactionDetailsModels = (List<VwTransactionDetailsModel>)results;
            //result = VwTransactionDetailsModels.Select(x => new TwoWayModelDateDiff()
            //{
            //    TerminalId = x.TerminalId,
            //    CardNo = x.CardNumber,
            //    AccountNo = x.AccountNo,
            //    TraceNo = x.TraceNo,
            //    AuthCode = x.AuthCode,
            //    TerminalType = UiHelpers.UIHelper.GetEnumValue<TerminalType>(x.TerminalType ?? 0).ToString(),
            //    Currency = x.Currency != null ? x.Currency.ToString() : "0",
            //    StatusSource = x.Status,
            //    SourceTransactionDate = Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[0].Trim(), "TransactionDate"))).ToString("yyyyMMdd"),
            //    TransactionAmountSource = x.NpnAmount == null ? 0.00 : Convert.ToDouble(x.NpnAmount),
            //    ResponseCodeSource = "'" + x.NpnResponseCode + "/" + x.NpnResponseCodeDescription,
            //    DestinationTransactionDate = Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[1].Trim(), "TransactionDate"))).ToString("yyyyMMdd"),
            //    TransactionAmountDestination = Convert.ToDouble(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[1].Trim(), "Amount"))),
            //    ResponseCodeDestination = Convert.ToString(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[1].Trim(), "ResponseCode"))),
            //    FT_Branch = x.CBSRefValue == null
            //        ? string.Empty
            //        : "'" + x.CBSRefValue.ToString(),
            //    Remarks =
            //              Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[0].Trim(), "TransactionDate"))) != DateFrom ?
            //                  Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[0].Trim(), "TransactionDate"))).ToString("yyyyMMdd") :
            //              Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[0].Trim(), "TransactionDate"))) > Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[1].Trim(), "TransactionDate")))
            //              ? "  Txn of prev date in CBS "
            //              : Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[0].Trim(), "TransactionDate"))) < Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[1].Trim(), "TransactionDate")))
            //                          ? "  Transaction of Next Date in CBS " : "",


            //    CbsMainCode = x.CbsMainCode,
            //    ReferenceNo = x.ReferenceNo
            //}).ToList();
            TwoWayModelDateDiff twoWayModelDateDiff = new TwoWayModelDateDiff();
            foreach (var x in VwTransactionDetailsModels)
            {
                twoWayModelDateDiff.TerminalId = x.TerminalId;
                twoWayModelDateDiff.CardNo = x.CardNumber;
                twoWayModelDateDiff.AccountNo = x.AccountNo;
                twoWayModelDateDiff.TraceNo = x.TraceNo;
                twoWayModelDateDiff.AuthCode = x.AuthCode;
                twoWayModelDateDiff.TerminalType = UiHelpers.UIHelper.GetEnumValue<TerminalType>(x.TerminalType ?? 0).ToString();
                twoWayModelDateDiff.Currency = x.Currency != null ? x.Currency.ToString() : "0";
                twoWayModelDateDiff.StatusSource = x.Status;
                twoWayModelDateDiff.SourceTransactionDate = Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[0].Trim(), "TransactionDate"))).ToString("yyyyMMdd");
                twoWayModelDateDiff.TransactionAmountSource = x.NpnAmount == null ? 0.00 : Convert.ToDouble(x.NpnAmount);
                twoWayModelDateDiff.ResponseCodeSource = "'" + x.NpnResponseCode + "/" + x.NpnResponseCodeDescription;
                twoWayModelDateDiff.DestinationTransactionDate = Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[1].Trim(), "TransactionDate"))).ToString("yyyyMMdd");
                twoWayModelDateDiff.TransactionAmountDestination = Convert.ToDouble(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[1].Trim(), "Amount")));
                twoWayModelDateDiff.ResponseCodeDestination = Convert.ToString(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[1].Trim(), "ResponseCode")));
                twoWayModelDateDiff.FT_Branch = x.CBSRefValue == null
                    ? string.Empty
                    : "'" + x.CBSRefValue.ToString();
                twoWayModelDateDiff.Remarks =
                          Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[0].Trim(), "TransactionDate"))) != DateFrom ?
                              Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[0].Trim(), "TransactionDate"))).ToString("yyyyMMdd") :
                          Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[0].Trim(), "TransactionDate"))) > Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[1].Trim(), "TransactionDate")))
                          ? "  Txn of prev date in CBS "
                          : Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[0].Trim(), "TransactionDate"))) < Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[1].Trim(), "TransactionDate")))
                                      ? "  Transaction of Next Date in CBS " : "";
                if (ReconBAL.IsMemberBank(x.CardNumber))
                {
                    twoWayModelDateDiff.Remarks += " (Member Bank)";
                }

                twoWayModelDateDiff.CbsMainCode = x.CbsMainCode;
                twoWayModelDateDiff.ReferenceNo = x.ReferenceNo;
                result.Add(twoWayModelDateDiff);

            }
            return result;
        }
        private List<ThreeWayModel> ExportExcelThreeWayDataSource(string ReconMappedMethodName, object[] parametersArray, List<ThreeWayModel> result, string[] fieldSelector)
        {
            object results = new object();
            MethodInfo method = recon.GetType().GetMethod(ReconMappedMethodName);
            results = method.Invoke(recon, parametersArray);
            var filtermodel = (Db.Filter)parametersArray[0];
            var DateFrom = GlobalHelper.ChangeDateFormat(filtermodel.FromDate);

            string Remarks = string.Empty, AdditionalRemark = string.Empty;

            List<VwTransactionDetailsModel> VwTransactionDetailsModels = (List<VwTransactionDetailsModel>)results;
            result.Clear();
            foreach (var x in VwTransactionDetailsModels)
            {
                ThreeWayModel threeWayModel = new ThreeWayModel();

                var cbsResponseCodeDes = Convert.ToString(UiHelpers.UIHelper.GetPropertyValue(x,
                    string.Concat(fieldSelector[0].Trim(), "ResponseCodeDescription")));
                var NpnResponseCodeDes = Convert.ToString(UiHelpers.UIHelper.GetPropertyValue(x,
                    string.Concat(fieldSelector[1].Trim(), "ResponseCodeDescription")));
                var visaOrEjResponseCodeDes = Convert.ToString(UiHelpers.UIHelper.GetPropertyValue(x,
                    string.Concat(fieldSelector[2].Trim(), "ResponseCodeDescription")));

                threeWayModel.TransactionDate = fieldSelector[2].ToLower().Equals("visa")
                    ? Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x,
                        string.Concat(fieldSelector[0].Trim(), "TransactionGmtDate"))).ToString("yyyyMMdd")
                    : Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x,
                        string.Concat(fieldSelector[0].Trim(), "TransactionDate"))).ToString("yyyyMMdd");

                threeWayModel.TerminalId = x.TerminalId;
                threeWayModel.CardNo = x.CardNumber;
                threeWayModel.AccountNo = x.AccountNo;
                threeWayModel.TraceNo = x.TraceNo;
                threeWayModel.AuthCode = x.AuthCode;
                threeWayModel.TerminalType = UiHelpers.UIHelper.GetEnumValue<TerminalType>(x.TerminalType ?? 0).ToString();
                threeWayModel.StatusSource = x.Status;
                threeWayModel.Currency = x.Currency != null ? x.Currency.ToString() : "0";
                threeWayModel.TransactionAmountSource = x.NpnAmount == null ? 0.00 : Convert.ToDouble(x.NpnAmount);
                threeWayModel.ResponseCodeSource = "'" + x.NpnResponseCode + "/" + x.NpnResponseCodeDescription;
                threeWayModel.TransactionAmountDestination1 = Convert.ToDouble(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[1].Trim(), "Amount")));
                threeWayModel.ResponseCodeDestination1 =
                    Convert.ToString(
                        UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[1].Trim(), "ResponseCode")));
                threeWayModel.TransactionAmountDestination2 = Convert.ToDouble(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[2].Trim(), "Amount")));
                threeWayModel.ResponseCodeDestination2 = Convert.ToString(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[2].Trim(), "ResponseCode")));
                if (UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[2].Trim(), "AdviseDate")) != null)
                {
                    threeWayModel.CPDDate = Convert
                        .ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x,
                            string.Concat(fieldSelector[2].Trim(), "AdviseDate"))).ToString("yyyyMMdd");
                }
                threeWayModel.FT_Branch = x.CBSRefValue == null
                    ? string.Empty
                    : "'" + x.CBSRefValue.ToString();

                if (!fieldSelector[2].Trim().ToLower().Equals("ej"))
                {
                    if (filtermodel.IsOwnUsPayableReceivable.ToLower().Equals("receivable") &&
                        x.Status.ToLower().Equals("invalid"))
                    {
                        Remarks =
                            x.CbsTransactionStatus == null
                                ? x.TerminalType == 1
                                    ? "Atm Loro txn not auto posted"
                                    : x.TerminalType == 2
                                        ? "Pos txns to be settled"
                                        : ""
                                : "Missing at Visa";
                    }
                    else if (filtermodel.IsOwnUsPayableReceivable.ToLower().Equals("payable") &&
                             x.Status.ToLower().Equals("invalid"))
                    {
                        Remarks = x.CardNumber.Substring(0, 6).Equals("430728")
                            ? "Mbnl Credit Card txn"
                            : x.CardNumber.Substring(0, 6).Equals("430719")
                                ? "Mbnl Dollar Prepaid Card txn"
                                : "Missing at Visa";
                    }

                    if (!Remarks.Equals("Missing at Visa")) //if the txn only missing at visa non it to concate extra info..
                    {
                        AdditionalRemark = string.IsNullOrEmpty(x.VisaResponseCode)
                            ? string.IsNullOrEmpty(Remarks) ? "Missing at visa" : " & Missing at visa"
                            : "";
                    }



                    threeWayModel.Remarks = Remarks + AdditionalRemark;

                    if (cbsResponseCodeDes.ToLower().Equals("reversal") &&
                        NpnResponseCodeDes.ToLower().Equals("reversal") &&
                        visaOrEjResponseCodeDes.ToLower().Equals("reversal"))
                        threeWayModel.Remarks = "Auto Reversal";
                }
                else //for ej
                {
                    if (cbsResponseCodeDes.ToLower().Equals("reversal") &&
                        NpnResponseCodeDes.ToLower().Equals("reversal") &&
                        string.IsNullOrEmpty(visaOrEjResponseCodeDes))
                        threeWayModel.Remarks = "Auto Reversal";

                    if (x.EjResponseCode == "0000" && x.EjTransactionStatus == (int)TransactionStatus.Fail)
                    {
                        threeWayModel.Remarks += " RETRACTED / ERROR";
                    }
                }
                threeWayModel.CbsMainCode = x.CbsMainCode;
                threeWayModel.ReferenceNo = x.ReferenceNo;
                threeWayModel.VisaCurrency = Convert.ToString(UiHelpers.UIHelper.GetPropertyValue(x,
                    string.Concat(fieldSelector[2].Trim(), "Currency")));

                result.Add(threeWayModel);
            }
            return result;
        }
        private List<ThreeWayModelDateDiff> ExportExcelThreeWayDateDiffDataSource(string ReconMappedMethodName, object[] parametersArray, List<ThreeWayModelDateDiff> result, string[] fieldSelector)
        {
            object results = new object();
            MethodInfo method = recon.GetType().GetMethod(ReconMappedMethodName);
            results = method.Invoke(recon, parametersArray);
            var filtermodel = (Db.Filter)parametersArray[0];
            var DateFrom = GlobalHelper.ChangeDateFormat(filtermodel.FromDate);


            List<VwTransactionDetailsModel> VwTransactionDetailsModels = (List<VwTransactionDetailsModel>)results;
            result.Clear();
            foreach (var x in VwTransactionDetailsModels)
            {
                ThreeWayModelDateDiff threeWayModelDateDiff = new ThreeWayModelDateDiff();

                threeWayModelDateDiff.TerminalId = x.TerminalId;
                threeWayModelDateDiff.CardNo = x.CardNumber;
                threeWayModelDateDiff.AccountNo = x.AccountNo;
                threeWayModelDateDiff.TraceNo = x.TraceNo;
                threeWayModelDateDiff.AuthCode = x.AuthCode;
                threeWayModelDateDiff.TerminalType = UiHelpers.UIHelper.GetEnumValue<TerminalType>(x.TerminalType ?? 0).ToString();
                threeWayModelDateDiff.Currency = x.Currency != null ? x.Currency.ToString() : "0";
                threeWayModelDateDiff.StatusSource = x.Status;
                threeWayModelDateDiff.TransactionDateSource = Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[0].Trim(), "TransactionDate"))).ToString("yyyyMMdd");
                threeWayModelDateDiff.TransactionAmountSource = x.NpnAmount == null ? 0.00 : Convert.ToDouble(x.NpnAmount);
                threeWayModelDateDiff.ResponseCodeSource = "'" + x.NpnResponseCode + "/" + x.NpnResponseCodeDescription;
                threeWayModelDateDiff.TransactionDateDestination1 = Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[1].Trim(), "TransactionDate"))).ToString("yyyyMMdd");
                threeWayModelDateDiff.TransactionAmountDestination1 = Convert.ToDouble(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[1].Trim(), "Amount")));
                threeWayModelDateDiff.ResponseCodeDestination1 = Convert.ToString(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[1].Trim(), "ResponseCode")));
                //                threeWayModelDateDiff.TransactionDateDestination2 = Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[2].Trim(), "TransactionDate"))).ToString("yyyyMMdd");

                threeWayModelDateDiff.TransactionDateDestination2 = fieldSelector[2].ToLower().Equals("visa")
                    ? Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x,
                        string.Concat(fieldSelector[0].Trim(), "TransactionGmtDate"))).ToString("yyyyMMdd")
                    : Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x,
                        string.Concat(fieldSelector[2].Trim(), "TransactionDate"))).ToString("yyyyMMdd");

                threeWayModelDateDiff.TransactionAmountDestination2 = Convert.ToDouble(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[2].Trim(), "Amount")));
                threeWayModelDateDiff.ResponseCodeDestination2 = Convert.ToString(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[2].Trim(), "ResponseCode")));
                if (UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[2].Trim(), "AdviseDate")) != DBNull.Value)
                {
                    threeWayModelDateDiff.CPDDate = Convert
                        .ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x,
                            string.Concat(fieldSelector[2].Trim(), "AdviseDate"))).ToString("yyyyMMdd");
                }
                threeWayModelDateDiff.FT_Branch = x.CBSRefValue == null
                    ? string.Empty
                    : "'" + x.CBSRefValue.ToString();

                threeWayModelDateDiff.Remarks =
                    Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x,
                        string.Concat(fieldSelector[0].Trim(), "TransactionDate"))) != DateFrom
                        ? "  " + Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[0].Trim(), "TransactionDate"))).ToString("yyyyMMdd")
                        : Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[0].Trim(), "TransactionDate"))) >
                          Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[1].Trim(), "TransactionDate"))) &&
                          Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[0].Trim(), "TransactionDate"))) >
                          Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[2].Trim(), "TransactionDate")))
                            ? "  Txn of Prev Date in CBS & VISA "
                            : Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[0].Trim(), "TransactionDate"))) <
                              Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[1].Trim(), "TransactionDate"))) &&
                              Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[0].Trim(), "TransactionDate"))) <
                              Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[2].Trim(), "TransactionDate")))
                                ? "  Txn of Next Date in CBS & VISA "
                        : Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[0].Trim(), "TransactionDate"))) >
                          Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[1].Trim(), "TransactionDate")))
                            ? "  Txn of Prev date in CBS "
                            : Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[0].Trim(), "TransactionDate"))) <
                              Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[1].Trim(), "TransactionDate")))
                                ? "  Txn of Next Date in CBS "
                                : Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[0].Trim(), "TransactionDate"))) >
                              Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[2].Trim(), "TransactionDate")))
                                  ? "  Txn of Prev date in VISA "
                                  : Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[0].Trim(), "TransactionDate"))) <
                              Convert.ToDateTime(UiHelpers.UIHelper.GetPropertyValue(x, string.Concat(fieldSelector[2].Trim(), "TransactionDate")))
                                ? "  Txn of Next Date in VISA "
                                : "";
                threeWayModelDateDiff.CbsMainCode = x.CbsMainCode;
                threeWayModelDateDiff.ReferenceNo = x.ReferenceNo;
                threeWayModelDateDiff.VisaCurrency = Convert.ToString(UiHelpers.UIHelper.GetPropertyValue(x,
                    string.Concat(fieldSelector[2].Trim(), "Currency")));
                result.Add(threeWayModelDateDiff);
            }
            return result;
        }
        private static void TwoWayWorkSheetHeaderTemplate(IXLWorksheet ws, string excelName, string[] reconName)
        {
            ws.Row(1).InsertRowsAbove(2);
            ws.Cell("A1").Value = excelName;
            ws.Range("A1:O1").Row(1).Merge();
            ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A2").Value = "";
            ws.Range("A2:I2").Row(1).Merge();
            ws.Cell("J2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("J2").Value = reconName[0];
            ws.Range("J2:K2").Row(1).Merge();
            ws.Cell("L2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("L2").Value = reconName[1];
            ws.Range("L2:M2").Row(1).Merge();
        }
        private static void TwoWayDateDiffWorkSheetHeaderTemplate(IXLWorksheet ws, string excelName, string[] reconName)
        {
            ws.Row(1).InsertRowsAbove(2);
            ws.Cell("A1").Value = excelName;
            ws.Range("A1:P1").Row(1).Merge();
            ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A2").Value = "";
            ws.Range("A2:I2").Row(1).Merge();
            ws.Cell("J2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("J2").Value = reconName[0];
            ws.Range("J2:K2").Row(1).Merge();
            ws.Cell("L2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("L2").Value = reconName[1];
            ws.Range("L2:N2").Row(1).Merge();
            ws.Cell("O2").Value = "";
        }
        private static void ThreeWayWorkSheetHeaderTemplate(IXLWorksheet ws, string excelName, string[] reconName)
        {
            ws.Row(1).InsertRowsAbove(2);
            ws.Cell("A1").Value = excelName;
            ws.Range("A1:R1").Row(1).Merge();
            ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A2").Value = "";
            ws.Range("A2:I2").Row(1).Merge();
            ws.Cell("J2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("J2").Value = reconName[0];
            ws.Range("J2:K2").Row(1).Merge();
            ws.Cell("L2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("L2").Value = reconName[1];
            ws.Range("L2:M2").Row(1).Merge();
            ws.Cell("N2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("N2").Value = reconName[2];
            ws.Range("N2:O2").Row(1).Merge();
            ws.Range("P2:R2").Row(1).Merge();
        }
        private static void ThreeWayDateDiffWorkSheetHeaderTemplate(IXLWorksheet ws, string excelName, string[] reconName)
        {
            ws.Row(1).InsertRowsAbove(2);
            ws.Cell("A1").Value = excelName;
            ws.Range("A1:T1").Row(1).Merge();
            ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A2").Value = "";
            ws.Range("A2:H2").Row(1).Merge();
            ws.Cell("I2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("I2").Value = reconName[0];
            ws.Range("I2:K2").Row(1).Merge();
            ws.Cell("L2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("L2").Value = reconName[1];
            ws.Range("L2:N2").Row(1).Merge();
            ws.Cell("O2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("O2").Value = reconName[2];
            ws.Range("O2:Q2").Row(1).Merge();
            ws.Range("R2:T2").Row(1).Merge();
        }
        public MemoryStream GetStream(XLWorkbook excelWorkbook)
        {
            MemoryStream fs = new MemoryStream();
            excelWorkbook.SaveAs(fs);
            fs.Position = 0;
            return fs;
        }
        public void ExportCoreExcel(DataTable dt, Db.Filter filterModel, string[] headers)
        {
            try
            {
                string excelName = filterModel.ReconTypeName + "_" + filterModel.TerminalId + "_" + filterModel.IsOwnUsPayableReceivable + "_" +
                                   filterModel.Status + "_" +
                                   (filterModel.FromDate.Equals(filterModel.ToDate) ? filterModel.FromDate : filterModel.FromDate + " To " + filterModel.ToDate);
                var reconName = Regex.Split(filterModel.ReconTypeName, "Vs");


                Dictionary<int, string> dictionary = new Dictionary<int, string>();
                Dictionary<int, int> countDictionary = new Dictionary<int, int>();
                using (var wb = new XLWorkbook())
                {
                    var columns = dt.Columns;
                    var rows = dt.Rows.OfType<DataRow>();
                    DataRow datarwfooter;
                    datarwfooter = dt.NewRow();

                    //getting sum and total records to show in footer ..
                    for (int i = 0; i < columns.Count; i++)
                    {
                        var columnName = columns[i].ColumnName;

                        if (columnName.ToLower().Contains("amount"))
                        {
                            var responseCodeColumnName = columns[i + 1].ColumnName; // to count the total txn rows..

                            var columnTotal = rows.Skip(0) // instead of dt_.Rows.IndexOf(row) !=0
                                .Sum(r => r[columnName] == DBNull.Value ? 0.00 : Convert.ToDouble(r[columnName]));

                            var countTotal = rows.Skip(0)
                                .Where(r => !string.IsNullOrEmpty(Convert.ToString(r[responseCodeColumnName])))
                                .ToList().Count;

                            dictionary.Add(i, String.Format("{0:0.00}", columnTotal));  //store in dictionay
                            countDictionary.Add(i++, countTotal);
                        }
                    }

                    // set values to footer array....
                    string[] footer = new string[dt.Columns.Count];
                    for (int i = 0; i < footer.Length; i++)
                    {
                        footer[i] = string.Empty;
                    }
                    var key = dictionary.Take(1).Select(d => d.Key).First() - 1;
                    footer[key] = "Total";
                    foreach (KeyValuePair<Int32, string> keyValuePair in dictionary)
                    {
                        footer[keyValuePair.Key] = keyValuePair.Value.ToString();
                    }
                    foreach (KeyValuePair<Int32, Int32> keyValuePair in countDictionary)
                    {
                        footer[keyValuePair.Key + 1] = keyValuePair.Value.ToString();
                    }

                    //insert footer..
                    for (int i = 0; i < footer.Length; i++)
                    {
                        datarwfooter[i] = footer[i];
                    }
                    dt.Rows.Add(datarwfooter);

                    //insert headers..
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dt.Columns[i].ColumnName = headers[i];
                    }
                    dt.TableName = "Recon";
                    var ws = wb.Worksheets.Add(dt);


                    if (reconName.Length == 2)
                    {
                        if (filterModel.Status.ToLower().Equals("datediff"))
                        {
                            TwoWayDateDiffWorkSheetHeaderTemplate(ws, excelName, reconName);
                        }
                        else
                        {
                            TwoWayWorkSheetHeaderTemplate(ws, excelName, reconName);
                        }
                    }
                    else if (reconName.Length == 3)
                    {

                        if (filterModel.Status.ToLower().Equals("datediff"))
                        {
                            ThreeWayDateDiffWorkSheetHeaderTemplate(ws, excelName, reconName);
                        }
                        else
                        {
                            ThreeWayWorkSheetHeaderTemplate(ws, excelName, reconName);
                        }
                    }
                    wb.Author = "Elite";
                    wb.ShowRowColHeaders = true;
                    //                    foreach (IXLWorksheet workSheet in wb.Worksheets)
                    //                    {
                    //                        foreach (IXLTable table in workSheet.Tables)
                    //                        {
                    //                            workSheet.Table(table.Name).ShowAutoFilter = true;
                    //                            workSheet.ConditionalFormats.RemoveAll();
                    //                            workSheet.Table(table.Name).ShowColumnStripes = true;
                    //                            workSheet.Table(table.Name).ShowRowStripes = true;
                    //                        }
                    //                    }
                    ws.Tables.FirstOrDefault().ShowAutoFilter = false;
                    ws.ConditionalFormats.RemoveAll();
                    ws.Tables.FirstOrDefault().ShowColumnStripes = true;
                    ws.Tables.FirstOrDefault().ShowRowStripes = true;
                    ws.Rows(1, 3).Style.Font.Bold = true;
                    var totalRows = ws.RowsUsed().Count();
                    ws.Row(totalRows).Style.Font.Bold = true;

                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition",
                        "attachment;filename=" + excelName + ".xlsx");
                    using (var MyMemoryStream = new MemoryStream())
                    {
                        wb.SaveAs(MyMemoryStream);
                        MyMemoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }
            }
            catch (Exception e)
            {
            }
        }
        protected List<VwTransactionDetailsModel> GetOnUs(List<VwTransactionDetailsModel> listVwTransactionDetailsModel)
        {
            return listVwTransactionDetailsModel.Where(tr => tr.TerminalOwner == (int)TerminalOwner.OwnTerminal
                                                             && tr.CardType == (int)CardType.OwnCard).ToList();
        }
        protected List<VwTransactionDetailsModel> GetPayable(List<VwTransactionDetailsModel> listVwTransactionDetailsModel)
        {
            return listVwTransactionDetailsModel.Where(tr => tr.TerminalOwner != (int)TerminalOwner.OwnTerminal
                                                             && (tr.CardType == (int)CardType.OwnCard || tr.CardType ==
                                                                 (int)CardType.CreditCard)
            ).ToList();
        }
        protected List<VwTransactionDetailsModel> GetReceivable(List<VwTransactionDetailsModel> listVwTransactionDetailsModel)
        {
            return listVwTransactionDetailsModel.Where(tr => tr.TerminalOwner == (int)TerminalOwner.OwnTerminal
                                                             && tr.CardType != (int)CardType.OwnCard).ToList();
        }
        protected double GetTotalAmountAfterReconcile(List<VwTransactionDetailsModel> listVwTransactionDetailsModel)
        {
            return listVwTransactionDetailsModel.Select(x => x.NpnAmount).DefaultIfEmpty(0.00).Sum() ?? 0.00;
        }
        protected List<Transaction> GetOnUs(List<Transaction> listTransaction)
        {
            return listTransaction.Where(tr => tr.TerminalOwner == TerminalOwner.OwnTerminal
                                                             && tr.CardType == CardType.OwnCard).ToList();
        }
        protected List<Transaction> GetPayable(List<Transaction> listTransaction)
        {
            return listTransaction.Where(tr => tr.TerminalOwner != TerminalOwner.OwnTerminal
                                                             && (tr.CardType == CardType.OwnCard
                                                             || tr.CardType == CardType.CreditCard)
                                                             ).ToList();
        }
        protected List<Transaction> GetReceivable(List<Transaction> listTransaction)
        {
            return listTransaction.Where(tr => tr.TerminalOwner == TerminalOwner.OwnTerminal
                                                             && tr.CardType != CardType.OwnCard).ToList();
        }
        protected decimal GetTotalAmountBeforeReconcile(List<Transaction> listVwTransactionDetailsModel)
        {
            return listVwTransactionDetailsModel.Select(x => x.TransactionAmount).DefaultIfEmpty(0).Sum();
        }
        protected int SummaryCount(int RawCbsTotalCount, int FailedTransactionsCount, int InvalidTransactionsCount, int PrevDateTransactionsCount, int NextDateTransactionsCount, int CbsInvalid)
        {
            var total = RawCbsTotalCount - FailedTransactionsCount + InvalidTransactionsCount +
                        PrevDateTransactionsCount + NextDateTransactionsCount - CbsInvalid;

            return total;
        }
        protected double SummaryAmount(decimal RawCbsTotalAmount, double FailedTransactionsAmount, double InvalidTransactionsAmount, double PrevDateTransactionsAmount, double NextDateTransactionsAmount, double CbsInvalidAmount)
        {
            var total = Convert.ToDouble(RawCbsTotalAmount) - FailedTransactionsAmount + InvalidTransactionsAmount +
                        PrevDateTransactionsAmount + NextDateTransactionsAmount - CbsInvalidAmount;

            return total;
        }
        public ActionResult ExportSummeryReport(Db.Filter filterModel)
        {
            if (Regex.Split(filterModel.ReconTypeName, "Vs").Length == 2)
            {
                ExportSummeryReportTwoWay(filterModel);
            }
            else
            {
                ExportSummeryReportThreeWay(filterModel);
            }
            return View();
        }
        public ActionResult ExportSummeryReportTwoWay(Db.Filter filterModel)
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                var dashboardresult = Dashboard.DashboardFromFilter(filterModel);

                var DateFrom = Helper.GlobalHelpers.GlobalHelper.ChangeDateFormat(filterModel.FromDate);
                var DateTo = Helper.GlobalHelpers.GlobalHelper.ChangeDateFormat(filterModel.ToDate);
                string excelName = filterModel.ReconTypeName + "_Summary Report_" +
                                   (filterModel.FromDate.Equals(filterModel.ToDate) ? filterModel.FromDate : filterModel.FromDate + " To " + filterModel.ToDate);

                List<Transaction> rawNpn = new List<Transaction>();
                List<Transaction> rawCbs = new List<Transaction>();
                using (var context = new ReconContext())
                {
                    rawNpn = context.Transactions.Where(x =>
                        x.Source_SourceId == 4
                        && (x.TransactionStatus == TransactionStatus.Success || x.TransactionStatus == TransactionStatus.Success_With_Suspected)
                        && x.TransactionDate >= DateFrom
                        && x.TransactionDate <= DateTo
                        && x.TransactionType == TransactionType.Financial).ToList();


                    rawCbs = context.Transactions.Where(x =>
                        x.Source_SourceId == 3 && x.TransactionStatus == TransactionStatus.Success &&
                        x.TransactionDate >= DateFrom
                        && x.TransactionDate <= DateTo
                        && x.TransactionType == TransactionType.Financial).ToList();
                }

                var reconResult = new Recon().GetVwTransactionDetailsModels(filterModel);

                var RawNpnOnUs = GetOnUs(rawNpn).Count;
                var RawCbsOnUs = GetOnUs(rawCbs).Count - dashboardresult.OnUsSummary.CbsInvalid;
                var RawNpnOnUsAmount = GetTotalAmountBeforeReconcile(GetOnUs(rawNpn));
                var RawCbsOnUsAmount = GetTotalAmountBeforeReconcile(GetOnUs(rawCbs)) - Convert.ToDecimal(dashboardresult.OnUsSummary.CbsInvalid_Amount);


                var OnUsDateDiff = GetOnUs(reconResult.Where(x => x.Status.Equals(ReconStatus.DateDiff.ToString())).ToList());
                var OnUsPrevDate = OnUsDateDiff.Where(x => x.NpnTransactionDate > x.CbsTransactionDate).Count();
                var OnUsNextDate = OnUsDateDiff.Where(x => x.NpnTransactionDate < x.CbsTransactionDate).Count();

                var OnUsPrevDateAmount = GetTotalAmountAfterReconcile(OnUsDateDiff.Where(x => x.NpnTransactionDate > x.CbsTransactionDate).ToList());
                var OnUsNextDateAmount = GetTotalAmountAfterReconcile(OnUsDateDiff.Where(x => x.NpnTransactionDate < x.CbsTransactionDate).ToList());


                var PayableDateDiff = GetPayable(reconResult.Where(x => x.Status.Equals(ReconStatus.DateDiff.ToString())).ToList());
                var PayablePrevDate = PayableDateDiff.Where(x => x.NpnTransactionDate > x.CbsTransactionDate && x.NpnTransactionDate == DateFrom).Count();
                var PayableNextDate = PayableDateDiff.Where(x => x.NpnTransactionDate < x.CbsTransactionDate && x.NpnTransactionDate == DateFrom).Count();
                var PayableRemainingDate = PayableDateDiff.Where(x => x.NpnTransactionDate != DateFrom);

                var PayablePrevDateAmount = GetTotalAmountAfterReconcile(PayableDateDiff.Where(x => x.NpnTransactionDate > x.CbsTransactionDate && x.NpnTransactionDate == DateFrom).ToList());
                var PayableNextDateAmount = GetTotalAmountAfterReconcile(PayableDateDiff.Where(x => x.NpnTransactionDate < x.CbsTransactionDate && x.NpnTransactionDate == DateFrom).ToList());
                var PayableRemainingDateAmount = GetTotalAmountAfterReconcile(PayableRemainingDate.Where(x => x.NpnTransactionDate != DateFrom).ToList());


                var RawNpnPayable = GetPayable(rawNpn).Count;
                var RawCbsPayable = GetPayable(rawCbs).Count - PayableRemainingDate.Count() - dashboardresult.PayableSummary.CbsInvalid;
                //                var RawCbsPayable = GetPayable(rawCbs).Count;
                var RawNpnPayableAmount = GetTotalAmountBeforeReconcile(GetPayable(rawNpn));
                var RawCbsPayableAmount = GetTotalAmountBeforeReconcile(GetPayable(rawCbs)) -
                                          Convert.ToDecimal(PayableRemainingDateAmount) -
                                          Convert.ToDecimal(dashboardresult.PayableSummary.CbsInvalid_Amount);
                //                var RawCbsPayableAmount = GetTotalAmountBeforeReconcile(GetPayable(rawCbs));



                var RawNpnReceivable = GetReceivable(rawNpn).Count;
                var RawCbsReceivable = GetReceivable(rawCbs).Count - dashboardresult.RecievableSummary.CbsInvalid;
                var RawNpnReceivableAmount = GetTotalAmountBeforeReconcile(GetReceivable(rawNpn));
                var RawCbsReceivableAmount = GetTotalAmountBeforeReconcile(GetReceivable(rawCbs)) -
                                             Convert.ToDecimal(dashboardresult.RecievableSummary.CbsInvalid_Amount);

                var ReceivableDateDiff = GetReceivable(reconResult.Where(x => x.Status.Equals(ReconStatus.DateDiff.ToString())).ToList());
                var ReceivablePrevDate = ReceivableDateDiff.Where(x => x.NpnTransactionDate > x.CbsTransactionDate).Count();
                var ReceivableNextDate = ReceivableDateDiff.Where(x => x.NpnTransactionDate < x.CbsTransactionDate).Count();

                var ReceivablePrevDateAmount = GetTotalAmountAfterReconcile(ReceivableDateDiff.Where(x => x.NpnTransactionDate > x.CbsTransactionDate).ToList());
                var ReceivableNextDateAmount = GetTotalAmountAfterReconcile(ReceivableDateDiff.Where(x => x.NpnTransactionDate < x.CbsTransactionDate).ToList());


                var ReceivableInvalidAtm = GetReceivable(reconResult).Where(x => x.Status == ReconStatus.Invalid.ToString() && x.TerminalType == (int)TerminalType.ATM).ToList();
                var ReceivableInvalidAtmAmount = GetTotalAmountAfterReconcile(ReceivableInvalidAtm);
                var ReceivableInvalidPos = GetReceivable(reconResult).Where(x => x.Status == ReconStatus.Invalid.ToString() && x.TerminalType == (int)TerminalType.POS).ToList();
                var ReceivableInvalidPosAmount = GetTotalAmountAfterReconcile(ReceivableInvalidPos);

                var OnUsSummaryCountTotal = SummaryCount(RawCbsOnUs, dashboardresult.OnUsSummary.UnMatched, dashboardresult.OnUsSummary.Invalid, OnUsPrevDate, OnUsNextDate, 0);

                var PayableSummaryCountTotal = SummaryCount(RawCbsPayable, dashboardresult.PayableSummary.UnMatched, dashboardresult.PayableSummary.Invalid, PayablePrevDate, PayableNextDate, 0);

                var ReceivableSummaryCountTotal = SummaryCount(RawCbsReceivable, dashboardresult.RecievableSummary.UnMatched, dashboardresult.RecievableSummary.Invalid, ReceivablePrevDate, ReceivableNextDate, 0);



                var OnUsSummaryAmountTotal = SummaryAmount(RawCbsOnUsAmount, dashboardresult.OnUsSummary.UnMatched_Amount, dashboardresult.OnUsSummary.Invalid_Amount,
                    OnUsPrevDateAmount, OnUsNextDateAmount, 0);

                var PayableSummaryAmountTotal = SummaryAmount(RawCbsPayableAmount, dashboardresult.PayableSummary.UnMatched_Amount, dashboardresult.PayableSummary.Invalid_Amount,
                    PayablePrevDateAmount, PayableNextDateAmount, 0);

                var ReceivableSummaryAmountTotal = SummaryAmount(RawCbsReceivableAmount, dashboardresult.RecievableSummary.UnMatched_Amount, dashboardresult.RecievableSummary.Invalid_Amount,
                    ReceivablePrevDateAmount, ReceivableNextDateAmount, 0);


                var heading = new[] { "Particular", "Txn", "Amount" };

                var ws = wb.Worksheets.Add("Summary Report");
                ws.Cell("A1").Value = excelName;
                ws.Range("A1:E1").Row(1).Merge();
                ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range("A1:E1").Style.Font.Bold = true;




                //ON US
                ws.Cell("A2").Value = "ON Us";
                ws.Cell("A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("B2").Value = "Switch(Npn)";
                ws.Range("B2:C2").Row(1).Merge();
                ws.Cell("B2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("D2").Value = "CBS";
                ws.Range("D2:E2").Row(1).Merge();
                ws.Cell("D2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range("A2:E2").Style.Font.Bold = true;
                ws.Cell("A3").Value = heading[0];
                ws.Cell("B3").Value = heading[1];
                ws.Cell("C3").Value = heading[2];
                ws.Cell("D3").Value = heading[1];
                ws.Cell("E3").Value = heading[2];
                ws.Range("A3:E3").Style.Font.Bold = true;
                ws.Cell("A4").Value = "ON US SUCCESS";
                ws.Cell("A5").Value = "Failed OnUs";
                ws.Cell("A6").Value = "Missing at CBS";
                ws.Cell("A7").Value = "Total";
                ws.Range("A7:E7").Style.Font.Bold = true;
                //SWITCH(Npn)
                ws.Cell("B4").Value = RawNpnOnUs;
                ws.Cell("C4").Value = RawNpnOnUsAmount;
                ws.Cell("B5").Value = string.Empty;
                ws.Cell("B5").Value = 0;
                ws.Cell("C5").Value = 0;
                ws.Cell("B6").Value = 0;
                ws.Cell("C6").Value = 0;
                ws.Cell("B7").Value = RawNpnOnUs;
                ws.Cell("C7").Value = RawNpnOnUsAmount;
                //CBS
                ws.Cell("D4").Value = RawCbsOnUs;
                ws.Cell("E4").Value = RawCbsOnUsAmount;
                ws.Cell("D5").Value = "-" + dashboardresult.OnUsSummary.UnMatched;
                ws.Cell("E5").Value = "-" + dashboardresult.OnUsSummary.UnMatched_Amount;
                ws.Cell("D6").Value = dashboardresult.OnUsSummary.Invalid;
                ws.Cell("E6").Value = dashboardresult.OnUsSummary.Invalid_Amount;
                ws.Cell("D7").Value = OnUsSummaryCountTotal;
                ws.Cell("E7").Value = OnUsSummaryAmountTotal;

                //Receivable
                ws.Cell("A10").Value = "Receivable";
                ws.Cell("A10").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("B10").Value = "Switch(Npn)";
                ws.Range("B10:C10").Row(1).Merge();
                ws.Cell("B10").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("D10").Value = "CBS";
                ws.Range("D10:E10").Row(1).Merge();
                ws.Cell("D10").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range("A10:E10").Style.Font.Bold = true;
                ws.Cell("A11").Value = heading[0];
                ws.Cell("B11").Value = heading[1];
                ws.Cell("C11").Value = heading[2];
                ws.Cell("D11").Value = heading[1];
                ws.Cell("E11").Value = heading[2];
                ws.Range("A11:E11").Style.Font.Bold = true;
                ws.Cell("A12").Value = "ACQUIRER SUCCESS";
                ws.Cell("A13").Value = "Transaction of Previous Date in CBS";
                ws.Cell("A14").Value = "ATM Loro txns not auto posted";
                ws.Cell("A15").Value = "Pos txns to be settled";
                ws.Cell("A16").Value = "Loro failed (entry to be reversed)";
                ws.Cell("A17").Value = "Transaction of Next Date in CBS";
                ws.Cell("A18").Value = "Total";
                ws.Range("A18:E18").Style.Font.Bold = true;
                //SWITCH(Npn)
                ws.Cell("B12").Value = RawNpnReceivable;
                ws.Cell("C12").Value = RawNpnReceivableAmount;
                ws.Cell("B13").Value = 0;
                ws.Cell("C13").Value = 0;
                ws.Cell("B14").Value = 0;
                ws.Cell("C14").Value = 0;
                ws.Cell("B15").Value = 0;
                ws.Cell("C15").Value = 0;
                ws.Cell("B16").Value = 0;
                ws.Cell("C16").Value = 0;
                ws.Cell("B17").Value = 0;
                ws.Cell("C17").Value = 0;
                ws.Cell("B18").Value = RawNpnReceivable;
                ws.Cell("C18").Value = RawNpnReceivableAmount;
                //CBS
                ws.Cell("D12").Value = RawCbsReceivable;
                ws.Cell("E12").Value = RawCbsReceivableAmount;
                ws.Cell("D13").Value = ReceivablePrevDate;
                ws.Cell("E13").Value = ReceivablePrevDateAmount;
                ws.Cell("D14").Value = ReceivableInvalidAtm.Count;
                ws.Cell("E14").Value = ReceivableInvalidAtmAmount;
                ws.Cell("D15").Value = ReceivableInvalidPos.Count;
                ws.Cell("E15").Value = ReceivableInvalidPosAmount;
                ws.Cell("D16").Value = "-" + dashboardresult.RecievableSummary.UnMatched;
                ws.Cell("E16").Value = "-" + dashboardresult.RecievableSummary.UnMatched_Amount;
                ws.Cell("D17").Value = ReceivableNextDate;
                ws.Cell("E17").Value = ReceivableNextDateAmount;
                ws.Cell("D18").Value = ReceivableSummaryCountTotal;
                ws.Cell("E18").Value = ReceivableSummaryAmountTotal;

                //Remarks CODE NOT IN USE
                //if (ReceivableInvalidAtm.Count > 0)
                //    ws.Cell("F14").Value = " Count " + ReceivableInvalidAtm.Count + " Amount " + ReceivableInvalidAtmAmount + " LORO TXNS NOT AUTO POSTED.";
                //if (ReceivableInvalidPos.Count > 0)
                //    ws.Cell("F15").Value = " Count " + ReceivableInvalidPos.Count + " Amount " + ReceivableInvalidPosAmount + " POS TXNS NOT AUTO POSTED.";
                ws.Cell("F25").Value = "Mbnl Credit Card txn & Dollar Prepaid Card txn";


                //Payable
                ws.Cell("A20").Value = "Payable";
                ws.Cell("A20").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("B20").Value = "Switch(Npn)";
                ws.Range("B20:C20").Row(1).Merge();
                ws.Cell("B20").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("D20").Value = "CBS";
                ws.Range("D20:E20").Row(1).Merge();
                ws.Cell("D20").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range("A20:E20").Style.Font.Bold = true;
                ws.Cell("A21").Value = heading[0];
                ws.Cell("B21").Value = heading[1];
                ws.Cell("C21").Value = heading[2];
                ws.Cell("D21").Value = heading[1];
                ws.Cell("E21").Value = heading[2];
                ws.Range("A21:E21").Style.Font.Bold = true;
                ws.Cell("A22").Value = "ISSUER SUCCESS";
                ws.Cell("A23").Value = "Transaction of Previous Date in CBS";
                ws.Cell("A24").Value = "FAILED -TXN";
                ws.Cell("A25").Value = "Missing at CBS";
                ws.Cell("A26").Value = "Transaction of Next Date in CBS";
                ws.Cell("A27").Value = "Total";
                ws.Range("A27:E27").Style.Font.Bold = true;
                //SWITCH(Npn)
                ws.Cell("B22").Value = RawNpnPayable;
                ws.Cell("C22").Value = RawNpnPayableAmount;
                //                ws.Cell("C22").Value = RawCbsPayableAmount;
                ws.Cell("B23").Value = 0;
                ws.Cell("C23").Value = 0;
                ws.Cell("B24").Value = 0;
                ws.Cell("C24").Value = 0;
                ws.Cell("B25").Value = 0;
                ws.Cell("C25").Value = 0;
                ws.Cell("B26").Value = 0;
                ws.Cell("C26").Value = 0;
                ws.Cell("B27").Value = RawNpnPayable;
                ws.Cell("C27").Value = RawNpnPayableAmount;
                //                ws.Cell("C27").Value = RawCbsPayableAmount;
                //CBS
                ws.Cell("D22").Value = RawCbsPayable;
                ws.Cell("E22").Value = RawCbsPayableAmount;
                //                ws.Cell("E22").Value = RawNpnPayableAmount;
                ws.Cell("D23").Value = PayablePrevDate;
                ws.Cell("E23").Value = PayablePrevDateAmount;
                ws.Cell("D24").Value = "-" + dashboardresult.PayableSummary.UnMatched;
                ws.Cell("E24").Value = "-" + dashboardresult.PayableSummary.UnMatched_Amount;
                ws.Cell("D25").Value = dashboardresult.PayableSummary.Invalid;
                ws.Cell("E25").Value = dashboardresult.PayableSummary.Invalid_Amount;
                ws.Cell("D26").Value = PayableNextDate;
                ws.Cell("E26").Value = PayableNextDateAmount;
                ws.Cell("D27").Value = PayableSummaryCountTotal;
                ws.Cell("E27").Value = PayableSummaryAmountTotal;

                //                MemoryStream stream = GetStream(wb);// The method is defined below
                //                Response.Clear();
                //                Response.Buffer = true;
                //                Response.AddHeader("Content-Disposition", "attachment; filename=" + "TESTname" + ".xlsx");
                //                Response.ContentType = "application/vnd.ms-excel";
                //                Response.BinaryWrite(stream.ToArray());
                //                Response.End();

                ws.Range("B1", "E27").Style.NumberFormat.Format = "_ * #,##0.00_ ;_ * -#,##0.00_ ;_ * \"-\"??_ ;_ @_ ";//positive numbers;negative numbers;zeros;text
                ws.Columns().AdjustToContents();  // Adjust column width
                ws.Rows().AdjustToContents();     // Adjust row heights
                var rngTable = ws.Range("A1:E27");
                rngTable.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                rngTable.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                rngTable = ws.Range("A1:F27");
                rngTable.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;


                wb.Author = "Elite";
                wb.ShowRowColHeaders = true;
                //foreach (IXLWorksheet workSheet in wb.Worksheets)
                //{
                //    foreach (IXLTable table in workSheet.Tables)
                //    {
                //        workSheet.Table(table.Name).ShowAutoFilter = true;
                //        workSheet.ConditionalFormats.RemoveAll();
                //        workSheet.Table(table.Name).ShowColumnStripes = true;
                //        workSheet.Table(table.Name).ShowRowStripes = true;
                //    }
                //}

                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition",
                    "attachment;filename=" + excelName + ".xlsx");
                using (var MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }                //                return View("Index", InitialVal(new Db.Filter()));
                return View("Index", filterModel);
            }
        }
        public ActionResult ExportSummeryReportThreeWay(Db.Filter filterModel)
        {
            using (XLWorkbook wb = new XLWorkbook())
            {

                var dashboardresult = Dashboard.DashboardFromFilter(filterModel);
                var DateFrom = GlobalHelper.ChangeDateFormat(filterModel.FromDate);
                var DateTo = GlobalHelper.ChangeDateFormat(filterModel.ToDate);

                string excelName = filterModel.ReconTypeName + "_Summary Report_" +
                                   (filterModel.FromDate.Equals(filterModel.ToDate) ? filterModel.FromDate : filterModel.FromDate + " To " + filterModel.ToDate);
                var heading1 = new[] { "Switch(Npn)", "CBS", "VISA" };
                var heading2 = new[] { "Particular", "Txn", "Amount", "Date" };
                var ws = wb.Worksheets.Add("Summary Report");

                List<Transaction> rawNpn = new List<Transaction>();
                List<Transaction> rawCbs = new List<Transaction>();
                List<VwTransactionDetailsModel> RawVisaWithRespectToNpnFromViewTransactons = new List<VwTransactionDetailsModel>();
                using (var context = new ReconContext())
                {
                    rawNpn = context.Transactions.Where(x =>
                        x.Source_SourceId == 4
                        && (x.TransactionStatus == TransactionStatus.Success || x.TransactionStatus == TransactionStatus.Success_With_Suspected)
                        && x.TransactionDate >= DateFrom
                        && x.TransactionDate <= DateTo
                        && x.TransactionType == TransactionType.Financial).ToList();


                    rawCbs = context.Transactions.Where(x =>
                        x.Source_SourceId == 3 && x.TransactionStatus == TransactionStatus.Success &&
                        x.TransactionDate >= DateFrom
                        && x.TransactionDate <= DateTo
                        && x.TransactionType == TransactionType.Financial).ToList();

                    string RawVisaWithRespectToNpnSql = string.Format(
                        @"select * from vw_transaction_details where ReconSource = 4  
                                                                 and TransactionDate = '{0}'
                                                                 and NpnTransactionDate = '{0}'
                                                                 and visaTransactionDate = '{0}'
                                                                and visaTransactionStatus = 1 ",
                        GlobalHelper.ChangeDateFormatWithDash(filterModel.FromDate));

                    RawVisaWithRespectToNpnFromViewTransactons = GlobalHelper.DatatableToClass<VwTransactionDetailsModel>(_DbOperation.ExecuteDataTable(RawVisaWithRespectToNpnSql)).ToList();

                }

                var reconResult = new Recon().GetVwTransactionDetailsModels(filterModel);

                var RawNpnOnUs = GetOnUs(rawNpn).Count;
                var RawCbsOnUs = GetOnUs(rawCbs).Count;
                var RawNpnOnUsAmount = GetTotalAmountBeforeReconcile(GetOnUs(rawNpn));
                var RawCbsOnUsAmount = GetTotalAmountBeforeReconcile(GetOnUs(rawCbs));

                var OnUsDateDiff = GetOnUs(reconResult.Where(x => x.Status.Equals(ReconStatus.DateDiff.ToString())).ToList());
                var OnUsPrevDate = OnUsDateDiff.Where(x => x.NpnTransactionDate > x.CbsTransactionDate).Count();
                var OnUsNextDate = OnUsDateDiff.Where(x => x.NpnTransactionDate < x.CbsTransactionDate).Count();

                var OnUsPrevDateAmount = GetTotalAmountAfterReconcile(OnUsDateDiff.Where(x => x.NpnTransactionDate > x.CbsTransactionDate).ToList());
                var OnUsNextDateAmount = GetTotalAmountAfterReconcile(OnUsDateDiff.Where(x => x.NpnTransactionDate < x.CbsTransactionDate).ToList());


                /** Cbs calculation started **/

                var PayableDateDiff = GetPayable(reconResult
                    .Where(x => x.Status.Equals(ReconStatus.DateDiff.ToString()) && x.NpnTransactionDate != x.CbsTransactionDate).ToList());
                //                var PayableDateDiff = GetPayable(reconResult
                //                    .Where(x => x.Status.Equals(ReconStatus.DateDiffNpnCbs.ToString()) && x.NpnTransactionDate != x.CbsTransactionDate).ToList());


                var PayablePrevDate = PayableDateDiff.Where(x => x.NpnTransactionDate > x.CbsTransactionDate && x.NpnTransactionDate == DateFrom).Count();
                var PayableNextDate = PayableDateDiff.Where(x => x.NpnTransactionDate < x.CbsTransactionDate && x.NpnTransactionDate == DateFrom).Count();

                var RemainingPayableDateDiff = PayableDateDiff.Count - PayablePrevDate - PayableNextDate;

                var PayablePrevDateAmount = GetTotalAmountAfterReconcile(PayableDateDiff.Where(x => x.NpnTransactionDate > x.CbsTransactionDate && x.NpnTransactionDate == DateFrom).ToList());
                var PayableNextDateAmount = GetTotalAmountAfterReconcile(PayableDateDiff.Where(x => x.NpnTransactionDate < x.CbsTransactionDate && x.NpnTransactionDate == DateFrom).ToList());
                //                var RemainingPayableDateDiffAmount = GetTotalAmountAfterReconcile(PayableDateDiff);

                var RemainingPayableDateDiffAmount = GetTotalAmountAfterReconcile(PayableDateDiff
                    .Where(x => x.TransactionDate != DateFrom).ToList());



                //date diff calculated for cbs..just deduct total received cbs of remainaing date diff ie either prev nor nex date..

                //payable..
                var PayableDateDiffForNpnCbs = GetPayable(reconResult
                    .Where(x => x.Status.Equals(ReconStatus.DateDiffNepsCbs.ToString()) && x.NpnTransactionDate != x.CbsTransactionDate).ToList());
                var PayablePrevDateForNpnCbs = PayableDateDiffForNpnCbs.Where(x => x.NpnTransactionDate > x.CbsTransactionDate && x.NpnTransactionDate == DateFrom).Count();
                var PayableNextDateForNpnCbs = PayableDateDiffForNpnCbs.Where(x => x.NpnTransactionDate < x.CbsTransactionDate && x.NpnTransactionDate == DateFrom).Count();

                var RemainingPayableDateDiffForNpnCbs = PayableDateDiffForNpnCbs.Count - PayablePrevDateForNpnCbs - PayableNextDateForNpnCbs;
                var RemainingPayableDateDiffForNpnCbsAmount = GetTotalAmountAfterReconcile(PayableDateDiffForNpnCbs
                    .Where(x => x.TransactionDate != DateFrom).ToList());

                //receivable
                var ReceivableDateDiffForNpnCbs = GetReceivable(reconResult
                    .Where(x => x.Status.Equals(ReconStatus.DateDiffNepsCbs.ToString()) && x.NpnTransactionDate != x.CbsTransactionDate).ToList());
                var ReceivablePrevDateForNpnCbs = ReceivableDateDiffForNpnCbs.Where(x => x.NpnTransactionDate > x.CbsTransactionDate && x.NpnTransactionDate == DateFrom).Count();
                var ReceivableNextDateForNpnCbs = ReceivableDateDiffForNpnCbs.Where(x => x.NpnTransactionDate < x.CbsTransactionDate && x.NpnTransactionDate == DateFrom).Count();

                var RemainingReceivableDateDiffForNpnCbs = ReceivableDateDiffForNpnCbs.Count - ReceivablePrevDateForNpnCbs - ReceivableNextDateForNpnCbs;
                var RemainingReceivableDateDiffForNpnCbsAmount = GetTotalAmountAfterReconcile(ReceivableDateDiffForNpnCbs
                    .Where(x => x.TransactionDate != DateFrom).ToList());

                var RawNpnPayable = GetPayable(rawNpn).Count;
                //                var RawCbsPayable = GetPayable(rawCbs).Count - RemainingPayableDateDiff;
                var RawCbsPayable = GetPayable(rawCbs).Count - RemainingPayableDateDiffForNpnCbs - dashboardresult.PayableSummary.CbsInvalid;
                //                var RawCbsPayable = GetPayable(rawCbs).Count;
                var RawNpnPayableAmount = GetTotalAmountBeforeReconcile(GetPayable(rawNpn));
                var RawCbsPayableAmount = GetTotalAmountBeforeReconcile(GetPayable(rawCbs)) - Convert.ToDecimal(RemainingPayableDateDiffForNpnCbsAmount) - Convert.ToDecimal(dashboardresult.PayableSummary.CbsInvalid_Amount);
                //                var RawCbsPayableAmount = GetTotalAmountBeforeReconcile(GetPayable(rawCbs));

                var RawNpnReceivable = GetReceivable(rawNpn).Count;
                //                var RawCbsReceivable = GetReceivable(rawCbs).Count;
                var RawCbsReceivable = GetReceivable(rawCbs).Count - RemainingReceivableDateDiffForNpnCbs - dashboardresult.RecievableSummary.CbsInvalid;

                var RawNpnReceivableAmount = GetTotalAmountBeforeReconcile(GetReceivable(rawNpn));
                //                var RawCbsReceivableAmount = GetTotalAmountBeforeReconcile(GetReceivable(rawCbs));
                var RawCbsReceivableAmount = GetTotalAmountBeforeReconcile(GetReceivable(rawCbs)) - Convert.ToDecimal(RemainingReceivableDateDiffForNpnCbsAmount) - Convert.ToDecimal(dashboardresult.RecievableSummary.CbsInvalid_Amount);


                var ReceivableDateDiff = GetReceivable(reconResult.Where(x => x.Status.Equals(ReconStatus.DateDiff.ToString())).ToList());
                var ReceivablePrevDate = ReceivableDateDiff.Where(x => x.NpnTransactionDate > x.CbsTransactionDate).ToList();
                var ReceivableNextDate = ReceivableDateDiff.Where(x => x.NpnTransactionDate < x.CbsTransactionDate).ToList();

                var ReceivablePrevDateAmount = GetTotalAmountAfterReconcile(ReceivablePrevDate);
                var ReceivableNextDateAmount = GetTotalAmountAfterReconcile(ReceivableNextDate);


                var ReceivableInvalidAtm = GetReceivable(reconResult)
                    .Where(x => x.Status == ReconStatus.Invalid.ToString() &&
                                x.TerminalType == (int)TerminalType.ATM &&
                                x.CbsResponseCode == null).ToList();

                var ReceivableInvalidAtmAmount = GetTotalAmountAfterReconcile(ReceivableInvalidAtm);

                var ReceivableInvalidPos = GetReceivable(reconResult).Where(
                    x => x.Status == ReconStatus.Invalid.ToString() && x.TerminalType == (int)TerminalType.POS &&
                         x.CbsResponseCode == null).ToList();

                var ReceivableInvalidPosAmount = GetTotalAmountAfterReconcile(ReceivableInvalidPos);


                var PayableInvalidCbs = GetPayable(reconResult).Where(x => x.Status == ReconStatus.Invalid.ToString() && string.IsNullOrEmpty(x.CbsResponseCode)).ToList();
                var PayableInvalidCbsAmount = GetTotalAmountAfterReconcile(PayableInvalidCbs);

                var ReceivableFailedCbs = GetReceivable(reconResult.Where(x => x.Status.Equals(ReconStatus.Failed.ToString()) && x.CbsResponseCode != null && x.CbsTransactionStatus == (int)TransactionStatus.Success).ToList());
                var ReceivableFailedCbsAmount = GetTotalAmountAfterReconcile(ReceivableFailedCbs);

                var PayableFailedCbs = GetPayable(reconResult.Where(x => x.Status.Equals(ReconStatus.Failed.ToString()) && x.CbsResponseCode != null).ToList());
                var PayableFailedCbsAmount = GetTotalAmountAfterReconcile(PayableFailedCbs);

                var PayableSummaryCbsAmountTotal = SummaryAmount(RawCbsPayableAmount, PayableFailedCbsAmount, PayableInvalidCbsAmount,
                    PayablePrevDateAmount, PayableNextDateAmount, 0);

                var ReceivableSummaryCbsAmountTotal = SummaryAmount(RawCbsReceivableAmount, ReceivableFailedCbsAmount, ReceivableInvalidAtmAmount + ReceivableInvalidPosAmount,
                    ReceivablePrevDateAmount, ReceivableNextDateAmount, dashboardresult.RecievableSummary.CbsInvalid_Amount);

                var PayableSummaryCbsCountTotal = SummaryCount(RawCbsPayable, PayableFailedCbs.Count, PayableInvalidCbs.Count, PayablePrevDate, PayableNextDate, 0);

                var ReceivableSummaryCbsCountTotal = SummaryCount(RawCbsReceivable, ReceivableFailedCbs.Count, ReceivableInvalidAtm.Count + ReceivableInvalidPos.Count, ReceivablePrevDate.Count, ReceivableNextDate.Count, dashboardresult.RecievableSummary.CbsInvalid);
                /** Cbs calculation ended  **/

                /** Visa calculation started  **/
                var ReceivableGroupAmoutByAdviseDate =
                    GetReceivable(RawVisaWithRespectToNpnFromViewTransactons).GroupBy(x => x.VisaAdviseDate)
                                                                        .Select(y => new
                                                                        {
                                                                            Count = y.Count(),
                                                                            GroupedAmount = y.Sum(x => x.NpnAmount),
                                                                            VisaAdviseDate = y.Key
                                                                        }).OrderBy(x => x.VisaAdviseDate);

                var PayableGroupAmoutByAdviseDate =
                    GetPayable(RawVisaWithRespectToNpnFromViewTransactons).GroupBy(x => x.VisaAdviseDate)
                        .Select(y => new
                        {
                            Count = y.Count(),
                            GroupedAmount = y.Sum(x => x.NpnAmount),
                            VisaAdviseDate = y.Key
                        }).OrderBy(x => x.VisaAdviseDate).ToList();


                /** visa date diff  **/
                var VisaDateDiff = reconResult.Where(x => x.Status.Equals(ReconStatus.DateDiff.ToString())).ToList();

                var ReceivablePrevDateVisa = GetReceivable(VisaDateDiff).Where(x => x.NpnTransactionDate > x.VisaTransactionDate && x.NpnTransactionDate == DateFrom).ToList();
                var ReceivableNextDateVisa = GetReceivable(VisaDateDiff).Where(x => x.NpnTransactionDate < x.VisaTransactionDate && x.NpnTransactionDate == DateFrom).ToList();

                var ReceivablePrevDateAmountVisa = GetTotalAmountAfterReconcile(ReceivablePrevDateVisa);
                var ReceivableNextDateAmountVisa = GetTotalAmountAfterReconcile(ReceivableNextDateVisa);

                var PayablePrevDateVisa = GetPayable(VisaDateDiff).Where(x => x.NpnTransactionDate > x.VisaTransactionDate && x.NpnTransactionDate == DateFrom).ToList();
                var PayableNextDateVisa = GetPayable(VisaDateDiff).Where(x => x.NpnTransactionDate < x.VisaTransactionDate && x.NpnTransactionDate == DateFrom).ToList();

                var PayablePrevDateAmountVisa = GetTotalAmountAfterReconcile(PayablePrevDateVisa);
                var PayableNextDateAmountVisa = GetTotalAmountAfterReconcile(PayableNextDateVisa);

                /** missing visa **/

                var ReceivableMissingVisa = GetReceivable(reconResult).Where(
                    x => x.Status == ReconStatus.Invalid.ToString() && x.VisaResponseCode == null).ToList();

                var PayableMissingVisa = GetPayable(reconResult).Where(
                    x => x.Status == ReconStatus.Invalid.ToString() && x.VisaResponseCode == null).ToList();

                var ReceivableMissingVisaAmount = GetTotalAmountAfterReconcile(ReceivableMissingVisa);

                var PayableMissingVisaAmount = GetTotalAmountAfterReconcile(PayableMissingVisa);

                var TotalReceivableVisaMissing = ReceivableMissingVisa.Count;
                var TotalPayableVisaMissing = PayableMissingVisa.Count;


                var TotalReceivableMissingVisaAmount = ReceivableMissingVisaAmount;
                var TotalPayableVisaMissingAmount = PayableMissingVisaAmount;

                /** Atm loro failed  -- visa **/
                var ReceivableFailPostedInVisaOnly = GetReceivable(reconResult).Where(    // in receivable case take all failed txn...
                    x => x.Status == ReconStatus.Failed.ToString() && x.VisaTransactionDate != null).ToList();

                var PayableFailNotPostedInCbs = GetPayable(reconResult).Where(  // in payable case take only fail txn that are not posted in cbs..
                    x => x.Status == ReconStatus.Failed.ToString() && x.CbsResponseCode == null).ToList();

                var ReceivableFailAmount = GetTotalAmountAfterReconcile(ReceivableFailPostedInVisaOnly);

                var PayableFailNotPostedInCbsAmount = GetTotalAmountAfterReconcile(PayableFailNotPostedInCbs);


                var totalReceivableFailedTxn = ReceivableFailPostedInVisaOnly.Count;

                var totalPayableFailedTxn = PayableFailNotPostedInCbs.Count;

                var totalReceivableFailedTxnAmount = ReceivableFailAmount;

                var totalPayableFailedTxnAmount = PayableFailNotPostedInCbsAmount;

                /** Get visa total **/

                var ReceivableSummaryVisaCountTotal = ReceivableGroupAmoutByAdviseDate.Sum(x => x.Count) +
                                                      ReceivablePrevDateVisa.Count + ReceivableNextDateVisa.Count -
                                                      totalReceivableFailedTxn + TotalReceivableVisaMissing;

                var ReceivableSummaryVisaAmountTotal = ReceivableGroupAmoutByAdviseDate.Sum(x => x.GroupedAmount) +
                                                      ReceivablePrevDateAmountVisa + ReceivableNextDateAmountVisa -
                                                       totalReceivableFailedTxnAmount + TotalReceivableMissingVisaAmount;

                var PayableSummaryVisaCountTotal = PayableGroupAmoutByAdviseDate.Sum(x => x.Count) +
                                                   PayablePrevDateVisa.Count + PayableNextDateVisa.Count -
                                                   totalPayableFailedTxn + TotalPayableVisaMissing;

                var PayableSummaryVisaAmountTotal = PayableGroupAmoutByAdviseDate.Sum(x => x.GroupedAmount) +
                                                      PayablePrevDateAmountVisa + PayableNextDateAmountVisa -
                                                    totalPayableFailedTxnAmount + TotalPayableVisaMissingAmount;

                /** end of calculation  for visa..**/

                /** Visa info mapping **/

                var ReceivableTxnPrevDateVisaCount = ReceivablePrevDateVisa.Count;
                var ReceivableTxnPrevDateVisaAmount = ReceivablePrevDateAmountVisa;

                var ReceivableTxnNextDateVisaCount = ReceivableNextDateVisa;
                var ReceivableTxnNextDateVisaAmount = ReceivableNextDateAmountVisa;

                var ReceivableMissingAtVisaCount = TotalReceivableVisaMissing;
                var ReceivableMissingAtVisaAmount = TotalReceivableMissingVisaAmount;

                var ReceivableLoroFailedPostedOnlyInVisaCount = totalReceivableFailedTxn;
                var ReceivableLoroFailedPostedOnlyInVisaAmount = totalReceivableFailedTxnAmount;

                var PayableTxnPrevDateVisaCount = PayablePrevDateVisa.Count;
                var PayableTxnPrevDateVisaAmount = PayablePrevDateAmountVisa;

                var PayableTxnNextDateVisaCount = PayableNextDateVisa.Count;
                var PayableTxnNextDateVisaAmount = PayableNextDateAmountVisa;

                var PayableFailedTxnNotHitInCbsCount = totalPayableFailedTxn;
                var PayableFailedTxnNotHitInCbsAmount = totalPayableFailedTxnAmount;

                var PayableMissingAtVisaCount = TotalPayableVisaMissing;
                var PayableMissingAtVisaAmount = TotalPayableVisaMissingAmount;

                var VisaTotalReceivableCount = ReceivableSummaryVisaCountTotal;
                var VisaTotalReceivableAmount = ReceivableSummaryVisaAmountTotal;

                var VisaTotalPayableCount = PayableSummaryVisaCountTotal;
                var VisaTotalPayableAmount = PayableSummaryVisaAmountTotal;

                ws.Cell("A1").Value = excelName;
                ws.Range("A1:H1").Row(1).Merge();
                ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range("A1:H1").Style.Font.Bold = true;



                //Receivable
                ws.Cell("A2").Value = "Receivable";
                ws.Cell("A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("B2").Value = heading1[0];
                ws.Range("B2:C2").Row(1).Merge();
                ws.Cell("B2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("D2").Value = heading1[1];
                ws.Range("D2:E2").Row(1).Merge();
                ws.Cell("D2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("F2").Value = heading1[2];
                ws.Range("F2:G2").Row(1).Merge();
                ws.Cell("F2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range("A2:H2").Style.Font.Bold = true;
                ws.Cell("A3").Value = heading2[0];
                ws.Cell("B3").Value = heading2[1];
                ws.Cell("C3").Value = heading2[2];
                ws.Cell("D3").Value = heading2[1];
                ws.Cell("E3").Value = heading2[2];
                ws.Cell("F3").Value = heading2[1];
                ws.Cell("G3").Value = heading2[2];
                ws.Cell("H3").Value = heading2[3];
                ws.Range("A3:H3").Style.Font.Bold = true;

                ws.Cell("A4").Value = "ACQUIRER SUCCESS";
                ws.Cell("B4").Value = RawNpnReceivable;
                ws.Cell("C4").Value = RawNpnReceivableAmount;
                ws.Cell("D4").Value = RawCbsReceivable;
                ws.Cell("E4").Value = RawCbsReceivableAmount;

                int countRow = 4;
                int gapValue = ReceivableGroupAmoutByAdviseDate.Count();
                for (int i = 0; i < gapValue; i++)
                {
                    ws.Cell("C" + countRow + 1).Value = 0;
                    ws.Cell("E" + countRow + 1).Value = 0;
                    ws.Cell("F" + countRow).Value = ReceivableGroupAmoutByAdviseDate.ToArray()[i].Count;
                    ws.Cell("G" + countRow).Value = ReceivableGroupAmoutByAdviseDate.ToArray()[i].GroupedAmount;
                    ws.Cell("H" + countRow).Value =
                    "'" + Convert.ToDateTime(ReceivableGroupAmoutByAdviseDate.ToArray()[i].VisaAdviseDate).ToString("yyyyMMdd");
                    countRow++;
                }

                int countNpn = countRow;
                int countCBS = countRow;
                int countVISA = countRow;

                ws.Cell("A" + countRow++).Value = "Transaction of Previous Date in Switch";
                ws.Cell("A" + countRow++).Value = "Transaction of Previous Date in CBS";
                //ws.Cell("H" + countRow).Value = "Date" + countRow; //Dummy Date
                ws.Cell("A" + countRow++).Value = "Transaction of Previous Date in VISA";
                ws.Cell("A" + countRow++).Value = "ATM Loro txns not auto posted";
                ws.Cell("A" + countRow++).Value = "Pos txns to be settled";
                ws.Cell("A" + countRow++).Value = "Loro failed (entry to be reversed)";
                //ws.Cell("H" + countRow).Value = "Date" + countRow;//Dummy Date
                ws.Cell("A" + countRow++).Value = "Loro failed";// but not posted in CBS";
                ws.Cell("A" + countRow++).Value = "Missing at Npn";
                ws.Cell("A" + countRow++).Value = "Missing at VISA";
                ws.Cell("A" + countRow++).Value = "Transaction of Next Date in Switch";
                ws.Cell("A" + countRow++).Value = "Transaction of Next Date in CBS";
                ws.Cell("A" + countRow++).Value = "Transaction of Next Date in VISA";
                ws.Cell("A" + countRow).Value = "Total";
                ws.Range("A" + countRow + ":I" + countRow++).Style.Font.Bold = true;

                //SWITCH(Npn)
                ws.Cell("C" + countNpn++).Value = 0; //Transaction of Previous Date in Switch
                ws.Cell("C" + countNpn++).Value = 0; //Transaction of Previous Date in CBS
                ws.Cell("C" + countNpn++).Value = 0; //Transaction of Previous Date in VISA
                ws.Cell("C" + countNpn++).Value = 0; //Loro txns not auto posted
                ws.Cell("C" + countNpn++).Value = 0; //Pos txns to be settled
                ws.Cell("C" + countNpn++).Value = 0; //Loro failed (entry to be reversed)
                ws.Cell("C" + countNpn++).Value = 0; //Loro failed but not posted in CBS
                ws.Cell("C" + countNpn++).Value = 0; //Missing at Npn
                ws.Cell("C" + countNpn++).Value = 0; //Missing at VISA
                ws.Cell("C" + countNpn++).Value = 0; //Transaction of Next Date in Switch
                ws.Cell("C" + countNpn++).Value = 0; //Transaction of Next Date in CBS
                ws.Cell("C" + countNpn++).Value = 0; //Transaction of Next Date in VISA
                ws.Cell("B" + countNpn).Value = RawNpnReceivable; //TOTAL
                ws.Cell("C" + countNpn++).Value = RawNpnReceivableAmount; //TOTAL
                //CBS
                ws.Cell("D" + countCBS).Value = 0; //Transaction of Previous Date in Switch
                ws.Cell("E" + countCBS++).Value = 0; //Transaction of Previous Date in Switch
                ws.Cell("D" + countCBS).Value = ReceivablePrevDate; //Transaction of Previous Date in CBS
                ws.Cell("E" + countCBS++).Value = ReceivablePrevDateAmount; //Transaction of Previous Date in CBS
                ws.Cell("E" + countCBS++).Value = 0; //Transaction of Previous Date in VISA
                ws.Cell("D" + countCBS).Value = ReceivableInvalidAtm.Count; //Loro txns not auto posted
                ws.Cell("E" + countCBS++).Value = ReceivableInvalidAtmAmount; //Loro txns not auto posted
                ws.Cell("D" + countCBS).Value = ReceivableInvalidPos.Count; //Pos txns to be settled
                ws.Cell("E" + countCBS++).Value = ReceivableInvalidPosAmount; //Pos txns to be settled
                ws.Cell("D" + countCBS).Value = "-" + ReceivableFailedCbs.Count; //Loro failed (entry to be reversed)
                ws.Cell("E" + countCBS++).Value = "-" + ReceivableFailedCbsAmount; ////Loro failed (entry to be reversed)
                ws.Cell("E" + countCBS++).Value = 0; ////Loro failed but not posted in CBS
                ws.Cell("D" + countCBS).Value = 0; //Missing at Npn
                ws.Cell("E" + countCBS++).Value = 0; //Missing at Npn
                ws.Cell("E" + countCBS++).Value = 0; //Missing at VISA
                ws.Cell("D" + countCBS).Value = 0; //Transaction of Next Date in Switch
                ws.Cell("E" + countCBS++).Value = 0; //Transaction of Next Date in Switch
                ws.Cell("D" + countCBS).Value = ReceivableNextDate; //Transaction of Next Date in CBS
                ws.Cell("E" + countCBS++).Value = ReceivableNextDateAmount; //Transaction of Next Date in CBS
                ws.Cell("E" + countCBS++).Value = 0; //Transaction of Next Date in VISA
                ws.Cell("D" + countCBS).Value = ReceivableSummaryCbsCountTotal; //TOTAL
                ws.Cell("E" + countCBS++).Value = ReceivableSummaryCbsAmountTotal; //TOTAL
                //VISA

                ws.Cell("F" + countVISA).Value = 0; //Transaction of Previous Date in Switch
                ws.Cell("G" + countVISA++).Value = 0; //Transaction of Previous Date in Switch
                ws.Cell("G" + countVISA++).Value = 0; //Transaction of Previous Date in CBS
                ws.Cell("F" + countVISA).Value = ReceivableTxnPrevDateVisaCount; //Transaction of Previous Date in VISA
                ws.Cell("G" + countVISA++).Value = ReceivableTxnPrevDateVisaAmount; //Transaction of Previous Date in VISA
                ws.Cell("F" + countVISA).Value = 0; //Loro txns not auto posted
                ws.Cell("G" + countVISA++).Value = 0; //Loro txns not auto posted
                //ws.Cell("I" + countVISA).Value = "Count " + ReceivableInvalidAtm.Where(x => x.TransactionDate == x.CbsTransactionDate).ToList().Count + " amount " + ReceivableInvalidAtmAmount + " POS TXNS TO BE SETTLED."; //Remarks CODE NOT IN USE
                ws.Cell("F" + countVISA).Value = 0;  //Pos txns to be settled
                ws.Cell("G" + countVISA++).Value = 0; //Pos txns to be settled
                //ws.Cell("I" + countVISA).Value = "Count " + ReceivableInvalidPos.Count + " amount " + ReceivableInvalidPosAmount + " POS TXNS TO BE SETTLED."; //Remarks CODE NOT IN USE
                ws.Cell("G" + countVISA++).Value = 0; //Loro failed (entry to be reversed)
                ws.Cell("F" + countVISA).Value = "-" + ReceivableLoroFailedPostedOnlyInVisaCount; //Loro failed but not posted in CBS
                ws.Cell("G" + countVISA++).Value = "-" + ReceivableLoroFailedPostedOnlyInVisaAmount; //Loro failed but not posted in CBS
                //ws.Cell("I" + countVISA).Value = "Mismatch: Count " + 123 + " amount " + 123 + " in " + excelName;//Remarks CODE NOT IN USE
                ws.Cell("G" + countVISA++).Value = 0;  //Missing at Npn
                ws.Cell("F" + countVISA).Value = ReceivableMissingAtVisaCount;  //Missing at VISA
                ws.Cell("G" + countVISA++).Value = ReceivableMissingAtVisaAmount;  //Missing at VISA
                ws.Cell("F" + countVISA).Value = 0;  //Transaction of Next Date in Switch
                ws.Cell("G" + countVISA++).Value = 0;  //Transaction of Next Date in Switch
                ws.Cell("G" + countVISA++).Value = 0; //Transaction of Next Date in CBS
                ws.Cell("F" + countVISA).Value = ReceivableTxnNextDateVisaCount; //Transaction of Next Date in VISA
                ws.Cell("G" + countVISA++).Value = ReceivableTxnNextDateVisaAmount; //Transaction of Next Date in VISA
                ws.Cell("F" + countVISA).Value = VisaTotalReceivableCount; //TOTAL
                ws.Cell("G" + countVISA++).Value = VisaTotalReceivableAmount; //TOTAL
                ws.Cell("G" + countVISA).Value = 0;
                ws.Cell("H" + countVISA++).Value = 0;


                //PAYABLE
                countRow++;
                countNpn++;
                countCBS++;
                //                countVISA+;

                ws.Range("A" + countRow + ":H" + countRow).Style.Font.Bold = true;
                ws.Cell("A" + countRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("A" + countRow++).Value = "Payable";
                ws.Range("A" + countRow + ":H" + countRow).Style.Font.Bold = true;
                ws.Range("B" + countNpn + ":C" + countNpn).Row(1).Merge();
                ws.Cell("B" + countNpn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("B" + countNpn++).Value = heading1[0];
                ws.Range("D" + countCBS + ":E" + countCBS).Row(1).Merge();
                ws.Cell("D" + countCBS).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("D" + countCBS++).Value = heading1[1];
                ws.Range("F" + countVISA + ":G" + countVISA).Row(1).Merge();
                ws.Cell("F" + countVISA).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("F" + countVISA++).Value = heading1[2];
                ws.Cell("A" + countRow++).Value = heading2[0];
                ws.Cell("B" + countNpn).Value = heading2[1];
                ws.Cell("C" + countNpn++).Value = heading2[2];
                ws.Cell("D" + countCBS).Value = heading2[1];
                ws.Cell("E" + countCBS++).Value = heading2[2];
                ws.Cell("F" + countVISA).Value = heading2[1];
                ws.Cell("G" + countVISA).Value = heading2[2];
                ws.Cell("H" + countVISA++).Value = heading2[3];

                ws.Cell("A" + countRow).Value = "ISSUER SUCCESS";
                ws.Cell("B" + countNpn).Value = RawNpnPayable;
                ws.Cell("C" + countNpn++).Value = RawNpnPayableAmount;
                ws.Cell("D" + countCBS).Value = RawCbsPayable;
                ws.Cell("E" + countCBS++).Value = RawCbsPayableAmount;
                //ws.Cell("H" + countVISA).Value = "Date" + countVISA;//Dummy Date
                ws.Cell("F" + countVISA).Value = 0;
                ws.Cell("G" + countVISA++).Value = 0;

                gapValue = PayableGroupAmoutByAdviseDate.Count();

                for (int i = 0; i < gapValue; i++)
                {
                    ws.Cell("C" + countRow + 1).Value = 0;
                    ws.Cell("E" + countRow + 1).Value = 0;
                    ws.Cell("F" + countRow).Value = PayableGroupAmoutByAdviseDate.ToArray()[i].Count;
                    ws.Cell("G" + countRow).Value = PayableGroupAmoutByAdviseDate.ToArray()[i].GroupedAmount;
                    ws.Cell("H" + countRow).Value =
                    "'" + Convert.ToDateTime(PayableGroupAmoutByAdviseDate.ToArray()[i].VisaAdviseDate).ToString("yyyyMMdd");
                    countRow++;
                }

                countNpn = countRow;
                countCBS = countRow;
                countVISA = countRow;
                ws.Cell("A" + countRow++).Value = "Transaction of Previous Date in Switch";
                ws.Cell("A" + countRow++).Value = "Transaction of Previous Date in CBS";
                //ws.Cell("H" + countRow).Value = "Date" + countRow;//Dummy Date
                ws.Cell("A" + countRow++).Value = "Transaction of Previous Date in VISA";
                ws.Cell("A" + countRow++).Value = "FAILED -TXN";
                ws.Cell("A" + countRow++).Value = "Fail Txn but not hit in CBS";
                ws.Cell("A" + countRow++).Value = "Missing at CBS";
                ws.Cell("A" + countRow++).Value = "Missing at Npn";
                ws.Cell("A" + countRow++).Value = "Missing at VISA";
                ws.Cell("A" + countRow++).Value = "Transaction of Next Date in Switch";
                ws.Cell("A" + countRow++).Value = "Transaction of Next Date in CBS";
                ws.Cell("A" + countRow++).Value = "Transaction of Next Date in VISA";
                ws.Range("A" + countRow + ":I" + countRow).Style.Font.Bold = true;
                ws.Cell("A" + countRow++).Value = "Total";

                //SWITCH(Npn)
                ws.Cell("C" + countNpn++).Value = 0; //Transaction of Previous Date in Switch
                ws.Cell("C" + countNpn++).Value = 0; //Transaction of Previous Date in CBS
                ws.Cell("C" + countNpn++).Value = 0; //Transaction of Previous Date in VISA
                ws.Cell("C" + countNpn++).Value = 0; //FAILED -TXN
                ws.Cell("C" + countNpn++).Value = 0; //Fail Txn but not hit in CBS
                ws.Cell("C" + countNpn++).Value = 0; //Missing at CBS
                ws.Cell("C" + countNpn++).Value = 0; //Missing at Npn
                ws.Cell("C" + countNpn++).Value = 0; //Missing at VISA
                ws.Cell("C" + countNpn++).Value = 0; //Transaction of Next Date in Switch
                ws.Cell("C" + countNpn++).Value = 0; //Transaction of Next Date in CBS
                ws.Cell("C" + countNpn++).Value = 0; //Transaction of Next Date in VISA
                ws.Cell("B" + countNpn).Value = RawNpnPayable; //TOTAL
                ws.Cell("C" + countNpn++).Value = RawNpnPayableAmount; //TOTAL
                //CBS
                ws.Cell("D" + countCBS).Value = 0;  //Transaction of Previous Date in Switch
                ws.Cell("E" + countCBS++).Value = 0;  //Transaction of Previous Date in Switch
                ws.Cell("D" + countCBS).Value = PayablePrevDate; //Transaction of Previous Date in CBS
                ws.Cell("E" + countCBS++).Value = PayablePrevDateAmount; //Transaction of Previous Date in CBS
                ws.Cell("E" + countCBS++).Value = 0; //Transaction of Previous Date in VISA
                ws.Cell("D" + countCBS).Value = "-" + PayableFailedCbs.Count; //FAILED -TXN
                ws.Cell("E" + countCBS++).Value = "-" + PayableFailedCbsAmount; //FAILED -TXN
                ws.Cell("E" + countCBS++).Value = 0; //Fail Txn but not hit in CBS
                ws.Cell("D" + countCBS).Value = PayableInvalidCbs.Count; //Missing at CBS
                ws.Cell("E" + countCBS++).Value = PayableInvalidCbsAmount; //Missing at CBS
                ws.Cell("D" + countCBS).Value = 0; //Missing at Npn
                ws.Cell("E" + countCBS++).Value = 0; //Missing at Npn
                ws.Cell("E" + countCBS++).Value = 0; //Missing at VISA
                ws.Cell("D" + countCBS).Value = 0; //Transaction of Next Date in Switch
                ws.Cell("E" + countCBS++).Value = 0; //Transaction of Next Date in Switch
                ws.Cell("D" + countCBS).Value = PayableNextDate; //Transaction of Next Date in CBS
                ws.Cell("E" + countCBS++).Value = PayableNextDateAmount; //Transaction of Next Date in CBS
                ws.Cell("E" + countCBS++).Value = 0; //Transaction of Next Date in VISA
                ws.Cell("D" + countCBS).Value = PayableSummaryCbsCountTotal; //TOTAL
                ws.Cell("E" + countCBS++).Value = PayableSummaryCbsAmountTotal; //TOTAL
                //VISA


                ws.Cell("F" + countVISA).Value = 0; //Transaction of Previous Date in Switch
                ws.Cell("G" + countVISA++).Value = 0; //Transaction of Previous Date in Switch
                ws.Cell("G" + countVISA++).Value = 0; //Transaction of Previous Date in CBS
                ws.Cell("F" + countVISA).Value = PayableTxnPrevDateVisaCount; //Transaction of Previous Date in VISA
                ws.Cell("G" + countVISA++).Value = PayableTxnPrevDateVisaAmount; //Transaction of Previous Date in VISA
                //ws.Cell("I" + countVISA).Value = "Mismatch: Count " + 123 + " amount " + 123 + " in " + excelName + "\r\n" + "Not reversed CBS FT number should be reflected in summary report.";//Remarks CODE NOT IN USE
                ws.Cell("G" + countVISA++).Value = 0; //FAILED -TXN
                ws.Cell("F" + countVISA).Value = "-" + PayableFailedTxnNotHitInCbsCount;  //Fail Txn but not hit in CBS
                ws.Cell("G" + countVISA++).Value = "-" + PayableFailedTxnNotHitInCbsAmount; //Fail Txn but not hit in CBS
                ws.Cell("F" + countVISA).Value = 0;  //Missing at CBS
                ws.Cell("I" + countVISA).Value = "Mbnl Credit Card & Dollar Prepaid Card txn";
                ws.Cell("G" + countVISA++).Value = 0; //Missing at CBS
                ws.Cell("G" + countVISA++).Value = 0;  //Missing at Npn
                ws.Cell("F" + countVISA).Value = PayableMissingAtVisaCount;  //Missing at VISA
                ws.Cell("G" + countVISA++).Value = PayableMissingAtVisaAmount;  //Missing at VISA
                ws.Cell("F" + countVISA).Value = 0;  //Transaction of Next Date in Switch
                ws.Cell("G" + countVISA++).Value = 0;  //Transaction of Next Date in Switch
                ws.Cell("G" + countVISA++).Value = 0; //Transaction of Next Date in CBS
                ws.Cell("F" + countVISA).Value = PayableTxnNextDateVisaCount;  //Transaction of Next Date in VISA
                ws.Cell("G" + countVISA++).Value = PayableTxnNextDateVisaAmount; //Transaction of Next Date in VISA
                ws.Cell("F" + countVISA).Value = VisaTotalPayableCount; //TOTAL
                ws.Cell("G" + countVISA++).Value = VisaTotalPayableAmount; //TOTAL 
                ws.Cell("G" + countVISA).Value = 0;

                ws.Range("B1", "I" + countVISA).Style.NumberFormat.Format = "_ * #,##0.00_ ;_ * -#,##0.00_ ;_ * \"-\"??_ ;_ @_ ";//positive numbers;negative numbers;zeros;text
                ws.Columns().AdjustToContents();  // Adjust column width
                ws.Rows().AdjustToContents();     // Adjust row heights
                var rngTable = ws.Range("A1:H" + countVISA);
                rngTable.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                rngTable.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                rngTable = ws.Range("A1:I" + countVISA);
                rngTable.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                wb.Author = "Elite";
                wb.ShowRowColHeaders = true;
                //foreach (IXLWorksheet workSheet in wb.Worksheets)
                //{
                //    foreach (IXLTable table in workSheet.Tables)
                //    {
                //        workSheet.Table(table.Name).ShowAutoFilter = true;
                //        workSheet.ConditionalFormats.RemoveAll();
                //        workSheet.Table(table.Name).ShowColumnStripes = true;
                //        workSheet.Table(table.Name).ShowRowStripes = true;
                //    }
                //}

                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition",
                    "attachment;filename=" + excelName + ".xlsx");
                using (var MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }                //                return View("Index", InitialVal(new Db.Filter()));
                return View("Index", filterModel);
            }
        }
        public ActionResult ExportFileHistoryRecord(int fileId)
        {
            var txnList = ReconBAL.GetListTransaction(fileId);
            var fileName = ReconBAL.GetFileNameById(fileId);
            fileName = Regex.Split(fileName, "Recon")[1];

            List<TransactionViewModel> lstTransactionViewModels = new List<TransactionViewModel>();
            foreach (var ob in txnList)
            {
                TransactionViewModel transactionViewModel = new TransactionViewModel();
                transactionViewModel.TransactionDate = ob.TransactionDate.ToString("yyyyMMdd");
                transactionViewModel.CardNo = ob.CardNo;
                transactionViewModel.TraceNo = ob.TraceNo;
                transactionViewModel.AuthCode = ob.AuthCode;
                transactionViewModel.ResponseCode = ob.ResponseCode;
                transactionViewModel.ResponseCodeDescription = ob.ResponseCodeDescription;
                transactionViewModel.ReferenceNo = ob.ReferenceNo;
                transactionViewModel.TerminalId = ob.TerminalId;
                transactionViewModel.TransactionAmount = ob.TransactionAmount;
                transactionViewModel.AccountNo = ob.AccountNo;
                transactionViewModel.Currency = ob.Currency;
                transactionViewModel.CBSValueDate = ob.CBSValueDate;
                transactionViewModel.AdviseDate = ob.AdviseDate;
                transactionViewModel.AdviseDate = ob.AdviseDate;
                transactionViewModel.TransactionStatus = Enum.GetName(typeof(TransactionStatus), ob.TransactionStatus);

                lstTransactionViewModels.Add(transactionViewModel);
            }
            var exportDataTable = UiHelpers.UIHelper.ToDataTable<Model.TransactionViewModel>(lstTransactionViewModels.OrderBy(x => x.TransactionDate).ToList());

            BaseExportExcel(exportDataTable, fileName);

            return View();
        }
        public ActionResult ExportTerminalRecord()
        {
            var terminaList = ReconBAL.GetAllTerminalListToExport();

            List<TerminalViewModel> lsTerminalViewModels = new List<TerminalViewModel>();
            foreach (var ob in terminaList)
            {
                TerminalViewModel transactionViewModel = new TerminalViewModel();
                transactionViewModel.TerminalId = ob.TerminalId;
                transactionViewModel.Address = ob.Address;
                transactionViewModel.Name = ob.Name;
                transactionViewModel.TerminalBrand = ob.TerminalBrand;

                lsTerminalViewModels.Add(transactionViewModel);
            }
            var exportDataTable = UiHelpers.UIHelper.ToDataTable<TerminalViewModel>(lsTerminalViewModels.OrderBy(x => x.Name).ToList());

            BaseExportExcel(exportDataTable, "Terminals");

            return View();
        }

        #endregion
    }
}
