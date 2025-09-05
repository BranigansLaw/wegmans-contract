	SELECT    
	right('000' + cast(dbo.[TRANSACTION].Store as varchar(3)),3),
	cast(datepart(yyyy,dbo.[TRANSACTION].TxDate) as varchar(4)) + right('0'+cast(datepart(MM,dbo.[TRANSACTION].TxDate) as varchar(2)),2) + right('0'+cast(datepart(DD,dbo.[TRANSACTION].TxDate) as varchar(2)),2),  
	right('0'+cast(datepart(hh,dbo.[TRANSACTION].TxDate) as varchar(4)),2) + right('0'+cast(datepart(mi,dbo.[TRANSACTION].TxDate) as varchar(2)),2) + right('0'+cast(datepart(ss,dbo.[TRANSACTION].TxDate) as varchar(2)),2),    
	left(cast(dbo.[TRANSACTION].KeyId as char(3)) + ' ',3) ,
	case 
	when dbo.[TRANSACTION].Quantity <> 0 then right('0000000'+cast(dbo.[TRANSACTION].Quantity as varchar(7)),7)
	else '       '
	END,
	right('   '+cast(dbo.[TRANSACTION].terminal as varchar(3)),3),
	right(' ' +cast(dbo.[TRANSACTION].VoidTx as varchar(1)),1),
	' RESERVE FOR FUTURE EXPANSION. ~'
	FROM         dbo.[TRANSACTION] 
	Where  convert(nvarchar(10),dbo.[TRANSACTION].TxDate, 101) = convert(nvarchar(10),dateadd(day, -1, @RunDate), 101)
	order by Store, TxDate, 
	case
	when keyid = 'ENT' then '99'
	else cast(substring(keyid,2,2) as int)
	END,
keyid

