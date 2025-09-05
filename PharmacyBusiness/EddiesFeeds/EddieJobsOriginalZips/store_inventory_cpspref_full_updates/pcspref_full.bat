SET TENTENUID=%TENTENUID2%
SET TENTENPW=%TENTENPW2%

tenup32 --ignore-update -k -y -Y . -8 -C"%CPSAZURECONNECT%" ^
@C:\Scheduled\RetiredHistorical\wegmans.wegmansdata.cps\pcspref_full.xml ^
@C:\Scheduled\RetiredHistorical\wegmans.wegmansdata.cps\pcspref_full.sql



