REM Loop execution for a range of dates when two date parameters are provided.
REM If just one date parameter provided, then rerun job just for that one date.
@echo off &SETLOCAL
SET START_DATE=%~1
SET END_DATE=%~2

IF "%1"=="" (
    SET START_DATE=%date:~4,10%
)
IF "%2"=="" (
    SET END_DATE=%START_DATE%
)

@ECHO Date range is from %START_DATE% to %END_DATE%.
CALL:DateToJDN %START_DATE% startJulian
CALL:DateToJDN %END_DATE% endJulian

:loop
    CALL:JDNToDate %startJulian% runDate
    @ECHO Executing with Run Date %runDate%.
    %BATCH_ROOT%\PharmacyBusiness\bin\Wegmans.InterfaceEngine.exe -a RX.PharmacyBusiness.ETL.dll -i DownloadPatientAddresses -RunDate %runDate%
    IF %ERRORLEVEL% NEQ 0 (
        @ECHO "Error with Run Date %runDate% !!!""
        EXIT /B %ERRORLEVEL%
    )
    IF %endJulian% gtr %startJulian% (
        SET /a startJulian=startJulian+1
        goto loop
    )
EXIT /B 0

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

EXIT /B %ERRORLEVEL%
