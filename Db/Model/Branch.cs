using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Model
{
    public class Branch : Base
    {
        [Key]
        public int BranchId { get; set; }
        public string Name { get; set; }
        public string BranchCode { get; set; }
        public string Address { get; set; }
        public string Contact { get; set; }
        public virtual List<Terminal> Terminals { get; set; }
    }
}
