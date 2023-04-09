using Db.Enum;
using Db.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReconParser.App_Code.Recon.Ej.Diebold.Neps
{
    public class NepsDiebold:Diebold
    {
        public NepsDiebold(String FileName, int FileCount)
            : base(FileName,FileCount)
        {
        }

        public void TransactionBlockSeperator(List<string> listString)
        {
            int lineCount = 0;
            TransactionBlock _TransactionBlock = new TransactionBlock();
            foreach (string str in listString)
            {
                if (str.IndexOf("LUNO123") >= 0 || str.IndexOf("LUNO000") >= 0)
                {
                    if (str.Contains("a-----------------------------------a") || (str.Contains("ac-----------------------------------a") && str.Contains("ATM ID")) ||
                         (str.Contains("****** CASH WITHDRAWAL ***********")) || (str.Contains("****** STATEMENT REQUEST ***********")))
                    {
                        if (lineCount > 0)
                        {
                            TransactionBlocks.Add(_TransactionBlock);
                            lineCount = 0;
                            //atmRefillData = false;
                        }
                        _TransactionBlock = new TransactionBlock();
                        string[] strArray = str.Split('a');

                        _TransactionBlock.TransactionBlockList.AddRange(strArray);
                        if (_TransactionBlock.TransactionBlockList.Count >= 15)
                        {
                            lineCount = 1;
                            //listStrObj.Add(stringListObj);
                        }
                    }
                    else if (str.Contains("****** FEE PREAUTH ***********") || (str.Contains("****** PIN AUTH ***********")) ||
                        str.Contains("****** AVAILABLE BALANCE ***********") || (str.Contains("CARD ENTERED:")) || (str.Contains("BALANCE INQUIRY")))
                    {
                        if (lineCount > 0)
                        {
                            TransactionBlocks.Add(_TransactionBlock);
                            lineCount = 0;
                            //atmRefillData = false;
                        }
                    }
                    else if (lineCount >= 1 && lineCount < 4)
                    {
                        lineCount++;
                        _TransactionBlock.TransactionBlockList.Add(str);
                    }

                    if (lineCount == 4)
                    {
                        if (_TransactionBlock.TransactionBlockList.Count >= 15)
                        {
                            TransactionBlocks.Add(_TransactionBlock);
                        }
                        lineCount = 0;
                    }
                }
            }
        }

        public override void Parse()
        {
            Console.WriteLine("Parsing file {0} ",FileName);
           
            List<string> listString = ReadDataFromFile(FileName);
            try
            {
                TransactionBlockSeperator(listString);

                GetTransactionList();
            }
            catch (Exception ex) {
                Console.WriteLine("Error->NepsDiebold->Parse->{0}", ex.Message);
            }
            Console.WriteLine("Parsed File {0}",FileName);
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
            DateTimeFormatInfo dateFormatInfo = new DateTimeFormatInfo();
            dateFormatInfo.ShortDatePattern = @"dd/MM/yy";

            TransactionType _TransactionType = TransactionType.NotDefine;
            TransactionStatus _TransactionStatus = TransactionStatus.NotDefine; 

            Transaction _Transaction = new Transaction();
            foreach (string str in _TransactionBlock.TransactionBlockList)
            {
                try
                {
                    List<string> listStr = new List<string>();
                    string[] strArry = str.Split(' ');
                    foreach (var list in strArry)
                    {
                        if (!string.IsNullOrEmpty(list))
                        {
                            listStr.Add(list);
                        }
                    }

                    if (str.Contains("ATM ID") || str.Contains("ATM_ID"))
                    {
                        if (str.Contains("DATE"))
                        {
                            dateFormatInfo = new DateTimeFormatInfo();
                            dateFormatInfo.ShortDatePattern = @"dd/MM/yy";
                            _Transaction.TransactionDate = Convert.ToDateTime(listStr[1], dateFormatInfo);
                            _Transaction.TransactionTime = TimeSpan.Parse(listStr[2]);
                        }
                        _Transaction.TerminalId = listStr[listStr.Count - 1];
                    }
                    else if (str.Contains("CASH WITHDRAWAL") || str.Contains("FUNDS TRANSFER"))
                    {
                        _TransactionType = TransactionType.Financial;
                    }
                    else if (str.Contains("BALANCE INQUIRY"))
                    {
                        _TransactionType = TransactionType.BalInquiry;
                    }
                    else if (str.Contains("STMTREQ"))
                    {
                        _TransactionType = TransactionType.MiniStatement;
                    }
                    else if (str.Contains("NEW PIN REENTRY"))
                    {
                        _TransactionType = TransactionType.PinChange;
                    }
                    else if (str.Contains("RRN"))
                    {
                        _Transaction.ReferenceNo = listStr[listStr.Count - 2];
                    }
                    else if (str.Contains("DATE & TIME") || str.Contains("DATE"))
                    {
                        _Transaction.TransactionDate = Convert.ToDateTime(listStr[listStr.Count - 2], dateFormatInfo);
                        _Transaction.TransactionTime = TimeSpan.Parse(listStr[listStr.Count - 1]);
                    }
                    else if (str.Contains("TXN TYPE"))
                    {
                        string[] strArry1 = str.Split(':');
                        string ejData = strArry1[strArry1.Length - 1];
                        if (ejData.Trim().Equals(" BALANCE INQUIRY(SA) ".Trim()) || ejData.Trim().Equals(" BALANCE INQUIRY".Trim()) ||
                            ejData.Trim().Equals("BALANCE INQUIRY".Trim()) || ejData.Trim().Equals("BALANCE-ENQUIRY(CA)".Trim()) ||
                            ejData.Trim().Equals("BALANCE-INQUIRY(SB)".Trim()))
                        {
                            _TransactionType = TransactionType.BalInquiry;
                        }
                        else if (ejData.Trim().Equals("CASH-WITHDRAWAL(SA)".Trim()) || ejData.Trim().Equals(" FAST CASH(DR) ".Trim()) ||
                            ejData.Trim().Equals(" FAST CASH ".Trim()) || ejData.Trim().Equals("FAST-CASH".Trim()) ||
                            ejData.Trim().Equals("CASH-WITHDRAWAL(CA)".Trim()) || ejData.Trim().Equals("CASH-WITHDRAWAL".Trim()) ||
                            ejData.Trim().Equals("CASH WITHDRAWAL".Trim()))
                        {
                            _TransactionType = TransactionType.Financial;
                        }
                        else if (ejData.Trim().Equals("PIN-CHANGE".Trim()) || ejData.Trim().Equals("PIN - CHANGE(DR)".Trim()) ||
                            ejData.Trim().Equals("PIN-CHANGE(CR)".Trim()))
                        {
                            _TransactionType = TransactionType.PinChange;
                        }
                        else if (ejData.Trim().Equals("LOAD-CASH".Trim()) || ejData.Trim().Equals("UNLOAD-CASH".Trim()))
                        {
                            _TransactionType = TransactionType.NotDefine;
                        }
                        else
                        {
                            _TransactionType = TransactionType.NotDefine;
                        }
                    }
                    else if (str.Contains("CARD NO"))
                    {
                        if (str.Contains("(") && str.Contains(")"))
                        {
                            _Transaction.CardNo = listStr[listStr.Count - 4];
                        }
                        else
                        {
                            _Transaction.CardNo = listStr[listStr.Count - 1];
                        }
                    }
                    else if (str.Contains("AUTH CODE"))
                    {
                        _Transaction.AuthCode = listStr[listStr.Count - 1];
                    }
                    else if (str.Contains("TRACE NO/ID"))
                    {
                        _Transaction.TraceNo = listStr[listStr.Count - 1];
                    }
                    else if (str.Contains("RESP CODE"))
                    {
                        if (str.Contains("LTXN NO       :"))
                        {
                        }
                        else
                        {
                            _Transaction.ResponseCode = GetResponseCode(listStr[listStr.Count - 1]);
                            _TransactionStatus = TransactionStatus.Success;
                            if (!_Transaction.ResponseCode.Equals("0000"))
                            {
                                _TransactionStatus = TransactionStatus.Fail;
                            }
                        }
                    }
                    else if (str.Contains("TRANS AMOUNT"))
                    {
                        _Transaction.TransactionAmount = Convert.ToDecimal(listStr[listStr.Count - 2]);
                    }
                    else if (str.Contains("COUNTERS"))
                    {
                        string[] lineEight = str.Split(':');
                        _Transaction.CashLeaves = GetCashLeavesCountDetail(lineEight[lineEight.Length - 1]);
                    }
                    else if (str.Contains("000DI01:3A:36:30") || str.Contains("SENSOR FAILURE"))
                    {
                        _TransactionStatus = TransactionStatus.Fail;
                    }
                }
                catch
                {
                    throw;
                }
            }
            _Transaction = GetTransaction(_Transaction, _TransactionType, _TransactionStatus, TerminalType.ATM);
            return _Transaction;

        }
    }
}
