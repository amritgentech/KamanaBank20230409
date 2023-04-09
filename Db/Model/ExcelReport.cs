using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Model
{
    public class ExcelReport : Base
    {
        public int Id { get; set; }
        public String Sql { get; set; }
        public String ReportName { get; set; }
    }
}
