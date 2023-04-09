using Db.Enum;
using Db.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ReconParser.App_Code.Recon.Ej.Wincor.Kumari
{
    public class KumariYcsWincor : Wincor
    {
        public KumariYcsWincor(String FileName, int FileCount)
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
                var splittedTransactionBlock = SplitList(_TransactionBlock);
                Transaction _Transaction = GetTransaction(splittedTransactionBlock);

                if (_Transaction != null)
                {
                    Transactions.Add(_Transaction);
                }
            }
        }
        private TransactionBlock SplitList(TransactionBlock transactionBlock)
        {
            TransactionBlock splittedTransactionBlock = new TransactionBlock();
            List<List<string>> transactionBlocks = new List<List<string>>();
            int index = 0;
            foreach (var elementObject in transactionBlock.TransactionBlockList)
            {
                int subIndex = 0;
                if (elementObject.Equals("........................................"))
                {
                    subIndex = index;
                    List<string> TransactionBlockListTemp = new List<string>();

                    foreach (var ob in elementObject)
                    {
                        var pair = transactionBlock.TransactionBlockList.Select((Value, Index) => new { Value, Index })
                            .Single(p => p.Index == subIndex + 1).Value.ToString();

                        if (pair.Equals("........................................") || pair.Contains("TRANSACTION END"))
                        {
                            continue;
                        }
                        TransactionBlockListTemp.Add(pair);
                        subIndex++;
                    }
                    transactionBlocks.Add(TransactionBlockListTemp);
                }
                index++;
            }
            if (transactionBlocks.Count == 2)
            {
                var value = transactionBlocks.Select((Value, Index) => new { Value, Index })
                    .Single(p => p.Index == 0).Value;
                splittedTransactionBlock.TransactionBlockList = value;

            }
            if (transactionBlocks.Count > 3)
            {
                var value = transactionBlocks.Select((Value, Index) => new { Value, Index })
                    .Single(p => p.Index == 2).Value;
                splittedTransactionBlock.TransactionBlockList = value;
            }

            return splittedTransactionBlock;
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


                    if (str.Contains("REC"))
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
                    else if (str.Contains("FR"))
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
                    else if (str.Contains("FR"))
                    {
                        var accountNo = str.Split(':')[1].Trim();
                        _Transaction.AccountNo = accountNo;
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
