using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Model
{
  public class EsewaTransaction: Base
    {
        [Key]
        public int EsewaTransactionId { get; set; }
        public String EsewaId { get; set; }
        public Decimal TransactionAmount { get; set; }
        public string Status { get; set; }
        public String UniqueId { get; set; }
        public string Settlement { get; set; }
        public DateTime TransactionDate { get; set; }
        public String NoofAttempt { get; set; }
        public String Remark { get; set; }
        public String InitiatingAc { get; set; }
        public String CreatedBy { get; set; }
        public String VerifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
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
