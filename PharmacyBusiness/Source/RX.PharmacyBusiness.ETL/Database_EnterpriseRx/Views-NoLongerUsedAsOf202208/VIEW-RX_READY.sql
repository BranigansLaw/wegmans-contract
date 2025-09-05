/*
NOTES on calling this view

SELECT * FROM RX_READY WHERE ready_date = (trunc(sysdate)-1)

*/


CREATE OR REPLACE FORCE EDITIONABLE VIEW WEGMANS.RX_READY (
    store_num,--1
    rx_num,--2
    refill_num,--3
    part_seq_num,--4
    patient_num,--5
    ready_date,--6
    ready_time_key,--7
    refills_remain,--8
    order_source,--9
    rx_origin_code,--10
    del_method,--11
    order_num,---12
    shipment_num,---13
    status_desc,--14
    pricing_formula_name,---15
    sig_text,---16
    dispensed_date,---17
    dispensed_ndc,--18
    dispensed_drug_name,--19
    qty_dispensed,---20
    written_date,--21
    written_ndc,--22
    written_drug_name,--23
    written_qty,--24
    before_map_ndc,--25
    before_map_drug_name,--26
    cf_store,--27
    cf_store_indicator,--28
    bypass_pv_code,--29
    tx_number,--30
    ADJUDICATION_GROUP_NUM,--31
    order_date,---32
    sold_date,---33
    unsold_date,---34
    sync_date,---35
    adj_date_of_service,---36
    adj_date,---37
    rtp_date,---38
    rts_date,---39
    is_rts,---40
    rts_qty,---41
    fill_dur_sec,---42
    rx_record_num,--43
    rx_fill_seq,--44
    is_prn,--45
    is_mail,--46
    prescriber_key,---47
    tpld_plan_key,---48
    rph_user_key,---49
    data_entry_user_key,---50
    disp_user_key,---51
    erp_code,---52
    erp_code_changed_date,---53
    erp_target_date,---54
    cash_tp,---55
    high_pri,---56
    bin_name,--57
    prescribe_date,--58
    promise_date,--59
    daw,--60
    days_supply,--61
    prim_tp_balance,--62
    prim_patient_pay,--63
    prim_fee,--64
    tx_price,--65
    tx_awp,--66
    acq_cost,--67
    u_c_price,--68
    total_tp_balance,--69
    cancel_date,--70
    first_fill_date,--71
    last_fill_date,--72
    future_fill_date,--73
    promise_time_key,--added 11/28/17, ES
    expiry_date,--added 11/28/17, ES
    counsel_comments,--added 11/28/17, ES
    counsel_rph,--added 11/28/17, ES
    counsel_rph_initial,--added 11/28/17, ES
    ingred_cost_paid,--added 11/28/17, ES
    prescribe_addr_key,--prescriber address key added 11/28/17, ES
    patient_prim_prescribe_num, --added 11/28/17, ES
    foa_date, --added 11/28/17, ES
    disp_item_lot_num, --added 11/28/17, ES
    acute_promise_reason_code --added 11/28/17, RXBST
) AS   
SELECT facility.FD_FACILITY_ID store_num,--1
        fillfact.RX_NUMBER rx_num,--2
        fillfact.REFILL_NUM refill_num,--3
        NVL(fillfact.PARTIAL_FILL_SEQ,0) part_seq_num,--4
        pt.PD_PATIENT_NUM patient_num,--5
        readydate.CAL_DATE ready_date,--6
        fillfact.READY_TIME_KEY ready_time_key,--7
        trunc(fillfact.TOTAL_REFILLS_REMAINING) refills_remain,--8
        os.OS_ORDER_SOURCE_DESC order_source,--9
        fillfact.RX_ORIGIN_CODE||'-'||origin.DESCRIPTION rx_origin_code,--10
        delmethod.DM_DELIVERY_METHOD_DESC del_method,--11
        fillfact.FACILITY_ORDER_NUMBER order_num,---12
        fillfact.SHIPMENT_NUM shipment_num,---13
        fillstat.FS_FILL_STATUS_DESC status_desc,--14
        fillfact.COST_PRICING_FORMULA_NAME pricing_formula_name,---15
        REPLACE(REPLACE(fillfact.SIG,CHR(10),''),CHR(124),'') sig_text,---16
        dispensedate.CAL_DATE dispensed_date,---17
        dispdrug.DRUG_NDC dispensed_ndc,--18
        dispdrug.DRUG_LABEL_NAME dispensed_drug_name,--19
        fillfact.QTY_DISPENSED qty_dispensed,---20
        nullif(trunc(writtendate.CAL_DATE),trunc(to_date('5555-12-31','YYYY-MM-DD'))) written_date,--21
        writdrug.DRUG_NDC written_ndc,--22
        writdrug.DRUG_LABEL_NAME written_drug_name,--23
        fillfact.WRITTEN_QTY written_qty,--24
        cfdrug.DRUG_NDC before_map_ndc,--25
        cfdrug.DRUG_LABEL_NAME before_map_drug_name,--26
        cffacility.FD_FACILITY_ID cf_store,--27
        CASE WHEN cffacility.FD_FACILITY_ID is null THEN 'N' ELSE 'Y' END cf_store_indicator,--28
        fillfact.SKIP_PRE_VER_CODE bypass_pv_code,--29
	fillfact.transaction_code tx_number,--30
        fillfact.ADJUDICATION_GROUP_NUM,--31
        orderdate.CAL_DATE order_date,---32
        nullif(trunc(solddate.CAL_DATE),trunc(to_date('5555-12-31','YYYY-MM-DD'))) sold_date,---33
        nullif(trunc(unsolddate.CAL_DATE),trunc(to_date('5555-12-31','YYYY-MM-DD'))) unsold_date,---34
        nullif(trunc(syncdate.CAL_DATE),trunc(to_date('5555-12-31','YYYY-MM-DD'))) sync_date,---35
        nullif(trunc(adjdos.CAL_DATE),trunc(to_date('5555-12-31','YYYY-MM-DD'))) adj_date_of_service,---36
        nullif(trunc(adjdate.CAL_DATE),trunc(to_date('5555-12-31','YYYY-MM-DD'))) adj_date,---37
        nullif(trunc(rtpdate.CAL_DATE),trunc(to_date('5555-12-31','YYYY-MM-DD'))) rtp_date,---38
        nullif(trunc(rtsdate.CAL_DATE),trunc(to_date('5555-12-31','YYYY-MM-DD'))) rts_date,---39
        fillfact.IS_RETURNED_TO_STOCK is_rts,---40
        fillfact.RET_TO_STCK_QTY rts_qty,---41
        fillfact.FILL_DURATION_SECS fill_dur_sec,---42
        fillfact.RX_RECORD_NUM rx_record_num,--43
        fillfact.RX_FILL_SEQ rx_fill_seq,--44
        fillfact.IS_PRN is_prn,--45
        fillfact.IS_MAIL is_mail,--46
        fillfact.PRS_PRESCRIBER_KEY prescriber_key,---47
        fillfact.PRIMARY_TPLD_PLAN_KEY tpld_plan_key,---48
        fillfact.VERIFY_RPH_SYS_USER_KEY rph_user_key,---49
        fillfact.DATA_ENTRY_USER_KEY data_entry_user_key,---50
        fillfact.DISPENSING_USER_KEY disp_user_key,---51
        fillfact.ERP_ENROLLMENT_CODE_NUM erp_code,---52
        fillfact.ERP_ENROLLMENT_STATUS_DATE erp_code_changed_date,---53
        fillfact.ERP_TARGET_DATE erp_target_date,---54
        fillindc.CASH_TP_INDICATOR cash_tp,---55
        fillfact.PRIORITY high_pri,---56
        fillfact.BIN_NAME bin_name,--57
        prescribedate.CAL_DATE prescribe_date,--58
        promisedate.CAL_DATE promise_date,--59
        daw.DC_DAW_CODE daw,--60
        fillfact.DAYS_SUPPLY days_supply,--61
        fillfact.BILLED_AMOUNT prim_tp_balance,--62
        fillfact.PATIENT_PRICE_PAID prim_patient_pay,--63
        fillfact.DISPENSING_FEE_AMOUNT prim_fee,--64
        fillfact.TOTAL_PRICE_PAID tx_price,--65
        fillfact.DISPENSED_QTY_AWP tx_awp,--66
        fillfact.USER_DEFINED_STEP_AMT acq_cost,--67
        fillfact.TOTAL_UNC u_c_price,--68
        fillfact.TP_PRICE_PAID total_tp_balance,--69
        nullif(trunc(canceldate.CAL_DATE),trunc(to_date('5555-12-31','YYYY-MM-DD'))) cancel_date,--70
        nullif(trunc(firstfilldate.CAL_DATE),trunc(to_date('5555-12-31','YYYY-MM-DD'))) first_fill_date,--71
        nullif(trunc(lastfilldate.CAL_DATE),trunc(to_date('5555-12-31','YYYY-MM-DD'))) last_fill_date,--72
        nullif(trunc(futurefilldate.CAL_DATE),trunc(to_date('5555-12-31','YYYY-MM-DD'))) future_fill_date,--73        
        fillfact.PROMISE_TIME_KEY promise_time_key,--added 11/28/17, ES
        nullif(trunc(expirydate.CAL_DATE),trunc(to_date('5555-12-31','YYYY-MM-DD'))) expiry_date,--added 11/28/17, ES
        fillfact.COUNSELING_COMMENTS counsel_comments,--added 11/28/17, ES
        fillfact.COUNSELING_NAME counsel_rph,--added 11/28/17, ES
        fillfact.COUNSELING_INITIALS counsel_rph_initial,--added 11/28/17, ES
        fillfact.INGREDIENT_COST ingred_cost_paid,--added 11/28/17, ES
        fillfact.PADR_KEY prescribe_addr_key,--prescriber address key added 11/28/17, ES
        pt.PD_PRESCRIBER_NUM patient_prim_prescribe_num, --added 11/28/17, ES
        fillfact.FOA_DATE foa_date, --added 11/28/17, ES
        fillfact.DISPENSED_ITEM_LOT_NUMBER disp_item_lot_num, --added 11/28/17, ES
        fillfact.ACUTE_PROMISE_REASON_CODE acute_promise_reason_code --added 11/28/17, RXBST
