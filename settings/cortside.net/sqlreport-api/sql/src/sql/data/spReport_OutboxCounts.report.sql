declare @reportGroupId int
declare @reportId int
declare @stateQueryId int
declare @name nvarchar(50)

set @name = 'spReport_OutboxCounts'
--update ReportArgumentQuery set ArgQuery='select distinct a.St ID, a.St State from eboa.dbo.Contractor c join eboa.dbo.Address a on c.MailingAddressId=a.AddressID' where ReportArgumentQueryId=2
--exec @stateQueryId = spAddReportArgumentQuery 'select distinct a.St ID, a.St State from eboa.dbo.Contractor c join eboa.dbo.Address a on c.MailingAddressId=a.AddressID'

set @reportGroupId = 1
exec @reportId = spAddReport @name, 'spReport_OutboxCounts', @reportGroupId
--exec spAddReportArgument @reportId, 'Reference Number', '@referenceNumber', 'BigInt', null, 1
--exec spAddReportArgument @reportId, 'End Date', '@enddate', 'DateTime', null, 2
--exec spAddReportArgument @reportId, 'State', '@state', 'varchar(2)', @stateQueryId, 3

--update ReportArgument set ArgName='@state', ArgType='varchar(2)' where ReportArgumentId=6

--delete from ReportArgumentQuery where ReportArgumentQueryId not in (select ReportArgumentQueryId from ReportArgument where ReportArgumentQueryId is not null)

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spReport_OutboxCounts]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].spReport_OutboxCounts
GO

CREATE PROCEDURE [dbo].spReport_OutboxCounts
AS
BEGIN
	SET NOCOUNT ON;

    select 'LoanServicing' db, RoutingKey, count(*) messages, min(createddate), max(createddate)
    from {{LoanServicingDB}}.dbo.outbox with (nolock) 
    where ScheduledDate < GETUTCDATE() 
    group by RoutingKey
    union
    select 'DataMart', RoutingKey, count(*), min(createddate), max(createddate)
    from {{DataMartDB}}.dbo.outbox with (nolock) 
    where ScheduledDate < GETUTCDATE() 
    group by RoutingKey
    union
    select 'FundingManagement', RoutingKey, count(*), min(createddate), max(createddate)
    from {{FundingManagementDB}}.dbo.outbox with (nolock) 
    where ScheduledDate < GETUTCDATE() 
    group by RoutingKey
    union
    select 'PartnerPortal', RoutingKey, count(*), min(createddate), max(createddate)
    from {{PartnerPortalDB}}.dbo.outbox with (nolock) 
    where ScheduledDate < GETUTCDATE() 
    group by RoutingKey
    union
    select 'Document', RoutingKey, count(*), min(createddate), max(createddate)
    from {{DocumentDB}}.dbo.outbox with (nolock) 
    where ScheduledDate < GETUTCDATE() 
    group by RoutingKey
    union
    select 'Comms', RoutingKey, count(*), min(createddate), max(createddate)
    from {{CommsDB}}.dbo.outbox with (nolock) 
    where ScheduledDate < GETUTCDATE() 
    group by RoutingKey
    union
    select 'Application', RoutingKey, count(*), min(createddate), max(createddate)
    from {{ApplicationDB}}.dbo.outbox with (nolock) 
    where ScheduledDate < GETUTCDATE() 
    group by RoutingKey
    union
    select 'Product', RoutingKey, count(*), min(createddate), max(createddate)
    from {{ProductDB}}.dbo.outbox with (nolock) 
    where ScheduledDate < GETUTCDATE() 
    group by RoutingKey
    union
    select 'User', RoutingKey, count(*), min(createddate), max(createddate)
    from [{{UserDB}}].dbo.outbox with (nolock) 
    where ScheduledDate < GETUTCDATE() 
    group by RoutingKey
END
GO
