using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Model.CBS
{
    public class Cbs
    {
        DateTime TranDate { get; set; }
        TimeSpan TimeStamp { get; set; }
        string TerminalID { get; set; }
        string CardNo { get; set; }
        decimal Amount { get; set; }
        string TraceNo { get; set; }
        string ReferenceNo { get; set; }
        string ResponseCode { get; set; }
        int TypeOfNT { get; set; }
        int TypeOfTerminal { get; set; }
        int DRCR { get; set; }
        string Currency { get; set; }
        string AuthCode { get; set; }
        int IsFinancial { get; set; }
    }
}
