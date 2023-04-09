using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Model
{
    public partial class ATMCashRefill : Base
    {
        public int ATMCashRefillId { get; set; }
        public string TerminalId { get; set; }
        public decimal RefilledAmount { get; set; }
        public Nullable<decimal> RetainedAmount { get; set; }
        public Nullable<decimal> RetractedAmount { get; set; }
        public Nullable<decimal> RejectedAmount { get; set; }
        public Nullable<decimal> RemaingAmount { get; set; }
        public System.DateTime RefilledDate { get; set; }
        public int IsBeforeOrAfterRefill { get; set; }
        public int Source_SourceId { get; set; }
        [ForeignKey("Source_SourceId")]
        public virtual Source Source { get; set; }
        public virtual List<ATMCashRefillDetail> ATMCashRefillDetails { get; set; }
    }
}
