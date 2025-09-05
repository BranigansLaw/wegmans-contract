WITH step_keys AS (
    SELECT
        f.FWFS_KEY,          
        f.FWFS_RX_RECORD_NUM,
        f.FWFS_RX_FILL_SEQ,
        f.FWFS_DT_IN,
        f.FWFS_DT_OUT,
        f.FWFS_RX_NUMBER,
        f.PD_PATIENT_KEY,
        f.OWNING_FACILITY_KEY,
        f.FD_FACILITY_KEY,
        f.FWFS_COMPLETE_FACILITY_KEY,
        f.WFSD_KEY,
        f.OS_ORDER_SOURCE_KEY,
        f.DRUG_KEY,
        f.NEXT_WFSD_KEY,
        f.DM_DELIVERY_METHOD_KEY,
        f.FS_FILL_STATUS_KEY,
        f.CENTRAL_FILL_INDICATOR_KEY,
        f.DR_DECLINE_REASON_KEY,
        f.SYS_USER_KEY,        
        f.PADR_KEY,
        f.PRS_PRESCRIBER_KEY,
        f.PRD_PRODUCT_KEY,
        f.FWFS_DT_QUEUE,
        f.READY_DATE,
        f.FWFS_PICKUP_TIME,
        f.FWFS_IS_BAR,
        f.RX_TRANSACTION_CODE,
        f.FWFS_ORDER_PRIORITY,
        f.IS_OUT_OF_STOCK,
        f.FWFS_QTY_DISPENSED,
        f.WRITTEN_QTY,
        f.FWFS_IS_DECLINED,
        f.FWFS_ORDER_DATE,
        f.FACILITY_ORDER_NUMBER,
        f.PARTIAL_FILL_SEQ,
        f.FWFS_REFILL_NUM
    FROM TREXONE_DW_DATA.FILL_WORKFLOW_FACT f
    WHERE f.FWFS_DT_OUT BETWEEN (:RunDate - 1) 
                            AND (:RunDate - 1 + INTERVAL '23:59:59' HOUR TO SECOND)
), fills AS (
    SELECT 
         max_rx_fill.SKIP_PRE_VER_CODE
        ,max_rx_fill.DAYS_SUPPLY
        ,max_rx_fill.RX_RECORD_NUM
        ,max_rx_fill.RX_FILL_SEQ
        ,max_rx_fill.FWFS_DT_OUT
        
         --NEW FIELDS ARE BELOW HERE
        ,max_rx_fill.RXF_DISPENSE_DATE --DATE
        ,max_rx_fill.RXF_DAYS_SUPPLY --NUMBER(9,3)
        ,max_rx_fill.RXF_QTY_DISPENSED --NUMBER(13,3)
        ,max_rx_fill.RXF_METRIC_QTY_DISPENSED --NUMBER(13,5)
        ,max_rx_fill.RXF_INTENDED_QTY_DISPENSED --NUMBER(13,3)
        ,max_rx_fill.RXF_INTENDED_DAYS_SUPPLY --NUMBER(9,3)
        ,max_rx_fill.RXF_IS_CENTRAL_FILL --CHAR(1)
        ,max_rx_fill.RXF_SELECTED_UPC --VARCHAR2(20)
        ,max_rx_fill.RXF_RTP_DATE --DATE
        ,max_rx_fill.RXF_READY_DATE --DATE
        ,max_rx_fill.RXF_IS_340B --CHAR(1)
        ,max_rx_fill.RXF_IS_CASH --CHAR(1)
        ,max_rx_fill.RXF_SKIP_PRE_VER_CODE --NUMBER(2)
    
        ,max_rx_fill.ADS_ACTUAL_SHIPPING_METHOD --VARCHAR2(100)
        ,max_rx_fill.ADS_ACTUAL_SHIP_CHARGE --NUMBER(12,2)
        ,max_rx_fill.ADS_ACTUAL_SHIP_DATE --DATE
        ,max_rx_fill.ADS_ESTIMATED_DELIVERY_DATE --DATE
        ,max_rx_fill.ADS_TOTE_LICENSE_PLATE --VARCHAR2(50)
        ,max_rx_fill.ADS_MANIFEST_DELIVERY_DATE --VARCHAR2(50)
        ,max_rx_fill.ADS_EXCEPTION_TEXT --VARCHAR2(250)
        ,max_rx_fill.ADS_external_order_number --VARCHAR2(12)
        ,max_rx_fill.ADS_reject_code --VARCHAR2(10)
        ,max_rx_fill.ADS_qty_dispensed --qty_picked --NUMBER(13,3)
        ,max_rx_fill.ADS_number_of_vials --NUMBER(3)
        ,max_rx_fill.ADS_package_size --NUMBER(13,3)
        ,max_rx_fill.ADS_date_completed --DATE
        ,max_rx_fill.ADS_date_filled --DATE
        ,max_rx_fill.ADS_ndc_requested --VARCHAR2(13)
        ,max_rx_fill.ADS_ndc_dispensed --VARCHAR2(13)
        ,max_rx_fill.ADS_qc_initials --VARCHAR2(3)
        ,max_rx_fill.ADS_is_easy_open_cap --CHAR(1)
        ,max_rx_fill.ADS_status_code --VARCHAR2(10)
        ,max_rx_fill.ADS_status_desc --VARCHAR2(36)
        ,max_rx_fill.ADS_date_updated --DATE
        ,max_rx_fill.ADS_datestamp --DATE
        
        ,max_rx_fill.MAN_CHECK_OUT_DATESTAMP --DATE
        ,max_rx_fill.MAN_CHECK_IN_DATESTAMP --DATE
        ,max_rx_fill.MAN_DATESTAMP --DATE
        ,max_rx_fill.MAN_IS_PULLED_BACK --CHAR(1)
		
        ,max_rx_fill.order_num
        ,max_rx_fill.item_seq
    FROM (
        SELECT 
             AUD__Item.order_num
            ,AUD__Item.item_seq
            ,AUD__Rx_Fill.SKIP_PRE_VER_CODE
            ,AUD__Rx_Fill.DAYS_SUPPLY
            ,AUD__Rx_Fill.RX_RECORD_NUM
            ,AUD__Rx_Fill.RX_FILL_SEQ
            ,steps.FWFS_DT_OUT
                         
            --NEW FIELDS ARE BELOW HERE        
            ,Round((AUD__Rx_Fill.DISPENSE_DATE - To_Date('20350101','YYYYMMDD')),6) AS RXF_DISPENSE_DATE --DATE
            ,AUD__Rx_Fill.DAYS_SUPPLY AS RXF_DAYS_SUPPLY --NUMBER(9,3)
            ,AUD__Rx_Fill.QTY_DISPENSED AS RXF_QTY_DISPENSED --NUMBER(13,3)
            ,AUD__Rx_Fill.METRIC_QTY_DISPENSED AS RXF_METRIC_QTY_DISPENSED --NUMBER(13,5)
            ,AUD__Rx_Fill.INTENDED_QTY_DISPENSED AS RXF_INTENDED_QTY_DISPENSED --NUMBER(13,3)
            ,AUD__Rx_Fill.INTENDED_DAYS_SUPPLY AS RXF_INTENDED_DAYS_SUPPLY --NUMBER(9,3)
            ,AUD__Rx_Fill.IS_CENTRAL_FILL AS RXF_IS_CENTRAL_FILL --CHAR(1)
            ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Rx_Fill.SELECTED_UPC),1,20) AS RXF_SELECTED_UPC --VARCHAR2(20)
            ,Round((AUD__Rx_Fill.RELEASED_TO_PATIENT_DATE - To_Date('20350101','YYYYMMDD')),6) AS RXF_RTP_DATE --DATE
            ,Round((AUD__Rx_Fill.READY_DATE - To_Date('20350101','YYYYMMDD')),6) AS RXF_READY_DATE --DATE
            ,AUD__Rx_Fill.IS_340B AS RXF_IS_340B --CHAR(1)
            ,AUD__Rx_Fill.IS_CASH AS RXF_IS_CASH --CHAR(1)
            ,AUD__Rx_Fill.SKIP_PRE_VER_CODE AS RXF_SKIP_PRE_VER_CODE --NUMBER(2)
        
            ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Ads_Response.ACTUAL_SHIPPING_METHOD),1,100) AS ADS_ACTUAL_SHIPPING_METHOD --VARCHAR2(100)
            ,AUD__Ads_Response.ACTUAL_SHIP_CHARGE AS ADS_ACTUAL_SHIP_CHARGE --NUMBER(12,2)
            ,Round((AUD__Ads_Response.ACTUAL_SHIP_DATE - To_Date('20350101','YYYYMMDD')),6) AS ADS_ACTUAL_SHIP_DATE --DATE
            ,Round((AUD__Ads_Response.ESTIMATED_DELIVERY_DATE - To_Date('20350101','YYYYMMDD')),6) AS ADS_ESTIMATED_DELIVERY_DATE --DATE
            ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Ads_Response.TOTE_LICENSE_PLATE),1,50) AS ADS_TOTE_LICENSE_PLATE --VARCHAR2(50)
            ,Round((To_Date(AUD__Ads_Response.MANIFEST_SYS_EST_DELIVERY_DATE,'MM/DD/YYYY HH:MI AM') - To_Date('20350101','YYYYMMDD')),6) AS ADS_MANIFEST_DELIVERY_DATE --VARCHAR2(50)
            ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Ads_Response.EXCEPTION_TEXT),1,250) AS ADS_EXCEPTION_TEXT --VARCHAR2(250)
            ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Ads_Response.external_order_number),1,12) AS ADS_external_order_number --VARCHAR2(12)
            ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Ads_Response.reject_code),1,10) AS ADS_reject_code --VARCHAR2(10)
            ,AUD__Ads_Response.qty_dispensed AS ADS_qty_dispensed --qty_picked --NUMBER(13,3)
            ,AUD__Ads_Response.number_of_vials AS ADS_number_of_vials --NUMBER(3)
            ,AUD__Ads_Response.package_size AS ADS_package_size --NUMBER(13,3)
            ,Round((AUD__Ads_Response.date_completed - To_Date('20350101','YYYYMMDD')),6) AS ADS_date_completed --DATE
            ,Round((AUD__Ads_Response.date_filled - To_Date('20350101','YYYYMMDD')),6) AS ADS_date_filled --DATE
            ,AUD__Ads_Response.ndc_requested AS ADS_ndc_requested --VARCHAR2(13)
            ,AUD__Ads_Response.ndc_dispensed AS ADS_ndc_dispensed --VARCHAR2(13)
            ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Ads_Response.qc_initials),1,3) AS ADS_qc_initials --VARCHAR2(3)
            ,AUD__Ads_Response.is_easy_open_cap AS ADS_is_easy_open_cap --CHAR(1)
            ,AUD__Ads_Response.status AS ADS_status_code --VARCHAR2(10)
            ,(CASE AUD__Ads_Response.status
                   WHEN 'A' THEN 'Rx Accepted - Filling at CF Facility'
                   WHEN 'H' THEN 'Order Shipped'
                   WHEN 'P' THEN 'Complete - Filled at Store'
                   WHEN 'R' THEN 'Rx Rejected by CFD'
                   WHEN 'V' THEN 'Received at Store'
                   WHEN 'C' THEN 'Order Cancelled'
                   WHEN 'E' THEN 'Error in Processing Response'
                   WHEN 'F' THEN 'Refused Cancel'
                   ELSE NULL    
              END) AS ADS_status_desc --VARCHAR2(36)
            ,Round((AUD__Ads_Response.date_updated - To_Date('20350101','YYYYMMDD')),6) AS ADS_date_updated --DATE
            ,Round((AUD__Ads_Response.datestamp - To_Date('20350101','YYYYMMDD')),6) AS ADS_datestamp --DATE
            
            ,Round((AUD__Manifest_Item.CHECK_OUT_DATESTAMP - To_Date('20350101','YYYYMMDD')),6) AS MAN_CHECK_OUT_DATESTAMP --DATE
            ,Round((AUD__Manifest_Item.CHECK_IN_DATESTAMP - To_Date('20350101','YYYYMMDD')),6) AS MAN_CHECK_IN_DATESTAMP --DATE
            ,Round((AUD__Manifest_Item.DATESTAMP - To_Date('20350101','YYYYMMDD')),6) AS MAN_DATESTAMP --DATE
            ,AUD__Manifest_Item.IS_PULLED_BACK AS MAN_IS_PULLED_BACK --CHAR(1)
                         
            ,Rank() OVER (PARTITION BY 
                               AUD__Rx_Fill.RX_RECORD_NUM
                              ,AUD__Rx_Fill.RX_FILL_SEQ
                              ,steps.FWFS_DT_OUT
                              ,AUD__Item.order_num
                              ,AUD__Item.item_seq
                          ORDER BY 
                               AUD__Rx_Fill.h_level Desc
                              --,AUD__Item.eff_start_date Desc
                              --,AUD__Item.eff_end_date Desc
                              ,AUD__Ads_Response.eff_start_date Desc
                              ,AUD__Ads_Response.eff_end_date Desc
                              ,AUD__Manifest_Item.eff_start_date --Desc
                              ,AUD__Manifest_Item.eff_end_date Desc
                              ,rownum Desc
                         ) AS CentralFill_Rank

        FROM TREXONE_AUD_DATA.Rx_Fill AUD__Rx_Fill
             INNER JOIN step_keys steps
                ON AUD__Rx_Fill.rx_record_num = steps.fwfs_rx_record_num
               AND AUD__Rx_Fill.rx_fill_seq = steps.fwfs_rx_fill_seq
               AND steps.fwfs_dt_out BETWEEN CAST(FROM_TZ(CAST(AUD__Rx_Fill.datestamp AS TIMESTAMP), 'UTC') AT TIME ZONE 'US/Eastern' AS DATE) AND AUD__Rx_Fill.eff_end_date
             LEFT OUTER JOIN TREXONE_AUD_DATA.Item AUD__Item
                ON AUD__Item.rx_record_num = steps.fwfs_rx_record_num
               AND AUD__Item.rx_fill_seq = steps.fwfs_rx_fill_seq
               --AND steps.fwfs_dt_out BETWEEN AUD__Item.eff_start_date AND AUD__Item.eff_end_date
               AND steps.fwfs_dt_out BETWEEN CAST(FROM_TZ(CAST(AUD__Item.eff_start_date AS TIMESTAMP), 'UTC') AT TIME ZONE 'US/Eastern' AS DATE)
                                         AND CAST(FROM_TZ(CAST(AUD__Item.eff_end_date AS TIMESTAMP), 'UTC') AT TIME ZONE 'US/Eastern' AS DATE)
             LEFT OUTER JOIN TREXONE_AUD_DATA.Ads_Response AUD__Ads_Response
                ON AUD__Ads_Response.order_num = AUD__Item.order_num
               AND AUD__Ads_Response.item_seq = AUD__Item.item_seq
               AND steps.fwfs_dt_out >= AUD__Ads_Response.date_updated
             LEFT OUTER JOIN TREXONE_AUD_DATA.Manifest_Item AUD__Manifest_Item
                ON AUD__Manifest_Item.order_num = AUD__Item.order_num
               AND AUD__Manifest_Item.item_seq = AUD__Item.item_seq
               AND (steps.fwfs_dt_out = CAST(FROM_TZ(CAST(AUD__Manifest_Item.eff_start_date AS TIMESTAMP), 'UTC') AT TIME ZONE 'US/Eastern' AS DATE)
                    OR 
                    AUD__Manifest_Item.eff_end_date = To_Date('29991231','YYYYMMDD')
                    )
               AND steps.fwfs_dt_out <= CAST(FROM_TZ(CAST(AUD__Manifest_Item.eff_end_date AS TIMESTAMP), 'UTC') AT TIME ZONE 'US/Eastern' AS DATE)
         ) max_rx_fill
    WHERE max_rx_fill.CentralFill_Rank = 1
)
SELECT
	ownstore.FD_FACILITY_ID "store_num",
	wlbstore.FD_FACILITY_ID "store_num_wlb",
	REGEXP_REPLACE(fwf.FWFS_RX_NUMBER, '[^0-9]+', '') "rx_num",
	fwf.FWFS_RX_NUMBER "rx_num_txt",
	fwf.FWFS_REFILL_NUM "refill_num",
	TO_NUMBER(NVL(fwf.PARTIAL_FILL_SEQ,'0')) "part_seq_num",
	patient.PD_PATIENT_NUM "patient_num",
	fwf.FACILITY_ORDER_NUMBER "order_num",
	wfs.WFSD_STEP_DESCRIPTION "wfs_desc",
	To_Number(To_Char(fwf.FWFS_ORDER_DATE,'YYYYMMDD')) "order_date",	
	
	NVL(bpv.SKIP_PRE_VER_CODE,0) "bypasspv",

	To_Number(To_Char(fwf.FWFS_DT_IN,'YYYYMMDD')) "date_entered_wfs",
	Round((fwf.FWFS_DT_IN - To_Date('20350101','YYYYMMDD')),6) "datetime_entered_wfs",
	To_Number(To_Char(fwf.FWFS_DT_OUT,'YYYYMMDD')) "date_exited_wfs",
	Round((fwf.FWFS_DT_OUT - To_Date('20350101','YYYYMMDD')),6) "datetime_exited_wfs",

	ROUND(((fwf.FWFS_DT_OUT - fwf.FWFS_DT_IN) * 86400),0) "step_dur_seconds",  
	
    CASE
        WHEN INSTR(sysuser.SYS_USER_LOGIN_NAME, '@') > 0 THEN
            TRIM(SUBSTR(sysuser.SYS_USER_LOGIN_NAME, 1, INSTR(sysuser.SYS_USER_LOGIN_NAME, '@') - 1))
        ELSE
            TRIM(sysuser.SYS_USER_LOGIN_NAME)
    END AS "emp_login_name",

	WEGMANS.Clean_String_ForTenUp(userrole.DISPLAY_NAME) "emp_displayname",
    	CASE
        	WHEN userrole.ROLE_SET LIKE '%super%' THEN 'Super User'
        	WHEN userrole.ROLE_SET LIKE '%rph%' THEN 'RPh'
        	WHEN userrole.ROLE_SET LIKE '%intern%' THEN 'Intern'
        	WHEN userrole.ROLE_SET LIKE '%tech%' THEN 'Tech'
        	WHEN userrole.ROLE_SET LIKE '%extern%' THEN 'Extern'
        	WHEN userrole.ROLE_SET LIKE '%stl%' THEN 'STL'      
        	ELSE 'Other'
    	END "emp_erx_role",
	REPLACE(userrole.ROLE_SET,',','/') "rightsroles",

	nextwfs.WFSD_STEP_DESCRIPTION "next_wfs_desc",
	fwf.FWFS_IS_DECLINED "is_declined",
	DR.DR_DECLINE_REASON "decline_reason",
  
	drug.DRUG_NDC "ndc_wo",
	WEGMANS.Clean_String_ForTenUp(drug.DRUG_LABEL_NAME) "drug_name",
	fwf.WRITTEN_QTY "qty_written",
	fwf.FWFS_QTY_DISPENSED "qty_dispensed",    
	
	NVL(bpv.DAYS_SUPPLY,0) "days_supply",

	ordersource.OS_ORDER_SOURCE_DESC "order_source",
	delmethod.DM_DELIVERY_METHOD_DESC "del_method",
    fillstat.FS_FILL_STATUS_DESC "status_desc",
	NVL(fwf.IS_OUT_OF_STOCK,'N') "is_outofstock",
	fwf.FWFS_ORDER_PRIORITY "order_priority",
	cfind.IS_CENTRAL_FILL "cf_ind",
	cfind.IS_PULLED_BACK "pulled_back",
	cfind.IS_PACKED "is_cf_packed",
	cfind.IS_CENTRAL_FILL_DECLINED "cf_instock_declined",
	fwf.RX_TRANSACTION_CODE "tx_number",
	NVL(fwf.FWFS_IS_BAR,'N') "is_bar",

	To_Number(To_Char(fwf.FWFS_PICKUP_TIME,'YYYYMMDD')) "pickup_date",
	Round((fwf.FWFS_PICKUP_TIME - To_Date('20350101','YYYYMMDD')),6) "pickup_datetime",
	To_Number(To_Char(fwf.READY_DATE,'YYYYMMDD')) "ready_date",
	Round((fwf.READY_DATE - To_Date('20350101','YYYYMMDD')),6) "ready_datetime",
	To_Number(To_Char(fwf.FWFS_DT_QUEUE,'YYYYMMDD')) "initial_wfs_date",
	Round((fwf.FWFS_DT_QUEUE - To_Date('20350101','YYYYMMDD')),6) "initial_wfs_datetime",
  
	fwf.FWFS_RX_RECORD_NUM "rx_record_num",
	fwf.FWFS_RX_FILL_SEQ "rx_fill_seq",
	fwf.DRUG_KEY "drug_key",
	fwf.PRD_PRODUCT_KEY "product_key",
	fwf.PRS_PRESCRIBER_KEY "prescriber_key",
	fwf.PADR_KEY "padr_key",
	sysuser.SYS_USER_KEY "sys_user_key",
	fwf.FWFS_KEY "fwfs_key"
	
    --NEW FIELDS ARE BELOW HERE
    ,bpv.RXF_DISPENSE_DATE --DATE
    ,bpv.RXF_DAYS_SUPPLY --NUMBER(9,3)
    ,bpv.RXF_QTY_DISPENSED --NUMBER(13,3)
    ,bpv.RXF_METRIC_QTY_DISPENSED --NUMBER(13,5)
    ,bpv.RXF_INTENDED_QTY_DISPENSED --NUMBER(13,3)
    ,bpv.RXF_INTENDED_DAYS_SUPPLY --NUMBER(9,3)
    ,bpv.RXF_IS_CENTRAL_FILL --CHAR(1)
    ,bpv.RXF_SELECTED_UPC --VARCHAR2(20)
    ,bpv.RXF_RTP_DATE --DATE
    ,bpv.RXF_READY_DATE --DATE
    ,bpv.RXF_IS_340B --CHAR(1)
    ,bpv.RXF_IS_CASH --CHAR(1)
    ,bpv.RXF_SKIP_PRE_VER_CODE --NUMBER(2)

    ,bpv.ADS_ACTUAL_SHIPPING_METHOD --VARCHAR2(100)
    ,bpv.ADS_ACTUAL_SHIP_CHARGE --NUMBER(12,2)
    ,bpv.ADS_ACTUAL_SHIP_DATE --DATE
    ,bpv.ADS_ESTIMATED_DELIVERY_DATE --DATE
    ,bpv.ADS_TOTE_LICENSE_PLATE --VARCHAR2(50)
    ,bpv.ADS_MANIFEST_DELIVERY_DATE --VARCHAR2(50)
    ,bpv.ADS_EXCEPTION_TEXT --VARCHAR2(250)
    ,bpv.ADS_external_order_number --VARCHAR2(12)
    ,bpv.ADS_reject_code --VARCHAR2(10)
    ,bpv.ADS_qty_dispensed --qty_picked --NUMBER(13,3)
    ,bpv.ADS_number_of_vials --NUMBER(3)
    ,bpv.ADS_package_size --NUMBER(13,3)
    ,bpv.ADS_date_completed --DATE
    ,bpv.ADS_date_filled --DATE
    ,bpv.ADS_ndc_requested --VARCHAR2(13)
    ,bpv.ADS_ndc_dispensed --VARCHAR2(13)
    ,bpv.ADS_qc_initials --VARCHAR2(3)
    ,bpv.ADS_is_easy_open_cap --CHAR(1)
    ,bpv.ADS_status_code --VARCHAR2(10)
    ,bpv.ADS_status_desc --VARCHAR2(36)
    ,bpv.ADS_date_updated --DATE
    ,bpv.ADS_datestamp --DATE
    
    ,bpv.MAN_CHECK_OUT_DATESTAMP --DATE
    ,bpv.MAN_CHECK_IN_DATESTAMP --DATE
    ,bpv.MAN_DATESTAMP --DATE
    ,bpv.MAN_IS_PULLED_BACK --CHAR(1)
    
    ,bpv.order_num
    ,bpv.item_seq

