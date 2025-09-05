SELECT facility.FD_FACILITY_ID AS "store_num"
,po.PO_NUMBER "po_number"
,po.PURCHASE_ORDER_TYPE_CODE AS "po_type"
,vendrug.vendor_name AS "vendor_name"
,po.IS_SCHEDULE_2_ORDER AS "is_c2_order"
,Round((poit.CREATE_DATE - To_Date('20350101','YYYYMMDD')),6) AS "po_create_datetime"
,poit.LINE_NUMBER AS "line_num"
,poit.H_TYPE AS "update_type"
,Round((poit.DATESTAMP - To_Date('20350101','YYYYMMDD')),6) AS "update_datetime"
,sysusers.SYS_USER_LOGIN_NAME AS "user_login"
,poit.PURCHASE_ORDER_ITEM_NUM
,vendrug.NDC_WO "ordered_ndc_wo"
,Substr(WEGMANS.Clean_String_ForTenUp(vendrug.DRUG_NAME),1,35) AS "ordered_drug_name"
,vendrugsub.NDC_WO "subsituted_ndc_wo"
,Substr(WEGMANS.Clean_String_ForTenUp(vendrugsub.DRUG_NAME),1,35) AS "subsituted_drug_name"
,poit.ORDERED_QUANTITY AS "ordered_qty"
,poit.RECEIVED_QUANTITY AS "received_qty"
,Round((poit.RECEIVED_DATE - To_Date('20350101','YYYYMMDD')),6) AS "received_datetime"
,poit.EXPECTED_ACQ_COST AS "expected_acq_cost"
,poit.ACTUAL_ACQ_COST AS "actual_acq_cost"
,poit.RCVNG_EXCPTN_STATUS_CODE AS "rec_exception_status"
,poit.MODIFIED_AFTER_ORDERED AS "modified_afer_ordered"

from TREXONE_AUD_DATA.PURCHASE_ORDER_ITEM poit

LEFT OUTER JOIN TREXONE_DW_DATA.FACILITY facility
ON poit.FACILITY_NUM = facility.FD_FACILITY_NUM

LEFT OUTER JOIN 
(SELECT drug.DRUG_NDC AS "NDC_WO"
,drug.drug_label_name AS "DRUG_NAME"
,vendorprice.VENDOR_ITEM_NUM AS "VENDOR_ITEM_NUM"
,vendorprice.EFFECTIVE_START_DATE AS "START_DATE"
,vendorprice.EFFECTIVE_END_DATE AS "END_DATE"
,vendor.name AS "VENDOR_NAME"
FROM TREXONE_DW_DATA.VENDOR_PRICE_CATALOG vendorprice
LEFT OUTER JOIN TREXONE_DW_DATA.DRUG drug
ON vendorprice.DRUG_KEY = drug.DRUG_KEY
LEFT OUTER JOIN TREXONE_DW_DATA.VENDOR vendor
ON vendorprice.VENDOR_KEY = vendor.VENDOR_KEY)vendrug

ON poit.CREATE_DATE BETWEEN vendrug.START_DATE AND vendrug.END_DATE
AND vendrug.VENDOR_ITEM_NUM = poit.ORDERED_VENDOR_ITEM_NUM


LEFT OUTER JOIN TREXONE_AUD_DATA.PURCHASE_ORDER po
ON poit.PURCHASE_ORDER_NUM = po.PURCHASE_ORDER_NUM
AND po.EFF_END_DATE = TO_DATE('12312999','MMDDYYYY')

LEFT OUTER JOIN 
(SELECT drug.DRUG_NDC AS "NDC_WO"
,drug.drug_label_name AS "DRUG_NAME"
,vendorpricesub.VENDOR_ITEM_NUM AS "VENDOR_ITEM_NUM"
,vendorpricesub.EFFECTIVE_START_DATE AS "START_DATE"
,vendorpricesub.EFFECTIVE_END_DATE AS "END_DATE"
FROM TREXONE_DW_DATA.VENDOR_PRICE_CATALOG vendorpricesub
LEFT OUTER JOIN TREXONE_DW_DATA.DRUG drug
ON vendorpricesub.DRUG_KEY = drug.DRUG_KEY)vendrugsub

ON poit.CREATE_DATE BETWEEN vendrugsub.START_DATE AND vendrugsub.END_DATE
AND vendrugsub.VENDOR_ITEM_NUM = poit.SUBSTITUTED_VENDOR_ITEM_NUM

LEFT OUTER JOIN TREXONE_DW_DATA.SYS_USER sysusers
ON poit.SYS_USER = sysusers.SYS_USER_NUM

WHERE poit.DATESTAMP between (:RunDate - 1) AND (:RunDate - 1 + INTERVAL '23:59:59' HOUR TO SECOND)