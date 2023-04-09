using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Db.Model
{
    public class SubSource : Base
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SubSourceId { get; set; }
        public String Description { get; set; }
        public virtual Source Source { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<SubChildSource> SubChildSources { get; set; }
        public virtual ICollection<CbsTransaction> CbsTransactions { get; set; }
        public virtual ICollection<EsewaTransaction> EsewaTransactions { get; set; }
        public virtual ICollection<MirrorTransaction> MirrorTransactions { get; set; }
        public virtual ICollection<NostroTransaction> NostroTransactions { get; set; }
        public virtual ICollection<MobileTopupTransaction> MobileTopupTransactions { get; set; }
        public virtual ICollection<MobileIbftTransaction> MobileIbftTransactions { get; set; }
        public virtual ICollection<InternetTopupTransaction> InternetTopupTransactions { get; set; }
        public virtual ICollection<InternetIbftTransaction> InternetIbftTransactions { get; set; }

        public static SubSource Find(int SubSourceId)
        {
            return FindSubSource(s => s.SubSourceId == SubSourceId);
        }
        
        public static SubSource Find_By_Name(string SubSourceName)
        {
            return FindSubSource(s => s.Description == SubSourceName);
        }
        public static SubSource Find_By_SourceId(int Sourceid)
        {
            SubSource subSource = null;
            using (var context = new ReconContext())
            {
                subSource = context.SubSources.Where(x => x.Source.SourceId == Sourceid).FirstOrDefault();
            }
            return subSource;
        }
        public static SubSource FindSubSource(Func<SubSource, bool> where)
        {
            SubSource subSource = null;
            using (var context = new ReconContext())
            {
                subSource = context.SubSources.First(where);
            }
            return subSource;
        }

        public List<Transaction> FetchRecords(int pageSize, int pageNumber)
        {
            return Transactions.OrderBy(t => t.TransactionId)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize).ToList();
        }
    }
}
