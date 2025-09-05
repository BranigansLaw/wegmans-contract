SELECT  patient.PATIENT_NUM "patient_num",
        fac.FACILITY_ID "store_num",
        cast(QUESFACT.RESOLVED_DATE AS date) "resolve_date",
        cast(quesfact.ORIGINAL_DUE_DATE as date) "due_date_orig",
        cast(quesfact.OVERRIDE_DUE_DATE as date) "due_date_edit",
        quesfact.UPDATED_BY "updated_by_emp",
        CPSPROG.CPS_PROGRAM_NAME "program_name",
        msgfact.CPS_MESSAGE_KEY "message_key",
        quesfact.CPS_MESSAGE_NUM "message_num",
        quesfact.CPS_QUESTIONNAIRE_KEY "ques_key",
        quesfact.PATIENT_QUESTIONNAIRE_NUM "ques_num"

FROM CPS_QUESTIONNAIRE_FACT quesfact

LEFT OUTER JOIN CPS_PROGRAM cpsprog
ON quesfact.CPS_PROGRAM_KEY = cpsprog.CPS_PROGRAM_KEY

LEFT OUTER JOIN PATIENT patient
ON patient.PATIENT_KEY = quesfact.PATIENT_KEY

LEFT OUTER JOIN FACILITY fac
ON quesfact.FACILITY_KEY = fac.FACILITY_KEY

LEFT OUTER JOIN CPS_MESSAGE_FACT msgfact
ON quesfact.CPS_MESSAGE_NUM = msgfact.CPS_MESSAGE_NUM

WHERE convert(date,QUESFACT.RESOLVED_DATE) = DATEADD(DD, -1, @RunDate)

ORDER BY quesfact.RESOLVED_DATE



