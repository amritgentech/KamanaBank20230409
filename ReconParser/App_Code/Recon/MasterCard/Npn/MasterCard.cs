using Db.Enum;
using Db.Model;
using ReadWriteFiles;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helper.GlobalHelpers;

namespace ReconParser.App_Code.Recon.MasterCard.Npn
{
    public class MasterCardNpn : Base
    {
        public MasterCardNpn(String FileName, int FileCount)
            : base(FileName, FileCount)
        {
            _Source = Source.FindName("MasterCard");
            _SubSource = SubSource.Find_By_SourceId(_Source.SourceId);
        }

        public override void Parse()
        {
            Console.WriteLine("Parsing File {0}", FileName);
            DataTable _DataTable = ReadDataFromFile(FileName);
            //            ReadDataFromFile(FileName);

            GetTransactionList(_DataTable);

            Console.WriteLine("Parsed File {0}", FileName);
        }
        public DataTable ReadDataFromFile(string FileName)
        {
            DataTable dt = new DataTable();
            try
            {
                try
                {
                    dt = DataReadFromExcel.ReadExcelFileNPN(FileName, "Sheet 1");
                }
                catch
                {
                    dt = DataReadFromExcel.ReadExcelFileNPN(FileName, "Sheet1");
                }
            }
            catch
            {
                string sheetName = System.IO.Path.GetFileNameWithoutExtension(FileName);
                dt = DataReadFromExcel.ReadExcelFileNPN(FileName, sheetName);
            }
            return dt;
        }
        //        public void ReadDataFromFile(string FileName)
        //        {
        //            DataTable dt = new DataTable();
        //            try
        //            {
        //                ExcelFileReadingClass testCell = new ExcelFileReadingClass();
        //                dt = testCell.ReadExcelFileEPPUnmerged(FileName);
        //                GetTransactionList(dt);
        //            }
        //            catch
        //            {
        //                dt = DataReadFromExcel.ReadExcelFile(FileName);
        //                GetTransactionList(dt);
        //            }
        //        }

