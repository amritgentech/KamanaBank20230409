using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Model
{
   public class NostroTransaction:Base
    {
        [Key]
        public int NostroTransactionId { get; set; }
        public string TransactionId { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime ValueDate { get; set; }
        public String ChequeNo { get; set; }
        public Decimal TransactionAmount { get; set; }
        public Decimal? BalanceAmount { get; set; }
        public string Description { get; set; }
        public string UniqueId { get; set; }
        public int Source_SourceId { set; get; }
        public int SubSource_SubSourceId { get; set; }
        public int UploadedFile_UploadedFileId { get; set; }
        [ForeignKey("Source_SourceId")]
        public virtual Source Source { get; set; }
        [ForeignKey("SubSource_SubSourceId")]
        public virtual SubSource SubSource { get; set; }
        [ForeignKey("UploadedFile_UploadedFileId")]
        public virtual UploadedFile UploadedFile { get; set; }
    }
}
