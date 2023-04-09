using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReconUi.Models.DigitalBanking
{
    public class MirrorCbsTransactionViewModel
    {
        public string TransactionDate { get; set; }
        public string Particulars { get; set; }
        public Decimal TransactionAmount { get; set; }
        public string DebitOrCredit { set; get; }
        public string ReferenceNumber { set; get; }
        public String Balance { set; get; }
    }
}