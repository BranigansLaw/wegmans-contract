SELECT patient.PATIENT_NUM "patient_num",
        fac.FACILITY_ID "store_num",
        resolvefac.FACILITY_ID "store_num_resolve",
        convert(date, msgfact.RECEIVED_DATE) "receive_date",
        convert(date, MSGFACT.UPDATE_DATE) "update_date",
        SUBSTRING(CAST(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(MSGFACT.MESSAGE_TEXT,'<a href=''https:\\secure.outcomesmtm.com/index.cfm?event=login'' target=''_blank''>OutcomesMTM Website</a>',''),'<strong>',''),'<br>',''),'<b>',''),'<p>','') AS varchar(500)),1,1000) "message_text",
        cpsstatus.CPS_STATUS_NAME "message_status",
        cpsmsgtype.TYPE_NAME "message_type",
        cpsmsgpri.PRIORITY_NAME "message_pri",
        MSGFACT.CPS_MESSAGE_KEY "message_key",
        msgfact.CPS_MESSAGE_NUM "message_num",
        cpsprog.CPS_PROGRAM_NAME "program_name",
        cpsprog.CPS_PROGRAM_TAG "program_tag",
        cpsprog.CPS_PROGRAM_TYPE "program_type",
        cpsprog.CPS_ENROLLMENT "enroll_type",
        msgfact.DUE_DATE "due_date",
        MSGFACT.EXPIRATION_DATE "expire_date",
        msgfact.UPDATED_BY "updated_by_emp",
        msgfact.ELIGIBILITY_RX_NUMBER "rx_num",
        MSGFACT.ELIGIBILITY_REFILL_NUM "refill_num",
	MSGFACT.ASSIGNEE "msg_assignee"
        
FROM CPS_MESSAGE_FACT msgfact

LEFT OUTER JOIN CPS_PROGRAM cpsprog
ON msgfact.CPS_PROGRAM_KEY = cpsprog.CPS_PROGRAM_KEY

LEFT OUTER JOIN CPS_STATUS cpsstatus
ON msgfact.CPS_STATUS_KEY = cpsstatus.CPS_STATUS_KEY

LEFT OUTER JOIN CPS_MESSAGE_TYPE cpsmsgtype
ON msgfact.CPS_MESSAGE_TYPE_KEY = cpsmsgtype.CPS_MESSAGE_TYPE_KEY

LEFT OUTER JOIN CPS_MESSAGE_PRIORITY cpsmsgpri
ON msgfact.CPS_MESSAGE_PRIORITY_KEY = cpsmsgpri.CPS_MESSAGE_PRIORITY_KEY

LEFT OUTER JOIN PATIENT patient
ON msgfact.patient_key = patient.patient_key

LEFT OUTER JOIN FACILITY fac
ON msgfact.FACILITY_KEY = fac.FACILITY_KEY

LEFT OUTER JOIN FACILITY resolvefac
ON MSGFACT.RESOLVED_FACILITY_KEY = resolvefac.FACILITY_KEY

WHERE convert(date,msgfact.UPDATE_DATE) = DATEADD(DD, -1, @RunDate)

ORDER BY msgfact.UPDATE_DATE