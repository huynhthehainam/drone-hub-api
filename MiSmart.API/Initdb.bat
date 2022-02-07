@echo off
if exist wwwroot rm -rf wwwroot
dotnet ef database drop -f
dotnet ef database update