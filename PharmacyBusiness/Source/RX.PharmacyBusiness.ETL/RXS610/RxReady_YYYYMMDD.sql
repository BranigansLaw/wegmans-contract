SELECT
     To_Number(facility.FD_FACILITY_ID) AS store_num
    ,To_Number(REGEXP_REPLACE(ff.RX_NUMBER,'[^0-9]+','')) AS rx_num
    ,ff.REFILL_NUM AS refill_num
    ,NVL(ff.PARTIAL_FILL_SEQ,0) AS part_seq_num
    ,pt.PD_PATIENT_NUM AS patient_num
    ,(CASE WHEN ff.READY_DATE_KEY = 0 THEN NULL ELSE ff.READY_DATE_KEY END) AS ready_date
    ,ff.READY_TIME_KEY AS ready_time_key
    ,Trunc(ff.TOTAL_REFILLS_REMAINING) AS refills_remain
    ,Substr(WEGMANS.Clean_String_ForTenUp(os.OS_ORDER_SOURCE_DESC),1,45) AS order_source
    ,ff.RX_ORIGIN_CODE || '-' || origin.DESCRIPTION AS rx_origin_code
    ,Substr(WEGMANS.Clean_String_ForTenUp(delmethod.DM_DELIVERY_METHOD_DESC),1,45) AS del_method
    ,ff.FACILITY_ORDER_NUMBER AS order_num
    ,ff.SHIPMENT_NUM AS shipment_num
    ,Substr(WEGMANS.Clean_String_ForTenUp(fillstat.FS_FILL_STATUS_DESC),1,30) AS status_desc
    ,Substr(WEGMANS.Clean_String_ForTenUp(ff.COST_PRICING_FORMULA_NAME),1,30) AS pricing_formula_name
    ,Substr(WEGMANS.Clean_String_ForTenUp(ff.SIG),1,140) AS sig_text
    ,(CASE WHEN ff.DISPENSED_DATE_KEY = 0 THEN NULL ELSE ff.DISPENSED_DATE_KEY END) AS dispensed_date
    ,dispdrug.DRUG_NDC AS dispensed_ndc
    ,Substr(WEGMANS.Clean_String_ForTenUp(dispdrug.DRUG_LABEL_NAME),1,35) AS dispensed_drug_name
    ,ff.QTY_DISPENSED AS qty_dispensed
    ,(CASE WHEN ff.WRITTEN_DATE_KEY = 0 THEN 55551231 ELSE ff.WRITTEN_DATE_KEY END) AS written_date
    ,writdrug.DRUG_NDC AS written_ndc
    ,Substr(WEGMANS.Clean_String_ForTenUp(writdrug.DRUG_LABEL_NAME),1,35) AS written_drug_name
    ,ff.WRITTEN_QTY AS written_qty
    ,cfdrug.DRUG_NDC AS before_map_ndc
    ,Substr(WEGMANS.Clean_String_ForTenUp(cfdrug.DRUG_LABEL_NAME),1,35) AS before_map_drug_name
    ,cffacility.FD_FACILITY_ID AS cf_store
    ,(CASE WHEN cffacility.FD_FACILITY_ID IS NULL THEN 'N' ELSE 'Y' END) AS cf_store_indicator
    ,ff.SKIP_PRE_VER_CODE AS bypass_pv_code
    ,ff.TRANSACTION_CODE AS tx_number
    ,ff.ADJUDICATION_GROUP_NUM
    ,(CASE WHEN ff.ORDER_DATE_KEY = 0 THEN NULL ELSE ff.ORDER_DATE_KEY END) AS order_date
    ,(CASE WHEN ff.SOLD_DATE_KEY = 0 THEN 55551231 ELSE ff.SOLD_DATE_KEY END) AS sold_date
    ,(CASE WHEN ff.UNSOLD_DATE_KEY = 0 THEN 55551231 ELSE ff.UNSOLD_DATE_KEY END) AS unsold_date
    ,(CASE WHEN ff.SYNC_DATE_KEY = 0 THEN 55551231 ELSE ff.SYNC_DATE_KEY END) AS sync_date
    ,(CASE WHEN ff.ADJ_DATE_OF_SERVICE_KEY = 0 THEN 55551231 ELSE ff.ADJ_DATE_OF_SERVICE_KEY END) AS adj_date_of_service
    ,(CASE WHEN ff.ADJUDICATION_DATE_KEY = 0 THEN 55551231 ELSE ff.ADJUDICATION_DATE_KEY END) AS adj_date
    ,(CASE WHEN ff.RTP_DATE_KEY = 0 THEN 55551231 ELSE ff.RTP_DATE_KEY END) AS rtp_date
    ,(CASE WHEN ff.RET_TO_STCK_DT_KEY = 0 THEN 55551231 ELSE ff.RET_TO_STCK_DT_KEY END) AS rts_date
    ,ff.IS_RETURNED_TO_STOCK AS is_rts
    ,ff.RET_TO_STCK_QTY AS rts_qty
    ,ff.FILL_DURATION_SECS AS fill_dur_sec
    ,ff.RX_RECORD_NUM AS rx_record_num
    ,ff.RX_FILL_SEQ AS rx_fill_seq
    ,ff.IS_PRN AS is_prn
    ,ff.IS_MAIL AS is_mail
    ,ff.PRIMARY_TPLD_PLAN_KEY AS tpld_plan_key
    ,ff.VERIFY_RPH_SYS_USER_KEY AS rph_user_key
    ,ff.DATA_ENTRY_USER_KEY AS data_entry_user_key
    ,ff.DISPENSING_USER_KEY AS disp_user_key
    ,ff.ERP_ENROLLMENT_CODE_NUM AS erp_code
    ,To_Number(To_Char(ff.ERP_ENROLLMENT_STATUS_DATE,'YYYYMMDD')) AS erp_code_changed_date
    ,To_Number(To_Char(ff.ERP_TARGET_DATE,'YYYYMMDD')) AS erp_target_date
    ,fillindc.CASH_TP_INDICATOR AS cash_tp
    ,ff.PRIORITY AS high_pri
    ,ff.BIN_NAME AS bin_name
    ,(CASE WHEN ff.PRESCRIBED_DATE_KEY = 0 THEN NULL ELSE ff.PRESCRIBED_DATE_KEY END) AS prescribe_date
    ,(CASE WHEN ff.PROMISE_DATE_KEY = 0 THEN NULL ELSE ff.PROMISE_DATE_KEY END) AS promise_date
    ,daw.DC_DAW_CODE AS daw
    ,ff.DAYS_SUPPLY AS days_supply
    ,ff.BILLED_AMOUNT AS prim_tp_balance
    ,ff.PATIENT_PRICE_PAID AS prim_patient_pay
    ,ff.DISPENSING_FEE_AMOUNT AS prim_fee
    ,ff.TOTAL_PRICE_PAID AS tx_price
    ,ff.DISPENSED_QTY_AWP AS tx_awp
    ,ff.USER_DEFINED_STEP_AMT AS acq_cost
    ,ff.TOTAL_UNC AS u_c_price
    ,ff.TP_PRICE_PAID AS total_tp_balance
    ,(CASE WHEN ff.CANCEL_DATE_KEY = 0 THEN 55551231 ELSE ff.CANCEL_DATE_KEY END) AS cancel_date
    ,(CASE WHEN ff.FIRST_FILL_DATE_KEY = 0 THEN 55551231 ELSE ff.FIRST_FILL_DATE_KEY END) AS first_fill_date
    ,(CASE WHEN ff.LAST_FILL_DATE_KEY = 0 THEN 55551231 ELSE ff.LAST_FILL_DATE_KEY END) AS last_fill_date
    ,(CASE WHEN ff.FUTURE_FILL_DATE_KEY = 0 THEN 55551231 ELSE ff.FUTURE_FILL_DATE_KEY END) AS future_fill_date
    ,ff.PROMISE_TIME_KEY AS promise_time_key
    ,(CASE WHEN ff.EXPIRY_DATE_KEY = 0 THEN 55551231 ELSE ff.EXPIRY_DATE_KEY END) AS expiry_date
    ,Substr(WEGMANS.Clean_String_ForTenUp(ff.COUNSELING_COMMENTS),1,100) AS counsel_comments
    ,Substr(WEGMANS.Clean_String_ForTenUp(ff.COUNSELING_NAME),1,100) AS counsel_rph
    ,ff.COUNSELING_INITIALS AS counsel_rph_initial
    ,ff.INGREDIENT_COST AS ingred_cost_paid
    ,ff.FOA_DATE AS foa_date
    ,ff.DISPENSED_ITEM_LOT_NUMBER AS disp_item_lot_num
    ,ff.ACUTE_PROMISE_REASON_CODE AS acute_promise_reason_code
    ,ff.PRS_PRESCRIBER_KEY AS prescriber_key
    ,ff.PADR_KEY AS padr_key
    ,pt.PD_PRESCRIBER_NUM AS patient_prim_prescribe_num
