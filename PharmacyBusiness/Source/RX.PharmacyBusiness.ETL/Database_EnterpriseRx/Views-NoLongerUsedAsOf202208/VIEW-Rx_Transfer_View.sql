
CREATE OR REPLACE FORCE EDITIONABLE VIEW WEGMANS.RX_TRANSFER_VIEW (
  BASE_STORE_NUM,
  BASE_STORE_NAME,
  TO_STORE_NUM,
  TO_STORE,
  FROM_STORE_NUM,
  FROM_STORE,
  TRANSFER_DEST,
  PATIENT_NUM,
  ORIG_RX_NUM,
  REFILL_NUM,
  TRANSFER_DATE,
  SOLD_DATE,
  READY_DATE,
  CANCEL_DATE,
  TRANSFER_TIME_KEY,
  WRITTEN_NDC_WO,
  WRITTEN_DRUG_NAME,
  DISP_NDC_WO,
  DISP_DRUG_NAME,
  QTY_DISPENSED,
  DAW,
  TRANS_TYPE,
  TRANS_METHOD,
  SIG_TEXT,
  PRESCRIBER_KEY,
  RX_RECORD_NUM,
  RX_FILL_SEQ,
  PATIENT_PAY,
  TP_PAY,
  TX_PRICE,
  ACQ_COST,
  U_C_PRICE,
  FIRST_FILL_DATE,
  LAST_FILL_DATE,
  SENDING_RPH,
  RECEIVE_RPH,
  XFER_ADDR,
  XFER_ADDRE,
  XFER_CITY,
  XFER_ST,
  XFER_ZIP,
  XFER_PHONE,
  TRANSFER_REASON,
  NEW_RX_RECORD_NUM,
  COMPETITOR_GROUP,
  COMPETITOR_STORE_NUM,
  TRANSFER_DATE_KEY
) AS
SELECT
    basefac.FD_FACILITY_ID AS base_store_num,
    basefac.FD_LEGAL_NAME AS base_store_name,
    (CASE WHEN transtype.RX_TRANSFER_TYPE_IND = 'OUT' THEN transfact.FACILITY_ID
          ELSE basefac.FD_FACILITY_ID
     END) AS to_store_num,  
    (CASE WHEN transtype.RX_TRANSFER_TYPE_IND = 'OUT' THEN transfact.FACILITY_NAME
          ELSE basefac.FD_LEGAL_NAME
     END) AS to_store,
    (CASE WHEN transtype.RX_TRANSFER_TYPE_IND = 'IN' THEN transfact.FACILITY_ID
          ELSE basefac.FD_FACILITY_ID
     END) AS from_store_num,
    (CASE WHEN transtype.RX_TRANSFER_TYPE_IND = 'IN' THEN transfact.FACILITY_NAME
          ELSE basefac.FD_LEGAL_NAME
     END) AS from_store,
    (CASE WHEN transfact.FACILITY_NAME = facname.FD_LEGAL_NAME THEN 'In-Chain'
          ELSE 'Out-Of-Chain'
     END) AS transfer_dest,
    pt.PD_PATIENT_NUM AS patient_num,
    To_Number(
      (CASE WHEN (DECODE(replace(translate(transfact.RX_NUMBER,'1234567890','##########'),'#'),NULL,'1','0') = '1') THEN transfact.RX_NUMBER
            ELSE '9999999'
       END)) AS orig_rx_num,
    transfact.REFILL_NUM AS refill_num,
    transdate.CAL_DATE AS transfer_date,
    nullif(trunc(solddate.CAL_DATE),trunc(to_date('5555-12-31','YYYY-MM-DD'))) AS sold_date,
    nullif(trunc(readydate.CAL_DATE),trunc(to_date('5555-12-31','YYYY-MM-DD'))) AS ready_date,
    nullif(trunc(canceldate.CAL_DATE),trunc(to_date('5555-12-31','YYYY-MM-DD'))) AS cancel_date,
    transfact.TRANSFER_TIME_KEY AS transfer_time_key,
    to_char(writdrug.DRUG_NDC) AS written_ndc_wo,
    writdrug.DRUG_LABEL_NAME AS written_drug_name,
    to_char(dispdrug.DRUG_NDC) AS disp_ndc_wo,
    dispdrug.DRUG_LABEL_NAME AS disp_drug_name,
    transfact.QTY_DISPENSED AS qty_dispensed,
    daw.DC_DAW_CODE AS daw,
    transtype.RX_TRANSFER_TYPE_IND AS trans_type,
    transmeth.RX_TRANSFER_METHOD_IND AS trans_method,
    REPLACE(TRANSFACT.SIG,CHR(10),'') AS sig_text,
    transfact.PRS_PRESCRIBER_KEY AS prescriber_key,
    transfact.RX_RECORD_NUM AS rx_record_num,
    transfact.RX_FILL_SEQ AS rx_fill_seq,
    transfact.PATIENT_PRICE_PAID AS patient_pay,
    transfact.TOTAL_TP_PAID AS tp_pay,
    transfact.TOTAL_PRICE_PAID AS tx_price,
    transfact.USER_DEFINED_STEP_AMT AS acq_cost,
    transfact.TOTAL_UNC AS u_c_price,
    transfact.FIRST_FILL_DATE AS first_fill_date,
    transfact.LAST_FILL_DATE AS last_fill_date,
    transfact.SENDING_PHARMACIST AS sending_rph,
    transfact.RECEIVING_PHARMACIST AS receive_rph,
    transfact.REC_PHARMACY_ADDR1 AS xfer_addr,
    transfact.REC_PHARMACY_ADDR2 AS xfer_addre,
    transfact.REC_PHARMACY_CITY AS xfer_city,
    transfact.REC_PHARMACY_STATE AS xfer_st,
    transfact.REC_PHARMACY_ZIP AS xfer_zip,
    transfact.REC_PHARMACY_PHONE AS xfer_phone,
    transfact.TRANSFER_REASON AS transfer_reason,
    transfact.TRANSFER_TO_RX_RECORD_NUM AS new_rx_record_num,
    transfact.COMPETITOR_GROUP_CODE AS competitor_group,
    transfact.STORE_NUMBER AS competitor_store_num,
    transfact.TRANSFER_DATE_KEY
