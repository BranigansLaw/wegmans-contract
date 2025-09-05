WITH FACT AS (
    SELECT 
        DateAdd(hh, DateDiff(hh, GetDate() at time zone 'US Eastern Standard Time', GetUTCDate()), msr.CREATE_DATE) AS DateEntered,
        Trim(msr.UPDATED_BY) AS EnteredBy,
        msr.PATIENT_KEY,
        CASE 
            WHEN msr.KEY_NAME IN ('BMI')
                THEN 'BodyMassIndex'
            WHEN msr.KEY_NAME IN ('3228f3c9-4467-4015-a4ca-fc20af300280','0ab06d66-e01b-4035-a7b3-d38fc4854e2d') 
                THEN 'Cotinine'
            WHEN msr.KEY_NAME IN ('BLOOD_PRESSURE_DIASTOLIC')
                THEN 'DiastolicBloodPressure'
            WHEN msr.KEY_NAME IN ('255ad81a-d054-4157-b597-d6ed910abada','1c4a90d2-d411-46a1-80b1-ce7f5dd38167') 
                THEN 'EmployerName'
            WHEN msr.KEY_NAME IN ('193f94c3-8b38-4481-a7e3-9c5f68bfc75d', 'ec8b5b7e-9f9e-4945-8a3b-06d9dfd637c5', 'f5315f54-e516-42b2-8772-6fdef6857b69', '3d066b1b-7595-4870-b8fb-fddbb5aa8887') 
                THEN 'Fasting'
            WHEN msr.KEY_NAME IN ('BLOOD_GLUCOSE_A1C')
                THEN 'HemoglobinA1C'
            WHEN msr.KEY_NAME IN ('7071285a-a29d-482d-9ddc-cf89a7e35d5f', 'c6a0fc41-5885-4484-bd26-296c85cb31b5') 
                THEN 'Glucose'
            WHEN msr.KEY_NAME IN ('be435e8f-917a-4d52-a7fc-389cdba69dea', '6cd56c5d-5b60-40a2-999a-409166e28563') 
                THEN 'GroupId'
            WHEN msr.KEY_NAME IN ('CHOLESTEROL_HDL')
                THEN 'HdlCholesterol'
            WHEN msr.KEY_NAME IN ('5774f5bd-9858-43b5-b8e3-b40635b8f5c1','f589e4dd-9a2d-47f6-b9f7-4900d8e361c5') 
                THEN 'Height'
            WHEN msr.KEY_NAME IN ('CHOLESTEROL_LDL')
                THEN 'LdlCholesterol'
            WHEN msr.KEY_NAME IN ('8e2d3b1e-bd73-493f-a5ec-b0c53b3b19c7','0d982970-9c65-4caa-86af-653fd5f118ea') 
                THEN 'PersonCode'
            WHEN msr.KEY_NAME IN ('964f8ec6-6456-468a-b55e-90508a63d3a6', 'b0e780f8-23f5-41f5-91cd-17f2226b24c6','53071e7e-3014-4708-a368-94eae20b93e4','80208bbd-41fb-4280-b3f7-b961aaf0d8b6') 
                THEN 'Pregnant'
            WHEN msr.KEY_NAME IN ('c132c04a-0f4c-4b18-bf37-acd386dc6aba','773a8cb5-bab6-46b5-a331-830e1ff15ef0') 
                THEN 'DateMeasured'
            WHEN msr.KEY_NAME IN ('BLOOD_PRESSURE_SYSTOLIC')
                THEN 'SystolicBloodPressure'
            WHEN msr.KEY_NAME IN ('604f5630-aaf3-43cc-8461-1f6acf05f79d','ae69b7b2-f7ca-4610-96c6-a04d17dce987') 
                THEN 'TcHdlRatio'
            WHEN msr.KEY_NAME IN ('12351684-01a3-48b4-bbb7-3af96867798f','55f47ebf-a716-4948-bb0c-dc5842b1f758') 
                THEN 'ThirdPartyIdNumber'
            WHEN msr.KEY_NAME IN ('TOTAL_CHOLESTEROL')
                THEN 'TotalCholesterol'
            WHEN msr.KEY_NAME IN ('TRIGLYCERIDES')
                THEN 'Triglycerides'
            WHEN msr.KEY_NAME IN ('8d2412c3-8b5c-4382-85d0-cf4a762b969d','b8864e15-3ddf-4a6f-a3ea-2009d9d1d87b') 
                THEN 'WaistCircumference'
            WHEN msr.KEY_NAME IN ('75f2a070-003d-46b8-a5c4-a1718e0bb7e0','7e83d88b-4ae7-4713-9c76-8bfb488519ea') 
                THEN 'Weight'
            WHEN msr.KEY_NAME IN (
                '62d3766b-532c-42cd-bd6f-814ff14135bf','9b32ceae-433e-4a40-a277-7cf33a8561d1','a4fe0ed8-ceec-48a8-ac92-92a9d90ba99e','2c29ab2c-4fde-4e55-9b56-1388fccca611'
               ,'cbe0cee8-1f9b-43fa-862a-e796294ed4fe', 'd836b7fe-8612-4dd2-9bf8-38525f167c29','de29ff7f-47b1-4f49-bfdd-f1e7f5b9cae9','6dc1ac85-5d6a-4826-827e-06c6f388f748') 
                THEN 'Vendor'
            END AS KEY_NAME, 
        CASE 
            WHEN msr.KEY_NAME IN ('a4fe0ed8-ceec-48a8-ac92-92a9d90ba99e','de29ff7f-47b1-4f49-bfdd-f1e7f5b9cae9') THEN 'Health Now/Blue Cross of WNY' 
            WHEN msr.KEY_NAME IN ('2c29ab2c-4fde-4e55-9b56-1388fccca611','6dc1ac85-5d6a-4826-827e-06c6f388f748') THEN 'Independent Health/NOVA'
            WHEN msr.KEY_NAME IN ('9b32ceae-433e-4a40-a277-7cf33a8561d1','d836b7fe-8612-4dd2-9bf8-38525f167c29') THEN 'Excellus/Univera'
            WHEN msr.KEY_NAME IN ('62d3766b-532c-42cd-bd6f-814ff14135bf','cbe0cee8-1f9b-43fa-862a-e796294ed4fe') THEN 'Employer Sponsored'
            ELSE msr.KEY_VALUE
        END AS KEY_VALUE
    FROM CPS_Measure_Fact msr 
         INNER JOIN CPS_Program prg
            ON prg.CPS_PROGRAM_KEY = msr.CPS_PROGRAM_KEY
           AND prg.CPS_PROGRAM_NAME = 'Wegmans Business Partnership Health Screening'
  WHERE DateAdd(hh, DateDiff(hh, GetDate() at time zone 'US Eastern Standard Time', GetUTCDate()), msr.CREATE_DATE) 
        BETWEEN DATEADD(DD, -1, @RunDate) AND @RunDate
)
SELECT 
    FORMAT(f.DateEntered,'yyyy-MM-dd') AS "Date Entered",
    p.PATIENT_NUM AS "Patient Num",
    p.FIRST_NAME AS "Patient First Name",
    p.LAST_NAME AS "Patient Last Name",
    FORMAT(p.BIRTH_DATE,'yyyy-MM-dd') AS "Patient Date of Birth",

    c.DateMeasured AS "Screening Date",
    (CASE WHEN Try_Parse(c.DateMeasured as date) IS NOT NULL THEN 'Y'
	      ELSE 'N'
     END) AS "ScreeningDt IsValid",
	 
    c.ThirdPartyIdNumber AS "Third Party Id Number",
	(CASE WHEN Len(c.ThirdPartyIdNumber) = 11 THEN 'Y'
	      ELSE 'N'
	 END) AS "TPId IsValid",

    c.Weight AS "Weight",  
    (CASE WHEN Try_Parse(c.Weight as decimal(12,2)) IS NOT NULL THEN 'Y'
	      ELSE 'N'
     END) AS "Weight IsValid",

    c.BodyMassIndex AS "Body Mass Index",
    (CASE WHEN Try_Parse(c.BodyMassIndex as decimal(12,2)) IS NOT NULL THEN 'Y'
	      ELSE 'N'
     END) AS "BMI IsValid",

    c.TcHdlRatio AS "TC/HDL Ratio",
    (CASE WHEN Try_Parse(c.TcHdlRatio as decimal(12,2)) IS NOT NULL THEN 'Y'
	      ELSE 'N'
     END) AS "TC/HDL IsValid",

    c.HemoglobinA1C AS "HbA1C",
    (CASE WHEN Try_Parse(c.HemoglobinA1C as decimal(12,2)) IS NOT NULL THEN 'Y'
	      ELSE 'N'
     END) AS "HbA1C IsValid",

    c.Cotinine AS "Cotinine ng/ml",
    (CASE WHEN Try_Parse(c.Cotinine as decimal(12,2)) IS NOT NULL THEN 'Y'
	      ELSE 'N'
     END) AS "Cotinine IsValid"

