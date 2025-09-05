@echo off &SETLOCAL
SET jobNbrString=%~1
SET runDateString=%~2
SET executionDateTimeString=%~3

REM ECHO Begin executing JobNotifier for JobNbr [%jobNbrString%] and RunDate [%runDateString%] and ExecutedOn [%executionDateTimeString%].
%BATCH_ROOT%\PharmacyBusiness\bin\Wegmans.InterfaceEngine.exe -a RX.PharmacyBusiness.ETL.dll -i JobNotifier -JobNbr %jobNbrString% -RunDate %runDateString% -ExecutionDateTime %executionDateTimeString%
REM ECHO Completed Execution of JobNotifier.