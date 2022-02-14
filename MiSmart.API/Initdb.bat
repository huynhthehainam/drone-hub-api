@echo off
if exist wwwroot/images del "wwwroot/images" /s /f /q
dotnet ef database drop -f
dotnet ef database update