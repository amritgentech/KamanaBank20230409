using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db
{
   public class SettlementModel
    {
        public int SettlementId { get; set; }
        public string VisaTransactionId { get; set; }
        public string NpnTransactionId { get; set; }
        public string CBSTransactionId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TerminalId { get; set; }
        public string CardNo { get; set; }
        public string TraceNo { get; set; }
        public string AuthCode { get; set; }
        public string Currency { get; set; }
        public string VisaTransactionAmount { get; set; }
        public string NpnTransactionAmount { get; set; }
        public string CbsTransactionAmount { get; set; }
        public string VisaResponseCode { get; set; }
        public string NpnResponseCode { get; set; }
        public string CbsResponseCode { get; set; }
        public string TerminalType { get; set; }
        public string FT_Branch { get; set; }
        public string IsOwnUsPayableReceivable { get; set; }//OWNUS,PAYABLE,RECEIVABLE
        public string Status { get; set; }
        public string Reason { get; set; }
        public bool IsChecked { get; set; }
    }
}
