SELECT
     w.TABLE_NAME AS TableNane
	 /*
    ,Format(DateAdd(hh, 
                   DateDiff(hh, 
                            GetDate() at time zone 'US Eastern Standard Time', 
                            GetUTCDate()), 
                   w.LAST_LOAD_DATE),
           'MM/dd/yyyy HH:mm:ss') AS LAST_LOAD_DATE_EST
    ,Format(DateAdd(hh, 
                   DateDiff(hh, 
                            GetDate() at time zone 'US Eastern Standard Time', 
                            GetUTCDate()), 
                   GetUTCDate()),
           'MM/dd/yyyy HH:mm:ss') AS NOW_DATE_EST
	 */
    ,r.ROW_COUNT_NOW AS RowCount
FROM ETL.DW_Last_Load_Watermark w
    INNER JOIN (
        SELECT 'CPS_CONVERSATION_FACT' AS TABLE_NAME, Count(*) AS ROW_COUNT_NOW
        FROM CPS_CONVERSATION_FACT --WHERE SENT_DATE
        UNION ALL
        SELECT 'CPS_ENROLLMENT_FACT' AS TABLE_NAME, Count(*) AS ROW_COUNT_NOW
        FROM CPS_ENROLLMENT_FACT --WHERE STATUS_DATE
        UNION ALL
        SELECT 'CPS_MEASURE_FACT' AS TABLE_NAME, Count(*) AS ROW_COUNT_NOW
        FROM CPS_MEASURE_FACT --WHERE CREATE_DATE
        UNION ALL
        SELECT 'CPS_MESSAGE_FACT' AS TABLE_NAME, Count(*) AS ROW_COUNT_NOW
        FROM CPS_MESSAGE_FACT --WHERE UPDATE_DATE
        UNION ALL
        SELECT 'CPS_PCS_PATIENT_COMM_SETTING' AS TABLE_NAME, Count(*) AS ROW_COUNT_NOW
        FROM CPS_PCS_PATIENT_COMM_SETTING
        UNION ALL
        SELECT 'CPS_PROGRAM' AS TABLE_NAME, Count(*) AS ROW_COUNT_NOW
        FROM CPS_PROGRAM
        UNION ALL
        SELECT 'CPS_PROGRAM_MESSAGE' AS TABLE_NAME, Count(*) AS ROW_COUNT_NOW
        FROM CPS_PROGRAM_MESSAGE
        UNION ALL
        SELECT 'CPS_QUESTIONNAIRE_FACT' AS TABLE_NAME, Count(*) AS ROW_COUNT_NOW
        FROM CPS_QUESTIONNAIRE_FACT --WHERE STATUS_DATE, H_LEVEL
        UNION ALL
        SELECT 'ETL.PATIENT_QUESTIONNAIRE_REOPEN_HISTORY_STG_V' AS TABLE_NAME, -1 AS ROW_COUNT_NOW
        UNION ALL
        SELECT 'FACILITY' AS TABLE_NAME, Count(*) AS ROW_COUNT_NOW
        FROM FACILITY --WHERE H_LEVEL
        UNION ALL
        SELECT 'PATIENT' AS TABLE_NAME, Count(*) AS ROW_COUNT_NOW
        FROM PATIENT --WHERE LAST_UPDATED, H_LEVEL
    ) r ON r.TABLE_NAME = w.TABLE_NAME
ORDER BY 1