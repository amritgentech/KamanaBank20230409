using Db.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Model
{
    public class Transaction : Base
    {
        [Key]
        [Index("TXN_INDEX_TransactionId")]
        public int TransactionId { get; set; }

        private String _cardNo;
        public String CardNo
        {
            get
            {
                return !string.IsNullOrEmpty(_cardNo)?_cardNo.Substring(0, 6) + "XXXXXX" + _cardNo.Substring(_cardNo.Length - 4, 4):string.Empty;

            }
            set { this._cardNo = value; }
        }
        public String AuthCode { get; set; }
        public String ResponseCode { get; set; }
        public String ResponseCodeDescription { get; set; }
        public String TransactionNo { get; set; }
        public String TraceNo { get; set; }
//        private String _referenceNo;
//        public String ReferenceNo
//        {
//            get
//            {
//                return String.IsNullOrEmpty(_referenceNo) ? string.Empty : _referenceNo.Substring(_referenceNo.Length - 6, 6);
//
//            }
//            set { this._referenceNo = value; }
//        }
        public String ReferenceNo { get; set; }
        public String TerminalId { get; set; }
        public String UtrNo { get; set; }

        [Index("TXN_INDEX_TransactionDate")]
        public DateTime TransactionDate { get; set; }
        public DateTime? GmtToLocalTransactionDate { get; set; }
        public Decimal TransactionAmount { get; set; }
        public Decimal SourceTransactionAmount { get; set; }
        public Decimal DestinationTransactionAmount { get; set; }
        public TimeSpan TransactionTime { get; set; }
        public String ProcessingCode { get; set; }
        public DateTime? AdviseDate { set; get; }
        public string AccountNo { set; get; }
        public string Currency { set; get; }
        public DateTime? CBSValueDate { set; get; }
        public string CBSRefValue { set; get; }
        public TransactionType TransactionType { set; get; }
        public CardType CardType { set; get; }
        public TerminalOwner TerminalOwner { set; get; }
        [Index("TXN_INDEX_TransactionStatus")]
        public TransactionStatus TransactionStatus { set; get; }
        public TerminalType TerminalType { set; get; }
        public NetworkType NetworkType { set; get; }
        public bool ApplicationGenerated { set; get; }
        public string Recon_status { set; get; }
        public string MainCode { set; get; } 
        public int Source_SourceId { set; get; }
        public int SubSource_SubSourceId { get; set; }
        public int UploadedFile_UploadedFileId { get; set; }
        public int Issuing_Bank { get; set; }
        [ForeignKey("Source_SourceId")]
        public virtual Source Source { get; set; }
        [ForeignKey("SubSource_SubSourceId")]
        public virtual SubSource SubSource { get; set; }
        public virtual List<CashLeaf> CashLeaves { get; set; }
        [ForeignKey("UploadedFile_UploadedFileId")]
        public virtual UploadedFile UploadedFile { get; set; }

        public bool Invalid { get; set; }

        public static Transaction Find(int transactionId)
        {
            Transaction transaction = FindTransaction(t => t.TransactionId == transactionId);
            return transaction;
        }

        public static Transaction Find_by_TraceNo(String traceNo)
        {
            Transaction transaction = FindTransaction(t => t.TraceNo == traceNo);
            return transaction;
        }

        public static int Create(Transaction transaction)
        {
            Transaction _Transaction = null;
            using (var context = new ReconContext())
            {
                _Transaction = context.Transactions.Add(transaction);
                context.SaveChanges();
            }
            return _Transaction.TransactionId;
        }

        public override string ToString()
        {
            return String.Format("TerminalId: {0} + CreatedAt: {1} ", this.TransactionId, this.CreatedAt);
        }

        public bool AssociatedTransaction(Transaction _Transaction)
        {
            AuthCode = string.IsNullOrEmpty(AuthCode) ? string.Empty : AuthCode;
            _Transaction.AuthCode = string.IsNullOrEmpty(_Transaction.AuthCode) ? string.Empty : _Transaction.AuthCode;

            try
            {
                return TransactionDate.Equals(_Transaction.TransactionDate) &&
                TerminalId.Equals(_Transaction.TerminalId) &&
                MaskedCardNumber().Equals(_Transaction.MaskedCardNumber()) &&
                (TraceNo.Equals(_Transaction.TraceNo) ||
                    AuthCode.Equals(_Transaction.AuthCode) ||
                    ReferenceNo.Equals(_Transaction.ReferenceNo)
                ) &&
                ResponseCode.Equals(_Transaction.ResponseCode) &&
                CardType.Equals(_Transaction.CardType) &&
                TransactionType.Equals(_Transaction.TransactionType);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DateDiffAssociatedTransaction(Transaction _Transaction)
        {
            AuthCode = string.IsNullOrEmpty(AuthCode) ? string.Empty : AuthCode;
            _Transaction.AuthCode = string.IsNullOrEmpty(_Transaction.AuthCode) ? string.Empty : _Transaction.AuthCode;

            return _Transaction.TransactionDate >= TransactionDate.AddDays(-1) && _Transaction.TransactionDate <= TransactionDate.AddDays(1) &&
                TerminalId.Equals(_Transaction.TerminalId) &&
                MaskedCardNumber().Equals(_Transaction.MaskedCardNumber()) &&
                (TraceNo.Equals(_Transaction.TraceNo) ||
                    AuthCode.Equals(_Transaction.AuthCode) ||
                    ReferenceNo.Equals(_Transaction.ReferenceNo)
                ) &&
                ResponseCode.Equals(_Transaction.ResponseCode) &&
                CardType.Equals(_Transaction.CardType) &&
                TransactionType.Equals(_Transaction.TransactionType);
        }

        public bool Matched(Transaction _Transaction)
        {
            return AssociatedTransaction(_Transaction) &&
                TransactionStatus.Equals(_Transaction.TransactionStatus);
        }

        public bool MatchedTerminalId(Transaction _Transaction)
        {
            return TerminalId.Equals(_Transaction.TerminalId);
        }

        public bool MatchedTransactionStatus(Transaction _Transaction)
        {
            return TransactionStatus.Equals(_Transaction.TransactionStatus);
        }

        public bool DateDiffMatched(Transaction _Transaction)
        {
            return AssociatedTransaction(_Transaction) &&
                TransactionStatus.Equals(_Transaction.TransactionStatus);
        }

        public String MaskedCardNumber()
        {
            return CardNo.Substring(0, 6) + "XXXXXX" + CardNo.Substring(CardNo.Length - 4, 4);
        }

        public bool IsInvalid()
        {
            bool invalid = Invalid || TransactionList(t => t.TransactionId != TransactionId &&
                t.TransactionDate == TransactionDate &&
                (
                    (TraceNo.Equals(t.TraceNo) ||
                        AuthCode.Equals(t.AuthCode) ||
                        ReferenceNo.Equals(t.ReferenceNo)
                    )
                ) && t.MaskedCardNumber() == MaskedCardNumber()
            ).Count == 0;

            Invalid = invalid;
            using (var context = new ReconContext())
            {
                context.SaveChanges();
            }
            return Invalid;
        }

        public static bool IsPresent(Transaction _Transaction)
        {
            if (_Transaction == null)
            {
                return false;
            }
            return (_Transaction != null) && _Transaction.TransactionId > 0;
        }

        public Transaction GetReversalTransaction()
        {
            DateTime TransactionDateFrom = TransactionDate.AddDays(-1);
            DateTime TransactionDateTo = TransactionDate.AddDays(1);
            List<Transaction> Transactions = TransactionList(t => t.TransactionId != TransactionId &&
            t.Source.SourceId == Source.SourceId &&
                (
                    (string.IsNullOrEmpty(TraceNo) ? false : TraceNo.Equals(string.IsNullOrEmpty(t.TraceNo) ? false : TraceNo.Equals(t.TraceNo)) ||
                      string.IsNullOrEmpty(AuthCode) ? false : AuthCode.Equals(string.IsNullOrEmpty(t.AuthCode) ? false : AuthCode.Equals(t.AuthCode)) ||
                      string.IsNullOrEmpty(ReferenceNo) ? false : ReferenceNo.Equals(string.IsNullOrEmpty(t.ReferenceNo) ? false : ReferenceNo.Equals(t.ReferenceNo))
                     )
                ) && t.CardNo == CardNo &&
                t.TransactionDate >= TransactionDateFrom && t.TransactionDate <= TransactionDateTo
            );

            Transaction _ReversalTransaction = Transactions.Find(GetReversalTransaction(this));
            return _ReversalTransaction;
        }

        public static Predicate<Transaction> GetReversalTransaction(Transaction _Transaction1)
        {
            return delegate (Transaction _Transaction2)
            {
                return _Transaction1.CardType.Equals(_Transaction2.CardType)
                    & _Transaction1.TransactionType.Equals(_Transaction2.TransactionType)
                    & !_Transaction1.TransactionStatus.Equals(TransactionStatus.Reversal) &
                    _Transaction2.TransactionStatus.Equals(TransactionStatus.Reversal);
            };
        }

        public static Transaction FindTransaction(Func<Transaction, bool> where)
        {
            Transaction transaction = null;
            using (var context = new ReconContext())
            {
                transaction = context.Transactions.First(where);
            }
            return transaction;
        }

        public static List<Transaction> Where(Func<Transaction, bool> where)
        {
            List<Transaction> transactions = null;
            using (var context = new ReconContext())
            {
                transactions = context.Transactions.Where(where).ToList();
            }
            return transactions;
        }

        public List<Transaction> TransactionList(Func<Transaction, bool> where)
        {
            List<Transaction> transactions = null;
            using (var context = new ReconContext())
            {
                transactions = context.Transactions.Where(where).ToList();
            }
            return transactions;
        }

        public static String MaxTransactionDate()
        {
            String maxDate = null;
            using (var context = new ReconContext())
            {
                Transaction transaction = context.Transactions.OrderByDescending(x => x.TransactionDate).FirstOrDefault();
                if (transaction == null)
                {
                    maxDate = DateTime.Now.ToString("dd/MM/yyyy");
                }
                else
                {
                    maxDate = transaction.TransactionDate.ToString("dd/MM/yyyy");
                }
            }
            return maxDate;
        }

    }
}
