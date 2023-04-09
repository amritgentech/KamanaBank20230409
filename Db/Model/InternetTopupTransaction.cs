using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Model
{
    public class InternetTopupTransaction : Base
    {
        [Key]
        public int InternetTopupTransactionId { get; set; }
        public string TransactionTraceId { get; set; }
        public string CustomerName { get; set; }
        public string UserName { get; set; }
        public string Profile { get; set; }
        public string TopupTraceId { get; set; }
        public string FromAccount { get; set; }
        public string ToAccount { get; set; }
        public Decimal TransactionAmount { get; set; }
        public DateTime TransactionDate { get; set; }
        public String TopupNumber { get; set; }
        public string TransactionStatus { get; set; }
        public String Remarks { get; set; }
        public string TopupStatus { get; set; }
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
