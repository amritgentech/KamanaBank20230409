using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReconUi.Models.DigitalBanking
{
    public class NostroTransactionViewModel
    {
        public string TransactionId { get; set; }
        public string TransactionDate { get; set; }
        public string ValueDate { get; set; }
        public string ChequeNo { get; set; }
        public Decimal TransactionAmount { get; set; }
        public Decimal? BalanceAmount { set; get; }
        public string Description { set; get; }
        public String UniqueId { set; get; }
    }
}