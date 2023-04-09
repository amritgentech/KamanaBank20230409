using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Model
{
   public class CbsTransaction : Base
    {
        [Key]
        public int CbsTransactionId { get; set; }
        public DateTime TransactionDate { get; set; }
        public String Particulars { get; set; }
        public Decimal TransactionAmount { get; set; }
        public string DebitOrCredit { get; set; }
        public String ReferenceNumber { get; set; }
        public string IbftCreditUniqueId { get; set; }
        public String Balance { get; set; }
        public int Source_SourceId { set; get; }
        public int SubSource_SubSourceId { get; set; }
        public int SubChildSource_SubChildSourceId { get; set; }
        public int UploadedFile_UploadedFileId { get; set; }
        [ForeignKey("Source_SourceId")]
        public virtual Source Source { get; set; }
        [ForeignKey("SubSource_SubSourceId")]
        public virtual SubSource SubSource { get; set; }
        [ForeignKey("SubChildSource_SubChildSourceId")]
        public virtual SubChildSource SubChildSource { get; set; }
        [ForeignKey("UploadedFile_UploadedFileId")]
        public virtual UploadedFile UploadedFile { get; set; }
    }
}
