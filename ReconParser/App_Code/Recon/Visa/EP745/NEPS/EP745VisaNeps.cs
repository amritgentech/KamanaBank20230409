using Db.Enum;
using Db.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helper.GlobalHelpers;

namespace ReconParser.App_Code.Recon.Visa.NEPS.EP745
{
    public class EP745VisaNeps : Visa
    {
        public EP745VisaNeps(String FileName, int FileCount)
            : base(FileName, FileCount)
        {
            _SubSource = SubSource.Find_By_Name("EP745");
            _SubChildSource = SubChildSource.Find_By_SubSourceId(_SubSource.SubSourceId);
        }

        public override void Parse()
        {
            Console.WriteLine("Parsing File {0}", FileName);
            List<string> listString = ReadDataFromFile(FileName);

            TransactionBlockSeperator(listString);

            GetTransactionList();

            Console.WriteLine("Parsed File {0}", FileName);
        }

        public void TransactionBlockSeperator(List<string> listString)
        {
            //StringList stringListObj = new StringList();
            //List<StringList> listStrObj = new List<StringList>();
            //List<StringList> listStrObj1 = new List<StringList>();
            int strCount = 0;
            TransactionBlock _TransactionBlock = new TransactionBlock();
            string temp = string.Empty;
            DateTimeFormatInfo dateFormatInfo = new DateTimeFormatInfo();
            dateFormatInfo.ShortDatePattern = @"dd/MM/yyyy";
            string date = string.Empty;
            int countLimit = 0;

            foreach (string str in listString)
            {
                strCount++;
                DateTime dt;
                //if (Convert.ToDateTime(str, dateFormatInfo))
                if (DateTime.TryParse(str, out dt))
                {
                    date = str.Trim();
                    _TransactionBlock.TransactionBlockList.Add(str.Trim());
                    countLimit = 4;
                }
                else
                {
                    if (TransactionBlocks.Count == 0)
                        countLimit = 4;
                    else
                        countLimit = 3;

                    if (strCount == 1)
                    {
                        strCount = 0;
                        if (!str.Contains("REPEAT OR DUP/ORI"))
                        {
                            strCount = 1;
                            _TransactionBlock.TransactionBlockList.Add(date);
                            _TransactionBlock.TransactionBlockList.Add(str.Trim());
                        }
                    }
                    else
                    {
                        if ((!(str.Contains("ACI:") || str.Contains("SCHG:") || str.Contains("TR ID:") || str.Contains("REPEAT OR DUP/ORI")) && strCount == 3 && TransactionBlocks.Count > 0) ||
                            (!(str.Contains("ACI:") || str.Contains("SCHG:") || str.Contains("TR ID:") || str.Contains("REPEAT OR DUP/ORI")) && strCount == 4 && TransactionBlocks.Count == 0))
                        {
                            TransactionBlocks.Add(_TransactionBlock);
                            _TransactionBlock = new TransactionBlock();
                            strCount = 1;

                            _TransactionBlock.TransactionBlockList.Add(date);
                            _TransactionBlock.TransactionBlockList.Add(str.Trim());
                        }
                        else
                        {
                            _TransactionBlock.TransactionBlockList.Add(str.Trim());
                        }

                    }
                }

                string v = str.Trim();
                if (strCount == countLimit)
                {
                    TransactionBlocks.Add(_TransactionBlock);
                    _TransactionBlock = new TransactionBlock();
                    strCount = 0;
                }

            }
        }

