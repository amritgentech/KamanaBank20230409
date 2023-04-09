using Db.Enum;
using Db.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Db
{
    public class Filter
    {
        public Filter()
        {

        }
        public ReconType _ReconType { get; set; }
        public TerminalType _TerminalType { get; set; }
        public Terminal _Terminal { get; set; }
        [DisplayName("Recon Type")]
        public int ReconType { get; set; }
        [Required(ErrorMessage = "Start Date is required")]
        public String FromDate { get; set; }
        [Required(ErrorMessage = "End Date is required")]
        public String ToDate { get; set; }
        public Currency _Currency { get; set; }
        public int TerminalTypeId { get; set; }
        public String TerminalId { get; set; }
        public enum Currency { NPR = 524, INR = 356, USD, ALL }
        public enum Terminal { ATM, POS, ALL }
        [DisplayName("Reason Type")]
        public int ReasonType { get; set; }
        public Dictionary<Int32, String> PartialReconTypeHash = new Dictionary<int, string>
        {
//                        { 1, "~/Views/Report/ReconGridType/VISAVsCBS.cshtml" },
//                        { 2, "~/Views/Report/ReconGridType/ATMVsCBS.cshtml" },
//                        { 3, "~/Views/Report/ReconGridType/VISAVsCBSVsATM.cshtml" },
                       // { 4, "~/Views/Report/ReconGridType/SCTVsCBS.cshtml" },
                        //{ 5, "~/Views/Report/ReconGridType/SWITCHVsCBS.cshtml" },
                        //{ 6, "~/Views/Report/ReconGridType/HBLVsCBS.cshtml" },
//                        { 4, "~/Views/Report/ReconGridType/NPNVsCBS.cshtml" },
                        { 1, "~/Views/Report/ReconGridType/NPNVsCBS.cshtml" },
                        //{ 9, "~/Views/Report/ReconGridType/ATMVsSCTVsCBS.cshtml" },
                        //{ 10, "~/Views/Report/ReconGridType/ATMVsNPNVsCBS.cshtml" },
                        { 2, "~/Views/Report/ReconGridType/ATMVsNPNVsCBS.cshtml" },
                        //{ 12, "~/Views/Report/ReconGridType/ATMVsSWITCHVsCBS.cshtml" },
                        { 3, "~/Views/Report/ReconGridType/NPNVsCBSVsVISA.cshtml" },
            { 4, "~/Views/Report/ReconGridType/ESEWAVsCBS.cshtml" },
            { 5, "~/Views/Report/ReconGridType/TOPUPVsCBS.cshtml" }
        };
        public String currentPartial
        {
            get
            {
                return Partial();
            }
        }
        public String Partial()
        {
            foreach (KeyValuePair<Int32, String> keyValuePair in PartialReconTypeHash)
            {
                if (keyValuePair.Key == ReconType)
                {
                    return keyValuePair.Value;
                }
            }
            return null;
        }
        public string Status { get; set; }//Match,Un_Match,Invalid { get; set; }
        public string ReconTypeName { get; set; }
        public string IsOwnUsPayableReceivable { get; set; }//OWNUS,PAYABLE,RECEIVABLE

        //to pass dashboard info directly to the report controller
        public double amountAccordingToStatus { get; set; }
        public int statusTotalTxnRecord { get; set; }
        public String InitialDateFrom
        {
            get
            {
                if (string.IsNullOrEmpty(FromDate))
                {
                    //                    return Transaction.MaxTransactionDate();
                    return DateTime.Now.ToString("dd/MM/yyyy");
                }
                else
                {
                    if (FromDate == null)
                    {
                        return DateTime.Now.ToString("dd/MM/yyyy");
                    }
                    return FromDate;
                }
            }
        }
        public String InitialDateTo
        {
            get
            {
                if (string.IsNullOrEmpty(ToDate))
                {
                    //                    return Transaction.MaxTransactionDate();
                    return DateTime.Now.ToString("dd/MM/yyyy");
                }
                else
                {
                    if (ToDate == null)
                    {
                        return DateTime.Now.ToString("dd/MM/yyyy");
                    }
                    return ToDate;
                }
            }
        }

    }
}
