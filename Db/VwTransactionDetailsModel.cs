using Db.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Db.Model;

namespace Db
{
    public class VwTransactionDetailsModel
    {
        public int TransactionId { get; set; }
        public int CbsTransactionId { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string CardNumber { get; set; }
        public string ReconSource { get; set; }
        public string TerminalId { get; set; }
        public string TraceNo { get; set; }
        public string ReferenceNo { get; set; }
        public string AuthCode { get; set; }
        public int? TerminalOwner { get; set; }
        public int? CardType { get; set; }
        public string TransactionType { get; set; }
        public string ReconStatus { get; set; }
        public double? CbsAmount { get; set; }
        public string CbsResponseCode { get; set; }
        public DateTime? CbsTransactionDate { get; set; }
        public int? CbsTransactionStatus { get; set; }
        public string CbsResponseCodeDescription { get; set; }
        public string CbsAdviseDate { get; set; }
        public string CbsMainCode { get; set; }
        public double? NepsAmount { get; set; }
        public string NepsResponseCode { get; set; }
        public DateTime? NepsTransactionDate { get; set; }
        public int? NepsTransactionStatus { get; set; }
        public string NepsResponseCodeDescription { get; set; }
        public string NepsAdviseDate { get; set; }
        public double? NpnAmount { get; set; }
        public string NpnResponseCode { get; set; }
        public DateTime? NpnTransactionDate { get; set; }
        public int? NpnTransactionStatus { get; set; }
        public string NpnResponseCodeDescription { get; set; }
        public string NpnAdviseDate { get; set; }
        public double? EjAmount { get; set; }
        public string EjResponseCode { get; set; }
        public DateTime? EjTransactionDate { get; set; }
        public int? EjTransactionStatus { get; set; }
        public string EjResponseCodeDescription { get; set; }
        public string EjAdviseDate { get; set; }
        public string EjCurrency { get; set; }
        public double? VisaAmount { get; set; }
        public string VisaResponseCode { get; set; }
        public DateTime? VisaTransactionDate { get; set; }
        public DateTime? VisaTransactionGmtDate { get; set; }
        public int? VisaTransactionStatus { get; set; }
        public string VisaResponseCodeDescription { get; set; }
        public DateTime? VisaAdviseDate { get; set; }
        public string VisaCurrency { get; set; }
        public string Status { get; set; }
        public int? TerminalType { get; set; }
        public int? Currency { get; set; }
        public string AccountNo { get; set; }
        public string CBSRefValue { get; set; }
        public int Issuing_Bank { get; set; }
    }
}
