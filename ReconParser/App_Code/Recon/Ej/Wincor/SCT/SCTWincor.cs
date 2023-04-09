using Db.Enum;
using Db.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReconParser.App_Code.Recon.Ej.Wincor.SCT
{
    public class SCTWincor : Wincor
    {

        public SCTWincor(String FileName, int FileCount)
            : base(FileName,FileCount)
        {
            // TransactionBlocks = new List<TransactionBlock>();
        }

        public override void Parse()
        {
            Console.WriteLine("Parsing File {0}",FileName);
            List<string> listString = ReadDataFromFile(FileName);

            TransactionBlockSeperator(listString);

            GetTransactionList();
            Console.WriteLine("Parsed File {0}",FileName);
        }

        public void GetTransactionList()
        {
            foreach (TransactionBlock _TransactionBlock in TransactionBlocks)
            {
                Transaction _Transaction = GetTransaction(_TransactionBlock);
                if (_Transaction != null) {
                    Transactions.Add(_Transaction);
                }
            }
        }

        public Transaction GetTransaction(TransactionBlock _TransactionBlock)
        {
            string terminalIdFirstPart = "";

            //if (ConfigurationManager.AppSettings["TerminalId_StartingPart"] != null)
            //{
            //    terminalIdFirstPart = ConfigurationManager.AppSettings["TerminalId_StartingPart"].ToString();
            //}

            TransactionType _TransactionType = TransactionType.NotDefine;
            TransactionStatus _TransactionStatus = TransactionStatus.NotDefine; 

            int error = 0;
            Transaction _Transaction = null;
            int index = 0;
            string status = string.Empty;
            foreach (string str in _TransactionBlock.TransactionBlockList)
            {
                try
                {
                    if (str.IndexOf("TRANSACTION REQUEST") >= 0)
                    {
                        _Transaction = new Transaction();
                    }
                    else if (str.IndexOf("TRANSACTION REPLY NEXT") >= 0)
                    {
                        if (_Transaction == null)
                        {
                            _Transaction = new Transaction();
                        }
                    }
                    else if (str.IndexOf("----------") >= 0 || str.IndexOf("INTERACTIVE TRANSACTION REPLY") >= 0)
                    {
                    }
                    else if (str.IndexOf("CDM ERROR") >= 0)
                    {
                        error = 1;
                    }
                    else if (str.IndexOf("CASH REQUEST") >= 0)
                    {
                        index = 4;
                    }
                    else if (index == 4)
                    {
                        string[] lineTwelfth = str.Split(' ');
                        _Transaction.CashLeaves = GetCashLeavesCountDetail(lineTwelfth[lineTwelfth.Length - 1]);
                        index = 0;
                    }
                    else if (str.IndexOf("CASH PRESENTED") >= 0)
                    {
                        index = 1;
                    }
                    else if (str.IndexOf("FAILED") >= 0)
                    {
                        _Transaction = null;
                    }
                    else if (index == 1)
                    {
                        if (str.IndexOf("COMMUNICATION ERROR") >= 0)
                        {
                            return null;
                        }
                        DateTimeFormatInfo dateFormatInfo = new DateTimeFormatInfo();
                        dateFormatInfo.ShortDatePattern = @"yy/MM/dd";

                        string[] firstLine = str.Split(' ');
                        DateTime dt;
                        if (DateTime.TryParse(firstLine[1], dateFormatInfo, DateTimeStyles.None, out dt))
                        {
                            _Transaction.TransactionDate = Convert.ToDateTime(firstLine[0], dateFormatInfo);
                        }
                        else
                        {
                            _Transaction.TransactionDate = Convert.ToDateTime(firstLine[0], dateFormatInfo);
                        }
                        if (firstLine[2].Contains("TRANSACTION"))
                        {
                            _Transaction = null;
                            return _Transaction;
                        }
                        else
                        {
                            _Transaction.TransactionTime = TimeSpan.Parse(firstLine[1]);
                        }

                        _Transaction.TerminalId = terminalIdFirstPart + firstLine[2];
                        _Transaction.TraceNo = firstLine[3];
                        _Transaction.ProcessingCode = firstLine[4];

                        if (_Transaction.ProcessingCode.Equals("301000") || _Transaction.ProcessingCode.Equals("302000"))
                        {
                            _TransactionType = TransactionType.BalInquiry;
                        }
                        else if (_Transaction.ProcessingCode.Equals("010000") || _Transaction.ProcessingCode.Equals("011000") ||
                                 _Transaction.ProcessingCode.Equals("012000") || _Transaction.ProcessingCode.Equals("013000"))
                        {
                            _TransactionType = TransactionType.Financial;
                        }
                        else if (_Transaction.ProcessingCode.Equals("940100") || _Transaction.ProcessingCode.Equals("940101") ||
                               _Transaction.ProcessingCode.Equals("940102"))
                        {
                            _Transaction = null;
                            return _Transaction;
                        }

                        index = 2;
                    }
                    else if (index == 2)
                    {
                        string[] ninethLine = str.Split(' ');
                        if (ninethLine[0].StartsWith("000"))
                        {
                            _Transaction.CardNo = ninethLine[0].Substring(3, ninethLine[0].Length - 3);
                        }
                        else
                        {
                            _Transaction.CardNo = ninethLine[0];
                        }
                        _Transaction.TransactionAmount = Convert.ToDecimal(getNumber(ninethLine, ninethLine.Length - 2));
                        index = 3;
                    }
                    else if (index == 3)
                    {
                        if (str.IndexOf("SURCHARGE:") >= 0)
                        {
                            index = 3;
                        }
                        else
                        {
                            string[] seventhLine = str.Split(' ');
                            _Transaction.TraceNo = seventhLine[0];
                            _Transaction.ResponseCode = GetResponseCode(seventhLine[seventhLine.Length - 1]);
                            index = 0;

                            _TransactionStatus = TransactionStatus.Success;
                            if (!_Transaction.ResponseCode.Equals("0000"))
                            {
                                _TransactionStatus = TransactionStatus.Fail;
                            }
                        }
                    }
                    else if (str.IndexOf("CASH RETRACTS") >= 0 || str.IndexOf("CASH RETRACTED") >= 0)
                    {
                        _TransactionStatus = TransactionStatus.Retracted;
                    }
                }
                catch
                {
                    throw;
                }
            }

            if (error == 1)
            {
                _TransactionStatus = TransactionStatus.Fail;
                _Transaction.ResponseCode = "0CDM";
            }

            _Transaction = GetTransaction(_Transaction, _TransactionType, _TransactionStatus, TerminalType.ATM);

            return _Transaction;
        }

        public List<CashLeaf> GetCashLeavesCountDetail(string str) {
            List<CashLeaf> listTransactionNoteCountDetail = new List<CashLeaf>();
            string[] strArray = str.Split(';');
            for (int i = 0; i < strArray.Length; i++)
            {
                if (!string.IsNullOrEmpty(strArray[i]))
                {
                    CashLeaf _CashLeaf = new CashLeaf();
                    string[] splitFromColon = strArray[i].Split(':');
                    _CashLeaf.Cash = Cash.Find(Convert.ToInt32(splitFromColon[0]));
                    string[] spliteFromComa = splitFromColon[1].Split(',');
                    _CashLeaf.PhysicalCassettePosition = (PhysicalCassettePosition)Convert.ToInt32(spliteFromComa[0]);
                    _CashLeaf.TotalNoteCount = Convert.ToInt32(spliteFromComa[1]);
                    listTransactionNoteCountDetail.Add(_CashLeaf);
                }
            }
            return listTransactionNoteCountDetail;
        }

    }
}
