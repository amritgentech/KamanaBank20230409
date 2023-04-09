using Db.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReconParser.App_Code.Recon.Ej.Diebold
{
    public class Diebold:Ejournal
    {
        
        public Diebold(String FileName, int FileCount) : base(FileName,FileCount) {
            _SubSource = SubSource.Find_By_Name("DIEBOLD");
            _SubChildSource = SubChildSource.Find_By_SubSourceId(_SubSource.SubSourceId);
        }

        protected string TruncateValue(string truncateValue, int startIndex, int noOfDigits)
        {
            string truncated = truncateValue.Substring(startIndex, noOfDigits);
            return truncated;
        }
    }
}
