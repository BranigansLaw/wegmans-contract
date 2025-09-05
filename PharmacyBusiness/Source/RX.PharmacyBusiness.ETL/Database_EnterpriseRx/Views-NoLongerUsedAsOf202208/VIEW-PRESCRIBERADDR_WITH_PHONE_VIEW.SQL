CREATE OR REPLACE FORCE EDITIONABLE VIEW WEGMANS.PRESCRIBERADDR_WITH_PHONE_VIEW (
     prescriber_key
    ,padr_key
    ,prescrib_addr_num
    ,addr_type
    ,address_one
    ,address_two
    ,addr_city
    ,email
    ,web_addr
    ,state
    ,zipcode
    ,zip_ext
    ,county
    ,is_default
    ,add_source
    ,drug_sched
    ,practice_name
    ,addr_id_num
    ,pref_contact
    ,addr_status
    ,npi
    ,npi_billing
    ,npi_expire_date
    ,state_lic_num
    ,state_lic_billing
    ,state_lic_expire_date
    ,license_state
    ,dea_num
    ,dea_billing
    ,dea_expire_date
    ,area_code_prim
    ,phone_num_prim
    ,ext_prim
    ,area_code_sec
    ,phone_num_sec
    ,ext_sec
    ,area_code_fax
    ,phone_num_fax
    ,ext_fax
    ,area_code_other
    ,phone_num_other
    ,ext_other
    ,phone_type_other2
    ,area_code_other2
    ,phone_num_other2
    ,ext_other2      
    ,last_update
) AS 
SELECT UNIQUE 
    addr.PRS_PRESCRIBER_KEY "prescriber_key",
    addr.PADR_KEY "padr_key",
    addr.PADR_PRESCRIBER_ADDRESS_NUM "prescrib_addr_num",
    DECODE(addr.PADR_ADDRESS_TYPE_CODE,
        1,'HOME',
        2,'HOME/OFFICE',
        3,'HOSPITAL',
        4,'OFFICE',
        5,'OTHER',
        6,'UNKNOWN',NULL) "addr_type",
    addr.PADR_ADDRESS1 "address_one",
    addr.PADR_ADDRESS2 "address_two",
    addr.PADR_CITY "addr_city",
    addr.PADR_EMAIL_ADDRESS "email",
    addr.PADR_WEB_ADDRESS "web_addr",
    addr.PADR_STATE_CODE "state",
    addr.PADR_ZIPCODE "zipcode",
    addr.PADR_EXT "zip_ext",
    addr.PADR_COUNTY "county",
    NVL(addr.PADR_IS_DEFAULT,'N') "is_default",
    DECODE(addr.PADR_SOURCE_CODE,'E','ESV','MANUAL') "add_source",
    addr.PADR_DRUG_SCHEDULE "drug_sched",
    addr.PADR_PRACTICE_NAME "practice_name",
    addr.PADR_PRESCRIBER_ID_NUM "addr_id_num",
    DECODE(addr.PRESCRIBER_ADDR_CONTACT_KEY,1,'Phone',2,'Fax',3,'PPI',4,'Do Not Contact',NULL) "pref_contact",
    addr.PADR_STATUS "addr_status",
    NVL(addrnpi.NPI,npi.NPI) "npi",
    
    CASE addrnpi.NPI WHEN
      NULL THEN NVL(npi.USE_FOR_BILLING,'N')
      ELSE NVL(addrnpi.USE_FOR_BILLING,'N')
    END "npi_billing",
    
    CASE addrnpi.NPI WHEN
      NULL THEN npi.EXPIRE_DATE
      ELSE addrnpi.EXPIRE_DATE
    END "npi_expire_date",
    
    NVL(ststateid.STATELIC,NVL(addrstateid.STATELIC,stateid.STATELIC)) "state_lic_num",
    CASE ststateid.STATELIC WHEN
      NULL THEN
          CASE addrstateid.STATELIC WHEN
             NULL THEN NVL(stateid.USE_FOR_BILLING,'N')
             ELSE NVL(addrstateid.USE_FOR_BILLING,'N')
           END
      ELSE NVL(ststateid.USE_FOR_BILLING,'N')
    END "state_lic_billing",
    
    CASE ststateid.STATELIC WHEN
      NULL THEN
          CASE addrstateid.STATELIC WHEN
             NULL THEN stateid.EXPIRE_DATE
             ELSE addrstateid.EXPIRE_DATE
           END
      ELSE ststateid.EXPIRE_DATE
    END "state_lic_expire_date",
    
    CASE ststateid.STATELIC WHEN
      NULL THEN
          CASE addrstateid.STATELIC WHEN
             NULL THEN stateid.STATE
             ELSE addrstateid.STATE
           END
      ELSE ststateid.STATE
    END "license_state",
    
    dea.DEA_NUM "dea_num",
    NVL(dea.USE_FOR_BILLING,'N') "dea_billing",
    dea.EXPIRE_DATE "dea_expire_date",
    
    primph.AREA_CODE "area_code_prim",
    primph.PHONE_NUM "phone_num_prim",
    primph.EXT "ext_prim",
    
    secnph.AREA_CODE "area_code_sec",
    secnph.PHONE_NUM "phone_num_sec",
    secnph.EXT "ext_sec",
    
    faxph.AREA_CODE "area_code_fax",
    faxph.PHONE_NUM "phone_num_fax",
    faxph.EXT "ext_fax",
    
    otherph.AREA_CODE "area_code_other",
    otherph.PHONE_NUM "phone_num_other",
    otherph.EXT "ext_other",
    
    elseph.PHONE_TYPE "phone_type_other2",
    elseph.AREA_CODE "area_code_other2",
    elseph.PHONE_NUM "phone_num_other2",
    elseph.EXT "ext_other2",      
    TO_NUMBER(SUBSTR(TO_CHAR(addr.H_LEVEL),1,8)) "last_update"
       
