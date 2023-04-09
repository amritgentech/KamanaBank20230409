using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Model
{
    public class NPNSettlement : Base
    {
        public int Id { get; set; }
        public string BinNo { get; set; }
        public string BankName { get; set; }
        public string TranDate { get; set; }
        public string TerminalId { get; set; }
        public string CardNo { get; set; }
        public string TraceNo { get; set; }
        public string RRN { get; set; }
        public double TranAmt { get; set; }
        public string AuthCode { get; set; }
        public string Loro { get; set; }

    }
}
