using Db.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReconParser.App_Code.Recon.DigitalBanking.Nostro
{
    public class Nostro : DigitalBanking
    {
        public Nostro(String FileName, int FileCount)
            : base(FileName, FileCount)
        {
            _SubSource = SubSource.Find_By_Name("Nostro");
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
                NostroTransaction _NostroTransactions = GetNostroTransaction(_dataRow);
                if (_NostroTransactions != null)
                    NostroTransactions.Add(_NostroTransactions);
            }
        }

        protected NostroTransaction GetNostroTransaction(DataRow _dataRow)
        {
            try
            {
                var rowNull = _dataRow.ItemArray.All(c => c is DBNull);
                NostroTransaction _NostroTransaction = new NostroTransaction();
                if (!rowNull)
                {
                    DateTimeFormatInfo Date = new DateTimeFormatInfo();
                    Date.ShortDatePattern = @"MMddyyyy";

                    _NostroTransaction.TransactionId = _dataRow[0].ToString();
                    _NostroTransaction.TransactionDate = Convert.ToDateTime(_dataRow[1], Date);
                    _NostroTransaction.ValueDate = Convert.ToDateTime(_dataRow[2], Date);
                    _NostroTransaction.ChequeNo = _dataRow[3].ToString();
                    if (!_dataRow.IsNull("Debit Amount"))
                        _NostroTransaction.TransactionAmount = Convert.ToDecimal(_dataRow[4].ToString());
                    else
                        _NostroTransaction.TransactionAmount = Convert.ToDecimal(_dataRow[5].ToString());
                    _NostroTransaction.BalanceAmount = Convert.ToDecimal(_dataRow[6].ToString());
                    _NostroTransaction.Description = _dataRow[7].ToString();

                    if (_dataRow[7].ToString().ToLower().Contains("esewa,"))
                    {
                        var firstsplit = _dataRow[7].ToString().Split(',');
                        var secondsplit = firstsplit[1].Split(':');
                        _NostroTransaction.UniqueId = secondsplit[0];
                    }
                    else if (_dataRow[7].ToString().ToLower().Contains(":esewa"))
                    {
                        var firstsplit = _dataRow[7].ToString().Split('-');
                        var secondsplit = firstsplit[1].Split(':');
                        _NostroTransaction.UniqueId = secondsplit[0];
                    }
                    else if (_dataRow[7].ToString().ToLower().Contains("fl:") && !_dataRow[7].ToString().ToLower().Contains("fp:"))
                    {
                        var firstsplit = _dataRow[7].ToString().Split(':');
                        _NostroTransaction.UniqueId = firstsplit[1];
                    }
                    else if (_dataRow[7].ToString().ToLower().Contains("fp:"))
                    {
                        if (_dataRow[7].ToString().ToLower().Contains("fp:") && _dataRow[7].ToString().Contains("-"))
                        {
                            var firstsplit = _dataRow[7].ToString().Split('-');
                            _NostroTransaction.UniqueId = firstsplit[1];
                        }
                        else if (_dataRow[7].ToString().ToLower().Contains("fp:"))
                        {
                            var firstsplit = _dataRow[7].ToString().Split(':');
                            _NostroTransaction.UniqueId = firstsplit[1];
                        }
                    }
                    else if (_dataRow[7].ToString().ToLower().Contains("mc:") && !_dataRow[7].ToString().ToLower().Contains("esewa"))
                    {
                        var firstsplit = _dataRow[7].ToString().Split(':');
                        _NostroTransaction.UniqueId = firstsplit[1];
                    }
                    else if (_dataRow[7].ToString().ToLower().Contains("ew:") && !_dataRow[7].ToString().ToLower().Contains("esewa"))
                    {
                        var firstsplit = _dataRow[7].ToString().Split(':');
                        _NostroTransaction.UniqueId = firstsplit[1];
                    }
                    else if (_dataRow[7].ToString().ToLower().Contains("ce-") && !_dataRow[7].ToString().ToLower().Contains("esewa"))
                    {
                        var firstsplit = _dataRow[7].ToString().Split(':');
                        var secondsplit = firstsplit[0].Split('-');
                        _NostroTransaction.UniqueId = secondsplit[1];
                    }
                    else
                    {
                        _NostroTransaction.UniqueId = _NostroTransaction.Description;
                    }
                    return _NostroTransaction;
                }
                else
                    return _NostroTransaction = null;

            }
            catch
            {
                throw;
            }
        }
    }
}
