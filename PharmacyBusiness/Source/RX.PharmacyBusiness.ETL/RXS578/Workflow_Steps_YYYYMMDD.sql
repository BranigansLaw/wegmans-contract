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
        max_rx_fill.SKIP_PRE_VER_CODE,
        max_rx_fill.DAYS_SUPPLY,
        max_rx_fill.RX_RECORD_NUM,
        max_rx_fill.RX_FILL_SEQ,
        max_rx_fill.FWFS_DT_OUT
    FROM (
        SELECT 
            r.SKIP_PRE_VER_CODE,
            r.DAYS_SUPPLY,
            r.RX_RECORD_NUM,
            r.RX_FILL_SEQ,
            s.FWFS_DT_OUT,
            Rank() OVER (PARTITION BY r.RX_RECORD_NUM, r.RX_FILL_SEQ, s.FWFS_DT_OUT
                         ORDER BY r.H_LEVEL Desc, rownum Desc) AS RX_FILL_Rank
        FROM TREXONE_AUD_DATA.RX_FILL r
             INNER JOIN step_keys s
               ON r.RX_RECORD_NUM = s.FWFS_RX_RECORD_NUM
              AND r.RX_FILL_SEQ = s.FWFS_RX_FILL_SEQ
              AND s.FWFS_DT_OUT BETWEEN CAST(FROM_TZ(CAST(r.DATESTAMP AS TIMESTAMP), 'UTC') AT TIME ZONE 'US/Eastern' AS DATE) AND r.EFF_END_DATE
         ) max_rx_fill
    WHERE max_rx_fill.RX_FILL_Rank = 1
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
	TRIM(sysuser.SYS_USER_LOGIN_NAME) "emp_login_name",
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
