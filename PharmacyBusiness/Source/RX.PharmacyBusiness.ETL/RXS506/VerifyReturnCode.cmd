REM Verify ReturnCode=10 bubbles up to Control-M without running any actual programs - this is just a simulation.
@ECHO off
setlocal enabledelayedexpansion

REM Modify MAX_RETRIES and SECONDS_PER_WAIT as needed. 
REM Note that 300 seconds is five minutes of waiting between retries, and if started at 1 AM then 36 retries should take us to about 4 AM.
SET /a MAX_RETRIES=3
SET /a SECONDS_PER_WAIT=30

@FOR /F "tokens=2 delims==" %%I IN ('wmic os get localdatetime /format:list') DO @SET TIME_STAMP=%%I
SET DATE_TODAY=!TIME_STAMP:~0,8!
SET /a RETRY_NUMBER=0
@ECHO Running verification of Return Code with maximumn [!MAX_RETRIES!] retries and [!SECONDS_PER_WAIT!] seconds wait between each retry.

:loop
    @ECHO Simulating program failed with Error Code [1].
	SET RETURNED_ERROR_LEVEL=1
    IF !RETURNED_ERROR_LEVEL! NEQ 0 (

		IF !RETRY_NUMBER! EQU !MAX_RETRIES! (
			@ECHO Program failed with Error Code [!RETURNED_ERROR_LEVEL!], so exiting after [!MAX_RETRIES!] retries with RC=10 for Control-M.
            EXIT /B 10
		)

        @ECHO Program failed with Error Code [!RETURNED_ERROR_LEVEL!], so now waiting [!SECONDS_PER_WAIT!] seconds, then will try again...
        ping 127.0.0.1 -n !SECONDS_PER_WAIT! >NUL
		
		SET /a RETRY_NUMBER += 1
		@ECHO Running retry number [!RETRY_NUMBER!] of maximumn [!MAX_RETRIES!] retries.

        goto loop
    )

@ECHO Program succeeded with Error Code [0], so exiting after [!MAX_RETRIES!] retries with RC=0 for Control-M.
EXIT /B 0
