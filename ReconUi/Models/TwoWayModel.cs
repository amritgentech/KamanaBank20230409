using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace ReconUi.Models
{
    public class TwoWayModel
    {
        public string TransactionDate { get; set; }
        public string TerminalId { get; set; }
        public string CardNo { get; set; }
        public string AccountNo { get; set; }
        public string TraceNo { get; set; }
        public string AuthCode { get; set; }
        public string TerminalType { get; set; }
        public string Currency { get; set; }
        public double TransactionAmountSource { get; set; }
        public string ResponseCodeSource { get; set; }
        public string StatusSource { get; set; }
        public double TransactionAmountDestination { get; set; }
        public string ResponseCodeDestination { get; set; }
        public string FT_Branch { get; set; }
        //        public string RemarksDestination { get; set; }
    }
}