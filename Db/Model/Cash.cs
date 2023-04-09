using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Model
{
    public class Cash : Base
    {
        public int CashId { get; set; }
        public string Note { get; set; }
        public string Description { get; set; }

        public static Cash Find(int CashId){
            Cash _Cash = null;
            using (var context = new ReconContext())
            {
                _Cash = context.Cashs.First(c => c.CashId == CashId);
            }
            return _Cash;
        }
    }
}
