using Db.Enum;
using Db.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ReconParser.App_Code.Recon.Ej.Ncr.Kumari
{
    public class KumariYcsNcr : Ncr
    {
        public KumariYcsNcr(String FileName, int FileCount)
            : base(FileName, FileCount)
        {
            //TransactionBlocks = new List<TransactionBlock>();
        }
        public void TransactionBlockSeperator(List<string> listString)
        {
            int increament = 0;
            bool uselessBlock = false;
            TransactionBlock _TransactionBlock = new TransactionBlock();

            try
            {

                foreach (string str in listString)
                {

                    _TransactionBlock.TransactionBlockList.Add(str);

                    if (str.IndexOf("........................................") >= 0)
                    {
                        if (_TransactionBlock.TransactionBlockList.Count > 7 &&
                            _TransactionBlock.TransactionBlockList.Count < 10)
                        {
                            foreach (var val in _TransactionBlock.TransactionBlockList)
                            {
                                if (HasUnicodeCharacter(val))   //filter unicode character like \u001B
                                {
                                    uselessBlock = true;
                                    continue;
                                }

                            }
                            if (!uselessBlock)
                                TransactionBlocks.Add(_TransactionBlock);

                            uselessBlock = false;
                        }
                        _TransactionBlock = new TransactionBlock();
                    }
                }

            }
            catch (Exception e)
            {
            }
        }

        public bool HasUnicodeCharacter(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                var substr = str.Substring(0, 1);
                foreach (var c in substr)
                {
                    if ((int)c == 27)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public override void Parse()
        {
            Console.WriteLine("Parsing File {0}", FileName);
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
            TransactionType _TransactionType = TransactionType.NotDefine;
            TransactionStatus _TransactionStatus = TransactionStatus.NotDefine;

            DateTimeFormatInfo dateFormatInfo = new DateTimeFormatInfo();
            dateFormatInfo.ShortDatePattern = @"dd/MM/yy";

            int error = 0;
            Transaction _Transaction = new Transaction();

            var val = _TransactionBlock.TransactionBlockList.Where(t => !string.IsNullOrEmpty(t)).ToList();
            foreach (var str in val)
            {
                try
                {
                    var datetimeAndTerminal = val[0].Split(' ').Where(x => !string.IsNullOrEmpty(x)).ToList();

                    dateFormatInfo = new DateTimeFormatInfo();
                    dateFormatInfo.ShortDatePattern = @"dd/MM/yy";

                    DateTime dt;
                    if (DateTime.TryParse(datetimeAndTerminal[0], dateFormatInfo, DateTimeStyles.None, out dt))
                    {
                        _Transaction.TransactionDate = Convert.ToDateTime(datetimeAndTerminal[0], dateFormatInfo);
                    }
                    _Transaction.TransactionTime = TimeSpan.Parse(datetimeAndTerminal[1]);
                    _Transaction.TerminalId = datetimeAndTerminal[2];


                    if (str.Contains("REC:"))
                    {
                        var cardno = Regex.Split(str, "REC")[0];
                        _Transaction.CardNo = cardno.Trim();
                    }
                    else if (str.Contains("REF.NO"))
                    {
                        var ReferenceNo = str.Split(':')[1];
                        _Transaction.ReferenceNo = ReferenceNo;

                        if (!string.IsNullOrEmpty(_Transaction.ReferenceNo))
                        {
                            _Transaction.TraceNo = _Transaction.ReferenceNo.Substring(_Transaction.ReferenceNo.Length - 6, 6);
                        }
                    }
                    //                    string spaceIninitial = string.Empty;
                    //                    string spaceInilast = string.Empty;
                    //                    var frIndex = str.IndexOf("FR:");
                    //                    if (frIndex > 0)
                    //                    {
                    //                        spaceIninitial = str.Substring(frIndex - 1);
                    //                        spaceInilast = str.Substring(frIndex + 3);
                    //                    }


                    //                    if (frIndex >= 0 && spaceIninitial.Equals(" ") && spaceInilast.Equals(" "))
                    else if (str.Contains("FR : "))
                    {
                        var AccountNo = str.Split(':')[1];
                        _Transaction.AccountNo = AccountNo;
                    }
                    else if (str.Contains("RESP"))
                    {
                        var isresopnsecode = str.Split(':');
                        if (isresopnsecode.Length < 2)
                        {
                            continue;
                        }
                        var ResponseCode = str.Split(':')[1].Trim();

                        _Transaction.ResponseCode = GetResponseCode(ResponseCode);
                        _TransactionStatus = TransactionStatus.Success;
                        if (!_Transaction.ResponseCode.Equals("0000"))
                        {
                            _TransactionStatus = TransactionStatus.Fail;
                        }
                    }
                    else if (str.Contains("RS"))
                    {
                        var AmountBlock = Regex.Split(str, "RS");
                        if (AmountBlock[0].Trim().Equals("BALANCE INQUIRY"))
                        {
                            _TransactionType = TransactionType.BalInquiry;
                        }
                        else if (AmountBlock[0].Trim().Equals("MINI STATEMENT") || AmountBlock[0].Trim().Equals("STATEMENT REQUEST") || AmountBlock[0].Trim().Equals("PIN VERFICATION"))
                        {
                            _TransactionType = TransactionType.MiniStatement;
                        }
                        else if (AmountBlock[0].Trim().Equals("CASH WITHDRAWAL") || AmountBlock[0].Trim().Equals("FAST CASH ") || AmountBlock[0].Trim().Equals("MOBILE PAYMENT"))
                        {
                            _TransactionType = TransactionType.Financial;
                            _Transaction.TransactionAmount =
                                Convert.ToDecimal(AmountBlock[1]);
                        }
                        else if (AmountBlock[0].Trim().Equals("PIN CHANGE "))
                        {
                            _TransactionType = TransactionType.PinChange;
                        }
                    }
                    else if (str.IndexOf("CDM ERROR") >= 0)
                    {
                        error = 1;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            if (error == 1)
            {
                _TransactionStatus = TransactionStatus.Fail;
                _Transaction.ResponseCode = "0CDM";
            }
            if (val.Count == 0)
            {
                return null;
            }
            _Transaction = GetTransaction(_Transaction, _TransactionType, _TransactionStatus, TerminalType.ATM);
            return _Transaction;
        }
    }
}
