using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Model
{
    public class ReportType:Base
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string DiagramType { get; set; }

    }
}
