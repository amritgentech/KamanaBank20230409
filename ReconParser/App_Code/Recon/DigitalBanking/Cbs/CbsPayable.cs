using Db;
using Db.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReconParser.App_Code.Recon.DigitalBanking.Cbs
{
    public class CbsPayable : DigitalBanking
    {
        public CbsPayable(String FileName, int FileCount)
            : base(FileName, FileCount)
        {
            _SubSource = SubSource.Find_By_Name("Cbs");
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
                CbsTransaction _CbsTransactions = GetNostroTransaction(_dataRow);
                if (_CbsTransactions != null)
                    CbsTransactions.Add(_CbsTransactions);
            }
        }

        protected CbsTransaction GetNostroTransaction(DataRow _dataRow)
        {
            try
            {
                var rowNull = _dataRow.ItemArray.All(c => c is DBNull);
                CbsTransaction _CbsTransaction = new CbsTransaction();
                if (!rowNull)
                {
                    DateTimeFormatInfo dateFormat = new DateTimeFormatInfo();
                    dateFormat.ShortDatePattern = @"yyyyMMdd";

                    _CbsTransaction.TransactionDate = Convert.ToDateTime(_dataRow[0], dateFormat);
                    _CbsTransaction.Particulars = _dataRow[1].ToString();

                    if (_dataRow["Debit"] != DBNull.Value && Convert.ToDouble(_dataRow[2].ToString()) > 0.00)
                    {
                        _CbsTransaction.TransactionAmount = Convert.ToDecimal(_dataRow[2].ToString());
                        _CbsTransaction.DebitOrCredit = "Debit";
                    }


                    else if (_dataRow["Credit"] != DBNull.Value && Convert.ToDouble(_dataRow[3].ToString()) > 0.00)
                    {
                        _CbsTransaction.TransactionAmount = Convert.ToDecimal(_dataRow[3].ToString());
                        _CbsTransaction.DebitOrCredit = "Credit";
                    }


                    _CbsTransaction.Balance = _dataRow[4].ToString();
                    _CbsTransaction.ReferenceNumber = _dataRow[5].ToString();
                    using (var context = new ReconContext())
                    {
                        var subChildSourceName = context.SubChildSources.Where(x => x.SubChildSourceId == _SubChildSource.SubChildSourceId).Select(x => x.Description).FirstOrDefault();
                        if (subChildSourceName == "FonepayIbftParking" && Convert.ToDouble(_dataRow[3].ToString()) > 0.00)
                        {
                            if (_dataRow[1].ToString().ToLower().Contains("ebank"))
                            {
                                var firstsplit = _dataRow[1].ToString().Split('-');
                                _CbsTransaction.IbftCreditUniqueId = firstsplit[1];
                            }
                            else if (_dataRow[1].ToString().ToLower().Contains("mc:"))
                            {
                                var firstsplit = _dataRow[1].ToString().Split(':');
                                var secondsplit = firstsplit[1].Split(' ');
                                _CbsTransaction.IbftCreditUniqueId = secondsplit[0];
                            }
                            else
                            {
                                var firstsplit = _dataRow[1].ToString().Split(',');
                                var secondsplit = firstsplit[0].Split(' ');
                                _CbsTransaction.IbftCreditUniqueId = secondsplit[1];
                            }
                        }
                    }
                    return _CbsTransaction;
                }
                else
                    return _CbsTransaction = null;

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
