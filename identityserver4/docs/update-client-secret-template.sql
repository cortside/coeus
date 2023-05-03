declare @hash nvarchar(100)
declare @clientId nvarchar(400)
declare @id int
set @clientId='client-name'

select @id=id from auth.clients where clientId=@clientId

-- use this to hash and encode password for storage:  https://dotnetfiddle.net/h3aeqd
-- generate unique secret and update line 8 with that secret

-- update this value to be output of .net fiddle result
set @hash = 'somehashthatendsin='

update auth.ClientSecrets set value=@hash where clientId=@id
select * from auth.ClientSecrets where clientId=@id
