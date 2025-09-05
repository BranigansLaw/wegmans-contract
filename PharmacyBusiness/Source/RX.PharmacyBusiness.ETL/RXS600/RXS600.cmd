@echo off
Setlocal EnableDelayedExpansion

powershell.exe -ExecutionPolicy Bypass -nologo -noninteractive -noprofile %~dp0RXS600.ps1 %~dp0RXS600.json > %~dp0%~n0.log

EXIT /B %ERRORLEVEL%