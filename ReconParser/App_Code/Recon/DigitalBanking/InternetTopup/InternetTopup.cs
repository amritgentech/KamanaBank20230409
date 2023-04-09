using Db.Enum;
using Db.Model;
using ReadWriteFiles;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helper.GlobalHelpers;

namespace ReconParser.App_Code.Recon.DigitalBanking.InternetTopup
{
    public class InternetTopup : DigitalBanking
    {
        public InternetTopup(String FileName, int FileCount)
            : base(FileName, FileCount)
        {
            _SubSource = SubSource.Find_By_Name("InternetTopup");
        }

        public override void Parse()
        {
            Console.WriteLine("Parsing File {0}", FileName);

            ReadDataFromFile(FileName);

            Console.WriteLine("Parsed File {0}", FileName);
        }

        public void ReadDataFromFile(string FileName)
        {
            try
            {
                DataTable dt = new DataTable();
                try
                {
                    dt = ReadExcelFileEPPUnmerged(FileName);
                    GetInternetTopupTransactions(dt);
                }
                catch (Exception ex)
                {

                }
            }
            catch (Exception ex)
            {
                Console.Write("Erro->Pumori->ReadDataFromFile->" + ex.Message);
            }
        }

        public void GetInternetTopupTransactions(DataTable _GetInternetTopupTransactionDataTable)
        {
            foreach (DataRow _dataRow in _GetInternetTopupTransactionDataTable.Rows)
            {
                InternetTopupTransaction _InternetTopupTransactions = GetInternetTopupTransaction(_dataRow);
                if (_InternetTopupTransactions != null)
                    InternetTopupTransactions.Add(_InternetTopupTransactions);
            }
        }

        protected InternetTopupTransaction GetInternetTopupTransaction(DataRow _dataRow)
        {
            try
            {
                var rowNull = _dataRow.ItemArray.All(c => c is DBNull);
                InternetTopupTransaction _InternetTopupTransaction = new InternetTopupTransaction();
                if (!rowNull)
                {

                    DateTimeFormatInfo dateFormat = new DateTimeFormatInfo();
                    dateFormat.ShortDatePattern = @"yyyyMMdd";

                    _InternetTopupTransaction.TransactionTraceId = _dataRow[0].ToString();
                    _InternetTopupTransaction.CustomerName = _dataRow[1].ToString();
                    _InternetTopupTransaction.UserName = _dataRow[2].ToString();
                    _InternetTopupTransaction.Profile = _dataRow[3].ToString();
                    _InternetTopupTransaction.TopupTraceId = _dataRow[4].ToString();
                    _InternetTopupTransaction.FromAccount = _dataRow[5].ToString();
                    _InternetTopupTransaction.ToAccount = _dataRow[6].ToString();
                    _InternetTopupTransaction.TransactionAmount = Convert.ToDecimal(_dataRow[7].ToString());
                    _InternetTopupTransaction.TopupNumber = _dataRow[8].ToString();
                    _InternetTopupTransaction.Remarks = _dataRow[9].ToString();
                    _InternetTopupTransaction.TransactionDate = Convert.ToDateTime(_dataRow[10], dateFormat).Date;
                    _InternetTopupTransaction.TopupStatus = _dataRow[11].ToString();
                    _InternetTopupTransaction.TransactionStatus = _dataRow[12].ToString();

                    return _InternetTopupTransaction;
                }
                else
                    return _InternetTopupTransaction = null;
            }
            catch
            {
                throw;
            }
        }
    }
}
