SELECT col.TABLE_CATALOG, col.TABLE_SCHEMA, col.TABLE_NAME, tprop.value TABLE_DESCRIPTION, COLUMN_NAME, ORDINAL_POSITION, prop.value AS [COLUMN_DESCRIPTION],
	case col.DATA_TYPE	
		when 'decimal' then cast (col.DATA_TYPE as varchar) + ' ('+cast (NUMERIC_PRECISION as varchar) + ' , '+cast (NUMERIC_SCALE as varchar) +')'
		when 'varchar' then cast (col.DATA_TYPE as varchar) + ' ('+cast (CHARACTER_OCTET_LENGTH as varchar) + ')'
		when 'nvarchar' then cast (col.DATA_TYPE as varchar)+' ('+case when CHARACTER_OCTET_LENGTH<0 then 'MAX' else cast (CHARACTER_OCTET_LENGTH as varchar) end + ')'
		else col.DATA_TYPE
	end DATA_TYPE, col.IS_NULLABLE, sc.object_id
FROM INFORMATION_SCHEMA.TABLES AS tbl
INNER JOIN INFORMATION_SCHEMA.COLUMNS AS col ON col.TABLE_NAME = tbl.TABLE_NAME
INNER JOIN sys.columns AS sc ON sc.object_id = object_id(tbl.table_schema + '.' + tbl.table_name) AND sc.NAME = col.COLUMN_NAME
LEFT JOIN sys.extended_properties prop ON prop.major_id = sc.object_id AND prop.minor_id = sc.column_id AND prop.NAME = 'MS_Description'
LEFT JOIN sys.extended_properties tprop ON tprop.major_id = sc.object_id AND tprop.minor_id=0 AND tprop.NAME = 'MS_Description'
order by col.TABLE_CATALOG, col.TABLE_SCHEMA, col.TABLE_NAME, col.ORDINAL_POSITION
