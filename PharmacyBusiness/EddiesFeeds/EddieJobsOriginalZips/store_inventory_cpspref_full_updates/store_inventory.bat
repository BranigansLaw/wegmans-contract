SET TENTENUID=%TENTENUID2%
SET TENTENPW=%TENTENPW2%

SET logfile1="C:\Scheduled\wegmans.wegmansdata.dwfeeds.store_inv_history\Logs\DW-TenUpLog_StoreInv"%date:~10%%date:~4,2%%date:~7,2%".txt"
set filea="C:\Scheduled\DailyJobLogs\"DW-TenUpLog_%date:~10%%date:~4,2%%date:~7,2%".csv"

echo %date%,%time%,%TENTENUID%,Start,Store Inventory Snapshot Stores 1 to 17 (14 Stores-1)>> %filea%

tenup32 -K -L -a -Y "wegmans_rx" -C"%ERXPRODCONNECT%" ^
@C:\Scheduled\wegmans.wegmansdata.dwfeeds.store_inv_history\store_inv_history.xml ^
@C:\Scheduled\wegmans.wegmansdata.dwfeeds.store_inv_history\store_inventory1of6.sql -vv >%logfile1%

echo %date%,%time%,%TENTENUID%,End,Store Inventory Snapshot Stores 1 to 17 (14 Stores-1)>> %filea%

echo %date%,%time%,%TENTENUID%,Start,Store Inventory Snapshot Stores 18 to 34 (13 Stores-2)>> %filea%

tenup32 -K -L -a -Y "wegmans_rx" -C"%ERXPRODCONNECT%" ^
@C:\Scheduled\wegmans.wegmansdata.dwfeeds.store_inv_history\store_inv_history.xml ^
@C:\Scheduled\wegmans.wegmansdata.dwfeeds.store_inv_history\store_inventory2of6.sql -vv >%logfile1%

echo %date%,%time%,%TENTENUID%,End,Store Inventory Snapshot Stores 18 to 34 (13 Stores-2)>> %filea%

echo %date%,%time%,%TENTENUID%,Start,Store Inventory Snapshot Stores 35 to 55 (20 Stores-3)>> %filea%

tenup32 -K -L -a -Y "wegmans_rx" -C"%ERXPRODCONNECT%" ^
@C:\Scheduled\wegmans.wegmansdata.dwfeeds.store_inv_history\store_inv_history.xml ^
@C:\Scheduled\wegmans.wegmansdata.dwfeeds.store_inv_history\store_inventory3of6.sql -vv >%logfile1%

echo %date%,%time%,%TENTENUID%,End,Store Inventory Snapshot Stores 35 to 55 (20 Stores-3)>> %filea%

echo %date%,%time%,%TENTENUID%,Start,Store Inventory Snapshot Stores 56 to 71 (15 Stores-4)>> %filea%

tenup32 -K -L -a -Y "wegmans_rx" -C"%ERXPRODCONNECT%" ^
@C:\Scheduled\wegmans.wegmansdata.dwfeeds.store_inv_history\store_inv_history.xml ^
@C:\Scheduled\wegmans.wegmansdata.dwfeeds.store_inv_history\store_inventory4of6.sql -vv >%logfile1%

echo %date%,%time%,%TENTENUID%,End,Store Inventory Snapshot Stores 56 to 71 (15 Stores-4)>> %filea%

echo %date%,%time%,%TENTENUID%,Start,Store Inventory Snapshot Stores 73 to 87 (13 Stores-5)>> %filea%

tenup32 -K -L -a -Y "wegmans_rx" -C"%ERXPRODCONNECT%" ^
@C:\Scheduled\wegmans.wegmansdata.dwfeeds.store_inv_history\store_inv_history.xml ^
@C:\Scheduled\wegmans.wegmansdata.dwfeeds.store_inv_history\store_inventory5of6.sql -vv >%logfile1%

echo %date%,%time%,%TENTENUID%,End,Store Inventory Snapshot Stores 73 to 87 (13 Stores-5)>> %filea%

echo %date%,%time%,%TENTENUID%,Start,Store Inventory Snapshot Stores 88 to 199 (24 Stores-6)>> %filea%

tenup32 -K -L -a -Y "wegmans_rx" -C"%ERXPRODCONNECT%" ^
@C:\Scheduled\wegmans.wegmansdata.dwfeeds.store_inv_history\store_inv_history.xml ^
@C:\Scheduled\wegmans.wegmansdata.dwfeeds.store_inv_history\store_inventory6of6.sql -vv >%logfile1%

echo %date%,%time%,%TENTENUID%,End,Store Inventory Snapshot Stores 88 to 199 (24 Stores-6)>> %filea%

echo %date%,%time%,%TENTENUID%,Start,PCSPref Chris>> %filea%

tenup32 --ignore-update -k -y -Y . -8 -C"%CPSAZURECONNECT%" ^
@C:\Scheduled\RetiredHistorical\wegmans.wegmansdata.cps\pcspref_full.xml ^
@C:\Scheduled\RetiredHistorical\wegmans.wegmansdata.cps\pcspref_full.sql -vv >%logfile1%

echo %date%,%time%,%TENTENUID%,End,PCSPref Full>> %filea%

