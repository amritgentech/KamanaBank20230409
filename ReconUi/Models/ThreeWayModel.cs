using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace ReconUi.Models
{
    public class ThreeWayModel
    {
        public DateTime TransactionDate { get; set; }
        public string TerminalId { get; set; }
        public string CardNo { get; set; }
        public string TraceNo { get; set; }
        public string StatusSource { get; set; }
        public string TransactionAmountSource { get; set; }
        public string ResponseCodeSource { get; set; }
        public string TransactionAmountDestination1 { get; set; }
        public string ResponseCodeDestination1 { get; set; }
        public string TransactionAmountDestination2 { get; set; }
        public string ResponseCodeDestination2 { get; set; }
    }
}