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
        
FROM [cps-ERX-WEGP01-ERX].[CPS_MEASURE_FACT] measfact

LEFT OUTER JOIN [cps-ERX-WEGP01-ERX].CPS_PROGRAM cpsprog
ON measfact.CPS_PROGRAM_KEY = cpsprog.CPS_PROGRAM_KEY

LEFT OUTER JOIN [cps-ERX-WEGP01-ERX].PATIENT patient
ON measfact.patient_key = patient.patient_key

WHERE convert(date, MEASFACT.CREATE_DATE) = convert(date, (GETDATE()-1))

--WHERE measfact.CREATE_DATE BETWEEN 
--CAST('2021-04-04 00:00:00' AS datetime2) AND
--CAST('2021-04-10 23:59:59' AS datetime2)
--^ This is for AdHoc analysis to pick date range

ORDER BY measfact.CREATE_DATE