FROM TREXONE_DW_DATA.FILL_FACT fillfact
INNER JOIN TREXONE_DW_DATA.DATE_DIM orderdate
        ON fillfact.ORDER_DATE_KEY = orderdate.DATE_KEY
INNER JOIN TREXONE_DW_DATA.TIME_DIM ordertime
        ON fillfact.ORDER_TIME_KEY = ordertime.TIME_KEY
INNER JOIN TREXONE_DW_DATA.DATE_DIM readydate
        ON fillfact.READY_DATE_KEY = readydate.DATE_KEY
INNER JOIN TREXONE_DW_DATA.TIME_DIM readytime
        ON fillfact.READY_TIME_KEY = readytime.TIME_KEY
INNER JOIN TREXONE_DW_DATA.FACILITY facility
        ON fillfact.FD_FACILITY_KEY = facility.FD_FACILITY_KEY
LEFT OUTER JOIN TREXONE_DW_DATA.PATIENT pt
        ON fillfact.PD_PATIENT_KEY = pt.PD_PATIENT_KEY
LEFT OUTER JOIN TREXONE_DW_DATA.ORDER_SOURCE_TYPE os
        ON fillfact.OS_ORDER_SOURCE_KEY = os.OS_ORDER_SOURCE_KEY
LEFT OUTER JOIN TREXONE_DW_DATA.FILL_INDICATOR fillindc
        ON fillfact.FILL_INDICATOR_KEY = fillindc.FILL_INDICATOR_KEY
