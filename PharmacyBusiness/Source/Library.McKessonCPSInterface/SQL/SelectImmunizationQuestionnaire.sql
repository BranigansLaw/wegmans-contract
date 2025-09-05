SELECT
     r.StoreNbr
    ,r.PatientNum
    ,r.RxNbr
    ,r.RefillNbr
    ,r.KeyDesc
    ,r.KeyValue
    ,r.CreateDate_EST AS CreateDate
FROM (
        SELECT
             f.FACILITY_ID AS StoreNbr
            ,p.PATIENT_NUM AS PatientNum
            ,msg.ELIGIBILITY_RX_NUMBER AS RxNbr
            ,msg.ELIGIBILITY_REFILL_NUM AS RefillNbr
            ,Trim(msr.KEY_DESC) AS KeyDesc
            ,Trim(msr.KEY_VALUE) AS KeyValue
            ,msr.CREATE_DATE AS CreateDate_UTC
            ,dateadd(hh, datediff(hh, getdate() at time zone 'US Eastern Standard Time', getutcdate()), msr.CREATE_DATE) AS CreateDate_EST
            ,RANK () OVER (PARTITION BY 
                                f.FACILITY_ID
                               ,msg.PATIENT_KEY
                               ,msg.ELIGIBILITY_RX_NUMBER
                               ,msg.ELIGIBILITY_REFILL_NUM
                               ,msr.KEY_DESC
                           ORDER BY msr.CREATE_DATE Desc
                          ) AS LastestResponseRank
        FROM CPS_Questionnaire_Fact que
            INNER JOIN CPS_Measure_Fact msr
                ON msr.CPS_PROGRAM_KEY = que.CPS_PROGRAM_KEY
                AND msr.PATIENT_QUESTIONNAIRE_NUM = que.PATIENT_QUESTIONNAIRE_NUM
            INNER JOIN CPS_Message_Fact msg
                ON msg.CPS_PROGRAM_KEY = que.CPS_PROGRAM_KEY
                AND msg.CPS_MESSAGE_NUM = que.CPS_MESSAGE_NUM
            INNER JOIN Facility f
                ON f.FACILITY_KEY = msg.FACILITY_KEY
            INNER JOIN Patient p
                ON p.PATIENT_KEY = msg.PATIENT_KEY
        WHERE que.CPS_PROGRAM_KEY IN (
                SELECT prg.CPS_PROGRAM_KEY 
                FROM CPS_Program prg
                     INNER JOIN CPS_Status s
                        ON s.CPS_STATUS_KEY = prg.CPS_STATUS_KEY
                WHERE Trim(prg.CPS_PROGRAM_NAME) = 'COVID-19 Vaccine'
                  AND s.CPS_STATUS_NAME = 'Active'
              )
		  AND msr.CREATE_DATE BETWEEN DATEADD(DD, -30, @RunDate) AND DATEADD(DD, 2, @RunDate)
		  AND NOT (msr.KEY_DESC IN ('Native American or Alaska Native','Asian','Native Hawaiian or Other Pacific Islander','Black or African American','White','Other','Unknown') AND msr.KEY_VALUE = 'false')
     ) r
WHERE r.LastestResponseRank = 1
ORDER BY 
     r.StoreNbr
    ,r.RxNbr
    ,r.RefillNbr
    ,r.KeyDesc
    ,r.CreateDate_EST