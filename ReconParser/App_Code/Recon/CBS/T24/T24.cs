using Db.Enum;
using Db.Model;
using ReadWriteFiles;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using BAL;
using Helper.GlobalHelpers;

namespace ReconParser.App_Code.Recon.CBS.T24
{
    public class T24 : Cbs
    {
        public T24(String FileName, int FileCount)
            : base(FileName, FileCount)
        {
            _Source = Source.CBS();
            _SubSource = SubSource.Find_By_Name("T24");
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
            try
            {
                DataTable dt = CBSDataPulling.GetT24CBSData(DateTime.Today.AddDays(-2).ToString("dd-MMM-yyyy"), DateTime.Today.AddDays(-1).ToString("dd-MMM-yyyy"));
                GetTransactionsAuto(dt);
            }
            catch (Exception ex)
            {
                Console.Write("Erro->T24->DataReadFromDatabase->" + ex.Message);
            }

            Console.WriteLine("Data pulling Complete...");
        }

        public void ReadDataFromFile(string FileName)
        {
            try
            {
                string contentType = "";
                string ext = System.IO.Path.GetExtension(FileName).ToLower();
                Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
                if (registryKey != null && registryKey.GetValue("Content Type") != null)
                    contentType = registryKey.GetValue("Content Type").ToString();


                if (contentType.Equals("text/plain"))
                {
                    List<string> listStr = ReadDataFromTxt(FileName);
                    GetTransactions(listStr);
                }
                else
                {
                    DataTable dt = DataReadFromExcel.ReadExcelFileCBS(FileName);
                    GetTransactions(dt);
                }
            }
            catch (Exception ex)
            {
                Console.Write("Erro->T24->ReadDataFromFile->" + ex.Message);
            }
        }

        public List<string> ReadDataFromTxt(string fileName)
        {
            return File.ReadAllLines(fileName).ToList();
        }

        public void GetTransactions(List<string> listStr)
        {
            foreach (string line in listStr)
            {
                Transaction _Transaction = TransasctionFromFilePerLine(line);
                Transactions.Add(_Transaction);
            }
        }

