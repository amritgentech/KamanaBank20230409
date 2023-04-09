using Db.Enum;
using Db.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ReconParser.App_Code.Recon.Ej
{
    public class Ejournal:Base
    {
        public List<TransactionBlock> TransactionBlocks;
        public Ejournal(String FileName, int FileCount)
            : base(FileName,FileCount)
        {
           TransactionBlocks = new List<TransactionBlock>();
            _Source = Source.EJ();
        }

        public List<string> ReadDataFromFile(string fileName)
        {
            try
            {
                List<string> listString = new List<string>();
                //string sourceFile = System.IO.Path.Combine(fileName);
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    StreamReader sr = new StreamReader(fs);
                    while (true)
                    {
                        string line = sr.ReadLine();
                        if (line == null)
                        { break; }
                        line = RemoveMultipleSpace(line);
                        listString.Add(line);
                    }
                    sr.Close();
                }

                return listString;
            }
            catch (Exception ex)
            {
                Console.Write("Error->Ejournal->ReadDataFromFile->" + ex.Message);
                return new List<string>();
            }
        }

        public List<CashLeaf> GetCashLeavesCountDetail(string str)
        {
            List<CashLeaf> listTransactionNoteCountDetail = new List<CashLeaf>();
            string[] strArray = str.Split(',');
            for (int i = 0; i < strArray.Length; i++)
            {
                if (!string.IsNullOrEmpty(strArray[i]))
                {
                    CashLeaf _CashLeaf = new CashLeaf();
                    string[] splitFromColon = strArray[i].Split('-');
                    //_CashLeaf.Cash = GetCashById(Convert.ToInt32(splitFromColon[0]));
                    //string[] spliteFromComa = splitFromColon[1].Split(',');
                    _CashLeaf.PhysicalCassettePosition = (PhysicalCassettePosition)Convert.ToInt32(splitFromColon[0]);
                    _CashLeaf.TotalNoteCount = Convert.ToInt32(splitFromColon[1]);
                    listTransactionNoteCountDetail.Add(_CashLeaf);
                }
            }
            return listTransactionNoteCountDetail;
        }

        /// <summary>
        /// We know that this is EJ so we will compare it with other sources than EJ
        /// First we will check if todays respective transactions are saved in transaction table or not.
        /// We are making a must rule that CBS data should be present first.
        /// 
        /// My assumptions here is that recon.Reconcile method should create all necessary relations
        /// </summary>
        override
        public void ProcessRecon()
        {
            Console.WriteLine("Recon Start..............");
            if (Transactions.Count < 1)
            {
                Console.WriteLine("Recon Complete..............");
                return;
            }
//            EjVsCbs recon = new EjVsCbs(Transactions);
//            recon.Reconcile();
            Console.WriteLine("Recon Complete..............");
        }
        public string RemoveMultipleSpace(string param)
        {
            if (!string.IsNullOrEmpty(param))
            {
                param = Regex.Replace(param, "[ ]{2,}", " ", RegexOptions.None);
            }
            return param;
        }
    }
}
