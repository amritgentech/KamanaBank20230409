using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Db.Enum;
using Db.Model;
using DbOperations;
using Helper.GlobalHelpers;

namespace BAL.App_Code
{
    public class ConvertReversalsToSingle : Base
    {
        private List<Transaction> Transactions = null;
        private DbOperation _DbOperation = null;
        public ConvertReversalsToSingle(List<Transaction> Transactions)
        {
            this.Transactions = Transactions;
            _DbOperation = new DbOperation();
        }

        public void ConvertReversalsToSingleTransactions()
        {
            ConvertReversalTransactionsToSingleTransactions(this.Transactions, TransactionStatus.Reversal); //neps..
        }

        public void ConvertReversalTransactionsToSingleTransactions(List<Transaction> transactions, TransactionStatus reversalCode)
        {
            Console.Write("Converting Transactions Reversals into Single Transaction");

            List<Transaction> notReversalTransactions =
                transactions.Where(s => s.TransactionStatus != reversalCode).ToList();

            List<Transaction> reversalTransactions =
                transactions.Where(s => s.TransactionStatus == reversalCode).ToList();

            if (reversalTransactions.Count < 1)
            {
                return;
            }
            List<Transaction> OtherReversalTransactions =
                (from t1 in notReversalTransactions
                 join t2 in reversalTransactions
                 on
                 new
                 {
                     t1.TerminalId,
                     t1.TransactionDate,
                     card = t1.CardNo
                 } equals
                 new
                 {
                     t2.TerminalId,
                     t2.TransactionDate,
                     card = t2.CardNo
                 }
                 select new { t1, t2 }).
                Where(x => x.t1.TraceNo == x.t2.TraceNo || (x.t1.TerminalType.Equals(TerminalType.POS) && x.t1.AuthCode == x.t2.AuthCode))
                .Select(x => x.t1).ToList();

            if (OtherReversalTransactions.Count < 1)
            {
                return;
            }

            var TransactionIdArray = OtherReversalTransactions.Select(t => t.TransactionId).ToArray();
            if (TransactionIdArray.Length < 1)
            {
                return;
            }

            int pageCountForUpdate = 0;
            RecursiveTransactionStatusUpdateSqlForInClause(TransactionIdArray.ToList(), (int)TransactionStatus.Transaction_With_Reversal, ref pageCountForUpdate);

            var reversalIdArray = reversalTransactions.Select(t => t.TransactionId).ToArray();
            if (reversalIdArray.Length < 1)
            {
                return;
            }

            int pageCount = 0;
            RecursiveDeleteSqlForInClause(reversalIdArray.ToList(), ref pageCount);

            Console.Write("CONVERTED!! TransactionReversals into Single Transaction");
        }

        public virtual int RecursiveTransactionStatusUpdateSqlForInClause(List<int> lst, int transactionStatus, ref int pagecount)
        {
            int page = pagecount;
            int pagesize = 1000;
            var listIds = GlobalHelper.GetPage(lst, page, pagesize);
            pagecount++;
            if (listIds.Count == 0)
            {
                return lst.Count;
            }
            String csvTransactionIdArray = String.Join(",", listIds);
            String sql = "update Transactions set TransactionStatus = " + transactionStatus +
                         " where TransactionId in (" + csvTransactionIdArray + ")";

            _DbOperation.ExecuteNonQuery(sql);

            return RecursiveTransactionStatusUpdateSqlForInClause(lst, transactionStatus, ref pagecount);
        }
        public virtual int RecursiveDeleteSqlForInClause(List<int> lst, ref int pagecount)
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

            return RecursiveDeleteSqlForInClause(lst, ref pagecount);
        }
    }
}
