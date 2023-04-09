using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Db.Model
{
    public class SubChildSource : Base
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SubChildSourceId { get; set; }
        public String Description { get; set; }
        public String SourceChildDescription { get; set; }
        public virtual SubSource SubSource { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<CbsTransaction> CbsTransactions { get; set; }

        public static SubChildSource Find_By_SubSourceId(int SubSourceId)
        {
            //            return FindSubSource(s => s.SubSource.SubSourceId == SubSourceId);
            SubChildSource subChildSource = null;
            using (var context = new ReconContext())
            {
                subChildSource = context.SubChildSources.Where(x => x.SubSource.SubSourceId == SubSourceId)
                    .FirstOrDefault();
            }
            return subChildSource;
        }
        //digital banking
        public static SubChildSource Find_By_Name(string SubChildSourceName)
        {
            return FindSubChildSource(s => s.Description == SubChildSourceName);
        }
        public static SubChildSource FindSubChildSource(Func<SubChildSource, bool> where)
        {
            SubChildSource subChildSource = null;
            using (var context = new ReconContext())
            {
                subChildSource = context.SubChildSources.First(where);
            }
            return subChildSource;
        }
        public static SubChildSource FindSubSource(Func<SubChildSource, bool> where)
        {
            SubChildSource subChildSource = null;
            using (var context = new ReconContext())
            {
                subChildSource = context.SubChildSources.First(where);
            }
            return subChildSource;
        }
    }
}
