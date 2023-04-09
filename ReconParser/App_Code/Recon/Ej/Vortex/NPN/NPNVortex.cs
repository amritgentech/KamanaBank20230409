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
    public class NPNVortex : Vortex
    {
        public NPNVortex(String FileName, int FileCount)
            : base(FileName, FileCount)
        {
        }

        public void TransactionBlockSeperator(List<string> listString)
        {
            TransactionBlock _TransactionBlock = new TransactionBlock();
            foreach (string str in listString)
            {
                if (str.IndexOf("TRANSACTION START") >= 0)
                {
                    _TransactionBlock = new TransactionBlock();
                }
                _TransactionBlock.TransactionBlockList.Add(str);

                if (str.IndexOf("TRANSACTION END") >= 0)
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
                        if (ejData.Count > 0)
                        {

                            if (_TransactionBlock.TransactionBlockList[e].Contains("CARD NO"))
                            {
                                _Transaction.CardNo = ejData[2].Substring(3, 16);
                            }
                            else if (_TransactionBlock.TransactionBlockList[e].Contains("AMOUNT RS") || _TransactionBlock.TransactionBlockList[e].Contains("AMOUNT NPR"))
                            {
                                           //old Code committed                   
                                //var amt = Convert.ToDecimal(ejData[1].Substring(7, ejData[1].Length - 7));
                                  _Transaction.TransactionAmount = Convert.ToDecimal(ejData[2]);
                                //var amount = ejData[1].Replace("AMOUNT:",""); //Changes by amrit
                                //_Transaction.TransactionAmount = Convert.ToDecimal(ejData[1].Replace("AMOUNT:", ""));
                            }
                            else if (_TransactionBlock.TransactionBlockList[e].Contains("DATE TIME TERMINAL"))
                            {
                                lineCount = 1;
                            }
                            else if (lineCount == 1)
                            {
                             
                                if (DateTime.TryParseExact(ejData[0].ToString(), new string[] { "dd/MM/yyyy", "yy/MM/dd" }, null, DateTimeStyles.None, out dateTime))
                                {
                                    _Transaction.TransactionDate = Convert.ToDateTime(ejData[0], dateFormatInfo);
                                    _Transaction.TransactionTime = TimeSpan.Parse(ejData[1], dateFormatInfo);
                                }

                                _Transaction.TerminalId = ejData[2].ToString().Trim();

                                lineCount = 2;
                            }
                            else if (_TransactionBlock.TransactionBlockList[e].Contains("CARD NUMBER AUTH CODE"))
                            {
                                lineCount = 3;
                            }
                            else if (lineCount == 3)
                            {
                                if (ejData.Count > 1)
                                {
                                    _Transaction.AuthCode = ejData[1];
                                }
                                else
                                {
                                    _Transaction.AuthCode = null;
                                }
                               

                                lineCount = 4;
                            }
                            else if (_TransactionBlock.TransactionBlockList[e].Contains("TRANSACTION A/C"))
                            {
                                lineCount = 5;
                            }
                            else if (lineCount == 5)
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
                                else if (ejData[0].Trim().Equals("PIN-CHANGE".Trim()) || ejData[0].Trim().Equals("PIN-CHANGE(DR)".Trim()) ||
                                    ejData[0].Trim().Equals("PIN-CHANGE(CR)".Trim()))
                                {
                                    _TransactionType = TransactionType.PinChange;
                                }

                                lineCount = 6;
                            }
                            else if (_TransactionBlock.TransactionBlockList[e].Contains("TXN NO"))
                            {
                                _Transaction.TransactionNo = ejData[1].Substring(3);
                            }
                            else if (_TransactionBlock.TransactionBlockList[e].Contains("PRESENTER ERROR"))
                            {
                                _TransactionStatus = TransactionStatus.Retracted;
                            }
                            else if (_TransactionBlock.TransactionBlockList[e].Contains("REFERENCE NO."))
                            {
                                if (ejData.Count > 4)
                                {
                                    _Transaction.ReferenceNo = null;
                                }
                                else
                                {
                                    _Transaction.ReferenceNo = ejData[3];
                                }
                               
                            }
                            else if (_TransactionBlock.TransactionBlockList[e].Contains("TRACE NO"))
                            {
                                if (ejData.Count > 4)
                                {
                                    _Transaction.TraceNo = null;
                                }
                                else
                                {
                                    _Transaction.TraceNo = ejData[3];
                                }
                                
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
                }
                if (_Transaction != null)
                {
                    if (GlobalHelper.IsEmpty(_Transaction.TransactionDate)) //is due to sometime  transaction block not hitted..
                    {
                        return null;
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
