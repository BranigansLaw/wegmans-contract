SELECT PATIENT.PATIENT_NUM as "patient_num",
       pcs.preferred_comm_method as "preferred_comm_method",
       CASE PCS.PHONE_CALL
           WHEN NULL THEN NULL
           WHEN 'Unknown' THEN NULL
           WHEN '0' THEN NULL
           ELSE SUBSTRING(PCS.PHONE_CALL,2,3)
       END as "phone_call",
       LOWER(SUBSTRING(pcs.EMAIL,CHARINDEX('@',PCS.EMAIL)+1,100)) as "email_domain",
       CASE PCS.TEXT_MESSAGE
           WHEN NULL THEN NULL
           WHEN 'Unknown' THEN NULL
           WHEN '0' THEN NULL
           ELSE SUBSTRING(PCS.TEXT_MESSAGE,2,3)
       END as "text_message"
FROM [cps-ERX-WEGP01-ERX].[CPS_PCS_PATIENT_COMM_SETTING] pcs
LEFT OUTER JOIN [cps-ERX-WEGP01-ERX].PATIENT PATIENT
ON patient.PATIENT_KEY = pcs.PATIENT_KEY