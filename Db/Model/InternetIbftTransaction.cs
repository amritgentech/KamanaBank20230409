using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Model
{
    public class InternetIbftTransaction : Base
    {
        [Key]
        public int InternetIbftTransactionId { get; set; }
        public string CustomerName { get; set; }
        public string MobileNumber { get; set; }
        public string UserName { get; set; }
        public string FromAccount { get; set; }
        public string ToAccount { get; set; }
        public string BankCode { get; set; }
        public Decimal TransactionAmount { get; set; }
        public Decimal? ChargeAmount { get; set; }
        public DateTime TransactionDate { get; set; }
        public String TransactionUri { get; set; }
        public String OrginatingUniqueId { get; set; }
        public string TransactionStatus { get; set; }
        public string Description { get; set; }
        public string TraceId { get; set; }
        public String ServiceInfoId { get; set; }
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
