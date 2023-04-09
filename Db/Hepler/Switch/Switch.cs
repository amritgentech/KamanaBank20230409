using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Model.Switch
{
    public class Switch
    {
        DateTime TranDate { get; set; }
        string TerminalID { get; set; }
        string CardNo { get; set; }
        decimal Amount { get; set; }
        string TraceNo { get; set; }
        string ResponseCode { get; set; }
        string ReferenceNo { get; set; }
        string AuthCode { get; set; }
        string ProcessCode { get; set; }
        string ReversalFlag { get; set; }
        string DeviceType { get; set; }
    }
}
