SELECT 
    storenum.FD_FACILITY_ID AS "store_num",
    drug.DRUG_NDC AS "ndc_wo",
    purchplanitem.OVERRIDE_MIN_QTY AS "min_qty_override",
    purchplanitem.OVERRIDE_MAX_QTY AS "max_qty_override",
    purchplanitem.PURCHASE_PLAN_NAME AS "purchase_plan",
    To_Number(To_Char(updatedate.CAL_DATE,'YYYYMMDD')) AS "last_updated"
FROM TREXONE_DW_DATA.PURCHASING_PLAN_ITEM purchplanitem
     LEFT OUTER JOIN TREXONE_DW_DATA.DATE_DIM updatedate
        ON updatedate.DATE_KEY = purchplanitem.REORDER_PARAM_UPDATE_DATE_KEY
     INNER JOIN TREXONE_DW_DATA.FACILITY storenum
        ON purchplanitem.FD_FACILITY_KEY = storenum.FD_FACILITY_KEY
       AND storenum.FACILITY_STATUS_NUM = 1
     INNER JOIN TREXONE_DW_DATA.DRUG drug
        ON purchplanitem.DRUG_KEY = drug.DRUG_KEY
WHERE purchplanitem.EFFECTIVE_END_DATE = To_Date('12312999','MMDDYYYY')
  AND purchplanitem.OVERRIDE_MIN_QTY > 0
  AND purchplanitem.IS_ACTIVE = 'Y'
ORDER BY 1, 2