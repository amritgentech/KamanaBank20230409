using Db.Enum;
using Db.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReconParser.App_Code.Recon.Ej.Wincor.Neps
{
    public class NEPSWincor : Wincor
    {
        public NEPSWincor(String FileName, int FileCount)
            : base(FileName, FileCount)
        {
            // TransactionBlocks = new List<TransactionBlock>();
        }

        public override void Parse()
        {
            Console.WriteLine("Parsing File: {0}", FileName);
            List<string> listString = ReadDataFromFile(FileName);
            TransactionBlockSeperator(listString);
            GetTransactionList();
            Console.WriteLine("Parsed File: {0}", FileName);
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
            TransactionType _TransactionType = TransactionType.NotDefine;
            TransactionStatus _TransactionStatus = TransactionStatus.NotDefine;

            DateTimeFormatInfo dateFormatInfo = new DateTimeFormatInfo();
            dateFormatInfo.ShortDatePattern = @"dd/MM/yy";

            int error = 0;
            Transaction _Transaction = null;
            int index = 0;
            float amount = 0;
            int isPinAuth = 0;

            foreach (string str in _TransactionBlock.TransactionBlockList)
            {
                try
                {
                    if (str.IndexOf("TRANSACTION REQUEST") >= 0)
                    {
                        _Transaction = new Transaction();
                        index = 1;
                        isPinAuth = 0;
                    }
                    else if (str.IndexOf("INTERACTIVE TRANSACTION REPLY") >= 0)
                    {
                    }
                    else if (str.IndexOf("TRANSACTION REPLY NEXT") >= 0)
                    {
                        if (_Transaction == null)
                        {
                            _Transaction = new Transaction();
                            index = 1;
                            isPinAuth = 0;
                        }
                    }
                    else if (str.IndexOf("-----------------------------------") >= 0
                        || str.IndexOf("****** CASH WITHDRAWAL ***********") >= 0 || str.IndexOf("****** BALANCE INQUIRY ***********") >= 0
                        || str.IndexOf("****** FUNDS TRANSFER ***********") >= 0 || str.IndexOf("****** STMTREQ ***********") >= 0
                        || str.IndexOf("****** NEW PIN REENTRY ***********") >= 0)
                    {
                        if (index == 1)
                        {
                            index = 2;
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
                    }
                    else if ((str.IndexOf("LOGICAL CASSETTE 2 LOW") >= 0) || (str.IndexOf("PHYSICAL CASSETTE 2 LOW") >= 0)
                        || (str.IndexOf("LOGICAL CASSETTE 2 EMPTY") >= 0) || (str.IndexOf("PHYSICAL CASSETTE 2 EMPTY") >= 0)
                        || (str.IndexOf("LOGICAL CASSETTE 1 LOW") >= 0) || (str.IndexOf("PHYSICAL CASSETTE 1 LOW") >= 0)
                        || (str.IndexOf("LOGICAL CASSETTE 1 EMPTY") >= 0) || (str.IndexOf("PHYSICAL CASSETTE 1 EMPTY") >= 0)
                        || (str.IndexOf("LOGICAL CASSETTE") >= 0) || (str.IndexOf("PHYSICAL CASSETTE") >= 0) || (str.IndexOf("GO OUT OF SERVICE COMMAND") >= 0)
                        || (str.IndexOf("RECEIVED MESSAGE IN WRONG MODE") >= 0)
                        || (str.IndexOf("GO IN SERVICE COMMAND") >= 0) || (str.IndexOf("DEVICE CCCdmFW STATUS 2 SUPPLY 1") >= 0)
                        || (str.IndexOf("DEVICE LOG_CASS_1 STATUS 4 SUPPLY 3") >= 0) || (str.IndexOf("CASH TAKEN") >= 0)
                        || (str.IndexOf("CASH PRESENTED") >= 0) || (str.IndexOf("TAKEN") >= 0) || (str.IndexOf("OPERATOR DOOR CLOSED") >= 0))
                    {
                    }
                    else if ((str.IndexOf("****** PIN AUTH ***********") >= 0 || str.IndexOf("****** FEE PREAUTH ***********") >= 0) && index == 1)
                    {
                        isPinAuth = 1;
                    }
                    else if ((str.IndexOf("TRANSACTION END") >= 0 && index == 2))
                    {
                        return null;
                    }
                    else if ((str.IndexOf("TRANSACTION") >= 0 && str.IndexOf("FAILED") >= 0) || (str.IndexOf("INFORMATION") >= 0 && str.IndexOf("ENTERED") >= 0)
                        || (str.IndexOf("JRN/REC ERROR:") >= 0) || (str.IndexOf("IDCU ERROR:") >= 0) || (str.IndexOf("DEVICE CCRecPrtFW STATUS 4 SUPPLY 1") >= 0)
                        || (str.IndexOf("OPERATOR DOOR OPENED") >= 0) || (str.IndexOf("GO OUT OF SERVICE COMMAND") >= 0))
                    {
                        if (index > 0)
                        {
                            _Transaction = null;
                        }
                    }
                    else if (str.IndexOf("CDM ERROR") >= 0)
                    {
                        error = 1;
                    }
                    else if ((str.IndexOf("COMMUNICATION ERROR") >= 0) || (str.IndexOf("COMMUNICATION OFFLINE") >= 0) && index == 1)
                    {
                        return null;
                    }
                    else if (index == 2)
                    {
                        if ((str.IndexOf("COMMUNICATION ERROR") >= 0) || (str.IndexOf("COMMUNICATION OFFLINE") >= 0))
                        {
                            return null;
                        }
                        try
                        {
                            if (str.IndexOf("ATM ID        :") >= 0)
                            {
                                string[] firstLine = str.Split(':');
                                _Transaction.TerminalId = firstLine[firstLine.Length - 1].Trim();
                                index = 3;
                            }

                            if (str.IndexOf("DATE:") >= 0 && str.IndexOf("ATM_ID:") >= 0)
                            {
                                string[] firstLine = str.Split(' ');

                                DateTime dt;
                                if (DateTime.TryParse(firstLine[1], dateFormatInfo, DateTimeStyles.None, out dt))
                                {
                                    _Transaction.TransactionDate = Convert.ToDateTime(firstLine[1], dateFormatInfo);
                                }
                                _Transaction.TransactionTime = TimeSpan.Parse(firstLine[2]);

                                _Transaction.TerminalId = firstLine[4].Trim();
                                index = 3;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    else if (index == 3)
                    {
                        if (str.IndexOf("RRN") >= 0)
                        {
                            string[] secondLine = str.Split(':');
                            string[] secondLine2 = secondLine[secondLine.Length - 1].Split(' ');
                            _Transaction.ReferenceNo = secondLine2[secondLine2.Length - 2].Trim();
                            index = 4;
                        }
                    }
                    else if (index == 4)
                    {
                        if (str.IndexOf("TXN NO        :") >= 0)
                        {
                            string[] thirdLine = str.Split(':');
                            _Transaction.TransactionNo = thirdLine[thirdLine.Length - 1];
                            index = 5;
                        }

                        if (str.IndexOf("CARD NO:") >= 0)
                        {
                            string[] lineSix = str.Split(':');
                            string[] lineSix2 = lineSix[lineSix.Length - 1].Split(' ');
                            _Transaction.CardNo = lineSix2[1].Trim();
                            index = 8;
                        }
                    }
                    else if (index == 5)
                    {
                        if (str.IndexOf("DATE & TIME   :") >= 0)
                        {
                            try
                            {
                                string[] forthLine = str.Split(' ');

                                DateTime dt;
                                if (DateTime.TryParse(forthLine[forthLine.Length - 2], dateFormatInfo, DateTimeStyles.None, out dt))
                                {
                                    _Transaction.TransactionDate = Convert.ToDateTime(forthLine[forthLine.Length - 2], dateFormatInfo);
                                }
                                _Transaction.TransactionTime = TimeSpan.Parse(forthLine[forthLine.Length - 1]);
                            }
                            catch
                            {
                                throw;
                            }
                            index = 6;
                        }
                    }
                    else if (index == 6)
                    {
                        if (str.IndexOf("TXN TYPE      :") >= 0)
                        {
                            string[] fifthLine = str.Split(':');
                            if (fifthLine[1].Trim().Equals("BALANCE INQUIRY".Trim()))
                            {
                                _TransactionType = TransactionType.BalInquiry;
                                index = 9;
                            }
                            else if (fifthLine[1].Trim().Equals("MINI STATEMENT".Trim()) || fifthLine[1].Trim().Equals(" STATEMENT REQUEST".Trim()))
                            {
                                _TransactionType = TransactionType.MiniStatement;
                                index = 9;
                            }
                            else if (fifthLine[1].Trim().Equals("CASH WITHDRAWAL".Trim()) || fifthLine[1].Trim().Equals(" FAST CASH ".Trim()) || fifthLine[1].Trim().Equals("MOBILE PAYMENT"))
                            {
                                _TransactionType = TransactionType.Financial;
                                index = 7;
                            }
                            else if (fifthLine[1].Trim().Equals(" PIN CHANGE ".Trim()))
                            {
                                _TransactionType = TransactionType.PinChange;
                                index = 9;
                            }
                            index = 7;
                        }
                    }

                    else if (index == 7)
                    {

                        if (str.IndexOf("CARD NO       :") >= 0)
                        {
                            string[] lineSix = str.Split(':');
                            _Transaction.CardNo = lineSix[lineSix.Length - 1].Trim(); ;
                            index = 8;
                        }
                    }
                    else if (index == 8)
                    {
                        if (str.IndexOf("AUTH CODE") >= 0)
                        {
                            string[] lineEight = str.Split(':');
                            _Transaction.AuthCode = lineEight[lineEight.Length - 1].Trim();
                            index = 10;
                        }
                        if (str.IndexOf("TRANS AMOUNT  :") >= 0)
                        {
                            try
                            {
                                string[] lineSeven = str.Split(':');
                                string[] lineSeven1 = lineSeven[lineSeven.Length - 1].Split(' ');
                                _Transaction.TransactionAmount = Convert.ToDecimal(lineSeven1[1]);
                                index = 9;
                            }
                            catch
                            {
                                throw;
                            }
                        }
                    }
                    else if (index == 9)
                    {
                        if (str.IndexOf("COUNTERS:") >= 0)
                        {
                            string[] lineEight = str.Split(':');
                            _Transaction.CashLeaves = GetCashLeavesCountDetail(lineEight[lineEight.Length - 1]);
                        }
                        if (str.IndexOf("AUTH CODE     :") >= 0)
                        {
                            string[] lineEight = str.Split(':');
                            _Transaction.AuthCode = lineEight[lineEight.Length - 1].Trim();
                            index = 10;
                        }
                    }
                    else if (index == 10)
                    {
                        if (str.IndexOf("TRACE NO/ID    :") >= 0)
                        {
                            string[] lineNine = str.Split(':');
                            _Transaction.TraceNo = lineNine[lineNine.Length - 1].Trim();
                            index = 11;
                        }
                    }
                    else if (index == 11)
                    {
                        if (str.IndexOf("RESP CODE     :") >= 0)
                        {
                            string[] lineTen = str.Split(':');
                            _Transaction.ResponseCode = GetResponseCode(lineTen[lineTen.Length - 1].Trim());
                            _TransactionStatus = TransactionStatus.Success;
                            if (!_Transaction.ResponseCode.Equals("0000"))
                            {
                                _TransactionStatus = TransactionStatus.Fail;
                            }
                            index = 0;
                        }
                    }
                    else if (str.Contains("CASH RETRACTED"))
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

            if (isPinAuth == 0)
            {
                if (_Transaction != null)
                    _Transaction = GetTransaction(_Transaction, _TransactionType, _TransactionStatus, TerminalType.ATM);
                return _Transaction;
            }
            else
            {
                return null;
            }


        }
    }
}
