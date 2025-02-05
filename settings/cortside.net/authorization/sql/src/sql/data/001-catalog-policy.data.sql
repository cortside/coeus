declare @PolicyName varchar(100) = 'Catalog'
declare @PolicyResourceId uniqueidentifier = '15F50AAB-5FBD-406E-A1E0-BDC961B4CDBA' -- used in appsettings.json of calling services

-- upsert policy
exec spAddPolicy @PolicyName, @PolicyResourceId, 'Policy for catalog service'

-- add roles
exec spAddRole @PolicyName, 'Read', 'Has permissions to read service resources'
exec spAddRole @PolicyName, 'Write', 'Has permissions to create and update service resources'

-- assign permissions to roles (creates permission in policy, if non-existent)
exec spAddPermission @PolicyName, 'Read', 'GetCatalog', 'Can get a catalog resource'
exec spAddPermission @PolicyName, 'Write', 'CreateCatalog', 'Can create a catalog resource'
exec spAddPermission @PolicyName, 'Write', 'UpdateCatalog', 'Can update a catalog resource'

-- assign claim types to roles
-- policyname, rolename, claimtype, claimvalue, record's description
exec spAddClaimTypeToRole @PolicyName, 'Write', 'sub', 'b62ecf91-960c-4b65-83a4-50dac6748c7c', 'subject has write permissions'
exec spAddClaimTypeToRole @PolicyName, 'Read', 'sub', '132953b2-f6a7-4c1d-8da1-2b3c3dafe1c5', 'subject has read permissions'
exec spAddClaimTypeToRole @PolicyName, 'Read', 'group', '6e3ddd04-50f8-4aab-b375-3332ef18d8eb', 'Management group id'
