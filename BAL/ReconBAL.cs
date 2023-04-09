using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Db.Model;
using DbOperations;
using Helper;
using Helper.GlobalHelpers;

namespace BAL
{
    public static class ReconBAL
    {
        public static List<SelectListItem> GetReconList()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            using (var context = new Db.ReconContext())
            {
                items = context.ReconTypes.Where(x => x.IsDisplay == true).ToList().Select(x => new SelectListItem()
                {
                    Text = x.Description,
                    Value = x.ReconTypeId.ToString()
                }).ToList();
            }
            return items;
        }
        //mobile banking
        public static List<SelectListItem> ReconList()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            using (var context = new Db.ReconContext())
            {
                items = context.ReconTypes.Where(x => x.IsDisplay == false).ToList().Select(x => new SelectListItem()
                {
                    Text = x.Description,
                    Value = x.ReconTypeId.ToString()
                }).ToList();
            }
            return items;
        }
        public static List<SelectListItem> GetReconLists()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            using (var context = new Db.ReconContext())
            {
                items = context.ReconTypes.Where(x => x.IsDisplay == false).ToList().Select(x => new SelectListItem()
                {
                    Text = x.Description,
                    Value = x.ReconTypeId.ToString()
                }).ToList();
            }
            return items;
        }
        public static List<SelectListItem> GetTerminalList()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            using (var context = new Db.ReconContext())
            {
                items = context.Terminals.Where(x => true).ToList().Select(x => new SelectListItem()
                {
                    Text = x.Address + "-" + x.TerminalId,
                    Value = x.TerminalId.ToString()
                }).OrderBy(x => x.Text).ToList();
            }
            return items;
        }
        public static List<SelectListItem> GetReasonList()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            using (var context = new Db.ReconContext())
            {
                items = context.Reasons.Where(x => x.IsDisplay == true).ToList().Select(x => new SelectListItem()
                {
                    Text = x.Description,
                    Value = x.ReasonId.ToString()
                }).ToList();
            }
            return items;
        }
        public static string GetReconTypeMethodName(int ReconType)
        {
            string ReconMappedMethodName;
            using (var context = new Db.ReconContext())
            {
                ReconMappedMethodName = context.ReconTypes.FirstOrDefault(x => x.ReconTypeId == ReconType)
                    .MapReconMethod;
            }
            return ReconMappedMethodName;
        }
        public static List<SelectListItem> GetSourceList()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            using (var context = new Db.ReconContext())
            {
                items = context.Sources.ToList().Select(x => new SelectListItem()
                {
                    Text = x.Description,
                    Value = x.SourceId.ToString()
                }).ToList();
            }
            return items;
        }
        public static List<SelectListItem> GetSubSourceList(int? sourceId)
        {
            List<SelectListItem> SubSourceList = new List<SelectListItem>();
            using (var context = new Db.ReconContext())
            {
                SubSourceList = context.SubSources.Where(x => x.Source.SourceId == sourceId).Select(
                   x => new SelectListItem()
                   {
                       Text = x.Description,
                       Value = x.SubSourceId.ToString()
                   }).ToList();
            }
            return SubSourceList;
        }
        public static List<SelectListItem> GetSubChildSourceList(int? subSourceId)
        {
            List<SelectListItem> SubChildSourceList = new List<SelectListItem>();
            using (var context = new Db.ReconContext())
            {
                SubChildSourceList = context.SubChildSources.Where(x => x.SubSource.SubSourceId == subSourceId).Select(
                   x => new SelectListItem()
                   {
                       Text = x.Description,
                       Value = x.SubChildSourceId.ToString()
                   }).ToList();
            }
            return SubChildSourceList;
        }
        public static SubSource GetSubSource(int? sourceId)
        {
            using (var context = new Db.ReconContext())
            {
                return context.SubSources.Where(x => x.Source.SourceId == sourceId).FirstOrDefault();
            }
        }

        public static int GetSourceIdByName(string sourceName)
        {
            using (var context = new Db.ReconContext())
            {
                return context.Sources.Where(x => x.Description.Equals(sourceName)).Select(x => x.SourceId).FirstOrDefault();
            }
        }
        public static int GetSubSourceIdByName(string subSourceName)
        {
            using (var context = new Db.ReconContext())
            {
                return context.SubSources.Where(x => x.Description.Equals(subSourceName)).Select(x => x.SubSourceId).FirstOrDefault();
            }
        }
        public static int GetSubChildSourceIdByName(string subChildSourceName)
        {
            using (var context = new Db.ReconContext())
            {
                return context.SubChildSources.Where(x => x.SourceChildDescription.Equals(subChildSourceName)).Select(x => x.SubChildSourceId).FirstOrDefault();
            }
        }
        public static string GetReconTypeMappedMethod(int? reconType)
        {
            using (var context = new Db.ReconContext())
            {
                return context.ReconTypes.FirstOrDefault(x => x.ReconTypeId == reconType).MapReconMethod;
            }
        }
        public static SubChildSource GetSubChildSource(int? subSourceId)
        {
            using (var context = new Db.ReconContext())
            {
                return context.SubChildSources.Where(x => x.SubSource.SubSourceId == subSourceId).FirstOrDefault();
            }
        }
        public static void SaveActivityLog(ActivityLog activityLog)
        {
            using (var context = new Db.ReconContext())
            {
                context.ActivityLogs.Add(activityLog);
                context.SaveChanges();
            }
        }
        public static string IsReconActive()
        {
            ReconProcessStatus _ReconProcessStatus = null;
            _ReconProcessStatus = ReconProcessStatus.GetFirst();
            if (_ReconProcessStatus == null)
            {
                return string.Empty;
            }

            return _ReconProcessStatus.IsReconStarted;
        }
        public static void UpdateIsReconStarted(string isReconStarted)
        {
            ReconProcessStatus.UpdateIsReconStarted(isReconStarted);
        }
        public static void ResetReconActivateFlag()
        {
            ReconProcessStatus.ResetReconActivateFlag();
        }
        public static bool IsExistFile(string fileFullPath)
        {
            using (var context = new Db.ReconContext())
            {
                return context.UploadedFiles.Any(x => x.ActualFileName.Equals(fileFullPath));
            }
        }
        public static List<UploadedFile> GetUploadedFiles(int SourceId, int? subSourceId, int? subChildSourceId)
        {
            using (var context = new Db.ReconContext())
            {
                if (subSourceId == null && subChildSourceId == null)
                {
                    return context.UploadedFiles
                        .Where(x => x.SourceId == SourceId).OrderBy(x => x.UpdatedAt)
                        .ToList();
                }
                else if (subChildSourceId == null)
                {
                    return context.UploadedFiles
                        .Where(x => x.SourceId == SourceId && x.SubSourceId == subSourceId).OrderBy(x => x.UpdatedAt).ToList();
                }
                else
                {
                    return context.UploadedFiles
                        .Where(x => x.SourceId == SourceId && x.SubSourceId == subSourceId &&
                                    x.SubChildSourceId == subChildSourceId).OrderBy(x => x.UpdatedAt)
                        .ToList();
                }
            }
        }
        public static void DeleteUploadedFile(int? id)
        {
            using (var context = new Db.ReconContext())
            {
                var dataToDelete = context.UploadedFiles.Find(id);
                context.UploadedFiles.Remove(dataToDelete);
                context.SaveChanges();
            }
        }
        public static bool IsCbs(int? id)
        {
            using (var context = new Db.ReconContext())
            {
                return context.UploadedFiles.Any(x => x.UploadedFileId == id && x.Catagory.ToLower().Equals("cbs"));
            }
        }
        public static string GetFileNameById(int id)
        {
            using (var context = new Db.ReconContext())
            {
                return context.UploadedFiles.Where(x => x.UploadedFileId == id).Select(x => x.ActualFileName).FirstOrDefault();
            }
        }

        public static List<Transaction> GetListTransaction(int id)
        {
            using (var context = new Db.ReconContext())
            {
                UploadedFile uploadedFile = context.UploadedFiles.Find(id);
                int minTransactionId = uploadedFile.MinTransactionId;
                int maxtransactionId = uploadedFile.MaxTransactionId;
                return context.Transactions.Where(x => x.TransactionId >= minTransactionId && x.TransactionId <= maxtransactionId).ToList();
            }
        }
        //digitalbankingtransaction
        public static List<NostroTransaction> GetListNostroTransaction(int id)
        {
            using (var context = new Db.ReconContext())
            {
                return context.NostroTransactions.Where(x => x.UploadedFile_UploadedFileId == id).ToList();
            }
        }
        public static List<MirrorTransaction> GetListMirrorTransaction(int id)
        {
            using (var context = new Db.ReconContext())
            {
                return context.MirrorTransactions.Where(x => x.UploadedFile_UploadedFileId == id).ToList();
            }
        }
        public static List<CbsTransaction> GetListCbsTransaction(int id)
        {
            using (var context = new Db.ReconContext())
            {
                return context.CbsTransactions.Where(x => x.UploadedFile_UploadedFileId == id).ToList();
            }
        }
        public static List<MobileIbftTransaction> GetListMobileIbftTransaction(int id)
        {
            using (var context = new Db.ReconContext())
            {
                return context.MobileIbftTransactions.Where(x => x.UploadedFile_UploadedFileId == id).ToList();
            }
        }
        public static List<InternetIbftTransaction> GetListInternetIbftTransaction(int id)
        {
            using (var context = new Db.ReconContext())
            {
                return context.InternetIbftTransactions.Where(x => x.UploadedFile_UploadedFileId == id).ToList();
            }
        }
        public static List<EsewaTransaction> GetListEsewaTransaction(int id)
        {
            using (var context = new Db.ReconContext())
            {
                return context.EsewaTransactions.Where(x => x.UploadedFile_UploadedFileId == id).ToList();
            }
        }
       
        public static List<Terminal> GetAllTerminalListToExport()
        {
            using (var context = new Db.ReconContext())
            {
                return context.Terminals.ToList();
            }
        }
        public static String GetFileName(int id)
        {
            using (var context = new Db.ReconContext())
            {
                UploadedFile uploadedFile = context.UploadedFiles.Find(id);
                return uploadedFile.ShowFileName;
            }
        }
        // for category name
        public static String GetCategory(int id)
        {
            using (var context = new Db.ReconContext())
            {
                UploadedFile uploadedFile = context.UploadedFiles.Find(id);
                return uploadedFile.Catagory;
            }
        }
      
        public static String GetSubSourceName(int id)
        {
            using (var context = new Db.ReconContext())
            {
                UploadedFile uploadedFile = context.UploadedFiles.Find(id);
                var subsource = context.SubSources.Where(x => x.SubSourceId == uploadedFile.SubSourceId).SingleOrDefault();
                return subsource.Description;
            }
        }
        public static bool IsMemberBank(string cardNumber)
        {
            var binNo = cardNumber.Substring(0, 6);
            using (var context = new Db.ReconContext())
            {
                return context.MemberBankCardBinNos.Any(x=>x.BinNo.Equals(binNo));
            }
        }
        public static bool IsMemberBankPayable(int binNo)
        {
            using (var context = new Db.ReconContext())
            {
                bool test= context.MemberBankCardBinNos.Any(x => x.BinNo==binNo.ToString());
                return context.MemberBankCardBinNos.Any(x => x.BinNo==binNo.ToString());
            }
        }
        public class ReconBALDbOperation
        {
            public DbOperation _DbOperation { get; set; }
            public ReconBALDbOperation()
            {
                _DbOperation = new DbOperation("ReconContextConnectionString");
            }
            public void DeleteUploadedFileTransactions(int? id)
            {
                using (var context = new Db.ReconContext())
                {
                    var minTransactionId = context.UploadedFiles.Where(x => x.UploadedFileId == id)
                        .Select(x => x.MinTransactionId).FirstOrDefault();
                    var maxTransactionId = context.UploadedFiles.Where(x => x.UploadedFileId == id)
                        .Select(x => x.MaxTransactionId).FirstOrDefault();

                    var dataToDeleteForThatDate =
                        context.Transactions.Where(x => x.TransactionId >= minTransactionId &&
                                                        x.TransactionId <= maxTransactionId);

                    List<int> OldTransactionsIds = dataToDeleteForThatDate.Select(t => t.TransactionId).ToList();

                    //finally delete from transaction..
                    int pageCountTransaction = 0;
                    RecursiveDeleteTransactionSqlForInClause(OldTransactionsIds, ref pageCountTransaction); // delete dup transactions from transaction table..
                }
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
            public virtual int RecursiveDeleteTransactionRelationSqlForInClause(List<int> lst, string transactionSource, ref int pagecount)
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
                String sql = "Delete from TransactionRelations where " + transactionSource + " in (" + csvTransactionIdArray + ")";

                _DbOperation.ExecuteNonQuery(sql);

                return RecursiveDeleteTransactionRelationSqlForInClause(lst, transactionSource, ref pagecount);
            }
            public virtual int RecursiveDeleteReconTypeTransactionRelationSqlForInClause(List<int> lst, ref int pagecount)
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
                String sql = "Delete from ReconTypeTransactionRelations where TransactionRelationId3 in (" + csvTransactionIdArray + ")";

                _DbOperation.ExecuteNonQuery(sql);

                return RecursiveDeleteReconTypeTransactionRelationSqlForInClause(lst, ref pagecount);
            }
        }
    }
}
