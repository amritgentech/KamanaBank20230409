using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Model
{
    public class Bank : Base
    {
        public int BankId { get; set; }
        public string Name { get; set; }
        public string BankCode { get; set; }
        public string Address { get; set; }
        public string Contact { get; set; }
        public virtual List<BankCardBinNo> BankCardBinNos { get; set; }
        public virtual List<Branch> Branchs { get; set; }
    }
}
