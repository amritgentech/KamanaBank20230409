using Db.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Model
{
    public class MemberBankCardBinNo : Base
    {
        public int MemberBankCardBinNoId { get; set; }
        [Required]
        public string BinNo { get; set; }
        [Required]
        public NetworkType NetworkType { set; get; }
        [Required]
        public int MemberBankId { get; set; }
        public virtual MemberBank MemberBank { get; set; }

        public static List<MemberBankCardBinNo> All()
        {
            List<MemberBankCardBinNo> MemberBankCardBinNos = null;
            using (var context = new ReconContext())
            {
                MemberBankCardBinNos = context.MemberBankCardBinNos.ToList();
            }
            return MemberBankCardBinNos;
        }
    }
}
