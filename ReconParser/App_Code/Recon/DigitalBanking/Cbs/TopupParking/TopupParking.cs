using Db.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReconParser.App_Code.Recon.DigitalBanking.Cbs.TopupParking
{
    public class TopupParking : CbsPayable
    {
        public TopupParking(String FileName, int FileCount)
            : base(FileName, FileCount)
        {
            _SubChildSource = SubChildSource.Find_By_Name("TopupParking");
        }
    }
}
