USE [NepalRecon]
GO
/****** Object:  StoredProcedure [dbo].[sp_TwoWayRecon]    Script Date: 1/25/2019 10:22:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER    procedure [dbo].[sp_TwoWayRecon]
@fromDate varchar(10),
@toDate varchar(10),
@Selector1 varchar(10),
@Selector2 varchar(10)
as
begin
  --selector 1 is sourcetransactions ie '+@Selector1+'.
	--selector 2 and selector 3 is destinationtransactions 

			 --reversal make single cbs
			 Update Transactions set TransactionStatus=9 where TransactionId in 
			(Select main.TransactionId from
			(Select * from Transactions where Source_SourceId=3 and TransactionStatus<>4) main
			inner join
			(Select * from Transactions where Source_SourceId=3 and TransactionStatus=4) reversal
			on CONCAT(SUBSTRING(main.CardNo, 0,6), 'XXXXXX', right(main.CardNo,4))=CONCAT(SUBSTRING(reversal.CardNo, 0,6), 'XXXXXX', right(reversal.CardNo,4)) 
			and main.TerminalId=reversal.TerminalId 
			and main.TransactionAmount = -(reversal.TransactionAmount)
			and reversal.TransactionDate between main.TransactionDate and DATEADD(dd,1,cast(main.TransactionDate as date))
			--and main.TransactionDate=reversal.TransactionDate 
			and (main.TraceNo=reversal.TraceNo or (main.TerminalType=2 and main.AuthCode=reversal.AuthCode))
			--or main.UtrNo = reversal.UtrNo
			)


			Delete from Transactions where TransactionId in 
			(Select reversal.TransactionId from
			(Select * from Transactions where Source_SourceId=3 and TransactionStatus=9) main
			inner join
			(Select * from Transactions where Source_SourceId=3 and TransactionStatus=4) reversal
			on CONCAT(SUBSTRING(main.CardNo, 0,6), 'XXXXXX', right(main.CardNo,4))=CONCAT(SUBSTRING(reversal.CardNo, 0,6), 'XXXXXX', right(reversal.CardNo,4))  
			and main.TerminalId=reversal.TerminalId 
			and main.TransactionAmount = -(reversal.TransactionAmount)
			and main.TransactionDate=reversal.TransactionDate 
			and (main.TraceNo=reversal.TraceNo or (main.TerminalType=2 and main.AuthCode=reversal.AuthCode))
			)

		--reversal make single neps
		 Update Transactions set TransactionStatus=9 where TransactionId in 
		(Select main.TransactionId from
		(Select * from Transactions where Source_SourceId=4 and TransactionStatus<>4) main
		inner join
		(Select * from Transactions where Source_SourceId=4 and TransactionStatus=4) reversal
		on CONCAT(SUBSTRING(main.CardNo, 0,6), 'XXXXXX', right(main.CardNo,4))=CONCAT(SUBSTRING(reversal.CardNo, 0,6), 'XXXXXX', right(reversal.CardNo,4)) 
		and main.TerminalId=reversal.TerminalId 
	    --and main.TransactionAmount = reversal.TransactionAmount
	    and reversal.TransactionDate between main.TransactionDate and DATEADD(dd,1,cast(main.TransactionDate as date))
		--and main.TransactionDate=reversal.TransactionDate E:\recon\recon\ReconUi\Sql\sp_TwoWayRecon.sql
		and (main.TraceNo=reversal.TraceNo or (main.TerminalType=2 and main.AuthCode=reversal.AuthCode))
		--or main.UtrNo = reversal.UtrNo
		)


		Delete from Transactions where TransactionId in 
		(Select reversal.TransactionId from
		(Select * from Transactions where Source_SourceId=4 and TransactionStatus=9) main
		inner join
		(Select * from Transactions where Source_SourceId=4 and TransactionStatus=4) reversal
		on CONCAT(SUBSTRING(main.CardNo, 0,6), 'XXXXXX', right(main.CardNo,4))=CONCAT(SUBSTRING(reversal.CardNo, 0,6), 'XXXXXX', right(reversal.CardNo,4))  
		and main.TerminalId=reversal.TerminalId 

	    and reversal.TransactionDate between main.TransactionDate and DATEADD(dd,1,cast(main.TransactionDate as date))
		--and main.TransactionDate=reversal.TransactionDate 
		and (main.TraceNo=reversal.TraceNo or (main.TerminalType=2 and main.AuthCode=reversal.AuthCode))
		--and main.UtrNo = reversal.UtrNo
		)


		--For void txn case..only for pos
		Update Transactions set TransactionStatus = 0  FROM Transactions A
       inner join (
		(Select main.TransactionId mainTransactonid,voidtxn.TransactionId voidTransactionId from 
		(Select * from Transactions where Source_SourceId=4 and TransactionStatus=1) main
		inner join
		(Select * from Transactions where Source_SourceId=4 and TransactionStatus=4) voidtxn
		on CONCAT(SUBSTRING(main.CardNo, 0,6), 'XXXXXX', right(main.CardNo,4))=CONCAT(SUBSTRING(voidtxn.CardNo, 0,6), 'XXXXXX', right(voidtxn.CardNo,4)) 
		and main.TerminalId=voidtxn.TerminalId 
		and main.UtrNo = voidtxn.UtrNo
		--and main.TransactionDate=reversal.TransactionDate 
		and (main.TraceNo=(main.TraceNo+1) 
		or (main.TerminalType=2)))
		)  B
		on A.transactionid = B.mainTransactonid
		or A.TransactionId = B.voidTransactionId

Declare @common_query nvarchar(max)
set @common_query = 'SELECT  distinct
                      CardNumber,
					  TraceNo,
					  ReferenceNo,
					  AuthCode,
					  TerminalId,
					  TerminalType,
					  TerminalOwner,
					  CardType,
					  Currency,
					  AccountNo,
					  CBSRefValue,
					  CbsMainCode,
					  Issuing_Bank,
					  CASE 
					    WHEN '+@Selector1+'TransactionStatus = 9
						  then ''Reversal''
						Else '+@Selector1+'ResponseCodeDescription
					  end '+@Selector1+'ResponseCodeDescription,
					  
					    CASE 
					    WHEN '+@Selector2+'TransactionStatus = 9
						  then ''Reversal''
						WHEN '+@Selector2+'TransactionStatus = 0
						  then ''Fail''
						WHEN '+@Selector2+'TransactionStatus = 1
						  then ''Approve Transaction''
						Else '+@Selector2+'ResponseCodeDescription
					  end '+@Selector2+'ResponseCodeDescription,

					  TransactionDate,
                      '+@Selector1+'TransactionDate,
                      '+@Selector2+'TransactionDate,

					  '+@Selector1+'ResponseCode,
                      '+@Selector2+'ResponseCode,

					  '+@Selector1+'TransactionStatus,
                      '+@Selector2+'TransactionStatus,

					  '+@Selector1+'Amount,
                      '+@Selector2+'Amount,'



Declare @query nvarchar(max)
set @query = 'select * from ('+
@common_query+ 'CASE  
                WHEN '+@Selector1+'transactiondate = '+@Selector2+'transactiondate 
				     AND '+@Selector1+'transactionstatus IN ( 1, 8 ) 
					 AND '+@Selector2+'transactionstatus IN ( 1, 8 )
                     AND '+@Selector1+'transactiontype = 1 
                     AND reconsource = 4  THEN ''Matched'' 

				WHEN        '+@Selector1+'transactionstatus = 9 
                        AND '+@Selector2+'transactionstatus = 9 
					 and reconsource = 4
                     THEN ''Exception''
                WHEN '+@Selector1+'transactiondate = '+@Selector2+'transactiondate 
                     AND '+@Selector1+'transactiontype = 1 
                     AND CASE 
                           WHEN '+@Selector1+'transactionstatus IN ( 9, 0 ) 
                                AND '+@Selector2+'transactionstatus IN ( 1, 9 ) THEN 1 
                           ELSE 0 
                         END = 1 
                     AND reconsource = 4 THEN ''Failed''
                           
                 WHEN '+@Selector1+'transactiondate <> '+@Selector2+'transactiondate 
                     AND reconsource = 4 
					 AND '+@Selector1+'transactiontype = 1
                     AND 
                          '+@Selector2+'transactiondate BETWEEN DATEADD(dd,-1,cast('''+@fromDate+''' as date))
                                                          AND DATEADD(dd,1,cast('''+@toDate+''' as date)) 
                          THEN ''DateDiffWithOutInvalid''
                WHEN '+@Selector1+'transactiontype = 1 
                     AND '+@Selector1+'transactionstatus IN ( 1, 8 ) AND '+@Selector2+'transactionstatus IS NULL
                     AND reconsource = 4 
                     THEN ''Invalid''
                WHEN '+@Selector1+'transactionstatus = 0 
                     AND reconsource = 4 
                     AND '+@Selector2+'transactiondate IS NULL 
                     THEN ''Exception'' 
                WHEN '+@Selector2+'transactionstatus = 1 
                     AND '+@Selector1+'transactiondate IS NULL 
                     AND reconsource = 3 
                     THEN ''Exception'' 
              END Status 
       FROM   vw_transaction_details 
       WHERE  transactionid IS NOT NULL 
               AND '+@Selector1+'transactiondate BETWEEN 
                         cast('''+@fromDate+''' as date) AND 
                         cast('''+@toDate+''' as date)) a
						    where a.status is not null
			                AND a.'+@Selector1+'transactionstatus <> 4
			   '


      Declare @failed_suspected_query nvarchar(max)
set @failed_suspected_query = @common_query+ 'status=''Suspected'' 
               
       FROM   vw_transaction_details 
       WHERE  transactionid IS NOT NULL 
	           AND '+@Selector1+'responsecode IN ( ''0034'', ''0059'', ''0090'' )
               AND '+@Selector1+'transactiondate BETWEEN 
                         cast('''+@fromDate+''' as date) AND 
                         cast('''+@toDate+''' as date)
			  AND reconsource = 4 
              AND '+@Selector1+'transactiontype = 1
			   AND '+@Selector1+'transactionstatus <> 4
			  '
			  			

Declare @dateDiff_query nvarchar(max)
set @dateDiff_query =@common_query+ 'status=''DateDiff'' 
       FROM   vw_transaction_details 
       WHERE  transactionid IS NOT NULL
	           AND '+@Selector1+'transactiondate <> '+@Selector2+'transactiondate 
                     AND reconsource = 4 
					 AND '+@Selector1+'transactionstatus IN (1,8)
					 AND '+@Selector1+'transactiontype = 1
                     AND 
					         '+@Selector1+'transactiondate BETWEEN DATEADD(dd,-1,cast('''+@fromDate+''' as date))
                                                          AND DATEADD(dd,1,cast('''+@toDate+''' as date)) 
						     and
                             '+@Selector2+'transactiondate BETWEEN DATEADD(dd,-1,cast('''+@fromDate+''' as date))
                                                          AND DATEADD(dd,1,cast('''+@toDate+''' as date)) 
			  AND reconsource = 4 
              AND '+@Selector1+'transactiontype = 1
			  AND '+@Selector1+'transactionstatus <> 4'

       Declare @cbsInvalid nvarchar(max)
	  set @cbsInvalid = 
                  @common_query+ 'status=''CbsInvalid'' 
       FROM   vw_transaction_details 
       WHERE  transactionid IS NOT NULL 
	           AND '+@Selector2+'transactionstatus = 1 
              AND '+@Selector1+'transactionstatus IS NULL 
			  AND reconsource = 3
              AND '+@Selector2+'transactiontype = 1
			   AND '+@Selector2+'transactiondate BETWEEN 
                         cast('''+@fromDate+''' as date) AND 
                         cast('''+@toDate+''' as date)
			  AND transactiondate ='+@Selector2+'transactiondate'

exec(
     @query  
	  +' Union '+
	 @dateDiff_query 
	 +' Union '+
	 @failed_suspected_query 
	 +' Union ' +
	 @cbsInvalid
	 );
end

--exec sp_TwoWayRecon '2018-07-01','2018-07-01','npn','cbs'