FROM TREXONE_DW_DATA.FILL_FACT ff
     INNER JOIN TREXONE_DW_DATA.FACILITY facility
        ON facility.FD_FACILITY_KEY = ff.FD_FACILITY_KEY
     LEFT OUTER JOIN TREXONE_DW_DATA.PATIENT pt
        ON pt.PD_PATIENT_KEY = ff.PD_PATIENT_KEY
     LEFT OUTER JOIN TREXONE_DW_DATA.ORDER_SOURCE_TYPE os
        ON os.OS_ORDER_SOURCE_KEY = ff.OS_ORDER_SOURCE_KEY
     LEFT OUTER JOIN TREXONE_DW_DATA.FILL_INDICATOR fillindc
        ON fillindc.FILL_INDICATOR_KEY = ff.FILL_INDICATOR_KEY
     LEFT OUTER JOIN TREXONE_DW_DATA.DELIVERY_METHOD_TYPE delmethod
        ON delmethod.DM_DELIVERY_METHOD_KEY = ff.DM_DELIVERY_METHOD_KEY
     LEFT OUTER JOIN TREXONE_AUD_DATA.RX_ORIGINATION origin
        ON origin.RX_ORIGINATION_NUM = ff.RX_ORIGIN_CODE
     LEFT OUTER JOIN TREXONE_DW_DATA.DRUG dispdrug
        ON dispdrug.DRUG_KEY = ff.DISPENSED_DRUG_KEY
     LEFT OUTER JOIN TREXONE_DW_DATA.DRUG writdrug
        ON writdrug.DRUG_KEY = ff.WRITTEN_DRUG_KEY
     INNER JOIN TREXONE_DW_DATA.DAW_CODE_TYPE daw
        ON daw.DC_DAW_CODE_KEY = ff.DC_DAW_CODE_KEY
     LEFT OUTER JOIN TREXONE_DW_DATA.PRODUCT cfproduct
        ON cfproduct.PRD_PRODUCT_KEY = ff.CF_LOCAL_PRD_PRODUCT_KEY
     LEFT OUTER JOIN TREXONE_DW_DATA.DRUG cfdrug
        ON cfdrug.DRUG_NUM = cfproduct.PRD_DRUG_NUM
     INNER JOIN TREXONE_DW_DATA.FACILITY cffacility
        ON cffacility.FD_FACILITY_KEY = ff.CENTRAL_FILL_FD_FACILITY_KEY
     INNER JOIN TREXONE_DW_DATA.FILL_STATUS_TYPE fillstat
        ON fillstat.FS_FILL_STATUS_KEY = ff.FS_FILL_STATUS_KEY
WHERE  ff.READY_DATE_KEY = To_Number(To_Char((:RunDate - 1),'YYYYMMDD'))