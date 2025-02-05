declare @PolicyName varchar(100) = 'SqlReport'
declare @PolicyResourceId uniqueidentifier = 'ca22aed2-f337-4baa-bb33-cbaab3b7a87f' -- used in appsettings.json of calling services

-- upsert policy
exec spAddPolicy @PolicyName, @PolicyResourceId, 'Policy for sqlreport service'

-- add roles
exec spAddRole @PolicyName, 'Support', 'Has permissions to read service resources'

-- assign permissions to roles (creates permission in policy, if non-existent)
exec spAddPermission @PolicyName, 'Support', 'CanGetReports', 'Can get a catalog resource'
exec spAddPermission @PolicyName, 'Support', 'spReport_Tables', 'Can create a catalog resource'
exec spAddPermission @PolicyName, 'Support', 'spReport_Table', 'Can update a catalog resource'
exec spAddPermission @PolicyName, 'Support', 'spReport_OutboxCounts', 'Can update a catalog resource'

-- assign claim types to roles
-- policyname, rolename, claimtype, claimvalue, record's description
exec spAddClaimTypeToRole @PolicyName, 'Support', 'sub', '46f6eecf-e483-4428-a2a9-2bc5f6ce62db', 'subject has write permissions'
