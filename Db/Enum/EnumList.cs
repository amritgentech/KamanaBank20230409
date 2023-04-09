using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Enum
{
    public enum TransactionType
    {
        NotDefine = -1,
        BalInquiry = 0,
        Financial = 1,
        PinChange = 2,
        MiniStatement = 3,
        TransactionCharge = 4,
        Other = 5
    }

    public enum CardType
    {
        NotDefine = -1,
        OwnCard = 1,
        OffUsCard = 2,
        ForeignCard = 3,
        CreditCard = 4,
        MasterCard = 5
    }

    public enum TerminalOwner
    {
        NotDefine = -1,
        OwnTerminal = 1,
        OffUsTerminal = 2,
        ForeignTerminal = 3,
        INRTerminal = 4
    }

    public enum TerminalType
    {
        NotDefine = -1,
        ATM = 1,
        POS = 2,
        ALL = 3
    }

    public enum NetworkType
    {
        NotDefine = -1,
        SCT = 1,
        NPN = 2,
        VISA = 3,
        HBL = 4,
        OTHER = 5
    }


    /// <summary>
    /// 0,1 -- all, 4 -- except EJ,, to be considered fro recon
    /// 3,7 -- EJ
    /// 2   -- VISA + NPN
    /// 5,6 -- VISA
    /// </summary>
    public enum TransactionStatus
    {
        NotDefine = -1,
        Fail = 0,
        Success = 1,
        DuplicateTransaction = 2,
        Retracted = 3,
        Reversal = 4,
        Representment = 5,
        Credit_Adjustment = 6,
        Sensor_Failure = 7,
        Success_With_Suspected = 8, // NEPS
        Transaction_With_Reversal = 9,
        DateDiff = 10,
        NoDataInCBS = 11,
        MatchedWithCBS = 12,
        Undefined = -2,
        Exception = 13
    }

    public enum ReconStatus
    {
        Matched = 1,
        Failed = 2,  // is unmatched also
        Invalid = 3,
        DateDiff = 4,
        Exception = 5,
        Suspected = 6,
        CbsInvalid = 7,
        DateDiffNepsCbs = 8,
        Missing
    }

    public enum PhysicalCassettePosition
    {
        NotDefine = -1,
        First = 1,
        Second = 2,
        Third = 3,
        Fourth = 4
    }

    public enum SettlementFlag
    {
        UnSettled = 0,
        AutoSettled = 1,
        ManualSettled = 2
    }
    public enum ReconTypeMapWithMethod
    {
        VisaCbsTransactions = 1,
        VisaCbsEjTransactions = 4,
        CBSEJTransactions = 2,
        NPNCBSTransactions = 7,
        NPNCBSEJTransactions = 10,
        SwitchCBSTransactions = 5,
        SwitchCBSEJTransactions = 12
    }

    public enum Currency { NPR = 524, INR = 356, USD, GBP }
    public enum IsOwnUsPayableReceivable
    {
        OnUs = 1,
        Receivable = 2,
        Payable = 3
    }
    public enum ParameterDataType
    {
        String, Number, Date, DropDown
    }
}
