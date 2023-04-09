using Db.Enum;
using Db.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helper.GlobalHelpers;

namespace ReconParser.App_Code.Recon.Ej.Ncr.NPN
{
    public class NPNNcr : Ncr
    {
        public NPNNcr(String FileName, int FileCount)
            : base(FileName, FileCount)
        {
        }

        public void TransactionBlockSeperator(List<string> listString)
        {
            TransactionBlock _TransactionBlock = new TransactionBlock();
            foreach (string str in listString)
            {
                if (str.IndexOf("******JOURNAL RECORD RECEIPT******") >= 0)
                {
                    _TransactionBlock = new TransactionBlock();
                }
                _TransactionBlock.TransactionBlockList.Add(str);

                if (str.IndexOf("*********************************") >= 0 || str.IndexOf("******************************") >= 0)
                //if (str.IndexOf("DATE TIME TERMINAL") >= 0)
                {
                    TransactionBlocks.Add(_TransactionBlock);
                    _TransactionBlock = new TransactionBlock();
                }
            }
        }

        public override void Parse()
        {
            List<string> listString = ReadDataFromFile(FileName);

            TransactionBlockSeperator(listString);

            GetTransactionList();
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

        //public Transaction GetTransaction(TransactionBlock _TransactionBlock)
        //{
        //    string terminalIdFirstPart = "";

        //    //if (ConfigurationManager.AppSettings["TerminalId_StartingPart"] != null)
        //    //{
        //    //    terminalIdFirstPart = ConfigurationManager.AppSettings["TerminalId_StartingPart"].ToString();
        //    //}

        //    TransactionType _TransactionType = TransactionType.NotDefine;
        //    TransactionStatus _TransactionStatus = TransactionStatus.NotDefine;

        //    Transaction _Transaction = new Transaction();
        //    try
        //    {
        //        DateTimeFormatInfo dateFormatInfo = new DateTimeFormatInfo();
        //        dateFormatInfo.ShortDatePattern = @"dd/MM/yyyy";

        //        List<string> ejData = new List<string>();
        //        List<string> ejExtractData = new List<string>();


        //        if (_TransactionBlock.TransactionBlockList.Count >= 1)
        //        {
        //            List<string> finalData = new List<string>();
        //            int lineCount = 0;
        //            for (int e = 0; e < _TransactionBlock.TransactionBlockList.Count; e++)
        //            {
        //                DateTime dateTime;
        //                string[] eliminateData = _TransactionBlock.TransactionBlockList[e].Split(' ');
        //                ejData.Clear();
        //                for (int f = 0; f < eliminateData.Length; f++)
        //                {
        //                    if (!string.IsNullOrEmpty(eliminateData[f]))
        //                    {
        //                        ejData.Add(eliminateData[f]);
        //                    }
        //                }

        //                if (DateTime.TryParseExact(eliminateData[0], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
        //                {
        //                    ejExtractData.Add(_TransactionBlock.TransactionBlockList[e]);

        //                    _Transaction.TransactionDate = Convert.ToDateTime(ejData[0], dateFormatInfo);

        //                    //                            _Transaction.TransactionDate = Convert.ToDateTime(ejData[ejData.Count - 2], dateFormatInfo);
        //                    //                            _Transaction.TransactionTime = TimeSpan.Parse(ejData[ejData.Count - 1], dateFormatInfo);
        //                    if (!string.IsNullOrEmpty(ejData[1]))
        //                    {
        //                        try
        //                        {
        //                            dateFormatInfo.ShortTimePattern = @"HH:mm:ss";
        //                        }
        //                        catch
        //                        {
        //                            throw;
        //                        }
        //                    }
        //                    _Transaction.TransactionTime = TimeSpan.Parse(ejData[ejData.Count - 2], dateFormatInfo);
        //                    _Transaction.TerminalId = ejData[2];
        //                }
        //                else if (_TransactionBlock.TransactionBlockList[e].Contains("PRESENTER ERROR"))
        //                {
        //                    _TransactionStatus = TransactionStatus.Retracted;
        //                }
        //                else if (_TransactionBlock.TransactionBlockList[e].Contains("CARD NUMBER AUTH CODE"))
        //                {
        //                    lineCount = 1;
        //                }
        //                else if (lineCount == 1)
        //                {
        //                    _Transaction.CardNo = ejData[0];
        //                    if (_Transaction.CardNo.Contains("="))
        //                    {
        //                        string[] cardNoArry = _Transaction.CardNo.Split('=');
        //                        _Transaction.CardNo = cardNoArry[0];
        //                    }

        //                    if (ejData.Count > 1)
        //                    {
        //                        _Transaction.AuthCode = ejData[ejData.Count - 1];
        //                    }
        //                    lineCount = 2;
        //                }

        //                else if (_TransactionBlock.TransactionBlockList[e].Contains("TRANSACTION A/C"))
        //                {
        //                    lineCount = 3;
        //                }
        //                else if (lineCount == 3)
        //                {
        //                    if (ejData[0].Trim().Equals(" BALANCE INQUIRY(SA) ".Trim()) || ejData[0].Trim().Equals(" BALANCE INQUIRY".Trim()) ||
        //                        ejData[0].Trim().Equals("BALANCE-INQUIRY".Trim()) || ejData[0].Trim().Equals("BALANCE-ENQUIRY(CA)".Trim()) ||
        //                        ejData[0].Trim().Equals("BALANCE-INQUIRY(SB)".Trim()))
        //                    {
        //                        _TransactionType = TransactionType.BalInquiry;
        //                    }
        //                    else if (ejData[0].Trim().Equals("CASH-WITHDRAWAL(SA)".Trim()) || ejData[0].Trim().Equals(" FAST CASH(DR) ".Trim()) ||
        //                        ejData[0].Trim().Equals(" FAST CASH ".Trim()) || ejData[0].Trim().Equals("FAST-CASH".Trim()) ||
        //                        ejData[0].Trim().Equals("CASH-WITHDRAWAL(CA)".Trim()) || ejData[0].Trim().Equals("CASH-WITHDRAWAL".Trim()) ||
        //                        ejData[0].Trim().Equals("CASH-BALANCE".Trim()))
        //                    {
        //                        _TransactionType = TransactionType.Financial;
        //                    }
        //                    else if (ejData[0].Trim().Equals("PIN-CHANGE".Trim()) || ejData[0].Trim().Equals("PIN - CHANGE(DR)".Trim()) ||
        //                        ejData[0].Trim().Equals("PIN-CHANGE(CR)".Trim()))
        //                    {
        //                        _TransactionType = TransactionType.PinChange;
        //                    }

        //                    if (ejData.Count > 1)
        //                    {
        //                        _Transaction.AccountNo = ejData[ejData.Count - 1];
        //                    }

        //                    lineCount = 4;
        //                }

        //                else if (_TransactionBlock.TransactionBlockList[e].Contains("REFERENCE NO."))
        //                {
        //                    _Transaction.ReferenceNo = ejData[ejData.Count - 1];
        //                }
        //                else if (_TransactionBlock.TransactionBlockList[e].Contains("AMOUNT"))
        //                {
        //                    _Transaction.TransactionAmount = Convert.ToDecimal(ejData[ejData.Count - 1]);
        //                }
        //                else if (_TransactionBlock.TransactionBlockList[e].Contains("TRACE NO"))
        //                {
        //                    _Transaction.TraceNo = ejData[ejData.Count - 1];
        //                }
        //                else if (_TransactionBlock.TransactionBlockList[e].Contains("RESPCODE"))
        //                {
        //                    _Transaction.ResponseCode = GetResponseCode(ejData[ejData.Count - 1]);
        //                    _TransactionStatus = TransactionStatus.Success;
        //                    if (!_Transaction.ResponseCode.Equals("0000"))
        //                    {
        //                        _TransactionStatus = TransactionStatus.Fail;
        //                    }
        //                }



        //            }
        //        }
        //        if (_Transaction != null)
        //        {
        //            if (GlobalHelper.IsEmpty(_Transaction.TransactionDate)) //is due to sometime  transaction block not hitted..
        //            {
        //                return null;
        //            }
        //            _Transaction = GetTransaction(_Transaction, _TransactionType, _TransactionStatus, TerminalType.ATM);
        //        }

        //        return _Transaction;
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}
        public Transaction GetTransaction(TransactionBlock _TransactionBlock)
        {
            string terminalIdFirstPart = "";

            //if (ConfigurationManager.AppSettings["TerminalId_StartingPart"] != null)
            //{
            //    terminalIdFirstPart = ConfigurationManager.AppSettings["TerminalId_StartingPart"].ToString();
            //}

            TransactionType _TransactionType = TransactionType.NotDefine;
            TransactionStatus _TransactionStatus = TransactionStatus.NotDefine;

            Transaction _Transaction = new Transaction();
            try
            {
                DateTimeFormatInfo dateFormatInfo = new DateTimeFormatInfo();
                dateFormatInfo.ShortDatePattern = @"dd/MM/yyyy";

                List<string> ejData = new List<string>();
                List<string> ejExtractData = new List<string>();


                if (_TransactionBlock.TransactionBlockList.Count >= 1)
                {
                    List<string> finalData = new List<string>();
                    int lineCount = 0;
                    for (int e = 0; e < _TransactionBlock.TransactionBlockList.Count; e++)
                    {
                        DateTime dateTime;
                        string[] eliminateData = _TransactionBlock.TransactionBlockList[e].Split(' ');
                        ejData.Clear();
                        for (int f = 0; f < eliminateData.Length; f++)
                        {
                            if (!string.IsNullOrEmpty(eliminateData[f]))
                            {
                                ejData.Add(eliminateData[f]);
                            }
                        }

                        if (DateTime.TryParseExact(eliminateData[0], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime) ||
                            DateTime.TryParseExact(eliminateData[0], "dd/MM/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime)
                            )
                        {
                            ejExtractData.Add(_TransactionBlock.TransactionBlockList[e]);

                            _Transaction.TransactionDate = Convert.ToDateTime(ejData[0], dateFormatInfo);

                            //_Transaction.TransactionDate = Convert.ToDateTime(ejData[ejData.Count - 2], dateFormatInfo);
                            //_Transaction.TransactionTime = TimeSpan.Parse(ejData[ejData.Count - 1], dateFormatInfo);
                            if (!string.IsNullOrEmpty(ejData[1]))
                            {
                                try
                                {
                                    dateFormatInfo.ShortTimePattern = @"HH:mm:ss";
                                }
                                catch
                                {
                                    throw;
                                }
                            }
                            _Transaction.TransactionTime = TimeSpan.Parse(ejData[ejData.Count - 2], dateFormatInfo);
                            _Transaction.TerminalId = ejData[2];
                        }
                        else if (_TransactionBlock.TransactionBlockList[e].Contains("PRESENTER ERROR"))
                        {
                            _TransactionStatus = TransactionStatus.Retracted;
                        }
                        else if (_TransactionBlock.TransactionBlockList[e].Contains("CARD NUMBER AUTH CODE") || _TransactionBlock.TransactionBlockList[e].Contains("CARD NUMBER  AUTH CODE") || _TransactionBlock.TransactionBlockList[e].Contains("CARD NUMBERAUTH CODE"))
                        {
                            lineCount = 1;
                        }
                        else if (lineCount == 1)
                        {
                            _Transaction.CardNo = ejData[0];
                            if (_Transaction.CardNo.Contains("="))
                            {
                                string[] cardNoArry = _Transaction.CardNo.Split('=');
                                _Transaction.CardNo = cardNoArry[0];
                            }

                            if (ejData.Count > 1)
                            {
                                _Transaction.AuthCode = ejData[ejData.Count - 1];
                            }
                            lineCount = 2;
                        }

                        else if (_TransactionBlock.TransactionBlockList[e].Contains("TRANSACTION A/C") || _TransactionBlock.TransactionBlockList[e].Contains("TRANSACTION  A/C"))
                        {
                            lineCount = 3;
                        }
                        else if (lineCount == 3)
                        {
                            if (ejData[0].Trim().Equals(" BALANCE INQUIRY(SA) ".Trim()) || ejData[0].Trim().Equals(" BALANCE INQUIRY".Trim()) ||
                                ejData[0].Trim().Equals("BALANCE-INQUIRY".Trim()) || ejData[0].Trim().Equals("BALANCE-ENQUIRY(CA)".Trim()) ||
                                ejData[0].Trim().Equals("BALANCE-INQUIRY(SB)".Trim()))
                            {
                                _TransactionType = TransactionType.BalInquiry;
                            }
                            else if (ejData[0].Trim().Equals("CASH-WITHDRAWAL(SA)".Trim()) || ejData[0].Trim().Equals(" FAST CASH(DR) ".Trim()) ||
                                ejData[0].Trim().Equals(" FAST CASH ".Trim()) || ejData[0].Trim().Equals("FAST-CASH".Trim()) ||
                                ejData[0].Trim().Equals("CASH-WITHDRAWAL(CA)".Trim()) || ejData[0].Trim().Equals("CASH-WITHDRAWAL".Trim()) ||
                                ejData[0].Trim().Equals("CASH-BALANCE".Trim()) || ejData[0].Trim().Equals("CASH".Trim()))
                            {
                                _TransactionType = TransactionType.Financial;
                            }
                            else if (ejData[0].Trim().Equals("PIN-CHANGE".Trim()) || ejData[0].Trim().Equals("PIN - CHANGE(DR)".Trim()) ||
                                ejData[0].Trim().Equals("PIN-CHANGE(CR)".Trim()))
                            {
                                _TransactionType = TransactionType.PinChange;
                            }

                            if (ejData.Count > 1)
                            {
                                _Transaction.AccountNo = ejData[ejData.Count - 1];
                            }

                            lineCount = 4;
                        }

                        else if (_TransactionBlock.TransactionBlockList[e].Contains("REFERENCE NO.") || _TransactionBlock.TransactionBlockList[e].Contains("REFERENCE NO. "))
                        {
                            _Transaction.ReferenceNo = ejData[ejData.Count - 1];
                        }
                        else if (_TransactionBlock.TransactionBlockList[e].Contains("AMOUNT"))
                        {
                            string[] amt = _TransactionBlock.TransactionBlockList[e].Trim(' ').Split(' ');
                            if (amt[amt.Length - 1].IndexOf("NPR") >= 0)
                            {
                                string[] amnt = amt[amt.Length - 1].Split('R');
                                _Transaction.TransactionAmount = Convert.ToDecimal(amnt[1]);
                            }
                            else
                            {
                                _Transaction.TransactionAmount = Convert.ToDecimal(ejData[ejData.Count - 1]);
                            }
                        }
                        else if (_TransactionBlock.TransactionBlockList[e].Contains("TRACE NO"))
                        {
                            _Transaction.TraceNo = ejData[ejData.Count - 1];
                        }
                        else if (_TransactionBlock.TransactionBlockList[e].Contains("RESPCODE"))
                        {
                            _Transaction.ResponseCode = GetResponseCode(ejData[ejData.Count - 1]);
                            _TransactionStatus = TransactionStatus.Success;
                            if (!_Transaction.ResponseCode.Equals("0000"))
                            {
                                _TransactionStatus = TransactionStatus.Fail;
                            }
                        }



                    }
                }
                if (_Transaction != null)
                {
                    if (GlobalHelper.IsEmpty(_Transaction.TransactionDate)) //is due to sometime  transaction block not hitted..
                    {
                        return null;
                    }
                    if (_Transaction.TerminalId.Length > 8)
                    {
                        _Transaction.TerminalId = _Transaction.TerminalId.Remove(0, 1);
                    }
                    _Transaction = GetTransaction(_Transaction, _TransactionType, _TransactionStatus, TerminalType.ATM);
                }

                return _Transaction;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
