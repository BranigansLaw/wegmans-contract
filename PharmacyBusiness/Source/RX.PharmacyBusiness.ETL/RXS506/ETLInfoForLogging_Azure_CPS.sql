SELECT
    'TABLE_NAME:"' +
    w.TABLE_NAME +
    '", LAST_LOAD_DATE:"' +
    Format(DateAdd(hh, 
                   DateDiff(hh, 
                            GetDate() at time zone 'US Eastern Standard Time', 
                            GetUTCDate()), 
                   w.LAST_LOAD_DATE),
           'MM/dd HH:mm:ss') +
    '"' AS LOGGING_INFO
FROM ETL.DW_Last_Load_Watermark w
ORDER BY 1