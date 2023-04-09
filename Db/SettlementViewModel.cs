using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db
{
    public class SettlementViewModel
    {
        public Filter _Filter { get; set; }
        public SettlementModel Settlements { get; set; }
        public List<SettlementModel> ListSettlements { get; set; }

    }
}