LEFT OUTER JOIN TREXONE_DW_DATA.DATE_DIM dispensedate
        ON fillfact.DISPENSED_DATE_KEY = dispensedate.DATE_KEY
LEFT OUTER JOIN TREXONE_DW_DATA.DELIVERY_METHOD_TYPE delmethod
        ON fillfact.DM_DELIVERY_METHOD_KEY = delmethod.DM_DELIVERY_METHOD_KEY
LEFT OUTER JOIN TREXONE_AUD_DATA.RX_ORIGINATION origin
        ON fillfact.RX_ORIGIN_CODE = origin.RX_ORIGINATION_NUM
INNER JOIN TREXONE_DW_DATA.DATE_DIM solddate
        ON fillfact.SOLD_DATE_KEY = solddate.DATE_KEY
INNER JOIN TREXONE_DW_DATA.DATE_DIM unsolddate
        ON fillfact.UNSOLD_DATE_KEY = unsolddate.DATE_KEY
INNER JOIN TREXONE_DW_DATA.DATE_DIM syncdate
        ON fillfact.SYNC_DATE_KEY = syncdate.DATE_KEY
INNER JOIN TREXONE_DW_DATA.DATE_DIM rtsdate
        ON fillfact.RET_TO_STCK_DT_KEY = rtsdate.DATE_KEY
