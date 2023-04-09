using Db;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Db
{
    public class Report
    {
        public Summary _Summary { get; set; }

        public Filter _Filter { get; set; }

        public DataTable Data { get; set; }

        public Report(Summary _Summary, Filter _Filter)
        {
            this._Summary = _Summary;
            this._Filter = _Filter;
        }

        public Report()
        {

        }

        public static Report GetReportFromFilter(Filter _Filter)
        {
            Report _Report = new Report();
            Dashboard _Dashboard = new Dashboard();
            Recon _Recon = new Recon();
            _Dashboard = _Recon.GetSummary(_Filter);
            //put summary as per the request like OnUs,Payable or receivable..
            _Report._Summary = _Dashboard.Summary;
            _Report._Filter = _Filter;

            return _Report;
        }
    }
}