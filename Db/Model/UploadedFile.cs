using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Model
{
    public class UploadedFile : Base
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UploadedFileId { get; set; }
        public String ActualFileName { get; set; }
        public String ShowFileName { get; set; }
        public String Catagory { get; set; }
        public int SourceId { get; set; }
        public int? SubSourceId { get; set; }
        public int? SubChildSourceId { get; set; }
        public int MinTransactionId { get; set; }
        public int MaxTransactionId { get; set; }
        public String TerminalId { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<EsewaTransaction> EsewaTransactions { get; set; }
        public virtual ICollection<CbsTransaction> CbsTransactions { get; set; }
        public virtual ICollection<MirrorTransaction> MirrorTransactions { get; set; }
        public virtual ICollection<NostroTransaction> NostroTransactions { get; set; }
        public virtual ICollection<MobileTopupTransaction> MobileTopupTransactions { get; set; }
        public virtual ICollection<MobileIbftTransaction> MobileIbftTransactions { get; set; }
        public virtual ICollection<InternetTopupTransaction> InternetTopupTransactions { get; set; }
        public virtual ICollection<InternetIbftTransaction> InternetIbftTransactions { get; set; }
    }
}
