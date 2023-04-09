using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReconUi.Models.DigitalBanking
{
    public class InternetIbftTransactionViewModel
    {
        public string CustomerName { get; set; }
        public string MobileNumber { get; set; }
        public string UserName { get; set; }
        public string FromAccount { get; set; }
        public string ToAccount { get; set; }
        public string BankCode { get; set; }
        public Decimal TransactionAmount { get; set; }
        public Decimal? ChargeAmount { get; set; }
        public string TransactionDate { get; set; }
        public String TransactionUri { get; set; }
        public String OrginatingUniqueId { get; set; }
        public string TransactionStatus { get; set; }
        public string Description { get; set; }
        public string TraceId { get; set; }
        public String ServiceInfoId { get; set; }
    }
}