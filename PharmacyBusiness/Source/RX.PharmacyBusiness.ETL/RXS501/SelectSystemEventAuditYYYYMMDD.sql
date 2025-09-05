
SELECT 
 To_Number(To_Char(:RunDate,'YYYYMMDD')) AS RunDate
,DATE_KEY                    --NUMBER(18)     
,FD_FACILITY_KEY             --NUMBER(18)     
,WFSD_KEY                    --NUMBER(18)     
,SYS_USER_KEY                --NUMBER(18)     
,SAE_RX_RECORD_NUM           --NUMBER(18)     
,Trim(Substr(WEGMANS.Clean_String_ForTenUp(SAE_RX_FILL_SEQ),1,20)) AS SAE_RX_FILL_SEQ             --VARCHAR2(20)   
,Trim(Substr(WEGMANS.Clean_String_ForTenUp(SAE_COMMENT),1,1000)) AS SAE_COMMENT                 --VARCHAR2(1000) 
,Trim(Substr(WEGMANS.Clean_String_ForTenUp(SAE_ADDITIONAL_PROPS),1,4000)) AS SAE_ADDITIONAL_PROPS        --VARCHAR2(4000) 
,Trim(Substr(WEGMANS.Clean_String_ForTenUp(SAE_AUDIT_NAME),1,500)) AS SAE_AUDIT_NAME              --VARCHAR2(500)  
,Round((SAE_START_TIME - To_Date('20350101','YYYYMMDD')),6) AS SAE_START_TIME              --DATE           
,Round((SAE_END_TIME - To_Date('20350101','YYYYMMDD')),6) AS SAE_END_TIME                --DATE           
,SAE_TP_ITEM_CLAIM_NUM       --NUMBER(18)     
,FD_FOR_FACILITY_KEY         --NUMBER(18)     
,TIME_KEY                    --NUMBER(5)      
,Trim(Substr(WEGMANS.Clean_String_ForTenUp(SAE_RX_NUMBER),1,20)) AS SAE_RX_NUMBER               --VARCHAR2(20)   
,SAE_REFILL_NUM              --NUMBER(18)     
,Trim(Substr(WEGMANS.Clean_String_ForTenUp(ERX_CLIENT_ID),1,100)) AS ERX_CLIENT_ID              --VARCHAR2(100)  
,H_LEVEL                     --NUMBER         
,PATIENT_KEY                 --NUMBER(18)     
FROM TREXONE_DW_DATA.SYSTEM_EVENT_AUDIT sea
WHERE sea.SAE_START_TIME 
      BETWEEN (:RunDate - 1) 
          AND (:RunDate - 1 + INTERVAL '23:59:59' HOUR TO SECOND)