        public void GetTransactionList()
        {
            List<Transaction> listTransaction = new List<Transaction>();
            List<Transaction> listTransaction2 = new List<Transaction>();

            TransactionType _TransactionType = TransactionType.NotDefine;
            TransactionStatus _TransactionStatus = TransactionStatus.NotDefine;
            CardType _CardType = CardType.OffUsCard;
            NetworkType _NetworkType = NetworkType.VISA;
            TerminalType _TerminalType = TerminalType.ATM;
            TerminalOwner _TerminalOwner = TerminalOwner.OwnTerminal;

            Transaction _Transaction = new Transaction();
            string year = string.Empty;

            foreach (TransactionBlock _TransactionBlock in TransactionBlocks)
            {
                if (_TransactionBlock.TransactionBlockList.Count >= 3)
                {
                    DateTimeFormatInfo dateFormatInfo = new DateTimeFormatInfo();
                    dateFormatInfo.ShortDatePattern = @"dd/MM/yyyy";

                    List<string> ejData = new List<string>();
                    List<string> ejExtractData = new List<string>();
                    bool isRequiredData = false;


                    List<string> finalData = new List<string>();

                    for (int e = 0; e < _TransactionBlock.TransactionBlockList.Count; e++)
                    {
                        try
                        {
                            string[] eliminateData = _TransactionBlock.TransactionBlockList[e].Split(' ');
                            ejData = eliminateData.Where(t => !string.IsNullOrEmpty(t)).ToList();

                            if (e == 0)
                            {

                                try
                                {
                                    DateTime tempYear = new DateTime();
                                    tempYear = Convert.ToDateTime(ejData[0].ToString(), dateFormatInfo);
                                    string[] splitYearTime = tempYear.ToString().Split('/');
                                    string[] splitYear = splitYearTime[2].Split(' ');
                                    year = splitYear[0];

                                    _Transaction = new Transaction();

                                    _TransactionType = TransactionType.NotDefine;
                                    _TransactionStatus = TransactionStatus.NotDefine;
                                    _CardType = CardType.OffUsCard;
                                    _NetworkType = NetworkType.VISA;
                                    _TerminalType = TerminalType.ATM;
                                    _TerminalOwner = TerminalOwner.OwnTerminal;

                                    _Transaction.AdviseDate = Convert.ToDateTime(settlementdate);

                                }
                                catch
                                {
                                    throw;
                                }

                            }
                            else if (e == 1)
                            {
                                try
                                {
                                    string yyyy = year;
                                    string dd = ejData[1].Substring(0, 2);
                                    string MM = ejData[1].Substring(2, 3);
                                    string stringDate = dd + "/" + MM + "/" + yyyy;
                                    _Transaction.TransactionDate = Convert.ToDateTime(stringDate, dateFormatInfo);
                                    if (_Transaction.TransactionDate > _Transaction.AdviseDate)
                                    {
                                        _Transaction.TransactionDate = _Transaction.TransactionDate.AddYears(-1);
                                    }
                                }
                                catch
                                {
                                    throw;
                                }

                                _Transaction.TransactionTime = TimeSpan.Parse(ejData[2]);


                                _Transaction.CardNo = ejData[3];
                                _Transaction.ReferenceNo = ejData[4];
                                _Transaction.TraceNo = ejData[5];
                                _Transaction.ProcessingCode = ejData[8];


                                if (ejData[ejData.Count - 1].Contains("CR"))
                                {
                                    string processCode = ejData[7];
                                    int respCodeIndex = 11;
                                    int amtIndex = 12;
                                    int amtChrIndex = 14;
                                    if (processCode.Equals("0422") || processCode.Equals("0420"))
                                    {
                                        respCodeIndex = 9;
                                        amtIndex = 11;
                                        amtChrIndex = 13;
                                    }
                                    _Transaction.ResponseCode = GetResponseCode(ejData[respCodeIndex]);
                                    _Transaction.TransactionAmount = decimal.Parse(ejData[amtIndex]);
                                    if (_Transaction.TransactionAmount > 0)
                                        _TransactionType = TransactionType.Financial;
                                    else
                                        _TransactionType = TransactionType.BalInquiry;

                                    if (_Transaction.ResponseCode == "0000")
                                        if (ejData[amtChrIndex] == "0.00")
                                        {
                                            _TransactionStatus = TransactionStatus.Fail;
                                        }
                                        else
                                        {
                                            _TransactionStatus = TransactionStatus.Success;
                                        }
                                    else
                                        _TransactionStatus = TransactionStatus.Fail;
                                }
                                else if (ejData[ejData.Count - 1].Contains("DR"))
                                {
                                    if (ejData.Count == 14)
                                    {
                                        _Transaction.ResponseCode = GetResponseCode(ejData[10]);
                                        _Transaction.TransactionAmount = decimal.Parse(ejData[11]);
                                        //obj.RESPONSE_CODE = GetResponseCode(ejData[10]);
                                        //obj.AMOUNT = decimal.Parse(ejData[11]);
                                    }
                                    else
                                    {
                                        _Transaction.ResponseCode = GetResponseCode(ejData[11]);
                                        _Transaction.TransactionAmount = decimal.Parse(ejData[12]);
                                        //obj.RESPONSE_CODE = GetResponseCode(ejData[11]);
                                        //obj.AMOUNT = decimal.Parse(ejData[12]);
                                    }
                                    if (_Transaction.TransactionAmount > 0)
                                        _TransactionType = TransactionType.Financial;
                                    else
                                        _TransactionType = TransactionType.BalInquiry;

                                    if (_Transaction.ResponseCode == "0000")
                                        if (ejData.Count == 14)
                                        {
                                            if (ejData[13] == "0.00")
                                            {
                                                _TransactionStatus = TransactionStatus.Success;
                                            }
                                            else
                                            {
                                                _TransactionStatus = TransactionStatus.Reversal;
                                            }
                                        }
                                        else
                                        {
                                            if (ejData[14] == "0.00")
                                            {
                                                _TransactionStatus = TransactionStatus.Success;
                                            }
                                            else
                                            {
                                                _TransactionStatus = TransactionStatus.Reversal;
                                            }
                                        }
                                    else
                                        _TransactionStatus = TransactionStatus.Representment;
                                }
                                else
                                {
                                    _Transaction.ResponseCode = GetResponseCode(ejData[11]);
                                    _Transaction.TransactionAmount = decimal.Parse(ejData[12]);

                                    if (decimal.Parse(ejData[12]) != 0)
                                        _TransactionType = TransactionType.Financial;
                                    else
                                        _TransactionType = TransactionType.BalInquiry;

                                    if (_Transaction.ResponseCode == "0000")
                                        if (ejData[14] == "0.00")
                                        {
                                            _TransactionStatus = TransactionStatus.Fail;
                                        }
                                        else
                                        {
                                            _TransactionStatus = TransactionStatus.Success;
                                        }
                                    else
                                        _TransactionStatus = TransactionStatus.Fail;
                                }
                            }
                            else if (e == 2)
                            {
                                _Transaction.TerminalId = ejData[2];
                                //                                _Transaction.Currency = "NPR";
                                _Transaction.Currency = "524";
                                if (ejData.Contains("FPI:"))
                                {
                                    //                                    _Transaction.Currency = "USD";
                                    _Transaction.Currency = "840";
                                    _CardType = CardType.ForeignCard;
                                }
                                isRequiredData = true;

                            }
                            else if (e == 3)
                            {
                                if (ejData.Contains("SCHG:"))
                                {
                                    decimal schg = decimal.Parse(ejData[ejData.Count - 1]);
                                    _Transaction.TransactionAmount = _Transaction.TransactionAmount - schg;
                                }
                            }
                        }
                        catch
                        {
                            throw;
                        }
                    }
                    if (isRequiredData == true)
                    {
                        _Transaction = GetTransaction(_Transaction, _TransactionType, _TransactionStatus, _TerminalType, _CardType, _TerminalOwner, _NetworkType);
                        _Transaction.GmtToLocalTransactionDate =
                            GlobalHelper.ConvertGmtToLocal(_Transaction.TransactionDate,
                                _Transaction.TransactionTime).Date;
                        listTransaction.Add(_Transaction);
                    }
                }
            }

            var DistinctItems = listTransaction.GroupBy(x => new { x.TraceNo, x.CardNo, x.TransactionDate, x.TerminalId }).Select(y => y.First());
            foreach (var item in DistinctItems)
            {
                listTransaction2.Add(item);
            }
            Transactions = listTransaction2.Distinct().ToList();
        }
    }
}
