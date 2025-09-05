@ECHO off
setlocal enabledelayedexpansion

REM Modify MAX_RETRIES and SECONDS_PER_WAIT as needed. 
REM Note that 300 seconds is five minutes of waiting between retries, and if started at 1 AM then 84 retries should take us to about 9 AM.
SET /a MAX_RETRIES=84
SET /a SECONDS_PER_WAIT=300

@FOR /F "tokens=2 delims==" %%I IN ('wmic os get localdatetime /format:list') DO @SET TIME_STAMP=%%I
SET DATE_TODAY=!TIME_STAMP:~0,8!
SET /a RETRY_NUMBER=0
@ECHO Running a check on row counts in each table within McKesson Azure CPS database on [!DATE_TODAY!] with maximumn [!MAX_RETRIES!] retries and [!SECONDS_PER_WAIT!] seconds wait between each retry.
@ECHO If any row counts change during this job run, an error will be returned (RC=1) so that a ticket may be opened with McKesson, and we might need to rerun some jobs querying this CPS database.

:loop
    %BATCH_ROOT%\PharmacyBusiness\bin\Wegmans.InterfaceEngine.exe -a RX.PharmacyBusiness.ETL.dll -i McKessonETLCheck -Database CPS_RECORD_COUNTS
    SET RETURNED_ERROR_LEVEL=!ERRORLEVEL!
    IF !RETURNED_ERROR_LEVEL! NEQ 0 (
	    @ECHO Returned error code [!RETURNED_ERROR_LEVEL!], so exiting after [!MAX_RETRIES!] retries with RC=1.
        EXIT /B 1
    )

    IF !RETRY_NUMBER! LEQ !MAX_RETRIES! (
        @ECHO Returned error code [!RETURNED_ERROR_LEVEL!], so now waiting [!SECONDS_PER_WAIT!] seconds, then will try again...
        ping 127.0.0.1 -n !SECONDS_PER_WAIT! >NUL
		
		SET /a RETRY_NUMBER += 1
		@ECHO Running retry number [!RETRY_NUMBER!] of maximumn [!MAX_RETRIES!] retries.

        goto loop
    )

@ECHO McKesson Azure CPS database had static row counts since the ETL completed.
EXIT /B 0
