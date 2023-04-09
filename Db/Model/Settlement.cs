using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Model
{
    public class Settlement : Base
    {
        public int SettlementId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TerminalId { get; set; }
        public string CardNo { get; set; }
        public string TraceNo { get; set; }
        public string AuthCode { get; set; }
        public string VisaTransactionId { get; set; }
        public string NpnTransactionId { get; set; }
        public string CBSTransactionId { get; set; }
        public Decimal VisaTransactionAmount { get; set; }
        public Decimal NpnTransactionAmount { get; set; }
        public Decimal CbsTransactionAmount { get; set; }
        public string VisaResponseCode { get; set; }
        public string NpnResponseCode { get; set; }
        public string CbsResponseCode { get; set; }
        public string TerminalType { get; set; }
        public string FT_Branch { get; set; }
        public string IsOwnUsPayableReceivable { get; set; }//OWNUS,PAYABLE,RECEIVABLE
        public string Status { get; set; }
        public string Reason { get; set; }
        public bool IsSettled { get; set; }
    }
}