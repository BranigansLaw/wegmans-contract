@echo off &SETLOCAL
%BATCH_ROOT%\PharmacyBusiness\bin\Wegmans.InterfaceEngine.exe -a RX.PharmacyBusiness.ETL.dll -i DownloadNaloxone
EXIT /B %ERRORLEVEL%