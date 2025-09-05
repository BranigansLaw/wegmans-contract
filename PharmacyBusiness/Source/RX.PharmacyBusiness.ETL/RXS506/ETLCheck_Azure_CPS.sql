SELECT etl.Maximum_Load_Date
FROM (
     SELECT
         Min(DateAdd(hh, 
                     DateDiff(hh, 
                              GetDate() at time zone 'US Eastern Standard Time', 
                              GetUTCDate()), 
                     w.LAST_LOAD_DATE)) AS Minimum_Load_Date,
         Max(DateAdd(hh, 
                     DateDiff(hh, 
                              GetDate() at time zone 'US Eastern Standard Time', 
                              GetUTCDate()), 
                     w.LAST_LOAD_DATE)) AS Maximum_Load_Date
     FROM ETL.DW_Last_Load_Watermark w
     ) etl
WHERE etl.Minimum_Load_Date > @RunDate