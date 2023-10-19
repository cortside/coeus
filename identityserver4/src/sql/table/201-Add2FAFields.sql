IF COL_LENGTH('AUTH.User', 'TwoFactor') IS NULL
BEGIN
	alter table [AUTH].[User] add TwoFactor nvarchar(50) null
END

IF COL_LENGTH('AUTH.User', 'TwoFactorVerified') IS NULL
BEGIN
	alter table [AUTH].[User] add TwoFactorVerified bit not null default (0)
END
