using Db.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Model
{
    public class BankCardBinNo : Base
    {
        public int BankCardBinNoId { get; set; }
        [Required]
        public string BinNo { get; set; }
        public NetworkType NetworkType { set; get; }

        public static List<BankCardBinNo> All()
        {
            List<BankCardBinNo> BankCardBinNos = null;
            using (var context = new ReconContext())
            {
                BankCardBinNos = context.BankCardBinNos.ToList();
            }
            return BankCardBinNos;
        }
    }
}
