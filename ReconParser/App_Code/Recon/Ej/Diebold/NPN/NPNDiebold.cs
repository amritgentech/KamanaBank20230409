using Db.Enum;
using Db.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReconParser.App_Code.Recon.Ej.Diebold.NPN
{
    public class NPNDiebold : Diebold
    {
        private string Diboldtype { get; set; }
        //  private string Diboldtype { get; set; }
        public NPNDiebold(String FileName, int FileCount)
            : base(FileName, FileCount)
        {
        }

        public void TransactionBlockSeperator(List<string> listString)
        {
            var count = 0;
            TransactionBlock _TransactionBlock = new TransactionBlock();
            foreach (string str in listString)
            {
                if (str.IndexOf("JOURNALING STARTED") >= 0 && count == 0)
                {
                    Diboldtype = "s2m";
                    count++;
                }
                else if (str.IndexOf("STATUS LUNO000") >= 0 && count == 0)
                {
                    Diboldtype = "s2m1";
                    count++;
                }
                if (str.IndexOf("LUNO123") >= 0 || str.IndexOf("LUNO000") >= 0)
                {
                    if (str.Contains("a*****JOURNAL RECORD RECEIPT*****ad") || str.Contains("a******JOURNAL RECORD RECEIPT******a") || str.Contains("a*****JOURNAL RECORD RECEIPT*****aa") || str.Contains("a******JOURNAL RECORD RECEIPT**********a"))
                    //if (str.Contains("a*****JOURNAL RECORD RECEIPT*****ad")  || str.Contains("a*****JOURNAL RECORD RECEIPT*****aa"))
                    {
                        if (str.Contains("a******JOURNAL RECORD RECEIPT******a"))
                        {
                            Diboldtype = "new";
                        }
                        if (str.Contains("a******JOURNAL RECORD RECEIPT**********a"))
                        {
                            Diboldtype = "newkamanabank";
                        }
                        _TransactionBlock = new TransactionBlock();
                        string[] strArray = str.Split('a');

                        _TransactionBlock.TransactionBlockList.AddRange(strArray);
                        TransactionBlocks.Add(_TransactionBlock);
                        _TransactionBlock = new TransactionBlock();

                    }
                    //if (str.Contains("******JOURNAL RECORD RECEIPT**********"))
                    //{
                    //    Diboldtype = "s2m";
                    //}
                }
                else if (Diboldtype == "s2m")
                {
                    if (str.IndexOf("TRANSACTION START") >= 0)
                    {
                        _TransactionBlock = new TransactionBlock();
                    }

                    _TransactionBlock.TransactionBlockList.Add(str);

                    if (str.IndexOf("TRANSACTION END") >= 0)
                    {
                        if (_TransactionBlock.TransactionBlockList.Count > 10)
                        {
                            TransactionBlocks.Add(_TransactionBlock);
                        }
                        _TransactionBlock = new TransactionBlock();
                    }

                }
                else if (Diboldtype == "s2m1")
                {
                    if (str.IndexOf("******JOURNAL RECORD RECEIPT**********") >= 0)
                    {
                        _TransactionBlock = new TransactionBlock();
                    }

                    _TransactionBlock.TransactionBlockList.Add(str);

                    if (str.IndexOf("************************************************") >= 0)
                    {
                        if (_TransactionBlock.TransactionBlockList.Count > 10)
                        {
                            TransactionBlocks.Add(_TransactionBlock);
                        }
                        _TransactionBlock = new TransactionBlock();
                    }
                }
                else //older diebold ,more old diebold
                {
                    if (str.IndexOf("*****JOURNAL RECORD RECEIPT*****") >= 0)
                    {
                        _TransactionBlock = new TransactionBlock();
                    }
                    _TransactionBlock.TransactionBlockList.Add(str);
                    if (str.IndexOf("*******************************") >= 0)
                    {
                        TransactionBlocks.Add(_TransactionBlock);
                        _TransactionBlock = new TransactionBlock();
                    }
                }
            }
        }
        public override void Parse()
        {
            Console.WriteLine("Parsing File {0}", FileName);
            try
            {
                List<string> listString = ReadDataFromFile(FileName);

                TransactionBlockSeperator(listString);

                GetTransactionList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error->NPNDiebold->Parse->{0}", ex.Message);
            }
            Console.WriteLine("Parsed File {0}", FileName);
        }
        public void GetTransactionList()
        {
            try
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public Transaction GetTransactionOlderDiebold(TransactionBlock _TransactionBlock)
        {

            TransactionType _TransactionType = TransactionType.NotDefine;
            TransactionStatus _TransactionStatus = TransactionStatus.NotDefine;

            Transaction _Transaction = new Transaction();
            try
            {
                DateTimeFormatInfo dateFormatInfo = new DateTimeFormatInfo();
                dateFormatInfo.ShortDatePattern = @"dd/MM/yyyy";

                string[] ejData = _TransactionBlock.TransactionBlockList[3].Split(' ');
                string[] cardAuthCode = _TransactionBlock.TransactionBlockList[6].Split(' ');

                _Transaction.TransactionDate = Convert.ToDateTime(ejData[4], dateFormatInfo);
                _Transaction.TransactionTime = TimeSpan.Parse(ejData[5]);
                _Transaction.TerminalId = ejData[6];

                List<string> lstCArdAuthCode = new List<string>();

                lstCArdAuthCode = cardAuthCode.Where(x => !string.IsNullOrEmpty(x)).ToList();
                if (lstCArdAuthCode[0].Substring(0, 3) == "000")
                {

                    _Transaction.CardNo = lstCArdAuthCode[4];
                    try
                    {
                        _Transaction.AuthCode = lstCArdAuthCode[5];
                    }
                    catch
                    {
                        _Transaction.AuthCode = "";
                    }

                }

                ejData = _TransactionBlock.TransactionBlockList[7].Split(':');

                if (ejData[3].Trim().Equals(" BALANCE INQUIRY(SA) ".Trim()) || ejData[3].Trim().Equals(" BALANCE INQUIRY".Trim()) ||
                                    ejData[3].Trim().Equals("BALANCE-INQUIRY".Trim()) || ejData[3].Trim().Equals("BALANCE-ENQUIRY(CA)".Trim()) ||
                                    ejData[3].Trim().Equals("BALANCE-INQUIRY(SB)".Trim()))
                {
                    _TransactionType = TransactionType.BalInquiry;
                }
                else if (ejData[3].Trim().Equals("CASH-WITHDRAWAL(SA)".Trim()) || ejData[3].Trim().Equals(" FAST CASH(DR) ".Trim()) ||
                    ejData[3].Trim().Equals(" FAST CASH ".Trim()) || ejData[3].Trim().Equals("FAST-CASH".Trim()) ||
                    ejData[3].Trim().Equals("CASH-WITHDRAWAL(CA)".Trim()) || ejData[3].Trim().Equals("CASH-WITHDRAWAL".Trim()) ||
                    ejData[3].Trim().Equals("CASH-BALANCE".Trim()))
                {
                    _TransactionType = TransactionType.Financial;
                }
                else if (ejData[3].Trim().Equals("PIN-CHANGE".Trim()) || ejData[3].Trim().Equals("PIN - CHANGE(DR)".Trim()) ||
                    ejData[3].Trim().Equals("PIN-CHANGE(CR)".Trim()))
                {
                    _TransactionType = TransactionType.PinChange;
                }
                else if (ejData[3].Trim().Equals("LOAD-CASH".Trim()) || ejData[3].Trim().Equals("UNLOAD-CASH".Trim()))
                {
                    _TransactionType = TransactionType.NotDefine;
                }
                else
                {
                    _TransactionType = TransactionType.BalInquiry;
                }

                List<string> lstAC = new List<string>();

                ejData = _TransactionBlock.TransactionBlockList[8].Split(':');

                lstAC = ejData.Where(x => !string.IsNullOrEmpty(x)).ToList();
                if (lstAC.Count > 1)
                {
                    _Transaction.AccountNo = lstAC[3];
                }

                List<string> lstAmount = new List<string>();

                ejData = _TransactionBlock.TransactionBlockList[9].Split(' ');

                lstAmount = ejData.Where(x => !string.IsNullOrEmpty(x)).ToList();
                _Transaction.TransactionAmount = decimal.Parse(lstAmount[6]);

                string[] traceResp = null;
                List<string> lstTraceNO = new List<string>();

                traceResp = _TransactionBlock.TransactionBlockList[10].Split(':');


                lstTraceNO = traceResp.Where(x => !string.IsNullOrEmpty(x)).ToList();
                _Transaction.TraceNo = lstTraceNO[3].Trim();

                List<string> lstResponsNO = new List<string>();
                string[] RespCode = null;

                RespCode = _TransactionBlock.TransactionBlockList[11].Split(' ');

                lstResponsNO = RespCode.Where(x => !string.IsNullOrEmpty(x)).ToList();

                _Transaction.ResponseCode = GetResponseCode(lstResponsNO[7]);
                _TransactionStatus = TransactionStatus.Success;
                if (!_Transaction.ResponseCode.Equals("0000"))
                {
                    _TransactionStatus = TransactionStatus.Fail;
                }

                _Transaction = GetTransaction(_Transaction, _TransactionType, _TransactionStatus, TerminalType.ATM);

                return _Transaction;
            }
            catch
            {
                throw;

            }
        }
        public Transaction GetTransaction(TransactionBlock _TransactionBlock)
        {

            bool isOldDiebold = false;
            TransactionType _TransactionType = TransactionType.NotDefine;
            TransactionStatus _TransactionStatus = TransactionStatus.NotDefine;

            Transaction _Transaction = new Transaction();
            try
            {
                DateTimeFormatInfo dateFormatInfo = new DateTimeFormatInfo();
                dateFormatInfo.ShortDatePattern = @"dd/MM/yyyy";

                string[] ejData = _TransactionBlock.TransactionBlockList[5].Split(' ');
                string[] cardAuthCode = _TransactionBlock.TransactionBlockList[8].Split(' ');
                if (ejData[0] == "00000000")
                {
                    var txn = GetTransactionOlderDiebold(_TransactionBlock);
                    return txn;
                }
                else if (Diboldtype == "s2m" || Diboldtype == "s2m1")
                {
                    var txn2 = GetTransactionS2m(_TransactionBlock);
                    return txn2;
                }
                else if (Diboldtype == "new")
                {
                    var txn3 = GetTransactionnew(_TransactionBlock);
                    return txn3;
                }
                else if(Diboldtype == "newkamanabank")
                {
                    var txn4 = GetTransactionnewkamana(_TransactionBlock);
                    return txn4;
                }
                if (string.IsNullOrEmpty(ejData[0].ToString().Trim()))
                {
                    isOldDiebold = true;
                    ejData = _TransactionBlock.TransactionBlockList[4].Split(' ');
                    _Transaction.TransactionDate = Convert.ToDateTime(ejData[0], dateFormatInfo);
                    _Transaction.TransactionTime = TimeSpan.Parse(ejData[1]);
                    _Transaction.TerminalId = ejData[2];

                    cardAuthCode = _TransactionBlock.TransactionBlockList[6].Split(' ');
                }
                else
                {
                    _Transaction.TransactionDate = Convert.ToDateTime(ejData[0], dateFormatInfo);
                    _Transaction.TransactionTime = TimeSpan.Parse(ejData[1]);
                    _Transaction.TerminalId = ejData[2];



                }


                List<string> lstCArdAuthCode = new List<string>();

                lstCArdAuthCode = cardAuthCode.Where(x => !string.IsNullOrEmpty(x)).ToList();
                if (lstCArdAuthCode[0].Substring(0, 3) == "000")
                {
                    if (lstCArdAuthCode[0].Length >= 19)
                    {
                        _Transaction.CardNo = TruncateValue(lstCArdAuthCode[0], 3, 16);
                    }
                    else
                    {
                        _Transaction.CardNo = lstCArdAuthCode[0];
                    }
                }
                else
                {
                    if (lstCArdAuthCode[0].Contains("TRANSACTION"))
                    {
                        return null;
                    }
                    _Transaction.CardNo = lstCArdAuthCode[0];
                }
                if (lstCArdAuthCode.Count > 1)
                {
                    _Transaction.AuthCode = lstCArdAuthCode[1];
                }
                if (isOldDiebold)
                {
                    ejData = _TransactionBlock.TransactionBlockList[7].Split(':');
                }
                else
                {
                    ejData = _TransactionBlock.TransactionBlockList[9].Split(':');
                }

                if (ejData[1].Trim().Equals(" BALANCE INQUIRY(SA) ".Trim()) || ejData[1].Trim().Equals(" BALANCE INQUIRY".Trim()) ||
                                    ejData[1].Trim().Equals("BALANCE-INQUIRY".Trim()) || ejData[1].Trim().Equals("BALANCE-ENQUIRY(CA)".Trim()) ||
                                    ejData[1].Trim().Equals("BALANCE-INQUIRY(SB)".Trim()))
                {
                    _TransactionType = TransactionType.BalInquiry;
                }
                else if (ejData[1].Trim().Equals("CASH-WITHDRAWAL(SA)".Trim()) || ejData[1].Trim().Equals(" FAST CASH(DR) ".Trim()) ||
                    ejData[1].Trim().Equals(" FAST CASH ".Trim()) || ejData[1].Trim().Equals("FAST-CASH".Trim()) ||
                    ejData[1].Trim().Equals("CASH-WITHDRAWAL(CA)".Trim()) || ejData[1].Trim().Equals("CASH-WITHDRAWAL".Trim()) ||
                    ejData[1].Trim().Equals("CASH-BALANCE".Trim()))
                {
                    _TransactionType = TransactionType.Financial;
                }
                else if (ejData[1].Trim().Equals("PIN-CHANGE".Trim()) || ejData[1].Trim().Equals("PIN - CHANGE(DR)".Trim()) ||
                    ejData[1].Trim().Equals("PIN-CHANGE(CR)".Trim()))
                {
                    _TransactionType = TransactionType.PinChange;
                }
                else if (ejData[1].Trim().Equals("LOAD-CASH".Trim()) || ejData[1].Trim().Equals("UNLOAD-CASH".Trim()))
                {
                    _TransactionType = TransactionType.NotDefine;
                }
                else
                {
                    _TransactionType = TransactionType.BalInquiry;
                }

                List<string> lstAC = new List<string>();
                if (isOldDiebold)
                {
                    ejData = _TransactionBlock.TransactionBlockList[8].Split(':');
                }
                else
                {
                    ejData = _TransactionBlock.TransactionBlockList[10].Split(':');
                }
                lstAC = ejData.Where(x => !string.IsNullOrEmpty(x)).ToList();
                if (lstAC.Count > 1)
                {
                    _Transaction.AccountNo = lstAC[1];
                }

                List<string> lstAmount = new List<string>();
                if (isOldDiebold)
                {
                    ejData = _TransactionBlock.TransactionBlockList[9].Split(' ');
                }
                else
                {
                    ejData = _TransactionBlock.TransactionBlockList[11].Split(' ');
                }
                lstAmount = ejData.Where(x => !string.IsNullOrEmpty(x)).ToList();
                _Transaction.TransactionAmount = decimal.Parse(lstAmount[2]);

                string[] traceResp = null;
                List<string> lstTraceNO = new List<string>();
                if (isOldDiebold)
                {
                    traceResp = _TransactionBlock.TransactionBlockList[10].Split(':');
                }
                else
                {
                    traceResp = _TransactionBlock.TransactionBlockList[12].Split(':');
                }

                lstTraceNO = traceResp.Where(x => !string.IsNullOrEmpty(x)).ToList();
                _Transaction.TraceNo = lstTraceNO[1].Trim();

                List<string> lstResponsNO = new List<string>();
                string[] RespCode = null;
                if (isOldDiebold)
                {
                    RespCode = _TransactionBlock.TransactionBlockList[11].Split(' ');
                }
                else
                {
                    RespCode = _TransactionBlock.TransactionBlockList[13].Split(' ');
                }
                lstResponsNO = RespCode.Where(x => !string.IsNullOrEmpty(x)).ToList();

                _Transaction.ResponseCode = GetResponseCode(lstResponsNO[3]);
                _TransactionStatus = TransactionStatus.Success;
                if (!_Transaction.ResponseCode.Equals("0000"))
                {
                    _TransactionStatus = TransactionStatus.Fail;
                }

                _Transaction = GetTransaction(_Transaction, _TransactionType, _TransactionStatus, TerminalType.ATM);

                return _Transaction;
            }
            catch
            {
                throw;
            }

            //if (error == 1)
            //{
            //    obj.IS_TXN_SUCCESS = 1;
            //    obj.RESPONSE_CODE = "0CDM";
            //}



        }
        public Transaction GetTransactionnew(TransactionBlock _TransactionBlock)
        {
            bool isOldDiebold = false;
            TransactionType _TransactionType = TransactionType.NotDefine;
            TransactionStatus _TransactionStatus = TransactionStatus.NotDefine;
            string Ejlogs = null;
            Transaction _Transaction = new Transaction();
            try
            {
                //NEW code changes by amrit
                var transactionList = _TransactionBlock.TransactionBlockList.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();                
                foreach (string str in transactionList)
                {

                    Ejlogs += str + Environment.NewLine;
                }
                DateTimeFormatInfo dateFormatInfo = new DateTimeFormatInfo();
                dateFormatInfo.ShortDatePattern = @"dd/MM/yyyy";              
                string[] ejData = transactionList[4].Split(' ');              
                string[] cardAuthCode = transactionList[6].Split(' ');
                _Transaction.TransactionDate = Convert.ToDateTime(ejData[0], dateFormatInfo);
                _Transaction.TransactionTime = TimeSpan.Parse(ejData[1]);
                _Transaction.TerminalId = ejData[2];
                cardAuthCode = transactionList[6].Split(' ');
                List<string> lstCArdAuthCode = new List<string>();

                lstCArdAuthCode = cardAuthCode.Where(x => !string.IsNullOrEmpty(x)).ToList();
                if (lstCArdAuthCode[0].Substring(0, 3) == "000")
                {
                    if (lstCArdAuthCode[0].Length >= 19)
                    {
                        _Transaction.CardNo = TruncateValue(lstCArdAuthCode[0], 3, 16);
                    }
                    else
                    {
                        _Transaction.CardNo = lstCArdAuthCode[0];
                    }
                }
                else
                {
                    if (lstCArdAuthCode[0].Contains("TRANSACTION"))
                    {
                        return null;
                    }
                    _Transaction.CardNo = lstCArdAuthCode[0];
                }
                if (lstCArdAuthCode.Count > 1)
                {
                    _Transaction.AuthCode = lstCArdAuthCode[1];
                }

                ejData = new string[] { transactionList[7].ToString() };
                
                if (ejData[0].Trim().Equals(" BALANCE INQUIRY(SA) ".Trim()) || ejData[0].Trim().Equals(" BALANCE INQUIRY".Trim()) ||
                                    ejData[0].Trim().Equals("BALANCE-INQUIRY".Trim()) || ejData[0].Trim().Equals("BALANCE-ENQUIRY(CA)".Trim()) ||
                                    ejData[0].Trim().Equals("BALANCE-INQUIRY(SB)".Trim()))
                {
                    _TransactionType = TransactionType.BalInquiry;
                }
                else if (ejData[0].Trim().Equals("TRANSACTION A/C".Trim()) || ejData[0].Trim().Equals("TTRANSACTION  A/C".Trim())
                    || ejData[0].Trim().Equals("CASH-WITHDRAWAL(SA)".Trim()) || ejData[0].Trim().Equals(" FAST CASH(DR) ".Trim()) ||
                    ejData[0].Trim().Equals(" FAST CASH ".Trim()) || ejData[0].Trim().Equals("FAST-CASH".Trim()) ||
                    ejData[0].Trim().Equals("CASH-WITHDRAWAL(CA)".Trim()) || ejData[0].Trim().Equals("CASH-WITHDRAWAL".Trim()) ||
                    ejData[0].Trim().Equals("CASH-BALANCE".Trim())|| ejData[0].Trim().Equals("TRANSACTION : FAST-CASH ".Trim())|| ejData[0].Trim().Equals("TRANSACTION : CASH-WITHDRAWAL ".Trim()))
                {
                    _TransactionType = TransactionType.Financial;
                }
                else if (ejData[0].Trim().Equals("PIN-CHANGE".Trim()) || ejData[0].Trim().Equals("PIN - CHANGE(DR)".Trim()) ||
                    ejData[0].Trim().Equals("PIN-CHANGE(CR)".Trim()))
                {
                    _TransactionType = TransactionType.PinChange;
                }
                else if (ejData[0].Trim().Equals("LOAD-CASH".Trim()) || ejData[0].Trim().Equals("UNLOAD-CASH".Trim()))
                {
                    _TransactionType = TransactionType.NotDefine;
                }
                else
                {
                    _TransactionType = TransactionType.BalInquiry;
                }

                List<string> lstAC = new List<string>();
                ejData = transactionList[9].Split(':');
                

                lstAC = ejData.Where(x => !string.IsNullOrEmpty(x)).ToList();
                if (lstAC.Count > 1)
                {
                    _Transaction.AccountNo = lstAC[1];
                }

                List<string> lstAmount = new List<string>();

               
                ejData = transactionList[9].Split(' ');
                if (ejData[0] == "REFERENCE" && ejData[1] == "NO." && ejData[3] != null)
                {
                    ejData = transactionList[10].Split(' ');
                    lstAmount = ejData.Where(x => !string.IsNullOrEmpty(x)).ToList();
                    _Transaction.TransactionAmount = decimal.Parse(lstAmount[2]);
                }
                else
                {
                    lstAmount = ejData.Where(x => !string.IsNullOrEmpty(x)).ToList();
                    _Transaction.TransactionAmount = decimal.Parse(lstAmount[2]);
                }


                string[] traceResp = null;
                List<string> lstTraceNO = new List<string>();

                traceResp = transactionList[9].Split(':');
                if (traceResp[0] == "REFERENCE NO. " && traceResp[1] != null)
                {
                    traceResp = transactionList[11].Split(':');
                    lstTraceNO = traceResp.Where(x => !string.IsNullOrEmpty(x)).ToList();
                    _Transaction.TraceNo = lstTraceNO[1].Trim();
                }
                else
                {
                    traceResp = transactionList[10].Split(':');
                    lstTraceNO = traceResp.Where(x => !string.IsNullOrEmpty(x)).ToList();
                    _Transaction.TraceNo = lstTraceNO[1].Trim();
                }


                List<string> lstResponsNO = new List<string>();
                string[] RespCode = null;
                
                RespCode = transactionList[9].Split(':');
                if (RespCode[0] == "REFERENCE NO. " && RespCode[1] != null)
                {
                    RespCode = transactionList[12].Split(' ');
                    lstResponsNO = RespCode.Where(x => !string.IsNullOrEmpty(x)).ToList();
                    _Transaction.ResponseCode = GetResponseCode(lstResponsNO[3]);
                }
                else
                {
                    RespCode = transactionList[11].Split(' ');
                    lstResponsNO = RespCode.Where(x => !string.IsNullOrEmpty(x)).ToList();
                    _Transaction.ResponseCode = GetResponseCode(lstResponsNO[3]);
                }

                _TransactionStatus = TransactionStatus.Success;
                if (!_Transaction.ResponseCode.Equals("0000"))
                {
                    _TransactionStatus = TransactionStatus.Fail;
                }
                //_Transaction.EJlogs = Ejlogs;
                if (_Transaction.TerminalId.Length > 8)
                {
                    _Transaction.TerminalId = _Transaction.TerminalId.Remove(0, 1);
                }
                _Transaction = GetTransaction(_Transaction, _TransactionType, _TransactionStatus, TerminalType.ATM);

                return _Transaction;

               
        }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public Transaction GetTransactionnewkamana(TransactionBlock _TransactionBlock)
        {
            bool isOldDiebold = false;
            TransactionType _TransactionType = TransactionType.NotDefine;
            TransactionStatus _TransactionStatus = TransactionStatus.NotDefine;
            string Ejlogs = null;
            Transaction _Transaction = new Transaction();
            try
            {
                //NEW code changes by amrit
                var transactionList = _TransactionBlock.TransactionBlockList.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                foreach (string str in transactionList)
                {

                    Ejlogs += str + Environment.NewLine;
                }
                DateTimeFormatInfo dateFormatInfo = new DateTimeFormatInfo();
                dateFormatInfo.ShortDatePattern = @"dd/MM/yyyy";
                string[] ejData = transactionList[4].Split(' ');
                string[] cardAuthCode = transactionList[6].Split(' ');
                _Transaction.TransactionDate = Convert.ToDateTime(ejData[0], dateFormatInfo);
                _Transaction.TransactionTime = TimeSpan.Parse(ejData[1]);
                _Transaction.TerminalId = ejData[2];
                cardAuthCode = transactionList[6].Split(' ');
                List<string> lstCArdAuthCode = new List<string>();

                lstCArdAuthCode = cardAuthCode.Where(x => !string.IsNullOrEmpty(x)).ToList();
                if (lstCArdAuthCode[0].Substring(0, 3) == "000")
                {
                    if (lstCArdAuthCode[0].Length >= 19)
                    {
                        _Transaction.CardNo = TruncateValue(lstCArdAuthCode[0], 3, 16);
                    }
                    else
                    {
                        _Transaction.CardNo = lstCArdAuthCode[0];
                    }
                }
                else
                {
                    if (lstCArdAuthCode[0].Contains("TRANSACTION"))
                    {
                        return null;
                    }
                    _Transaction.CardNo = lstCArdAuthCode[0];
                }
                if (lstCArdAuthCode.Count > 1)
                {
                    _Transaction.AuthCode = lstCArdAuthCode[1];
                }

                ejData = new string[] { transactionList[7].ToString() };

                if (ejData[0].Trim().Equals(" BALANCE INQUIRY(SA) ".Trim()) || ejData[0].Trim().Equals(" BALANCE INQUIRY".Trim()) ||
                                    ejData[0].Trim().Equals("BALANCE-INQUIRY".Trim()) || ejData[0].Trim().Equals("BALANCE-ENQUIRY(CA)".Trim()) ||
                                    ejData[0].Trim().Equals("BALANCE-INQUIRY(SB)".Trim()))
                {
                    _TransactionType = TransactionType.BalInquiry;
                }
                else if (ejData[0].Trim().Equals("TRANSACTION A/C".Trim()) || ejData[0].Trim().Equals("TTRANSACTION  A/C".Trim())
                    || ejData[0].Trim().Equals("CASH-WITHDRAWAL(SA)".Trim()) || ejData[0].Trim().Equals(" FAST CASH(DR) ".Trim()) ||
                    ejData[0].Trim().Equals(" FAST CASH ".Trim()) || ejData[0].Trim().Equals("FAST-CASH".Trim()) ||
                    ejData[0].Trim().Equals("CASH-WITHDRAWAL(CA)".Trim()) || ejData[0].Trim().Equals("CASH-WITHDRAWAL".Trim()) ||
                    ejData[0].Trim().Equals("CASH-BALANCE".Trim()) || ejData[0].Trim().Equals("TRANSACTION : FAST-CASH ".Trim()) || ejData[0].Trim().Equals("TRANSACTION : CASH-WITHDRAWAL ".Trim()))
                {
                    _TransactionType = TransactionType.Financial;
                }
                else if (ejData[0].Trim().Equals("PIN-CHANGE".Trim()) || ejData[0].Trim().Equals("PIN - CHANGE(DR)".Trim()) ||
                    ejData[0].Trim().Equals("PIN-CHANGE(CR)".Trim()))
                {
                    _TransactionType = TransactionType.PinChange;
                }
                else if (ejData[0].Trim().Equals("LOAD-CASH".Trim()) || ejData[0].Trim().Equals("UNLOAD-CASH".Trim()))
                {
                    _TransactionType = TransactionType.NotDefine;
                }
                else
                {
                    _TransactionType = TransactionType.BalInquiry;
                }

                List<string> lstAC = new List<string>();
                ejData = transactionList[9].Split(':');


                lstAC = ejData.Where(x => !string.IsNullOrEmpty(x)).ToList();
                if (lstAC.Count > 1)
                {
                    if (lstAC[0].Trim().Equals("REFERENCE NO. ".Trim()))
                    {
                        _Transaction.ReferenceNo = lstAC[1];
                    }
                    else
                    {
                        _Transaction.AccountNo = lstAC[1];
                    }
                    
                }

                List<string> lstAmount = new List<string>();


                ejData = transactionList[9].Split(' ');
                //Changes By amrit
                if (ejData[0] == "REFERENCE" && ejData[1] == "NO." && ejData[2] != null)
                {

                    ejData = transactionList[10].Split(' ');
                    lstAmount = ejData.Where(x => !string.IsNullOrEmpty(x)).ToList();
                    if (lstAmount[1].Substring(0, 2) == "RS")
                    {
                        _Transaction.TransactionAmount = decimal.Parse(lstAmount[1].Substring(2, 20));
                    }
                    else
                    {
                        _Transaction.TransactionAmount = decimal.Parse(lstAmount[1].Substring(3, 8));
                    }
                    //end 
                }
                else
                {
                    lstAmount = ejData.Where(x => !string.IsNullOrEmpty(x)).ToList();
                    _Transaction.TransactionAmount = decimal.Parse(lstAmount[2]);
                }


                string[] traceResp = null;
                List<string> lstTraceNO = new List<string>();

                traceResp = transactionList[9].Split(':');
                if (traceResp[0] == "REFERENCE NO. " && traceResp[1] != null)
                {
                    traceResp = transactionList[11].Split(':');
                    lstTraceNO = traceResp.Where(x => !string.IsNullOrEmpty(x)).ToList();
                    _Transaction.TraceNo = lstTraceNO[1].Trim();
                }
                else
                {
                    traceResp = transactionList[10].Split(':');
                    lstTraceNO = traceResp.Where(x => !string.IsNullOrEmpty(x)).ToList();
                    _Transaction.TraceNo = lstTraceNO[1].Trim();
                }


                List<string> lstResponsNO = new List<string>();
                string[] RespCode = null;

                RespCode = transactionList[9].Split(':');
                if (RespCode[0] == "REFERENCE NO. " && RespCode[1] != null)
                {
                    RespCode = transactionList[12].Split(' ');
                    lstResponsNO = RespCode.Where(x => !string.IsNullOrEmpty(x)).ToList();
                    _Transaction.ResponseCode = GetResponseCode(lstResponsNO[4]);
                }
                else
                {
                    RespCode = transactionList[11].Split(' ');
                    lstResponsNO = RespCode.Where(x => !string.IsNullOrEmpty(x)).ToList();
                    _Transaction.ResponseCode = GetResponseCode(lstResponsNO[3]);
                }

                _TransactionStatus = TransactionStatus.Success;
                if (!_Transaction.ResponseCode.Equals("0000"))
                {
                    _TransactionStatus = TransactionStatus.Fail;
                }
                //_Transaction.EJlogs = Ejlogs;
                if (_Transaction.TerminalId.Length > 8)
                {
                    _Transaction.TerminalId = _Transaction.TerminalId.Remove(0, 1);
                }
                _Transaction = GetTransaction(_Transaction, _TransactionType, _TransactionStatus, TerminalType.ATM);

                return _Transaction;


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        // for new s2m
        public Transaction GetTransactionS2m(TransactionBlock _TransactionBlock)
        {
            string Ejlogs = null;
            TransactionType _TransactionType = TransactionType.NotDefine;
            TransactionStatus _TransactionStatus = TransactionStatus.NotDefine;

            string terminalIdFirstPart = "";

            //if (ConfigurationManager.AppSettings["TerminalId_StartingPart"] != null)
            //{
            //    terminalIdFirstPart = ConfigurationManager.AppSettings["TerminalId_StartingPart"].ToString();
            //}

            Transaction _Transaction = null;
            int index = 0;
            foreach (string str in _TransactionBlock.TransactionBlockList)
            {
                try
                {
                    Ejlogs += str + System.Environment.NewLine;

                    if (str.IndexOf("JOURNAL RECORD") >= 0)
                    {
                        if (index == 1)
                        {
                            index = 2;
                        }
                        else if (index == 0)
                        {
                            _Transaction = new Transaction();
                            index = 2;
                        }
                    }

                    //S2M
                    else if (str.Contains("DATE  TIME  TERMINAL") || str.Contains("DATE TIME TERMINAL") || str.Contains("WINCOR ATM") || str.Contains("NABIL ATM") || str.Contains("LXBL"))
                    {

                        index = 2;

                    }
                    //Old
                    //else if (str.Contains("DATE TIME TERMINAL") || str.Contains("WINCOR ATM") || str.Contains("NABIL ATM") || str.Contains("LXBL"))
                    //{

                    //    index = 2;

                    //}

                    else if ((str.IndexOf("LOGICAL CASSETTE 2 LOW") >= 0) || (str.IndexOf("PHYSICAL CASSETTE 2 LOW") >= 0)
                        || (str.IndexOf("LOGICAL CASSETTE 2 EMPTY") >= 0) || (str.IndexOf("PHYSICAL CASSETTE 2 EMPTY") >= 0)
                        || (str.IndexOf("LOGICAL CASSETTE 1 LOW") >= 0) || (str.IndexOf("PHYSICAL CASSETTE 1 LOW") >= 0)
                        || (str.IndexOf("LOGICAL CASSETTE 1 EMPTY") >= 0) || (str.IndexOf("PHYSICAL CASSETTE 1 EMPTY") >= 0)
                        || (str.IndexOf("LOGICAL CASSETTE") >= 0) || (str.IndexOf("PHYSICAL CASSETTE") >= 0) || (str.IndexOf("GO OUT OF SERVICE COMMAND") >= 0)
                        || (str.IndexOf("CDM ERROR") >= 0) || (str.IndexOf("RECEIVED MESSAGE IN WRONG MODE") >= 0)
                        || (str.IndexOf("GO IN SERVICE COMMAND") >= 0) || (str.IndexOf("DEVICE CCCdmFW STATUS 2 SUPPLY 1") >= 0)
                        || (str.IndexOf("DEVICE LOG_CASS_1 STATUS 4 SUPPLY 3") >= 0) || (str.IndexOf("CASH TAKEN") >= 0)
                        || (str.IndexOf("CASH PRESENTED") >= 0) || (str.IndexOf("TAKEN") >= 0) || (str.IndexOf("OPERATOR DOOR CLOSED") >= 0))
                    {
                    }

                    else if ((str.IndexOf("TRANSACTION END") >= 0) && index == 1)
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
                    else if (index == 2 || str.Contains("SBL"))
                    {
                        if ((str.IndexOf("COMMUNICATION ERROR") >= 0) || (str.IndexOf("COMMUNICATION OFFLINE") >= 0))
                        {
                            return null;
                        }


                        try
                        {
                            DateTimeFormatInfo dateFormatInfo = new DateTimeFormatInfo();
                            dateFormatInfo.ShortDatePattern = @"dd/MM/yy";

                            string[] firstLine = str.Trim(' ').Split(' ');
                            DateTime dt;
                            if (firstLine.Length < 2)
                            {
                            }
                            else if (firstLine.Length < 4)
                            {
                                if (DateTime.TryParse(firstLine[0], dateFormatInfo, DateTimeStyles.None, out dt))
                                {
                                    _Transaction.TransactionDate = Convert.ToDateTime(firstLine[0], dateFormatInfo);
                                }
                                _Transaction.TransactionTime = TimeSpan.Parse(firstLine[1]);
                                _Transaction.TerminalId = firstLine[2];
                            }
                            else
                            {
                                if (DateTime.TryParse(firstLine[0], dateFormatInfo, DateTimeStyles.None, out dt))
                                {
                                    _Transaction.TransactionDate = Convert.ToDateTime(firstLine[0], dateFormatInfo);
                                }
                                if (firstLine.Length > 4)
                                {
                                    //S2M
                                    _Transaction.TransactionTime = TimeSpan.Parse(firstLine[2]);
                                    _Transaction.TerminalId = firstLine[4];
                                }
                                else
                                {
                                    //old
                                    _Transaction.TransactionTime = TimeSpan.Parse(firstLine[1]);
                                    _Transaction.TerminalId = firstLine[3];
                                }


                            }

                        }
                        catch (Exception ex)
                        {

                        }
                        index = 0;
                    }
                    else if (str.IndexOf("CARD") >= 0)
                    {
                        if (str.Contains("JAMMED") || str.Contains("INSERTED"))
                        {
                            continue;
                        }
                        if (str.Contains("RETAIN CARD NOT SUPPORTED") || str.Contains("RETAINED"))
                        {
                            if (str.Contains("RESPCODENL"))
                            {
                                string[] respcode = str.Split(' ');
                                string[] resp = respcode[14].Split('*');
                                _Transaction.ResponseCode = GetResponseCode(resp[1]);

                                _TransactionStatus = TransactionStatus.Success;

                                if (!_Transaction.ResponseCode.Equals("0000"))
                                {
                                    _TransactionStatus = TransactionStatus.Fail;
                                }
                            }
                            else
                            {
                                continue;
                            }

                        }
                        //S2M
                        else if (str.Contains("CARD NUMBER  AUTH CODE") || str.Contains("CARD NUMBER AUTH CODE") || str.Contains("CARD NUMBERAUTH CODE"))
                        {
                            index = 7;
                            continue;
                        }
                        //old
                        //else if (str.Contains("CARD NUMBER AUTH CODE"))
                        //{
                        //    index = 7;
                        //    continue;
                        //}
                        else if (str.Contains("VISA CARD ISSU "))
                        {

                        }
                        else if (str.Contains("CARD EJECT TIMER EXPIRED") || str.Contains("CARD RETAINED BY HOST"))
                        {
                            continue;
                        }
                        else if (str.IndexOf("111111") >= 0)
                        {
                            _Transaction = null;
                            break;
                        }

                        else if (str.Contains("CARD NUMBER") || str.Contains("CARD NUM "))
                        {
                            string[] cardno = str.Split(':');
                            _Transaction.CardNo = cardno[1];
                            if (_Transaction.CardNo.Contains("="))
                            {
                                string[] cardNoArry = _Transaction.CardNo.Split('=');
                                _Transaction.CardNo = cardNoArry[0];
                            }
                        }

                        else
                        {
                            string[] crd = str.Split(' ');
                            _Transaction.CardNo = crd[1];

                            if (_Transaction.CardNo.Contains("="))
                            {
                                string[] cardNoArry = _Transaction.CardNo.Split('=');
                                _Transaction.CardNo = cardNoArry[0];
                            }
                        }
                        if (_Transaction.CardNo.IndexOf("999999") >= 0 || _Transaction.CardNo.IndexOf("111111") >= 0)
                        {
                            return null;
                        }
                    }

                    if (index == 7)
                    {

                        string[] crdno = str.Split(' ');
                        if (crdno[0].Equals("")) // if card is null then skip..
                        {
                            continue;
                        }
                        if (crdno[0].Contains("-"))
                        {
                            string[] crdarray = crdno[0].Split('-');
                            string cardno = null;
                            foreach (var item in crdarray)
                                cardno += item;
                            _Transaction.CardNo = cardno;
                        }
                        else
                            _Transaction.CardNo = crdno[0];
                        index = 0;

                        if (_Transaction.CardNo.IndexOf("999999") >= 0 || _Transaction.CardNo.IndexOf("111111") >= 0)
                        {
                            return null;
                        }
                        if (_Transaction.CardNo.Contains("="))
                        {
                            string[] cardNoArry = _Transaction.CardNo.Split('=');
                            _Transaction.CardNo = cardNoArry[0];
                        }
                    }

                    else if (str.IndexOf("REFERENCE NO") >= 0 || str.IndexOf("REFERENCE NO.") >= 0 || str.IndexOf("REF NO") >= 0)
                    {
                        if (str.Contains("REF NO"))
                        {
                            if (str.Contains("*********************************"))
                            {
                                string[] refno = str.Split(' ');
                                _Transaction.ReferenceNo = refno[2];
                                string[] amnt = refno[refno.Length - 1].Split('*');
                                _Transaction.TransactionAmount = Convert.ToDecimal(amnt[0]);

                            }
                            else
                            {
                                string[] refn = str.Split(' ');
                                _Transaction.ReferenceNo = refn[refn.Length - 1];
                            }
                        }
                        else
                        {
                            string[] refno = str.Split(':');
                            if (refno[1].Trim().Equals(""))
                            {
                                continue;
                            }
                            _Transaction.ReferenceNo = refno[1];
                        }

                        _Transaction.ReferenceNo = _Transaction.ReferenceNo.Trim();
                    }

                    else if (str.IndexOf("TRACE NUMBER") >= 0 || str.IndexOf("TRACE") >= 0)
                    {
                        string[] traceno = str.Split(':');
                        _Transaction.TraceNo = traceno[1].Trim();
                    }
                    else if (str.IndexOf("TRANSACTION  ") >= 0 || str.IndexOf("TRANSACTION A/C") >= 0)
                    {
                        if (str.Contains(":"))
                        {
                            string[] tran = str.Split(':');
                            if (tran[1].Trim().Equals(" BALANCE INQUIRY(SA) ".Trim()))
                            {
                                _TransactionType = TransactionType.BalInquiry;
                            }
                            else if (tran[1].Trim().Equals("  CASH-WITHDRAWAL(SB)".Trim()) || tran[1].Trim().Equals(" FAST CASH(DR) ".Trim()) || tran[1].Trim().Equals("CASH-WITHDRAWAL".Trim()) || tran[1].Trim().Equals("FAST-CASH".Trim()))
                            {
                                _TransactionType = TransactionType.Financial;

                            }
                            else if (tran[1].Trim().Equals("UNLOAD-CASH".Trim()) || tran[1].Trim().Equals("LOAD-CASH".Trim()) || tran[1].Trim().Equals("MINI-STMT".Trim()) || tran[1].Trim().Equals("MINI-STATEMENT(SA)".Trim()) || tran[1].Trim().Equals("MINI-STMT(SA)".Trim()) || tran[1].Trim().Equals("CASH-BALANCE"))
                            {
                                _Transaction = null;
                                break;

                            }
                            index = 0;
                        }
                        else
                        {
                            index = 5;
                        }
                    }
                    else if (str.Contains("MINI-STATEMENT(SA)") || str.Contains("MINI-STATEMENT") || str.Contains("GLOBAL IME BANK LIMITED") || str.Contains("MINI STATEMENT ADVICE") || str.Contains("MINI-STMT"))
                    {
                        _Transaction = null;
                        break;
                    }


                    else if (index == 5 || str.Contains("TXN"))
                    {
                        string[] txn = str.Split(' ');

                        if (txn[0].Trim().Equals("BALANCE-INQUIRY(SA)".Trim()) || txn[0].Trim().Equals("BALANCE-INQUIRY".Trim()) ||
                            txn[0].Trim().Equals("BALANCE-INQUIRY(SB)".Trim()) || txn[0].Trim().Equals("BALANCE-INQUIRY(CR)".Trim()) ||
                            txn[0].Trim().Equals("PIN-CHANGE".Trim()))
                        {
                            _TransactionType = TransactionType.BalInquiry;
                        }
                        else if (txn[0].Trim().Equals("UNLOAD-CASH".Trim()) || txn[0].Trim().Equals("LOAD-CASH".Trim()) ||
                            txn[0].Trim().Equals("MINI-STMT".Trim()) || txn[0].Trim().Equals("MINI-STATEMENT(SA)".Trim()) ||
                            txn[0].Trim().Equals("MINI-STMT(SB)".Trim()) || txn[0].Trim().Equals("MINI-STMT(SA)".Trim()))
                        {
                            _Transaction = null;
                            break;
                        }
                        else if (txn[0].Trim().Equals(" CASH WITHDRAWAL(SA) ".Trim()) || txn[0].Trim().Equals(" FAST CASH(DR) ".Trim()) ||
                            txn[0].Trim().Equals(" FAST CASH ".Trim()) || txn[0].Trim().Equals("FAST-CASH(DR)".Trim()) || txn[0].Trim().Equals("FAST-CASH".Trim()) ||
                            txn[0].Trim().Equals("CASH-WITHDRAWAL".Trim()) || txn[0].Trim().Equals("CASH-WITHDRAWAL(SA)".Trim()) || txn[0].Trim().Equals("CASH".Trim()))
                        {
                            _TransactionType = TransactionType.Financial;
                        }
                        else if (txn[1].Trim().Equals("BALANCE-INQUIRY".Trim()) || txn[1].Trim().Equals("BALANCE-INQUIRY(SA)".Trim()) ||
                            txn[1].Trim().Equals("BALANCE-INQUIRY(SB)".Trim()) || txn[1].Trim().Equals("PIN-CHANGE".Trim()))
                        {
                            _TransactionType = TransactionType.BalInquiry;
                        }

                        else if (txn[1].Trim().Equals("UNLOAD-CASH".Trim()) || txn[1].Trim().Equals("LOAD-CASH".Trim()) ||
                            txn[1].Trim().Equals("MINI-STMT".Trim()) || txn[1].Trim().Equals("MINI-STATEMENT(SA)".Trim()) ||
                            txn[1].Trim().Equals("MINI-STMT(SB)".Trim()) || txn[1].Trim().Equals("MINI-STMT(SA)".Trim()))
                        {
                            _Transaction = null;
                            break;
                        }
                        else if (txn[1].Trim().Equals("CASH-WITHDRAWAL(SA)".Trim()) || txn[1].Trim().Equals(" FAST CASH(DR) ".Trim()) ||
                            txn[1].Trim().Equals("FAST-CASH".Trim()) || txn[1].Trim().Equals("CASH-WITHDRAWAL".Trim()) ||
                            txn[1].Trim().Equals("FAST-CASH(DR)".Trim()) || txn[1].Trim().Equals("CASH-BALANCE".Trim()) || txn[0].Trim().Equals("CASH".Trim()))
                        {
                            _TransactionType = TransactionType.Financial;
                        }
                        else if (txn[2].Trim().Equals("BALANCE-INQUIRY(SB)".Trim()) || txn[2].Trim().Equals("PIN-CHANGE".Trim()) ||
                            txn[2].Trim().Equals("BALANCE-INQUIRY".Trim()) || txn[2].Trim().Equals("BALANCE-INQUIRY(SB)".Trim()) ||
                            txn[2].Trim().Equals("BALANCE-INQUIRY(CR)".Trim()))
                        {
                            _TransactionType = TransactionType.BalInquiry;
                        }
                        else if (txn[2].Trim().Equals("UNLOAD-CASH".Trim()) || txn[2].Trim().Equals("LOAD-CASH".Trim()) ||
                            txn[2].Trim().Equals("MINI-STMT".Trim()) || txn[2].Trim().Equals("MINI-STATEMENT(SA)".Trim()) ||
                            txn[2].Trim().Equals("MINI-STMT(SB)".Trim()) || txn[2].Trim().Equals("MINI-STMT(SA)".Trim()))
                        {
                            _Transaction = null;
                            break;
                        }
                        else if (txn[2].Trim().Equals("CASH-WITHDRAWAL(SB)".Trim()) || txn[2].Trim().Equals(" FAST CASH(DR) ".Trim()) ||
                            txn[2].Trim().Equals("FAST-CASH".Trim()) || txn[2].Trim().Equals("CASH-WITHDRAWAL".Trim()) ||
                            txn[2].Trim().Equals("CASH-BALANCE".Trim()) || txn[0].Trim().Equals("CASH".Trim()))
                        {
                            _TransactionType = TransactionType.Financial;
                        }
                        else if (txn[3].Trim().Equals("UNLOAD-CASH".Trim()) || txn[3].Trim().Equals("LOAD-CASH".Trim()) ||
                            txn[3].Trim().Equals("MINI-STMT".Trim()) || txn[3].Trim().Equals("MINI-STATEMENT(SA)".Trim()) ||
                            txn[3].Trim().Equals("MINI-STMT(SB)".Trim()) || txn[3].Trim().Equals("MINI-STMT(SA)".Trim()))
                        {
                            _Transaction = null;
                            break;
                        }
                        else if (txn[3].Trim().Equals(" BALANCE INQUIRY(SA) ".Trim()) || txn[3].Trim().Equals("BALANCE-ENQUIRY(CA)".Trim()) ||
                            txn[3].Trim().Equals("BALANCE-INQUIRY(SB)".Trim()) || txn[3].Trim().Equals("PIN-CHANGE".Trim()))
                        {
                            _TransactionType = TransactionType.BalInquiry;
                        }
                        else if (txn[3].Trim().Equals("CASH-WITHDRAWAL(SA)".Trim()) || txn[3].Trim().Equals(" FAST CASH(DR) ".Trim()) ||
                            txn[3].Trim().Equals(" FAST CASH ".Trim()) || txn[3].Trim().Equals("FAST-CASH".Trim()) ||
                            txn[3].Trim().Equals("CASH-WITHDRAWAL(CA)".Trim()) || txn[3].Trim().Equals("CASH-WITHDRAWAL".Trim()) ||
                            txn[3].Trim().Equals("CASH-BALANCE".Trim()) || txn[0].Trim().Equals("CASH".Trim()))
                        {
                            _TransactionType = TransactionType.Financial;
                        }
                        index = 0;
                    }

                    else if (str.IndexOf("A/C") >= 0)
                    {
                        string[] accno = str.Trim(' ').Split(' ');
                        _Transaction.AccountNo = accno[accno.Length - 1];
                        if (accno[accno.Length - 1].Contains("A/C"))
                        {
                            _Transaction.AccountNo = null;
                        }
                    }


                    else if (str.IndexOf("AMOUNT") >= 0)
                    {
                        string[] amt = str.Trim(' ').Split(' ');
                        if (amt[amt.Length - 1] == "")
                        {
                            _Transaction.TransactionAmount = Convert.ToDecimal(0);
                        }
                        else if (amt[amt.Length - 1] == "ENTERED")
                        {

                        }
                        else if (str.IndexOf("=") >= 0)
                        {
                            continue;
                        }
                        // for kamanasewa bank
                        else if (amt[amt.Length - 1].IndexOf("NPR") >= 0)
                        {
                            string[] amnt = amt[amt.Length - 1].Split('R');
                            _Transaction.TransactionAmount = Convert.ToDecimal(amnt[1]);
                        }
                        else
                        {
                            if (amt[amt.Length - 1].Contains("*********************************"))
                            {
                                string[] amnt = amt[amt.Length - 1].Split('*');
                                _Transaction.TransactionAmount = Convert.ToDecimal(amnt[0]);
                            }
                            else
                            {
                                if (amt[amt.Length - 1].Contains(":"))
                                {
                                    _Transaction.TransactionAmount = Convert.ToDecimal(amt[amt.Length - 1].Trim(':'));
                                }
                                else
                                {
                                    _Transaction.TransactionAmount = Convert.ToDecimal(amt[amt.Length - 1]);
                                }
                            }
                        }
                    }
                    else if (str.Contains("RESPCODE") || str.Contains("RESPCODENL") || str.Contains("RCODE") || str.Contains("RESPONSE CODE"))
                    {
                        if (str.Contains("RESPCODE") || str.Contains("RCODE"))
                        {
                            string[] respcode = str.Split(' ');
                            if (respcode.Length > 17)
                            {
                                string[] resp = respcode[14].Split('*');
                                _Transaction.ResponseCode = GetResponseCode(resp[1]);
                            }
                            else
                            {
                                _Transaction.ResponseCode = GetResponseCode(respcode[respcode.Length - 1]);
                            }
                        }
                        else if (str.Contains("RESPONSE CODE "))
                        {
                            string[] res = str.Split(':');
                            if (res[1].Contains(" "))
                            {
                                string[] resp = res[1].Split(' ');
                                _Transaction.ResponseCode = GetResponseCode(resp[1]);
                            }
                            else
                            {
                                _Transaction.ResponseCode = GetResponseCode(res[1]);
                            }
                        }

                        _TransactionStatus = TransactionStatus.Success;
                        if (!_Transaction.ResponseCode.Equals("0000"))
                        {
                            _TransactionStatus = TransactionStatus.Fail;
                        }

                    }
                    else if (str.Contains("CASH RETRACTED") || str.ToLower().Contains("CDM ERROR"))
                    {
                        _TransactionStatus = TransactionStatus.Retracted;
                    }

                }
                catch (Exception ex)
                {
                    var sts = str;
                    throw;
                }
            }

            if (_Transaction != null)
            {
                if (_Transaction.TerminalId.Length > 8)
                {
                    _Transaction.TerminalId = _Transaction.TerminalId.Remove(0, 1);
                }
                //_Transaction.EJlogs = Ejlogs;
                _Transaction = GetTransaction(_Transaction, _TransactionType, _TransactionStatus, TerminalType.ATM);
            }


            return _Transaction;
        }
    }
}