INNER JOIN TREXONE_DW_DATA.DATE_DIM adjdos
        ON fillfact.ADJ_DATE_OF_SERVICE_KEY = adjdos.DATE_KEY
INNER JOIN TREXONE_DW_DATA.DATE_DIM adjdate
        ON fillfact.ADJUDICATION_DATE_KEY = adjdate.DATE_KEY
INNER JOIN TREXONE_DW_DATA.DATE_DIM rtpdate
        ON fillfact.RTP_DATE_KEY = rtpdate.DATE_KEY
INNER JOIN TREXONE_DW_DATA.DATE_DIM prescribedate
        ON fillfact.PRESCRIBED_DATE_KEY = prescribedate.DATE_KEY
INNER JOIN TREXONE_DW_DATA.DATE_DIM promisedate
        ON fillfact.PROMISE_DATE_KEY = promisedate.DATE_KEY
LEFT OUTER JOIN TREXONE_DW_DATA.DRUG dispdrug
        ON fillfact.DISPENSED_DRUG_KEY = dispdrug.DRUG_KEY
LEFT OUTER JOIN TREXONE_DW_DATA.DRUG writdrug
        ON fillfact.WRITTEN_DRUG_KEY = writdrug.DRUG_KEY
INNER JOIN TREXONE_DW_DATA.DAW_CODE_TYPE daw
        ON fillfact.DC_DAW_CODE_KEY = daw.DC_DAW_CODE_KEY
INNER JOIN TREXONE_DW_DATA.DATE_DIM canceldate
        ON fillfact.CANCEL_DATE_KEY = canceldate.DATE_KEY
LEFT OUTER JOIN TREXONE_DW_DATA.PRODUCT cfproduct
        ON fillfact.CF_LOCAL_PRD_PRODUCT_KEY = cfproduct.PRD_PRODUCT_KEY
LEFT OUTER JOIN TREXONE_DW_DATA.DRUG cfdrug
        ON cfproduct.PRD_DRUG_NUM = cfdrug.DRUG_NUM
INNER JOIN TREXONE_DW_DATA.DATE_DIM writtendate
        ON fillfact.WRITTEN_DATE_KEY = writtendate.DATE_KEY
INNER JOIN TREXONE_DW_DATA.DATE_DIM firstfilldate
        ON fillfact.FIRST_FILL_DATE_KEY = firstfilldate.DATE_KEY
INNER JOIN TREXONE_DW_DATA.DATE_DIM lastfilldate
        ON fillfact.LAST_FILL_DATE_KEY = lastfilldate.DATE_KEY
INNER JOIN TREXONE_DW_DATA.DATE_DIM rtsdate
        ON fillfact.RET_TO_STCK_DT_KEY = rtsdate.DATE_KEY
INNER JOIN TREXONE_DW_DATA.DATE_DIM futurefilldate
        ON fillfact.FUTURE_FILL_DATE_KEY = futurefilldate.DATE_KEY
INNER JOIN TREXONE_DW_DATA.FACILITY cffacility
        ON fillfact.CENTRAL_FILL_FD_FACILITY_KEY = cffacility.FD_FACILITY_KEY
INNER JOIN TREXONE_DW_DATA.FILL_STATUS_TYPE fillstat
        ON fillfact.FS_FILL_STATUS_KEY = fillstat.FS_FILL_STATUS_KEY
INNER JOIN TREXONE_DW_DATA.DATE_DIM expirydate
        ON fillfact.EXPIRY_DATE_KEY = expirydate.DATE_KEY --added 11/28/17, ES

WITH READ ONLY;
/ 