FROM TREXONE_DW_DATA.RX_TRANSFER_FACT transfact
    LEFT OUTER JOIN TREXONE_DW_DATA.PATIENT pt
       ON transfact.PD_PATIENT_KEY = pt.PD_PATIENT_KEY
    LEFT OUTER JOIN TREXONE_DW_DATA.RX_TRANSFER_METHOD_TYPE transmeth
       ON transfact.RX_TRANSFER_METHOD_KEY = transmeth.RX_TRANSFER_METHOD_KEY
    LEFT OUTER JOIN TREXONE_DW_DATA.RX_TRANSFER_TYPE transtype
       ON transfact.RX_TRANSFER_TYPE_KEY = transtype.RX_TRANSFER_TYPE_KEY
    LEFT OUTER JOIN TREXONE_DW_DATA.FACILITY basefac
       ON transfact.FD_FACILITY_KEY = basefac.FD_FACILITY_KEY    
    LEFT OUTER JOIN TREXONE_DW_DATA.PRODUCT writprod
       ON transfact.PRD_PRODUCT_KEY = writprod.PRD_PRODUCT_KEY
    LEFT OUTER JOIN TREXONE_DW_DATA.DRUG writdrug
       ON writprod.PRD_DRUG_NUM = writdrug.DRUG_NUM    
    LEFT OUTER JOIN TREXONE_DW_DATA.PRODUCT dispprod
       ON transfact.DISPENSED_PRD_PRODUCT_KEY = dispprod.PRD_PRODUCT_KEY
    LEFT OUTER JOIN TREXONE_DW_DATA.DRUG dispdrug
       ON dispprod.PRD_DRUG_NUM = dispdrug.DRUG_NUM    
    LEFT OUTER JOIN TREXONE_DW_DATA.DAW_CODE_TYPE daw
       ON transfact.DC_DAW_CODE_KEY = daw.DC_DAW_CODE_KEY
    LEFT OUTER JOIN TREXONE_DW_DATA.DATE_DIM transdate
       ON transfact.TRANSFER_DATE_KEY = transdate.DATE_KEY
    LEFT OUTER JOIN TREXONE_DW_DATA.TIME_DIM transtime
       ON transfact.TRANSFER_TIME_KEY = transtime.TIME_KEY
    LEFT OUTER JOIN TREXONE_DW_DATA.DATE_DIM solddate
       ON transfact.SOLD_DATE_KEY = solddate.DATE_KEY
    LEFT OUTER JOIN TREXONE_DW_DATA.DATE_DIM readydate
       ON transfact.READY_DATE_KEY = readydate.DATE_KEY    
    LEFT OUTER JOIN TREXONE_DW_DATA.FACILITY facname
       ON transfact.FACILITY_NAME = facname.FD_LEGAL_NAME
    LEFT OUTER JOIN TREXONE_DW_DATA.DATE_DIM canceldate
       ON transfact.CANCEL_DATE_KEY = canceldate.DATE_KEY

WITH READ ONLY;
/ 

