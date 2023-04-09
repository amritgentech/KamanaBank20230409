using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;

namespace Db
{
    public class TwoWayMbViewModel
    {
        public string TransactionDate { get; set; }
        public string UniqueId  { get; set; }
        public double? Amount { get; set; }
        public string EsewaSettlement { get; set; }
        public string Status  { get; set; }
        
    }
}