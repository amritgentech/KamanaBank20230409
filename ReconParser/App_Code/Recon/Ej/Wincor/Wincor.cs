using Db.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReconParser.App_Code.Recon.Ej.Wincor
{
    public class Wincor : Ejournal
    {
        public Wincor(String FileName, int FileCount) : base(FileName,FileCount)
        {
            _SubSource = SubSource.Find_By_Name("WINCOR");
            _SubChildSource = SubChildSource.Find_By_SubSourceId(_SubSource.SubSourceId);
        }

        public void TransactionBlockSeperator(List<string> listString)
        {
            try
            {
                TransactionBlock _TransactionBlock = new TransactionBlock();
                foreach (string str in listString)
                {
                    if (str.Contains("112711"))
                    {

                    }

                    if (str.IndexOf("TRANSACTION START") >= 0)
                    {
                        _TransactionBlock = new TransactionBlock();
                    }

                    _TransactionBlock.TransactionBlockList.Add(str);
                    TransactionBlocks.Add(_TransactionBlock); // This line is changes by amrit

                    if (str.IndexOf("TRANSACTION END") >= 0)
                    {
                        if (_TransactionBlock.TransactionBlockList.Count > 10)
                        {
                            TransactionBlocks.Add(_TransactionBlock);
                        }
                        _TransactionBlock = new TransactionBlock();
                    }
                }
            }
            catch (Exception e)
            {

                throw;
            }
            
        }
    }
}
