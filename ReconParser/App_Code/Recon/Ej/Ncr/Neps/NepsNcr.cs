using Db.Enum;
using Db.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReconParser.App_Code.Recon.Ej.Ncr.Neps
{
    public class NepsNcr: Ncr
    {
        public NepsNcr(String FileName, int FileCount)
            : base(FileName,FileCount)
        {
            //TransactionBlocks = new List<TransactionBlock>();
        }

        public void TransactionBlockSeperator(List<string> listString)
        {
            TransactionBlock _TransactionBlock2 = new TransactionBlock();
            TransactionBlock _TransactionBlock = new TransactionBlock();
            int increament = 0;
            int endLine = 1;

            foreach (string str in listString)
            {
                int totalBlocks = TransactionBlocks.Count;
                int startIndex = str.IndexOf("-----------------------------------");
                if (str.Contains("PRESENTER ERROR"))
                {
                    _TransactionBlock2.TransactionBlockList.Add(str);
                    if (totalBlocks > 0)
                    {
                        TransactionBlocks[totalBlocks - 1].TransactionBlockList.Add("PRESENTER ERROR");
                    }

                }
                if ((str.IndexOf("-----------------------------------") >= 0 || str.IndexOf("****** CASH WITHDRAWAL ***********") >= 0
                    || str.IndexOf("****** BALANCE INQUIRY ***********") >= 0 || str.IndexOf("****** STMTREQ ***********") >= 0
                    || str.IndexOf("****** NEW PIN REENTRY ***********") >= 0 || str.IndexOf("****** FUNDS TRANSFER ***********") >= 0) && endLine == 1)
                {
                    _TransactionBlock = new TransactionBlock();
                    endLine = 0;
                    increament = 0;
                }

                _TransactionBlock.TransactionBlockList.Add(str);
                increament++;

                if ((str.IndexOf("-----------------------------------") >= 0 || str.IndexOf("***************************************") >= 0) && increament > 4 && endLine == 0)
                {
                    if (_TransactionBlock.TransactionBlockList.Count > 12)
                    {
                        TransactionBlocks.Add(_TransactionBlock);
                    }
                    _TransactionBlock = new TransactionBlock();
                    endLine = 1;
                    increament = 0;
                }
            }
        }

        public override void Parse()
        {
            Console.WriteLine("Parsing File {0}",FileName);
            List<string> listString = ReadDataFromFile(FileName);

            TransactionBlockSeperator(listString);

            GetTransactionList();
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
            string terminalIdFirstPart = "";

            //if (ConfigurationManager.AppSettings["TerminalId_StartingPart"] != null)
            //{
            //    terminalIdFirstPart = ConfigurationManager.AppSettings["TerminalId_StartingPart"].ToString();
            //}
            Transaction _Transaction = new Transaction();

            TransactionType _TransactionType = TransactionType.NotDefine;
            TransactionStatus _TransactionStatus = TransactionStatus.NotDefine; 

            DateTimeFormatInfo dateFormatInfo = new DateTimeFormatInfo();
            dateFormatInfo.ShortDatePattern = @"dd/MM/yyyy";

            List<string> ejData = new List<string>();
            List<string> ejExtractData = new List<string>();

            foreach (string str in _TransactionBlock.TransactionBlockList)
            {
                try
                {
                    if (str.Contains("****** PIN AUTH ***********") || str.Contains("POWER-UP/RESET") ||
                        str.Contains("APTRA ADVANCE NDC") || str.Contains("SUPERVISOR MODE ENTRY") || str.Contains("SUPERVISOR MODE EXIT"))
                    {
                        return null;
                    }

                    if (str.IndexOf("****** CASH WITHDRAWAL ***********") >= 0 || str.IndexOf("****** FUNDS TRANSFER ***********") >= 0)
                    {
                        _TransactionType = TransactionType.Financial;
                    }
                    else if (str.IndexOf("****** BALANCE INQUIRY ***********") >= 0)
                    {
                        _TransactionType = TransactionType.BalInquiry;
                    }
                    else if (str.IndexOf("****** STMTREQ ***********") >= 0)
                    {
                        _TransactionType = TransactionType.MiniStatement;
                    }
                    else if (str.IndexOf("****** NEW PIN REENTRY ***********") >= 0)
                    {
                        _TransactionType = TransactionType.PinChange;
                    }

                    string[] eliminateData = str.Split(' ');
                    ejData.Clear();
                    for (int f = 0; f < eliminateData.Length; f++)
                    {
                        if (!string.IsNullOrEmpty(eliminateData[f]))
                        {
                            ejData.Add(eliminateData[f]);
                        }
                    }

                    if (str.Contains("DATE:") && str.Contains("ATM_ID:"))
                    {

                        _Transaction.TransactionDate = Convert.ToDateTime(ejData[1], dateFormatInfo);
                        _Transaction.TransactionTime = TimeSpan.Parse(ejData[2], dateFormatInfo);

                        _Transaction.TerminalId = ejData[ejData.Count - 1];
                    }
                    else if (str.Contains("ATM ID"))
                    {
                        _Transaction.TerminalId = ejData[ejData.Count - 1];
                    }
                    else if (str.Contains("RRN"))
                    {
                        _Transaction.ReferenceNo = ejData[ejData.Count - 2];
                    }
                    else if (str.Contains("DATE & TIME"))
                    {
                        try
                        {
                            _Transaction.TransactionDate = Convert.ToDateTime(ejData[ejData.Count - 2], dateFormatInfo);
                            _Transaction.TransactionTime = TimeSpan.Parse(ejData[ejData.Count - 1], dateFormatInfo);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }

                    else if (str.Contains("TXN TYPE "))
                    {
                        string[] eliminateData1 = str.Split(':');
                        string ejDataStr = eliminateData1[eliminateData1.Length - 1];
                        if (ejDataStr.Trim().Equals(" BALANCE INQUIRY(SA) ".Trim()) || ejDataStr.Trim().Equals(" BALANCE INQUIRY".Trim()) ||
                                    ejDataStr.Trim().Equals("BALANCE-INQUIRY".Trim()) || ejDataStr.Trim().Equals("BALANCE-ENQUIRY(CA)".Trim()) ||
                                    ejDataStr.Trim().Equals("BALANCE-INQUIRY(SB)".Trim()) || ejDataStr.Trim().Equals("BALANCE INQUIRY".Trim()))
                        {
                            _TransactionType = TransactionType.BalInquiry;
                        }
                        else if (ejDataStr.Trim().Equals("CASH-WITHDRAWAL(SA)".Trim()) || ejDataStr.Trim().Equals(" FAST CASH(DR) ".Trim()) ||
                            ejDataStr.Trim().Equals(" FAST CASH ".Trim()) || ejDataStr.Trim().Equals("FAST-CASH".Trim()) ||
                            ejDataStr.Trim().Equals("CASH-WITHDRAWAL(CA)".Trim()) || ejDataStr.Trim().Equals("CASH-WITHDRAWAL".Trim()) ||
                            ejDataStr.Trim().Equals("CASH-BALANCE".Trim()) || ejDataStr.Trim().Equals("FAST CASH".Trim()) ||
                            ejDataStr.Trim().Equals("CASH WITHDRAWAL".Trim()))
                        {
                            _TransactionType = TransactionType.Financial;
                        }
                        else if (ejDataStr.Trim().Equals("PIN-CHANGE".Trim()) || ejDataStr.Trim().Equals("PIN - CHANGE(DR)".Trim()) ||
                            ejDataStr.Trim().Equals("PIN-CHANGE(CR)".Trim()))
                        {
                            _TransactionType = TransactionType.PinChange;
                        }
                        else
                        {
                            _TransactionType = TransactionType.NotDefine;
                        }
                    }
                    else if (str.Contains("CARD NO"))
                    {
                        int i = ejData.IndexOf(":");
                        if (i < 0)
                        {
                            i = ejData.IndexOf("NO:");
                        }

                        _Transaction.CardNo = ejData[i + 1];
                    }
                    else if (str.Contains("TRANS AMOUNT"))
                    {
                        _Transaction.TransactionAmount = Convert.ToDecimal(ejData[ejData.Count - 2]);
                    }
                    else if (str.Contains("COUNTERS"))
                    {
                        string[] lineEight = str.Split(':');
                        _Transaction.CashLeaves = GetCashLeavesCountDetail(lineEight[lineEight.Length - 1]);
                    }
                    else if (str.Contains("AUTH CODE"))
                    {
                        _Transaction.AuthCode = ejData[ejData.Count - 1];
                    }
                    else if (str.Contains("TRACE NO/ID"))
                    {
                        _Transaction.TraceNo = ejData[ejData.Count - 1];
                    }
                    else if (str.Contains("RESP CODE"))
                    {
                        _Transaction.ResponseCode = GetResponseCode(ejData[ejData.Count - 1]);
                        _TransactionStatus = TransactionStatus.Success;
                        if (!_Transaction.ResponseCode.Equals("0000"))
                        {
                            _TransactionStatus = TransactionStatus.Fail;
                        }
                    }
                    else if (str.Contains("PRESENTER ERROR"))
                    {
                        _TransactionStatus = TransactionStatus.Retracted;
                    }
                }
                catch
                {
                    throw;
                }
            }
            if (_Transaction != null)
                _Transaction = GetTransaction(_Transaction, _TransactionType, _TransactionStatus, TerminalType.ATM);

            return _Transaction;
        }
    }
}
