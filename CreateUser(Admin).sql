USE [AgencyApp]
GO
insert into [User] (UserName, PasswordHash, Role, Status) values ('default@test.org', (SELECT CAST('123456' as varbinary(max)) FOR XML PATH(''), BINARY BASE64), 'Admin', 'Active')
