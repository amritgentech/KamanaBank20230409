using Db.Enum;
using Db.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Db.Hepler;
using DbOperations;
using Helper.GlobalHelpers;

namespace Db
{
    /// <summary>
    /// This model will be used by ui. Both dashboard and report.
    /// </summary>
    public partial class Recon
    {
        /// <summary>
        /// All the variables will be set as per the recon type
        /// </summary>

        public VwTransactionDetailsModel VwTransactionDetailsModel;
        public Recon()
        {
            VwTransactionDetailsModel = new VwTransactionDetailsModel();
        }
        public Dashboard GetSummary(Filter _Filter)
        {
            List<VwTransactionDetailsModel> VwTransactionDetailsModels = VwTransactionDetailsModelByFilterType(_Filter);
            Dashboard _Dashboard = new Dashboard();
            _Dashboard.OnUsSummary = GetOnUsSummary(VwTransactionDetailsModels);
            _Dashboard.PayableSummary = GetPayableSummary(VwTransactionDetailsModels);
            _Dashboard.RecievableSummary = GetReceivableSummary(VwTransactionDetailsModels);

            if (_Filter.IsOwnUsPayableReceivable == null)
            {
                _Dashboard.Summary = new Summary();
                return _Dashboard;
            }

            if (_Filter.IsOwnUsPayableReceivable.Equals(IsOwnUsPayableReceivable.OnUs.ToString()))
                _Dashboard.Summary = _Dashboard.OnUsSummary;
            else if (_Filter.IsOwnUsPayableReceivable.Equals(IsOwnUsPayableReceivable.Receivable.ToString()))
                _Dashboard.Summary = _Dashboard.RecievableSummary;
            else
                _Dashboard.Summary = _Dashboard.PayableSummary;

            return _Dashboard;
        }
        public List<VwTransactionDetailsModel> VwTransactionDetailsModelByFilterType(Filter _Filter)
        {
            List<VwTransactionDetailsModel> VwTransactionDetailsModels = new List<VwTransactionDetailsModel>();

            VwTransactionDetailsModels = GetVwTransactionDetailsModels(_Filter);

            return VwTransactionDetailsModels;
        }
        public List<VwTransactionDetailsModel> GetVwTransactionDetailsModels(Filter filterModel)
        {
            //            var DateFrom = Helper.Helper.Helper.ChangeDateFormat(filterModel.FromDate);
            //            var DateTo = Helper.Helper.Helper.ChangeDateFormat(filterModel.ToDate);
            DbOperation dbOperation = new DbOperation("ReconContextConnectionString");
            DataTable resultDataTable = new DataTable();
            ParameterCollection collection = new ParameterCollection();
            collection.Add(new Parameter("@fromDate", GlobalHelper.ChangeDateFormatWithDash(filterModel.FromDate)));
            collection.Add(new Parameter("@toDate", GlobalHelper.ChangeDateFormatWithDash(filterModel.ToDate)));

            int count = 1;
            var fieldSelectorArray = Regex.Split(filterModel.ReconTypeName, "Vs");

            foreach (var reconName in fieldSelectorArray)
            {
                collection.Add(new Parameter("selector" + count, reconName.Trim()));
                count++;
            }

            if (filterModel.ReconTypeName.Equals("Npn Vs Cbs"))
            {
                resultDataTable = dbOperation.ExecuteDataTable("sp_TwoWayRecon", string.Empty, collection, CommandType.StoredProcedure);
            }
            if (filterModel.ReconTypeName.Equals("Visa Vs Cbs"))
            {
                resultDataTable = dbOperation.ExecuteDataTable("sp_VisaVsCbs", string.Empty, collection, CommandType.StoredProcedure);

                var ihkn = (from t1 in resultDataTable.AsEnumerable().Where(row => row.Field<int>("ReconSource") == 1 && row.Field<int>("TerminalType") == 2 && row.Field<string>("Status") == "Matched")
                            join t2 in resultDataTable.AsEnumerable().Where(row => row.Field<int>("ReconSource") == 1 && row.Field<int>("TerminalType") == 2 && row.Field<string>("Status") == "Matched")
                            on
                            new
                            {
                                VisaAmount = t1.Field<decimal?>("visaAmount"),
                                CbsAmount = t1.Field<decimal?>("cbsAmount"),
                                TermainId = t1.Field<string>("TerminalId"),
                                Card = t1.Field<string>("CardNumber"),
                                Date = t1.Field<DateTime>("TransactionDate")
                            }
                            equals
                            new
                            {
                                VisaAmount = t2.Field<decimal?>("visaAmount"),
                                CbsAmount = t2.Field<decimal?>("cbsAmount"),
                                TermainId = t2.Field<string>("TerminalId"),
                                Card = t2.Field<string>("CardNumber"),
                                Date = t2.Field<DateTime>("TransactionDate")
                            }
                            select new { trace1 = t1.Field<string>("TraceNo"), trace2 = t2.Field<string>("TraceNo") })
                            .Where(r => r.trace1 != r.trace2).Distinct().ToList();

                foreach (var item in ihkn)
                {
                    int countA = resultDataTable.AsEnumerable().Count(row => row.Field<string>("TraceNo") == item.trace1 && row.Field<string>("Status") == "Matched" && row.Field<DateTime>("cbsTransactionDate") == row.Field<DateTime>("visaTransactionDate"));
                    int countB = resultDataTable.AsEnumerable().Count(row => row.Field<string>("TraceNo") == item.trace2 && row.Field<string>("Status") == "Matched" && row.Field<DateTime>("cbsTransactionDate") == row.Field<DateTime>("visaTransactionDate"));

                    resultDataTable.AsEnumerable()
                                   .Where(row => row.Field<string>("TraceNo") == item.trace1)
                                   .Each(row => row.SetField(columnName: "Status", value: "Matched Invalid"));

                }

            }
            else if (filterModel.ReconTypeName.Equals("Npn Vs Cbs Vs Ej"))
            {
                resultDataTable = dbOperation.ExecuteDataTable("sp_ThreeWayRecon", string.Empty, collection, CommandType.StoredProcedure);
            }
            else if (filterModel.ReconTypeName.Equals("Npn Vs Cbs Vs Visa"))
            {
                resultDataTable = dbOperation.ExecuteDataTable("sp_ThreeWayRecon_Visa", string.Empty, collection, CommandType.StoredProcedure);
            }

            var list = GlobalHelper.DatatableToClass<VwTransactionDetailsModel>(resultDataTable).ToList();
            var result = FilterVwTransactionDetailsModels(list, filterModel);
            return result;
        }
        public List<VwTransactionDetailsModel> FilterVwTransactionDetailsModels(List<VwTransactionDetailsModel> _VwTransactionDetailsModels, Filter filter)
        {
            var DateFrom = GlobalHelper.ChangeDateFormat(filter.FromDate);
            var DateTo = GlobalHelper.ChangeDateFormat(filter.ToDate);

            List<VwTransactionDetailsModel> VwTransactionDetailsModels = new List<VwTransactionDetailsModel>();
            VwTransactionDetailsModels = _VwTransactionDetailsModels;
            if (_VwTransactionDetailsModels == null)
            {
                return null;
            }
            if (filter.Status != null)
            {
                VwTransactionDetailsModels = _VwTransactionDetailsModels.Where(x =>
                    x.Status.Equals(filter.Status)).ToList();
            }
            if (filter.TerminalId != null)
            {
                VwTransactionDetailsModels = VwTransactionDetailsModels.Where(x =>
                    x.TerminalId.Equals(filter.TerminalId)).ToList();
            }
            //            //for terminal type filter pos/atm ..
            if (!filter._TerminalType.Equals(TerminalType.ALL))
            {
                VwTransactionDetailsModels = VwTransactionDetailsModels.Where(x =>
                    Convert.ToInt32(x.TerminalType).Equals((int)filter._TerminalType)).ToList();
            }


            //check currency ..
            if (!filter._Currency.Equals(Filter.Currency.ALL))
            {
                if (filter._Currency.Equals(Filter.Currency.USD)) //except npr,inr all other are foreign currency..
                {
                    VwTransactionDetailsModels = VwTransactionDetailsModels.Where(x => x.Currency == null)
                        .ToList();
                }
                else
                {
                    VwTransactionDetailsModels = VwTransactionDetailsModels.Where(x => x.Currency != null)
                        .ToList();
                    VwTransactionDetailsModels = VwTransactionDetailsModels
                        .Where(x => Convert.ToInt32(x.Currency).Equals((int)filter._Currency)
                        ).ToList();
                }
            }
            if (!string.IsNullOrEmpty(filter.IsOwnUsPayableReceivable))
            {
                if (filter.IsOwnUsPayableReceivable.Equals(IsOwnUsPayableReceivable.OnUs.ToString()))
                    VwTransactionDetailsModels = GetOnUs(VwTransactionDetailsModels);
                else if (filter.IsOwnUsPayableReceivable.Equals(IsOwnUsPayableReceivable.Payable.ToString()))
                    VwTransactionDetailsModels = GetPayable(VwTransactionDetailsModels);
                else if (filter.IsOwnUsPayableReceivable.Equals(IsOwnUsPayableReceivable.Receivable.ToString()))
                    VwTransactionDetailsModels = GetReceivable(VwTransactionDetailsModels);
            }
            return VwTransactionDetailsModels;
        }
     
