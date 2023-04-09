using Db.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReconUi.Model
{
    public class TransactionViewModel
    {
        public string TransactionDate { get; set; }

        private String _cardNo;
        public String CardNo
        {
            get
            {
                return _cardNo.Substring(0, 6) + "XXXXXX" + _cardNo.Substring(_cardNo.Length - 4, 4);

            }
            set { this._cardNo = value; }
        }
        public String TraceNo { get; set; }

        public String AuthCode { get; set; }
        public String ResponseCode { get; set; }
        public String ResponseCodeDescription { get; set; }
        private String _referenceNo;
        //        public String ReferenceNo
        //        {
        //            get
        //            {
        //                return String.IsNullOrEmpty(_referenceNo) ? string.Empty : _referenceNo.Substring(_referenceNo.Length - 6, 6);
        //
        //            }
        //            set { this._referenceNo = value; }
        //        }
        public String ReferenceNo { get; set; }
        public String TerminalId { get; set; }
        public Decimal TransactionAmount { get; set; }
        public string AccountNo { set; get; }
        public string Currency { set; get; }
        public DateTime? CBSValueDate { set; get; }
        public DateTime? AdviseDate { set; get; }

        public String TransactionStatus { set; get; }
    }
}
