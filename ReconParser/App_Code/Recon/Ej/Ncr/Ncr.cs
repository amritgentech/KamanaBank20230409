using Db.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReconParser.App_Code.Recon.Ej.Ncr
{
    public class Ncr : Ejournal
    {
        public Ncr(String FileName, int FileCount)
            : base(FileName,FileCount)
        {
            _SubSource = SubSource.Find_By_Name("NCR");
            _SubChildSource = SubChildSource.Find_By_SubSourceId(_SubSource.SubSourceId);
        }

        //public void TransactionBlockSeperator(List<string> listString)
        //{
        //    TransactionBlock _TransactionBlock = new TransactionBlock();
        //    foreach (string str in listString)
        //    {

        //        if (str.IndexOf("TRANSACTION START") >= 0 )
        //        {
        //            _TransactionBlock = new TransactionBlock();
        //        }

        //        _TransactionBlock.TransactionBlockList.Add(str);

        //        if (str.IndexOf("TRANSACTION END") >= 0)
        //        {
        //            if (_TransactionBlock.TransactionBlockList.Count > 10) {
        //                TransactionBlocks.Add(_TransactionBlock);
        //            }
        //            _TransactionBlock = new TransactionBlock();
        //        }
        //    }
        //}

        public string getNumber(string[] arrayStr)
        {
            string str = "";
            for (int i = 2; i < arrayStr[0].Length; i++)
            {
                str += arrayStr[i];
            }
            return str;
        }
    }
}
