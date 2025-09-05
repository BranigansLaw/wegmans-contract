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
        
FROM [cps-ERX-WEGP01-ERX].[CPS_MESSAGE_FACT] msgfact

LEFT OUTER JOIN [cps-ERX-WEGP01-ERX].CPS_PROGRAM cpsprog
ON msgfact.CPS_PROGRAM_KEY = cpsprog.CPS_PROGRAM_KEY

LEFT OUTER JOIN [cps-ERX-WEGP01-ERX].CPS_STATUS cpsstatus
ON msgfact.CPS_STATUS_KEY = cpsstatus.CPS_STATUS_KEY

LEFT OUTER JOIN [cps-ERX-WEGP01-ERX].CPS_MESSAGE_TYPE cpsmsgtype
ON msgfact.CPS_MESSAGE_TYPE_KEY = cpsmsgtype.CPS_MESSAGE_TYPE_KEY

LEFT OUTER JOIN [cps-ERX-WEGP01-ERX].CPS_MESSAGE_PRIORITY cpsmsgpri
ON msgfact.CPS_MESSAGE_PRIORITY_KEY = cpsmsgpri.CPS_MESSAGE_PRIORITY_KEY

LEFT OUTER JOIN [cps-ERX-WEGP01-ERX].PATIENT patient
ON msgfact.patient_key = patient.patient_key

LEFT OUTER JOIN FACILITY fac
ON msgfact.FACILITY_KEY = fac.FACILITY_KEY

LEFT OUTER JOIN FACILITY resolvefac
ON MSGFACT.RESOLVED_FACILITY_KEY = resolvefac.FACILITY_KEY

WHERE convert(date,msgfact.UPDATE_DATE) = convert(date, (GETDATE()-1))

--WHERE msgfact.UPDATE_DATE BETWEEN
--CAST('2021-04-04 00:00:00' AS datetime2) AND
--CAST('2021-04-10 23:59:59' AS datetime2)
--^ This is for AdHoc analysis to pick date range

--WHERE msgfact.RECEIVED_DATE BETWEEN 
--CAST('2020-11-10 00:00:00' AS datetime2) AND
--CAST('2020-11-16 23:59:59' AS datetime2)
--^ This is to back-populate

ORDER BY msgfact.UPDATE_DATE