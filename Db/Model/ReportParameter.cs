using Db.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Model
{
    public class ReportParameter:Base
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string DisplayName { get; set; }
        public string Query { get; set; }
        public ParameterDataType ParameterDataType { get; set; }
        public int ReportMasterId { get; set; }
        [ForeignKey("ReportMasterId")]
        public virtual ReportMaster ReportMasters { get; set; }
    }
}
