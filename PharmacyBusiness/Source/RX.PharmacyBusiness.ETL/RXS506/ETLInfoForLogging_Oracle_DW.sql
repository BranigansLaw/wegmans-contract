SELECT 'START_TIME:"' ||
       To_Char(er.start_time,'MM/DD HH24:MI:SS') ||
       '", END_TIME:"' ||
       To_Char(er.end_time,'MM/DD HH24:MI:SS') ||
       '", RUN_TYPE:"' ||
       er.run_type ||
       '", STATUS:"' ||
       er.status ||
       '"' AS LOGGING_INFO
FROM  TREXONE_ODS_DATA.ETL_Run er
WHERE er.start_time = (
     SELECT Max(er2.start_time)
     FROM TREXONE_ODS_DATA.ETL_Run er2
     WHERE er2.start_time > (sysdate - 2)
)