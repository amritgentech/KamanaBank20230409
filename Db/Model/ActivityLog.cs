using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Model
{
    public class ActivityLog : Base
    {
        public int ActivityLogId { get; set; }
        public string TableName { get; set; }
        public string LogData { get; set; }
        public string LogDescription { get; set; }
        public string ActivityUser { get; set; }
        public string ActivityUserId { get; set; }
    }
}