FROM (SELECT *
      FROM (SELECT f.DateEntered, f.PATIENT_KEY, f.KEY_NAME, f.KEY_VALUE 
            FROM FACT f) many_rows
      PIVOT (Min(KEY_VALUE) FOR KEY_NAME IN (
				  BodyMassIndex,
				  Cotinine,
				  DiastolicBloodPressure,
				  EmployerName,
				  Fasting,
				  HemoglobinA1C,
				  Glucose,
				  GroupId,
				  HdlCholesterol,
				  Height,
				  Vendor,
				  LdlCholesterol,
				  PersonCode,
				  Pregnant,
				  DateMeasured,
				  SystolicBloodPressure,
				  TcHdlRatio,
				  ThirdPartyIdNumber,
				  TotalCholesterol,
				  Triglycerides,
				  WaistCircumference,
				  Weight
				) 
            ) many_cols
     ) c
     INNER JOIN (SELECT Distinct PATIENT_KEY, DateEntered, EnteredBy 
                 FROM FACT) f 
        ON f.DateEntered = c.DateEntered 
       AND f.PATIENT_KEY = c.PATIENT_KEY
     INNER JOIN Patient P
        ON p.PATIENT_KEY = f.PATIENT_KEY
WHERE c.Vendor = 'Health Now/Blue Cross of WNY'
  AND (
       Len(c.ThirdPartyIdNumber) != 11
       OR 
       Try_Parse(c.DateMeasured as date) IS NULL
       OR
       Try_Parse(c.Weight as decimal(12,2)) IS NULL
       OR
       Try_Parse(c.BodyMassIndex as decimal(12,2)) IS NULL
       OR
       Try_Parse(c.TcHdlRatio as decimal(12,2)) IS NULL
       OR
       Try_Parse(c.HemoglobinA1C as decimal(12,2)) IS NULL
       OR
       Try_Parse(c.Cotinine as decimal(12,2)) IS NULL
      )
ORDER BY 1, 6, 8