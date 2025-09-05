CREATE OR REPLACE VIEW WEGMANS.PATIENT_ADDRESSES_VIEW (
"padr_key",
"patient_num",
"address_one",
"address_two",
"addr_city",
"addr_state",
"addr_zipcode",
"address_zipext",
"county",
"address_type",
"address_usage",
"addr_status",
"area_code",
"phone_num",
"phone_ext",
"address_updated",
"phone_updated",
"dwaddressnum",
"dwphonenum"
) AS
SELECT PTADDR.PAD_KEY "padr_key",
        DWPATIENT.PD_PATIENT_NUM "patient_num",
        Substr(WEGMANS.Clean_String_ForTenUp(REGEXP_REPLACE(PTADDR.PAD_ADDRESS1,'[,>|]','')),1,100) "address_one",
        Substr(WEGMANS.Clean_String_ForTenUp(REGEXP_REPLACE(PTADDR.PAD_ADDRESS2,'[,>|]','')),1,50) "address_two",
        PTADDR.PAD_CITY "addr_city",
        PTADDR.PAD_STATE "addr_state",
        Substr(WEGMANS.Clean_String_ForTenUp(REGEXP_REPLACE(LPAD(PTADDR.PAD_ZIP,5,'0'),'    ',null)),1,20) "addr_zipcode",
        Substr(WEGMANS.Clean_String_ForTenUp(REGEXP_REPLACE(LPAD(PTADDR.PAD_EXT,4,'0'),'    ',null)),1,6) "address_zipext",
        PTADDR.PAD_COUNTY "county",
        DECODE(ptaddr.pad_address_usage
               ,1,'Home 1'
               ,2,'Home 2'
               ,4,'Work'
               ,8,'Other'
               ,0,NULL
               ,'Unspecified') "address_type",
        DECODE(ptaddr.pad_usage
               ,2,'Shipping'
               ,3,'Default Billing/Shipping'
               ,4,'Display'
               ,5,'Default Billing/Display'
               ,6,'Display/Shipping'
               ,7,'Default Billing/Display/Shipping'
               ,0,NULL
               ,'Other') "address_usage",
       ptaddr.STATUS "addr_status",
       PTPHONE.PATP_AREA_CODE "area_code",
       SUBSTR(PTPHONE.PATP_PHONE_NUMBER,1,3)||'-'||SUBSTR(PTPHONE.PATP_PHONE_NUMBER,4,4) "phone_num",
       PTPHONE.PATP_EXT "phone_ext",
       To_Number(SUBSTR(To_Char(NVL(PTADDR.H_LEVEL,20070101)),1,8)) "address_updated",
       To_Number(SUBSTR(To_Char(NVL(PTPHONE.H_LEVEL,20070101)),1,8)) "phone_updated",
       PTADDR.PAD_ADDRESS_NUM "dwaddressnum",
       PTPHONE.PATP_PHONE_NUM "dwphonenum"
        
FROM TREXONE_DW_DATA.PATIENT_ADDRESS PTADDR

LEFT OUTER JOIN TREXONE_DW_DATA.PATIENT DWPATIENT
ON DWPATIENT.PD_PATIENT_KEY = PTADDR.PD_PATIENT_KEY

LEFT OUTER JOIN TREXONE_DW_DATA.PATIENT_PHONE PTPHONE
ON PTPHONE.PD_PATIENT_KEY = PTADDR.PD_PATIENT_KEY
AND PTPHONE.PAD_KEY = PTADDR.PAD_KEY
AND PTPHONE.EFF_END_DATE = TO_DATE('12/31/2999','MM/DD/YYYY')

WHERE PTADDR.PAD_END_DATE = TO_DATE('12/31/2999','MM/DD/YYYY')

WITH READ ONLY;
/
