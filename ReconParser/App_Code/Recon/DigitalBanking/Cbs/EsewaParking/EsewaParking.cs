using Db.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReconParser.App_Code.Recon.DigitalBanking.Cbs.EsewaParking
{
    public class EsewaParking : CbsPayable
    {
        public EsewaParking(String FileName, int FileCount)
            : base(FileName, FileCount)
        {
            _SubChildSource = SubChildSource.Find_By_Name("EsewaParking");
        }
    }
}