FROM step_keys fwf
    LEFT OUTER JOIN TREXONE_DW_DATA.PATIENT patient
      ON patient.PD_PATIENT_KEY = fwf.PD_PATIENT_KEY
    LEFT OUTER JOIN TREXONE_DW_DATA.FACILITY ownstore
      ON ownstore.FD_FACILITY_KEY = fwf.OWNING_FACILITY_KEY
    LEFT OUTER JOIN TREXONE_DW_DATA.FACILITY wlbstore
      ON wlbstore.FD_FACILITY_KEY = fwf.FD_FACILITY_KEY
    LEFT OUTER JOIN TREXONE_DW_DATA.FACILITY completestore
      ON completestore.FD_FACILITY_KEY = fwf.FWFS_COMPLETE_FACILITY_KEY
    LEFT OUTER JOIN TREXONE_DW_DATA.WF_STEP wfs
      ON wfs.WFSD_KEY = fwf.WFSD_KEY
    LEFT OUTER JOIN TREXONE_DW_DATA.ORDER_SOURCE_TYPE ordersource
      ON ordersource.OS_ORDER_SOURCE_KEY = fwf.OS_ORDER_SOURCE_KEY
    LEFT OUTER JOIN TREXONE_DW_DATA.DRUG drug
      ON drug.DRUG_KEY = fwf.DRUG_KEY
    LEFT OUTER JOIN TREXONE_DW_DATA.WF_STEP nextwfs
      ON nextwfs.WFSD_KEY = fwf.NEXT_WFSD_KEY
    LEFT OUTER JOIN TREXONE_DW_DATA.DELIVERY_METHOD_TYPE delmethod
      ON delmethod.DM_DELIVERY_METHOD_KEY = fwf.DM_DELIVERY_METHOD_KEY
    LEFT OUTER JOIN TREXONE_DW_DATA.FILL_STATUS_TYPE fillstat
      ON fillstat.FS_FILL_STATUS_KEY = fwf.FS_FILL_STATUS_KEY
    LEFT OUTER JOIN TREXONE_DW_DATA.CENTRAL_FILL_INDICATOR cfind
      ON cfind.CENTRAL_FILL_INDICATOR_KEY = fwf.CENTRAL_FILL_INDICATOR_KEY
    LEFT OUTER JOIN TREXONE_DW_DATA.DECLINE_REASON_TYPE dr
      ON dr.DR_DECLINE_REASON_KEY = fwf.DR_DECLINE_REASON_KEY
    LEFT OUTER JOIN TREXONE_DW_DATA.SYS_USER sysuser
      ON sysuser.SYS_USER_KEY = fwf.SYS_USER_KEY
    LEFT OUTER JOIN TREXONE_AUD_DATA.SYSTEM_USER userrole
      ON userrole.SYSTEM_USER_NUM = sysuser.SYS_USER_NUM
     AND userrole.H_LEVEL = sysuser.H_LEVEL
    LEFT OUTER JOIN fills bpv
      ON bpv.RX_RECORD_NUM = fwf.FWFS_RX_RECORD_NUM
     AND bpv.RX_FILL_SEQ = fwf.FWFS_RX_FILL_SEQ
     AND bpv.FWFS_DT_OUT = fwf.FWFS_DT_OUT
ORDER BY fwf.FWFS_DT_OUT, fwf.FWFS_DT_IN, ownstore.FD_FACILITY_ID, fwf.FWFS_RX_NUMBER