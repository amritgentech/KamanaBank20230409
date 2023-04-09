 create or alter  procedure [dbo].[sp_VisaVsCbs]
@fromDate varchar(10),
@toDate varchar(10),
@Selector1 varchar(10),
@Selector2 varchar(10)
as
begin
Declare @common_query nvarchar(max)
set @common_query = 'SELECT  distinct
					  TransactionId,
					  CbsTransactionId,
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
                      '+@Selector1+'TransactionGmtDate,
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

                WHEN '+@Selector1+'TransactionDate = '+@Selector2+'transactiondate 
				     AND '+@Selector1+'transactionstatus IN ( 1, 8 ) 
					 AND '+@Selector2+'transactionstatus IN ( 1, 8 )
                     AND '+@Selector1+'transactiontype = 1 
                     AND reconsource = 1 THEN ''Matched'' 

				WHEN        '+@Selector1+'transactionstatus = 9 
                        AND '+@Selector2+'transactionstatus = 9 
					 and reconsource = 1
                     THEN ''Exception''

                WHEN '+@Selector1+'TransactionDate = '+@Selector2+'transactiondate 
                     AND '+@Selector1+'transactiontype = 1 
                     AND CASE 
                           WHEN '+@Selector1+'transactionstatus IN ( 9, 0 ) 
                                AND '+@Selector2+'transactionstatus IN ( 1, 9 ) THEN 1 
                           ELSE 0 
                         END = 1 
                     AND reconsource = 1 THEN ''Failed''
                           
                 WHEN '+@Selector1+'TransactionDate <> '+@Selector2+'transactiondate 
                     AND reconsource = 1 
					 AND '+@Selector1+'transactiontype = 1
                     AND 
                          '+@Selector2+'transactiondate BETWEEN DATEADD(dd,-1,cast('''+@fromDate+''' as date))
                                                          AND DATEADD(dd,1,cast('''+@toDate+''' as date)) 
                          THEN ''DateDiffWithOutInvalid''

                WHEN '+@Selector1+'transactiontype = 1 
                     AND '+@Selector1+'transactionstatus IN ( 1, 8 ) AND '+@Selector2+'transactionstatus IS NULL
                     AND reconsource = 1 
                     THEN ''Invalid''
                WHEN '+@Selector1+'transactionstatus = 0 
                     AND reconsource = 1 
                     AND '+@Selector2+'transactiondate IS NULL 
                     THEN ''Exception'' 
                WHEN '+@Selector2+'transactionstatus = 1 
                     AND '+@Selector1+'TransactionDate IS NULL 
                     AND reconsource = 3 
                     THEN ''Exception'' 
              END Status 
       FROM   vw_transaction_details 
       WHERE  transactionid IS NOT NULL 
               AND '+@Selector1+'TransactionDate BETWEEN 
                         cast('''+@fromDate+''' as date) AND 
                         cast('''+@toDate+''' as date)) a
						    where a.status is not null
			                AND a.'+@Selector1+'transactionstatus <> 4
							and (case 
							when (a.cardtype = 1 and a.terminalowner = 1) or (a.cardtype <> 1 and a.terminalowner <> 1)
							    then 0
								else 1
								end)  = 1
			   '

      Declare @failed_suspected_query nvarchar(max)
set @failed_suspected_query = @common_query+ 'status=''Suspected'' 
               
       FROM   vw_transaction_details 
       WHERE  transactionid IS NOT NULL 
	           AND '+@Selector1+'responsecode IN ( ''0034'', ''0059'', ''0090'' )
               AND '+@Selector1+'TransactionDate BETWEEN 
                         cast('''+@fromDate+''' as date) AND 
                         cast('''+@toDate+''' as date)
			  AND reconsource = 1 
              AND '+@Selector1+'transactiontype = 1
			   AND '+@Selector1+'transactionstatus <> 4
			 and (case 
							when (cardtype = 1 and terminalowner = 1) or (cardtype <> 1 and terminalowner <> 1)
							      then 0
								else 1
								end)  = 1
			  '

Declare @dateDiff_query nvarchar(max)
set @dateDiff_query =@common_query+ 'status=''DateDiff'' 
       FROM   vw_transaction_details 
       WHERE  transactionid IS NOT NULL
	           AND '+@Selector1+'TransactionDate <> '+@Selector2+'transactiondate 
                     AND reconsource = 1 
					 AND '+@Selector1+'transactionstatus IN (1,8)
					 AND '+@Selector1+'transactiontype = 1
                     AND 
					         '+@Selector1+'TransactionDate BETWEEN DATEADD(dd,-1,cast('''+@fromDate+''' as date))
                                                          AND DATEADD(dd,1,cast('''+@toDate+''' as date)) 
						     and
                             '+@Selector2+'transactiondate BETWEEN DATEADD(dd,-1,cast('''+@fromDate+''' as date))
                                                          AND DATEADD(dd,1,cast('''+@toDate+''' as date)) 
			  AND reconsource = 1 
              AND '+@Selector1+'transactiontype = 1
			  AND '+@Selector1+'transactionstatus <> 4
			   and (case 
							when (cardtype = 1 and terminalowner = 1) or (cardtype <> 1 and terminalowner <> 1)
							    then 0
								else 1
								end)  = 1'

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
			  AND transactiondate ='+@Selector2+'transactiondate
			   and (case 
							when (cardtype = 1 and terminalowner = 1) or (cardtype <> 1 and terminalowner <> 1)
							    then 0
								else 1
						end)  = 1'

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


--exec sp_VisaVsCbs '2018-07-01','2018-07-01','visa','cbs'