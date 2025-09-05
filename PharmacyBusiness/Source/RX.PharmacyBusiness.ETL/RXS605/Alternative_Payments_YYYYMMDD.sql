SELECT /*+ cardinality(AUD__Fill_Fact,1) leading (AUD__Fill_Fact) index (AUD__Fill_Fact IN_FILL_FACT_03) */
     Distinct
     To_Number(DW__Facility.fd_facility_id) AS store_num
    ,To_Number(To_Char(AUD__Fill_Fact.fill_state_chg_ts,'YYYYMMDD')) AS sold_date
    ,Round((AUD__Fill_Fact.fill_state_chg_ts - To_Date('20350101','YYYYMMDD')),6) AS sold_datetime
    ,AUD__Order_Record.FACILITY_ORDER_NUMBER AS order_num        
    ,To_Number(AUD__Fill_Fact.rx_number) AS rx_num        
    ,AUD__Fill_Fact.refill_num
    ,AUD__Fill_Fact.partial_fill_seq AS part_seq_num    
    ,DW__Payment_Type.payment_type_name
    ,(AUD__Fill_Fact.final_price * (CASE WHEN AUD__Fill_Fact.fill_state_code = 16 THEN -1 ELSE 1 END)) AS total_price_paid
    ,(AUD__Fill_Fact.patient_pay_amount * (CASE WHEN AUD__Fill_Fact.fill_state_code = 16 THEN -1 ELSE 1 END)) AS patient_price_paid
    ,(AUD__Fill_Fact.total_uandc * (CASE WHEN AUD__Fill_Fact.fill_state_code = 16 THEN -1 ELSE 1 END)) AS total_unc        
    ,To_Number(To_Char(AUD__Payment.payment_date,'YYYYMMDD')) AS payment_date
    ,DW__Shipment_Fact.tracking_number
    ,DW__Shipment_Fact.pad_key AS ship_pad_key
    ,(DW__Shipment_Fact.ship_handle_fee * (CASE WHEN AUD__Fill_Fact.fill_state_code = 16 THEN -1 ELSE 1 END)) AS ship_handle_fee
    ,(DW__Shipment_Fact.courier_ship_charge * (CASE WHEN AUD__Fill_Fact.fill_state_code = 16 THEN -1 ELSE 1 END)) AS courier_ship_charge
    ,To_Number(To_Char(AUD__Fill_Fact.date_of_service,'YYYYMMDD')) AS date_of_service
    ,DW__Drug.drug_ndc
    ,DW__Tp_Plan.tpld_plan_code
    ,AUD__Fill_Fact.dispensed_prod_num AS product_num
    ,DW__Tp_Plan.tpld_plan_key AS primary_tpld_plan_key
    ,AUD__Fill_Fact.patient_num AS pd_patient_num
    ,AUD__Fill_Fact.rx_fill_seq
    ,AUD__Fill_Fact.rx_record_num
    ,NVL(AUD__Fill_Fact.is_bar,'N') AS is_bar
FROM TREXONE_AUD_DATA.Fill_Fact AUD__Fill_Fact
  INNER JOIN TREXONE_DW_DATA.Facility DW__Facility
     ON DW__Facility.fd_facility_num = AUD__Fill_Fact.facility_num
    AND DW__Facility.FD_IS_PPI_ENABLED = 'Y'
  INNER JOIN TREXONE_DW_DATA.Drug DW__Drug
     ON DW__Drug.drug_num = AUD__Fill_Fact.dispensed_drug_num
  LEFT OUTER JOIN TREXONE_DW_DATA.Tp_Patient DW__Tp_Patient
     ON DW__Tp_Patient.tpd_tp_patient_num = AUD__Fill_Fact.tp_patient_num
    AND DW__Tp_Patient.tpd_eff_end_date = TO_DATE('12/31/2999', 'MM/DD/YYYY')
  LEFT OUTER JOIN TREXONE_DW_DATA.Tp_Plan DW__Tp_Plan
     ON DW__Tp_Plan.tpld_plan_num = AUD__Fill_Fact.tp_plan_num
  LEFT OUTER JOIN TREXONE_AUD_DATA.Item AUD__Item
     ON AUD__Item.rx_record_num = AUD__Fill_Fact.rx_record_num
    AND AUD__Item.rx_fill_seq = AUD__Fill_Fact.rx_fill_seq
    AND AUD__Item.eff_end_date = TO_DATE('12/31/2999', 'MM/DD/YYYY')
    AND AUD__Item.H_TYPE != 'D'
  LEFT OUTER JOIN TREXONE_AUD_DATA.Order_Record AUD__Order_Record
     ON AUD__Order_Record.order_num = AUD__Item.order_num
    AND AUD__Order_Record.eff_end_date = TO_DATE('12/31/2999', 'MM/DD/YYYY')
  LEFT OUTER JOIN TREXONE_DW_DATA.Shipment_Fact DW__Shipment_Fact
     ON DW__Shipment_Fact.order_num = AUD__Item.order_num
    AND DW__Shipment_Fact.pd_patient_key = DW__Tp_Patient.pd_patient_key
  INNER JOIN TREXONE_AUD_DATA.Payment_Group_List AUD__Payment_Group_List
     ON AUD__Payment_Group_List.payment_group_num = AUD__Item.payment_group_num
    AND AUD__Payment_Group_List.eff_end_date = TO_DATE('12/31/2999', 'MM/DD/YYYY')  
  INNER JOIN TREXONE_AUD_DATA.Payment AUD__Payment
     ON AUD__Payment.payment_num = AUD__Payment_Group_List.payment_num
    AND AUD__Payment.eff_end_date = TO_DATE('12/31/2999', 'MM/DD/YYYY')
  LEFT OUTER JOIN TREXONE_DW_DATA.Payment_Type DW__Payment_Type
     ON DW__Payment_Type.payment_type_num = AUD__Payment.payment_type_num
  LEFT OUTER JOIN TREXONE_AUD_DATA.Payment_Status AUD__Payment_Status
     ON AUD__Payment_Status.payment_status_num = AUD__Payment.payment_status_num
WHERE AUD__Fill_Fact.fill_state_chg_ts BETWEEN (Trunc(:RunDate) - 1) AND (Trunc(:RunDate) - 1 + INTERVAL '23:59:59' HOUR TO SECOND)
  AND AUD__Fill_Fact.fill_state_code IN (14,16)
  AND AUD__Fill_Fact.rx_number NOT LIKE 'RR%'
  AND DW__Payment_Type.payment_type_name NOT IN ('CREDIT CARD','CREDIT CARD REVERSAL')
  AND AUD__Fill_Fact.IS_SAME_DAY_REVERSAL != 'Y'
  AND Upper(AUD__Payment_Status.payment_status_name) != 'CANCELLED'
ORDER BY
     AUD__Order_Record.FACILITY_ORDER_NUMBER
    ,AUD__Fill_Fact.rx_number
    ,Round((AUD__Fill_Fact.fill_state_chg_ts - To_Date('20350101','YYYYMMDD')),6)
    ,To_Number(To_Char(AUD__Payment.payment_date,'YYYYMMDD'))

