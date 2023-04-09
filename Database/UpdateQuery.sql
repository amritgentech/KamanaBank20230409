select * from TransactionDetails

Select * from Transactions where CardNo like '461004%'

select * from Transactions where TerminalId in ('2030AL01','2050AL02')

update TransactionDetails set CardType=1 
where TransactionDetailId in (Select TransactionDetail_TransactionDetailId from Transactions where CardNo like '461004%')

update TransactionDetails set TerminalOwner=1 
where TransactionDetailId in (Select TransactionDetail_TransactionDetailId from Transactions where TerminalId in ('2030AL01','2050AL02'))