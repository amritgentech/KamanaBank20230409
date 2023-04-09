using DbOperations;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Db;
using Db.Model;
using Quartz;

namespace CbsDataSyncer.App_Code
{
    public class CbsSyncer : IJob
    {
        private DbOperation _DbOperation = new DbOperation("CBS_ORACLE");
        private static String Query
        {
            get
            {
                return ConfigurationManager.AppSettings["Query"];
            }
        }
        private static String ViewName
        {
            get
            {
                return ConfigurationManager.AppSettings["ViewName"];
            }
        }
        public CbsSyncer()
        {

        }
        public List<Transaction> TransactionsFromQuery(String transactionDate)
        {
            string newQuery = string.Empty;
            DataTable TransactionsDataTable = null;
            if (string.IsNullOrEmpty(Query))
                newQuery = string.Format(
                    "select * from \"{0}\" where \"Dob\" > \"TO_DATE\"('{1}','YYYY/MM/DD')",
                    ViewName, transactionDate);

            //            Parameter _Parameter = new Parameter("@Date", transactionDate.Split(' ')[0]);

            if (!string.IsNullOrEmpty(Query))
                TransactionsDataTable = _DbOperation.ExecuteDataTable(Query);
            else
                TransactionsDataTable = _DbOperation.ExecuteDataTable(newQuery);

            List<Transaction> Transactions = new List<Transaction>();
            foreach (DataRow row in TransactionsDataTable.Rows)
            {
                Transaction _Transaction = new Transaction
                {
                    CardNo = row["CardNo"].ToString(),
                    AuthCode = row["AuthCode"].ToString(),
                    ResponseCode = row["ResponseCode"].ToString(),
                    TransactionNo = row["TransactionNo"].ToString(),
                    TraceNo = row["TraceNo"].ToString(),
                    ReferenceNo = row["ReferenceNo"].ToString(),
                    TerminalId = row["TerminalId"].ToString(),
                    TransactionDate = Convert.ToDateTime(row["TransactionDate"].ToString()),
                    TransactionTime = Convert.ToDateTime(row["TransactionTime"].ToString()).TimeOfDay,
                    ProcessingCode = row["ProcessingCode"].ToString(),
                    AdviseDate = !String.IsNullOrEmpty(row["AdviseDate"].ToString()) ? Convert.ToDateTime(row["AdviseDate"].ToString()) : (DateTime?)null,
                    AccountNo = row["AccountNo"].ToString(),
                    Currency = row["Currency"].ToString(),
                    CBSValueDate = !String.IsNullOrEmpty(row["CBSValueDate"].ToString()) ? Convert.ToDateTime(row["CBSValueDate"].ToString()) : (DateTime?)null,
                    CBSRefValue = row["CBSRefValue"].ToString(),
                    Invalid = false,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                _Transaction.Source = Source.CBS();
                Transactions.Add(_Transaction);
            }
            return Transactions;
        }

        public void Sync()
        {
            Console.WriteLine("Starting Syncer");
            using (var context = new ReconContext())
            {
                String maxdate = context.Transactions
                    .OrderByDescending(t => t.TransactionDate)
                    .Select(x => x.TransactionDate).FirstOrDefault().ToString("yyyy-MM-dd");
                List<Transaction> Transactions = TransactionsFromQuery(maxdate);
                foreach (Transaction _Transaction in Transactions)
                {
                    context.Transactions.Add(_Transaction);
                }
                context.SaveChanges();
            }
            Console.WriteLine("Syncer Completed!!");
        }

        public Task Execute(IJobExecutionContext context)
        {
            Task _Task = new Task(() => { Sync(); });
            _Task.Start();
            return _Task;
        }
    }
}