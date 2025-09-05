REM Loop execution for a range of dates when two date parameters are provided in MM/DD/YYYY format.
REM If just one date parameter provided, then rerun job just for that one date.
SET START_DATE=%~1
SET END_DATE=%~2
SET FILENAME_FOR_DATE_RANGE=%~3
@ECHO off
setlocal EnableDelayedExpansion

IF "%1"=="" (
    SET START_DATE=%date:~4,10%
)
IF "%2"=="" (
    SET END_DATE=%START_DATE%
)

@ECHO [%date% %time%] Date range provided is from %START_DATE% to %END_DATE%.
CALL:DateToJDN %START_DATE% startJulian
CALL:DateToJDN %END_DATE% endJulian

REM goto bundle_stuff

:loop
    CALL:JDNToDate %startJulian% runDate
    @ECHO Executing with Run Date %runDate%.
    %BATCH_ROOT%\PharmacyBusiness\bin\Wegmans.InterfaceEngine.exe -a RX.PharmacyBusiness.ETL.dll -i DownloadForCDCWithOldCvx -RunDate %runDate%
    IF %ERRORLEVEL% NEQ 0 (
        @ECHO [%date% %time%] ERROR with Run Date [%runDate%].
        EXIT /B %ERRORLEVEL%
    )
    IF %endJulian% gtr %startJulian% (
        SET /a startJulian=startJulian+1
        goto loop
    )
    
:bundle_stuff
REM Bundle all daily files into one file, with the file name provided in third parameter.
REM Naturally, a bundled, or concatonated, file should only have one header row.
SET PATH_RXS532=D:\Batch\RXS532
D:
cd "!PATH_RXS532!"


@ECHO [%date% %time%] Begin concatonating files from [!PATH_RXS532!\Output\Wegmans_CovidVaccinations_*.txt] into one file named [%FILENAME_FOR_DATE_RANGE%] on [%COMPUTERNAME%].
SET /a NBR_FILES=0
SET /a TOTAL_ROWS=1
@FOR /R "%PATH_RXS532%\Output\" %%i IN (Wegmans_CovidVaccinations_*.txt) DO (
    @ECHO Found file [%%~ni.txt].

    IF NOT "%%~ni.txt"=="%FILENAME_FOR_DATE_RANGE%" (
        @ECHO Processing file [%%~ni.txt].
        SET /a NBR_FILES += 1
        SET /a NBR_DATA_ROWS=-1

        @FOR /F "tokens=*" %%A IN (!PATH_RXS532!\Output\%%~ni.txt) DO (
            SET /a NBR_DATA_ROWS += 1

            IF !NBR_DATA_ROWS! EQU 0 (
                IF !NBR_FILES! EQU 1 (
                    REM Output the header row.
                    @ECHO %%A> !PATH_RXS532!\Output\!FILENAME_FOR_DATE_RANGE!
                )
            ) ELSE (
                @ECHO %%A>> !PATH_RXS532!\Output\!FILENAME_FOR_DATE_RANGE!
            )
        )

        @ECHO Concatonated file [%%~ni.txt] with [!NBR_DATA_ROWS!] data rows.
        SET /a TOTAL_ROWS += !NBR_DATA_ROWS!
        IF EXIST "!PATH_RXS532!\Output\%%~ni.txt" move "!PATH_RXS532!\Output\%%~ni.txt" "!PATH_RXS532!\Archive\%%~ni.txt"
    ) ELSE (
        @ECHO Skipping file [%%~ni.txt].
    )
)
@ECHO [%date% %time%] Finished concatonating [%NBR_FILES%] files into one file named [%FILENAME_FOR_DATE_RANGE%] with [!TOTAL_ROWS!] total rows.

goto exit_program


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

:exit_program
EXIT /B 0