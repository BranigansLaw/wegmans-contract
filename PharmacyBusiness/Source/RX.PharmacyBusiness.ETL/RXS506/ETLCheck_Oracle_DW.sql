SELECT end_time
FROM TREXONE_ODS_DATA.ETL_Run
WHERE status = 'C'
  AND start_time BETWEEN (:RunDate - 1 + INTERVAL '23:00:00' HOUR TO SECOND)
                     AND (:RunDate + INTERVAL '15:00:00' HOUR TO SECOND)
  AND end_time   BETWEEN :RunDate 
                     AND (:RunDate + INTERVAL '15:00:00' HOUR TO SECOND)
  AND rownum = 1
ORDER BY start_time Desc