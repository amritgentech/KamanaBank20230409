using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Model
{
    public class Source : Base
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SourceId { get; set; }
        public String Description { get; set; }
        public virtual ICollection<SubSource> SubSources { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<EsewaTransaction> EsewaTransactions { get; set; }
        public virtual ICollection<MirrorTransaction> MirrorTransactions { get; set; }
        public virtual ICollection<CbsTransaction> CbsTransactions { get; set; }
        public virtual ICollection<NostroTransaction> NostroTransactions { get; set; }
        public virtual ICollection<MobileTopupTransaction> MobileTopupTransactions { get; set; }
        public virtual ICollection<MobileIbftTransaction> MobileIbftTransactions { get; set; }
        public virtual ICollection<InternetTopupTransaction> InternetTopupTransactions { get; set; }
        public virtual ICollection<InternetIbftTransaction> InternetIbftTransactions { get; set; }
        public List<Transaction> GetTransactions(DateTime FromDate, DateTime ToDate)
        {
            List<Transaction> _Transactions = null;
            using(var context = new ReconContext())
            {
                _Transactions = context.Transactions.Where(t => t.TransactionDate >= FromDate && t.TransactionDate <= ToDate && t.Source.SourceId == SourceId).ToList<Transaction>();
            }
            return _Transactions;
        }

        public static Source Find(int sourceId)
        {
            return FindSource(s => s.SourceId == sourceId);
        }

        public static Source FindName(string sourceName)
        {
            return FindSource(s => s.Description == sourceName);
        }

        public static Source Visa()
        {
            return FindSource(s => s.Description == "VISA");
        }

        public static Source EJ()
        {
            return FindSource(s => s.Description == "EJOURNAL");
        }

        public static Source NPN()
        {
            return FindSource(s => s.Description == "NPN");
        }

        public static Source CBS()
        {
            return FindSource(s => s.Description == "CBS");
        }
        public static Source DigitalBanking()
        {
            var name = FindSource(s => s.Description == "DIGITALBANKING");
            return name;
        }

        public static List<Source> All()
        {
            List<Source> sources = null;
            using (var context = new ReconContext())
            {
                sources = context.Sources.ToList();
            }
            return sources;
        }

        public static Source FindSource(Func<Source, bool> where)
        {
            Source source = null;
            using (var context = new ReconContext())
            {
                source = context.Sources.First(where);
            }
            return source;
        }
    }
}
