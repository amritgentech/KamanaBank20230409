using Db.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReconUi.Model
{
    public class TerminalViewModel
    {
        public string TerminalId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string TerminalBrand { set; get; }
    }
}
