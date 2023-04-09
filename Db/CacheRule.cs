using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db
{
  public  class CacheRule
    {
        public bool IsFirstRequest { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string ReconType { get; set; }

        public void ResetCache()
        {
            //ResetCache 
            //changing any one value of Cache causes data to be fetched from database 
            this.FromDate = Convert.ToDateTime(this.FromDate).AddDays(1).ToShortDateString();
        }
    }
}
