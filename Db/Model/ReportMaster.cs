using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Model
{
    public class ReportMaster:Base
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Query { get; set; }
        public string DatabaseName { get; set; }
        public int CategorieId { get; set; }
        public int ReportTypeId { get; set; }
        [ForeignKey("CategorieId")]
        public virtual Category Categories { get; set; }
        [ForeignKey("ReportTypeId")]
        public virtual ReportType ReportTypes { get; set; }

    }
}
