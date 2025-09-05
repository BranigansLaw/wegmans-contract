SELECT 
 29991231 AS RunDate
,SYSTEM_USER_NUM          --NUMBER(18)     
,Trim(Substr(WEGMANS.Clean_String_ForTenUp(LOGIN_NAME),1,30)) AS LOGIN_NAME               --VARCHAR2(30)   
,Trim(Substr(WEGMANS.Clean_String_ForTenUp(FIRST_NAME),1,30)) AS FIRST_NAME              --VARCHAR2(30)   
,Trim(Substr(WEGMANS.Clean_String_ForTenUp(MIDDLE_NAME),1,30)) AS MIDDLE_NAME              --VARCHAR2(30)   
,Trim(Substr(WEGMANS.Clean_String_ForTenUp(LAST_NAME),1,30)) AS LAST_NAME                --VARCHAR2(30)   
,Trim(Substr(WEGMANS.Clean_String_ForTenUp(INITIALS),1,3)) AS INITIALS                 --VARCHAR2(3)    
-- commented for obvious reasons as it's a password got pproval from Edie yesterday on 01/05/2023
--,Trim(Substr(WEGMANS.Clean_String_ForTenUp(USER_PASSWORD),1,255)) AS USER_PASSWORD            --VARCHAR2(255)  
,Trim(Substr(WEGMANS.Clean_String_ForTenUp(EMPLOYEE_CODE),1,20)) AS EMPLOYEE_CODE            --VARCHAR2(20)   
,IS_PRIVATE               --CHAR(1)        
,SYS_USER                 --NUMBER(18)     
,Round((DATESTAMP - To_Date('20350101','YYYYMMDD')),6) AS DATESTAMP                --DATE           
,NUM_UPDATES              --NUMBER(9)      
,DEFAULT_DATA_GROUP       --NUMBER(18)     
,H_TYPE                   --VARCHAR2(1)    
,H_LEVEL                  --NUMBER         
,ARCHIVE_LEVEL            --NUMBER(9)      
,Trim(Substr(WEGMANS.Clean_String_ForTenUp(DISPLAY_NAME),1,100)) AS DISPLAY_NAME             --VARCHAR2(100)  
,LOGIN_FACILITY_NUM       --NUMBER(18)     
,Round((EFF_START_DATE - To_Date('20350101','YYYYMMDD')),6) AS EFF_START_DATE           --DATE           
,Round((EFF_END_DATE - To_Date('20350101','YYYYMMDD')),6) AS EFF_END_DATE             --DATE           
,Trim(Substr(WEGMANS.Clean_String_ForTenUp(USER_DN),1,1000)) AS USER_DN                  --VARCHAR2(1000) 
,Trim(Substr(WEGMANS.Clean_String_ForTenUp(ROLE_SET),1,4000)) AS ROLE_SET                 --VARCHAR2(4000) 
,Trim(Substr(WEGMANS.Clean_String_ForTenUp(ERX_CLIENT_ID),1,100)) AS ERX_CLIENT_ID            --VARCHAR2(100)  
FROM TREXONE_AUD_DATA.SYSTEM_USER
WHERE EFF_END_DATE = To_Date('29991231','YYYYMMDD')
OR EFF_END_DATE BETWEEN (:RunDate)
                       AND sysdate
