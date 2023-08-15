if not exists(select 1 from [Subject] where [SubjectId] = '00000000-0000-0000-0000-000000000000')
  begin
	insert into Subject (SubjectId, UserPrincipalName, CreatedDate) values('00000000-0000-0000-0000-000000000000', 'system', getdate())
  end
