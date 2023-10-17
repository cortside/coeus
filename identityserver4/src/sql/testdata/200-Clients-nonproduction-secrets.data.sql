UPDATE [AUTH].[ClientSecrets]
SET [Value] = N'K/yv2FT35ao1vDSt/YgIfs8KVq+Ja8lh0J6ZDWF1T9A='
FROM
    [AUTH].[Clients] C
    inner join [AUTH].[ClientSecrets] CS on C.Id = CS.ClientId
WHERE
    C.ClientId = 'shoppingcart-api'
