using Db;
using Db.Enum;
using Db.Model;
using DbOperations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using BAL;
using Helper.GlobalHelpers;

namespace ReconParser.App_Code.Recon
{
    /// <summary>
    /// Add documentation
    /// </summary>
    public abstract class Base
    {
        public List<Transaction> Transactions;
        //public List<TransactionDetail> TransactionDetails;
        public SubChildSource _SubChildSource;
        public SubSource _SubSource;
        public Source _Source;
        public static List<Terminal> Terminals = Terminal.All();
        public static List<BankCardBinNo> BankCardBinNos = BankCardBinNo.All();
        public static List<MemberBankCardBinNo> MemberBankCardBinNos = MemberBankCardBinNo.All();
        public DbOperation _DbOperation { get; set; }

        public string FileName { get; set; }
        public int FileCount { get; set; }

        public Base(string FileName, int FileCount)
        {
            this.FileName = FileName;
            this.FileCount = FileCount;
            Transactions = new List<Transaction>();
            _DbOperation = new DbOperation("ReconContextConnectionString");
        }

        public void Start()
        {
            try
            {
                ReconProcessStatus.UpdateIsReconStarted("True");
                Parse();
                Filter();
                if (CheckDuplicateAndRemove())
                {
                    Save();
                }
                Backup();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Occured in Recon: " + e.Message);
                ReconProcessStatus.UpdateIsReconStarted("Error");
                Environment.Exit(0);
            }
            finally
            {
                if (FileCount == 1)  //if the file count is last count then .. exit..
                {
                    ReconProcessStatus.UpdateIsReconStarted("False");
                    Environment.Exit(0);
                }
            }
            return;
        }

