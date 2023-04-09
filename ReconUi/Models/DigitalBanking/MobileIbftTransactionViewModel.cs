using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReconUi.Models.DigitalBanking
{
    public class MobileIbftTransactionViewModel
    {
        public string Id { get; set; }
        public String MobileNumber { get; set; }
        public string FromAccount { get; set; }
        public string ToAccount { get; set; }
        public Decimal? ChargeAmount { get; set; }
        public Decimal TransactionAmount { get; set; }
        public string TransactionDate { get; set; }
        public String AccessibleChannel { get; set; }
        public string TransactionStatus { get; set; }
        public String TransactionCode { get; set; }
        public string TransactionDescription { get; set; }
        public string DestinationAccountDescription { get; set; }
        public string FonepayTraceId { get; set; }
        public string FonepayTransactionStatus { get; set; }
        public string TopupResponseDescription { get; set; }
        public string TransactionTraceId { get; set; }
        public string StanID { get; set; }
    }
}