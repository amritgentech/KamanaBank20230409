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

namespace ReconParser.App_Code.Recon.DigitalBanking.MobileIbft
{
    public class MobileIbft : DigitalBanking
    {
        public MobileIbft(String FileName, int FileCount)
            : base(FileName, FileCount)
        {
            _SubSource = SubSource.Find_By_Name("MobileIbft");
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
                    GetMobileIbftTransactions(dt);
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

        public void GetMobileIbftTransactions(DataTable _GetMobileIbftTransactionDataTable)
        {
            foreach (DataRow _dataRow in _GetMobileIbftTransactionDataTable.Rows)
            {
                MobileIbftTransaction _MobileIbftTransactions = GetMobileIbftTransaction(_dataRow);
                if (_MobileIbftTransactions != null)
                    MobileIbftTransactions.Add(_MobileIbftTransactions);
            }
        }

        protected MobileIbftTransaction GetMobileIbftTransaction(DataRow _dataRow)
        {
            try
            {
                var rowNull = _dataRow.ItemArray.All(c => c is DBNull);
                MobileIbftTransaction _MobileIbftTransaction = new MobileIbftTransaction();
                if (!rowNull)
                {

                    DateTimeFormatInfo dateFormat = new DateTimeFormatInfo();
                    dateFormat.ShortDatePattern = @"yyyyMMdd";


                    _MobileIbftTransaction.Id = _dataRow[0].ToString();
                    _MobileIbftTransaction.MobileNumber = _dataRow[1].ToString();
                    _MobileIbftTransaction.FromAccount = _dataRow[2].ToString();
                    _MobileIbftTransaction.ToAccount = _dataRow[3].ToString();
                    _MobileIbftTransaction.TransactionAmount = Convert.ToDecimal(_dataRow[4].ToString());
                    if (!_dataRow.IsNull("Charge Amount"))
                        _MobileIbftTransaction.ChargeAmount = Convert.ToDecimal(_dataRow[5].ToString());
                    _MobileIbftTransaction.TransactionDate = Convert.ToDateTime(_dataRow[6], dateFormat).Date;
                    _MobileIbftTransaction.AccessibleChannel = _dataRow[7].ToString();
                    _MobileIbftTransaction.TransactionStatus = _dataRow[8].ToString();
                    _MobileIbftTransaction.TransactionDescription = _dataRow[9].ToString();
                    _MobileIbftTransaction.TransactionCode = _dataRow[10].ToString();
                    _MobileIbftTransaction.DestinationAccountDescription = _dataRow[11].ToString();
                    _MobileIbftTransaction.FonepayTraceId = _dataRow[12].ToString();
                    _MobileIbftTransaction.FonepayTransactionStatus = _dataRow[13].ToString();
                    _MobileIbftTransaction.TransactionTraceId = _dataRow[14].ToString();
                    _MobileIbftTransaction.StanID = _dataRow[15].ToString();

                    return _MobileIbftTransaction;
                }
                else
                    return _MobileIbftTransaction = null;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}
