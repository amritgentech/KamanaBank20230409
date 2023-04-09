using Db.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReconParser.App_Code.Recon.DigitalBanking.Mirror
{
    public class Mirror : DigitalBanking
    {
        public Mirror(String FileName, int FileCount)
            : base(FileName, FileCount)
        {
            _SubSource = SubSource.Find_By_Name("Mirror");
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
                    GetNostroTransactions(dt);
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

        public void GetNostroTransactions(DataTable _GetNostroTransactionDataTable)
        {
            foreach (DataRow _dataRow in _GetNostroTransactionDataTable.Rows)
            {
                MirrorTransaction _MirrorTransactions = GetNostroTransaction(_dataRow);
                if (_MirrorTransactions != null)
                    MirrorTransactions.Add(_MirrorTransactions);
            }
        }

        protected MirrorTransaction GetNostroTransaction(DataRow _dataRow)
        {
            try
            {
                var rowNull = _dataRow.ItemArray.All(c => c is DBNull);
                MirrorTransaction _MirrorTransaction = new MirrorTransaction();
                if (!rowNull)
                {
                    DateTimeFormatInfo dateFormat = new DateTimeFormatInfo();
                    dateFormat.ShortDatePattern = @"yyyyMMdd";

                    _MirrorTransaction.TransactionDate = Convert.ToDateTime(_dataRow[0], dateFormat);
                    _MirrorTransaction.Particulars = _dataRow[1].ToString();

                    if (_dataRow["Debit"] != DBNull.Value && Convert.ToDouble(_dataRow[2].ToString()) > 0.00)
                    {
                        _MirrorTransaction.TransactionAmount = Convert.ToDecimal(_dataRow[2].ToString());
                        _MirrorTransaction.DebitOrCredit = "Debit";
                    }


                    else if (_dataRow["Credit"] != DBNull.Value && Convert.ToDouble(_dataRow[3].ToString()) > 0.00)
                    {
                        _MirrorTransaction.TransactionAmount = Convert.ToDecimal(_dataRow[3].ToString());
                        _MirrorTransaction.DebitOrCredit = "Credit";
                    }


                    _MirrorTransaction.Balance = _dataRow[4].ToString();
                    _MirrorTransaction.ReferenceNumber = _dataRow[5].ToString();

                    return _MirrorTransaction;
                }
                else
                    return _MirrorTransaction = null;

            }
            catch(Exception ex)
            {
                throw;
            }
        }

    }
}
