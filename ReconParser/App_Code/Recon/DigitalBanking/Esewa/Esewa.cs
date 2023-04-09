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

namespace ReconParser.App_Code.Recon.DigitalBanking.Esewa
{
    public class Esewa : DigitalBanking
    {
        public Esewa(String FileName, int FileCount)
            : base(FileName, FileCount)
        {
            _SubSource = SubSource.Find_By_Name("Esewa");
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
                    GetEsewaTransactions(dt);
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

        public void GetEsewaTransactions(DataTable _GetEsewaTransactionDataTable)
        {
            foreach (DataRow _dataRow in _GetEsewaTransactionDataTable.Rows)
            {
                EsewaTransaction _EsewaTransactions = GetEsewaTransaction(_dataRow);
                if (_EsewaTransactions != null)
                    EsewaTransactions.Add(_EsewaTransactions);
            }
        }

        protected EsewaTransaction GetEsewaTransaction(DataRow _dataRow)
        {
            try
            {
                var rowNull = _dataRow.ItemArray.All(c => c is DBNull);
                EsewaTransaction _EsewaTransaction = new EsewaTransaction();
                if (!rowNull)
                {

                    DateTimeFormatInfo dateFormat = new DateTimeFormatInfo();
                    dateFormat.ShortDatePattern = @"yyyyMMdd";

                    _EsewaTransaction.EsewaId = _dataRow[1].ToString();
                    _EsewaTransaction.TransactionAmount = Convert.ToDecimal(_dataRow[2].ToString());
                    _EsewaTransaction.Status = _dataRow[3].ToString();
                    _EsewaTransaction.UniqueId = _dataRow[4].ToString();
                    _EsewaTransaction.Settlement = _dataRow[5].ToString();
                    _EsewaTransaction.TransactionDate = Convert.ToDateTime(_dataRow[6], dateFormat).Date;
                    _EsewaTransaction.NoofAttempt = _dataRow[7].ToString();
                    _EsewaTransaction.Remark = _dataRow[8].ToString();
                    _EsewaTransaction.InitiatingAc = _dataRow[9].ToString();
                    _EsewaTransaction.CreatedBy = _dataRow[10].ToString();
                    _EsewaTransaction.VerifiedBy = _dataRow[11].ToString();
                    _EsewaTransaction.LastModifiedDate = Convert.ToDateTime(_dataRow[12].ToString());

                    return _EsewaTransaction;
                }
                else
                    return _EsewaTransaction = null;
            }
            catch
            {
                throw;
            }
        }
    }
}
