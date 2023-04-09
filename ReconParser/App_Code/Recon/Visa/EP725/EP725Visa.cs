using Db.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Db.Enum;

namespace ReconParser.App_Code.Recon.Visa.EP725
{
    public class EP725Visa : Visa
    {
        public EP725Visa(String FileName, int FileCount)
            : base(FileName, FileCount)
        {
            _SubSource = SubSource.Find_By_Name("EP725");
        }

        public override void Parse()
        {
            Console.WriteLine("Parsing File {0}", FileName);
            List<string> listString = ReadDataFromFile(FileName);

            TransactionBlockSeperator(listString);

            GetTransactionList();

            Console.WriteLine("Parsed File {0}", FileName);
        }
        public override void GetTransactionList()
        {
            TransactionType _TransactionType = TransactionType.NotDefine;
            TransactionStatus _TransactionStatus = TransactionStatus.NotDefine;
            CardType _CardType = CardType.NotDefine;
            NetworkType _NetworkType = NetworkType.VISA;
            TerminalType _TerminalType = TerminalType.ATM;
            TerminalOwner _TerminalOwner = TerminalOwner.OffUsTerminal;

            Transaction _Transaction = new Transaction();

            long countId = 1;
            int blockCount = 0;
            int firstTime = 0;
            foreach (TransactionBlock _TransactionBlock in TransactionBlocks)
            {
                try
                {
                    if (_TransactionBlock.TransactionBlockList.Count >= 13)
                    {
                        firstTime++;

                        if (blockCount == 3 || firstTime == 1)
                        {
                            blockCount = 0;
                            _Transaction = new Transaction();

                            _TransactionType = TransactionType.NotDefine;
                            _TransactionStatus = TransactionStatus.NotDefine;
                            //                            _CardType = CardType.OwnCard;
                            _NetworkType = NetworkType.VISA;
                            _TerminalType = TerminalType.ATM;
                            _TerminalOwner = TerminalOwner.OffUsTerminal;

                            countId++;

                            DateTimeFormatInfo dateFInfo = new DateTimeFormatInfo();
                            dateFInfo.ShortDatePattern = @"MM/dd/yyyy";//ncc bank
                            try
                            {
                                _Transaction.AdviseDate = Convert.ToDateTime(settlementdate, dateFInfo);
                            }
                            catch
                            {
                                dateFInfo.ShortDatePattern = @"yyyy/MM/dd"; // nepal bank
                                _Transaction.AdviseDate = Convert.ToDateTime(settlementdate, dateFInfo);
                            }
                            if (FileName.Contains("EP725") || FileName.Contains("EP705"))
                            {
                                _TerminalType = TerminalType.POS;
                            }
                        }
                        blockCount++;

                        DateTimeFormatInfo dateFormatInfo = new DateTimeFormatInfo();
                        dateFormatInfo.ShortDatePattern = @"dd/MM/yyyy";

                        List<string> ejData = new List<string>();
                        List<string> ejExtractData = new List<string>();
                        bool isRequiredData = false;

                        foreach (string str in _TransactionBlock.TransactionBlockList)
                        {

                            string[] eliminateData = str.Split(' ');
                            ejData = new List<string>();
                            for (int s = 0; s < eliminateData.Length; s++)
                            {
                                if (!string.IsNullOrEmpty(eliminateData[s]))
                                {
                                    ejData.Add(eliminateData[s]);
                                }
                            }

                            if (str.Contains("Acct Number & Extension"))
                            {
                                _Transaction.CardNo = TruncateValue(ejData[4], 0, 16);
                                _Transaction.ResponseCode = GetResponseCode("0000");

                                if (FileName.Contains("EP725") || FileName.Contains("EP727"))
                                {
                                    _Transaction.TransactionStatus = TransactionStatus.Reversal;
                                }
                                else
                                {
                                    _Transaction.TransactionStatus = TransactionStatus.Success;
                                }
                            }
                            else if (str.Contains("Usage Code"))
                            {
                                if (ejData[ejData.Count - 1].Equals("2"))
                                {
                                    _Transaction.TransactionStatus = TransactionStatus.Representment;
                                }

                                if (str.Contains("Acquirer Reference Nbr"))
                                {
                                    _Transaction.ReferenceNo = ejData[3].Substring(7, 4) + ejData[3].Substring(ejData[3].Length - 9, 8);
                                }
                            }
                            else if (str.Contains("Acquirer Reference Nbr"))
                            {
                                _Transaction.ReferenceNo = ejData[3].Substring(7, 4) + ejData[3].Substring(ejData[3].Length - 9, 8);
                            }
                            else if (str.Contains("Settlement Flag"))
                            {

                                if (ejData[ejData.Count - 1].Equals("0"))
                                {
                                    _Transaction.Currency = "USD";
                                    _TerminalOwner = TerminalOwner.ForeignTerminal;
                                }

                                if (str.Contains("Purchase Date"))
                                {
                                    string yyyy = ejData[2].Substring(0, 4);
                                    string dd = ejData[2].Substring(6, 2);
                                    string MM = ejData[2].Substring(4, 2);
                                    string stringDate = dd + "/" + MM + "/" + yyyy;
                                    //04/08/2009
                                    if (ejData[2].Equals("00000000"))
                                    {
                                        _Transaction.TransactionDate = _Transaction.AdviseDate.Value.AddDays(-1);
                                    }
                                    else
                                    {
                                        _Transaction.TransactionDate = Convert.ToDateTime(stringDate, dateFormatInfo);
                                        if (_Transaction.TransactionDate > _Transaction.AdviseDate)
                                        {
                                            _Transaction.TransactionDate = _Transaction.TransactionDate.AddYears(-1);
                                        }
                                    }
                                }
                            }
                            else if (str.Contains("Purchase Date"))
                            {
                                string yyyy = ejData[2].Substring(0, 4);
                                string dd = ejData[2].Substring(6, 2);
                                string MM = ejData[2].Substring(4, 2);
                                string stringDate = dd + "/" + MM + "/" + yyyy;
                                //04/08/2009
                                if (ejData[2].Equals("00000000"))
                                {
                                    _Transaction.TransactionDate = _Transaction.AdviseDate.Value.AddDays(-1);
                                }
                                else
                                {
                                    _Transaction.TransactionDate = Convert.ToDateTime(stringDate, dateFormatInfo);
                                    if (_Transaction.TransactionDate > _Transaction.AdviseDate)
                                    {
                                        _Transaction.TransactionDate = _Transaction.TransactionDate.AddYears(-1);
                                    }
                                }

                            }
                            else if (str.Contains("Destination Amount"))
                            {
                                _Transaction.TransactionAmount = decimal.Parse(ejData[2]) / 100;
                                _Transaction.DestinationTransactionAmount = decimal.Parse(ejData[2]) / 100;

                                //                                if (decimal.Parse(ejData[2]) != 0)
                                //                                    _TransactionType = TransactionType.Financial;
                                //                                else
                                //                                    _TransactionType = TransactionType.BalInquiry;
                                //
                                //                                if (str.Contains("Authorization Code"))
                                //                                {
                                //                                    _Transaction.AuthCode = ejData[ejData.Count - 1];
                                //                                }
                            }
                            else if (str.Contains("Source Amount"))
                            {
                                //                                _Transaction.TransactionAmount = decimal.Parse(ejData[2]) / 100;
                                _Transaction.SourceTransactionAmount = decimal.Parse(ejData[2]) / 100;
                                if (decimal.Parse(ejData[2]) != 0)
                                    _TransactionType = TransactionType.Financial;
                                else
                                    _TransactionType = TransactionType.BalInquiry;

                                if (str.Contains("Authorization Code"))
                                {
                                    _Transaction.AuthCode = ejData[ejData.Count - 1];
                                }
                            }
                            else if (str.Contains("Destination Currency Code"))
                            {
                                if (string.IsNullOrEmpty(_Transaction.Currency))
                                {
                                    if (ejData[3] == "840")
                                    {
                                        _Transaction.Currency = "USD";
                                        _TerminalOwner = TerminalOwner.ForeignTerminal;
                                    }
                                    else
                                    {
                                        _Transaction.Currency = "NPR";
                                    }
                                }

                                if (str.Contains("Authorization Code"))
                                {
                                    _Transaction.AuthCode = ejData[ejData.Count - 1];
                                }
                            }
                            else if (str.Contains("Source Currency Code"))
                            {
                                if (ejData[3] == "356")
                                {
                                    _Transaction.Currency = "INR";
                                    _TerminalOwner = TerminalOwner.INRTerminal;
                                }
                            }
                            else if (str.Contains("Interface Trace Number"))
                            {
                                if (ejData.Count == 7)
                                {
                                    if (ejData[6] != "000000")
                                    {
                                        if (ejData[6] != "Number")
                                            _Transaction.TraceNo = ejData[6];
                                    }
                                }
                            }
                            else if (str.Contains("Terminal ID"))
                            {
                                if (ejData.Count == 5)
                                {
                                    if (ejData[2] != "Purchase")
                                        _Transaction.TerminalId = ejData[2];
                                }
                                else if (ejData.Count == 6)
                                {
                                    if (ejData[5] == "Identifier")
                                    {
                                        _Transaction.TerminalId = ejData[2] + " " + ejData[3];
                                    }
                                    else
                                    {
                                        _Transaction.TerminalId = ejData[2];
                                    }
                                }

                                isRequiredData = true;
                            }
                            else if (str.Contains("Member Message Text") && str.Contains("CASH DISB"))
                            {
                                if (ejData.Count() > 6)
                                {
                                    if (ejData.Count() == 9)
                                        _Transaction.TerminalId = ejData[6].Substring(3, 8);
                                    else if (ejData.Count() == 8)
                                        _Transaction.TerminalId = ejData[5].Substring(3, 8);
                                }
                            }
                        }
                        if (isRequiredData)
                        {


                            _TransactionStatus = TransactionStatus.Reversal;  // default reversal..ep 725
                            _Transaction.GmtToLocalTransactionDate = _Transaction.TransactionDate;

                            _TransactionStatus = _Transaction.TransactionStatus;
                            _Transaction = GetTransaction(_Transaction, _TransactionType, _TransactionStatus, _TerminalType, _CardType, _TerminalOwner, _NetworkType);


                            if (_Transaction.CardType != CardType.OwnCard &&
                                _Transaction.TerminalOwner == TerminalOwner.OwnTerminal &&
                                _Transaction.TerminalType == TerminalType.POS &&
                                _Transaction.DestinationTransactionAmount == 0)
                            {
                                _Transaction.TransactionAmount = _Transaction.SourceTransactionAmount;
                            }
                            Transactions.Add(_Transaction);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
