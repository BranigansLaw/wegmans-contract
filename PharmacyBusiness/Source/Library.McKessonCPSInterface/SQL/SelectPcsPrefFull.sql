SELECT PATIENT.PATIENT_NUM as "patient_num",
       CASE PCS.PHONE_CALL
           WHEN NULL THEN NULL
           WHEN 'Unknown' THEN NULL
           WHEN '0' THEN NULL
           ELSE PCS.PHONE_CALL
       END as "phone_call",
       LOWER(pcs.EMAIL) as "email_domain",
       CASE PCS.TEXT_MESSAGE
           WHEN NULL THEN NULL
           WHEN 'Unknown' THEN NULL
           WHEN '0' THEN NULL
           ELSE PCS.TEXT_MESSAGE
       END as "text_message"
FROM CPS_PCS_PATIENT_COMM_SETTING pcs
LEFT OUTER JOIN PATIENT PATIENT
ON patient.PATIENT_KEY = pcs.PATIENT_KEY