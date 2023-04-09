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

namespace ReconParser.App_Code.Recon.CBS.Pumori
{
    public class Pumori : Cbs
    {
        public Pumori(String FileName, int FileCount)
            : base(FileName, FileCount)
        {
            _SubSource = SubSource.Find_By_Name("Pumori");
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
                    ExcelFileReadingClass testCell = new ExcelFileReadingClass();
                    dt = testCell.ReadExcelFileEPPUnmerged(FileName);
                    var list = dt;
                    GetTransactions(dt);
                }
                catch
                {
                    dt = DataReadFromExcel.ReadExcelFile(FileName);
                    GetTransactions(dt);
                }
            }
            catch (Exception ex)
            {
                Console.Write("Erro->Pumori->ReadDataFromFile->" + ex.Message);
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
                NetworkType _NetworkType = NetworkType.VISA;
                TerminalType _TerminalType = TerminalType.ATM;
                TerminalOwner _TerminalOwner = TerminalOwner.NotDefine;

                Transaction _Transaction = new Transaction();

                DateTimeFormatInfo dateFormat = new DateTimeFormatInfo();
                dateFormat.ShortDatePattern = @"yyyyMMddhh";

                //               double d = double.Parse(_dataRow[7].ToString());
                //                DateTime conv = DateTime.FromOADate(d);
                try
                {
                    dateFormat.ShortDatePattern = @"ddMMyyyy";
                    _Transaction.TransactionDate = Convert.ToDateTime(_dataRow[7], dateFormat);
                    //_Transaction.TransactionDate = Convert.ToDateTime(_dataRow[7].ToString());
                }

                catch (Exception ex)
                {
                    dateFormat.ShortDatePattern = @"ddMMyyyy";
                    _Transaction.TransactionDate = Convert.ToDateTime(_dataRow[7], dateFormat);

                }
                _Transaction.TerminalId = _dataRow[6].ToString();
                _Transaction.CardNo = _dataRow[0].ToString();
                //reversal txnamount in pumori comes in negative..
                try
                {
                    //                    if (Convert.ToDecimal(_dataRow[8].ToString()) < 0)
                    //                    {
                    //                        _Transaction.TransactionAmount = Math.Abs((Convert.ToDecimal(_dataRow[8].ToString())) / 100);
                    //                        _TransactionStatus = TransactionStatus.Reversal;
                    //                    }
                    //                    else
                    _Transaction.TransactionAmount = Convert.ToDecimal(_dataRow[8].ToString());
                }
                catch
                {
                    throw;
                }

                _Transaction.AccountNo = _dataRow[12].ToString();

                _Transaction.ReferenceNo = _dataRow[5].ToString();
                _Transaction.TraceNo = _dataRow[4].ToString();
                _Transaction.ResponseCode = GetResponseCode(_dataRow[2].ToString());
                if (_dataRow[22].ToString().Equals("NPN"))
                {
                    _NetworkType = NetworkType.NPN;
                }
                else if (_dataRow[22].ToString().Equals("VISA"))
                {
                    _NetworkType = NetworkType.VISA;
                }
                else
                {
                    _NetworkType = NetworkType.SCT;
                }


                if (_dataRow[21].ToString() == "ATM")
                {
                    _TerminalType = TerminalType.ATM;
                }
                else if (_dataRow[21].ToString() == "POS")
                {
                    _TerminalType = TerminalType.POS;
                }

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

                _Transaction.TransactionNo = _dataRow[3].ToString();



                if (_dataRow[18].ToString().ToLower() == CardType.OwnCard.ToString())
                {
                    _CardType = CardType.OwnCard;
                }
                else
                {
                    _CardType = CardType.OffUsCard;
                }

                if (_dataRow[19].ToString().ToLower().Equals("OnUs terminal"))
                {
                    _TerminalOwner = TerminalOwner.OwnTerminal;
                }
                else
                {
                    _TerminalOwner = TerminalOwner.OffUsTerminal;
                }


                if (_dataRow[20].ToString().ToLower().Equals(TransactionStatus.Success.ToString()))
                {
                    _TransactionStatus = TransactionStatus.Success;
                }

                if (_dataRow[23].ToString().Equals("0420"))
                {
                    _TransactionStatus = TransactionStatus.Reversal;
                }
                _Transaction.MainCode = _dataRow[24].ToString();

                _Transaction = GetTransaction(_Transaction, _TransactionType, _TransactionStatus, _TerminalType, _CardType, _TerminalOwner, _NetworkType);

                return _Transaction;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