        public void GetTransactionList(DataTable excelDataTable)
        {
            //ejournalList = new List<EjournalAndNTDetail>();
            int cnt = 0;
            try
            {
                foreach (DataRow dr in excelDataTable.Rows)
                {
                    cnt++;
                    Transaction _Transaction = null;

                    if (excelDataTable.Columns.Count == 8 || excelDataTable.Columns.Count == 10 || excelDataTable.Columns.Count == 11 ||
                        excelDataTable.Columns.Count == 12)
                    {
                        if (CheckForTransactionData(dr))
                        {
                            DateTimeFormatInfo dateFormatInfo = new DateTimeFormatInfo();
                            dateFormatInfo.ShortDatePattern = @"M/d/yyyy h:mm:ss";
                            DateTimeFormatInfo dateFormatInfo2 = new DateTimeFormatInfo();
                            dateFormatInfo2.ShortDatePattern = @"d/M/yyyy h:mm:ss";
                            string isdate = dr[0].ToString();
                            DateTime date;

                            if (DateTime.TryParse(isdate, dateFormatInfo, DateTimeStyles.None, out date))
                            {
                                _Transaction = GetEjournalAndNTDetailForNPN(dr);
                            }
                            else if (DateTime.TryParse(isdate, dateFormatInfo2, DateTimeStyles.None, out date))
                            {
                                _Transaction = GetEjournalAndNTDetailForNPN(dr);
                            }
                            else if (dr[3].ToString().Contains("SALE"))
                            {
                                _Transaction = GetEjournalAndNTDetailForPayableReceivablePOS(dr);
                            }
                            else
                            {
                                _Transaction = GetEjournalAndNTDetailForPayableReceivableATM(dr);
                            }
                        }
                    }
                    else if (excelDataTable.Columns.Count == 15)
                    {
                        if (CheckForTransactionData(dr))
                        {
                            _Transaction = GetEjournalAndNTDetailForPayableReceivableBalanceInquiryATM(dr);
                        }
                    }
                    else if (excelDataTable.Columns.Count == 18 || excelDataTable.Columns.Count == 19 || excelDataTable.Columns.Count == 21)
                    {
                        if (!string.IsNullOrEmpty(dr[0].ToString()))
                        {
                            if (excelDataTable.Columns.Count == 21)
                            {
                                _Transaction = GetEjournalAndNTDetailForMasterReceivableATM(dr);
                            }
                            else
                            {
                                _Transaction = GetEjournalAndNTDetailForMasterReceivableATM_Old(dr);
                            }
                        }
                    }

                    if (_Transaction != null)
                    {
                        //if (_Transaction.IS_ONUS_OFFUS == null)
                        //{
                        //    _Transaction = GetEjournalAndNtDetailByCheckingLoro(_Transaction, binNo, listMemberBinNo);
                        //}
                        Transactions.Add(_Transaction);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Transaction GetEjournalAndNTDetailForNPN(DataRow dr)
        {
            try
            {
                Transaction _Transaction = new Transaction();
                DateTimeFormatInfo dateFormatInfo = new DateTimeFormatInfo();
                dateFormatInfo.ShortDatePattern = @"M/d/yyyy h:mm:ss";
                string datetime = dr[0].ToString();
                DateTime txnDate = Convert.ToDateTime(datetime, dateFormatInfo);
                _Transaction.TransactionDate = txnDate;
                string time = txnDate.ToString("HH:mm:ss");
                _Transaction.TransactionTime = TimeSpan.Parse(time);
                _Transaction.CardNo = dr[1].ToString();
                _Transaction.AuthCode = dr[2].ToString();
                _Transaction.ReferenceNo = dr[3].ToString();
                _Transaction.TraceNo = GetTraceno(dr[4].ToString());
                _Transaction.TransactionAmount = Convert.ToDecimal(dr[5].ToString());
                _Transaction.ProcessingCode = dr[7].ToString();
                _Transaction.ResponseCode = GetResponseCode(dr[8].ToString());
                _Transaction.TerminalId = dr[9].ToString();

                TransactionType _TransactionType = TransactionType.Financial;
                TransactionStatus _TransactionStatus = TransactionStatus.Success;
                CardType _CardType = CardType.NotDefine;
                NetworkType _NetworkType = NetworkType.NPN;
                TerminalType _TerminalType = TerminalType.ATM;
                TerminalOwner _TerminalOwner = TerminalOwner.NotDefine;

                if (_Transaction.TransactionAmount == 0)
                {
                    _TransactionType = TransactionType.BalInquiry;
                }

                _Transaction = GetTransaction(_Transaction, _TransactionType, _TransactionStatus, _TerminalType, _CardType, _TerminalOwner, _NetworkType);

                return _Transaction;
            }
            catch
            {
                throw;
            }
        }

        public Transaction GetEjournalAndNTDetailForPayableReceivableATM(DataRow dr)
        {
            try
            {
                Transaction _Transaction = new Transaction();
                DateTimeFormatInfo dateFormatInfo = new DateTimeFormatInfo();
                dateFormatInfo.ShortDatePattern = @"MM/dd/yyyy h:mm:ss";

                try
                {
                    DateTime txnDate = Convert.ToDateTime(dr[2], dateFormatInfo);
                    _Transaction.TransactionDate = txnDate;
                    string time = txnDate.ToString("HH:mm:ss");
                    _Transaction.TransactionTime = TimeSpan.Parse(time);
                }
                catch
                {
                    dateFormatInfo.ShortDatePattern = @"dd/MM/yyyy";
                    string datetime = dr[2].ToString();
                    DateTime txnDate = Convert.ToDateTime(dr[2], dateFormatInfo);
                    _Transaction.TransactionDate = txnDate;
                    string time = txnDate.ToString("HH:mm:ss");
                    _Transaction.TransactionTime = TimeSpan.Parse(time);
                }

                _Transaction.TerminalId = dr[3].ToString();

                _Transaction.CardNo = dr[4].ToString();
                _Transaction.TraceNo = GetTraceno(dr[5].ToString());
                _Transaction.ReferenceNo = dr[6].ToString();
                _Transaction.TransactionAmount = Convert.ToDecimal(dr[7].ToString());
                if (dr.Table.Columns.Count > 8)
                {
                    _Transaction.AuthCode = dr[8].ToString();
                }
                _Transaction.ResponseCode = "0000";

                TransactionType _TransactionType = TransactionType.Financial;
                TransactionStatus _TransactionStatus = TransactionStatus.Success;
                CardType _CardType = CardType.NotDefine;
                NetworkType _NetworkType = NetworkType.NPN;
                TerminalType _TerminalType = TerminalType.ATM;
                TerminalOwner _TerminalOwner = TerminalOwner.NotDefine;

                if (_Transaction.TransactionAmount == 0)
                {
                    _TransactionType = TransactionType.BalInquiry;
                }

                _Transaction = GetTransaction(_Transaction, _TransactionType, _TransactionStatus, _TerminalType, _CardType, _TerminalOwner, _NetworkType);

                return _Transaction;
            }
            catch
            {
                throw;
            }
        }

        public Transaction GetEjournalAndNTDetailForPayableReceivablePOS(DataRow dr)
        {
            try
            {
                Transaction _Transaction = new Transaction();
                DateTimeFormatInfo dateFormatInfo = new DateTimeFormatInfo();
                dateFormatInfo.ShortDatePattern = @"MM/dd/yyyy h:mm:ss";

                try
                {
                    string datetime = dr[1].ToString();
                    DateTime txnDate = Convert.ToDateTime(dr[1], dateFormatInfo);
                    _Transaction.TransactionDate = txnDate;
                    string time = txnDate.ToString("HH:mm:ss");
                    _Transaction.TransactionTime = TimeSpan.Parse(time);
                }
                catch
                {
                    dateFormatInfo.ShortDatePattern = @"dd/MM/yyyy";
                    string datetime = dr[1].ToString();
                    DateTime txnDate = Convert.ToDateTime(dr[1], dateFormatInfo);
                    _Transaction.TransactionDate = txnDate;
                    string time = txnDate.ToString("HH:mm:ss");
                    _Transaction.TransactionTime = TimeSpan.Parse(time);
                }

                _Transaction.TerminalId = dr[2].ToString();

                _Transaction.CardNo = dr[4].ToString();
                _Transaction.TraceNo = GetTraceno(dr[9].ToString());
                _Transaction.ReferenceNo = dr[6].ToString();
                _Transaction.TransactionAmount = Convert.ToDecimal(dr[7].ToString());
                _Transaction.AuthCode = dr[5].ToString();
                _Transaction.ResponseCode = "0000";

                dateFormatInfo = new DateTimeFormatInfo();
                dateFormatInfo.ShortDatePattern = @"MM-dd-yyyy";
                try
                {
                    _Transaction.AdviseDate = Convert.ToDateTime(dr[8].ToString(), dateFormatInfo);
                    if ((_Transaction.AdviseDate.Value > DateTime.Now) || (_Transaction.AdviseDate.Value.Month < DateTime.Now.Month))
                    {
                        try
                        {
                            dateFormatInfo.ShortDatePattern = @"dd-MM-yyyy";
                            _Transaction.AdviseDate = Convert.ToDateTime(dr[8].ToString(), dateFormatInfo);
                        }
                        catch
                        {
                            dateFormatInfo.ShortDatePattern = @"MM-dd-yyyy";
                            _Transaction.AdviseDate = Convert.ToDateTime(dr[8].ToString(), dateFormatInfo);
                        }
                    }
                }
                catch
                {
                    dateFormatInfo.ShortDatePattern = @"dd-MM-yyyy";
                    _Transaction.AdviseDate = Convert.ToDateTime(dr[8].ToString(), dateFormatInfo);
                }

                TransactionType _TransactionType = TransactionType.Financial;
                TransactionStatus _TransactionStatus = TransactionStatus.Success;
                CardType _CardType = CardType.NotDefine;
                NetworkType _NetworkType = NetworkType.NPN;
                TerminalType _TerminalType = TerminalType.POS;
                TerminalOwner _TerminalOwner = TerminalOwner.NotDefine;

                if (_Transaction.TransactionAmount == 0)
                {
                    _TransactionType = TransactionType.BalInquiry;
                }

                _Transaction = GetTransaction(_Transaction, _TransactionType, _TransactionStatus, _TerminalType, _CardType, _TerminalOwner, _NetworkType);

                return _Transaction;
            }
            catch
            {
                throw;
            }
        }

        public Transaction GetEjournalAndNTDetailForPayableReceivableBalanceInquiryATM(DataRow dr)
        {
            try
            {
                Transaction _Transaction = new Transaction();
                DateTimeFormatInfo dateFormatInfo = new DateTimeFormatInfo();
                dateFormatInfo.ShortDatePattern = @"MM/dd/yyyy h:mm:ss";
                try
                {
                    string datetime = dr[3].ToString();
                    DateTime txnDate = Convert.ToDateTime(datetime, dateFormatInfo);
                    _Transaction.TransactionDate = txnDate;
                    string time = txnDate.ToString("HH:mm:ss");
                    _Transaction.TransactionTime = TimeSpan.Parse(time);
                }
                catch
                {
                    throw;
                }
                _Transaction.TerminalId = dr[13].ToString();
                _Transaction.CardNo = dr[0].ToString();
                _Transaction.TraceNo = GetTraceno(dr[11].ToString());
                _Transaction.ReferenceNo = dr[10].ToString();
                _Transaction.AuthCode = dr[12].ToString();
                _Transaction.TransactionAmount = Convert.ToDecimal(dr[4].ToString());
                _Transaction.ResponseCode = "0000";

                TransactionType _TransactionType = TransactionType.Financial;
                TransactionStatus _TransactionStatus = TransactionStatus.Success;
                CardType _CardType = CardType.NotDefine;
                NetworkType _NetworkType = NetworkType.NPN;
                TerminalType _TerminalType = TerminalType.ATM;
                TerminalOwner _TerminalOwner = TerminalOwner.NotDefine;

                if (_Transaction.TransactionAmount == 0)
                {
                    _TransactionType = TransactionType.BalInquiry;
                }

                _Transaction = GetTransaction(_Transaction, _TransactionType, _TransactionStatus, _TerminalType, _CardType, _TerminalOwner, _NetworkType);

                return _Transaction;
            }
            catch
            {
                throw;
            }
        }

        public Transaction GetEjournalAndNTDetailForMasterReceivableATM_Old(DataRow dr)
        {
            try
            {
                Transaction _Transaction = new Transaction();
                DateTimeFormatInfo dateFormatInfo = new DateTimeFormatInfo();
                dateFormatInfo.ShortDatePattern = @"dd-MM-yyyy";

                string datetime = dr[1].ToString();
                DateTime txnDate = Convert.ToDateTime(datetime, dateFormatInfo);
                _Transaction.TransactionDate = txnDate;
                string time = dr[2].ToString();
                _Transaction.TransactionTime = TimeSpan.Parse(time);

                _Transaction.TerminalId = dr[11].ToString();
                _Transaction.CardNo = dr[0].ToString();

                //ejournalDetail.TXN_BANK = dr[1].ToString();
                _Transaction.TraceNo = GetTraceno(dr[8].ToString());
                _Transaction.ReferenceNo = dr[9].ToString();
                _Transaction.AuthCode = dr[13].ToString();
                _Transaction.TransactionAmount = 0;
                if (!string.IsNullOrEmpty(dr[6].ToString()))
                {
                    _Transaction.TransactionAmount = Convert.ToDecimal(dr[6].ToString());
                }
                _Transaction.ResponseCode = GetResponseCode(dr[12].ToString());

                TransactionType _TransactionType = TransactionType.Financial;
                TransactionStatus _TransactionStatus = TransactionStatus.Fail;
                CardType _CardType = CardType.NotDefine;
                NetworkType _NetworkType = NetworkType.NPN;
                TerminalType _TerminalType = TerminalType.ATM;
                TerminalOwner _TerminalOwner = TerminalOwner.ForeignTerminal;

                if (_Transaction.ResponseCode.Equals("0000"))
                {
                    _TransactionStatus = TransactionStatus.Success;
                }

                if (_Transaction.TransactionAmount == 0)
                {
                    _TransactionType = TransactionType.BalInquiry;
                }

                _Transaction = GetTransaction(_Transaction, _TransactionType, _TransactionStatus, _TerminalType, _CardType, _TerminalOwner, _NetworkType);

                return _Transaction;
            }
            catch
            {
                throw;
            }
        }

        public Transaction GetEjournalAndNTDetailForMasterReceivableATM(DataRow dr)
        {
            try
            {
                Transaction _Transaction = new Transaction();
                DateTimeFormatInfo dateFormatInfo = new DateTimeFormatInfo();
                dateFormatInfo.ShortDatePattern = @"dd-MM-yyyy";

                string datetime = dr[3].ToString();
                DateTime txnDate = Convert.ToDateTime(datetime, dateFormatInfo);
                _Transaction.TransactionDate = txnDate;
                string time = dr[4].ToString();
                _Transaction.TransactionTime = TimeSpan.Parse(time);

                _Transaction.TerminalId = dr[9].ToString();
                _Transaction.CardNo = dr[2].ToString();
                _Transaction.TraceNo = GetTraceno(dr[7].ToString());
                _Transaction.ReferenceNo = dr[8].ToString();
                _Transaction.AuthCode = dr[11].ToString();
                _Transaction.TransactionAmount = 0;
                if (!string.IsNullOrEmpty(dr[6].ToString()))
                {
                    _Transaction.TransactionAmount = Convert.ToDecimal(dr[6].ToString());
                }
                _Transaction.ResponseCode = GetResponseCode(dr[10].ToString());

                TransactionType _TransactionType = TransactionType.Financial;
                TransactionStatus _TransactionStatus = TransactionStatus.Fail;
                CardType _CardType = CardType.NotDefine;
                NetworkType _NetworkType = NetworkType.NPN;
                TerminalType _TerminalType = TerminalType.ATM;
                TerminalOwner _TerminalOwner = TerminalOwner.ForeignTerminal;

                if (_Transaction.ResponseCode.Equals("0000"))
                {
                    _TransactionStatus = TransactionStatus.Success;
                }

                if (_Transaction.TransactionAmount == 0)
                {
                    _TransactionType = TransactionType.BalInquiry;
                }

                _Transaction = GetTransaction(_Transaction, _TransactionType, _TransactionStatus, _TerminalType, _CardType, _TerminalOwner, _NetworkType);

                return _Transaction;
            }
            catch
            {
                throw;
            }
        }

        public bool CheckForTransactionData(DataRow dr)
        {
            bool flag = true;
            if (dr[4].ToString().Contains("Total") || dr[0].ToString().Contains("Total") || dr[0].ToString().Contains("Member Cash withdrawal (ATMs)") ||
                dr[0].ToString().Contains("Issuer (A)") || dr[0].ToString().Contains("Aquirer (B)") || dr[0].ToString().Contains("Net Difference C=(A-B)") ||
                dr[0].ToString().Contains("POS") || dr[0].ToString().Contains("POS Transaction") || string.IsNullOrEmpty(dr[0].ToString()))
            {
                flag = false;
            }
            else if (dr.Table.Columns.Count > 8)
            {
                if (dr[0].ToString().Equals("AcqBin") || dr[1].ToString().Equals("AcqBank") || dr[2].ToString().Equals("TranDate") ||
                    dr[3].ToString().Equals("TerminalId") || dr[4].ToString().Equals("CardNo") || dr[5].ToString().Equals("TraceNo") || dr[6].ToString().Equals("RRN") ||
                    dr[6].ToString().Equals("RefNo") || dr[7].ToString().Equals("TranAmt") || dr[8].ToString().Equals("AuthCode") || dr[9].ToString().Equals("Loro"))
                {
                    flag = false;
                }
                else if (dr[0].ToString().Equals("IssuingBin") || dr[1].ToString().Equals("IssuingBank") || dr[2].ToString().Equals("TranDate") ||
                         dr[3].ToString().Equals("TerminalId") || dr[4].ToString().Equals("CardNo") || dr[5].ToString().Equals("TraceNo") || dr[6].ToString().Equals("RRN") ||
                         dr[7].ToString().Equals("TranAmt") || dr[8].ToString().Equals("AuthCode") || dr[9].ToString().Equals("Loro"))
                {
                    flag = false;
                }
                else if (dr[0].ToString().Equals("CompanyName") || dr[1].ToString().Equals("TranDate") || dr[2].ToString().Equals("TerminalId") ||
                         dr[3].ToString().Equals("TranAbbrev") || dr[4].ToString().Equals("CardNo") || dr[5].ToString().Equals("AuthCode") || dr[6].ToString().Equals("RRN") ||
                         dr[7].ToString().Equals("TranAmt") || dr[8].ToString().Equals("StmtDate") || dr[9].ToString().Equals("TraceNo"))
                {
                    flag = false;
                }
            }
            else if (dr[0].ToString().Contains("Total"))
            {
                flag = false;
            }

            if (dr[0].ToString().Contains("POSH"))
            {
                flag = true;
            }
            return flag;
        }

        private string GetTraceno(string traceNo)
        {
            if (traceNo.Length == 6)
            {
                return traceNo;
            }
            else if (traceNo.Length == 5)
            {
                return "0" + traceNo;
            }
            else if (traceNo.Length == 4)
            {
                return "00" + traceNo;
            }
            else if (traceNo.Length == 3)
            {
                return "000" + traceNo;
            }
            else if (traceNo.Length == 2)
            {
                return "0000" + traceNo;
            }
            else if (traceNo.Length == 1)
            {
                return "00000" + traceNo;
            }
            else
            {
                return traceNo;
            }
        }

        override
            public void ProcessRecon()
        {
            Console.WriteLine("Recon Start..............");
            if (Transactions.Count < 1)
            {
                Console.WriteLine("Recon Complete..............");
                return;
            }
            //            NpnVsCbs recon = new NpnVsCbs(Transactions);
            //            recon.Reconcile();

            Console.WriteLine("Recon Complete..............");
        }
    }
}
