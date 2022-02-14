@echo off
if exist wwwroot/images del "wwwroot/images" /s /f /q
dotnet ef database drop -f
dotnet ef migrations remove
dotnet ef migrations add Initial
dotnet ef database update
