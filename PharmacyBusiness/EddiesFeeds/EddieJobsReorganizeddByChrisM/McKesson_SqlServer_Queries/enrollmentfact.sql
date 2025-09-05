SELECT patient.PATIENT_NUM "patient_num",
       fac.FACILITY_ID "store_num", 
       convert(date, enrollfact.STATUS_DATE) "status_update_date",
       cpsstatus.CPS_STATUS_NAME "pt_enroll_status",
       cpsprog.CPS_PROGRAM_NAME "program_name",
       cpsprog.CPS_PROGRAM_TAG "program_tag",
       cpsprog.CPS_PROGRAM_TYPE "program_type",
       cpsprog.CPS_ENROLLMENT "enroll_type",
       cpsprog.CPS_PROGRAM_NUM "program_num",
       enrollfact.CPS_PATIENT_STATUS_NUM "patient_status_num",
       enrollfact.UPDATED_BY "updated_by_emp"
        
FROM [cps-ERX-WEGP01-ERX].[CPS_ENROLLMENT_FACT] enrollfact

LEFT OUTER JOIN [cps-ERX-WEGP01-ERX].CPS_PROGRAM cpsprog
ON enrollfact.CPS_PROGRAM_KEY = cpsprog.CPS_PROGRAM_KEY

LEFT OUTER JOIN [cps-ERX-WEGP01-ERX].CPS_STATUS cpsstatus
ON enrollfact.CPS_STATUS_KEY = cpsstatus.CPS_STATUS_KEY

LEFT OUTER JOIN [cps-ERX-WEGP01-ERX].PATIENT patient
ON enrollfact.patient_key = patient.patient_key

LEFT OUTER JOIN [cps-ERX-WEGP01-ERX].FACILITY fac
ON enrollfact.FACILITY_KEY = fac.FACILITY_KEY

WHERE cast(ENROLLFACT.STATUS_DATE as date) = cast(GETDATE()-1 as date)

-- WHERE enrollfact.STATUS_DATE BETWEEN 
-- CAST('2021-04-04 00:00:00' AS datetime2) AND
-- CAST('2021-04-10 23:59:59' AS datetime2)
--^ This is to back-populate

ORDER BY convert(date, enrollfact.STATUS_DATE), enrollfact.CPS_PATIENT_STATUS_NUM