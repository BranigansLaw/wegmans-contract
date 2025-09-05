SELECT
     iaf.DATE_KEY
    ,fac.FD_FACILITY_ID as "store_num"
    ,d.DRUG_LABEL_NAME
    ,d.DRUG_NDC
    ,iaf.ADJUSTMENT_QUANTITY
    ,iaf.ADJUSTMENT_UNIT_COST
    ,iaf.ADJUSTMENT_EXTENDED_COST
    ,iaf.ADJUSTMENT_TYPE_CODE
    ,iaf.INVENTORY_ADJUSTMENT_NUM
    ,r.CODE
    ,r.DESCRIPTION
    ,su.sys_user_fname
    ,su.sys_user_lname
    ,TO_NUMBER(su.SYS_USER_KEY) as "sys_user_key"
    ,TRIM(to_char(d.DRUG_NDC,'00000g0000g00','nls_numeric_characters=.-')) as "ndc"
    ,iaf.REFERENCE_NUMBER as "ref_num"
FROM TREXONE_DW_DATA.INV_Adjustment_Fact iaf
     LEFT OUTER JOIN TREXONE_DW_DATA.Facility fac
         ON fac.FD_FACILITY_KEY = iaf.FD_FACILITY_KEY
     LEFT OUTER JOIN TREXONE_DW_DATA.Sys_User su
         ON su.SYS_USER_KEY = iaf.SYS_USER_KEY
     LEFT OUTER JOIN TREXONE_DW_DATA.Drug d
         ON d.DRUG_KEY = iaf.DRUG_KEY
     LEFT OUTER JOIN TREXONE_DW_DATA.INV_Adjustment_Reason_Type r
         ON r.IAR_KEY = iaf.IAR_KEY
WHERE iaf.PARTITION_DATE BETWEEN (:RunDate - 30)
                            AND (:RunDate + 1)
  AND iaf.DATE_KEY = To_Number(To_Char((:RunDate - 1),'YYYYMMDD'))