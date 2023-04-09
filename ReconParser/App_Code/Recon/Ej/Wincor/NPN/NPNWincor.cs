using Db.Enum;
using Db.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ReconParser.App_Code.Recon.Ej.Wincor.NPN
{
    public class NPNWincor : Wincor
    {
        public NPNWincor(String FileName, int FileCount)
            : base(FileName, FileCount)
        {
            // TransactionBlocks = new List<TransactionBlock>();
        }

        public override void Parse()
        {
            Console.WriteLine("Parsing File {0}", FileName);
            List<string> listString = ReadDataFromFile(FileName);

            TransactionBlockSeperator(listString);
            GetTransactionList();   
            Console.WriteLine("Parsed File{0}", FileName);
        }

        public void GetTransactionList()
        {
            try
            {

                // old are commented
                foreach (TransactionBlock _TransactionBlock in TransactionBlocks)
                {
                    //var a = TransactionBlocks.IndexOf(_TransactionBlock);

                    Transaction _Transaction = GetTransaction(_TransactionBlock);
                    if (_Transaction != null)
                    {
                        Transactions.Add(_Transaction);
                    }
                }

                //foreach (string str in listString)
                //{
                //    TransactionBlock _TransactionBlock = new TransactionBlock();
                //    _TransactionBlock.TransactionBlockList.Add(str);
                //    Transaction _Transaction = GetTransaction(_TransactionBlock);
                //    if (_Transaction != null)
                //    {
                //        Transactions.Add(_Transaction);
                //    }
                //}

            }
            catch(Exception e)
            {
            }
        }
        public Transaction GetTransaction(TransactionBlock _TransactionBlock)
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
                        || (str.IndexOf("CASH PRESENTED") >= 0) || (str.IndexOf("TAKEN") >= 0) || (str.IndexOf("OPERATOR DOOR CLOSED") >= 0) || (str.IndexOf("DEVICE CCEppFW/PIN30 EFC CODE: EV8 0 00 00") >= 0))
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
                        if (str.Contains("JAMMED") || str.Contains("INSERTED") || str.Contains("CAPTURED"))
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
                        else if (amt[amt.Length - 1] == "ENTERED" || amt[0] == "INVALID")
                        {

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
                    Console.WriteLine(ex.Message);
                    // newlyadded to save other transactions other then null
                    _Transaction = null;
                    break;
                    //
                    throw;
                }
            }

            if (_Transaction != null)
            {
                //_Transaction.EJlogs = Ejlogs;
                if (_Transaction.TerminalId.Length > 8)
                {
                    _Transaction.TerminalId = _Transaction.TerminalId.Remove(0, 1);
                }
                _Transaction = GetTransaction(_Transaction, _TransactionType, _TransactionStatus, TerminalType.ATM);
            }


            return _Transaction;
        }
        //public Transaction GetTransaction(TransactionBlock _TransactionBlock)
        //{
        //    TransactionType _TransactionType = TransactionType.NotDefine;
        //    TransactionStatus _TransactionStatus = TransactionStatus.NotDefine;

        //    string terminalIdFirstPart = "";

        //    //if (ConfigurationManager.AppSettings["TerminalId_StartingPart"] != null)
        //    //{
        //    //    terminalIdFirstPart = ConfigurationManager.AppSettings["TerminalId_StartingPart"].ToString();
        //    //}

        //    Transaction _Transaction = null;
        //    int index = 0;
        //    foreach (string str in _TransactionBlock.TransactionBlockList)
        //    {
        //        try
        //        {

        //            if (str.IndexOf("JOURNAL RECORD") >= 0)
        //            {
        //                if (index == 1)
        //                {
        //                    index = 2;
        //                }
        //                else if (index == 0)
        //                {
        //                    _Transaction = new Transaction();
        //                    index = 2;
        //                }
        //            }

        //            else if (str.Contains("DATE TIME TERMINAL") || str.Contains("WINCOR ATM") || str.Contains("NABIL ATM") || str.Contains("LXBL"))
        //            {

        //                index = 2;

        //            }

        //            else if ((str.IndexOf("LOGICAL CASSETTE 2 LOW") >= 0) || (str.IndexOf("PHYSICAL CASSETTE 2 LOW") >= 0)
        //                || (str.IndexOf("LOGICAL CASSETTE 2 EMPTY") >= 0) || (str.IndexOf("PHYSICAL CASSETTE 2 EMPTY") >= 0)
        //                || (str.IndexOf("LOGICAL CASSETTE 1 LOW") >= 0) || (str.IndexOf("PHYSICAL CASSETTE 1 LOW") >= 0)
        //                || (str.IndexOf("LOGICAL CASSETTE 1 EMPTY") >= 0) || (str.IndexOf("PHYSICAL CASSETTE 1 EMPTY") >= 0)
        //                || (str.IndexOf("LOGICAL CASSETTE") >= 0) || (str.IndexOf("PHYSICAL CASSETTE") >= 0) || (str.IndexOf("GO OUT OF SERVICE COMMAND") >= 0)
        //                || (str.IndexOf("CDM ERROR") >= 0) || (str.IndexOf("RECEIVED MESSAGE IN WRONG MODE") >= 0)
        //                || (str.IndexOf("GO IN SERVICE COMMAND") >= 0) || (str.IndexOf("DEVICE CCCdmFW STATUS 2 SUPPLY 1") >= 0)
        //                || (str.IndexOf("DEVICE LOG_CASS_1 STATUS 4 SUPPLY 3") >= 0) || (str.IndexOf("CASH TAKEN") >= 0)
        //                || (str.IndexOf("CASH PRESENTED") >= 0) || (str.IndexOf("TAKEN") >= 0) || (str.IndexOf("OPERATOR DOOR CLOSED") >= 0))
        //            {
        //            }

        //            else if ((str.IndexOf("TRANSACTION END") >= 0) && index == 1)
        //            {
        //                return null;
        //            }

        //            else if ((str.IndexOf("TRANSACTION") >= 0 && str.IndexOf("FAILED") >= 0) || (str.IndexOf("INFORMATION") >= 0 && str.IndexOf("ENTERED") >= 0)
        //                || (str.IndexOf("JRN/REC ERROR:") >= 0) || (str.IndexOf("IDCU ERROR:") >= 0) || (str.IndexOf("DEVICE CCRecPrtFW STATUS 4 SUPPLY 1") >= 0)
        //                || (str.IndexOf("OPERATOR DOOR OPENED") >= 0) || (str.IndexOf("GO OUT OF SERVICE COMMAND") >= 0))
        //            {
        //                if (index > 0)
        //                {
        //                    _Transaction = null;
        //                }
        //            }
        //            else if (index == 2 || str.Contains("SBL"))
        //            {
        //                if ((str.IndexOf("COMMUNICATION ERROR") >= 0) || (str.IndexOf("COMMUNICATION OFFLINE") >= 0))
        //                {
        //                    return null;
        //                }


        //                try
        //                {
        //                    DateTimeFormatInfo dateFormatInfo = new DateTimeFormatInfo();
        //                    dateFormatInfo.ShortDatePattern = @"dd/MM/yy";

        //                    string[] firstLine = str.Trim(' ').Split(' ');
        //                    DateTime dt;
        //                    if (firstLine.Length < 2)
        //                    {
        //                    }
        //                    else if (firstLine.Length < 4)
        //                    {
        //                        if (DateTime.TryParse(firstLine[0], dateFormatInfo, DateTimeStyles.None, out dt))
        //                        {
        //                            _Transaction.TransactionDate = Convert.ToDateTime(firstLine[0], dateFormatInfo);
        //                        }
        //                        _Transaction.TransactionTime = TimeSpan.Parse(firstLine[1]);
        //                        _Transaction.TerminalId = firstLine[2];
        //                    }
        //                    else
        //                    {
        //                        if (DateTime.TryParse(firstLine[0], dateFormatInfo, DateTimeStyles.None, out dt))
        //                        {
        //                            _Transaction.TransactionDate = Convert.ToDateTime(firstLine[0], dateFormatInfo);
        //                        }
        //                        _Transaction.TransactionTime = TimeSpan.Parse(firstLine[1]);
        //                        _Transaction.TerminalId = firstLine[3];
        //                    }

        //                }
        //                catch 
        //                {

        //                }
        //                index = 0;
        //            }
        //            else if (str.IndexOf("CARD") >= 0)
        //            {
        //                if (str.Contains("JAMMED"))
        //                {
        //                    continue;
        //                }
        //                if (str.Contains("RETAIN CARD NOT SUPPORTED") || str.Contains("RETAINED"))
        //                {
        //                    if (str.Contains("RESPCODENL"))
        //                    {
        //                        string[] respcode = str.Split(' ');
        //                        string[] resp = respcode[14].Split('*');
        //                        _Transaction.ResponseCode = GetResponseCode(resp[1]);

        //                        _TransactionStatus = TransactionStatus.Success;

        //                        if (!_Transaction.ResponseCode.Equals("0000"))
        //                        {
        //                            _TransactionStatus = TransactionStatus.Fail;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        continue;
        //                    }

        //                }
        //                else if (str.Contains("CARD NUMBER AUTH CODE"))
        //                {
        //                    index = 7;
        //                    continue;
        //                }
        //                else if (str.Contains("VISA CARD ISSU "))
        //                {

        //                }
        //                else if (str.Contains("CARD EJECT TIMER EXPIRED") || str.Contains("CARD RETAINED BY HOST"))
        //                {
        //                    continue;
        //                }
        //                else if (str.IndexOf("111111") >= 0)
        //                {
        //                    _Transaction = null;
        //                    break;
        //                }

        //                else if (str.Contains("CARD NUMBER") || str.Contains("CARD NUM "))
        //                {
        //                    string[] cardno = str.Split(':');
        //                    _Transaction.CardNo = cardno[1];
        //                    if (_Transaction.CardNo.Contains("="))
        //                    {
        //                        string[] cardNoArry = _Transaction.CardNo.Split('=');
        //                        _Transaction.CardNo = cardNoArry[0];
        //                    }
        //                }

        //                else
        //                {
        //                    string[] crd = str.Split(' ');
        //                    _Transaction.CardNo = crd[1];

        //                    if (_Transaction.CardNo.Contains("="))
        //                    {
        //                        string[] cardNoArry = _Transaction.CardNo.Split('=');
        //                        _Transaction.CardNo = cardNoArry[0];
        //                    }
        //                }
        //                if (_Transaction.CardNo.IndexOf("999999") >= 0 || _Transaction.CardNo.IndexOf("111111") >= 0)
        //                {
        //                    return null;
        //                }
        //            }

        //            if (index == 7)
        //            {

        //                string[] crdno = str.Split(' ');
        //                if (crdno[0].Equals("")) // if card is null then skip..
        //                {
        //                    continue;
        //                }

        //                _Transaction.CardNo = crdno[0];
        //                index = 0;

        //                if (_Transaction.CardNo.IndexOf("999999") >= 0 || _Transaction.CardNo.IndexOf("111111") >= 0)
        //                {
        //                    return null;
        //                }
        //                if (_Transaction.CardNo.Contains("="))
        //                {
        //                    string[] cardNoArry = _Transaction.CardNo.Split('=');
        //                    _Transaction.CardNo = cardNoArry[0];
        //                }
        //            }

        //            else if (str.IndexOf("REFERENCE NO") >= 0 || str.IndexOf("REFERENCE NO.") >= 0 || str.IndexOf("REF NO") >= 0)
        //            {
        //                if (str.Contains("REF NO"))
        //                {
        //                    if (str.Contains("*********************************"))
        //                    {
        //                        string[] refno = str.Split(' ');
        //                        _Transaction.ReferenceNo = refno[2];
        //                        string[] amnt = refno[refno.Length - 1].Split('*');
        //                        _Transaction.TransactionAmount = Convert.ToDecimal(amnt[0]);

        //                    }
        //                    else
        //                    {
        //                        string[] refn = str.Split(' ');
        //                        _Transaction.ReferenceNo = refn[refn.Length - 1];
        //                    }
        //                }
        //                else
        //                {
        //                    string[] refno = str.Split(':');
        //                    if (refno[1].Trim().Equals(""))
        //                    {
        //                        continue;
        //                    }
        //                    _Transaction.ReferenceNo = refno[1];
        //                }

        //                _Transaction.ReferenceNo = _Transaction.ReferenceNo.Trim();
        //            }

        //            else if (str.IndexOf("TRACE NUMBER") >= 0 || str.IndexOf("TRACE") >= 0)
        //            {
        //                string[] traceno = str.Split(':');
        //                _Transaction.TraceNo = traceno[1].Trim();
        //            }
        //            else if (str.IndexOf("TRANSACTION  ") >= 0 || str.IndexOf("TRANSACTION A/C") >= 0)
        //            {
        //                if (str.Contains(":"))
        //                {
        //                    string[] tran = str.Split(':');
        //                    if (tran[1].Trim().Equals(" BALANCE INQUIRY(SA) ".Trim()))
        //                    {
        //                        _TransactionType = TransactionType.BalInquiry;
        //                    }
        //                    else if (tran[1].Trim().Equals("  CASH-WITHDRAWAL(SB)".Trim()) || tran[1].Trim().Equals(" FAST CASH(DR) ".Trim()) || tran[1].Trim().Equals("CASH-WITHDRAWAL".Trim()) || tran[1].Trim().Equals("FAST-CASH".Trim()))
        //                    {
        //                        _TransactionType = TransactionType.Financial;

        //                    }
        //                    else if (tran[1].Trim().Equals("UNLOAD-CASH".Trim()) || tran[1].Trim().Equals("LOAD-CASH".Trim()) || tran[1].Trim().Equals("MINI-STMT".Trim()) || tran[1].Trim().Equals("MINI-STATEMENT(SA)".Trim()) || tran[1].Trim().Equals("MINI-STMT(SA)".Trim()) || tran[1].Trim().Equals("CASH-BALANCE"))
        //                    {
        //                        _Transaction = null;
        //                        break;

        //                    }
        //                    index = 0;
        //                }
        //                else
        //                {
        //                    index = 5;
        //                }
        //            }
        //            else if (str.Contains("MINI-STATEMENT(SA)") || str.Contains("MINI-STATEMENT") || str.Contains("GLOBAL IME BANK LIMITED") || str.Contains("MINI STATEMENT ADVICE") || str.Contains("MINI-STMT"))
        //            {
        //                _Transaction = null;
        //                break;
        //            }


        //            else if (index == 5 || str.Contains("TXN"))
        //            {
        //                string[] txn = str.Split(' ');

        //                if (txn[0].Trim().Equals("BALANCE-INQUIRY(SA)".Trim()) || txn[0].Trim().Equals("BALANCE-INQUIRY".Trim()) ||
        //                    txn[0].Trim().Equals("BALANCE-INQUIRY(SB)".Trim()) || txn[0].Trim().Equals("BALANCE-INQUIRY(CR)".Trim()) ||
        //                    txn[0].Trim().Equals("PIN-CHANGE".Trim()))
        //                {
        //                    _TransactionType = TransactionType.BalInquiry;
        //                }
        //                else if (txn[0].Trim().Equals("UNLOAD-CASH".Trim()) || txn[0].Trim().Equals("LOAD-CASH".Trim()) ||
        //                    txn[0].Trim().Equals("MINI-STMT".Trim()) || txn[0].Trim().Equals("MINI-STATEMENT(SA)".Trim()) ||
        //                    txn[0].Trim().Equals("MINI-STMT(SB)".Trim()) || txn[0].Trim().Equals("MINI-STMT(SA)".Trim()))
        //                {
        //                    _Transaction = null;
        //                    break;
        //                }
        //                else if (txn[0].Trim().Equals(" CASH WITHDRAWAL(SA) ".Trim()) || txn[0].Trim().Equals(" FAST CASH(DR) ".Trim()) ||
        //                    txn[0].Trim().Equals(" FAST CASH ".Trim()) || txn[0].Trim().Equals("FAST-CASH(DR)".Trim()) || txn[0].Trim().Equals("FAST-CASH".Trim()) ||
        //                    txn[0].Trim().Equals("CASH-WITHDRAWAL".Trim()) || txn[0].Trim().Equals("CASH-WITHDRAWAL(SA)".Trim()))
        //                {
        //                    _TransactionType = TransactionType.Financial;
        //                }
        //                else if (txn[1].Trim().Equals("BALANCE-INQUIRY".Trim()) || txn[1].Trim().Equals("BALANCE-INQUIRY(SA)".Trim()) ||
        //                    txn[1].Trim().Equals("BALANCE-INQUIRY(SB)".Trim()) || txn[1].Trim().Equals("PIN-CHANGE".Trim()))
        //                {
        //                    _TransactionType = TransactionType.BalInquiry;
        //                }

        //                else if (txn[1].Trim().Equals("UNLOAD-CASH".Trim()) || txn[1].Trim().Equals("LOAD-CASH".Trim()) ||
        //                    txn[1].Trim().Equals("MINI-STMT".Trim()) || txn[1].Trim().Equals("MINI-STATEMENT(SA)".Trim()) ||
        //                    txn[1].Trim().Equals("MINI-STMT(SB)".Trim()) || txn[1].Trim().Equals("MINI-STMT(SA)".Trim()))
        //                {
        //                    _Transaction = null;
        //                    break;
        //                }
        //                else if (txn[1].Trim().Equals("CASH-WITHDRAWAL(SA)".Trim()) || txn[1].Trim().Equals(" FAST CASH(DR) ".Trim()) ||
        //                    txn[1].Trim().Equals("FAST-CASH".Trim()) || txn[1].Trim().Equals("CASH-WITHDRAWAL".Trim()) ||
        //                    txn[1].Trim().Equals("FAST-CASH(DR)".Trim()) || txn[1].Trim().Equals("CASH-BALANCE".Trim()))
        //                {
        //                    _TransactionType = TransactionType.Financial;
        //                }
        //                else if (txn[2].Trim().Equals("BALANCE-INQUIRY(SB)".Trim()) || txn[2].Trim().Equals("PIN-CHANGE".Trim()) ||
        //                    txn[2].Trim().Equals("BALANCE-INQUIRY".Trim()) || txn[2].Trim().Equals("BALANCE-INQUIRY(SB)".Trim()) ||
        //                    txn[2].Trim().Equals("BALANCE-INQUIRY(CR)".Trim()))
        //                {
        //                    _TransactionType = TransactionType.BalInquiry;
        //                }
        //                else if (txn[2].Trim().Equals("UNLOAD-CASH".Trim()) || txn[2].Trim().Equals("LOAD-CASH".Trim()) ||
        //                    txn[2].Trim().Equals("MINI-STMT".Trim()) || txn[2].Trim().Equals("MINI-STATEMENT(SA)".Trim()) ||
        //                    txn[2].Trim().Equals("MINI-STMT(SB)".Trim()) || txn[2].Trim().Equals("MINI-STMT(SA)".Trim()))
        //                {
        //                    _Transaction = null;
        //                    break;
        //                }
        //                else if (txn[2].Trim().Equals("CASH-WITHDRAWAL(SB)".Trim()) || txn[2].Trim().Equals(" FAST CASH(DR) ".Trim()) ||
        //                    txn[2].Trim().Equals("FAST-CASH".Trim()) || txn[2].Trim().Equals("CASH-WITHDRAWAL".Trim()) ||
        //                    txn[2].Trim().Equals("CASH-BALANCE".Trim()))
        //                {
        //                    _TransactionType = TransactionType.Financial;
        //                }
        //                else if (txn[3].Trim().Equals("UNLOAD-CASH".Trim()) || txn[3].Trim().Equals("LOAD-CASH".Trim()) ||
        //                    txn[3].Trim().Equals("MINI-STMT".Trim()) || txn[3].Trim().Equals("MINI-STATEMENT(SA)".Trim()) ||
        //                    txn[3].Trim().Equals("MINI-STMT(SB)".Trim()) || txn[3].Trim().Equals("MINI-STMT(SA)".Trim()))
        //                {
        //                    _Transaction = null;
        //                    break;
        //                }
        //                else if (txn[3].Trim().Equals(" BALANCE INQUIRY(SA) ".Trim()) || txn[3].Trim().Equals("BALANCE-ENQUIRY(CA)".Trim()) ||
        //                    txn[3].Trim().Equals("BALANCE-INQUIRY(SB)".Trim()) || txn[3].Trim().Equals("PIN-CHANGE".Trim()))
        //                {
        //                    _TransactionType = TransactionType.BalInquiry;
        //                }
        //                else if (txn[3].Trim().Equals("CASH-WITHDRAWAL(SA)".Trim()) || txn[3].Trim().Equals(" FAST CASH(DR) ".Trim()) ||
        //                    txn[3].Trim().Equals(" FAST CASH ".Trim()) || txn[3].Trim().Equals("FAST-CASH".Trim()) ||
        //                    txn[3].Trim().Equals("CASH-WITHDRAWAL(CA)".Trim()) || txn[3].Trim().Equals("CASH-WITHDRAWAL".Trim()) ||
        //                    txn[3].Trim().Equals("CASH-BALANCE".Trim()))
        //                {
        //                    _TransactionType = TransactionType.Financial;
        //                }
        //                index = 0;
        //            }

        //            else if (str.IndexOf("A/C") >= 0)
        //            {
        //                string[] accno = str.Trim(' ').Split(' ');
        //                _Transaction.AccountNo = accno[accno.Length - 1];
        //                if (accno[accno.Length - 1].Contains("A/C"))
        //                {
        //                    _Transaction.AccountNo = null;
        //                }
        //            }


        //            else if (str.IndexOf("AMOUNT") >= 0)
        //            {
        //                string[] amt = str.Trim(' ').Split(' ');
        //                if (amt[amt.Length - 1] == "")
        //                {
        //                    _Transaction.TransactionAmount = Convert.ToDecimal(0);
        //                }
        //                else if (amt[amt.Length - 1] == "ENTERED")
        //                {

        //                }
        //                else
        //                {
        //                    if (amt[amt.Length - 1].Contains("*********************************"))
        //                    {
        //                        string[] amnt = amt[amt.Length - 1].Split('*');
        //                        _Transaction.TransactionAmount = Convert.ToDecimal(amnt[0]);
        //                    }
        //                    else
        //                    {
        //                        if (amt[amt.Length - 1].Contains(":"))
        //                        {
        //                            _Transaction.TransactionAmount = Convert.ToDecimal(amt[amt.Length - 1].Trim(':'));
        //                        }
        //                        else
        //                        {
        //                            _Transaction.TransactionAmount = Convert.ToDecimal(amt[amt.Length - 1]);
        //                        }
        //                    }
        //                }
        //            }
        //            else if (str.Contains("RESPCODE") || str.Contains("RESPCODENL") || str.Contains("RCODE") || str.Contains("RESPONSE CODE"))
        //            {
        //                if (str.Contains("RESPCODE") || str.Contains("RCODE"))
        //                {
        //                    string[] respcode = str.Split(' ');
        //                    if (respcode.Length > 17)
        //                    {
        //                        string[] resp = respcode[14].Split('*');
        //                        _Transaction.ResponseCode = GetResponseCode(resp[1]);
        //                    }
        //                    else
        //                    {
        //                        _Transaction.ResponseCode = GetResponseCode(respcode[respcode.Length - 1]);
        //                    }
        //                }
        //                else if (str.Contains("RESPONSE CODE "))
        //                {
        //                    string[] res = str.Split(':');
        //                    if (res[1].Contains(" "))
        //                    {
        //                        string[] resp = res[1].Split(' ');
        //                        _Transaction.ResponseCode = GetResponseCode(resp[1]);
        //                    }
        //                    else
        //                    {
        //                        _Transaction.ResponseCode = GetResponseCode(res[1]);
        //                    }
        //                }

        //                _TransactionStatus = TransactionStatus.Success;
        //                if (!_Transaction.ResponseCode.Equals("0000"))
        //                {
        //                    _TransactionStatus = TransactionStatus.Fail;
        //                }

        //            }
        //            else if (str.Contains("CASH RETRACTED") || str.ToLower().Contains("CDM ERROR"))
        //            {
        //                _TransactionStatus = TransactionStatus.Retracted;
        //            }

        //        }
        //        catch
        //        {
        //            var sts = str;
        //            throw;
        //        }
        //    }
        //    if (_Transaction != null)
        //        _Transaction = GetTransaction(_Transaction, _TransactionType, _TransactionStatus, TerminalType.ATM);

        //    return _Transaction;
        //}
    }
}
