update Auth.ClientClaims
set [Value] = lower([Value])
where [Type] = 'sub';
GO

