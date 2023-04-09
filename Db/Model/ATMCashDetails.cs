using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Model
{
    public partial class ATMCashRefillDetail : Base
    {
        public int ATMCashRefillDetailId { get; set; }
        public virtual Cash Cash { get; set; }
        private Nullable<int> CassetteNo { get; set; }
        public int TotalNoteCount { get; set; }
        public int IsRefill { get; set; }
    }
}
