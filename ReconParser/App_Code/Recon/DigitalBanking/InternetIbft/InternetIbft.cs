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

namespace ReconParser.App_Code.Recon.DigitalBanking.InternetIbft
{
    public class InternetIbft : DigitalBanking
    {
        public InternetIbft(String FileName, int FileCount)
            : base(FileName, FileCount)
        {
            _SubSource = SubSource.Find_By_Name("InternetIbft");
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
                    GetInternetIbftTransactions(dt);
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

        public void GetInternetIbftTransactions(DataTable _GetInternetIbftTransactionDataTable)
        {
            foreach (DataRow _dataRow in _GetInternetIbftTransactionDataTable.Rows)
            {
                InternetIbftTransaction _InternetIbftTransactions = GetInternetIbftTransaction(_dataRow);
                if (_InternetIbftTransactions != null)
                    InternetIbftTransactions.Add(_InternetIbftTransactions);
            }
        }

        protected InternetIbftTransaction GetInternetIbftTransaction(DataRow _dataRow)
        {
            try
            {
                var rowNull = _dataRow.ItemArray.All(c => c is DBNull);
                InternetIbftTransaction _InternetIbftTransaction = new InternetIbftTransaction();
                if (!rowNull)
                {

                    DateTimeFormatInfo dateFormat = new DateTimeFormatInfo();
                    dateFormat.ShortDatePattern = @"yyyyMMdd";

                    _InternetIbftTransaction.CustomerName = _dataRow[0].ToString();
                    _InternetIbftTransaction.MobileNumber = _dataRow[1].ToString();
                    _InternetIbftTransaction.UserName = _dataRow[2].ToString();
                    _InternetIbftTransaction.FromAccount = _dataRow[3].ToString();
                    _InternetIbftTransaction.ToAccount = _dataRow[4].ToString();
                    _InternetIbftTransaction.BankCode = _dataRow[5].ToString();
                    _InternetIbftTransaction.TransactionAmount = Convert.ToDecimal(_dataRow[6].ToString());
                    _InternetIbftTransaction.ChargeAmount = Convert.ToDecimal(_dataRow[7].ToString());
                    _InternetIbftTransaction.TransactionUri = _dataRow[8].ToString();
                    _InternetIbftTransaction.OrginatingUniqueId = _dataRow[9].ToString();
                    _InternetIbftTransaction.TransactionDate = Convert.ToDateTime(_dataRow[10], dateFormat).Date;
                    _InternetIbftTransaction.TransactionStatus = _dataRow[11].ToString();
                    _InternetIbftTransaction.Description = _dataRow[12].ToString();
                    _InternetIbftTransaction.TraceId = _dataRow[13].ToString();
                    _InternetIbftTransaction.ServiceInfoId = _dataRow[14].ToString();

                    return _InternetIbftTransaction;
                }
                else
                    return _InternetIbftTransaction = null;
            }
            catch
            {
                throw;
            }
        }
    }
}