FROM TREXONE_DW_DATA.PRESCRIBER_ADDRESS addr

--START - Getting All NPI Info for Prescriber Address
--Start - Join to get Address-Specific NPI info
LEFT OUTER JOIN
(SELECT pid_npi.PRS_PRESCRIBER_KEY AS "PRESCRIBER_KEY",
        pid_npi.PADR_KEY AS "PADR_KEY",
        pid_npi.PID_ID_TEXT AS "NPI",
        pid_npi.USE_FOR_BILLING AS "USE_FOR_BILLING",
        pid_npi.PID_EE_DT AS "EXPIRE_DATE"
FROM TREXONE_DW_DATA.PRESCRIBER_IDS pid_npi
WHERE pid_npi.PID_STATUS = 'A'
AND pid_npi.PIT_KEY = 509
ORDER BY pid_npi.PID_EE_DT desc nulls last)addrnpi
ON addr.PRS_PRESCRIBER_KEY = addrnpi.PRESCRIBER_KEY
AND addr.PADR_KEY = addrnpi.PADR_KEY
--End - Join to get Address-Specific NPI info
--Start - Join to get NON-Address-Specific NPI info
LEFT OUTER JOIN
(SELECT pid_npi.PRS_PRESCRIBER_KEY AS "PRESCRIBER_KEY",
        pid_npi.PADR_KEY AS "PADR_KEY",
        pid_npi.PID_ID_TEXT AS "NPI",
        pid_npi.USE_FOR_BILLING AS "USE_FOR_BILLING",
        pid_npi.PID_EE_DT AS "EXPIRE_DATE"
FROM TREXONE_DW_DATA.PRESCRIBER_IDS pid_npi
WHERE pid_npi.PID_STATUS = 'A'
AND pid_npi.PIT_KEY = 509
ORDER BY pid_npi.PID_EE_DT desc nulls last)npi
ON addr.PRS_PRESCRIBER_KEY = npi.PRESCRIBER_KEY
--End - Join to get NON-Address-Specific NPI info
--END - Getting All NPI Info for Prescriber Address

