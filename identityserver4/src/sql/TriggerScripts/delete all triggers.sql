select * from audit.AuditLogTransaction
select * from audit.AuditLog

select * 
from INFORMATION_SCHEMA.TABLES 
where TABLE_NAME not in (select TABLE_NAME from INFORMATION_SCHEMA.COLUMNS where COLUMN_NAMe='ModifiedBy')
	and TABLE_NAME in (select TABLE_NAME from INFORMATION_SCHEMA.COLUMNS where COLUMN_NAMe='ApplicationId')



DECLARE @dropTriggersCommand VARCHAR(8000);
SET @dropTriggersCommand = 
'declare @t varchar(100) = ''tr'' + PARSENAME(''?'', 1)
IF EXISTS(SELECT * FROM sys.triggers WHERE name = @t) 
	BEGIN
		print @t
			exec(''DROP TRIGGER ''+@t)
			PRINT ''dropped trigger for ?''
	END
';

exec sp_MSforeachtable @command1 = @dropTriggersCommand;
