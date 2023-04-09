using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Model
{
    public class MobileIbftTransaction : Base
    {
        [Key]
        public int MobileIbftTransactionId { get; set; }
        public string Id { get; set; }
        public String MobileNumber { get; set; }
        public string FromAccount { get; set; }
        public string ToAccount { get; set; }
        public Decimal? ChargeAmount { get; set; }
        public Decimal TransactionAmount { get; set; }
        public DateTime TransactionDate { get; set; }
        public String AccessibleChannel { get; set; }
        public string TransactionStatus { get; set; }
        public String TransactionCode { get; set; }
        public string TransactionDescription { get; set; }
        public string DestinationAccountDescription { get; set; }
        public string FonepayTraceId { get; set; }
        public string FonepayTransactionStatus { get; set; }
        public string TopupResponseDescription { get; set; }
        public string TransactionTraceId { get; set; }
        public string StanID { get; set; }
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