        public virtual void Save()
        {
            if (Transactions.Count == 0)
            {
                Console.WriteLine("Data not Found");
                return;
            }
            Console.WriteLine("Data Saving Start......");

            try
            {
                using (var context = new ReconContext())
                {
                    //upload file info saved..
                    var sourceName = context.Sources.Where(x => x.SourceId == _Source.SourceId).Select(x => x.Description).FirstOrDefault();
                    UploadedFile _UploadedFile = new UploadedFile();

                    var filesplit = FileName.Split('\\');
                    var stringcount = filesplit.Length - 1;


                    _UploadedFile.ShowFileName =

                        (Transactions.Select(x => x.AdviseDate).FirstOrDefault() != null
                            ? Convert.ToDateTime(Transactions.Select(x => x.AdviseDate).FirstOrDefault())
                                .ToString("dd MMMM, yyyy")
                            : Transactions.Select(x => x.TransactionDate).Min() ==
                              Transactions.Select(x => x.TransactionDate).Max()
                                ? Transactions.Select(x => x.TransactionDate).Min().ToString("dd MMMM, yyyy")
                                : Transactions.Select(x => x.TransactionDate).Min().ToString("dd MMMM, yyyy") +
                                  "-----" +
                                  Transactions.Select(x => x.TransactionDate).Max().ToString("dd MMMM, yyyy"))
                        + "-----" + Regex.Split(filesplit[stringcount], "Append")[0];


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


                    // prepare data for transaction bulk insert..
                    List<Transaction> listTransaction = new List<Transaction>();

                    try
                    {

                        foreach (var txn in Transactions)
                        {
                            txn.TransactionDate = txn.TransactionDate.Date;
                            txn.Source_SourceId = _Source.SourceId;

                            if (_SubSource != null)
                                txn.SubSource_SubSourceId = _SubSource.SubSourceId;
                            //                        txn.UploadedFile_UploadedFileId = IdentityValueUploadfile() + 1;  //increment identity for upcoming files
                            txn.UploadedFile_UploadedFileId = _UploadedFile.UploadedFileId;
                            listTransaction.Add(txn);
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                    _DbOperation.BulkInsert(listTransaction);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Finished process file. Saved {0} Transactions.", Transactions.Count);
        }

        public int IdentityValueTransaction()
        {
            String sql = "select IDENT_CURRENT('Transactions')";
            return Convert.ToInt32(_DbOperation.ExecuteScalar(sql));
        }

        public virtual void DataReadFromDatabase() { }

        public virtual void ProcessRecon()
        {

        }

        public virtual void Parse()
        {
        }

        public virtual void Filter()
        {
            Transactions = Transactions.Where(x => x != null).ToList();
        }

        public void Backup()
        {
        }
        public virtual bool CheckDuplicateAndRemove()
        {
            //if (Transactions.Count <= 0)
            //{
            //    return false;
            //}
            Console.WriteLine("Finding and Replacing Duplicate Transactions!!");

            using (var context = new ReconContext())
            {
                var minTransactionDate = Transactions.Select(x => x.TransactionDate).Min().Date;
                var maxTransactionDate = Transactions.Select(x => x.TransactionDate).Max().Date;


                var oldtxn = context.Transactions.Where(x =>
                                        x.Source_SourceId == _Source.SourceId &&
                                        x.TransactionDate >= minTransactionDate &&
                                        x.TransactionDate <= maxTransactionDate).ToList();

                var duplicateTransactions = (from t1 in Transactions // new neps transactions..
                                             join t2 in oldtxn //already saved neps transactions..
                                             on
                                             new
                                             {
                                                 //                                                 AccountNo = GlobalHelper.NullHelperString(t1.AccountNo),
                                                 CardNo = GlobalHelper.NullHelperString(t1.CardNo),
                                                 AuthCode = GlobalHelper.NullHelperString(t1.AuthCode),
                                                 ResponseCode = GlobalHelper.NullHelperString(t1.ResponseCode),
                                                 //                                                 ResponseCodeDescription = GlobalHelper.NullHelperString(t1.ResponseCodeDescription),
                                                 TransactionNo = GlobalHelper.NullHelperString(t1.TransactionNo),
                                                 ReferenceNo = GlobalHelper.NullHelperString(t1.ReferenceNo),
                                                 TerminalId = GlobalHelper.NullHelperString(t1.TerminalId),
                                                 //                                                 CBSRefValue = GlobalHelper.NullHelperString(t1.CBSRefValue),
                                                 TransactionAmount = GlobalHelper.NullHelperDecimal(t1.TransactionAmount),
                                                 Currency = GlobalHelper.NullHelperString(t1.Currency),
                                                 //                                                 AdviseDate = GlobalHelper.NullHelperDate(t1.AdviseDate),
                                                 TraceNo = GlobalHelper.NullHelperString(t1.TraceNo),
                                                 TransactionDate = GlobalHelper.NullHelperDate(t1.TransactionDate),
                                                 t1.TransactionStatus
                                                 //                                                 t1.Source_SourceId
                                             }
                                             equals
                                             new
                                             {
                                                 //                                                 AccountNo = GlobalHelper.NullHelperString(t2.AccountNo),
                                                 CardNo = GlobalHelper.NullHelperString(t2.CardNo),
                                                 AuthCode = GlobalHelper.NullHelperString(t2.AuthCode),
                                                 ResponseCode = GlobalHelper.NullHelperString(t2.ResponseCode),
                                                 //                                                 ResponseCodeDescription = GlobalHelper.NullHelperString(t2.ResponseCodeDescription),
                                                 TransactionNo = GlobalHelper.NullHelperString(t2.TransactionNo),
                                                 ReferenceNo = GlobalHelper.NullHelperString(t2.ReferenceNo),
                                                 TerminalId = GlobalHelper.NullHelperString(t2.TerminalId),
                                                 //                                                 CBSRefValue = GlobalHelper.NullHelperString(t2.CBSRefValue),
                                                 TransactionAmount = GlobalHelper.NullHelperDecimal(t2.TransactionAmount),
                                                 Currency = GlobalHelper.NullHelperString(t2.Currency),
                                                 //                                                 AdviseDate = GlobalHelper.NullHelperDate(t2.AdviseDate),
                                                 TraceNo = GlobalHelper.NullHelperString(t2.TraceNo),
                                                 TransactionDate = GlobalHelper.NullHelperDate(t2.TransactionDate),
                                                 t2.TransactionStatus
                                                 //                                                 t2.Source_SourceId
                                             }
                                             //                                             select new { OldNepsTransactions = t2, NewNepsTransactions = t1 })
                                             select t2).Distinct()
                    //                                        .Where(x =>
                    //                                                x.OldNepsTransactions.Source_SourceId == _Source.SourceId &&
                    //                                                x.OldNepsTransactions.TransactionDate >= minTransactionDate &&
                    //                                                x.OldNepsTransactions.TransactionDate <= maxTransactionDate
                    //                                        )
                    .ToList();

                if (duplicateTransactions.Count == Transactions.Count)
                {
                    return false;
                }
                if (duplicateTransactions.Count > 0)
                {
                    //                    var OldNepsTransactionsIds = duplicateTransactions.Select(t => t.OldNepsTransactions.TransactionId).ToList();
                    var OldNepsTransactionsIds = duplicateTransactions.Select(t => t.TransactionId).ToList();

                    //finally delete from transaction..
                    int pageCountTransaction = 0;
                    RecursiveDeleteTransactionSqlForInClause(OldNepsTransactionsIds, ref pageCountTransaction); // delete dup transactions from transaction table..
                }
            }
            return true;
        }
        public static bool JoinCaseConditions(Transaction SourceTransaction, Transaction DestinationTransaction)
        {
            if (SourceTransaction == null || DestinationTransaction == null)
                return false;

            if (SourceTransaction.TraceNo != null && DestinationTransaction.TraceNo != null)
            {
                return SourceTransaction.TraceNo == DestinationTransaction.TraceNo;
            }
            else if (SourceTransaction.ReferenceNo != null && DestinationTransaction.ReferenceNo != null)
            {
                return SourceTransaction.ReferenceNo == DestinationTransaction.ReferenceNo;
            }
            else if (SourceTransaction.AuthCode != null && DestinationTransaction.AuthCode != null)
            {
                return SourceTransaction.AuthCode == DestinationTransaction.AuthCode;
            }
            return false;
        }

        public virtual int RecursiveDeleteTransactionSqlForInClause(List<int> lst, ref int pagecount)
        {
            int page = pagecount;
            int pagesize = 1000;
            var listIds = GlobalHelper.GetPage(lst, page, pagesize);
            pagecount++;
            if (listIds.Count == 0)
            {
                return 0;
            }
            String csvTransactionIdArray = String.Join(",", listIds);
            String sql = "Delete from Transactions where TransactionId in (" + csvTransactionIdArray + ")";

            _DbOperation.ExecuteNonQuery(sql);

            return RecursiveDeleteTransactionSqlForInClause(lst, ref pagecount);
        }

        public string GetResponseCode(string responseCode)
        {
            if (responseCode.Length == 4)
            {
                return responseCode;
            }
            else if (responseCode.Length == 3)
            {
                return "0" + responseCode;
            }
            else if (responseCode.Length == 2)
            {
                return "00" + responseCode;
            }
            else if (responseCode.Length == 1)
            {
                return "000" + responseCode;
            }
            else
            {
                return responseCode;
            }
        }

        public string getNumber(string[] arrayStr, int endIndex)
        {
            string str = "";
            for (int i = 2; i < endIndex; i++)
            {
                str += arrayStr[i];
            }
            return str;
        }

        public Transaction GetTransaction(Transaction _Transaction, TransactionType _TransactionType,
            TransactionStatus _TransactionStatus, TerminalType _TerminalType)
        {
            if (_TransactionType > TransactionType.NotDefine)
            {
                _Transaction.TransactionType = _TransactionType;
            }

            if (_TransactionStatus > TransactionStatus.NotDefine)
            {
                _Transaction.TransactionStatus = _TransactionStatus;
            }

            if (_TerminalType > TerminalType.NotDefine)
            {
                _Transaction.TerminalType = _TerminalType;
            }

            if (string.IsNullOrEmpty(_Transaction.CardNo))
            {
                throw new Exception("Uploaded File doesnot contain the CARDNO of TRACE NO: " + _Transaction.TraceNo);
            }

            BankCardBinNo _BankCardBinNo = CheckCardNoIsOnUs(_Transaction.CardNo);
            if (_BankCardBinNo != null)
            {
                _Transaction.CardType = CardType.OwnCard;
                _Transaction.NetworkType = _BankCardBinNo.NetworkType;
            }
            else
            {
                MemberBankCardBinNo _MemberBankCardBinNo = CheckMemberBankCardNo(_Transaction.CardNo);
                if (_MemberBankCardBinNo != null)
                {
                    _Transaction.CardType = CardType.OffUsCard;
                    _Transaction.NetworkType = _MemberBankCardBinNo.NetworkType;
                }
                else
                {
                    _Transaction.CardType = CardType.ForeignCard;
                    _Transaction.NetworkType = NetworkType.VISA;
                }
            }

            Terminal _Terminal = Terminals.Find(GetTerminalByTerminalId(_Transaction.TerminalId));
            if (_Terminal != null)
            {
                _Transaction.TerminalOwner = TerminalOwner.OwnTerminal;
            }
            else
            {
                _Transaction.TerminalOwner = TerminalOwner.ForeignTerminal;
            }
            return _Transaction;
        }

        public Transaction GetTransaction(Transaction _Transaction, TransactionType _TransactionType,
            TransactionStatus _TransactionStatus, TerminalType _TerminalType, CardType _CardType, TerminalOwner _TerminalOwner, NetworkType _NetworkType)
        {
            if (_TransactionType > TransactionType.NotDefine)
            {
                _Transaction.TransactionType = _TransactionType;
            }

            if (_TransactionStatus > TransactionStatus.NotDefine)
            {
                _Transaction.TransactionStatus = _TransactionStatus;
            }

            if (_TerminalType > TerminalType.NotDefine)
            {
                _Transaction.TerminalType = _TerminalType;
            }



            BankCardBinNo _BankCardBinNo = CheckCardNoIsOnUs(_Transaction.CardNo);
            if (_BankCardBinNo != null)
            {
                _Transaction.CardType = CardType.OwnCard;
            }
            else
            {
                MemberBankCardBinNo _MemberBankCardBinNo = CheckMemberBankCardNo(_Transaction.CardNo);
                if (_MemberBankCardBinNo != null)
                {
                    _Transaction.CardType = CardType.OffUsCard;
                }
                else if (_CardType > CardType.NotDefine)
                {
                    _Transaction.CardType = _CardType;
                }
                else if (_CardType == CardType.MasterCard)
                {
                    _Transaction.CardType = _CardType;
                }
                else
                {
                    _Transaction.CardType = CardType.ForeignCard;
                }
            }

            if (_NetworkType > NetworkType.NotDefine)
            {
                _Transaction.NetworkType = _NetworkType;
            }

            Terminal _Terminal = Terminals.Find(GetTerminalByTerminalId(_Transaction.TerminalId));
            if (_Terminal != null)
            {
                _Transaction.TerminalOwner = TerminalOwner.OwnTerminal;
            }
            else if (_TerminalOwner == TerminalOwner.NotDefine)
            {
                _Transaction.TerminalOwner = TerminalOwner.ForeignTerminal;
            }
            else
            {
                _Transaction.TerminalOwner = _TerminalOwner;
            }

            return _Transaction;
        }

        static Predicate<BankCardBinNo> GetBankBinNoByBinNo(string binNO)
        {
            return delegate (BankCardBinNo _BankCardBinNo) { return _BankCardBinNo.BinNo == binNO; };
        }

        static Predicate<MemberBankCardBinNo> GetMemberBankBinNoByBinNo(string binNo)
        {
            return MemberBankCardBinNo => MemberBankCardBinNo.BinNo == binNo;
        }

        public static Predicate<Terminal> GetTerminalByTerminalId(string terminalId)
        {
            return Terminal => Terminal.TerminalId == terminalId;
        }

        private BankCardBinNo CheckCardNoIsOnUs(string cardNo)
        {
            string subStringBinNo = cardNo.Substring(0, 6);
            return BankCardBinNos.Find(GetBankBinNoByBinNo(subStringBinNo));
        }

        private MemberBankCardBinNo CheckMemberBankCardNo(string cardNo)
        {
            string subStringBinNo = cardNo.Substring(0, 6);
            return MemberBankCardBinNos.Find(GetMemberBankBinNoByBinNo(subStringBinNo));
        }
    }
}
