SELECT convofact.CPS_MESSAGE_KEY "message_key",
        convofact.CONVERSATION_TEXT "convo_text",
        cast(convofact.SENT_DATE AS date) "sent_date",
        convofact.SENDER "sender",
        convofact.CPS_CONVERSATION_NUM "convo_num",
        cast(convofact.DUE_DATE AS date) "due_date",
        convofact.FOLLOW_UP_RESPONSE "followup_resp"

FROM [cps-ERX-WEGP01-ERX].[CPS_CONVERSATION_FACT] convofact

WHERE convert(date,convofact.SENT_DATE) = convert(date, (GETDATE()-1))

--WHERE convofact.SENT_DATE BETWEEN 
--CAST('2021-04-04 00:00:00' AS datetime2) AND
--CAST('2021-04-10 23:59:59' AS datetime2)
--^ This is to back-populate

ORDER BY convofact.SENT_DATE