        protected Transaction TransasctionFromFilePerLine(string line)
        {
            try
            {
                TransactionType _TransactionType = TransactionType.BalInquiry;
                TransactionStatus _TransactionStatus = TransactionStatus.NotDefine;
                CardType _CardType = CardType.NotDefine;
                NetworkType _NetworkType = NetworkType.VISA;
                TerminalType _TerminalType = TerminalType.ATM;
                TerminalOwner _TerminalOwner = TerminalOwner.NotDefine;

                string[] ejournalDetailList = line.Split('|');
                Transaction _Transaction = new Transaction();

                if (ejournalDetailList[1].Equals("ACPS"))
                {
                    _TerminalType = TerminalType.POS;
                }

                decimal amount = Convert.ToDecimal(ejournalDetailList[2]);
                if (amount < 0)
                {
                    _TransactionStatus = TransactionStatus.Reversal;
                }


                var isReversalOrNot = ejournalDetailList[0].Split(';');
                if (isReversalOrNot.Length == 2)
                {
                    if (Convert.ToInt32(isReversalOrNot[1]) == 2) //reversal transaction 

                    {
                        _TransactionStatus = TransactionStatus.Reversal;
                    }
                }

                string ftBranchCodeStr = ejournalDetailList[0] + "\\" + ejournalDetailList[4];
                string curr = ejournalDetailList[3].ToString().Substring(4, 3);
                if (ejournalDetailList[9].ToString().StartsWith("9999"))
                {
                    curr = ejournalDetailList[9].ToString().Substring(4, 3);
                }

                if (curr.Equals("356"))
                {
                    _TerminalOwner = TerminalOwner.INRTerminal;
                }
                else if (curr.Equals("840"))
                {
                    _TerminalOwner = TerminalOwner.ForeignTerminal;
                }



                string date = ejournalDetailList[5].ToString();
                int year = Convert.ToInt32(date.Substring(0, 4));
                int month = Convert.ToInt32(date.Substring(4, 2));
                int day = Convert.ToInt32(date.Substring(6, 2));
                DateTime txnDate = new DateTime(year, month, day);
                DateTimeFormatInfo dateFormat = new DateTimeFormatInfo();
                dateFormat.ShortDatePattern = @"yyMMdd";
                string uiqueId = ejournalDetailList[7].ToString();
                if (string.IsNullOrEmpty(uiqueId))
                {
                    return null;
                }
                string datetime = ejournalDetailList[7].ToString().Substring(6, 6);
                year = Convert.ToInt32("20" + datetime.Substring(0, 2));
                month = Convert.ToInt32(datetime.Substring(2, 2));
                day = Convert.ToInt32(datetime.Substring(4, 2));

                _Transaction.TransactionAmount = Math.Abs(amount);
                _Transaction.CBSRefValue = ftBranchCodeStr;
                _Transaction.TransactionDate = new DateTime(year, month, day);
                _Transaction.CardNo = ejournalDetailList[8].ToString();
                _Transaction.TraceNo = ejournalDetailList[12].ToString();
                _Transaction.AuthCode = ejournalDetailList[13].ToString();
                _Transaction.ResponseCode = "0000";
                _Transaction.CBSValueDate = txnDate;
                _Transaction.TerminalId = ejournalDetailList[6].ToString();

                _TransactionType = TransactionType.Financial;

                if (_Transaction.TransactionAmount <= 250 && _TerminalType == TerminalType.ATM)
                {
                    _TransactionType = TransactionType.TransactionCharge;
                }

                if (_TransactionStatus == TransactionStatus.NotDefine)
                {
                    _TransactionStatus = TransactionStatus.Success;
                    if (!_Transaction.ResponseCode.Equals("0000"))
                    {
                        _TransactionStatus = TransactionStatus.Fail;
                    }
                }

                dateFormat = new DateTimeFormatInfo();
                dateFormat.ShortDatePattern = @"yMMddHHmm";
                string dateTimeStr = ejournalDetailList[ejournalDetailList.Length - 1].ToString();
                int hr = Convert.ToInt32(dateTimeStr.Substring(dateTimeStr.Length - 4, 2));
                int min = Convert.ToInt32(dateTimeStr.Substring(dateTimeStr.Length - 2, 2));
                _Transaction.TransactionTime = new TimeSpan(hr, min, 0);

                _Transaction = GetTransaction(_Transaction, _TransactionType, _TransactionStatus, _TerminalType, _CardType, _TerminalOwner, _NetworkType);

                if (ejournalDetailList[9].ToString().Substring(0, 7).Equals("NPR1010"))
                {
                    _Transaction.TerminalOwner = TerminalOwner.OwnTerminal;
                }
                if (_Transaction.CardNo.Substring(0, 6).Equals("402064")) //credit card bin no..//set default receivable in case of credit card...
                {
                    _Transaction.TerminalOwner = TerminalOwner.OwnTerminal;
                    _Transaction.CardType = CardType.CreditCard;   // off us  or recievable..
                }
                if (ejournalDetailList[9].ToString().Equals("9999524999998532"))
                {
                    _Transaction.TerminalOwner = TerminalOwner.OwnTerminal;
                }
                _Transaction.SubSource = _SubSource;
                _Transaction.Source = _Source;
                return _Transaction;
            }
            catch
            {
                Console.WriteLine("Error in File: " + FileName + " Line: " + line);
            }
            return null;
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
                NetworkType _NetworkType = NetworkType.VISA;
                TerminalType _TerminalType = TerminalType.ATM;
                TerminalOwner _TerminalOwner = TerminalOwner.NotDefine;

                Transaction _Transaction = new Transaction();

                if (_dataRow[1].Equals("ACPS"))
                {
                    _TerminalType = TerminalType.POS;
                }

                try
                {
                    if (Convert.ToDecimal(_dataRow[2].ToString()) < 0)
                    {
                        _Transaction.TransactionAmount = Math.Abs((Convert.ToDecimal(_dataRow[2].ToString())));
                        _TransactionStatus = TransactionStatus.Reversal;
                    }
                    else
                        _Transaction.TransactionAmount = (Convert.ToDecimal(_dataRow[2].ToString()));
                }
                catch
                {
                    throw;
                }

                string ftBranchCodeStr = _dataRow[0].ToString() + "/" + _dataRow[4].ToString();
                _Transaction.CBSRefValue = ftBranchCodeStr;

                string curr = _dataRow[3].ToString().Substring(4, 3);
                if (_dataRow[9].ToString().StartsWith("9999"))
                {
                    curr = _dataRow[9].ToString().Substring(4, 3);
                }

                if (curr.Equals("356"))
                {
                    _TerminalOwner = TerminalOwner.INRTerminal;
                }
                else if (curr.Equals("840"))
                {
                    _TerminalOwner = TerminalOwner.ForeignTerminal;
                }

                //string date = _dataRow[5].ToString();
                //int year = Convert.ToInt32(date.Substring(0, 4));
                //int month = Convert.ToInt32(date.Substring(4, 2));
                //int day = Convert.ToInt32(date.Substring(6, 2));

                //                DateTime txnDate = Convert.ToDateTime(_dataRow[5]);// new DateTime(year, month, day);
                DateTime txnDate = DateTime.ParseExact(_dataRow[5].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);

                _Transaction.CBSValueDate = txnDate;
                //_Transaction.AdviseDate = txnDate;

                _Transaction.TerminalId = _dataRow[6].ToString();

                DateTimeFormatInfo dateFormat = new DateTimeFormatInfo();
                dateFormat.ShortDatePattern = @"yyMMdd";
                string uiqueId = _dataRow[7].ToString();
                if (string.IsNullOrEmpty(uiqueId))
                {
                    return null;
                }
                //string datetime = _dataRow[7].ToString().Substring(6, 6);
                //int year = Convert.ToInt32("20" + datetime.Substring(0, 2));
                //int month = Convert.ToInt32(datetime.Substring(2, 2));
                //int day = Convert.ToInt32(datetime.Substring(4, 2));

                // _Transaction.TransactionDate = new DateTime(year, month, day);

                _Transaction.CardNo = _dataRow[8].ToString();

                _Transaction.ReferenceNo = _dataRow[12].ToString();

                if (!string.IsNullOrEmpty(_Transaction.ReferenceNo))
                {
                    _Transaction.TraceNo = _Transaction.ReferenceNo.Substring(_Transaction.ReferenceNo.Length - 6, 6);
                }

                //_Transaction.AuthCode = _dataRow[13].ToString();
                _Transaction.ResponseCode = "0000";

                if (_Transaction.TransactionAmount > 0)
                {
                    _TransactionType = TransactionType.Financial;
                }

                if (_Transaction.TransactionAmount <= 250 && _TerminalType == TerminalType.ATM && _TransactionType == TransactionType.Financial)
                {
                    _TransactionType = TransactionType.TransactionCharge;
                }

                if (_TransactionStatus == TransactionStatus.NotDefine)
                {
                    _TransactionStatus = TransactionStatus.Success;
                    if (!_Transaction.ResponseCode.Equals("0000"))
                    {
                        _TransactionStatus = TransactionStatus.Fail;
                    }
                }
                _Transaction.TransactionDate = DateTime.ParseExact(_dataRow[14].ToString(), "yyMMdd", CultureInfo.InvariantCulture);
                //string timeStamp = _dataRow[14].ToString();
                //int hr = int.Parse(timeStamp.Substring(0, 2));
                //int min = int.Parse(timeStamp.Substring(2, timeStamp.Length));
                //_Transaction.TransactionTime = Convert.Ti(_dataRow[14].ToString());

                _Transaction = GetTransaction(_Transaction, _TransactionType, _TransactionStatus, _TerminalType, _CardType, _TerminalOwner, _NetworkType);

                return _Transaction;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void GetTransactionsAuto(DataTable _TransactionDataTable)
        {
            foreach (DataRow _dataRow in _TransactionDataTable.Rows)
            {
                Transaction _Transaction = GetTransation(_dataRow);
                Transactions.Add(_Transaction);
            }
        }

        protected Transaction GetTransationAuto(DataRow _dataRow)
        {
            try
            {
                TransactionType _TransactionType = TransactionType.BalInquiry;
                TransactionStatus _TransactionStatus = TransactionStatus.NotDefine;
                CardType _CardType = CardType.NotDefine;
                NetworkType _NetworkType = NetworkType.VISA;
                TerminalType _TerminalType = TerminalType.ATM;
                TerminalOwner _TerminalOwner = TerminalOwner.NotDefine;

                Transaction _Transaction = new Transaction();

                if (_dataRow[1].Equals("ACPS"))
                {
                    _TerminalType = TerminalType.POS;
                }

                try
                {
                    if (Convert.ToDecimal(_dataRow[2].ToString()) < 0)
                    {
                        _Transaction.TransactionAmount = Math.Abs((Convert.ToDecimal(_dataRow[2].ToString())));
                        _TransactionStatus = TransactionStatus.Reversal;
                    }
                    else
                        _Transaction.TransactionAmount = (Convert.ToDecimal(_dataRow[2].ToString()));
                }
                catch
                {
                    throw;
                }

                string ftBranchCodeStr = _dataRow[0].ToString() + "/" + _dataRow[4].ToString();

                var isReversalOrNot = ftBranchCodeStr.Split(';');
                if (isReversalOrNot.Length == 2)
                {
                    if (Convert.ToInt32(isReversalOrNot[1]) == 2) //reversal transaction 
                    {
                        _TransactionStatus = TransactionStatus.Reversal;
                    }
                }
                _Transaction.CBSRefValue = ftBranchCodeStr;

                string curr = _dataRow[3].ToString().Substring(4, 3);
                if (_dataRow[9].ToString().StartsWith("9999"))
                {
                    curr = _dataRow[9].ToString().Substring(4, 3);
                }

                if (curr.Equals("356"))
                {
                    _TerminalOwner = TerminalOwner.INRTerminal;
                }
                else if (curr.Equals("840"))
                {
                    _TerminalOwner = TerminalOwner.ForeignTerminal;
                }

                string date = _dataRow[5].ToString();
                int year = Convert.ToInt32(date.Substring(0, 4));
                int month = Convert.ToInt32(date.Substring(4, 2));
                int day = Convert.ToInt32(date.Substring(6, 2));

                DateTime txnDate = new DateTime(year, month, day);

                _Transaction.CBSValueDate = txnDate;
                //_Transaction.AdviseDate = txnDate;

                _Transaction.TerminalId = _dataRow[6].ToString();

                DateTimeFormatInfo dateFormat = new DateTimeFormatInfo();
                dateFormat.ShortDatePattern = @"yyMMdd";
                string uiqueId = _dataRow[7].ToString();
                if (string.IsNullOrEmpty(uiqueId))
                {
                    return null;
                }
                string datetime = _dataRow[7].ToString().Substring(6, 6);
                year = Convert.ToInt32("20" + datetime.Substring(0, 2));
                month = Convert.ToInt32(datetime.Substring(2, 2));
                day = Convert.ToInt32(datetime.Substring(4, 2));

                _Transaction.TransactionDate = new DateTime(year, month, day);

                _Transaction.CardNo = _dataRow[8].ToString();

                _Transaction.TraceNo = _dataRow[12].ToString();
                _Transaction.AuthCode = _dataRow[13].ToString();
                _Transaction.ResponseCode = "0000";

                if (_Transaction.TransactionAmount > 0)
                {
                    _TransactionType = TransactionType.Financial;
                }

                if (_Transaction.TransactionAmount <= 250 && _TerminalType == TerminalType.ATM && _TransactionType == TransactionType.Financial)
                {
                    _TransactionType = TransactionType.TransactionCharge;
                }

                if (_TransactionStatus == TransactionStatus.NotDefine)
                {
                    _TransactionStatus = TransactionStatus.Success;
                    if (!_Transaction.ResponseCode.Equals("0000"))
                    {
                        _TransactionStatus = TransactionStatus.Fail;
                    }
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
