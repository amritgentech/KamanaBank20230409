using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Db
{
    public class ReportFilterModel
    {
        [Required(ErrorMessage = "Please select  Recon Type")]
        public int ReconTypeId { get; set; }
        public string ReconTypeName { get; set; }
        public string DateFromText { get; set; }
        public string DateToText { get; set; }
        public string CurrencyType { get; set; }//NPR,INR,USD
        public string TerminalType { get; set; }//ATM,POS
        public string Status { get; set; }//Match,Un_Match,Invalid
        public string IsOwnUsPayableReceivable { get; set; }//OWNUS,PAYABLE,RECEIVABLE
        public string TerminalId { get; set; }

        //initial transaction result..
        public string TotalTxns { get; set; }
        public string Matched { get; set; }
        public string UnMatched { get; set; }
        public string Invalid { get; set; }
        public string Payable { get; set; }
        public string Receivable { get; set; }
        public string TotalTxnAmount { get; set; }
       
    }
}