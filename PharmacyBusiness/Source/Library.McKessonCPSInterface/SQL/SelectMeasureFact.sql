SELECT patient.PATIENT_NUM "patient_num",
        measfact.UPDATED_BY "updated_by_emp",
        convert(date, measfact.CREATE_DATE) "create_date",
        measfact.KEY_NAME "cps_key_name",
        SUBSTRING(CAST(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(measfact.KEY_DESC,CHAR(125),''),CHAR(123),''),CHAR(9),''),CHAR(124),''),CHAR(30),''),CHAR(13),''),CHAR(10),''),'$',''),'*',''),',',''),'#',''),'&',''),'?',''),'"',''),'+',''),'~',''),'‘',''),'!',''),'%','percent'),'=','equals') AS varchar(500)),1,1000) "ques_desc",
        SUBSTRING(CAST(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(measfact.KEY_VALUE,CHAR(125),''),CHAR(123),''),CHAR(9),''),CHAR(124),''),CHAR(30),''),CHAR(13),''),CHAR(10),''),'$',''),'*',''),',',''),'#',''),'&',''),'?',''),'"',''),'+',''),'~',''),'‘',''),'!',''),'%','percent'),'=','equals') AS varchar(500)),1,1000) Collate SQL_Latin1_General_CP1253_CI_AI "ans_value",
        cpsprog.CPS_PROGRAM_NAME "program_name",
        cpsprog.CPS_PROGRAM_TAG "program_tag",
        cpsprog.CPS_PROGRAM_TYPE "program_type",
        cpsprog.CPS_ENROLLMENT "enroll_type",
        measfact.CPS_KEY_VALUE_NUM "key_value_num",
        measfact.PATIENT_QUESTIONNAIRE_NUM "ques_num"
        
FROM CPS_MEASURE_FACT measfact

LEFT OUTER JOIN CPS_PROGRAM cpsprog
ON measfact.CPS_PROGRAM_KEY = cpsprog.CPS_PROGRAM_KEY

LEFT OUTER JOIN PATIENT patient
ON measfact.patient_key = patient.patient_key

WHERE convert(date, MEASFACT.CREATE_DATE) = DATEADD(DD, -1, @RunDate)

ORDER BY measfact.CREATE_DATE