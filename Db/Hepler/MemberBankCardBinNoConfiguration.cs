using Db.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Hepler
{
    public class MemberBankCardBinNoConfiguration: EntityTypeConfiguration<MemberBankCardBinNo>
    {
        public MemberBankCardBinNoConfiguration()
        {
            this.Property(x => x.NetworkType).HasColumnType("int");
        }
    }
}
