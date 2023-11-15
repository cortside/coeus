update Auth.ClientClaims
set [Value] = lower([Value])
where [Type] = 'sub';
GO

-- populate LoginCount and LastLogin for any that are not populated that have had successful login attempts
update auth.[user] set LoginCount=l.LoginCount, LastLogin=l.LastLogin
from auth.[user] u
join (
	select userId, count(*) LoginCount, max(attemptedOn) LastLogin from auth.LoginAttempts where Successful=1 group by userId
) l on l.userId=u.userId
where u.LoginCount=0
GO
