CREATE FUNCTION [dbo].[ConvertGmtToLocal](@transactiondate date ,@transactionTime time)
RETURNS datetime
as
BEGIN

DECLARE @datetime datetime

 set @datetime =
CONVERT(datetime, 
               SWITCHOFFSET(CONVERT(datetimeoffset, 
                                 --   TransactionDate
								 CONVERT(DATETIME, CONVERT(CHAR(8), @transactiondate, 112) 
  + ' ' + CONVERT(CHAR(8), @transactionTime, 108))
									), 
                            DATENAME(TzOffset, SYSDATETIMEOFFSET()))) 


RETURN Convert(date,@datetime)
END