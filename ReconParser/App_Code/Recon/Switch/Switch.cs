using Db.Enum;
using Db.Model;
using ReadWriteFiles;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReconParser.App_Code.Recon.Switch
{
    class Switch : Base
    {
        public Switch(String FileName, int FileCount)
            : base(FileName, FileCount)
        {
            _Source = Source.FindName("SWITCH");
        }

        public override void Parse()
        {
            Console.WriteLine("Parsing File {0}", FileName);

            ReadDataFromFile(FileName);

            Console.WriteLine("Parsed File {0}", FileName);
        }

        public override void DataReadFromDatabase()
        {
            Console.WriteLine("Data pulling Start...");

            //            foreach (Terminal _Terminal in Terminals)
            //            {
            //                try
            //                {
            //                    AutomaticDataPullingService _AutomaticDataPullingService = new AutomaticDataPullingService();
            //                    DataTable dt = _AutomaticDataPullingService.GetSwitchData(_Terminal.TerminalId, DateTime.Today.AddDays(-2).ToString("dd/MM/yyyy"), DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy"));
            //                    GetTransactions(dt);
            //                }
            //                catch (Exception ex)
            //                {
            //                    Console.Write("Erro->Switch->DataReadFromDatabase->" + ex.Message);
            //                }
            //            }
            Console.WriteLine("Data pulling Complete...");
        }

        public void ReadDataFromFile(string FileName)
        {
            try
            {
                DataTable dt = DataReadFromExcel.ReadExcelFileCBS(FileName);
                GetTransactions(dt);
            }
            catch (Exception ex)
            {
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

                //                card_no,sys_trace_no,rrno,authid,proccode,txnamt,respons
                //e_code,reversal_flag,
                //device_type from authctl

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

                _TransactionType = TransactionType.BalInquiry;
                if (_Transaction.TransactionAmount > 0)
                {
                    _TransactionType = TransactionType.Financial;
                }

                _TransactionStatus = TransactionStatus.Success;
                if (!_Transaction.ResponseCode.Equals("0000"))
                {
                    _TransactionStatus = TransactionStatus.Fail;
                }

                string reversalFlag = _dataRow[9].ToString();
                if (reversalFlag.Equals("Y"))
                {
                    _TransactionStatus = TransactionStatus.Reversal;
                }
                else if (reversalFlag.Equals("T"))
                {
                    _TransactionStatus = TransactionStatus.Sensor_Failure;
                }

                string terminalType = _dataRow[10].ToString();
                if (terminalType.ToUpper() == "ATM")
                {
                    _TerminalType = TerminalType.ATM;
                }
                else if (terminalType.ToUpper() == "POS")
                {
                    _TerminalType = TerminalType.POS;
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
