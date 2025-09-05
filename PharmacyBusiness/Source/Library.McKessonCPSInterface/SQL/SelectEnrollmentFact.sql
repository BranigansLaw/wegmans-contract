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
        
FROM CPS_ENROLLMENT_FACT enrollfact

LEFT OUTER JOIN CPS_PROGRAM cpsprog
ON enrollfact.CPS_PROGRAM_KEY = cpsprog.CPS_PROGRAM_KEY

LEFT OUTER JOIN CPS_STATUS cpsstatus
ON enrollfact.CPS_STATUS_KEY = cpsstatus.CPS_STATUS_KEY

LEFT OUTER JOIN PATIENT patient
ON enrollfact.patient_key = patient.patient_key

LEFT OUTER JOIN FACILITY fac
ON enrollfact.FACILITY_KEY = fac.FACILITY_KEY

WHERE cast(ENROLLFACT.STATUS_DATE as date) = DATEADD(DD, -1, @RunDate)

ORDER BY convert(date, enrollfact.STATUS_DATE), enrollfact.CPS_PATIENT_STATUS_NUM