--Start - Getting all State License Info for Prescriber Address
--Start - Join to get State-Specific State License info
LEFT OUTER JOIN
(SELECT pid_stateid.PRS_PRESCRIBER_KEY AS "PRESCRIBER_KEY",
        pid_stateid.PADR_KEY AS "PADR_KEY",
        pid_stateid.PID_ID_TEXT AS "STATELIC",
        pid_stateid.USE_FOR_BILLING AS "USE_FOR_BILLING",
        pid_stateid.PID_EE_DT AS "EXPIRE_DATE",
        pid_stateid.PID_STATE_CODE AS "STATE"

FROM TREXONE_DW_DATA.PRESCRIBER_IDS pid_stateid
WHERE pid_stateid.PID_STATUS = 'A'
AND pid_stateid.PIT_KEY = 511

ORDER BY pid_stateid.PID_EE_DT desc nulls last)ststateid
ON addr.PRS_PRESCRIBER_KEY = ststateid.PRESCRIBER_KEY
AND addr.PADR_STATE_CODE = ststateid.STATE
--End - Join to get State-Specific State License Info
--Start - Join to get Address-Specific State License info
LEFT OUTER JOIN
(SELECT pid_stateid.PRS_PRESCRIBER_KEY AS "PRESCRIBER_KEY",
        pid_stateid.PADR_KEY AS "PADR_KEY",
        pid_stateid.PID_ID_TEXT AS "STATELIC",
        pid_stateid.USE_FOR_BILLING AS "USE_FOR_BILLING",
        pid_stateid.PID_EE_DT AS "EXPIRE_DATE",
        pid_stateid.PID_STATE_CODE AS "STATE"

FROM TREXONE_DW_DATA.PRESCRIBER_IDS pid_stateid
WHERE pid_stateid.PID_STATUS = 'A'
AND pid_stateid.PIT_KEY = 511

ORDER BY pid_stateid.PID_EE_DT desc nulls last)addrstateid
ON addr.PRS_PRESCRIBER_KEY = addrstateid.PRESCRIBER_KEY
AND addr.PADR_KEY = addrstateid.PADR_KEY
--End - Join to get Address-Specific State License Info
--Start - Join to get NON-Address-Specific State License Info
LEFT OUTER JOIN
(SELECT pid_stateid.PRS_PRESCRIBER_KEY AS "PRESCRIBER_KEY",
        pid_stateid.PADR_KEY AS "PADR_KEY",
        pid_stateid.PID_ID_TEXT AS "STATELIC",
        pid_stateid.USE_FOR_BILLING AS "USE_FOR_BILLING",
        pid_stateid.PID_EE_DT AS "EXPIRE_DATE",
        pid_stateid.PID_STATE_CODE AS "STATE"

FROM TREXONE_DW_DATA.PRESCRIBER_IDS pid_stateid
WHERE pid_stateid.PID_STATUS = 'A'
AND pid_stateid.PIT_KEY = 511

ORDER BY pid_stateid.PID_EE_DT desc nulls last)stateid
ON addr.PRS_PRESCRIBER_KEY = stateid.PRESCRIBER_KEY
--End - Join to get NON-Address-Specific State License Info
--End - Getting all State License Info for Prescriber Address

--Start - Join to get DEA info
LEFT OUTER JOIN
(SELECT pid_dea.PRS_PRESCRIBER_KEY AS "PRESCRIBER_KEY",
        pid_dea.PID_ID_TEXT AS "DEA_NUM",
        pid_dea.USE_FOR_BILLING AS "USE_FOR_BILLING",
        pid_dea.PID_EE_DT AS "EXPIRE_DATE"

FROM TREXONE_DW_DATA.PRESCRIBER_IDS pid_dea
WHERE pid_dea.PID_STATUS = 'A'
AND pid_dea.PIT_KEY = 515

ORDER BY pid_dea.PID_EE_DT desc nulls last)dea
ON addr.PRS_PRESCRIBER_KEY = dea.PRESCRIBER_KEY
--End - Join to get DEA info

--START--ALL Phone Info
--Start--PRIM numbers
LEFT OUTER JOIN(SELECT phone.PRS_PRESCRIBER_KEY AS "PRESCRIBER_KEY",
       phone.PADR_KEY AS "PADR_KEY",
       phone.PRSP_AREA_CODE AS "AREA_CODE",
       SUBSTR(phone.PRSP_PHONE_NUMBER,1,3)||'-'||SUBSTR(phone.PRSP_PHONE_NUMBER,4,4) AS "PHONE_NUM",
       phone.PRSP_EXT AS "EXT"
FROM TREXONE_DW_DATA.PRESCRIBER_PHONE phone
WHERE phone.PRSP_PHONE_ADDRESS_TYPE = 'PRIM'
AND phone.PRSP_AREA_CODE <> '555'
AND phone.PRSP_AREA_CODE <> '000'
AND phone.PRSP_PHONE_NUMBER IS NOT NULL
AND phone.PRSP_STATUS = 'A')primph
ON primph.PRESCRIBER_KEY = addr.PRS_PRESCRIBER_KEY
AND primph.PADR_KEY = addr.PADR_KEY
--End--PRIM numbers

