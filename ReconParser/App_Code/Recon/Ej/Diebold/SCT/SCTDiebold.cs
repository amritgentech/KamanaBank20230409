using Db.Enum;
using Db.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReconParser.App_Code.Recon.Ej.Diebold.SCT
{
    public class SCTDiebold : Diebold
    {
        public SCTDiebold(String FileName, int FileCount)
            : base(FileName,FileCount)
        {
        }

        public void TransactionBlockSeperator(List<string> listString)
        {
            TransactionBlock _TransactionBlock = new TransactionBlock();
            foreach (string str in listString)
            {
                if (str.IndexOf("LUNO123") >= 0)
                {
                    if (str.Contains("a*********************************a"))
                    {
                        _TransactionBlock = new TransactionBlock();
                        string[] strArray = str.Split('a');

                        if (strArray.Length > 5)
                        {
                            _TransactionBlock.TransactionBlockList.AddRange(strArray);
                            TransactionBlocks.Add(_TransactionBlock);
                            _TransactionBlock = new TransactionBlock();
                        }
                    }
                }
            }
        }

        public override void Parse()
        {
            Console.WriteLine("Parsing File {0}", FileName);
            try
            {
                List<string> listString = ReadDataFromFile(FileName);

                TransactionBlockSeperator(listString);

                GetTransactionList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error->SCTDiebold->Parse->{0}", ex.Message);
            }
            Console.WriteLine("Parsed File {0}", FileName);
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

            Transaction _Transaction = new Transaction();
            try
            {
                DateTimeFormatInfo dateFormatInfo = new DateTimeFormatInfo();
                dateFormatInfo.ShortDatePattern = @"yyyy-MMM-dd";

                string[] ejData = _TransactionBlock.TransactionBlockList[2].Split(' ');
                _Transaction.TransactionDate = Convert.ToDateTime(ejData[0], dateFormatInfo);
                _Transaction.TransactionTime = TimeSpan.Parse(ejData[1]);
                _Transaction.TerminalId = terminalIdFirstPart + ejData[2];
                _Transaction.ProcessingCode = ejData[4];

                if (_Transaction.ProcessingCode.Equals("010000") || _Transaction.ProcessingCode.Equals("011000") || _Transaction.ProcessingCode.Equals("012000") || _Transaction.ProcessingCode.Equals("013000"))
                {
                    _TransactionType = TransactionType.Financial;
                }
                else if (_Transaction.ProcessingCode.Equals("301000") || _Transaction.ProcessingCode.Equals("302000") || _Transaction.ProcessingCode.Equals("000000"))
                {
                    _TransactionType = TransactionType.BalInquiry;
                }
                else if (_Transaction.ProcessingCode.Equals("940100") || _Transaction.ProcessingCode.Equals("940101") || _Transaction.ProcessingCode.Equals("940102"))
                {
                    _TransactionType = TransactionType.MiniStatement;
                }

                List<string> lstCArdAmt = new List<string>();
                string[] cardAmt = _TransactionBlock.TransactionBlockList[3].Split(' ');
                foreach (var list in cardAmt)
                {
                    if (list == "")
                    {
                    }
                    else
                    {
                        lstCArdAmt.Add(list);
                    }
                }
                if (lstCArdAmt[0].Substring(0, 3) == "000")
                {
                    _Transaction.CardNo = TruncateValue(lstCArdAmt[0], 3, 16);
                }
                else
                {
                    _Transaction.CardNo = lstCArdAmt[0];
                }
                _Transaction.TransactionAmount = decimal.Parse(lstCArdAmt[1]);

                List<string> lstTraceResp = new List<string>();
                string[] traceResp = _TransactionBlock.TransactionBlockList[5].Split(' ');
                foreach (var list in traceResp)
                {
                    if (list == "")
                    {

                    }
                    else
                    {
                        lstTraceResp.Add(list);
                    }
                }
                _Transaction.TraceNo = lstTraceResp[0];
                _Transaction.ResponseCode = GetResponseCode(lstTraceResp[lstTraceResp.Count - 1]);

                _TransactionStatus = TransactionStatus.Success;
                if (!_Transaction.ResponseCode.Equals("0000"))
                {
                    _TransactionStatus = TransactionStatus.Fail;
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
