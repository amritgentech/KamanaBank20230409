using Db.Enum;
using Db.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ReconParser.App_Code.Recon.Ej.Diebold.Kumari
{
    public class KumariYcsDiebold : Diebold
    {
        public KumariYcsDiebold(String FileName, int FileCount)
            : base(FileName,FileCount)
        {
        }

        public void TransactionBlockSeperator(List<string> listString)
        {
            TransactionBlock _TransactionBlock = new TransactionBlock();
            foreach (string str in listString)
            {
                if (str.IndexOf("LUNO123") >= 0)
                {
                    if (str.Substring(0, 37).Contains("/"))
                    {
                        if (str.Substring(0, 37).Contains("("))
                        {
                        }
                        else
                        {
                            _TransactionBlock = new TransactionBlock();
                            string[] strArray = str.Split('a');

                            _TransactionBlock.TransactionBlockList.AddRange(strArray);
                            TransactionBlocks.Add(_TransactionBlock);
                        }
                    }
                }
            }
        }

        public override void Parse()
        {
            Console.WriteLine("Parsing file {0} ", FileName);

            List<string> listString = ReadDataFromFile(FileName);
            try
            {
                TransactionBlockSeperator(listString);

                GetTransactionList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error->NepsDiebold->Parse->{0}", ex.Message);
            }
            Console.WriteLine("Parsed File {0}", FileName);
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
            float amount = 0;

            if (_TransactionBlock.TransactionBlockList.Count > 0)
            {
                try
                {
                    string[] firstLine = Regex.Split(_TransactionBlock.TransactionBlockList[0], "e1");
                    string[] dateTime = firstLine[0].Split(' ');
                    _Transaction.TransactionDate = Convert.ToDateTime(dateTime[0], dateFormatInfo); //firstLine[0];

                    _Transaction.TransactionTime = TimeSpan.Parse(dateTime[1]);
                    _Transaction.TerminalId = firstLine[1];
                }
                catch
                {
                    throw;
                }
            }

            if (_TransactionBlock.TransactionBlockList.Count > 1)
            {
                string[] secondLine = Regex.Split(_TransactionBlock.TransactionBlockList[1], "e1");
                if (_TransactionBlock.TransactionBlockList[1].IndexOf("REF.NO:") >= 0 || _TransactionBlock.TransactionBlockList[1].IndexOf("REF. NO:") >= 0)
                {
                    _Transaction.ReferenceNo = secondLine[1];
                }
                else if (_TransactionBlock.TransactionBlockList[1].IndexOf("CRD NO:") >= 0)
                {
                    _Transaction.CardNo = secondLine[1];
                    if (_Transaction.CardNo.IndexOf("999999") >= 0)
                    {
                        return null;
                    }
                }
            }

            if (_TransactionBlock.TransactionBlockList.Count > 2)
            {
                try
                {
                    string[] thirdLine = Regex.Split(_TransactionBlock.TransactionBlockList[2], "e1");
                    if (_TransactionBlock.TransactionBlockList[2].IndexOf("CRD NO:") >= 0)
                    {
                        _Transaction.CardNo = thirdLine[1];
                    }
                    else if (_TransactionBlock.TransactionBlockList[2].IndexOf("CARD NO:") >= 0)
                    {
                        _Transaction.CardNo = thirdLine[1];
                    }
                    else if (_TransactionBlock.TransactionBlockList[2].IndexOf("CRD:") >= 0)
                    {
                        _Transaction.CardNo = thirdLine[1];
                    }

                    if (_Transaction.CardNo != null)
                    {
                        if (_Transaction.CardNo.IndexOf("999999") >= 0)
                        {
                            return null;
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }

            if (_TransactionBlock.TransactionBlockList.Count > 3)
            {
                try
                {
                    string[] fourthLine = Regex.Split(_TransactionBlock.TransactionBlockList[3], "e1");
                    if (_TransactionBlock.TransactionBlockList[3].IndexOf("ACC:") >= 0)
                    {
                        if (!string.IsNullOrEmpty(fourthLine[1].Trim()))
                        {
                            _Transaction.AccountNo = fourthLine[1];
                        }
                    }
                    else if (_TransactionBlock.TransactionBlockList[3].IndexOf("TXN:") >= 0)
                    {
                        if (fourthLine[1].Trim().Equals("CASH WITHDRAWAL".Trim()) ||
                            fourthLine[1].Trim().Equals("FAST CASH".Trim()) ||
                            fourthLine[1].Trim().Equals("ATM MOBILE".Trim())
                            )
                        {
                            _TransactionType = TransactionType.Financial;
                        }
                        else if (fourthLine[1].Trim().Equals(" LOAD CASH ".Trim()))
                        {
                            _TransactionType = TransactionType.Other;
                        }
                        else if (fourthLine[1].Trim().Equals(" BALANCE INQUIRY ".Trim()) ||
                                 fourthLine[1].Trim().Equals(" CASH BALANCE ".Trim()))
                        {
                            _TransactionType = TransactionType.BalInquiry;
                            _Transaction.TransactionAmount = Convert.ToDecimal(amount + ".00");
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }

            if (_TransactionBlock.TransactionBlockList.Count > 4)
            {
                try
                {
                    if (_TransactionBlock.TransactionBlockList[4].IndexOf("TXN:") >= 0)
                    {
                        string[] fifthLine = Regex.Split(_TransactionBlock.TransactionBlockList[4], "e1");
                        if (fifthLine[0].Contains("WDL"))
                        {
                            string[] fifthLine1 = fifthLine[0].Split(':');
                            if (fifthLine1[1].Trim().Equals("WDL"))
                            {
                                _TransactionType = TransactionType.Financial;
                            }
                        }
                        else
                        {
                            if (fifthLine[1].Trim().Equals("CASH WITHDRAWAL".Trim()) ||
                                fifthLine[1].Trim().Equals("FAST CASH".Trim()) ||
                                fifthLine[1].Trim().Equals("ATM MOBILE".Trim()))
                            {
                                _TransactionType = TransactionType.Financial;
                            }
                            else if (fifthLine[1].Trim().Equals(" LOAD CASH ".Trim()))
                            {
                                _TransactionType = TransactionType.Other;
                            }
                            else if (fifthLine[1].Trim().Equals(" BALANCE INQUIRY ".Trim()) ||
                                     fifthLine[1].Trim().Equals(" CASH BALANCE ".Trim()))
                            {
                                _TransactionType = TransactionType.BalInquiry;
                                _Transaction.TransactionAmount = Convert.ToDecimal(amount + ".00");
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            if (_TransactionBlock.TransactionBlockList.Count > 5)
            {
                try
                {
                    string[] sixthLine = Regex.Split(_TransactionBlock.TransactionBlockList[5], "e1");
                    if (_TransactionBlock.TransactionBlockList[5].IndexOf("DISP:") >= 0)
                    {
                        _Transaction.TransactionAmount = Convert.ToDecimal(sixthLine[1]);
                    }
                    if (sixthLine[0].Contains("RE-DISP NPR"))
                    {
                        _Transaction.TransactionAmount = Convert.ToDecimal(sixthLine[1]);
                    }
                    if (_TransactionBlock.TransactionBlockList[5].StartsWith("RESP:"))
                    {
                        _Transaction.ResponseCode = GetResponseCode(sixthLine[1]);

                        _TransactionStatus = TransactionStatus.Success;
                        if (!_Transaction.ResponseCode.Equals("0000"))
                        {
                            _TransactionStatus = TransactionStatus.Fail;
                        }
                    }
                }
                catch
                {
                    throw;
                }
            }
            if (_TransactionBlock.TransactionBlockList.Count > 6)
            {

                if (_TransactionBlock.TransactionBlockList[6].StartsWith("RESP:"))
                {
                    try
                    {
                        string[] seventhLine = Regex.Split(_TransactionBlock.TransactionBlockList[6], "e1");
                        _Transaction.ResponseCode = GetResponseCode(seventhLine[1]);

                        _TransactionStatus = TransactionStatus.Success;
                        if (!_Transaction.ResponseCode.Equals("0000"))
                        {
                            _TransactionStatus = TransactionStatus.Fail;
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                else if (_TransactionBlock.TransactionBlockList[6].StartsWith("DESC:") ||
                         _TransactionBlock.TransactionBlockList[6].IndexOf("REQ SERVICED") >= 0)
                {

                }
                else
                {
                    try
                    {
                        
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            if (_TransactionBlock.TransactionBlockList.Count > 7)
            {
                try
                {
                    if (_TransactionBlock.TransactionBlockList[7].StartsWith("RESP:"))
                    {
                        string[] eighthLine = Regex.Split(_TransactionBlock.TransactionBlockList[7], "e1");
                        _Transaction.ResponseCode = GetResponseCode(eighthLine[1]);

                        _TransactionStatus = TransactionStatus.Success;
                        if (!_Transaction.ResponseCode.Equals("0000"))
                        {
                            _TransactionStatus = TransactionStatus.Fail;
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }

            _Transaction = GetTransaction(_Transaction, _TransactionType, _TransactionStatus, TerminalType.ATM);
                
            return _Transaction;

        }
    }
}
