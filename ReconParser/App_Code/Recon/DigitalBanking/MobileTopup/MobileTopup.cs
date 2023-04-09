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

namespace ReconParser.App_Code.Recon.DigitalBanking.MobileTopup
{
    public class MobileTopup : DigitalBanking
    {
        public MobileTopup(String FileName, int FileCount)
            : base(FileName, FileCount)
        {
            _SubSource = SubSource.Find_By_Name("MobileTopup");
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
                    GetMobileTopupTransactions(dt);
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

        public void GetMobileTopupTransactions(DataTable _GetMobileTopupTransactionDataTable)
        {
            foreach (DataRow _dataRow in _GetMobileTopupTransactionDataTable.Rows)
            {
                MobileTopupTransaction _MobileTopupTransactions = GetMobileTopupTransaction(_dataRow);
                if (_MobileTopupTransactions != null)
                    MobileTopupTransactions.Add(_MobileTopupTransactions);
            }
        }

        protected MobileTopupTransaction GetMobileTopupTransaction(DataRow _dataRow)
        {
            try
            {
                var rowNull = _dataRow.ItemArray.All(c => c is DBNull);
                MobileTopupTransaction _MobileTopupTransaction = new MobileTopupTransaction();
                if (!rowNull)
                {

                    DateTimeFormatInfo dateFormat = new DateTimeFormatInfo();
                    dateFormat.ShortDatePattern = @"yyyyMMdd";

                    _MobileTopupTransaction.TraceId = _dataRow[0].ToString();
                    _MobileTopupTransaction.ServiceName = _dataRow[1].ToString();
                    _MobileTopupTransaction.CustomerName = _dataRow[2].ToString();
                    _MobileTopupTransaction.UserName = _dataRow[3].ToString();
                    _MobileTopupTransaction.FromAccount = _dataRow[4].ToString();
                    _MobileTopupTransaction.ToAccount = _dataRow[5].ToString();
                    _MobileTopupTransaction.Amount = Convert.ToDecimal(_dataRow[6].ToString());
                    _MobileTopupTransaction.ServiceAttribute = _dataRow[7].ToString();
                    _MobileTopupTransaction.RecordedDate = Convert.ToDateTime(_dataRow[8], dateFormat).Date;
                    _MobileTopupTransaction.TransactionStatus = _dataRow[9].ToString();
                    _MobileTopupTransaction.TransactionDate = Convert.ToDateTime(_dataRow[10], dateFormat).Date;
                    _MobileTopupTransaction.TransactionAmount = Convert.ToDecimal(_dataRow[11].ToString());
                    _MobileTopupTransaction.TransactionTraceId = _dataRow[12].ToString();
                    _MobileTopupTransaction.TransactionResponseCode = _dataRow[13].ToString();
                    _MobileTopupTransaction.TransactionResponseDescription = _dataRow[14].ToString();
                    _MobileTopupTransaction.TopupStatus = _dataRow[15].ToString();
                    _MobileTopupTransaction.TopupTraceId = _dataRow[16].ToString();
                    _MobileTopupTransaction.TopupResponseCode = _dataRow[17].ToString();
                    _MobileTopupTransaction.TopupResponseDescription = _dataRow[18].ToString();


                    return _MobileTopupTransaction;
                }
                else
                    return _MobileTopupTransaction = null;
            }
            catch
            {
                throw;
            }
        }
    }
}
