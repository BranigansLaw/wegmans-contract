SELECT facility.FD_FACILITY_ID AS "store_num"
,poit.PO_NUMBER AS "po_num"
,poit.PO_TYPE_CODE AS "po_type"
,vendor.NAME AS "vendor_name"
,poit.IS_SCHEDULE_2_ORDER AS "is_c2_order"
,sysusers.SYS_USER_LOGIN_NAME AS "po_created_by_user"
,Round((pocreateddate.CAL_DATE - To_Date('20350101','YYYYMMDD')),6) AS "po_created_date"
,pocreatedtime.HOUR_MIN_SEC_TXT AS "po_created_time"
,Round((poreceiveddate.CAL_DATE - To_Date('20350101','YYYYMMDD')),6) AS "po_received_date"
,poreceivedtime.HOUR_MIN_SEC_TXT AS "po_received_time"
,Round((poexpriationdate.CAL_DATE - To_Date('20350101','YYYYMMDD')),6) AS "po_expiration_date"
,poexpirationtime.HOUR_MIN_SEC_TXT AS "po_expiration_time"
,Round((poremoveddate.CAL_DATE - To_Date('20350101','YYYYMMDD')),6) AS "po_removed_date"
,poremovedtime.HOUR_MIN_SEC_TXT AS "po_removed_time"
,poit.H_TYPE AS "update_type"
,users.SYS_USER_LOGIN_NAME AS "update_user"
,Round((poit.PO_LAST_UPD_DATE - To_Date('20350101','YYYYMMDD')),6) AS "po_last_update_date"
,postatustype.DESCRIPTION AS "po_status_type"
,poit.LINE_NUMBER AS "line_num"
,drug.DRUG_NDC AS "ndc_wo"
,Substr(WEGMANS.Clean_String_ForTenUp(drug.DRUG_LABEL_NAME),1,35) AS "drug_name"
,Round((itemcreatedate.CAL_DATE - To_Date('20350101','YYYYMMDD')),6) AS "po_item_created_date"
,poitemcreatedtime.HOUR_MIN_SEC_TXT AS "po_item_created_time"
,Round((itemreceiveddate.CAL_DATE - To_Date('20350101','YYYYMMDD')),6) AS "po_item_received_date"
,poitemreceivedtime.HOUR_MIN_SEC_TXT AS "po_item_received_time"
,Round((itemrremoveddate.CAL_DATE - To_Date('20350101','YYYYMMDD')),6) AS "po_item_removed_date"
,poitemremovedtime.HOUR_MIN_SEC_TXT AS "po_item_removed_time"
---,poit.VENDOR_PRICE_CATALOG_KEY AS "vendor_price_catalog_key"
---,poit.SUBSTITUTED_PRICE_CATALOG_KEY AS "sub_price_catalog_key"
,poshipstatus.PO_ACK_SHIPPING_STATUS AS "po_ack_shipping_status"
,poshipstatus.PO_ACK_SHIPPING_DESC AS "ack_shipping_status_desc"
,poit.ORDERED_QTY AS "ordered_qty"
,poit.RECEIVED_QTY AS "received_qty"
,poit.ACK_QUANTITY AS "ack_qty"
,poit.EXPECTED_ACQ_COST AS "expected_acq_cost"
,poit.ACTUAL_ACQ_COST AS "actual_acq_cost"
,poit.ACK_ACQUISITION_COST AS "ack_acq_cost"
,poit.RCVNG_EXCPTN_STATUS_CODE AS "rec_exception_status"
,poit.MODIFIED_AFTER_ORDERED AS "modified_afer_ordered"
,Round((poit.PO_ACK_DATE - To_Date('20350101','YYYYMMDD')),6) AS "po_ack_date"
,poackship.DESCRIPTION AS "po_ack_type"

from TREXONE_DW_DATA.PURCHASE_ORDER_ITEM_FACT poit
LEFT OUTER JOIN TREXONE_DW_DATA.FACILITY facility
ON poit.FD_FACILITY_KEY = facility.FD_FACILITY_KEY
LEFT OUTER JOIN TREXONE_DW_DATA.DRUG drug
ON poit.DRUG_KEY = drug.DRUG_KEY
--Copy this for other date keys
LEFT OUTER JOIN TREXONE_DW_DATA.DATE_DIM itemcreatedate
ON poit.PO_ITEM_CREATED_DATE_KEY = itemcreatedate.DATE_KEY

