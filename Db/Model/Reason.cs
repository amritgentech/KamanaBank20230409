using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Model
{
    public class Reason : Base
    {
        public int ReasonId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDisplay { get; set; }

        public static Reason Find(int ReasonId)
        {
            Reason reason = null;
            using (var context = new ReconContext())
            {
                reason = context.Reasons.First(t => t.ReasonId == ReasonId);
            }
            return reason;
        }

        public static Reason Where(Func<Reason, bool> where)
        {
            Reason reason = null;
            using (var context = new ReconContext())
            {
                reason = context.Reasons.First(where);
            }
            return reason;
        }
    }
}
