using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReconUi.Models.DigitalBanking
{
    public class EsewaTransactionViewModel
    {
        public String EsewaId { get; set; }
        public Decimal TransactionAmount { get; set; }
        public string Status { get; set; }
        public String UniqueId { get; set; }
        public string Settlement { get; set; }
        public string TransactionDate { get; set; }
        public String NoofAttempt { get; set; }
        public String Remark { get; set; }
        public String InitiatingAc { get; set; }
        public String CreatedBy { get; set; }
        public String VerifiedBy { get; set; }
        public string LastModifiedDate { get; set; }
    }
}