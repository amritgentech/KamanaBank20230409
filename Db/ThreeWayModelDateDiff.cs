using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;

namespace Db
{
    public class ThreeWayModelDateDiff
    {
        public string TerminalId { get; set; }
        public string CardNo { get; set; }
        public string AccountNo { get; set; }
        public string TraceNo { get; set; }
        public string AuthCode { get; set; }
        public string TerminalType { get; set; }
        public string Currency { get; set; }
        public string StatusSource { get; set; }
        public string TransactionDateSource { get; set; }
        public double? TransactionAmountSource { get; set; }
        public string ResponseCodeSource { get; set; }
        public string TransactionDateDestination1 { get; set; }
        public double? TransactionAmountDestination1 { get; set; }
        public string ResponseCodeDestination1 { get; set; }
        public string TransactionDateDestination2 { get; set; }
        public double? TransactionAmountDestination2 { get; set; }
        public string ResponseCodeDestination2 { get; set; }
        public string CPDDate { get; set; }
        public string FT_Branch { get; set; }
        public string Remarks { get; set; }
        public string CbsMainCode { get; set; }
        public string ReferenceNo { get; set; }
        public string VisaCurrency { get; set; }
    }
}