        protected Summary GetPayableSummary(List<VwTransactionDetailsModel> listVwTransactionDetailsModel)
        {
            List<VwTransactionDetailsModel> payableList = GetPayable(listVwTransactionDetailsModel);
            List<VwTransactionDetailsModel> payableMatchList = GetMatch(payableList);
            List<VwTransactionDetailsModel> payableUnMatchList = GetUnMatch(payableList);
            List<VwTransactionDetailsModel> payableInvalidList = GetInvalid(payableList);
            List<VwTransactionDetailsModel> payableDateDiffList = GetDateDiff(payableList);
            List<VwTransactionDetailsModel> payableExceptionList = GetException(payableList);
            List<VwTransactionDetailsModel> payableSuspectedList = GetSuspected(payableList);
            List<VwTransactionDetailsModel> payableCbsInvalidList = GetCbsInvalid(payableList);

            Summary _payableSummary = new Summary();
            _payableSummary.Amount = payableList.Sum(x => x.NpnAmount ?? 0.00);
            _payableSummary.Transactions = payableList.Count;
            _payableSummary.Invalid = payableInvalidList.Count;
            _payableSummary.UnMatched = payableUnMatchList.Count;
            _payableSummary.Matched = payableMatchList.Count;
            _payableSummary.DateDiff = payableDateDiffList.Count;
            _payableSummary.Exception = payableExceptionList.Count;
            _payableSummary.Suspected = payableSuspectedList.Count;
            _payableSummary.CbsInvalid = payableCbsInvalidList.Count;

            _payableSummary.Invalid_Amount = payableInvalidList.Sum(x => x.NpnAmount ?? 0.00);
            _payableSummary.UnMatched_Amount = payableUnMatchList.Sum(x => x.CbsAmount ?? 0.00);
            _payableSummary.Matched_Amount = payableMatchList.Sum(x => x.NpnAmount ?? 0.00);
            _payableSummary.DateDiff_Amount = payableDateDiffList.Sum(x => x.NpnAmount ?? 0.00);
            _payableSummary.Exception_Amount = payableExceptionList.Sum(x => x.NpnAmount ?? 0.00);
            _payableSummary.Suspected_Amount = payableSuspectedList.Sum(x => x.NpnAmount ?? 0.00);
            _payableSummary.CbsInvalid_Amount = payableCbsInvalidList.Sum(x => x.CbsAmount ?? 0.00);
            return _payableSummary;
        }
        protected Summary GetReceivableSummary(List<VwTransactionDetailsModel> listVwTransactionDetailsModel)
        {
            List<VwTransactionDetailsModel> receivableList = GetReceivable(listVwTransactionDetailsModel);
            List<VwTransactionDetailsModel> receivableMatchList = GetMatch(receivableList);
            List<VwTransactionDetailsModel> receivableUnMatchList = GetUnMatch(receivableList);
            List<VwTransactionDetailsModel> receivableInvalidList = GetInvalid(receivableList);
            List<VwTransactionDetailsModel> receivableDateDiffList = GetDateDiff(receivableList);
            List<VwTransactionDetailsModel> receivableExceptionList = GetException(receivableList);
            List<VwTransactionDetailsModel> receivableSuspectedList = GetSuspected(receivableList);
            List<VwTransactionDetailsModel> receivableCbsInvalidList = GetCbsInvalid(receivableList);

            Summary _receivableSummary = new Summary();
            _receivableSummary.Amount = receivableList.Sum(x => x.NpnAmount ?? 0.00);
            _receivableSummary.Transactions = receivableList.Count;
            _receivableSummary.Invalid = receivableInvalidList.Count;
            _receivableSummary.UnMatched = receivableUnMatchList.Count;
            _receivableSummary.Matched = receivableMatchList.Count;
            _receivableSummary.DateDiff = receivableDateDiffList.Count;
            _receivableSummary.Exception = receivableExceptionList.Count;
            _receivableSummary.Suspected = receivableSuspectedList.Count;
            _receivableSummary.CbsInvalid = receivableCbsInvalidList.Count;

            _receivableSummary.Invalid_Amount = receivableInvalidList.Sum(x => x.NpnAmount ?? 0.00);
            _receivableSummary.UnMatched_Amount = receivableUnMatchList.Sum(x => x.CbsAmount ?? 0.00);
            _receivableSummary.Matched_Amount = receivableMatchList.Sum(x => x.NpnAmount ?? 0.00);
            _receivableSummary.DateDiff_Amount = receivableDateDiffList.Sum(x => x.NpnAmount ?? 0.00);
            _receivableSummary.Exception_Amount = receivableExceptionList.Sum(x => x.NpnAmount ?? 0.00);
            _receivableSummary.Suspected_Amount = receivableSuspectedList.Sum(x => x.CbsAmount ?? 0.00);
            _receivableSummary.CbsInvalid_Amount = receivableCbsInvalidList.Sum(x => x.CbsAmount ?? 0.00);
            return _receivableSummary;
        }
        protected Summary GetOnUsSummary(List<VwTransactionDetailsModel> listVwTransactionDetailsModel)
        {
            List<VwTransactionDetailsModel> onUsList = GetOnUs(listVwTransactionDetailsModel);
            List<VwTransactionDetailsModel> onUsMatchList = GetMatch(onUsList);
            List<VwTransactionDetailsModel> onUsUnMatchList = GetUnMatch(onUsList);
            List<VwTransactionDetailsModel> onUsInvalidList = GetInvalid(onUsList);
            List<VwTransactionDetailsModel> onUsDateDiffList = GetDateDiff(onUsList);
            List<VwTransactionDetailsModel> onUsExceptionList = GetException(onUsList);
            List<VwTransactionDetailsModel> onUsSuspectedList = GetSuspected(onUsList);
            List<VwTransactionDetailsModel> onUsCbsInvalidList = GetCbsInvalid(onUsList);

            Summary _onUsSummary = new Summary();
            _onUsSummary.Amount = onUsList.Sum(x => x.NpnAmount ?? 0.00);
            _onUsSummary.Transactions = onUsList.Count;
            _onUsSummary.Invalid = onUsInvalidList.Count;
            _onUsSummary.UnMatched = onUsUnMatchList.Count;
            _onUsSummary.Matched = onUsMatchList.Count;
            _onUsSummary.DateDiff = onUsDateDiffList.Count;
            _onUsSummary.Exception = onUsExceptionList.Count;
            _onUsSummary.Suspected = onUsSuspectedList.Count;
            _onUsSummary.CbsInvalid = onUsCbsInvalidList.Count;

            _onUsSummary.Invalid_Amount = onUsInvalidList.Sum(x => x.NpnAmount ?? 0.00);
            _onUsSummary.UnMatched_Amount = onUsUnMatchList.Sum(x => x.CbsAmount ?? 0.00);
            _onUsSummary.Matched_Amount = onUsMatchList.Sum(x => x.NpnAmount ?? 0.00);    
            _onUsSummary.DateDiff_Amount = onUsDateDiffList.Sum(x => x.NpnAmount ?? 0.00);
            _onUsSummary.Exception_Amount = onUsExceptionList.Sum(x => x.NpnAmount ?? 0.00);
            _onUsSummary.Suspected_Amount = onUsSuspectedList.Sum(x => x.CbsAmount ?? 0.00);
            _onUsSummary.CbsInvalid_Amount = onUsCbsInvalidList.Sum(x => x.CbsAmount ?? 0.00);
            return _onUsSummary;
        }
        protected List<VwTransactionDetailsModel> GetPayable(List<VwTransactionDetailsModel> listVwTransactionDetailsModel)
        {
            return
                listVwTransactionDetailsModel.FindAll(
                    tr => tr.TerminalOwner != (int)TerminalOwner.OwnTerminal
                          && (tr.CardType == (int)CardType.OwnCard || tr.CardType == (int)CardType.CreditCard));
        }
        protected List<VwTransactionDetailsModel> GetReceivable(List<VwTransactionDetailsModel> listVwTransactionDetailsModel)
        {
            var list = listVwTransactionDetailsModel.FindAll(tr => tr.TerminalOwner == (int)TerminalOwner.OwnTerminal
                                                               && tr.CardType != (int)CardType.OwnCard);
            return list;
        }
        protected List<VwTransactionDetailsModel> GetOnUs(List<VwTransactionDetailsModel> listVwTransactionDetailsModel)
        {
            var list = listVwTransactionDetailsModel.Where(tr => tr.TerminalOwner == (int)TerminalOwner.OwnTerminal
                                                             && tr.CardType == (int)CardType.OwnCard).ToList();
            return list;
        }
        protected List<VwTransactionDetailsModel> GetMatch(List<VwTransactionDetailsModel> listVwTransactionDetailsModel)
        {
            return listVwTransactionDetailsModel.FindAll(tr => tr.Status == ReconStatus.Matched.ToString());
        }
        protected List<VwTransactionDetailsModel> GetUnMatch(List<VwTransactionDetailsModel> listVwTransactionDetailsModel)
        {
            return listVwTransactionDetailsModel.FindAll(tr => tr.Status == ReconStatus.Failed.ToString()); //Un_Matched
        }
        protected List<VwTransactionDetailsModel> GetInvalid(List<VwTransactionDetailsModel> listVwTransactionDetailsModel)
        {
            return listVwTransactionDetailsModel.FindAll(tr => tr.Status == ReconStatus.Invalid.ToString());
        }
        protected List<VwTransactionDetailsModel> GetDateDiff(List<VwTransactionDetailsModel> listVwTransactionDetailsModel)
        {
            return listVwTransactionDetailsModel.FindAll(tr => tr.Status == ReconStatus.DateDiff.ToString());
        }
        protected List<VwTransactionDetailsModel> GetException(List<VwTransactionDetailsModel> listVwTransactionDetailsModel)
        {
            return listVwTransactionDetailsModel.FindAll(tr => tr.Status == ReconStatus.Exception.ToString());
        }
        protected List<VwTransactionDetailsModel> GetSuspected(List<VwTransactionDetailsModel> listVwTransactionDetailsModel)
        {
            return listVwTransactionDetailsModel.FindAll(tr => tr.Status == ReconStatus.Suspected.ToString());
        }
        protected List<VwTransactionDetailsModel> GetCbsInvalid(List<VwTransactionDetailsModel> listVwTransactionDetailsModel)
        {
            return listVwTransactionDetailsModel.FindAll(tr => tr.Status == ReconStatus.CbsInvalid.ToString()); //it is actually exception txn but effect in summar report for cbs total case to deduct from ther it required..
        }
    }
}
