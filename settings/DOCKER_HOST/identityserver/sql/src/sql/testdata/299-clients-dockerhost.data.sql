insert into auth.ClientCorsOrigins
select clientId, replace(origin, 'kehlstein', 'DOCKER_HOST')
from auth.ClientCorsOrigins y
where Origin like '%kehlstein%'
	and not exists (select 1 from auth.ClientCorsOrigins x where x.origin = replace(y.origin, 'kehlstein', 'DOCKER_HOST') and x.ClientId=y.ClientId)

insert into auth.ClientPostLogoutRedirectUris
select clientId, replace(y.PostLogoutRedirectUri, 'kehlstein', 'DOCKER_HOST')
from auth.ClientPostLogoutRedirectUris y
where PostLogoutRedirectUri like '%kehlstein%'
	and not exists (select 1 from auth.ClientPostLogoutRedirectUris x where x.PostLogoutRedirectUri = replace(y.PostLogoutRedirectUri, 'kehlstein', 'DOCKER_HOST') and x.ClientId=y.ClientId)

insert into auth.ClientRedirectUris
select clientId, replace(RedirectUri, 'kehlstein', 'DOCKER_HOST')
from auth.ClientRedirectUris y
where RedirectUri like '%kehlstein%'
	and not exists (select 1 from auth.ClientRedirectUris x where x.RedirectUri = replace(y.RedirectUri, 'kehlstein', 'DOCKER_HOST') and x.ClientId=y.ClientId)
