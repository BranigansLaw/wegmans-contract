SELECT
     r.StoreNbr
    ,r.ProgramName
    ,r.KeyDesc
    ,r.KeyValue
    ,r.StatusName
    ,r.CreateDate_EST AS CreateDate
FROM (
    SELECT
            f.FACILITY_ID AS StoreNbr
            ,msr.KEY_DESC AS KeyDesc
            ,msr.KEY_VALUE AS KeyValue
            ,msg_status.CPS_STATUS_NAME AS StatusName
            ,msr.CREATE_DATE AS CreateDate_UTC
            ,dateadd(hh, datediff(hh, getdate() at time zone 'US Eastern Standard Time', getutcdate()), msr.CREATE_DATE) AS CreateDate_EST
            ,pg.CPS_PROGRAM_NAME AS ProgramName
            ,RANK () OVER (PARTITION BY 
                                f.FACILITY_ID
                               ,msg.PATIENT_KEY                     
                               ,msr.KEY_DESC
                           ORDER BY msr.CREATE_DATE Desc
                          ) AS LastestResponseRank

        FROM CPS_Questionnaire_Fact que
            INNER JOIN CPS_Measure_Fact msr
                ON msr.CPS_PROGRAM_KEY = que.CPS_PROGRAM_KEY
                AND msr.PATIENT_QUESTIONNAIRE_NUM = que.PATIENT_QUESTIONNAIRE_NUM            
            LEFT OUTER JOIN CPS_Message_Fact msg
                ON msg.CPS_PROGRAM_KEY = que.CPS_PROGRAM_KEY
                AND msg.CPS_MESSAGE_NUM = que.CPS_MESSAGE_NUM
            inner join CPS_STATUS msg_status
                ON msg_status.CPS_STATUS_KEY = msg.CPS_STATUS_KEY
            INNER JOIN Facility f
                ON f.FACILITY_KEY = msg.FACILITY_KEY
            INNER JOIN CPS_Program  pg
                ON pg.CPS_PROGRAM_KEY = que.CPS_PROGRAM_KEY
        WHERE DateAdd(hh, DateDiff(hh, GetDate() at time zone 'US Eastern Standard Time', GetUTCDate()), msr.CREATE_DATE)
              BETWEEN DATEADD(DD, -1, @RunDate) AND @RunDate
    ) r
WHERE r.LastestResponseRank = 1
ORDER BY 
     r.ProgramName
    ,r.StoreNbr
    ,r.KeyDesc
    ,r.KeyValue
    ,r.CreateDate_EST