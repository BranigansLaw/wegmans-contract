SELECT  cast(convofact.CPS_MESSAGE_KEY AS varchar) "message_key",
        -- The DataSet class has issues determining the number type of these fields. 
        -- Casting these number types as varchar, then parsing the resulting string in C# was safer
        convofact.CONVERSATION_TEXT "convo_text",
        cast(convofact.SENT_DATE AS date) "sent_date",
        convofact.SENDER "sender",
        cast(convofact.CPS_CONVERSATION_NUM AS varchar) "convo_num",
        cast(convofact.DUE_DATE AS date) "due_date",
        convofact.FOLLOW_UP_RESPONSE "followup_resp"
FROM CPS_CONVERSATION_FACT convofact
WHERE convert(date,convofact.SENT_DATE) = DATEADD(DD, -1, @RunDate)
ORDER BY convofact.SENT_DATE