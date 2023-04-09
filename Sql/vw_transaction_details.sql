create or alter  view [dbo].[vw_transaction_details] as
SELECT       DISTINCT
             total_txns.TransactionId,
             total_txns.CardNo CardNumber,
			 total_txns.CardType CardType,
	         total_txns.Source_SourceId ReconSource,
	         total_txns.TerminalId,
	         total_txns.TraceNo,
	         total_txns.ReferenceNo,
	         total_txns.AuthCode,
	         total_txns.TerminalOwner,
			 total_txns.TransactionDate,
			 total_txns.TransactionType,
			 total_txns.TransactionStatus,
			 total_txns.TerminalType,
			 total_txns.Currency,
			 --total_txns.AccountNo,
			 Neps.TransactionId NepsTransactionId,
			 Neps.TransactionAmount NepsAmount,
	         Neps.ResponseCode NepsResponseCode,
	         Neps.TransactionDate NepsTransactionDate,
	         Neps.TransactionStatus NepsTransactionStatus,
			 Neps.TransactionType NepsTransactionType,
			 Neps.ResponseCodeDescription NepsResponseCodeDescription,
			 Neps.AdviseDate NepsAdviseDate,
			 Cbs.TransactionId CbsTransactionId,
			 Cbs.TransactionAmount CbsAmount,
	         Cbs.ResponseCode CbsResponseCode,
	         Cbs.TransactionDate CbsTransactionDate,
	         Cbs.TransactionStatus CbsTransactionStatus,
	         Cbs.TransactionType CbsTransactionType,
			 Cbs.CBSRefValue CBSRefValue,
			 Cbs.ResponseCodeDescription CbsResponseCodeDescription,
			 Cbs.MainCode CbsMainCode,
			 Cbs.AdviseDate CbsAdviseDate,
			 Cbs.AccountNo AccountNo,
			 Npn.TransactionId NpnTransactionId,
			 Npn.TransactionAmount NpnAmount,
	         Npn.ResponseCode NpnResponseCode,
	         Npn.TransactionDate NpnTransactionDate,
	         Npn.TransactionStatus NpnTransactionStatus,
			 Npn.TransactionType NpnTransactionType,
			 Npn.ResponseCodeDescription NpnResponseCodeDescription,
			 Npn.AdviseDate NpnAdviseDate,
	         Ej.TransactionId EjTransactionId,
			 Ej.TransactionAmount EjAmount,
	         Ej.ResponseCode EjResponseCode,
	         Ej.TransactionDate EjTransactionDate,
	         Ej.TransactionStatus EjTransactionStatus,
			 Ej.TransactionType EjTransactionType,
			 Ej.ResponseCodeDescription EjResponseCodeDescription,
			 Ej.AdviseDate EjAdviseDate,
			 Ej.Currency EjCurrency,
			 Visa.TransactionId VisaTransactionId,
			 Visa.TransactionAmount VisaAmount,
			 Visa.Currency VisaCurrency,
	         Visa.ResponseCode VisaResponseCode,
	         Visa.TransactionDate VisaTransactionGmtDate,
			 Visa.GmtToLocalTransactionDate VisaTransactionDate,
	         Visa.TransactionStatus VisaTransactionStatus,
			 Visa.TransactionType VisaTransactionType,
			 Visa.ResponseCodeDescription VisaResponseCodeDescription,
			 Visa.AdviseDate VisaAdviseDate

