REM There is one parameter for which database to check the ETL. If blank it checks all available.
REM Set the parameter to either "AZURE_CPS" or "ORACLE_DW" if just want to check that one database.
@ECHO off
setlocal enabledelayedexpansion
SET DATABASE=%~1

REM Modify MAX_RETRIES and SECONDS_PER_WAIT as needed. 
REM Note that 300 seconds is five minutes of waiting between retries, and if started at 1 AM then 36 retries should take us to about 4 AM.
SET /a MAX_RETRIES=36
SET /a SECONDS_PER_WAIT=300
SET /a RC=0

IF "%1"=="" (
    SET DATABASE=ALL
)

@FOR /F "tokens=2 delims==" %%I IN ('wmic os get localdatetime /format:list') DO @SET TIME_STAMP=%%I
SET DATE_TODAY=!TIME_STAMP:~0,8!
SET /a RETRY_NUMBER=0
@ECHO Running McKesson ETL Check in database [!DATABASE!] on [!DATE_TODAY!] with maximumn [!MAX_RETRIES!] retries and [!SECONDS_PER_WAIT!] seconds wait between each retry.

:loop
    %BATCH_ROOT%\PharmacyBusiness\bin\Wegmans.InterfaceEngine.exe -a RX.PharmacyBusiness.ETL.dll -i McKessonETLCheck -Database !DATABASE!
    SET RETURNED_ERROR_LEVEL=!ERRORLEVEL!
    IF !RETURNED_ERROR_LEVEL! NEQ 0 (

		IF !RETRY_NUMBER! EQU !MAX_RETRIES! (
			@ECHO Returned error code [!RETURNED_ERROR_LEVEL!], so exiting after [!MAX_RETRIES!] retries with RC=10.
            SET RC=10
			goto finish
		)

        @ECHO Returned error code [!RETURNED_ERROR_LEVEL!], so now waiting [!SECONDS_PER_WAIT!] seconds, then will try again...
        ping 127.0.0.1 -n !SECONDS_PER_WAIT! >NUL
		
		SET /a RETRY_NUMBER += 1
		@ECHO Running retry number [!RETRY_NUMBER!] of maximumn [!MAX_RETRIES!] retries.

        goto loop
    )

@ECHO McKesson ETL in database [!DATABASE!] completed successfully.

:finish
EXIT /B %RC%