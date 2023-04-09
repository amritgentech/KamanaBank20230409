using Db.Enum;
using Db.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ReconParser.App_Code.Recon.Visa
{
    public class Visa : Base
    {
        public List<TransactionBlock> TransactionBlocks;
        public string settlementdate;
        public Visa(String FileName, int FileCount)
            : base(FileName, FileCount)
        {
            settlementdate = "";
            TransactionBlocks = new List<TransactionBlock>();
            _Source = Source.FindName("VISA");
        }

        public void TransactionBlockSeperator(List<string> listString)
        {
            TransactionBlock _TransactionBlock = new TransactionBlock();
            foreach (string str in listString)
            {
                if (str.Trim().Contains("Cash Disbursement - Original          ----     Required Data") || str.Trim().Contains("Cash Disbursement - Original          ----     Additional Data") || str.Trim().Contains("Cash Disbursement - Original          ----     Payment Service Data")
                    || str.Trim().Contains("Cash Disbursement - Representment          ----     Required Data") || str.Trim().Contains("Cash Disbursement - Representment          ----     Additional Data") || str.Trim().Contains("Cash Disbursement - Representment          ----     Payment Service Data")
                    || str.Trim().Contains("Sales Draft - Original          ----     Required Data") || str.Trim().Contains("Sales Draft - Original          ----     Additional Data") || str.Trim().Contains("Sales Draft - Original          ----     Payment Service Data")
                    || str.Trim().Contains("Reversal, Cash Disbursement - First Presentment          ----     Required Data") || str.Trim().Contains("Reversal, Cash Disbursement - First Presentment          ----     Additional Data") || str.Trim().Contains("Reversal, Cash Disbursement - First Presentment          ----     Payment Service Data")
                    || str.Trim().Contains("Reversal, Sales Draft - First Presentment          ----     Required Data") || str.Trim().Contains("Reversal, Sales Draft - First Presentment          ----     Additional Data")
                    || str.Trim().Contains("Chargeback, Cash Disbursement - First          ----     Required Data") || str.Trim().Contains("Chargeback, Cash Disbursement - First          ----     Additional Data")
                    || str.Trim().Contains("Chargeback, Cash Disbursement - First          ----     Required Data") || str.Trim().Contains("Sales Draft - Representment          ----     Required Data")
                    || str.Trim().Contains("Sales Draft - Representment          ----     Additional Data") || str.Trim().Contains("Sales Draft - Representment          ----     Payment Service Data")
                    || str.Trim().Contains("Reversal, Sales Draft - First Presentment          ----     Payment Service Data"))
                {

                }
                else
                {
                    _TransactionBlock.TransactionBlockList.Add(str.Trim());
                }

                string v = str.Trim();
                if (str.Trim().Contains("Cash Disbursement - Original          ----     Required Data") || str.Trim().Contains("Cash Disbursement - Original          ----     Additional Data") || str.Trim().Contains("Cash Disbursement - Original          ----     Payment Service Data")
                    || str.Trim().Contains("Cash Disbursement - Representment          ----     Required Data") || str.Trim().Contains("Cash Disbursement - Representment          ----     Additional Data") || str.Trim().Contains("Cash Disbursement - Representment          ----     Payment Service Data")
                    || str.Trim().Contains("Sales Draft - Original          ----     Required Data") || str.Trim().Contains("Sales Draft - Original          ----     Additional Data") || str.Trim().Contains("Sales Draft - Original          ----     Payment Service Data")
                    || str.Trim().Contains("Reversal, Cash Disbursement - First Presentment          ----     Required Data") || str.Trim().Contains("Reversal, Cash Disbursement - First Presentment          ----     Additional Data") || str.Trim().Contains("Reversal, Cash Disbursement - First Presentment          ----     Payment Service Data")
                    || str.Trim().Contains("Reversal, Sales Draft - First Presentment          ----     Required Data") || str.Trim().Contains("Reversal, Sales Draft - First Presentment          ----     Additional Data")
                    || str.Trim().Contains("Reversal, Sales Draft - First Presentment          ----     Payment Service Data") || str.Trim().Contains("Sales Draft - Representment          ----     Required Data")
                    || str.Trim().Contains("Sales Draft - Representment          ----     Additional Data") || str.Trim().Contains("Sales Draft - Representment          ----     Payment Service Data")
                    || str.Trim().Contains("Chargeback, Cash Disbursement - First          ----     Required Data") || str.Trim().Contains("Chargeback, Cash Disbursement - First          ----     Additional Data"))
                {
                    TransactionBlocks.Add(_TransactionBlock);
                    _TransactionBlock = new TransactionBlock();
                    if (str.Contains("Cash Disbursement - Original          ----     Required Data") || str.Contains("Sales Draft - Original          ----     Required Data") || str.Contains("Reversal, Cash Disbursement - First Presentment          ----     Required Data") || str.Trim().Contains("Cash Disbursement - Representment          ----     Required Data") || str.Trim().Contains("Chargeback, Cash Disbursement - First          ----     Required Data") || str.Trim().Contains("Sales Draft - Representment          ----     Required Data"))
                        _TransactionBlock.TransactionBlockList.Add("Original Required Data");
                    else if (str.Contains("Cash Disbursement - Original          ----     Additional Data") || str.Contains("Sales Draft - Original          ----     Additional Data") || str.Contains("Reversal, Cash Disbursement - First Presentment          ----     Additional Data") || str.Trim().Contains("Cash Disbursement - Representment          ----     Additional Data") || str.Trim().Contains("Chargeback, Cash Disbursement - First          ----     Additional Data") || str.Trim().Contains("Sales Draft - Representment          ----     Additional Data"))
                        _TransactionBlock.TransactionBlockList.Add("Original Additional Data");
                    else if (str.Contains("Cash Disbursement - Original          ----     Payment Service Data") || str.Contains("Sales Draft - Original          ----     Payment Service Data") || str.Contains("Reversal, Cash Disbursement - First Presentment          ----     Payment Service Data") || str.Trim().Contains("Cash Disbursement - Representment          ----     Payment Service Data") || str.Trim().Contains("Sales Draft - Representment          ----     Payment Service Data"))
                        _TransactionBlock.TransactionBlockList.Add("Payment Service Data");
                }
            }
        }

        public List<string> ReadDataFromFile(string fileName)
        {
            try
            {
                List<string> currentcontent = new List<string>();
                string name = string.Empty;
                string repDate = string.Empty;
                List<string> listString = new List<string>();
                int lineCount = 0;
                //string sourceFile = System.IO.Path.Combine(fileName);
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    StreamReader sr = new StreamReader(fs);
                    while (true)
                    {
                        string line = sr.ReadLine();
                        if (line == null)
                            break;
                        if (fileName.Contains("EP705") || fileName.Contains("EP725") || fileName.Contains("EP707")
                            || fileName.Contains("EP727") || fileName.Contains("EP717") || fileName.Contains("EP715"))
                        {
                            if (line.Contains("REPORT EP-707") || line.Contains("REPORT EP-727") || line.Contains("REPORT EP-717") || line.Contains("SYSTEM DATE") || line.Contains("CENTER") || string.IsNullOrWhiteSpace(line)
                               || line.Contains("TRANSACTION TOTAL RECORD COUNT"))
                            {
                                if (line.Contains("SYSTEM DATE"))
                                {
                                    string[] strAry = line.Split(' ');
                                    //settlementdate = strAry[3];
                                    settlementdate = strAry[strAry.Length - 1];
                                }
                            }
                            else
                            {
                                currentcontent.Add(line);
                            }
                        }
                        else if (fileName.Contains("EP745"))
                        {
                            if (line.Contains("ONLINE SETTLMNT DATE:"))
                            {
                                string subStr = line.Substring(line.IndexOf("ONLINE SETTLMNT DATE:"));
                                string[] strAry = subStr.Split(':');
                                settlementdate = strAry[1].Trim();
                            }
                            if (line.Contains("FUNDS XFR:"))
                            {
                                name = "OwnATM";
                            }
                            if (name == "OwnATM")
                            {
                                if (line.Contains("REPORT ID:") || line.Contains("FUNDS XFR:") || line.Contains("PROCESSOR:") || line.Contains("SRE      :")
                                    || line.Contains("-----------") || line.Contains("BAT XMIT(GMT)/LOCL") || line.Contains("AFFILIATE:")
                                    || line.Contains("NUM DATE  TIME") || line.Contains("ISSUER ID:") || line.Contains("CREDITS") || line.Contains("AFFILIATE ID:")
                                    || line.Contains("SUBTOTAL") || line.Contains("NON FINANCIAL") || line.Contains("BAT TRANSMISSION") || line.Contains("DEBITS")
                                    || line.Contains("TOTAL FEES") || line.Contains("USAGE:") || line.Contains("TRANSACTION TOTAL RECORD COUNT")
                                    || string.IsNullOrWhiteSpace(line) || line.Contains("TR ID:") || line.Contains("FEE JURIS:") || line.Contains("DG CD:")
                                    || line.Contains("ROUTING:") || line.Contains("ACTIVE MEMBERS REPORTING") || line.Contains("ISSUER BIN TOTALS") || line.Contains("RESPONSE")
                                    || line.Contains("VOLUME  PERCENT") || line.Contains("TOTAL AUTHORISATION TRANSACTIONS") || line.Contains("TOTAL ELIGIBLE CVV TRANSACTIONS:")
                                    || line.Contains(" CORRECT CVV TRANSACTIONS") || line.Contains("INCORRECT CVV TRANSACTIONS:") || line.Contains("SHORT STRIPE")
                                    || line.Contains("CVV INCORRECT") || line.Contains("Other ERRORS (UNABLE to CHECK)") || line.Contains("TOTAL") || line.Contains("ISSUER")
                                    || line.Contains("STIP") || line.Contains("APPROVAL")
                                    || line.Contains("REFERRAL") || line.Contains("PICK-UP") || line.Contains("DECLINE") || line.Contains("BI-WEEKLY")
                                    || line.Contains("BIN NAME")
                                    || line.Contains("FRAUD TYPE/DESCRIPTION") || line.Contains("TRANSACTIONS") || line.Contains("CARD REPORTED")
                                    || line.Contains("NOT RECEIVED AS ISSUED") || line.Contains("FRAUDULENT")
                                    || line.Contains("ISSUER REPORTED") || line.Contains("MISCELLANEOUS") || line.Contains("ACQUIRER REPORTED") || line.Contains("MERCHANT")
                                    || line.Contains("DEPARTMENT STORES") || line.Contains("ACQUIRER'S  CURRENCY") || line.Contains("END OF REPORT")
                                    //|| line.Contains("BIN") 
                                    || line.Contains("VISA CONFIDENTIAL") || line.Contains("FRAUD")
                                    || line.Contains("DATA ") || line.Contains("ISSUER") || line.Contains("MANDATORY") || line.Contains("CAPACITY")
                                    || line.Contains("FORWARDING REQUESTED")
                                    || line.Contains("RANDOMLY SELECTED") || line.Contains("ACTIVITY")
                                    || line.Contains("EXCEED") || line.Contains("RESPONSES") || line.Contains("VERIFICATIONS") || line.Contains("LIMIT")
                                    || line.Contains("LIMITS")// || line.Contains("NON-FINANCIAL :")
                                    || line.Contains("SUPPRESS INQUIRY MODE") || line.Contains("ATR TIME-OUTS")
                                    || line.Contains("RECIPIENT") || line.Contains("MEDICAL") || line.Contains("COMMERCIAL TRAVEL") || line.Contains("LODGING")
                                    || line.Contains("AUTOMOBILE RENTAL") || line.Contains("RESTAURANT") || line.Contains("MAIL / TELEPHONE")
                                    || line.Contains("PURCHASE") || line.Contains("CASH") || line.Contains("TRAN") || line.Contains("FAIL") || line.Contains("UNRECOGNIZED CVM")
                                    || line.Contains("PIN") || line.Contains("APPLICATION") || line.Contains("SDA NOT PERFORMED") || line.Contains("SCRIPT")
                                    || line.Contains("MAG STRIPE READ") || line.Contains("TRACE NUMBER")
                                    || line.Contains("UNRELIABLE") || line.Contains("SELECTION") || line.Contains("ADVICE") || line.Contains("CARD") || line.Contains("REGION")
                                    || line.Contains("AVAILABLE") || line.Contains("MISC") || line.Contains("STOR") || line.Contains("|") || line.Contains("STATION NOT")
                                    || line.Contains("ACI:") || line.Contains("SCHG:") || line.Contains("ATC:") || line.Contains("CI:"))
                                {
                                    if (line.Contains("ACI:") || line.Contains("SCHG:") || line.Contains("TR ID:"))
                                    {
                                        currentcontent.Add(line);
                                    }
                                    else
                                    {
                                        lineCount++;
                                    }

                                    if (line.Contains("PROCESSOR:") && repDate == "")
                                    {
                                        string[] reportDate = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);// line.Split("DATE").ToString();
                                        repDate = reportDate[reportDate.Length - 1];
                                        currentcontent.Add(reportDate[reportDate.Length - 1]);

                                    }
                                }
                                else if (line.Contains("ACQUIRER") || line.Contains("VISANET BATCH NUMBER") || line.Contains("DEBITS") || line.Contains("CREDITS")
                                         || line.Contains("DEBIT REVERSALS") || line.Contains("CREDIT REVERSALS") || line.Contains("VISANET TOTAL")
                                         || line.Contains("ONLINE TOTAL") || line.Contains("COUNT        AMOUNT(USD)") || line.Contains("TOTAL ACTING AS ACQUIRER")
                                         || line.Contains("NET TOTAL") || line.Contains("TRANSACTION TOTAL RECORD COUNT") || line.Contains("NON-FINANCIAL :"))
                                {
                                }
                                else
                                {
                                    currentcontent.Add(line);
                                }
                            }

                            //if (line.Contains("ONLINE SETTLMNT DATE:"))
                            //{
                            //    string subStr = line.Substring(line.IndexOf("ONLINE SETTLMNT DATE:"));
                            //    string[] strAry = subStr.Split(':');
                            //    settlementdate = strAry[1].Trim();
                            //}
                            //if (line.Contains("FUNDS XFR:"))
                            //{
                            //    name = "OwnATM";
                            //}
                            //if (name == "OwnATM")
                            //{
                            //    if (line.Contains("REPORT ID:") || line.Contains("FUNDS XFR:") || line.Contains("PROCESSOR:") || line.Contains("SRE      :")
                            //        || line.Contains("-----------") || line.Contains("BAT XMIT(GMT)/LOCL") || line.Contains("AFFILIATE:")
                            //        || line.Contains("NUM DATE  TIME") || line.Contains("ISSUER ID:") || line.Contains("CREDITS") || line.Contains("AFFILIATE ID:")
                            //        || line.Contains("SUBTOTAL") || line.Contains("NON FINANCIAL") || line.Contains("BAT TRANSMISSION") || line.Contains("DEBITS")
                            //        || line.Contains("TOTAL FEES") || line.Contains("USAGE:") || line.Contains("TRANSACTION TOTAL RECORD COUNT")
                            //        || string.IsNullOrWhiteSpace(line) || line.Contains("TR ID:") || line.Contains("FEE JURIS:") || line.Contains("DG CD:")
                            //        || line.Contains("ROUTING:") || line.Contains("ACTIVE MEMBERS REPORTING") || line.Contains("ISSUER BIN TOTALS") || line.Contains("RESPONSE")
                            //        || line.Contains("VOLUME  PERCENT") || line.Contains("TOTAL AUTHORISATION TRANSACTIONS") || line.Contains("TOTAL ELIGIBLE CVV TRANSACTIONS:")
                            //        || line.Contains(" CORRECT CVV TRANSACTIONS") || line.Contains("INCORRECT CVV TRANSACTIONS:") || line.Contains("SHORT STRIPE")
                            //        || line.Contains("CVV INCORRECT") || line.Contains("Other ERRORS (UNABLE to CHECK)") || line.Contains("TOTAL") || line.Contains("ISSUER")
                            //        || line.Contains("STIP") || line.Contains("APPROVAL")
                            //        || line.Contains("REFERRAL") || line.Contains("PICK-UP") || line.Contains("DECLINE") || line.Contains("BI-WEEKLY")
                            //        || line.Contains("BIN NAME")
                            //        || line.Contains("FRAUD TYPE/DESCRIPTION") || line.Contains("TRANSACTIONS") || line.Contains("CARD REPORTED")
                            //        || line.Contains("NOT RECEIVED AS ISSUED") || line.Contains("FRAUDULENT")
                            //        || line.Contains("ISSUER REPORTED") || line.Contains("MISCELLANEOUS") || line.Contains("ACQUIRER REPORTED") || line.Contains("MERCHANT")
                            //        || line.Contains("DEPARTMENT STORES") || line.Contains("ACQUIRER'S  CURRENCY") || line.Contains("END OF REPORT")
                            //        //|| line.Contains("BIN")
                            //        || line.Contains("VISA CONFIDENTIAL") || line.Contains("FRAUD")
                            //        || line.Contains("DATA ") || line.Contains("ISSUER") || line.Contains("MANDATORY") || line.Contains("CAPACITY")
                            //        || line.Contains("FORWARDING REQUESTED")
                            //        || line.Contains("RANDOMLY SELECTED") || line.Contains("ACTIVITY")
                            //        || line.Contains("EXCEED") || line.Contains("RESPONSES") || line.Contains("VERIFICATIONS") || line.Contains("LIMIT")
                            //        || line.Contains("LIMITS")// || line.Contains("NON-FINANCIAL :")
                            //        || line.Contains("SUPPRESS INQUIRY MODE") || line.Contains("ATR TIME-OUTS")
                            //        || line.Contains("RECIPIENT") || line.Contains("MEDICAL") || line.Contains("COMMERCIAL TRAVEL") || line.Contains("LODGING")
                            //        || line.Contains("AUTOMOBILE RENTAL") || line.Contains("RESTAURANT") || line.Contains("MAIL / TELEPHONE")
                            //        || line.Contains("PURCHASE") || line.Contains("CASH") || line.Contains("TRAN") || line.Contains("FAIL") || line.Contains("UNRECOGNIZED CVM")
                            //        || line.Contains("PIN") || line.Contains("APPLICATION") || line.Contains("SDA NOT PERFORMED") || line.Contains("SCRIPT")
                            //        || line.Contains("MAG STRIPE READ") || line.Contains("TRACE NUMBER")
                            //        || line.Contains("UNRELIABLE") || line.Contains("SELECTION") || line.Contains("ADVICE") || line.Contains("CARD") || line.Contains("REGION")
                            //        || line.Contains("AVAILABLE") || line.Contains("MISC") || line.Contains("STOR") || line.Contains("|") || line.Contains("STATION NOT")
                            //        || line.Contains("ACI:") || line.Contains("SCHG:"))
                            //    {
                            //        if (line.Contains("ACI:") || line.Contains("SCHG:") || line.Contains("TR ID:"))
                            //        {
                            //            currentcontent.Add(line);
                            //        }
                            //        else
                            //        {
                            //            lineCount++;
                            //        }

                            //        if (line.Contains("PROCESSOR:") && repDate == "")
                            //        {
                            //            string[] reportDate = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);// line.Split("DATE").ToString();
                            //            repDate = reportDate[reportDate.Length - 1];
                            //            currentcontent.Add(reportDate[reportDate.Length - 1]);

                            //        }
                            //    }
                            //    else if (line.Contains("ACQUIRER") || line.Contains("VISANET BATCH NUMBER") || line.Contains("DEBITS") || line.Contains("CREDITS")
                            //        || line.Contains("DEBIT REVERSALS") || line.Contains("CREDIT REVERSALS") || line.Contains("VISANET TOTAL")
                            //        || line.Contains("ONLINE TOTAL") || line.Contains("COUNT        AMOUNT(USD)") || line.Contains("TOTAL ACTING AS ACQUIRER")
                            //        || line.Contains("NET TOTAL") || line.Contains("TRANSACTION TOTAL RECORD COUNT") || line.Contains("NON-FINANCIAL :"))
                            //    {
                            //    }
                            //    else
                            //    {
                            //        currentcontent.Add(line);
                            //    }
                            //}
                        }
                    }
                    sr.Close();
                }

                return currentcontent;
            }
            catch
            {
                return new List<string>();
            }
        }

        public string TruncateValue(string truncateValue, int startIndex, int noOfDigits)
        {
            string truncated = truncateValue.Substring(startIndex, noOfDigits);
            return truncated;

        }

        public virtual void GetTransactionList()
        {
            TransactionType _TransactionType = TransactionType.NotDefine;
            TransactionStatus _TransactionStatus = TransactionStatus.NotDefine;
            CardType _CardType = CardType.NotDefine;
            NetworkType _NetworkType = NetworkType.VISA;
            TerminalType _TerminalType = TerminalType.ATM;
            TerminalOwner _TerminalOwner = TerminalOwner.OffUsTerminal;

            Transaction _Transaction = new Transaction();

            long countId = 1;
            int blockCount = 0;
            int firstTime = 0;
            foreach (TransactionBlock _TransactionBlock in TransactionBlocks)
            {
                try
                {
                    if (_TransactionBlock.TransactionBlockList.Count >= 13)
                    {
                        firstTime++;

                        if (blockCount == 3 || firstTime == 1)
                        {
                            blockCount = 0;
                            _Transaction = new Transaction();

                            _TransactionType = TransactionType.NotDefine;
                            _TransactionStatus = TransactionStatus.NotDefine;
                            //                            _CardType = CardType.OwnCard;
                            _NetworkType = NetworkType.VISA;
                            _TerminalType = TerminalType.ATM;
                            _TerminalOwner = TerminalOwner.OffUsTerminal;

                            countId++;

                            DateTimeFormatInfo dateFInfo = new DateTimeFormatInfo();
                            dateFInfo.ShortDatePattern = @"MM/dd/yyyy";//ncc bank
                            try
                            {
                                _Transaction.AdviseDate = Convert.ToDateTime(settlementdate, dateFInfo);
                            }
                            catch
                            {
                                dateFInfo.ShortDatePattern = @"yyyy/MM/dd"; // nepal bank
                                _Transaction.AdviseDate = Convert.ToDateTime(settlementdate, dateFInfo);
                            }

                            if (FileName.Contains("EP725") || FileName.Contains("EP705"))
                            {
                                _TerminalType = TerminalType.POS;
                            }
                        }
                        blockCount++;

                        DateTimeFormatInfo dateFormatInfo = new DateTimeFormatInfo();
                        dateFormatInfo.ShortDatePattern = @"dd/MM/yyyy";

                        List<string> ejData = new List<string>();
                        List<string> ejExtractData = new List<string>();
                        bool isRequiredData = false;

                        foreach (string str in _TransactionBlock.TransactionBlockList)
                        {

                            string[] eliminateData = str.Split(' ');
                            ejData = new List<string>();
                            for (int s = 0; s < eliminateData.Length; s++)
                            {
                                if (!string.IsNullOrEmpty(eliminateData[s]))
                                {
                                    ejData.Add(eliminateData[s]);
                                }
                            }

                            if (str.Contains("Acct Number & Extension"))
                            {
                                _Transaction.CardNo = TruncateValue(ejData[4], 0, 16);
                                _Transaction.ResponseCode = GetResponseCode("0000");

                                if (FileName.Contains("EP725") || FileName.Contains("EP727"))
                                {
                                    _Transaction.TransactionStatus = TransactionStatus.Reversal;
                                }
                                else
                                {
                                    _Transaction.TransactionStatus = TransactionStatus.Success;
                                }
                            }
                            else if (str.Contains("Usage Code"))
                            {
                                if (ejData[ejData.Count - 1].Equals("2"))
                                {
                                    _Transaction.TransactionStatus = TransactionStatus.Representment;
                                }

                                if (str.Contains("Acquirer Reference Nbr"))
                                {
                                    _Transaction.ReferenceNo = ejData[3].Substring(7, 4) + ejData[3].Substring(ejData[3].Length - 9, 8);
                                }
                            }
                            else if (str.Contains("Acquirer Reference Nbr"))
                            {
                                _Transaction.ReferenceNo = ejData[3].Substring(7, 4) + ejData[3].Substring(ejData[3].Length - 9, 8);
                            }
                            else if (str.Contains("Settlement Flag"))
                            {

                                if (ejData[ejData.Count - 1].Equals("0"))
                                {
                                    _Transaction.Currency = "USD";
                                    _TerminalOwner = TerminalOwner.ForeignTerminal;
                                }

                                if (str.Contains("Purchase Date"))
                                {
                                    string yyyy = ejData[2].Substring(0, 4);
                                    string dd = ejData[2].Substring(6, 2);
                                    string MM = ejData[2].Substring(4, 2);
                                    string stringDate = dd + "/" + MM + "/" + yyyy;
                                    //04/08/2009
                                    if (ejData[2].Equals("00000000"))
                                    {
                                        _Transaction.TransactionDate = _Transaction.AdviseDate.Value.AddDays(-1);
                                    }
                                    else
                                    {
                                        _Transaction.TransactionDate = Convert.ToDateTime(stringDate, dateFormatInfo);
                                        if (_Transaction.TransactionDate > _Transaction.AdviseDate)
                                        {
                                            _Transaction.TransactionDate = _Transaction.TransactionDate.AddYears(-1);
                                        }
                                    }
                                }
                            }
                            else if (str.Contains("Purchase Date"))
                            {
                                string yyyy = ejData[2].Substring(0, 4);
                                string dd = ejData[2].Substring(6, 2);
                                string MM = ejData[2].Substring(4, 2);
                                string stringDate = dd + "/" + MM + "/" + yyyy;
                                //04/08/2009
                                if (ejData[2].Equals("00000000"))
                                {
                                    _Transaction.TransactionDate = _Transaction.AdviseDate.Value.AddDays(-1);
                                }
                                else
                                {
                                    _Transaction.TransactionDate = Convert.ToDateTime(stringDate, dateFormatInfo);
                                    if (_Transaction.TransactionDate > _Transaction.AdviseDate)
                                    {
                                        _Transaction.TransactionDate = _Transaction.TransactionDate.AddYears(-1);
                                    }
                                }

                            }
                            else if (str.Contains("Destination Amount"))
                            {
                                _Transaction.TransactionAmount = decimal.Parse(ejData[2]) / 100;
                                _Transaction.DestinationTransactionAmount = decimal.Parse(ejData[2]) / 100;

                                //                                if (decimal.Parse(ejData[2]) != 0)
                                //                                    _TransactionType = TransactionType.Financial;
                                //                                else
                                //                                    _TransactionType = TransactionType.BalInquiry;
                                //
                                //                                if (str.Contains("Authorization Code"))
                                //                                {
                                //                                    _Transaction.AuthCode = ejData[ejData.Count - 1];
                                //                                }
                            }
                            else if (str.Contains("Source Amount"))
                            {
                                //                                _Transaction.TransactionAmount = decimal.Parse(ejData[2]) / 100;
                                _Transaction.SourceTransactionAmount = decimal.Parse(ejData[2]) / 100;
                                if (decimal.Parse(ejData[2]) != 0)
                                    _TransactionType = TransactionType.Financial;
                                else
                                    _TransactionType = TransactionType.BalInquiry;

                                if (str.Contains("Authorization Code"))
                                {
                                    _Transaction.AuthCode = ejData[ejData.Count - 1];
                                }
                            }
                            else if (str.Contains("Destination Currency Code"))
                            {
                                if (string.IsNullOrEmpty(_Transaction.Currency))
                                {
                                    if (ejData[3] == "840")
                                    {
                                        _Transaction.Currency = "USD";
                                        _TerminalOwner = TerminalOwner.ForeignTerminal;
                                    }
                                    else
                                    {
                                        _Transaction.Currency = "NPR";
                                    }
                                }

                                if (str.Contains("Authorization Code"))
                                {
                                    _Transaction.AuthCode = ejData[ejData.Count - 1];
                                }
                            }
                            else if (str.Contains("Source Currency Code"))
                            {
                                if (ejData[3] == "356")
                                {
                                    _Transaction.Currency = "INR";
                                    _TerminalOwner = TerminalOwner.INRTerminal;
                                }
                            }
                            else if (str.Contains("Interface Trace Number"))
                            {
                                if (ejData.Count == 7)
                                {
                                    if (ejData[6] != "000000")
                                    {
                                        if (ejData[6] != "Number")
                                            _Transaction.TraceNo = ejData[6];
                                    }
                                }
                            }
                            else if (str.Contains("Terminal ID"))
                            {
                                if (ejData.Count == 5)
                                {
                                    if (ejData[2] != "Purchase")
                                        _Transaction.TerminalId = ejData[2];
                                }
                                else if (ejData.Count == 6)
                                {
                                    if (ejData[5] == "Identifier")
                                    {
                                        _Transaction.TerminalId = ejData[2] + " " + ejData[3];
                                    }
                                    else
                                    {
                                        _Transaction.TerminalId = ejData[2];
                                    }
                                }

                                isRequiredData = true;
                            }
                            else if (str.Contains("Member Message Text") && str.Contains("CASH DISB"))
                            {
                                if (ejData.Count() > 6)
                                {
                                    if (ejData.Count() == 9)
                                        _Transaction.TerminalId = ejData[6].Substring(3, 8);
                                    else if (ejData.Count() == 8)
                                        _Transaction.TerminalId = ejData[5].Substring(3, 8);
                                }
                            }
                        }
                        if (isRequiredData)
                        {

                            _Transaction.GmtToLocalTransactionDate = _Transaction.TransactionDate;

                            _TransactionStatus = _Transaction.TransactionStatus;
                            _Transaction = GetTransaction(_Transaction, _TransactionType, _TransactionStatus, _TerminalType, _CardType, _TerminalOwner, _NetworkType);


                            if (_Transaction.CardType != CardType.OwnCard &&
                                _Transaction.TerminalOwner == TerminalOwner.OwnTerminal &&
                                _Transaction.TerminalType == TerminalType.POS &&
                                _Transaction.DestinationTransactionAmount == 0)
                            {
                                _Transaction.TransactionAmount = _Transaction.SourceTransactionAmount;
                            }
                            Transactions.Add(_Transaction);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// We know that this is EJ so we will compare it with other sources than EJ
        /// First we will check if todays respective transactions are saved in transaction table or not.
        /// We are making a must rule that CBS data should be present first.
        ///
        /// My assumptions here is that recon.Reconcile method should create all necessary relations
        /// </summary>
        override
        public void ProcessRecon()
        {
            Console.WriteLine("Recon Start..............");
            if (Transactions.Count < 1)
            {
                Console.WriteLine("Recon Complete..............");
                return;
            }
            //            VisaVsCbs recon = new VisaVsCbs(Transactions);
            //            recon.Reconcile();
            Console.WriteLine("Recon Complete..............");
        }
    }
}