       -- Query Prescription fill
       SELECT f.fd_facility_id facility_id
             , fwf.fwfs_rx_number rx_number
             , d.drug_ndc ndc
             , REPLACE(d.drug_brand_name,',','') drug_name
             , fwf.fwfs_dt_in date_in
             , fwf.fwfs_dt_out date_out
             --, ws.WFSD_STEP_DESCRIPTION workflow_step
             , DECODE(ws.WFSD_STEP_DESCRIPTION,'Pre-Verification 1','Pre-Verification',DECODE(ws.WFSD_STEP_DESCRIPTION,'DUR - Pre-Verification 1','DUR - Pre-Verification',ws.WFSD_STEP_DESCRIPTION)) workflow_step
             , dmt.dm_delivery_method_desc delivery_method
             , su.SYS_USER_LOGIN_NAME user_id
             , fwf.fwfs_qty_dispensed qty_dispensed
             , f2.fd_facility_id owning_facility
             , DECODE(f.fd_facility_id, f2.fd_facility_id, 'No', 'Yes') WLB
             , (SELECT SKIP_PRE_VER_CODE
                  FROM TREXONE_AUD_DATA.RX_FILL bpv
                 WHERE bpv.RX_RECORD_NUM = fwf.FWFS_RX_RECORD_NUM
                   AND bpv.RX_FILL_SEQ = fwf.FWFS_RX_FILL_SEQ
                   AND bpv.H_LEVEL IN (
                       SELECT MAX(H_LEVEL)
                         FROM TREXONE_AUD_DATA.RX_FILL
                        WHERE RX_RECORD_NUM = fwf.FWFS_RX_RECORD_NUM
                          AND RX_FILL_SEQ = fwf.FWFS_RX_FILL_SEQ
                          AND CAST(FROM_TZ(CAST(DATESTAMP AS TIMESTAMP), 'UTC') AT TIME ZONE 'US/Eastern' AS DATE) <=  fwf.FWFS_DT_OUT
                   )
               ) SKIP_PRE_VER_CODE
          FROM trexone_dw_data.fill_workflow_fact fwf
          LEFT OUTER JOIN trexone_dw_data.facility f ON (fwf.fd_facility_key = f.fd_facility_key)
          LEFT OUTER JOIN trexone_dw_data.DELIVERY_METHOD_TYPE dmt ON (fwf.dm_delivery_method_key = dmt.dm_delivery_method_key)
          LEFT OUTER JOIN trexone_dw_data.wf_step ws ON (fwf.WFSD_KEY = ws.WFSD_KEY)
          LEFT OUTER JOIN trexone_dw_data.SYS_USER su ON (fwf.SYS_USER_KEY = su.SYS_USER_KEY)
          LEFT OUTER JOIN trexone_dw_data.drug d ON (fwf.drug_key = d.drug_key)
          LEFT OUTER JOIN trexone_dw_data.facility f2 ON (fwf.owning_facility_key = f2.fd_facility_key)
          LEFT OUTER JOIN trexone_dw_data.order_source_type ost ON (fwf.os_order_source_key = ost.os_order_source_key)
        WHERE fwf.fwfs_dt_out BETWEEN (:RunDate - 1) AND (:RunDate - 1 + INTERVAL '23:59:59' HOUR TO SECOND)
          AND su.SYS_USER_LOGIN_NAME <> 'SYSTEM'
          AND ws.WFSD_STEP_DESCRIPTION NOT IN ('Completion Pending', 'Cancelled', 'Order Grouping', 'Check Out', 'Post-Verification', 'In Process', 'Mail Shipping', 'Returns', 'System Block')
UNION ALL -- Query New Patient
SELECT 
             fac.fd_facility_id facility_id
             , null as rx_number
             , null as ndc
             , null as drug_name
             , pat.create_date date_in
             , pat.create_date date_out
             , 'New Patients' as workflow_step
             , null as delivery_method
             , null as user_id
             , count(*) as qty_dispensed
             , null as owning_facility
             , null as WLB
             , null as SKIP_PRE_VER_COD
        FROM trexone_aud_data.patient pat
	       INNER JOIN TREXONE_DW_DATA.facility fac ON fac.fd_facility_num = pat.create_facility_num
        WHERE pat.create_date BETWEEN (:RunDate - 1) AND (:RunDate - 1 + INTERVAL '23:59:59' HOUR TO SECOND)
          AND pat.eff_end_date = TO_DATE('12/31/2999', 'MM/DD/YYYY') 
       GROUP BY fac.fd_facility_id, pat.create_date
UNION ALL -- Query Drugs Recieved
SELECT
        fac.fd_facility_id AS facility_id
        ,null as rx_number
        ,null as ndc
        ,null as drug_name
        ,poi.received_date AS date_in 
        ,poi.received_date AS date_out
        ,'Items Received' AS WORKFLOW_STEP
        ,null as delivery_method
        ,null as user_id
        ,count(*) AS QTY_DISPENSED
        ,null as owning_facility
        ,null as WLB
        ,null as SKIP_PRE_VER_CODE
      FROM trexone_aud_data.purchase_order_item poi
        INNER JOIN trexone_aud_data.purchase_order po ON po.purchase_order_num = poi.purchase_order_num
        INNER JOIN TREXONE_DW_DATA.facility fac ON fac.fd_facility_num = po.facility_num
      WHERE poi.received_date BETWEEN (:RunDate - 1) AND (:RunDate - 1 + INTERVAL '23:59:59' HOUR TO SECOND)
       AND poi.eff_end_date = TO_DATE('12/31/2999', 'MM/DD/YYYY')
       AND po.eff_end_date = TO_DATE('12/31/2999', 'MM/DD/YYYY')
    GROUP BY fac.fd_facility_id, poi.received_date