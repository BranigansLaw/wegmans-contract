@echo off &SETLOCAL
%BATCH_ROOT%\PharmacyBusiness\bin\Wegmans.InterfaceEngine.exe -a RX.PharmacyBusiness.ETL.dll -i ArchiveScriptsTo1010 -FilterSchemaOwner %~1 -FilterTableName %~2 -OutputDataXmlFile %~3 -PerformCleanString %~4
EXIT /B %ERRORLEVEL%