FROM Transactions total_txns 
     FULL JOIN Transactions Neps 
	 ON Neps.Source_SourceId = 0
	    AND CONCAT(SUBSTRING(Neps.CardNo, 0, 6),'XXXXXX',right(Neps.CardNo, 4)) = CONCAT (SUBSTRING(total_txns.CardNo, 0, 6),'XXXXXX',right(total_txns.CardNo, 4))
	    AND ( CASE WHEN (Neps.TraceNo IS NOT NULL AND total_txns.TraceNo IS NOT NULL) AND (Neps.traceno = total_txns.traceno)	 THEN 1
			       WHEN (Neps.TraceNo IS NOT NULL AND total_txns.TraceNo IS NOT NULL) AND (Neps.traceno <> total_txns.traceno) THEN 0
			       WHEN (Neps.ReferenceNo IS NOT NULL AND total_txns.ReferenceNo IS NOT NULL) AND Neps.referenceno = total_txns.referenceno THEN 1
			       WHEN (Neps.ReferenceNo IS NOT NULL AND total_txns.ReferenceNo IS NOT NULL) AND Neps.referenceno <> total_txns.referenceno THEN 0
			       WHEN (Neps.AuthCode IS NOT NULL AND total_txns.AUTHCODE IS NOT NULL) AND Neps.AUTHCODE = total_txns.AUTHCODE THEN 1
			       WHEN (Neps.AuthCode IS NOT NULL AND total_txns.AUTHCODE IS NOT NULL) AND Neps.AUTHCODE <> total_txns.AUTHCODE THEN 0
			  ELSE 0
			  END ) = 1
	    AND Neps.TerminalId = total_txns.TerminalId
	    --AND CAST(Neps.TransactionDate AS DATE) = CAST(total_txns.TransactionDate AS DATE)
     FULL JOIN Transactions Npn 
	 ON Npn.Source_SourceId = 4
	    AND CONCAT(SUBSTRING(Npn.CardNo, 0, 6),'XXXXXX',right(Npn.CardNo, 4)) = CONCAT (SUBSTRING(total_txns.CardNo, 0, 6),'XXXXXX',right(total_txns.CardNo, 4))
	    AND ( CASE WHEN (Npn.TraceNo IS NOT NULL AND total_txns.TraceNo IS NOT NULL) AND (Npn.traceno = total_txns.traceno)	 THEN 1
			       WHEN (Npn.TraceNo IS NOT NULL AND total_txns.TraceNo IS NOT NULL) AND (Npn.traceno <> total_txns.traceno) THEN 0
			       WHEN (Npn.ReferenceNo IS NOT NULL AND total_txns.ReferenceNo IS NOT NULL) AND Npn.referenceno = total_txns.referenceno THEN 1
			       WHEN (Npn.ReferenceNo IS NOT NULL AND total_txns.ReferenceNo IS NOT NULL) AND Npn.referenceno <> total_txns.referenceno THEN 0
			       WHEN (Npn.AuthCode IS NOT NULL AND total_txns.AUTHCODE IS NOT NULL) AND Npn.AUTHCODE = total_txns.AUTHCODE THEN 1
			       WHEN (Npn.AuthCode IS NOT NULL AND total_txns.AUTHCODE IS NOT NULL) AND Npn.AUTHCODE <> total_txns.AUTHCODE THEN 0
			  ELSE 0
			  END ) = 1
	    AND Npn.TerminalId = total_txns.TerminalId
	    --AND CAST(Npn.TransactionDate AS DATE) = CAST(total_txns.TransactionDate AS DATE)
     FULL JOIN Transactions Ej 
	 ON Ej.Source_SourceId = 2
	    AND CONCAT(SUBSTRING(Ej.CardNo, 0, 6),'XXXXXX',right(Ej.CardNo, 4)) = CONCAT (SUBSTRING(total_txns.CardNo, 0, 6),'XXXXXX',right(total_txns.CardNo, 4))
	    AND ( CASE WHEN (Ej.TraceNo IS NOT NULL AND total_txns.TraceNo IS NOT NULL) AND (Ej.traceno = total_txns.traceno)	 THEN 1
			       WHEN (Ej.TraceNo IS NOT NULL AND total_txns.TraceNo IS NOT NULL) AND (Ej.traceno <> total_txns.traceno) THEN 0
			       WHEN (Ej.ReferenceNo IS NOT NULL AND total_txns.ReferenceNo IS NOT NULL) AND Ej.referenceno = total_txns.referenceno THEN 1
			       WHEN (Ej.ReferenceNo IS NOT NULL AND total_txns.ReferenceNo IS NOT NULL) AND Ej.referenceno <> total_txns.referenceno THEN 0
			       WHEN (Ej.AuthCode IS NOT NULL AND total_txns.AUTHCODE IS NOT NULL) AND Ej.AUTHCODE = total_txns.AUTHCODE THEN 1
			       WHEN (Ej.AuthCode IS NOT NULL AND total_txns.AUTHCODE IS NOT NULL) AND Ej.AUTHCODE <> total_txns.AUTHCODE THEN 0
			  ELSE 0
			  END ) = 1
	    AND Ej.TerminalId = total_txns.TerminalId
	    --AND CAST(Ej.TransactionDate AS DATE) = CAST(total_txns.TransactionDate AS DATE)
	FULL JOIN Transactions Cbs 
	ON Cbs.Source_SourceId = 3
	   AND CONCAT(SUBSTRING(Cbs.CardNo, 0, 6),'XXXXXX',right(Cbs.CardNo, 4)) = CONCAT (SUBSTRING(total_txns.CardNo, 0, 6),'XXXXXX',right(total_txns.CardNo, 4))
	   AND ( CASE WHEN (Cbs.TraceNo IS NOT NULL AND total_txns.TraceNo IS NOT NULL) AND (Cbs.traceno = total_txns.traceno)	 THEN 1
		       WHEN (Cbs.TraceNo IS NOT NULL AND total_txns.TraceNo IS NOT NULL) AND (Cbs.traceno <> total_txns.traceno) THEN 0
		       WHEN (Cbs.ReferenceNo IS NOT NULL AND total_txns.ReferenceNo IS NOT NULL) AND Cbs.referenceno = total_txns.referenceno THEN 1
		       WHEN (Cbs.ReferenceNo IS NOT NULL AND total_txns.ReferenceNo IS NOT NULL) AND Cbs.referenceno <> total_txns.referenceno THEN 0
		       WHEN (Cbs.AuthCode IS NOT NULL AND total_txns.AUTHCODE IS NOT NULL) AND Cbs.AUTHCODE = total_txns.AUTHCODE THEN 1
		       WHEN (Cbs.AuthCode IS NOT NULL AND total_txns.AUTHCODE IS NOT NULL) AND Cbs.AUTHCODE <> total_txns.AUTHCODE THEN 0
		  ELSE 0
		  END ) = 1
	  AND ( CASE WHEN total_txns.CardType = 1 and total_txns.TerminalOwner <> 1 then 1 -- There are no terminalId in Pumori Cbs for Off Us payable  txn  ..
			WHEN Cbs.TerminalId = total_txns.TerminalId THEN 1
		ELSE 0   
		END ) = 1
	   --AND CAST(Cbs.TransactionDate AS DATE) = CAST(total_txns.TransactionDate AS DATE)
	FULL JOIN Transactions Visa 
	ON Visa.Source_SourceId = 1
	   AND CONCAT(SUBSTRING(Visa.CardNo, 0, 6),'XXXXXX',right(Visa.CardNo, 4)) = CONCAT (SUBSTRING(total_txns.CardNo, 0, 6),'XXXXXX',right(total_txns.CardNo, 4))
	   AND ( CASE WHEN (Visa.TraceNo IS NOT NULL AND total_txns.TraceNo IS NOT NULL) AND (Visa.traceno = total_txns.traceno)	 THEN 1
		       WHEN (Visa.TraceNo IS NOT NULL AND total_txns.TraceNo IS NOT NULL) AND (Visa.traceno <> total_txns.traceno) THEN 0
		       WHEN (Visa.AuthCode IS NOT NULL AND total_txns.AUTHCODE IS NOT NULL) AND Visa.AUTHCODE = total_txns.AUTHCODE THEN 1
		       WHEN (Visa.AuthCode IS NOT NULL AND total_txns.AUTHCODE IS NOT NULL) AND Visa.AUTHCODE <> total_txns.AUTHCODE THEN 0
		       WHEN (Visa.ReferenceNo IS NOT NULL AND total_txns.ReferenceNo IS NOT NULL) AND Visa.referenceno = total_txns.referenceno THEN 1
		       WHEN (Visa.ReferenceNo IS NOT NULL AND total_txns.ReferenceNo IS NOT NULL) AND Visa.referenceno <> total_txns.referenceno THEN 0
		  ELSE 0
		  END ) = 1
	   AND Visa.TerminalId = total_txns.TerminalId
	   --AND CAST(Visa.TransactionDate AS DATE) = CAST(total_txns.TransactionDate AS DATE)