--Start--SECN numbers
LEFT OUTER JOIN(SELECT phone.PRS_PRESCRIBER_KEY AS "PRESCRIBER_KEY",
       phone.PADR_KEY AS "PADR_KEY",
       phone.PRSP_AREA_CODE AS "AREA_CODE",
       SUBSTR(phone.PRSP_PHONE_NUMBER,1,3)||'-'||SUBSTR(phone.PRSP_PHONE_NUMBER,4,4) AS "PHONE_NUM",
       phone.PRSP_EXT AS "EXT"
FROM TREXONE_DW_DATA.PRESCRIBER_PHONE phone
WHERE phone.PRSP_PHONE_ADDRESS_TYPE = 'SECN'
AND phone.PRSP_AREA_CODE <> '555'
AND phone.PRSP_AREA_CODE <> '000'
AND phone.PRSP_PHONE_NUMBER IS NOT NULL
AND phone.PRSP_STATUS = 'A')secnph
ON secnph.PRESCRIBER_KEY = addr.PRS_PRESCRIBER_KEY
AND secnph.PADR_KEY = addr.PADR_KEY
--End--SECN numbers

--Start--FAX numbers
LEFT OUTER JOIN(SELECT phone.PRS_PRESCRIBER_KEY AS "PRESCRIBER_KEY",
       phone.PADR_KEY AS "PADR_KEY",
       phone.PRSP_AREA_CODE AS "AREA_CODE",
       SUBSTR(phone.PRSP_PHONE_NUMBER,1,3)||'-'||SUBSTR(phone.PRSP_PHONE_NUMBER,4,4) AS "PHONE_NUM",
       phone.PRSP_EXT AS "EXT"
FROM TREXONE_DW_DATA.PRESCRIBER_PHONE phone
WHERE phone.PRSP_PHONE_ADDRESS_TYPE = 'FAX'
AND phone.PRSP_AREA_CODE <> '555'
AND phone.PRSP_AREA_CODE <> '000'
AND phone.PRSP_PHONE_NUMBER IS NOT NULL
AND phone.PRSP_STATUS = 'A')faxph
ON faxph.PRESCRIBER_KEY = addr.PRS_PRESCRIBER_KEY
AND faxph.PADR_KEY = addr.PADR_KEY
--End--FAX numbers

--Start--Other numbers
LEFT OUTER JOIN(SELECT phone.PRS_PRESCRIBER_KEY AS "PRESCRIBER_KEY",
       phone.PADR_KEY AS "PADR_KEY",
       phone.PRSP_AREA_CODE AS "AREA_CODE",
       SUBSTR(phone.PRSP_PHONE_NUMBER,1,3)||'-'||SUBSTR(phone.PRSP_PHONE_NUMBER,4,4) AS "PHONE_NUM",
       phone.PRSP_EXT AS "EXT"
FROM TREXONE_DW_DATA.PRESCRIBER_PHONE phone
WHERE phone.PRSP_PHONE_ADDRESS_TYPE = 'OTHE'
AND phone.PRSP_AREA_CODE <> '555'
AND phone.PRSP_AREA_CODE <> '000'
AND phone.PRSP_PHONE_NUMBER IS NOT NULL
AND phone.PRSP_STATUS = 'A')otherph
ON otherph.PRESCRIBER_KEY = addr.PRS_PRESCRIBER_KEY
AND otherph.PADR_KEY = addr.PADR_KEY
--End--Other numbers

--Start--Other2Defaults numbers
LEFT OUTER JOIN(SELECT phone.PRS_PRESCRIBER_KEY AS "PRESCRIBER_KEY",
       phone.PADR_KEY AS "PADR_KEY",
       phone.PRSP_AREA_CODE AS "AREA_CODE",
       SUBSTR(phone.PRSP_PHONE_NUMBER,1,3)||'-'||SUBSTR(phone.PRSP_PHONE_NUMBER,4,4) AS "PHONE_NUM",
       phone.PRSP_EXT AS "EXT",
       DECODE(phone.PRSP_PHONE_TYPE,'PHON','PHONE',NULL,'PHONE',phone.PRSP_PHONE_TYPE) AS "PHONE_TYPE"
FROM TREXONE_DW_DATA.PRESCRIBER_PHONE phone
WHERE phone.PRSP_PHONE_ADDRESS_TYPE is null
AND phone.PRSP_AREA_CODE <> '555'
AND phone.PRSP_AREA_CODE <> '000'
AND phone.PRSP_PHONE_NUMBER IS NOT NULL
AND phone.PRSP_STATUS = 'A')elseph
ON elseph.PRESCRIBER_KEY = addr.PRS_PRESCRIBER_KEY
AND elseph.PADR_KEY = addr.PADR_KEY
--End--Other2Defaults numbers
--END--ALL Phone Info

WITH READ ONLY;
/ 

