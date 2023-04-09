using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Db
{
    public class Dashboard
    {
        public Summary Summary { get; set; }
        public Summary OnUsSummary { get; set; }
        public Summary RecievableSummary { get; set; }
        public Summary PayableSummary { get; set; }
        public Filter Filter { get; set; }

        public static Dashboard DashboardFromFilter(Filter _Filter)
        {
            Recon _Recon = new Recon();
            return _Recon.GetSummary(_Filter);
        }

        public Dashboard(Summary OnUsSummary, Summary RecievableSummary, Summary PayableSummary, Filter _Filter)
        {
            this.OnUsSummary = OnUsSummary;
            this.RecievableSummary = RecievableSummary;
            this.PayableSummary = PayableSummary;
            this.Filter = _Filter;
        }

        public Dashboard()
        {

        }
    }
}