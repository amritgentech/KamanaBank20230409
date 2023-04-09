using Db.Enum;
using Db.Model;
using ReadWriteFiles;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Db;
using Helper.GlobalHelpers;
using System.Text.RegularExpressions;
using System.Configuration;

//using Helper.GlobalHelper;

namespace ReconParser.App_Code.Recon.Npn
{
    public class NPN : Base
    {
        public static String[] SUSPECTED_RESPONSE_CODES = new string[]
        {
            "0034",
            "0059",
            "0090"
        };
        public static string AQUIRE_BIN_ID = ConfigurationManager.AppSettings["AQUIRE_BIN_ID"];
        public String[] CREDITCARD_BIN_NO
        {
            get
            {
                return ConfigurationManager.AppSettings["CREDITCARD_BIN_NO"].Split(',');
            }
        }
        public NPN(String FileName, int FileCount)
            : base(FileName, FileCount)
        {
            _Source = Source.FindName("NPN");
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

                    if (dr.Table.Columns["BIN"] != null && !dr.Table.Columns["CardNo"].ToString().Contains("Total"))
                    {

                        try
                        {
                            var TranDate = dr["TranDate"].ToString();
                        }
                        catch
                        {
                        }
                        ReconContext db = new ReconContext();
                        NPNSettlement ns = new NPNSettlement();
                        ns.BinNo = dr["BIN"].ToString();
                        ns.BankName = dr["BankName"].ToString();
                        ns.TranDate = dr["TranDate"].ToString();
                        ns.TerminalId = dr["TerminalId"].ToString();
                        ns.CardNo = dr["CardNo"].ToString();
                        ns.TraceNo = dr["TraceNo"].ToString();
                        ns.RRN = dr["RRN"].ToString();
                        if (dr.Table.Columns["TranAmt"] != null)
                            ns.TranAmt = Convert.ToDouble(dr["TranAmt"].ToString());
                        else if (dr.Table.Columns["TXN_AMT"] != null)
                            ns.TranAmt = Convert.ToDouble(dr["TXN_AMT"].ToString());
                        ns.Loro = dr["Loro"].ToString();
                        ns.AuthCode = dr["Auth Code"].ToString();
                        db.NPNSettlement.Add(ns);
                        db.SaveChanges();
                        // insert into npnsettlement
                    }

                    // Environment.Exit(0);
                    else
                    {
                        if (filetype.Equals("xls"))
                        {
                            _Transaction = GetEjournalAndNTDetailForNPN_xls(dr);
                        }
                        else
                        {
                            _Transaction = GetEjournalAndNTDetailForNPN_xlsx(dr);
                        }
                    }




                    if (_Transaction != null)
                    {
                        //ejournalDetail = GetEjournalAndNtDetailByCheckingLoro(ejournalDetail, binNo, listMemberBinNo);

                        Transactions.Add(_Transaction);
                    }
                }
                if (excelDataTable.Columns[0].ColumnName == "BIN")
                {
                    var context = new ReconContext();
                    var sourceName = context.Sources.Where(x => x.SourceId == _Source.SourceId).Select(x => x.Description).FirstOrDefault();
                    UploadedFile _UploadedFile = new UploadedFile();

                    var filesplit = FileName.Split('\\');
                    var stringcount = filesplit.Length - 1;


                    _UploadedFile.ShowFileName =

                        "settlementfile-" + DateTime.Now.ToString();


                    _UploadedFile.ActualFileName = FileName;
                    _UploadedFile.Catagory = sourceName;
                    _UploadedFile.SourceId = _Source.SourceId;
                    _UploadedFile.MinTransactionId = IdentityValueTransaction() == 0 ? 1 : IdentityValueTransaction() + 1;
                    _UploadedFile.MaxTransactionId = IdentityValueTransaction() + Transactions.Count;

                    if (sourceName.Equals("EJOURNAL"))
                        _UploadedFile.TerminalId = Transactions.Select(x => x.TerminalId).FirstOrDefault();

                    if (_SubSource != null)
                        _UploadedFile.SubSourceId = _SubSource.SubSourceId;

                    if (_SubChildSource != null)
                        _UploadedFile.SubChildSourceId = _SubChildSource.SubChildSourceId;

                    context.UploadedFiles.Add(_UploadedFile);
                    context.SaveChanges();
                    ReconProcessStatus.UpdateIsReconStarted("TRUE");
                    //Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected Transaction GetEjournalAndNTDetailForNPN_xlsx(DataRow dr)
        {
            return GetEjournalAndNTDetailForNPN_xls(dr);
        }

        protected Transaction GetEjournalAndNTDetailForNPN_xls(DataRow dr)
        {
            try
            {
                if (dr.Table.Columns["AUT_PRIM_ACCT_NUMB_F002"] != null)
                {
                    return GetEjournalAndNTDetailForNPNS2m_xls(dr);

                }
                else
                {

                    string AquirBinId = string.Empty;
                    if (dr.Table.Columns["ACQUIRER_INSTITUTION_CODE"] != null)
                    {
                        AquirBinId = dr["ACQUIRER_INSTITUTION_CODE"].ToString();
                    }
                    if (dr.Table.Columns["acquirer_bank"] != null)
                    {
                        if (dr["acquirer_bank"].ToString() == "555555")
                        {
                            return null;
                        }
                    }


                    TransactionType _TransactionType = TransactionType.BalInquiry;
                    TransactionStatus _TransactionStatus = TransactionStatus.Fail;
                    CardType _CardType = CardType.NotDefine;
                    NetworkType _NetworkType = NetworkType.VISA;
                    TerminalType _TerminalType = TerminalType.ATM;
                    TerminalOwner _TerminalOwner = TerminalOwner.NotDefine;

                    Transaction _Transaction = new Transaction();

                    if (dr.Table.Columns["issuing_bank"] != null)
                    {
                        if (dr["issuing_bank"].ToString() == "666666")
                        {
                            _Transaction.CardType = CardType.MasterCard;
                        }
                        _Transaction.Issuing_Bank = Convert.ToInt32(dr[17]);
                    }
                    //                string txnTypeCode = dr[1].ToString();
                    string txnType = dr[2].ToString();

                    if (txnType.Equals("Purchase") || txnType.Equals("Cash advance"))
                    {
                        _TransactionType = TransactionType.Financial;
                        _TerminalType = TerminalType.POS;
                    }
                    else if (txnType.Equals("CWD"))
                    {
                        _TransactionType = TransactionType.Financial;
                        _TerminalType = TerminalType.ATM;
                    }

                    _Transaction.CardNo = dr[0].ToString();

                    var txnDateTime = dr[4];
                    DateTime txnDate = Convert.ToDateTime(txnDateTime.ToString().Split(' ')[0]);
                    _Transaction.TransactionDate = txnDate;

                    _Transaction.TerminalId = dr[1].ToString();
                    _Transaction.TransactionAmount = Convert.ToDecimal(dr[5].ToString());
                    decimal billingAmt = Convert.ToDecimal(dr[7].ToString());


                    Terminal _Terminal = Terminals.Find(GetTerminalByTerminalId(_Transaction.TerminalId));

                    if (AquirBinId.Equals("408893") || _Terminal != null)
                    {
                        _Transaction.TerminalOwner = TerminalOwner.OwnTerminal;
                    }


                    string currencyCode = dr[6].ToString();
                    if (!currencyCode.Equals("356") && !currencyCode.Equals("524"))
                    {
                        _TerminalOwner = TerminalOwner.ForeignTerminal;
                        _Transaction.Currency = currencyCode;
                    }
                    else if (currencyCode.Equals("356"))
                    {
                        _TerminalOwner = TerminalOwner.INRTerminal;
                        _Transaction.Currency = currencyCode;
                    }
                    else
                    {
                        _Transaction.Currency = currencyCode;

                        _NetworkType = NetworkType.VISA;

                        if (txnType.Equals("Purchase") && _Terminal != null)
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
                    _Transaction.ReferenceNo = dr[11].ToString();

                    if (!string.IsNullOrEmpty(_Transaction.ReferenceNo))
                    {
                        try
                        {
                            _Transaction.TraceNo = _Transaction.ReferenceNo.Substring(_Transaction.ReferenceNo.Length - 6, 6);
                        }
                        catch (Exception ex)
                        {

                            throw;
                        }

                    }

                    _Transaction.AuthCode = dr[10].ToString();


                    string responseCode = dr[13].ToString();

                    _Transaction.ResponseCode = GetResponseCode(responseCode);

                    _TransactionStatus = GetTransactionStatus(_Transaction.ResponseCode);


                    string responseCodeDescription = dr[14].ToString();

                    string reversal = dr[3].ToString();
                    if (reversal.Equals("Reversal"))
                    {
                        _TransactionStatus = TransactionStatus.Reversal;
                    }

                    if (reversal.Equals("Advice"))
                    {
                        return null;
                    }



                    string traceNo = dr[9].ToString();
                    if (!string.IsNullOrEmpty(traceNo))
                    {
                        _Transaction.TraceNo = traceNo;
                    }

                    _Transaction.TransactionAmount = billingAmt;
                    _Transaction.ResponseCodeDescription = responseCodeDescription;

                    if (_Transaction.CardNo.Substring(0, 6).Equals("430728") || _Transaction.CardNo.Substring(0, 6).Equals("430724")) //credit card bin no..//set default receivable in case of credit card...
                    {
                        _Transaction.CardType = CardType.CreditCard;   // off us  or recievable..
                    }

                    _Transaction = GetTransaction(_Transaction, _TransactionType, _TransactionStatus, _TerminalType,
                        _Transaction.CardType, _TerminalOwner, _NetworkType);

                    return _Transaction;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        protected Transaction GetEjournalAndNTDetailForNPNS2m_xls(DataRow dr)
        {

            try
            {

                TransactionType _TransactionType = TransactionType.BalInquiry;
                TransactionStatus _TransactionStatus = TransactionStatus.Fail;
                CardType _CardType = CardType.NotDefine;
                NetworkType _NetworkType = NetworkType.VISA;
                TerminalType _TerminalType = TerminalType.ATM;
                TerminalOwner _TerminalOwner = TerminalOwner.NotDefine;

                Transaction _Transaction = new Transaction();

                string AquirBinId = string.Empty;
                if (dr.Table.Columns["AUT_ACQR_INST_ID_CODE_F032"] != null)
                {
                    AquirBinId = dr["AUT_ACQR_INST_ID_CODE_F032"].ToString();
                }
                if (dr.Table.Columns["AUT_ACQ_BANK_CODE"] != null)
                {
                    if (dr["AUT_ACQ_BANK_CODE"].ToString() == "555555")
                    {
                        return null;
                    }
                }

                if (dr.Table.Columns["AUT_ISSU_BANK_CODE"] != null)
                {
                    if (dr["AUT_ISSU_BANK_CODE"].ToString() == "666666")
                    {
                        _CardType = CardType.MasterCard;
                    }
                    _Transaction.Issuing_Bank = CheckNullInt(dr["AUT_ACQR_INST_ID_CODE_F032"].ToString());
                }

                string txnType = ""; /// 

                if (dr.Table.Columns["TCO_LABE"] != null)
                {
                    txnType = dr["TCO_LABE"].ToString();
                }
                else if (dr.Table.Columns["TTY_TYPE_LABE"] != null)
                {
                    txnType = dr["TTY_TYPE_LABE"].ToString();
                }
                // string txnType = dr["TTY_TYPE_LABE"].ToString(); /// use this or top one wherer withdrawal or Cash Disbursement(ATM)
                if (!string.IsNullOrEmpty(txnType))
                {
                    txnType = txnType.ToLower();
                }

                if (txnType.Equals("purchase") || txnType.Equals("cash advance"))
                {
                    _TransactionType = TransactionType.Financial;
                    _TerminalType = TerminalType.POS;
                }
                else if (txnType.Equals("cwd") || txnType.Equals("cash disbursement(atm)") || txnType.Equals("withdrawal"))
                {
                    _TransactionType = TransactionType.Financial;
                    _TerminalType = TerminalType.ATM;
                }

                _Transaction.CardNo = dr["AUT_PRIM_ACCT_NUMB_F002"].ToString();

                var txnDateTime = dr["AUT_REQU_SYST_TIME"].ToString();
                DateTime txnDate = Convert.ToDateTime(txnDateTime.ToString().Split(' ')[0]);
                _Transaction.TransactionDate = txnDate;

                _Transaction.TerminalId = dr["AUT_CARD_ACCP_TERM_ID_F041"].ToString();
                var amount = dr["AUT_TRAN_AMOU_F004"].ToString();
                //if (!string.IsNullOrEmpty(amount))
                //{
                _Transaction.TransactionAmount = CheckNullDecimal(amount);
                //}
                var bilamount = "";
                if (dr.Table.Columns["AUT_BILL_AMOU_F006"] != null)
                {
                    bilamount = dr["AUT_BILL_AMOU_F006"].ToString();
                }
                var billingAmt = CheckNullDecimal(bilamount);

                Terminal _Terminal = Terminals.Find(GetTerminalByTerminalId(_Transaction.TerminalId));

                if (AquirBinId.Equals(AQUIRE_BIN_ID) || _Terminal != null)
                {
                    _Transaction.TerminalOwner = TerminalOwner.OwnTerminal;
                }


                string currencyCode = "";
                if (dr.Table.Columns["AUT_TRAN_CURR_F049"] != null)
                {
                    currencyCode = dr["AUT_TRAN_CURR_F049"].ToString();

                    if (!currencyCode.Equals("356") && !currencyCode.Equals("524") && !string.IsNullOrEmpty(currencyCode))
                    {
                        _TerminalOwner = TerminalOwner.ForeignTerminal;
                        _Transaction.Currency = currencyCode;
                    }
                    else if (currencyCode.Equals("356"))
                    {
                        _TerminalOwner = TerminalOwner.INRTerminal;
                        _Transaction.Currency = currencyCode;
                    }

                    else
                    {
                        _Transaction.Currency = currencyCode;
                        _NetworkType = NetworkType.VISA;

                        if (txnType.Equals("purchase") && _Terminal != null)
                        {
                            _TerminalOwner = TerminalOwner.OwnTerminal;
                            _NetworkType = NetworkType.OTHER;
                        }
                        else if (txnType.Equals("purchase"))
                        {
                            _TerminalOwner = TerminalOwner.OffUsTerminal;
                            _NetworkType = NetworkType.VISA;
                        }

                    }

                }
                else
                {
                    currencyCode = dr["AUT_TRAN_AMOU_FEE_F028_2"].ToString();

                    if (!currencyCode.Equals("356") && !currencyCode.Equals("0") && !string.IsNullOrEmpty(currencyCode))
                    {
                        _TerminalOwner = TerminalOwner.ForeignTerminal;
                        _Transaction.Currency = currencyCode;
                    }
                    else if (currencyCode.Equals("356"))
                    {
                        _TerminalOwner = TerminalOwner.INRTerminal;
                        _Transaction.Currency = currencyCode;
                    }

                    else
                    {
                        _Transaction.Currency = currencyCode;
                        _NetworkType = NetworkType.VISA;

                        if (txnType.Equals("purchase") && _Terminal != null)
                        {
                            _TerminalOwner = TerminalOwner.OwnTerminal;
                            _NetworkType = NetworkType.OTHER;
                        }
                        else if (txnType.Equals("purchase"))
                        {
                            _TerminalOwner = TerminalOwner.OffUsTerminal;
                            _NetworkType = NetworkType.VISA;
                        }

                    }

                }
                _Transaction.ReferenceNo = dr["AUT_RETR_REF_NUMB_F037"].ToString();

                if (!string.IsNullOrEmpty(_Transaction.ReferenceNo))
                {

                    _Transaction.TraceNo = _Transaction.ReferenceNo.Substring(_Transaction.ReferenceNo.Length - 6, 6);
                }
                if (dr.Table.Columns["AUT_TRAN_CODE"] != null)
                {

                    _Transaction.AuthCode = dr["AUT_TRAN_CODE"].ToString();
                }


                string responseCode = dr["AUT_RESP_CODE_F039"].ToString();

                _Transaction.ResponseCode = GetResponseCode(responseCode);

                _TransactionStatus = GetTransactionStatus(_Transaction.ResponseCode);


                string responseCodeDescription = dr["ARE_LABE"].ToString();

                string reversal = dr["AUT_REVE_STAT"].ToString();
                if (reversal.Equals("Y"))
                {
                    _TransactionStatus = TransactionStatus.Reversal;
                }

                if (reversal.Equals("Advice"))
                {
                    return null;
                }


                string traceNo = dr["AUT_SYST_TRAC_AUDIT_NUMB_F011"].ToString();
                if (!string.IsNullOrEmpty(traceNo) && _Transaction.TraceNo == null)
                {
                    _Transaction.TraceNo = traceNo;
                }

                //_Transaction.TransactionAmount = billingAmt;
                _Transaction.ResponseCodeDescription = responseCodeDescription;

                if (_Transaction.CardNo.Substring(0, 6).Equals(CREDITCARD_BIN_NO)) //credit card bin no..//set default receivable in case of credit card...
                {
                    _CardType = CardType.CreditCard;   // off us  or recievable..
                }
                _Transaction  = GetTransaction(_Transaction, _TransactionType, _TransactionStatus, _TerminalType,
                        _Transaction.CardType, _TerminalOwner, _NetworkType);

                return _Transaction;
            }
            catch (Exception ex)
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

        //        public override bool CheckDuplicateAndRemove()
        //        {
        //            Console.WriteLine("Finding and Replacing Duplicate Transactions!!");
        //
        //            using (var context = new ReconContext())
        //            {
        //                var minTransactionDate = Transactions.Select(x => x.TransactionDate).DefaultIfEmpty().Min();
        //                var maxTransactionDate = Transactions.Select(x => x.TransactionDate).DefaultIfEmpty().Max();
        //
        //                var existingTransactions = context.Transactions.Where(x =>
        //                    x.TransactionDate >= minTransactionDate &&
        //                    x.TransactionDate <= maxTransactionDate &&
        //                    x.Source_SourceId == _Source.SourceId).ToList();
        //
        //                if (existingTransactions.Count == Transactions.Count)
        //                {
        //                    return false;
        //                }
        //                if (existingTransactions.Count > 0)
        //                {
        //                    var OldNPNTransactionsIds = existingTransactions.Select(t => t.TransactionId).ToList();
        //
        //                    //finally delete from transaction..
        //                    int pageCountTransaction = 0;
        //                    RecursiveDeleteTransactionSqlForInClause(OldNPNTransactionsIds, ref pageCountTransaction); // delete dup transactions from transaction table..
        //                }
        //            }
        //            return true;
        //        }
        public decimal CheckNullDecimal(string param)
        {
            var result = new decimal();
            if (!string.IsNullOrEmpty(param))
            {
                result = Convert.ToDecimal(param);
            }
            return result;
        }
        public int CheckNullInt(string param)
        {
            var result = 0;
            if (!string.IsNullOrEmpty(param))
            {
                result = Convert.ToInt32(param);
            }
            return result;
        }

    }
}

