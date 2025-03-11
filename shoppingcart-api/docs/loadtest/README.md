# Use of GUIDs in Microsoft SQL Server

After being pointed to this article on implications of using GUIDs in indexes, specifically primary keys, it made me do some of my own research and analysis:

https://dev.to/connerphillis/sequential-guids-in-entity-framework-core-might-not-be-sequential-3408

The article is worth reading and talks about problems with using GUIDs in indexes.  Microsoft acknowledges the issue and has an issue linked in the article.  It does point out the downsides of using a GUID as a PK, even with using sequential GUIDs.  This article further solidified my stance that tables should have an int/long identity column primary key and to have an alternate GUID unique column and index.  The article does offer some potential options with pros and cons. 

Discussion about .NET 9 has `Guid.CreateVersion7()`, but still does not comply with how the sorting works in Microsoft SQL Server:

https://github.com/dotnet/efcore/issues/33579#issuecomment-2225877527

Compatible library for GUID generation for .NET that has a Microsoft SQL Server sequential GUID (v8):

https://github.com/mareek/UUIDNext

### Run the api:
```powershell
## Recreate a new empty database
.\update-database.ps1 -RebuildDatabase

## start the webapi
.\run.ps1
```

### Run the load tests:
```powershell
## change `$stopTime = (Get-Date).AddMinutes(10)` to the desired worker run duration
## change `$numberOfScripts = 10` to desired number of workers in `run-load.ps1`

## start up the worker threads
.\run-load.ps1
```

### SQL to show index analysis:
```sql
SELECT S.name as 'Schema',
	T.name as 'Table',
	I.name as 'Index',
	DDIPS.avg_fragmentation_in_percent,
	DDIPS.page_count, 
	DDIPS.avg_page_space_used_in_percent
FROM sys.dm_db_index_physical_stats (DB_ID(), NULL, NULL, NULL, 'DETAILED') AS DDIPS
INNER JOIN sys.tables T on T.object_id = DDIPS.object_id
INNER JOIN sys.schemas S on T.schema_id = S.schema_id
INNER JOIN sys.indexes I ON I.object_id = DDIPS.object_id AND DDIPS.index_id = I.index_id
WHERE DDIPS.database_id = DB_ID()
	and I.name is not null
	AND DDIPS.avg_fragmentation_in_percent > 0
ORDER BY DDIPS.avg_fragmentation_in_percent desc

select count(*) from [order]
select count(*) from outbox
```

### Results

With `Guid.NewGuid()`
![with guid.newguid()](Screenshot%202025-03-11%20093712.png)

With `Uuid.NewDatabaseFriendly(Database.SqlServer)`
![with sequential guids](Screenshot%202025-03-11%20090310.png)

I did run this multiple times for durations less than and up to 10 minutes and from 5 to 10 workers.  I had varied results, mostly because of activity on my machine and on my server that was hosting the docker container for SQL Server.  Using `Guid.NewGuid()` I consistently got 93 to 99 percent fragmentation.  Using `Uuid.NewDatabaseFriendly(Database.SqlServer)` I got varied results that varied from 4 to 29, but most commonly in the 16 to 23 percent.  Page count was always higher using `Guid.NewGuid()` with a average space per page used being less.  I did find that more often than not i had higher throughput in tests using `Uuid.NewDatabaseFriendly(Database.SqlServer)` with about 15% increase, despite the screenshots.  My environment and the amount of data I created in the tests made it hard for me to be able to do any evaluation of index performance itself, but the expectation is that index and statistics health would be much better.

My solution at this point, while using Microsoft SQL Server, is to use `Uuid.NewDatabaseFriendly(Database.SqlServer)` from `UuidNext`.  I did evaluate the use of OPTIMIZE_FOR_SEQUENTIAL_KEY on the `IDX_Order_OrderResourceId` index but didn't see anything significantly different leading me to opt to not making use of that.  I made no changes to indexes in the end.