using Db.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Hepler
{
    public class CashLeafConfiguration : EntityTypeConfiguration<CashLeaf>
    {
        public CashLeafConfiguration(){
            this.Property(x => x.PhysicalCassettePosition).HasColumnType("int");
        }
    }
}
