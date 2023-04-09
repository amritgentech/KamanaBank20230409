using Db.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReconParser.App_Code.Recon.CBS
{
    public class Cbs : Base
    {
        public Cbs(String FileName,int FileCount)
            : base(FileName,FileCount)
        {
            _Source = Source.CBS();
        }
    }
}