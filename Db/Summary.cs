using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Db
{
    public class Summary
    {
        public int Transactions { get; set; }
        public double Amount { get; set; }
        public int Invalid { get; set; }
        public int UnMatched { get; set; }
        public int Matched { get; set; }
        public int DateDiff { get; set; }
        public int Exception { get; set; }
        public int Suspected { get; set; }
        public int CbsInvalid { get; set; }
        public double Invalid_Amount { get; set; }
        public double UnMatched_Amount { get; set; }
        public double Matched_Amount { get; set; }
        public double DateDiff_Amount { get; set; }
        public double Exception_Amount { get; set; }
        public double Suspected_Amount { get; set; }
        public double CbsInvalid_Amount { get; set; }
        public int Cbsonly { get; set; }
        public int EsewaOnly{ get; set; }
        public int TopupOnly { get; set; }
        public int Missing { get; set; }
        public double EsewaOnly_Amount { get; set; }
        public double TopupOnly_Amount { get; set; }
        public double CbsOnly_Amount { get; set; }
        public double Missing_Amount { get; set; }
        public String UpdatedAt { get; set; }

        public String FormattedAmount
        {
            get { return string.Format("{0:N}", Amount); }
        }
    }
}