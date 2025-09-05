@echo off &SETLOCAL

REM Stop auto reruns after 7 PM in case the last run of the day could take four hours.
SET datetime_cutoff=%date:~10,4%%date:~4,2%%date:~7,2%190000
SET START_DATE=12/29/2019
SET END_DATE=12/31/2019
SET NEXT_RUN_DATE_FILE=%BATCH_ROOT%\RXS578\Input\AutoRerunWorkflowSteps3.dat
SET LOG_DATES_RAN_FILE=%BATCH_ROOT%\RXS578\Archive\AutoRerunWorkflowSteps3.log
SET PROGRAM_RETURN_CODE=0

@ECHO History load is complete, and now waiting for Pharmacy Accounting Team to confirm we may now retire Control-M job RXS579.HST.
goto wrap_it_up

IF NOT EXIST "%NEXT_RUN_DATE_FILE%" (
    @ECHO %START_DATE%> %NEXT_RUN_DATE_FILE%
)

REM Get last run date stored in dat file.
SET /p NEXT_RUN_DATE= < %NEXT_RUN_DATE_FILE%

IF "%NEXT_RUN_DATE%"=="" (
    @ECHO [%date% %time%] Error setting NEXT_RUN_DATE from dat file [%NEXT_RUN_DATE_FILE%].
    SET PROGRAM_RETURN_CODE=2
    goto wrap_it_up
)

@ECHO [%date% %time%] Looping through date range from [%START_DATE%] to [%END_DATE%] resuming today with latest run date [%NEXT_RUN_DATE%].

CALL:DateToJDN %NEXT_RUN_DATE% startJulian >NUL
CALL:DateToJDN %END_DATE% endJulian >NUL

:loop_for_date_range
    IF %startJulian% GTR %endJulian% (
        @ECHO [%date% %time%] Finished all reruns.
        SET PROGRAM_RETURN_CODE=0
        goto wrap_it_up
    )

    CALL:JDNToDate %startJulian% runDate >NUL
    @ECHO %runDate%> %NEXT_RUN_DATE_FILE%
    @ECHO [%date% %time%] Executing WorkflowSteps with RunDate [%runDate%].

	%BATCH_ROOT%\PharmacyBusiness\bin\Wegmans.InterfaceEngine.exe -a RX.PharmacyBusiness.ETL.dll -i DownloadWorkloadBalance -TargetRecipients WFS -RunDate %runDate%
    IF %ERRORLEVEL% NEQ 0 (
        SET PROGRAM_RETURN_CODE=%ERRORLEVEL%
        @ECHO [%date% %time%] Error [%PROGRAM_RETURN_CODE%] running WorkflowSteps with RunDate [%runDate%].
        goto wrap_it_up
    ) ELSE (
		@ECHO [%date% %time%] Successfully completed RunDate [%runDate%].>> %LOG_DATES_RAN_FILE%
	)
    
    SET /a startJulian=startJulian+1
    CALL:JDNToDate %startJulian% runDate >NUL
    @ECHO %runDate%> %NEXT_RUN_DATE_FILE%
    SET datetime_now=%date:~10,4%%date:~4,2%%date:~7,2%%time:~0,2%%time:~3,2%%time:~6,2%

    IF %endJulian% GEQ %startJulian% (        
        if "%datetime_now%" lss "%datetime_cutoff%" (
            goto loop_for_date_range
        ) else (
            @ECHO [%date% %time%] Getting too close to midnight, so begin again tomorrow with RunDate [%runDate%].
            SET PROGRAM_RETURN_CODE=0
            goto wrap_it_up
        )
    ) else (
        @ECHO [%date% %time%] Finished all reruns.
        SET PROGRAM_RETURN_CODE=0
        goto wrap_it_up
    )

GOTO:EOF

REM Convert the date to Julian Day Number
:DateToJDN mm.dd.yyyy jdn=
setlocal
set date=%1
set /A yy=%date:~-4%, mm=1%date:~-10,2% %% 100, dd=1%date:~-7,2% %% 100
set /A a=mm-14, jdn=(1461*(yy+4800+a/12))/4+(367*(mm-2-12*(a/12)))/12-(3*((yy+4900+a/12)/100))/4+dd-32075
endlocal & set %2=%jdn%
exit /B

REM Convert Julian Day Number back to date
:JDNToDate jdn mm.dd.yyyy=
setlocal
set /A l=%1+68569,n=(4*l)/146097,l=l-(146097*n+3)/4,i=(4000*(l+1))/1461001,l=l-(1461*i)/4+31,j=(80*l)/2447,dd=l-(2447*j)/80,l=j/11,mm=j+2-(12*l),yy=100*(n-49)+i+l
if %dd% lss 10 set dd=0%dd%
if %mm% lss 10 set mm=0%mm%
endlocal & set %2=%mm%/%dd%/%yy%
exit /B

endlocal

:wrap_it_up
EXIT /B %PROGRAM_RETURN_CODE%