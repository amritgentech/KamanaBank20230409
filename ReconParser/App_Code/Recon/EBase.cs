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

    public abstract class EBase
    {
        public List<EsewaTransaction> EsewaTransactions;
        public List<NostroTransaction> NostroTransactions;
        public List<MirrorTransaction> MirrorTransactions;
        public List<InternetIbftTransaction> InternetIbftTransactions;
        public List<InternetTopupTransaction> InternetTopupTransactions;
        public List<MobileIbftTransaction> MobileIbftTransactions;
        public List<MobileTopupTransaction> MobileTopupTransactions;
        public List<CbsTransaction> CbsTransactions;
        public Source _Source;
        public SubSource _SubSource;
        public SubChildSource _SubChildSource;
        public DbOperation _DbOperation { get; set; }
        public string FileName { get; set; }
        public int FileCount { get; set; }
        string subSourceName = null;
        public EBase(string FileName, int FileCount)
        {
            this.FileName = FileName;
            this.FileCount = FileCount;
            EsewaTransactions = new List<EsewaTransaction>();
            NostroTransactions = new List<NostroTransaction>();
            MirrorTransactions = new List<MirrorTransaction>();
            InternetIbftTransactions = new List<InternetIbftTransaction>();
            InternetTopupTransactions = new List<InternetTopupTransaction>();
            MobileIbftTransactions = new List<MobileIbftTransaction>();
            MobileTopupTransactions = new List<MobileTopupTransaction>();
            CbsTransactions = new List<CbsTransaction>();
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
            if (EsewaTransactions.Count == 0 && CbsTransactions.Count == 0 && NostroTransactions.Count == 0
                && MirrorTransactions.Count == 0 && InternetIbftTransactions.Count == 0 && InternetTopupTransactions.Count == 0
                && MobileIbftTransactions.Count == 0 && MobileTopupTransactions.Count == 0)
            {
                Console.WriteLine("Data not Found");
                return;
            }
            Console.WriteLine("Data Saving Start......");
            if (EsewaTransactions.Count > 0)
            {
                try
                {
                    using (var context = new ReconContext())
                    {
                        //upload file info saved..
                        var sourceName = context.Sources.Where(x => x.SourceId == _Source.SourceId).Select(x => x.Description).FirstOrDefault();
                        subSourceName = context.SubSources.Where(x => x.SubSourceId == _SubSource.SubSourceId).Select(x => x.Description).FirstOrDefault();
                        UploadedFile _UploadedFile = new UploadedFile();

                        var filesplit = FileName.Split('\\');
                        var stringcount = filesplit.Length - 1;

                        _UploadedFile.ShowFileName =

                         (EsewaTransactions.Select(x => x.TransactionDate).Min() ==
                                  EsewaTransactions.Select(x => x.TransactionDate).Max()
                                    ? EsewaTransactions.Select(x => x.TransactionDate).Min().ToString("dd MMMM, yyyy")
                                    : EsewaTransactions.Select(x => x.TransactionDate).Min().ToString("dd MMMM, yyyy") +
                                      "-----" +
                                      EsewaTransactions.Select(x => x.TransactionDate).Max().ToString("dd MMMM, yyyy"))
                            + "-----" + Regex.Split(filesplit[stringcount], "Append")[0];

                        _UploadedFile.ActualFileName = FileName;
                        _UploadedFile.Catagory = sourceName;
                        _UploadedFile.SourceId = _Source.SourceId;
                        _UploadedFile.MinTransactionId = IdentityValueTransaction() == 0 ? 1 : IdentityValueTransaction() + 1;
                        _UploadedFile.MaxTransactionId = IdentityValueTransaction() + EsewaTransactions.Count;

                        if (_SubSource != null)
                            _UploadedFile.SubSourceId = _SubSource.SubSourceId;

                        context.UploadedFiles.Add(_UploadedFile);
                        context.SaveChanges();

                        List<EsewaTransaction> listEsewaTransaction = new List<EsewaTransaction>();

                        try
                        {
                            foreach (var txn in EsewaTransactions)
                            {
                                txn.Source_SourceId = _Source.SourceId;
                                txn.SubSource_SubSourceId = _SubSource.SubSourceId;
                                txn.UploadedFile_UploadedFileId = _UploadedFile.UploadedFileId;
                                listEsewaTransaction.Add(txn);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                        _DbOperation.BulkInsert(listEsewaTransaction);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine("Finished process file. Saved {0} Transactions.", EsewaTransactions.Count);
            }
            else if (NostroTransactions.Count > 0)
            {
                try
                {
                    using (var context = new ReconContext())
                    {
                        //upload file info saved..
                        var sourceName = context.Sources.Where(x => x.SourceId == _Source.SourceId).Select(x => x.Description).FirstOrDefault();
                        subSourceName = context.SubSources.Where(x => x.SubSourceId == _SubSource.SubSourceId).Select(x => x.Description).FirstOrDefault();
                        UploadedFile _UploadedFile = new UploadedFile();

                        var filesplit = FileName.Split('\\');
                        var stringcount = filesplit.Length - 1;

                        _UploadedFile.ShowFileName =

                         (NostroTransactions.Select(x => x.TransactionDate).Min() ==
                                  NostroTransactions.Select(x => x.TransactionDate).Max()
                                    ? NostroTransactions.Select(x => x.TransactionDate).Min().ToString("dd MMMM, yyyy")
                                    : NostroTransactions.Select(x => x.TransactionDate).Min().ToString("dd MMMM, yyyy") +
                                      "-----" +
                                      NostroTransactions.Select(x => x.TransactionDate).Max().ToString("dd MMMM, yyyy"))
                            + "-----" + Regex.Split(filesplit[stringcount], "Append")[0];

                        _UploadedFile.ActualFileName = FileName;
                        _UploadedFile.Catagory = sourceName;
                        _UploadedFile.SourceId = _Source.SourceId;
                        _UploadedFile.MinTransactionId = IdentityValueTransaction() == 0 ? 1 : IdentityValueTransaction() + 1;
                        _UploadedFile.MaxTransactionId = IdentityValueTransaction() + NostroTransactions.Count;

                        if (_SubSource != null)
                            _UploadedFile.SubSourceId = _SubSource.SubSourceId;

                        context.UploadedFiles.Add(_UploadedFile);
                        context.SaveChanges();

                        List<NostroTransaction> listNostroTransaction = new List<NostroTransaction>();

                        try
                        {
                            foreach (var txn in NostroTransactions)
                            {
                                txn.Source_SourceId = _Source.SourceId;
                                txn.SubSource_SubSourceId = _SubSource.SubSourceId;
                                txn.UploadedFile_UploadedFileId = _UploadedFile.UploadedFileId;
                                listNostroTransaction.Add(txn);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                        _DbOperation.BulkInsert(listNostroTransaction);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine("Finished process file. Saved {0} Transactions.", NostroTransactions.Count);
            }
            else if (MirrorTransactions.Count > 0)
            {
                try
                {
                    using (var context = new ReconContext())
                    {
                        //upload file info saved..
                        var sourceName = context.Sources.Where(x => x.SourceId == _Source.SourceId).Select(x => x.Description).FirstOrDefault();
                        subSourceName = context.SubSources.Where(x => x.SubSourceId == _SubSource.SubSourceId).Select(x => x.Description).FirstOrDefault();
                        UploadedFile _UploadedFile = new UploadedFile();

                        var filesplit = FileName.Split('\\');
                        var stringcount = filesplit.Length - 1;

                        _UploadedFile.ShowFileName =

                         (MirrorTransactions.Select(x => x.TransactionDate).Min() ==
                                  MirrorTransactions.Select(x => x.TransactionDate).Max()
                                    ? MirrorTransactions.Select(x => x.TransactionDate).Min().ToString("dd MMMM, yyyy")
                                    : MirrorTransactions.Select(x => x.TransactionDate).Min().ToString("dd MMMM, yyyy") +
                                      "-----" +
                                      MirrorTransactions.Select(x => x.TransactionDate).Max().ToString("dd MMMM, yyyy"))
                            + "-----" + Regex.Split(filesplit[stringcount], "Append")[0];

                        _UploadedFile.ActualFileName = FileName;
                        _UploadedFile.Catagory = sourceName;
                        _UploadedFile.SourceId = _Source.SourceId;
                        _UploadedFile.MinTransactionId = IdentityValueTransaction() == 0 ? 1 : IdentityValueTransaction() + 1;
                        _UploadedFile.MaxTransactionId = IdentityValueTransaction() + MirrorTransactions.Count;

                        if (_SubSource != null)
                            _UploadedFile.SubSourceId = _SubSource.SubSourceId;

                        context.UploadedFiles.Add(_UploadedFile);
                        context.SaveChanges();

                        List<MirrorTransaction> listMirrorTransaction = new List<MirrorTransaction>();

                        try
                        {
                            foreach (var txn in MirrorTransactions)
                            {
                                txn.Source_SourceId = _Source.SourceId;
                                txn.SubSource_SubSourceId = _SubSource.SubSourceId;
                                txn.UploadedFile_UploadedFileId = _UploadedFile.UploadedFileId;
                                listMirrorTransaction.Add(txn);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                        _DbOperation.BulkInsert(listMirrorTransaction);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine("Finished process file. Saved {0} Transactions.", MirrorTransactions.Count);
            }
            else if (InternetIbftTransactions.Count > 0)
            {
                try
                {
                    using (var context = new ReconContext())
                    {
                        //upload file info saved..
                        var sourceName = context.Sources.Where(x => x.SourceId == _Source.SourceId).Select(x => x.Description).FirstOrDefault();
                        subSourceName = context.SubSources.Where(x => x.SubSourceId == _SubSource.SubSourceId).Select(x => x.Description).FirstOrDefault();
                        UploadedFile _UploadedFile = new UploadedFile();

                        var filesplit = FileName.Split('\\');
                        var stringcount = filesplit.Length - 1;

                        _UploadedFile.ShowFileName =

                         (InternetIbftTransactions.Select(x => x.TransactionDate).Min() ==
                                  InternetIbftTransactions.Select(x => x.TransactionDate).Max()
                                    ? InternetIbftTransactions.Select(x => x.TransactionDate).Min().ToString("dd MMMM, yyyy")
                                    : InternetIbftTransactions.Select(x => x.TransactionDate).Min().ToString("dd MMMM, yyyy") +
                                      "-----" +
                                      InternetIbftTransactions.Select(x => x.TransactionDate).Max().ToString("dd MMMM, yyyy"))
                            + "-----" + Regex.Split(filesplit[stringcount], "Append")[0];

                        _UploadedFile.ActualFileName = FileName;
                        _UploadedFile.Catagory = sourceName;
                        _UploadedFile.SourceId = _Source.SourceId;
                        _UploadedFile.MinTransactionId = IdentityValueTransaction() == 0 ? 1 : IdentityValueTransaction() + 1;
                        _UploadedFile.MaxTransactionId = IdentityValueTransaction() + InternetIbftTransactions.Count;

                        if (_SubSource != null)
                            _UploadedFile.SubSourceId = _SubSource.SubSourceId;

                        context.UploadedFiles.Add(_UploadedFile);
                        context.SaveChanges();

                        List<InternetIbftTransaction> listInternetIbftTransaction = new List<InternetIbftTransaction>();

                        try
                        {
                            foreach (var txn in InternetIbftTransactions)
                            {
                                txn.Source_SourceId = _Source.SourceId;
                                txn.SubSource_SubSourceId = _SubSource.SubSourceId;
                                txn.UploadedFile_UploadedFileId = _UploadedFile.UploadedFileId;
                                listInternetIbftTransaction.Add(txn);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                        _DbOperation.BulkInsert(listInternetIbftTransaction);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine("Finished process file. Saved {0} Transactions.", InternetIbftTransactions.Count);
            }
            else if (InternetTopupTransactions.Count > 0)
            {
                try
                {
                    using (var context = new ReconContext())
                    {
                        //upload file info saved..
                        var sourceName = context.Sources.Where(x => x.SourceId == _Source.SourceId).Select(x => x.Description).FirstOrDefault();
                        subSourceName = context.SubSources.Where(x => x.SubSourceId == _SubSource.SubSourceId).Select(x => x.Description).FirstOrDefault();
                        UploadedFile _UploadedFile = new UploadedFile();

                        var filesplit = FileName.Split('\\');
                        var stringcount = filesplit.Length - 1;

                        _UploadedFile.ShowFileName =

                         (InternetTopupTransactions.Select(x => x.TransactionDate).Min() ==
                                  InternetTopupTransactions.Select(x => x.TransactionDate).Max()
                                    ? InternetTopupTransactions.Select(x => x.TransactionDate).Min().ToString("dd MMMM, yyyy")
                                    : InternetTopupTransactions.Select(x => x.TransactionDate).Min().ToString("dd MMMM, yyyy") +
                                      "-----" +
                                      InternetTopupTransactions.Select(x => x.TransactionDate).Max().ToString("dd MMMM, yyyy"))
                            + "-----" + Regex.Split(filesplit[stringcount], "Append")[0];

                        _UploadedFile.ActualFileName = FileName;
                        _UploadedFile.Catagory = sourceName;
                        _UploadedFile.SourceId = _Source.SourceId;
                        _UploadedFile.MinTransactionId = IdentityValueTransaction() == 0 ? 1 : IdentityValueTransaction() + 1;
                        _UploadedFile.MaxTransactionId = IdentityValueTransaction() + InternetTopupTransactions.Count;

                        if (_SubSource != null)
                            _UploadedFile.SubSourceId = _SubSource.SubSourceId;

                        context.UploadedFiles.Add(_UploadedFile);
                        context.SaveChanges();

                        List<InternetTopupTransaction> listInternetTopupTransaction = new List<InternetTopupTransaction>();

                        try
                        {
                            foreach (var txn in InternetTopupTransactions)
                            {
                                txn.Source_SourceId = _Source.SourceId;
                                txn.SubSource_SubSourceId = _SubSource.SubSourceId;
                                txn.UploadedFile_UploadedFileId = _UploadedFile.UploadedFileId;
                                listInternetTopupTransaction.Add(txn);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                        _DbOperation.BulkInsert(listInternetTopupTransaction);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine("Finished process file. Saved {0} Transactions.", InternetTopupTransactions.Count);
            }
            else if (MobileIbftTransactions.Count > 0)
            {
                try
                {
                    using (var context = new ReconContext())
                    {
                        //upload file info saved..
                        var sourceName = context.Sources.Where(x => x.SourceId == _Source.SourceId).Select(x => x.Description).FirstOrDefault();
                        subSourceName = context.SubSources.Where(x => x.SubSourceId == _SubSource.SubSourceId).Select(x => x.Description).FirstOrDefault();
                        UploadedFile _UploadedFile = new UploadedFile();

                        var filesplit = FileName.Split('\\');
                        var stringcount = filesplit.Length - 1;

                        _UploadedFile.ShowFileName =

                         (MobileIbftTransactions.Select(x => x.TransactionDate).Min() ==
                                  MobileIbftTransactions.Select(x => x.TransactionDate).Max()
                                    ? MobileIbftTransactions.Select(x => x.TransactionDate).Min().ToString("dd MMMM, yyyy")
                                    : MobileIbftTransactions.Select(x => x.TransactionDate).Min().ToString("dd MMMM, yyyy") +
                                      "-----" +
                                      MobileIbftTransactions.Select(x => x.TransactionDate).Max().ToString("dd MMMM, yyyy"))
                            + "-----" + Regex.Split(filesplit[stringcount], "Append")[0];

                        _UploadedFile.ActualFileName = FileName;
                        _UploadedFile.Catagory = sourceName;
                        _UploadedFile.SourceId = _Source.SourceId;
                        _UploadedFile.MinTransactionId = IdentityValueTransaction() == 0 ? 1 : IdentityValueTransaction() + 1;
                        _UploadedFile.MaxTransactionId = IdentityValueTransaction() + MobileIbftTransactions.Count;

                        if (_SubSource != null)
                            _UploadedFile.SubSourceId = _SubSource.SubSourceId;

                        context.UploadedFiles.Add(_UploadedFile);
                        context.SaveChanges();

                        List<MobileIbftTransaction> listMobileIbftTransaction = new List<MobileIbftTransaction>();

                        try
                        {
                            foreach (var txn in MobileIbftTransactions)
                            {
                                txn.Source_SourceId = _Source.SourceId;
                                txn.SubSource_SubSourceId = _SubSource.SubSourceId;
                                txn.UploadedFile_UploadedFileId = _UploadedFile.UploadedFileId;
                                listMobileIbftTransaction.Add(txn);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                        _DbOperation.BulkInsert(listMobileIbftTransaction);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine("Finished process file. Saved {0} Transactions.", MobileIbftTransactions.Count);
            }
            else if (MobileTopupTransactions.Count > 0)
            {
                try
                {
                    using (var context = new ReconContext())
                    {
                        //upload file info saved..
                        var sourceName = context.Sources.Where(x => x.SourceId == _Source.SourceId).Select(x => x.Description).FirstOrDefault();
                        subSourceName = context.SubSources.Where(x => x.SubSourceId == _SubSource.SubSourceId).Select(x => x.Description).FirstOrDefault();
                        UploadedFile _UploadedFile = new UploadedFile();

                        var filesplit = FileName.Split('\\');
                        var stringcount = filesplit.Length - 1;

                        _UploadedFile.ShowFileName =

                         (MobileTopupTransactions.Select(x => x.TransactionDate).Min() ==
                                  MobileTopupTransactions.Select(x => x.TransactionDate).Max()
                                    ? MobileTopupTransactions.Select(x => x.TransactionDate).Min().ToString("dd MMMM, yyyy")
                                    : MobileTopupTransactions.Select(x => x.TransactionDate).Min().ToString("dd MMMM, yyyy") +
                                      "-----" +
                                      MobileTopupTransactions.Select(x => x.TransactionDate).Max().ToString("dd MMMM, yyyy"))
                            + "-----" + Regex.Split(filesplit[stringcount], "Append")[0];

                        _UploadedFile.ActualFileName = FileName;
                        _UploadedFile.Catagory = sourceName;
                        _UploadedFile.SourceId = _Source.SourceId;
                        _UploadedFile.MinTransactionId = IdentityValueTransaction() == 0 ? 1 : IdentityValueTransaction() + 1;
                        _UploadedFile.MaxTransactionId = IdentityValueTransaction() + MobileTopupTransactions.Count;

                        if (_SubSource != null)
                            _UploadedFile.SubSourceId = _SubSource.SubSourceId;

                        context.UploadedFiles.Add(_UploadedFile);
                        context.SaveChanges();

                        List<MobileTopupTransaction> listMobileTopupTransaction = new List<MobileTopupTransaction>();

                        try
                        {
                            foreach (var txn in MobileTopupTransactions)
                            {
                                txn.Source_SourceId = _Source.SourceId;
                                txn.SubSource_SubSourceId = _SubSource.SubSourceId;
                                txn.UploadedFile_UploadedFileId = _UploadedFile.UploadedFileId;
                                listMobileTopupTransaction.Add(txn);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                        _DbOperation.BulkInsert(listMobileTopupTransaction);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine("Finished process file. Saved {0} Transactions.", MobileTopupTransactions.Count);
            }
            else if (CbsTransactions.Count > 0)
            {
                try
                {
                    using (var context = new ReconContext())
                    {
                        //upload file info saved..
                        var sourceName = context.Sources.Where(x => x.SourceId == _Source.SourceId).Select(x => x.Description).FirstOrDefault();
                        subSourceName = context.SubSources.Where(x => x.SubSourceId == _SubSource.SubSourceId).Select(x => x.Description).FirstOrDefault();
                        UploadedFile _UploadedFile = new UploadedFile();

                        var filesplit = FileName.Split('\\');
                        var stringcount = filesplit.Length - 1;

                        _UploadedFile.ShowFileName =

                         (CbsTransactions.Select(x => x.TransactionDate).Min() ==
                                  CbsTransactions.Select(x => x.TransactionDate).Max()
                                    ? CbsTransactions.Select(x => x.TransactionDate).Min().ToString("dd MMMM, yyyy")
                                    : CbsTransactions.Select(x => x.TransactionDate).Min().ToString("dd MMMM, yyyy") +
                                      "-----" +
                                      CbsTransactions.Select(x => x.TransactionDate).Max().ToString("dd MMMM, yyyy"))
                            + "-----" + Regex.Split(filesplit[stringcount], "Append")[0];

                        _UploadedFile.ActualFileName = FileName;
                        _UploadedFile.Catagory = sourceName;
                        _UploadedFile.SourceId = _Source.SourceId;
                        _UploadedFile.MinTransactionId = IdentityValueTransaction() == 0 ? 1 : IdentityValueTransaction() + 1;
                        _UploadedFile.MaxTransactionId = IdentityValueTransaction() + CbsTransactions.Count;

                        if (_SubSource != null)
                            _UploadedFile.SubSourceId = _SubSource.SubSourceId;

                        if (_SubChildSource != null)
                            _UploadedFile.SubChildSourceId = _SubChildSource.SubChildSourceId;

                        context.UploadedFiles.Add(_UploadedFile);
                        context.SaveChanges();

                        List<CbsTransaction> listCbsTransaction = new List<CbsTransaction>();

                        try
                        {
                            foreach (var txn in CbsTransactions)
                            {
                                txn.Source_SourceId = _Source.SourceId;
                                txn.SubSource_SubSourceId = _SubSource.SubSourceId;
                                if (_SubChildSource != null)
                                    txn.SubChildSource_SubChildSourceId = _SubChildSource.SubChildSourceId;
                                else
                                    txn.SubChildSource_SubChildSourceId = 0;
                                txn.UploadedFile_UploadedFileId = _UploadedFile.UploadedFileId;
                                listCbsTransaction.Add(txn);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                        _DbOperation.BulkInsert(listCbsTransaction);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine("Finished process file. Saved {0} Transactions.", CbsTransactions.Count);
            }
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
            EsewaTransactions = EsewaTransactions.Where(x => x != null).ToList();
            NostroTransactions = NostroTransactions.Where(x => x != null).ToList();
            MirrorTransactions = MirrorTransactions.Where(x => x != null).ToList();
            InternetIbftTransactions = InternetIbftTransactions.Where(x => x != null).ToList();
            InternetTopupTransactions = InternetTopupTransactions.Where(x => x != null).ToList();
            MobileIbftTransactions = MobileIbftTransactions.Where(x => x != null).ToList();
            MobileTopupTransactions = MobileTopupTransactions.Where(x => x != null).ToList();
            CbsTransactions = CbsTransactions.Where(x => x != null).ToList();

        }
        public void Backup()
        {
        }
        public virtual bool CheckDuplicateAndRemove()
        {
            if (EsewaTransactions.Count <= 0 && CbsTransactions.Count <= 0 && NostroTransactions.Count <= 0 && MirrorTransactions.Count == 0
                && InternetIbftTransactions.Count == 0 && InternetTopupTransactions.Count <= 0 && MobileIbftTransactions.Count <= 0
              && MobileTopupTransactions.Count <= 0)
            {
                return false;
            }
            Console.WriteLine("Finding and Replacing Duplicate Transactions!!");

            using (var context = new ReconContext())
            {
                if (EsewaTransactions.Count > 0)
                {
                    var minTransactionDate = EsewaTransactions.Select(x => x.TransactionDate).Min();
                    var maxTransactionDate = EsewaTransactions.Select(x => x.TransactionDate).Max();


                    var oldtxn = context.EsewaTransactions.Where(x =>
                                            x.SubSource_SubSourceId == _SubSource.SubSourceId &&
                                            x.TransactionDate >= minTransactionDate &&
                                            x.TransactionDate <= maxTransactionDate).ToList();


                    var duplicateTransactions = new List<EsewaTransaction>();
                    duplicateTransactions = (from t1 in EsewaTransactions
                                             join t2 in oldtxn
                                             on
                                             new
                                             {
                                                 EsewaId = GlobalHelper.NullHelperString(t1.EsewaId),
                                                 TransactionAmount = GlobalHelper.NullHelperDecimal(t1.TransactionAmount),
                                                 TransactionDate = GlobalHelper.NullHelperDate(t1.TransactionDate),
                                                 UniqueId = GlobalHelper.NullHelperString(t1.UniqueId),
                                             }
                                             equals
                                             new
                                             {
                                                 EsewaId = GlobalHelper.NullHelperString(t2.EsewaId),
                                                 TransactionAmount = GlobalHelper.NullHelperDecimal(t2.TransactionAmount),
                                                 TransactionDate = GlobalHelper.NullHelperDate(t2.TransactionDate),
                                                 UniqueId = GlobalHelper.NullHelperString(t2.UniqueId),
                                             }
                                             select t2).Distinct()

                   .ToList();

                    if (duplicateTransactions.Count == EsewaTransactions.Count)
                    {
                        return false;
                    }
                    if (duplicateTransactions.Count > 0)
                    {
                        var OldNepsTransactionsIds = duplicateTransactions.Select(t => t.EsewaTransactionId).ToList();
                        int pageCountTransaction = 0;
                        RecursiveDeleteTransactionSqlForInClause(OldNepsTransactionsIds, ref pageCountTransaction);
                    }
                }
                else if (NostroTransactions.Count > 0)
                {
                    var minTransactionDate = NostroTransactions.Select(x => x.TransactionDate).Min();
                    var maxTransactionDate = NostroTransactions.Select(x => x.TransactionDate).Max();


                    var oldtxn = context.NostroTransactions.Where(x =>
                                            x.SubSource_SubSourceId == _SubSource.SubSourceId &&
                                            x.TransactionDate >= minTransactionDate &&
                                            x.TransactionDate <= maxTransactionDate).ToList();


                    var duplicateTransactions = new List<NostroTransaction>();
                    duplicateTransactions = (from t1 in NostroTransactions
                                             join t2 in oldtxn
                                             on
                                             new
                                             {
                                                 TransactionId = GlobalHelper.NullHelperString(t1.TransactionId),
                                                 Amount = GlobalHelper.NullHelperDecimal(t1.TransactionAmount),
                                                 TransactionDate = GlobalHelper.NullHelperDate(t1.TransactionDate),
                                             }
                                             equals
                                             new
                                             {
                                                 TransactionId = GlobalHelper.NullHelperString(t2.TransactionId),
                                                 Amount = GlobalHelper.NullHelperDecimal(t2.TransactionAmount),
                                                 TransactionDate = GlobalHelper.NullHelperDate(t2.TransactionDate),
                                             }
                                             select t2).Distinct()

                   .ToList();

                    if (duplicateTransactions.Count == NostroTransactions.Count)
                    {
                        return false;
                    }
                    if (duplicateTransactions.Count > 0)
                    {
                        var OldNepsTransactionsIds = duplicateTransactions.Select(t => t.NostroTransactionId).ToList();
                        int pageCountTransaction = 0;
                        RecursiveDeleteTransactionSqlForInClause(OldNepsTransactionsIds, ref pageCountTransaction);
                    }
                }
                else if (MirrorTransactions.Count > 0)
                {
                    var minTransactionDate = MirrorTransactions.Select(x => x.TransactionDate).Min();
                    var maxTransactionDate = MirrorTransactions.Select(x => x.TransactionDate).Max();


                    var oldtxn = context.MirrorTransactions.Where(x =>
                                            x.SubSource_SubSourceId == _SubSource.SubSourceId &&
                                            x.TransactionDate >= minTransactionDate &&
                                            x.TransactionDate <= maxTransactionDate).ToList();


                    var duplicateTransactions = new List<MirrorTransaction>();
                    duplicateTransactions = (from t1 in MirrorTransactions
                                             join t2 in oldtxn
                                             on
                                             new
                                             {
                                                 ReferenceNumber = GlobalHelper.NullHelperString(t1.ReferenceNumber),
                                                 Amount = GlobalHelper.NullHelperDecimal(t1.TransactionAmount),
                                                 TransactionDate = GlobalHelper.NullHelperDate(t1.TransactionDate),
                                             }
                                             equals
                                             new
                                             {
                                                 ReferenceNumber = GlobalHelper.NullHelperString(t2.ReferenceNumber),
                                                 Amount = GlobalHelper.NullHelperDecimal(t2.TransactionAmount),
                                                 TransactionDate = GlobalHelper.NullHelperDate(t2.TransactionDate),
                                             }
                                             select t2).Distinct()

                   .ToList();

                    if (duplicateTransactions.Count == MirrorTransactions.Count)
                    {
                        return false;
                    }
                    if (duplicateTransactions.Count > 0)
                    {
                        var OldNepsTransactionsIds = duplicateTransactions.Select(t => t.MirrorTransactionId).ToList();
                        int pageCountTransaction = 0;
                        RecursiveDeleteTransactionSqlForInClause(OldNepsTransactionsIds, ref pageCountTransaction);
                    }
                }
                else if (InternetIbftTransactions.Count > 0)
                {
                    var minTransactionDate = InternetIbftTransactions.Select(x => x.TransactionDate).Min();
                    var maxTransactionDate = InternetIbftTransactions.Select(x => x.TransactionDate).Max();


                    var oldtxn = context.InternetIbftTransactions.Where(x =>
                                            x.SubSource_SubSourceId == _SubSource.SubSourceId &&
                                            x.TransactionDate >= minTransactionDate &&
                                            x.TransactionDate <= maxTransactionDate).ToList();


                    var duplicateTransactions = new List<InternetIbftTransaction>();
                    duplicateTransactions = (from t1 in InternetIbftTransactions
                                             join t2 in oldtxn
                                             on
                                             new
                                             {
                                                 TraceId = GlobalHelper.NullHelperString(t1.TraceId),
                                                 Amount = GlobalHelper.NullHelperDecimal(t1.TransactionAmount),
                                                 TransactionDate = GlobalHelper.NullHelperDate(t1.TransactionDate),
                                             }
                                             equals
                                             new
                                             {
                                                 TraceId = GlobalHelper.NullHelperString(t2.TraceId),
                                                 Amount = GlobalHelper.NullHelperDecimal(t2.TransactionAmount),
                                                 TransactionDate = GlobalHelper.NullHelperDate(t2.TransactionDate),
                                             }
                                             select t2).Distinct()

                   .ToList();

                    if (duplicateTransactions.Count == InternetIbftTransactions.Count)
                    {
                        return false;
                    }
                    if (duplicateTransactions.Count > 0)
                    {
                        var OldNepsTransactionsIds = duplicateTransactions.Select(t => t.InternetIbftTransactionId).ToList();
                        int pageCountTransaction = 0;
                        RecursiveDeleteTransactionSqlForInClause(OldNepsTransactionsIds, ref pageCountTransaction);
                    }
                }
                else if (InternetTopupTransactions.Count > 0)
                {
                    var minTransactionDate = InternetTopupTransactions.Select(x => x.TransactionDate).Min();
                    var maxTransactionDate = InternetTopupTransactions.Select(x => x.TransactionDate).Max();


                    var oldtxn = context.InternetTopupTransactions.Where(x =>
                                            x.SubSource_SubSourceId == _SubSource.SubSourceId &&
                                            x.TransactionDate >= minTransactionDate &&
                                            x.TransactionDate <= maxTransactionDate).ToList();


                    var duplicateTransactions = new List<InternetTopupTransaction>();
                    duplicateTransactions = (from t1 in InternetTopupTransactions
                                             join t2 in oldtxn
                                             on
                                             new
                                             {
                                                 TransactionTraceId = GlobalHelper.NullHelperString(t1.TransactionTraceId),
                                                 TopupTraceId = GlobalHelper.NullHelperString(t1.TopupTraceId),
                                                 Amount = GlobalHelper.NullHelperDecimal(t1.TransactionAmount),
                                                 TransactionDate = GlobalHelper.NullHelperDate(t1.TransactionDate),
                                             }
                                             equals
                                             new
                                             {
                                                 TransactionTraceId = GlobalHelper.NullHelperString(t2.TransactionTraceId),
                                                 TopupTraceId = GlobalHelper.NullHelperString(t2.TopupTraceId),
                                                 Amount = GlobalHelper.NullHelperDecimal(t2.TransactionAmount),
                                                 TransactionDate = GlobalHelper.NullHelperDate(t2.TransactionDate),
                                             }
                                             select t2).Distinct()

                   .ToList();

                    if (duplicateTransactions.Count == InternetTopupTransactions.Count)
                    {
                        return false;
                    }
                    if (duplicateTransactions.Count > 0)
                    {
                        var OldNepsTransactionsIds = duplicateTransactions.Select(t => t.InternetTopupTransactionId).ToList();
                        int pageCountTransaction = 0;
                        RecursiveDeleteTransactionSqlForInClause(OldNepsTransactionsIds, ref pageCountTransaction);
                    }
                }
                else if (MobileIbftTransactions.Count > 0)
                {
                    var minTransactionDate = MobileIbftTransactions.Select(x => x.TransactionDate).Min();
                    var maxTransactionDate = MobileIbftTransactions.Select(x => x.TransactionDate).Max();


                    var oldtxn = context.MobileIbftTransactions.Where(x =>
                                            x.SubSource_SubSourceId == _SubSource.SubSourceId &&
                                            x.TransactionDate >= minTransactionDate &&
                                            x.TransactionDate <= maxTransactionDate).ToList();


                    var duplicateTransactions = new List<MobileIbftTransaction>();
                    duplicateTransactions = (from t1 in MobileIbftTransactions
                                             join t2 in oldtxn
                                             on
                                             new
                                             {
                                                 FonepayTraceId = GlobalHelper.NullHelperString(t1.FonepayTraceId),
                                                 StanID = GlobalHelper.NullHelperString(t1.StanID),
                                                 TransactionAmount = GlobalHelper.NullHelperDecimal(t1.TransactionAmount),
                                                 TransactionDate = GlobalHelper.NullHelperDate(t1.TransactionDate),
                                             }
                                             equals
                                             new
                                             {
                                                 FonepayTraceId = GlobalHelper.NullHelperString(t2.FonepayTraceId),
                                                 StanID = GlobalHelper.NullHelperString(t2.StanID),
                                                 TransactionAmount = GlobalHelper.NullHelperDecimal(t2.TransactionAmount),
                                                 TransactionDate = GlobalHelper.NullHelperDate(t2.TransactionDate),
                                             }
                                             select t2).Distinct()

                   .ToList();

                    if (duplicateTransactions.Count == MobileIbftTransactions.Count)
                    {
                        return false;
                    }
                    if (duplicateTransactions.Count > 0)
                    {
                        var OldNepsTransactionsIds = duplicateTransactions.Select(t => t.MobileIbftTransactionId).ToList();
                        int pageCountTransaction = 0;
                        RecursiveDeleteTransactionSqlForInClause(OldNepsTransactionsIds, ref pageCountTransaction);
                    }
                }
                else if (MobileTopupTransactions.Count > 0)
                {
                    var minTransactionDate = MobileTopupTransactions.Select(x => x.TransactionDate).Min();
                    var maxTransactionDate = MobileTopupTransactions.Select(x => x.TransactionDate).Max();


                    var oldtxn = context.MobileTopupTransactions.Where(x =>
                                            x.SubSource_SubSourceId == _SubSource.SubSourceId &&
                                            x.TransactionDate >= minTransactionDate &&
                                            x.TransactionDate <= maxTransactionDate).ToList();


                    var duplicateTransactions = new List<MobileTopupTransaction>();
                    duplicateTransactions = (from t1 in MobileTopupTransactions
                                             join t2 in oldtxn
                                             on
                                             new
                                             {
                                                 TraceId = GlobalHelper.NullHelperString(t1.TraceId),
                                                 TransactionTraceId = GlobalHelper.NullHelperString(t1.TransactionTraceId),
                                                 TransactionAmount = GlobalHelper.NullHelperDecimal(t1.TransactionAmount),
                                                 TransactionDate = GlobalHelper.NullHelperDate(t1.TransactionDate),
                                             }
                                             equals
                                             new
                                             {
                                                 TraceId = GlobalHelper.NullHelperString(t2.TraceId),
                                                 TransactionTraceId = GlobalHelper.NullHelperString(t2.TransactionTraceId),
                                                 TransactionAmount = GlobalHelper.NullHelperDecimal(t2.TransactionAmount),
                                                 TransactionDate = GlobalHelper.NullHelperDate(t2.TransactionDate),
                                             }
                                             select t2).Distinct()

                   .ToList();

                    if (duplicateTransactions.Count == MobileTopupTransactions.Count)
                    {
                        return false;
                    }
                    if (duplicateTransactions.Count > 0)
                    {
                        var OldNepsTransactionsIds = duplicateTransactions.Select(t => t.MobileTopupTransactionId).ToList();
                        int pageCountTransaction = 0;
                        RecursiveDeleteTransactionSqlForInClause(OldNepsTransactionsIds, ref pageCountTransaction);
                    }
                }
                else if (CbsTransactions.Count > 0)
                {
                    var minTransactionDate = CbsTransactions.Select(x => x.TransactionDate).Min();
                    var maxTransactionDate = CbsTransactions.Select(x => x.TransactionDate).Max();


                    var oldtxn = context.CbsTransactions.Where(x =>
                                            x.SubSource_SubSourceId == _SubSource.SubSourceId &&
                                            x.TransactionDate >= minTransactionDate &&
                                            x.TransactionDate <= maxTransactionDate).ToList();


                    var duplicateTransactions = new List<CbsTransaction>();
                    duplicateTransactions = (from t1 in CbsTransactions
                                             join t2 in oldtxn
                                             on
                                             new
                                             {
                                                 ReferenceNumber = GlobalHelper.NullHelperString(t1.ReferenceNumber),
                                                 Amount = GlobalHelper.NullHelperDecimal(t1.TransactionAmount),
                                                 TransactionDate = GlobalHelper.NullHelperDate(t1.TransactionDate),
                                             }
                                             equals
                                             new
                                             {
                                                 ReferenceNumber = GlobalHelper.NullHelperString(t2.ReferenceNumber),
                                                 Amount = GlobalHelper.NullHelperDecimal(t2.TransactionAmount),
                                                 TransactionDate = GlobalHelper.NullHelperDate(t2.TransactionDate),
                                             }
                                             select t2).Distinct()

                   .ToList();

                    if (duplicateTransactions.Count == CbsTransactions.Count)
                    {
                        return false;
                    }
                    if (duplicateTransactions.Count > 0)
                    {
                        var OldNepsTransactionsIds = duplicateTransactions.Select(t => t.CbsTransactionId).ToList();
                        int pageCountTransaction = 0;
                        RecursiveDeleteTransactionSqlForInClause(OldNepsTransactionsIds, ref pageCountTransaction);
                    }
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
            string sql = null;
            if (subSourceName == "Esewa")
                sql = "Delete from EsewaTransactions where TransactionId in (" + csvTransactionIdArray + ")";
            else if (subSourceName == "Nostro")
                sql = "Delete from NostroTransactions where NostroTransactionId in (" + csvTransactionIdArray + ")";
            else if (subSourceName == "Mirror")
                sql = "Delete from MirrorTransactions where MirrorTransactionId in (" + csvTransactionIdArray + ")";
            else if (subSourceName == "InternetIbft")
                sql = "Delete from InternetIbftTransactions where InternetIbftTransactionId in (" + csvTransactionIdArray + ")";
            else if (subSourceName == "InternetTopup")
                sql = "Delete from InternetTopupTransactions where InternetTopupTransactionId in (" + csvTransactionIdArray + ")";
            else if (subSourceName == "MobileIbft")
                sql = "Delete from MobileIbftTransactions where MobileIbftTransactionId in (" + csvTransactionIdArray + ")";
            else if (subSourceName == "MobileTopup")
                sql = "Delete from MobileTopupTransactions where MobileTopupTransactionId in (" + csvTransactionIdArray + ")";
            else if (subSourceName == "cbs")
                sql = "Delete from CbsTransactions where CbsTransactionId in (" + csvTransactionIdArray + ")";

            _DbOperation.ExecuteNonQuery(sql);

            return RecursiveDeleteTransactionSqlForInClause(lst, ref pagecount);
        }

    }
}
