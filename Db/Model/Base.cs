using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Model
{
    public class Base
    {
        public Base()
        {
            this.CreatedAt = DateTime.Now;
            this.UpdatedAt = DateTime.Now;
        }
        [System.ComponentModel.DefaultValue(typeof(DateTime), "")]
        public DateTime CreatedAt { get; set; }
        [System.ComponentModel.DefaultValue(typeof(DateTime), "")]
        public DateTime UpdatedAt { get; set; }
    }
}