LEFT OUTER JOIN TREXONE_DW_DATA.DATE_DIM itemreceiveddate
ON poit.PO_ITEM_RECEIVED_DATE_KEY = itemreceiveddate.DATE_KEY

LEFT OUTER JOIN TREXONE_DW_DATA.DATE_DIM itemrremoveddate
ON poit.PO_ITEM_REMOVED_DATE_KEY = itemrremoveddate.DATE_KEY

LEFT OUTER JOIN TREXONE_DW_DATA.DATE_DIM pocreateddate
ON poit.PO_CREATED_DATE_KEY = pocreateddate.DATE_KEY

LEFT OUTER JOIN TREXONE_DW_DATA.DATE_DIM poreceiveddate
ON poit.PO_RECEIVED_DATE_KEY = poreceiveddate.DATE_KEY

LEFT OUTER JOIN TREXONE_DW_DATA.DATE_DIM poexpriationdate
ON poit.PO_EXPRIATION_DATE_KEY = poexpriationdate.DATE_KEY

LEFT OUTER JOIN TREXONE_DW_DATA.DATE_DIM poremoveddate
ON poit.PO_REMOVED_DATE_KEY = poremoveddate.DATE_KEY

LEFT OUTER JOIN TREXONE_DW_DATA.VENDOR vendor
ON poit.VENDOR_KEY = vendor.VENDOR_KEY

LEFT OUTER JOIN TREXONE_DW_DATA.SYS_USER sysusers
ON poit.PO_CREATED_USER_KEY = sysusers.SYS_USER_KEY

LEFT OUTER JOIN TREXONE_DW_DATA.SYS_USER users
ON poit.PO_LAST_UPD_SYS_USER_KEY = users.SYS_USER_KEY

--time
LEFT OUTER JOIN TREXONE_DW_DATA.TIME_DIM pocreatedtime
ON poit.PO_CREATED_TIME_KEY = pocreatedtime.TIME_KEY

LEFT OUTER JOIN TREXONE_DW_DATA.TIME_DIM poreceivedtime
ON poit.PO_RECEIVED_TIME_KEY = poreceivedtime.TIME_KEY


LEFT OUTER JOIN TREXONE_DW_DATA.TIME_DIM poexpirationtime
ON poit.PO_EXPIRATION_TIME_KEY = poexpirationtime.TIME_KEY


LEFT OUTER JOIN TREXONE_DW_DATA.TIME_DIM poremovedtime
ON poit.PO_REMOVED_TIME_KEY = poremovedtime.TIME_KEY


LEFT OUTER JOIN TREXONE_DW_DATA.TIME_DIM poitemcreatedtime
ON poit.PO_ITEM_CREATED_TIME_KEY = poitemcreatedtime.TIME_KEY

LEFT OUTER JOIN TREXONE_DW_DATA.TIME_DIM poitemreceivedtime
ON poit.PO_ITEM_RECEIVED_TIME_KEY = poitemreceivedtime.TIME_KEY

LEFT OUTER JOIN TREXONE_DW_DATA.TIME_DIM poitemremovedtime
ON poit.PO_ITEM_REMOVED_TIME_KEY = poitemremovedtime.TIME_KEY

LEFT OUTER JOIN TREXONE_DW_DATA.PO_STATUS_TYPE postatustype
ON poit.PO_STATUS_TYPE_KEY = postatustype.PO_STATUS_TYPE_KEY

LEFT OUTER JOIN TREXONE_DW_DATA.PO_ACK_SHIPPING_STATUS_TYPE poshipstatus
ON poit.PO_ACK_SHIPPING_STATUS_KEY = poshipstatus.PO_ACK_SHIPPING_STATUS_KEY

LEFT OUTER JOIN TREXONE_DW_DATA.ACK_SHIPPING_STATUS_TYPE poackship
ON poit.PO_ACK_TYPE_CODE = poackship.ACK_SHIPPING_STATUS

WHERE poit.PO_CREATED_DATE_KEY BETWEEN To_Number(To_Char((:RunDate - 30),'YYYYMMDD')) AND To_Number(To_Char((:RunDate - 1),'YYYYMMDD'))
AND poit.PO_LAST_UPD_DATE between (:RunDate - 1) AND (:RunDate - 1 + INTERVAL '23:59:59' HOUR TO SECOND)