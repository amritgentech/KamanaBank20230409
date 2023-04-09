using Db.Enum;
using Db.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ReconParser.App_Code.Recon.Sct
{
    public class SCT : Base
    {
        public List<TransactionBlock> TransactionBlocks;
        public SCT(String FileName, int FileCount)
            : base(FileName, FileCount)
        {
            TransactionBlocks = new List<TransactionBlock>();
            _Source = Source.FindName("SCT");
        }

        public override void Parse()
        {
            Console.WriteLine("Parsing File {0}", FileName);
            List<string> listString = ReadDataFromFile(FileName);

            TransactionBlockSeperator(listString);

            GetTransactionList();
            Console.WriteLine("Parsed File {0}", FileName);
        }

        public void TransactionBlockSeperator(List<string> listString)
        {
            TransactionBlock _TransactionBlock = new TransactionBlock();

            List<string> listStr = new List<string>();
            string strLine = "";
            int counter = 0;

            foreach (string str in listString)
            {
                if (str.Contains("R|LORO|POS|") || str.Contains("O|LORO|POS|") || str.Contains("R|LORO|ATM|") || str.Contains("O|LORO|ATM|") ||
                    str.Contains("R|ONUS|POS|") || str.Contains("O|ONUS|POS|") || str.Contains("R|ONUS|ATM|") || str.Contains("O|ONUS|ATM|"))
                {
                    strLine = "";
                    counter = 1;
                }

                if (!str.StartsWith("</PRE></BODY>"))
                {
                    if (str.IndexOf("<BODY><PRE>") >= 0)
                    {
                        string[] strArry = Regex.Split(str, "<PRE>");
                        if (strArry.Length > 1)
                        {
                            strLine += strArry[1];
                        }
                    }
                    else if (str.IndexOf("=") >= 0)
                    {
                        strLine += str;
                    }
                    else
                    {
                        strLine += str;
                    }
                }
                else
                {
                    counter = 0;
                }

                if ((!(str.IndexOf("=") >= 0)) && counter == 1)
                {
                    listStr.Add(strLine);
                    strLine = "";
                }
            }

            foreach (var str in listStr)
            {
                _TransactionBlock = new TransactionBlock();
                _TransactionBlock.TransactionBlockList.AddRange(str.Split('|'));
                TransactionBlocks.Add(_TransactionBlock);
            }
        }

        public void GetTransactionList()
        {
            foreach (TransactionBlock _TransactionBlock in TransactionBlocks)
            {
                Transaction _Transaction = GetTransaction(_TransactionBlock);
                if (_Transaction != null)
                {
                    Transactions.Add(_Transaction);
                }
            }
        }

        public Transaction GetTransaction(TransactionBlock _TransactionBlock)
        {
            TransactionType _TransactionType = TransactionType.BalInquiry;
            TransactionStatus _TransactionStatus = TransactionStatus.Fail;
            CardType _CardType = CardType.NotDefine;
            NetworkType _NetworkType = NetworkType.SCT;
            TerminalType _TerminalType = TerminalType.ATM;
            TerminalOwner _TerminalOwner = TerminalOwner.NotDefine;

            Transaction _Transaction = new Transaction();
            for (int i = 0; i < _TransactionBlock.TransactionBlockList.Count; i++)
            {
                string str = _TransactionBlock.TransactionBlockList[i];
                try
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        if (i == 0)
                        {
                            if (str.Equals("R"))
                            {
                                _TransactionStatus = TransactionStatus.Reversal;
                            }
                        }
                        else if (i == 1)
                        {

                            if (str.Equals("LORO"))
                            {
                                _CardType = CardType.OffUsCard;
                            }
                            else if (str.Equals("ONUS"))
                            {
                                _CardType = CardType.OwnCard;
                            }
                            else
                            {
                                _CardType = CardType.ForeignCard;
                            }
                        }
                        else if (i == 2)
                        {
                            _TerminalType = TerminalType.POS;
                            if (str.Equals("ATM"))
                                _TerminalType = TerminalType.ATM;
                        }
                        else if (i == 3)
                        {
                            if (str.Substring(str.Length - 3, 3).Equals("000"))
                            {
                                _Transaction.CardNo = str.Substring(0, str.Length - 3);
                            }
                            else
                            {
                                _Transaction.CardNo = str;
                            }
                        }
                        else if (i == 4)
                        {
                            try
                            {
                                DateTime date;
                                if (DateTime.TryParseExact(str, "yyMMdd", CultureInfo.CurrentCulture, DateTimeStyles.None, out date))
                                    _Transaction.TransactionDate = date;
                            }
                            catch
                            {
                                throw;
                            }
                        }
                        else if (i == 5)
                        {
                            try
                            {
                                string strtime = str;
                                int strHr = Convert.ToInt32(strtime.Substring(0, 2));
                                if (strHr > 12)
                                {
                                    strHr = 12;
                                }
                                int strMin = Convert.ToInt32(strtime.Substring(2, 2));
                                int strSec = Convert.ToInt32(strtime.Substring(4, 2));
                                _Transaction.TransactionTime = new TimeSpan(strHr, strMin, strSec);
                            }
                            catch
                            {
                                throw;
                            }
                        }
                        else if (i == 6)
                        {
                            _Transaction.TransactionAmount = (Convert.ToDecimal(str) / 100);
                            _TransactionType = TransactionType.BalInquiry;
                            if (_Transaction.TransactionAmount > 0)
                            {
                                _TransactionType = TransactionType.Financial;
                            }
                        }
                        else if (i == 7)
                        {

                        }
                        else if (i == 8)
                        {
                            _Transaction.ReferenceNo = str.TrimStart('0');
                        }
                        else if (i == 9)
                        {
                            if (str.Equals("777777"))
                            {
                                _TerminalOwner = TerminalOwner.INRTerminal;
                            }
                        }
                        else if (i == 10)
                        {
                            _Transaction.ResponseCode = GetResponseCode(str);
                            _TransactionStatus = TransactionStatus.Success;
                            if (!_Transaction.ResponseCode.Equals("0000"))
                            {
                                _TransactionStatus = TransactionStatus.Fail;
                            }
                        }
                        else if (i == 11)
                        {
                            _Transaction.TraceNo = str;
                        }
                        else if (i == 12)
                        {
                            _Transaction.TerminalId = str;
                        }
                    }
                }
                catch
                {
                    throw;
                }
            }

            _Transaction = GetTransaction(_Transaction, _TransactionType, _TransactionStatus, _TerminalType, _CardType, _TerminalOwner, _NetworkType);

            return _Transaction;
        }

        public List<string> ReadDataFromFile(string fileName)
        {
            try
            {
                List<string> listString = new List<string>();
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    StreamReader sr = new StreamReader(fs);
                    while (true)
                    {
                        string line = sr.ReadLine();
                        if (line == null)
                            break;
                        listString.Add(line);
                    }
                    sr.Close();
                }

                return listString;
            }
            catch
            {
                return new List<string>();
            }
        }
    }
}
