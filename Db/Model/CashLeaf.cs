using Db.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Model
{
    public class CashLeaf : Base
    {
        public int CashLeafId { get; set; }
        public PhysicalCassettePosition PhysicalCassettePosition { get; set; }
        public int TotalNoteCount { get; set; }
        public virtual Cash Cash { get; set; }
    }
}
