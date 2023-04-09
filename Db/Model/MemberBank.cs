using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Model
{
    public class MemberBank : Base
    {
        public int MemberBankId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string BankCode { get; set; }
        public string Address { get; set; }
        public string Contact { get; set; }
        public virtual List<MemberBankCardBinNo> MemberBankCardBinNos { get; set; }
    }
}
