using Db.Enum;
using Db.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReconParser.App_Code.Recon.Ej.Ncr.SCT
{
    public class SCTNcr : Ncr
    {
        public SCTNcr(String FileName, int FileCount)
            : base(FileName,FileCount)
        {
            //TransactionBlocks = new List<TransactionBlock>();
        }

        public void TransactionBlockSeperator(List<string> listString)
        {
            TransactionBlock _TransactionBlock = new TransactionBlock();
            foreach (string str in listString)
            {
                if (str.IndexOf("***********JOURNAL RECORD***********") >= 0)
                {
                    _TransactionBlock = new TransactionBlock();
                }
                _TransactionBlock.TransactionBlockList.Add(str);

                if (str.IndexOf("****************************************") >= 0)
                {
                    TransactionBlocks.Add(_TransactionBlock);
                    _TransactionBlock = new TransactionBlock();
                }
            }
        }

        public override void Parse()
        {
            List<string> listString = ReadDataFromFile(FileName);

            TransactionBlockSeperator(listString);

            GetTransactionList();
        }

        public void GetTransactionList()
        {
            foreach (TransactionBlock _TransactionBlock in TransactionBlocks)
            {
                Transaction _Transaction = GetTransaction(_TransactionBlock);
                if (_Transaction != null)
                {
                    Transactions.Add(_Transaction);
                }
            }
        }

        public Transaction GetTransaction(TransactionBlock _TransactionBlock)
        {
            string terminalIdFirstPart = "";

            //if (ConfigurationManager.AppSettings["TerminalId_StartingPart"] != null)
            //{
            //    terminalIdFirstPart = ConfigurationManager.AppSettings["TerminalId_StartingPart"].ToString();
            //}

            TransactionType _TransactionType = TransactionType.NotDefine;
            TransactionStatus _TransactionStatus = TransactionStatus.NotDefine; 

            //int error = 0;
            Transaction _Transaction = new Transaction();
            //int index = 0;
            string status = string.Empty;
            try
            {
                DateTimeFormatInfo dateFormatInfo = new DateTimeFormatInfo();
                dateFormatInfo.ShortDatePattern = @"dd/MM/yyyy";

                if (_TransactionBlock.TransactionBlockList.Count > 2)
                {
                    string[] firstLine = _TransactionBlock.TransactionBlockList[2].Split(' ');
                    _Transaction.TransactionDate = Convert.ToDateTime(firstLine[0], dateFormatInfo); //firstLine[0]; 
                    _Transaction.TransactionTime = TimeSpan.Parse(firstLine[1]); //firstLine[1]; 
                    _Transaction.TerminalId = firstLine[3];
                }

                if (_TransactionBlock.TransactionBlockList.Count > 3)
                {
                    string[] secondLine = _TransactionBlock.TransactionBlockList[3].Split(' ');
                    _Transaction.TransactionNo = secondLine[2];
                    _Transaction.ResponseCode = GetResponseCode(secondLine[4]);

                    _TransactionStatus = TransactionStatus.Success;
                    if (!_Transaction.ResponseCode.Equals("0000"))
                    {
                        _TransactionStatus = TransactionStatus.Fail;
                    }
                }

                if (_TransactionBlock.TransactionBlockList.Count > 4)
                {
                    string[] fouthLine = _TransactionBlock.TransactionBlockList[4].Split(' ');
                    _Transaction.CardNo = fouthLine[1];
                    _Transaction.AuthCode = fouthLine[6];
                }
                if (_TransactionBlock.TransactionBlockList.Count > 5)
                {
                    string[] fifthLine = _TransactionBlock.TransactionBlockList[5].Split(' ');
                    if (!string.IsNullOrEmpty(fifthLine[1].Trim()))
                    {
                        _Transaction.AccountNo = fifthLine[1];
                    }
                }
                if (_TransactionBlock.TransactionBlockList.Count > 6)
                {
                    string[] sixthLine = _TransactionBlock.TransactionBlockList[6].Split(' ');
                    if (sixthLine[3].Equals("CASH-WITHDRAWAL") || sixthLine[3].Equals("FAST-CASH") || sixthLine[3].Equals("ATM-MOBILE"))
                    {
                        _TransactionType = TransactionType.Financial;
                    }
                    else if (sixthLine[3].Equals("LOAD-CASH"))
                    {
                        _TransactionType = TransactionType.NotDefine;
                    }
                    else if (sixthLine[3].Equals("BALANCE-INQUIRY") || sixthLine[3].Equals("CASH-BALANCE"))
                    {
                        _TransactionType = TransactionType.BalInquiry;
                    }
                    else
                    {
                        _TransactionType = TransactionType.NotDefine;
                    }
                }
                if (_TransactionBlock.TransactionBlockList.Count > 7)
                {
                    string[] seventhLine = _TransactionBlock.TransactionBlockList[7].Split(' ');
                    _Transaction.ReferenceNo = seventhLine[3];
                }
                if (_TransactionBlock.TransactionBlockList.Count > 8)
                {
                    string[] eighthLine = _TransactionBlock.TransactionBlockList[8].Split(' ');
                    _Transaction.TraceNo = eighthLine[4];
                }
                if (_TransactionBlock.TransactionBlockList.Count > 9)
                {
                    string[] ninethLine = _TransactionBlock.TransactionBlockList[9].Split(' ');
                    _Transaction.TransactionAmount = Convert.ToDecimal(getNumber(ninethLine));
                }

                _Transaction = GetTransaction(_Transaction, _TransactionType, _TransactionStatus, TerminalType.ATM);

                return _Transaction;
            }
            catch
            {
                throw;
            }
        }
    }
}
