using Db.Enum;
using Db.Model;
using ReadWriteFiles;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Db;
using Helper.GlobalHelpers;

//using Helper.GlobalHelper;

namespace ReconParser.App_Code.Recon.Neps
{
    public class NEPS : Base
    {
        public static String[] SUSPECTED_RESPONSE_CODES = new string[]
        {
            "0927",
            "0926",
            "0897"
        };

        public NEPS(String FileName, int FileCount)
            : base(FileName, FileCount)
        {
            _Source = Source.FindName("NEPS");
        }

        public override void Parse()
        {
            Console.WriteLine("Parsing File {0}", FileName);

            ReadDataFromFile(FileName);

            Console.WriteLine("Parsed File {0}", FileName);
        }

        public void GetTransactionList(DataTable excelDataTable, string filetype)
        {
            try
            {
                foreach (DataRow dr in excelDataTable.Rows)
                {
                    Transaction _Transaction = null;
                    if (excelDataTable.Columns.Count == 31 || excelDataTable.Columns.Count == 32 ||
                        excelDataTable.Columns.Count == 33)
                    {
                        if (filetype.Equals("xls"))
                        {
                            _Transaction = GetEjournalAndNTDetailForNEPS_xls(dr);
                        }
                        else
                        {
                            _Transaction = GetEjournalAndNTDetailForNEPS_xlsx(dr);
                        }
                    }

                    if (_Transaction != null)
                    {
                        //ejournalDetail = GetEjournalAndNtDetailByCheckingLoro(ejournalDetail, binNo, listMemberBinNo);

                        Transactions.Add(_Transaction);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected Transaction GetEjournalAndNTDetailForNEPS_xlsx(DataRow dr)
        {
            try
            {
                string AquirBinId = dr[28].ToString();
                TransactionType _TransactionType = TransactionType.BalInquiry;
                TransactionStatus _TransactionStatus = TransactionStatus.Fail;
                CardType _CardType = CardType.NotDefine;
                NetworkType _NetworkType = NetworkType.SCT;
                TerminalType _TerminalType = TerminalType.ATM;
                TerminalOwner _TerminalOwner = TerminalOwner.NotDefine;

                Transaction _Transaction = new Transaction();
                string txnTypeCode = dr[1].ToString();
                string txnType = dr[2].ToString();

                if (txnType.Equals("Purchase") || txnType.Equals("Cash advance"))
                {
                    _TransactionType = TransactionType.Financial;
                    _TerminalType = TerminalType.POS;
                }
                else if (txnType.Equals("Cash Withdrawal"))
                {
                    _TransactionType = TransactionType.Financial;
                    _TerminalType = TerminalType.ATM;
                }

                _Transaction.ProcessingCode = dr[3].ToString();
                _Transaction.CardNo = dr[4].ToString();

                _Transaction.AccountNo = dr[5].ToString();

                double d = double.Parse(dr[6].ToString());
                DateTime conv = DateTime.FromOADate(d);

                DateTime txnDate = DateTime.FromOADate(d);
                _Transaction.TransactionDate = txnDate;

                _Transaction.TerminalId = dr[8].ToString();
                _Transaction.TransactionAmount = Convert.ToDecimal(dr[9].ToString());
                decimal billingAmt = Convert.ToDecimal(dr[12].ToString());

                string currencyCode = dr[10].ToString();
                if (!currencyCode.Equals("356") && !currencyCode.Equals("524"))
                {
                    _Transaction.TransactionAmount = billingAmt;

                    _TerminalOwner = TerminalOwner.ForeignTerminal;
                    //                    _Transaction.Currency = Enum.GetName(typeof(Currency), Convert.ToInt32(currencyCode));
                    _Transaction.Currency = currencyCode;
                }
                else if (currencyCode.Equals("356"))
                {
                    _TerminalOwner = TerminalOwner.INRTerminal;
                    _Transaction.Currency = currencyCode;
                }
                else
                {
                    _TerminalOwner = TerminalOwner.INRTerminal;
                    _Transaction.Currency = currencyCode;

                    _NetworkType = NetworkType.VISA;
                    //                    string AquirBinId = dr[28].ToString();
                    if (txnType.Equals("Purchase") && AquirBinId.Equals("486278"))
                    {
                        _TerminalOwner = TerminalOwner.OffUsTerminal;
                        _NetworkType = NetworkType.OTHER;
                    }
                    else if (txnType.Equals("Purchase") && (AquirBinId.Equals("402810") || AquirBinId.Equals("470555")))
                    {
                        _TerminalOwner = TerminalOwner.OwnTerminal;
                        _NetworkType = NetworkType.OTHER;
                    }
                    else if (txnType.Equals("Purchase"))
                    {
                        _TerminalOwner = TerminalOwner.OffUsTerminal;
                        _NetworkType = NetworkType.VISA;
                    }
                }

                _Transaction.ReferenceNo = dr[17].ToString();

                if (!string.IsNullOrEmpty(_Transaction.ReferenceNo))
                {
                    _Transaction.TraceNo = _Transaction.ReferenceNo.Substring(_Transaction.ReferenceNo.Length - 6, 6);
                }

                _Transaction.AuthCode = dr[18].ToString();

                string responseCode = dr[19].ToString();
                string atmResponseCode = dr[21].ToString();
                if (responseCode.Equals("-1") || responseCode.Equals("0"))
                {
                    _Transaction.ResponseCode = GetResponseCode(atmResponseCode);
                }
                else
                {
                    _Transaction.ResponseCode = GetResponseCode(responseCode);
                }

                _TransactionStatus = GetTransactionStatus(_Transaction.ResponseCode);
                string responseCodeDescription = dr[20].ToString();

                string reversal = dr[24].ToString();
                if (reversal.Equals("1"))
                {
                    _TransactionStatus = TransactionStatus.Reversal;
                }
                else if (responseCode.Equals("0") && _TransactionStatus != TransactionStatus.Fail)
                {
                    _Transaction.ResponseCode = "0001";
                    _TransactionStatus = TransactionStatus.Fail;
                }

                if (dr.Table.Columns.Count > 32)
                {
                    string traceNo = dr[32].ToString();
                    if (!string.IsNullOrEmpty(traceNo))
                    {
                        _Transaction.TraceNo = traceNo;
                    }
                }

                _Transaction = GetTransaction(_Transaction, _TransactionType, _TransactionStatus, _TerminalType,
                    _CardType, _TerminalOwner, _NetworkType);

                Terminal _Terminal = Terminals.Find(GetTerminalByTerminalId(_Transaction.TerminalId));

                if (AquirBinId.Equals("402810") || AquirBinId.Equals("470555") || _Terminal != null)
                {
                    _Transaction.TerminalOwner = TerminalOwner.OwnTerminal;
                }

                if (_CardType == CardType.OwnCard && _TerminalType == TerminalType.POS &&
                    _TerminalOwner != TerminalOwner.OwnTerminal)
                {
                    _Transaction.TransactionAmount = billingAmt;
                }
                if (_Transaction.TerminalOwner != TerminalOwner.OwnTerminal && _Transaction.CardType == CardType.OwnCard
                    && string.IsNullOrEmpty(_Transaction.AccountNo) &&
                    _Transaction.TransactionStatus != TransactionStatus.Reversal &&
                    !_Transaction.ResponseCode.Equals("0903"))
                {
                    return null;
                }
                if (_Transaction.CardNo.Substring(0, 6).Equals("402064")) //credit card bin no..//set default receivable in case of credit card...
                {
                    _Transaction.CardType = CardType.CreditCard;   // off us  or recievable..
                }

                /* for transaction amount  credit card is also a own card*/
                if ((_Transaction.CardType.Equals(CardType.OwnCard) ||
                     _Transaction.CardType.Equals(CardType.CreditCard)) &&
                    _Transaction.TerminalOwner.Equals(TerminalOwner.OwnTerminal))
                {
                    _Transaction.TransactionAmount = _Transaction.TransactionAmount;
                }
                else if ((_Transaction.CardType.Equals(CardType.OwnCard) ||
                          _Transaction.CardType.Equals(CardType.CreditCard)) &&
                         !_Transaction.TerminalOwner.Equals(TerminalOwner.OwnTerminal))
                {
                    if (_TerminalType == TerminalType.ATM)
                    {
                        if (currencyCode.Equals("356"))
                        {
                            _Transaction.TransactionAmount =
                                Convert.ToDecimal(
                                    Convert.ToDouble(_Transaction.TransactionAmount) * 1.6015 + 250);

                            _Transaction.TransactionAmount = Math.Ceiling(_Transaction.TransactionAmount * 100) / 100;
                        }
                        else
                        {
                            _Transaction.TransactionAmount = billingAmt;
                        }
                    }
                    else if (_TerminalType == TerminalType.POS)
                    {
                        if (currencyCode.Equals("356"))
                        {
                            _Transaction.TransactionAmount =
                                Convert.ToDecimal(Convert.ToDouble(_Transaction.TransactionAmount) * 1.6);
                        }
                        else
                        {
                            _Transaction.TransactionAmount = billingAmt;
                        }
                    }
                }

                // only for payable case..

                _Transaction.ResponseCodeDescription = responseCodeDescription;
                return _Transaction;
            }
            catch
            {
                throw;
            }
        }

        protected Transaction GetEjournalAndNTDetailForNEPS_xls(DataRow dr)
        {
            try
            {
                string AquirBinId = dr[28].ToString();
                TransactionType _TransactionType = TransactionType.BalInquiry;
                TransactionStatus _TransactionStatus = TransactionStatus.Fail;
                CardType _CardType = CardType.NotDefine;
                NetworkType _NetworkType = NetworkType.VISA;
                TerminalType _TerminalType = TerminalType.ATM;
                TerminalOwner _TerminalOwner = TerminalOwner.NotDefine;

                Transaction _Transaction = new Transaction();
                string txnTypeCode = dr[1].ToString();
                string txnType = dr[2].ToString();

                if (txnType.Equals("Purchase") || txnType.Equals("Cash advance"))
                {
                    _TransactionType = TransactionType.Financial;
                    _TerminalType = TerminalType.POS;
                }
                else if (txnType.Equals("Cash Withdrawal"))
                {
                    _TransactionType = TransactionType.Financial;
                    _TerminalType = TerminalType.ATM;
                }

                _Transaction.ProcessingCode = dr[3].ToString();
                _Transaction.CardNo = dr[4].ToString();

                _Transaction.AccountNo = dr[5].ToString();

                DateTime txnDate = Convert.ToDateTime(dr[6]);
                _Transaction.TransactionDate = txnDate;

                _Transaction.TerminalId = dr[8].ToString();
                _Transaction.TransactionAmount = Convert.ToDecimal(dr[9].ToString());
                decimal billingAmt = Convert.ToDecimal(dr[12].ToString());




                string currencyCode = dr[10].ToString();
                if (!currencyCode.Equals("356") && !currencyCode.Equals("524"))
                {
                    _Transaction.TransactionAmount = billingAmt;

                    _TerminalOwner = TerminalOwner.ForeignTerminal;
                    _Transaction.Currency = currencyCode;
                    // _Transaction.Currency = Enum.GetName(typeof(Currency), Convert.ToInt32(currencyCode));
                }
                else if (currencyCode.Equals("356"))
                {
                    _TerminalOwner = TerminalOwner.INRTerminal;
                    _Transaction.Currency = currencyCode;
                }
                else
                {
                    _TerminalOwner = TerminalOwner.INRTerminal;
                    _Transaction.Currency = currencyCode;

                    _NetworkType = NetworkType.VISA;
                    if (txnType.Equals("Purchase") && AquirBinId.Equals("486278"))
                    {
                        _TerminalOwner = TerminalOwner.OffUsTerminal;
                        _NetworkType = NetworkType.OTHER;
                    }
                    else if (txnType.Equals("Purchase") && (AquirBinId.Equals("402810") || AquirBinId.Equals("470555")))
                    {
                        _TerminalOwner = TerminalOwner.OwnTerminal;
                        _NetworkType = NetworkType.OTHER;
                    }
                    else if (txnType.Equals("Purchase"))
                    {
                        _TerminalOwner = TerminalOwner.OffUsTerminal;
                        _NetworkType = NetworkType.VISA;
                    }

                }
                _Transaction.UtrNo = dr[16].ToString();
                _Transaction.ReferenceNo = dr[17].ToString();

                if (!string.IsNullOrEmpty(_Transaction.ReferenceNo))
                {
                    _Transaction.TraceNo = _Transaction.ReferenceNo.Substring(_Transaction.ReferenceNo.Length - 6, 6); 
                }

                _Transaction.AuthCode = dr[18].ToString();


                string responseCode = dr[19].ToString();
                string atmResponseCode = dr[21].ToString();
                if (responseCode.Equals("-1") || responseCode.Equals("0"))
                {
                    _Transaction.ResponseCode = GetResponseCode(atmResponseCode);
                }
                else
                {
                    _Transaction.ResponseCode = GetResponseCode(responseCode);
                }

                _TransactionStatus = GetTransactionStatus(_Transaction.ResponseCode);


                string responseCodeDescription = dr[20].ToString();

                string reversal = dr[24].ToString();
                if (reversal.Equals("1"))
                {
                    _TransactionStatus = TransactionStatus.Reversal;
                }
                else if (responseCode.Equals("0") && _TransactionStatus != TransactionStatus.Fail)
                {
                    _Transaction.ResponseCode = "0001";
                    _TransactionStatus = TransactionStatus.Fail;
                }

                if (dr.Table.Columns.Count > 32)
                {
                    string traceNo = dr[32].ToString();
                    if (!string.IsNullOrEmpty(traceNo))
                    {
                        _Transaction.TraceNo = traceNo;
                    }
                }

                _Transaction = GetTransaction(_Transaction, _TransactionType, _TransactionStatus, _TerminalType,
                    _CardType, _TerminalOwner, _NetworkType);
                //_Transaction.Source = _Source;
                //_Transaction.SubSource = _SubSource;

                Terminal _Terminal = Terminals.Find(GetTerminalByTerminalId(_Transaction.TerminalId));

                if (AquirBinId.Equals("402810") || AquirBinId.Equals("470555") || _Terminal != null)
                {
                    _Transaction.TerminalOwner = TerminalOwner.OwnTerminal;
                }

                if (_CardType == CardType.OwnCard && _TerminalType == TerminalType.POS &&
                    _TerminalOwner != TerminalOwner.OwnTerminal)
                {
                    _Transaction.TransactionAmount = billingAmt;
                }


                if (_Transaction.TerminalOwner != TerminalOwner.OwnTerminal && _Transaction.CardType == CardType.OwnCard
                && string.IsNullOrEmpty(_Transaction.AccountNo) &&
                _Transaction.TransactionStatus != TransactionStatus.Reversal &&
                !_Transaction.ResponseCode.Equals("0903"))
                {
                    return null;
                }

                if (_Transaction.CardNo.Substring(0, 6).Equals("402064")) //credit card bin no..//set default receivable in case of credit card...
                {
                    _Transaction.CardType = CardType.CreditCard;   // off us  or recievable..
                }

                /* for transaction amount  credit card is also a own card*/
                if ((_Transaction.CardType.Equals(CardType.OwnCard) ||
                     _Transaction.CardType.Equals(CardType.CreditCard)) &&
                    _Transaction.TerminalOwner.Equals(TerminalOwner.OwnTerminal))
                {
                    _Transaction.TransactionAmount = _Transaction.TransactionAmount;
                }
                else if ((_Transaction.CardType.Equals(CardType.OwnCard) ||
                          _Transaction.CardType.Equals(CardType.CreditCard)) &&
                   !_Transaction.TerminalOwner.Equals(TerminalOwner.OwnTerminal))
                {
                    if (_TerminalType == TerminalType.ATM)
                    {
                        if (currencyCode.Equals("356"))
                        {
                            _Transaction.TransactionAmount =
                                Convert.ToDecimal(
                                    Convert.ToDouble(_Transaction.TransactionAmount) * 1.6015 + 250);

                            _Transaction.TransactionAmount = Math.Ceiling(_Transaction.TransactionAmount * 100) / 100;
                        }
                        else
                        {
                            _Transaction.TransactionAmount = billingAmt;
                        }
                    }
                    else if (_TerminalType == TerminalType.POS)
                    {
                        if (currencyCode.Equals("356"))
                        {
                            _Transaction.TransactionAmount =
                                Convert.ToDecimal(Convert.ToDouble(_Transaction.TransactionAmount) * 1.6);
                        }
                        else
                        {
                            _Transaction.TransactionAmount = billingAmt;
                        }
                    }
                }

                // only for payable case..
                _Transaction.ResponseCodeDescription = responseCodeDescription;
                return _Transaction;
            }
            catch
            {
                throw;
            }
        }

        public void ReadDataFromFile(string FileName)
        {
            DataTable dt = new DataTable();
            try
            {
                ExcelFileReadingClass testCell = new ExcelFileReadingClass();
                dt = testCell.ReadExcelFileEPPUnmerged(FileName);
                GetTransactionList(dt, "xlsx");
            }
            catch
            {
                dt = DataReadFromExcel.ReadExcelFile(FileName);
                GetTransactionList(dt, "xls");
            }
        }

        private TransactionStatus GetTransactionStatus(string responseCode)
        {
            if (responseCode == null || responseCode.Length < 4)
            {
                return TransactionStatus.Fail;
            }
            else if (responseCode.Equals("0000"))
            {
                return TransactionStatus.Success;
            }
            else if (SUSPECTED_RESPONSE_CODES.Contains(responseCode))
            {
                return TransactionStatus.Success_With_Suspected;
            }
            //            else if (EXCEPTION_RESPONSE_CODES.Contains(responseCode))
            //            {
            //                return TransactionStatus.Exception;
            //            }
            else
            {
                return TransactionStatus.Fail;
            }
        }
        public override bool CheckDuplicateAndRemove()
        {
            Console.WriteLine("Finding and Replacing Duplicate Transactions!!");

            using (var context = new ReconContext())
            {
                var minTransactionDate = Transactions.Select(x => x.TransactionDate).DefaultIfEmpty().Min();
                var maxTransactionDate = Transactions.Select(x => x.TransactionDate).DefaultIfEmpty().Max();

                var existingTransactions = context.Transactions.Where(x =>
                    x.TransactionDate >= minTransactionDate &&
                    x.TransactionDate <= maxTransactionDate &&
                    x.Source_SourceId == _Source.SourceId).ToList();

                if (existingTransactions.Count == Transactions.Count)
                {
                    return false;
                }
                if (existingTransactions.Count > 0)
                {
                    var OldNepsTransactionsIds = existingTransactions.Select(t => t.TransactionId).ToList();

                    //finally delete from transaction..
                    int pageCountTransaction = 0;
                    RecursiveDeleteTransactionSqlForInClause(OldNepsTransactionsIds, ref pageCountTransaction); // delete dup transactions from transaction table..
                }
            }
            return true;
        }
    }
}

