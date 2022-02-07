@echo off
if exist wwwroot rm -rf wwwroot
dotnet ef database drop -f
dotnet ef migrations remove
dotnet ef migrations add Initial
dotnet ef database update
