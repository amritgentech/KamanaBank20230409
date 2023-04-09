using Db.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Hepler
{
    public class TerminalConfiguration : EntityTypeConfiguration<Terminal>
    {
        public TerminalConfiguration()
        {
            this.Property(x => x.TerminalType).HasColumnType("int");
        }
    }
}
