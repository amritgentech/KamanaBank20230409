using Db.Enum;
using Db.Model;
using ReadWriteFiles;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReconParser.App_Code.Recon.CBS.Finacle
{
    public class Finacle: Cbs
    {
        public Finacle(String FileName, int FileCount)
            : base(FileName,FileCount)
        {
            _SubSource = SubSource.Find_By_Name("Finacle");
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
                DataTable dt = DataReadFromExcel.ReadExcelFileCBS(FileName);
                GetTransactions(dt);
            }
            catch (Exception ex) {
                Console.Write("Erro->Finacle->ReadDataFromFile->" + ex.Message);
            }
        }

        public void GetTransactions(DataTable _TransactionDataTable)
        {
            foreach (DataRow _dataRow in _TransactionDataTable.Rows)
            {
                Transaction _Transaction = GetTransation(_dataRow);
                Transactions.Add(_Transaction);
            }
        }

        protected Transaction GetTransation(DataRow _dataRow)
        {
            try
            {
                TransactionType _TransactionType = TransactionType.BalInquiry;
                TransactionStatus _TransactionStatus = TransactionStatus.NotDefine;
                CardType _CardType = CardType.NotDefine;
                NetworkType _NetworkType = NetworkType.NotDefine;
                TerminalType _TerminalType = TerminalType.ATM;
                TerminalOwner _TerminalOwner = TerminalOwner.NotDefine;

                Transaction _Transaction = new Transaction();

                _Transaction.TerminalId = _dataRow[0].ToString();
                DateTime txnDate = Convert.ToDateTime(_dataRow[1]);
                _Transaction.TransactionDate = txnDate;
                string time = txnDate.ToString("HH:mm:ss");
                _Transaction.TransactionTime = TimeSpan.Parse(time);

                _Transaction.CardNo = _dataRow[2].ToString();
                _Transaction.TraceNo = _dataRow[3].ToString();
                _Transaction.ReferenceNo = _dataRow[4].ToString();
                _Transaction.AuthCode = _dataRow[5].ToString();
                _Transaction.ProcessingCode = _dataRow[6].ToString();
                _Transaction.TransactionAmount = Convert.ToDecimal(_dataRow[7].ToString());
                _Transaction.ResponseCode = GetResponseCode(_dataRow[8].ToString());


               _TransactionStatus = TransactionStatus.Success;
                if (!_Transaction.ResponseCode.Equals("0000"))
                {
                    _TransactionStatus = TransactionStatus.Fail;
                }

                string typeOfTerminal = _dataRow[9].ToString();
                if (typeOfTerminal.Equals("ATM"))
                {
                    _TerminalType = TerminalType.ATM;
                }
                else if (typeOfTerminal.Equals("POS"))
                {
                    _TerminalType = TerminalType.POS;
                }


                if (_Transaction.TransactionAmount >= 500 || (_Transaction.TransactionAmount < 500 && _TerminalType == TerminalType.POS))
                {
                    _TransactionType = TransactionType.Financial;
                }
                else if (_Transaction.TransactionAmount < 500 && _TerminalType == TerminalType.ATM)
                {
                    _TransactionType = TransactionType.TransactionCharge;
                }


                _TransactionStatus = (TransactionStatus)Convert.ToInt32(_dataRow[10]);
                if (_dataRow.Table.Columns.Count > 11)
                {
                    int networkType = Convert.ToInt32(_dataRow[11]);
                    if (networkType > 0)
                    {
                        _NetworkType = (NetworkType)networkType;
                    }
                }

                if (_dataRow.Table.Columns.Count > 12)
                {
                    _Transaction.Currency = _dataRow[12].ToString();
                    if (_Transaction.Currency.ToLower() == "usd")
                    {
                        _TerminalOwner = TerminalOwner.ForeignTerminal;

                        if (_Transaction.TransactionAmount < 500 && _TransactionType == TransactionType.TransactionCharge)
                        {
                            _TransactionType = TransactionType.Financial;
                        }
                    }
                }

                if (_dataRow.Table.Columns.Count > 13)
                {
                    _Transaction.TransactionNo = _dataRow[13].ToString();
                }

                if (_dataRow.Table.Columns.Count > 14)
                {
                    _Transaction.AccountNo = _dataRow[14].ToString();
                }

                _Transaction = GetTransaction(_Transaction, _TransactionType, _TransactionStatus, _TerminalType, _CardType, _TerminalOwner, _NetworkType);

                return _Transaction;
            }
            catch
            {
                throw;
            }
        }
    }
}
