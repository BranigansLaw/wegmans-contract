SELECT 
	 FILL_STATE_CHG_TS
	,FILL_STATE_CHG_TS_DATE
	,FILL_STATE_CHG_TS_datetime
	,store_num
	,Store_NPI
	,Store_NCPDP
	,Facility_NPI
	,rx_num
	,rx_num_txt
	,REFILL_NUM
	,PARTIAL_FILL_SEQ
	,benefit_group_id
	,date_of_service
	,daw_code
	,days_supply
	,dispensed_drug_num
	,dispensed_qty_awp
	,facility_num
	,fill_state_code
	,fill_state_key
	,fill_state_price_num
	,final_price
	,incentive_fee_amount
	,internal_fill_id
	,is_same_day_reversal
	,medical_condition_num
	,original_product_qty
	,patient_num
	,patient_pay_amount
	,prescribed_date
	,prescriber_address_num
	,prescriber_id_dea
	,prescriber_id_npi
	,prescriber_num
	,qty_dispensed
	,refills_authorized
	,refills_remaining
	,rx_fill_seq
	,rx_origin_code
	,rx_record_num
	,short_fill_status
	,split_billing_code
	,tlog_sequence_num
	,total_cost
	,total_fee
	,total_uandc
	,total_user_defined
	,tp_curr_patient_pay_amount
	,tp_curr_total_cost
	,tp_curr_total_fee
	,tp_item_claim_key
	,tp_item_claim_num
	,tp_message_key
	,tp_patient_num
	,tp_plan_amount_paid
	,tp_plan_num
	,tp_prior_auth_num
	,tp_processor_dest_num
	,transaction_code
	,IS_BAR
	,order_num
	,FS_FILL_STATE_CHG_TS_datetime
	,fs_fill_state_code
	,fs_rx_fill_seq
	,fs_rx_record_num
	,ingredient_ndc1
	,ingredient_ndc2
	,ingredient_ndc3
	,ingredient_ndc4
	,ingredient_ndc5
	,ingredient_ndc6
	,ingredient_price1
	,ingredient_price2
	,ingredient_price3
	,ingredient_price4
	,ingredient_price5
	,ingredient_price6
	,ingredient_qty1
	,ingredient_qty2
	,ingredient_qty3
	,ingredient_qty4
	,ingredient_qty5
	,ingredient_qty6
	,authorization_number
	,emergency_condition_code
	,external_billing_indicator
	,h_level        
	,tp_message_num
	,bin_number
	,processor_control_number
	,drug_gcn_seq
	,drug_is_maint_drug
	,drug_ndc
	,drug_package_size
	,drug_strength
	,drug_strength_units
	,mcd_frmttd_cond_ref_num
	,prs_prescriber_key
	,prs_prescriber_num
	,padr_key
	,prd_is_compound
	,prd_is_generic
	,prd_is_price_maintained
	,prd_name
	,prd_package_size
	,pd_patient_key
	,tpd_cardholder_id
	,tpd_cardholder_patient_num
	,tpd_relationship_num
	,tpd_tp_plan_num
	,tpld_plan_code
	,pcn
	,pcr_tp_plan_amount_paid
	,pcr_cash_tp_indicator
	,pcr_fill_state_price_num
	,pcr_total_cost
	,pcr_patient_pay_amount
	,pcr_total_fee
	,pcr_final_price
	,pcr_dispensed_qty_awp
	,pcr_dispensing_fee_amount
	,pcr_total_user_defined
	,pcr2_tp_plan_amount_paid
	,target_cf_facility_num
	,PICKUP_datetime
	,FACILITY_ORDER_NUMBER
	,rq_tpmsg_key
	,rq_tp_message_num
	,rq_tp_item_claim_num
	,rq_tp_processor_dest_seq
	,rq_tp_processor_num
	,rq_tpmsg_submission_datetime
	,rq_tpmsg_submission_status
	,rq_tpmsg_msg_category
	,rq_tpmsg_comm_failure
	,RQ_BIN_NUM
	,RQ_VERSION_RELEASE_NUM
	,RQ_TRANSACTION_CODE
	,RQ_PROCESSOR_CONTROL_NUM
	,RQ_TRANSACTION_COUNT
	,RQ_SERVICE_PROVIDER_ID_QUAL
	,RQ_SERVICE_PROVIDER_ID
	,RQ_DATE_OF_SERVICE
	,RQ_SOFTWARE_CERT_ID
	,RQ_RX_SERVICE_REF_QUAL
	,RQ_RX_SERVICE_REF_NUM
	,RQ_PRODUCT_SERVICE_ID_QUAL
	,RQ_PRODUCT_SERVICE_ID
	,RQ_QUANTITY_DISPENSED
	,RQ_FILL_NUMBER
	,RQ_DAYS_SUPPLY
	,RQ_COMPOUND_CODE
	,RQ_DAW_CODE
	,RQ_RX_WRITTEN_DATE
	,RQ_NUM_REFILLS_AUTHORIZED
	,RQ_RX_ORIGIN_CODE
	,RQ_SUBMISSION_CLARIFICATION
	,RQ_OTHER_COVERAGE_CODE
	,RQ_ORIG_RX_PROD_SVC_ID_QUAL
	,RQ_ORIG_RX_PROD_SVC_CODE
	,RQ_ORIG_RX_QTY
	,RQ_SCHEDULED_PRESCRIPTION_ID
	,RQ_UNIT_OF_MEASURE
	,RQ_PRIOR_AUTH_NUM_SUBMITTED
	,RQ_INTERMEDIARY_AUTH_ID
	,RQ_PHARMACY_SERVICE_TYPE
	,RQ_CARDHOLDER_ID
	,RQ_GROUP_ID
	,RQ_PERSON_CODE
	,RQ_PATIENT_RELATIONSHIP_CODE
	,RQ_PRESCRIBER_ID_QUAL
	,RQ_PRESCRIBER_ID
	,RQ_PRESCRIBER_LNAME
	,RQ_PRESCRIBER_FIRST_NAME
	,RQ_PRESCRIBER_STATE_ADDRESS
	,RQ_INGREDIENT_COST_SUBMITTED
	,RQ_DISPENSING_FEE_SUBMITTED
	,RQ_UANDC_CHARGE
	,RQ_GROSS_AMT_DUE
	,RQ_BASIS_OF_COST_DETERMINATION
	,rs_tpmsg_key
	,rs_tp_message_num
	,rs_tp_item_claim_num
	,rs_tp_processor_destinatn_seq
	,rs_tp_processor_num
	,rs_tpmsg_submission_datetime
	,rs_tpmsg_submission_status
	,rs_tpmsg_msg_category
	,rs_tpmsg_comm_failure
	,rs_version_release_num
	,rs_transaction_code
	,rs_transaction_count
	,rs_header_response_status
	,rs_svc_provider_id_qual
	,rs_svc_provider_id
	,rs_date_of_service
	,rs_transaction_rsp_status
	,rs_authorization_num
	,rs_approved_msg_code_count
	,rs_approved_msg_code1
	,rs_approved_msg_code2
	,rs_addtl_message_info
	,rs_rx_svc_ref_num
	,rs_group_id
	,rs_netwrk_reimbursement_id
	,rs_patient_pay_amt
	,rs_ingredient_cost_pd
	,rs_dispensing_fee_pd
	,rs_tax_exempt_ind
	,rs_flat_sales_tax_amt_pd
	,rs_pct_sales_tax_amt_pd
	,rs_pct_sales_tax_rate_pd
	,rs_pct_sales_tax_basis_pd
	,rs_incentive_fee_pd
	,rs_prof_service_fee_pd
	,rs_total_amt_pd
	,rs_basis_of_reimbrsmt_det
	,rs_amt_attr_to_sales_tax
	,rs_accumulated_deduct_amt
	,rs_remaining_deduct_amt
	,rs_remaining_benefit_amt
	,rs_amt_applied_to_per_deduct
	,rs_amt_of_copay
	,rs_amt_attrib_to_prod_select
	,rs_amt_exceed_per_benefit_max
	,rs_basis_of_calc_disp_fee
	,rs_basis_of_calc_copay
	,rs_basis_of_calc_flt_sales_tax
	,rs_basis_of_calc_pct_sales_tax
	,rs_actual_dispense_fee
	,rs_actual_tax_exempt_code
	,rs_actual_flat_sales_tax_amt
	,rs_actual_sales_tax_percent
	,rs_actual_sales_tax_rate
	,rs_actual_sales_tax_basis
	,rs_actual_incentive_amt
	,rs_actual_other_amt
	,rs_actual_sales_tax_amt
	,rs_actual_copay_amt
	,rs_processor_fee_amt
	,rs_patient_sales_tax_amount
	,rs_plan_sales_tax_amount
	,rs_coinsurance_amt
	,ship_num
	,ship_store_num
	,ship_pd_patient_key
	,ship_pad_key
	,ship_date
	,ship_datetime
	,ship_orig_exp_deliv_date
	,ship_orig_exp_deliv_datetime
	,ship_exp_deliv_date
	,ship_exp_deliv_datetime
	,ship_facility_order_number
	,ship_item_count
	,ship_package_count
	,ship_dea_class
	,ship_refrigeration
	,ship_weight
	,ship_weight_units
	,ship_tracking_number
	,ship_handle_fee
	,ship_courier_charge
	,ship_is_complete
	,ship_orig_method
	,ship_method
	,ship_orig_courier_name
	,ship_courier_name
	,ship_delivery_method
	,sig_required
    
    --NEW FIELDS ARE BELOW HERE    
	,is_change_status
    ,item_seq

    ,RXF_DISPENSE_DATE
    ,RXF_DAYS_SUPPLY
    ,RXF_QTY_DISPENSED
    ,RXF_METRIC_QTY_DISPENSED
    ,RXF_INTENDED_QTY_DISPENSED
    ,RXF_INTENDED_DAYS_SUPPLY
    ,RXF_IS_CENTRAL_FILL
    ,RXF_SELECTED_UPC
    ,RXF_RTP_DATE
    ,RXF_READY_DATE
    ,RXF_IS_340B
    ,RXF_IS_CASH
    ,RXF_SKIP_PRE_VER_CODE

    ,ADS_ACTUAL_SHIPPING_METHOD
    ,ADS_ACTUAL_SHIP_CHARGE
    ,ADS_ACTUAL_SHIP_DATE
    ,ADS_ESTIMATED_DELIVERY_DATE
    ,ADS_TOTE_LICENSE_PLATE
    ,ADS_MANIFEST_DELIVERY_DATE
    ,ADS_EXCEPTION_TEXT
    ,ADS_external_order_number
    ,ADS_reject_code
    ,ADS_qty_dispensed
    ,ADS_number_of_vials
    ,ADS_package_size
    ,ADS_date_completed
    ,ADS_date_filled
    ,ADS_ndc_requested
    ,ADS_ndc_dispensed
    ,ADS_qc_initials
    ,ADS_is_easy_open_cap
    ,ADS_status_code
    ,ADS_status_desc
    ,ADS_date_updated
    ,ADS_datestamp
    
    ,MAN_CHECK_OUT_DATESTAMP
    ,MAN_CHECK_IN_DATESTAMP
    ,MAN_DATESTAMP
    ,MAN_IS_PULLED_BACK
FROM (
  SELECT
	 FILL_STATE_CHG_TS
	,FILL_STATE_CHG_TS_DATE
	,FILL_STATE_CHG_TS_datetime
	,store_num
	,Store_NPI
	,Store_NCPDP
	,Facility_NPI
	,rx_num
	,rx_num_txt
	,REFILL_NUM
	,PARTIAL_FILL_SEQ
	,benefit_group_id
	,date_of_service
	,daw_code
	,days_supply
	,dispensed_drug_num
	,dispensed_qty_awp
	,facility_num
	,fill_state_code
	,fill_state_key
	,fill_state_price_num
	,final_price
	,incentive_fee_amount
	,internal_fill_id
	,is_same_day_reversal
	,medical_condition_num
	,original_product_qty
	,patient_num
	,patient_pay_amount
	,prescribed_date
	,prescriber_address_num
	,prescriber_id_dea
	,prescriber_id_npi
	,prescriber_num
	,qty_dispensed
	,refills_authorized
	,refills_remaining
	,rx_fill_seq
	,rx_origin_code
	,rx_record_num
	,short_fill_status
	,split_billing_code
	,tlog_sequence_num
	,total_cost
	,total_fee
	,total_uandc
	,total_user_defined
	,tp_curr_patient_pay_amount
	,tp_curr_total_cost
	,tp_curr_total_fee
	,tp_item_claim_key
	,tp_item_claim_num
	,tp_message_key
	,tp_patient_num
	,tp_plan_amount_paid
	,tp_plan_num
	,tp_prior_auth_num
	,tp_processor_dest_num
	,transaction_code
	,IS_BAR
	,order_num
	,FS_FILL_STATE_CHG_TS_datetime
	,fs_fill_state_code
	,fs_rx_fill_seq
	,fs_rx_record_num
	,ingredient_ndc1
	,ingredient_ndc2
	,ingredient_ndc3
	,ingredient_ndc4
	,ingredient_ndc5
	,ingredient_ndc6
	,ingredient_price1
	,ingredient_price2
	,ingredient_price3
	,ingredient_price4
	,ingredient_price5
	,ingredient_price6
	,ingredient_qty1
	,ingredient_qty2
	,ingredient_qty3
	,ingredient_qty4
	,ingredient_qty5
	,ingredient_qty6
	,authorization_number
	,emergency_condition_code
	,external_billing_indicator
	,h_level        
	,tp_message_num
	,bin_number
	,processor_control_number
	,drug_gcn_seq
	,drug_is_maint_drug
	,drug_ndc
	,drug_package_size
	,drug_strength
	,drug_strength_units
	,mcd_frmttd_cond_ref_num
	,prs_prescriber_key
	,prs_prescriber_num
	,padr_key
	,prd_is_compound
	,prd_is_generic
	,prd_is_price_maintained
	,prd_name
	,prd_package_size
	,pd_patient_key
	,tpd_cardholder_id
	,tpd_cardholder_patient_num
	,tpd_relationship_num
	,tpd_tp_plan_num
	,tpld_plan_code
	,pcn
	,pcr_tp_plan_amount_paid
	,pcr_cash_tp_indicator
	,pcr_fill_state_price_num
	,pcr_total_cost
	,pcr_patient_pay_amount
	,pcr_total_fee
	,pcr_final_price
	,pcr_dispensed_qty_awp
	,pcr_dispensing_fee_amount
	,pcr_total_user_defined
	,pcr2_tp_plan_amount_paid
	,target_cf_facility_num
	,PICKUP_datetime
	,FACILITY_ORDER_NUMBER
	,rq_tpmsg_key
	,rq_tp_message_num
	,rq_tp_item_claim_num
	,rq_tp_processor_dest_seq
	,rq_tp_processor_num
	,rq_tpmsg_submission_datetime
	,rq_tpmsg_submission_status
	,rq_tpmsg_msg_category
	,rq_tpmsg_comm_failure
	,RQ_BIN_NUM
	,RQ_VERSION_RELEASE_NUM
	,RQ_TRANSACTION_CODE
	,RQ_PROCESSOR_CONTROL_NUM
	,RQ_TRANSACTION_COUNT
	,RQ_SERVICE_PROVIDER_ID_QUAL
	,RQ_SERVICE_PROVIDER_ID
	,RQ_DATE_OF_SERVICE
	,RQ_SOFTWARE_CERT_ID
	,RQ_RX_SERVICE_REF_QUAL
	,RQ_RX_SERVICE_REF_NUM
	,RQ_PRODUCT_SERVICE_ID_QUAL
	,RQ_PRODUCT_SERVICE_ID
	,RQ_QUANTITY_DISPENSED
	,RQ_FILL_NUMBER
	,RQ_DAYS_SUPPLY
	,RQ_COMPOUND_CODE
	,RQ_DAW_CODE
	,RQ_RX_WRITTEN_DATE
	,RQ_NUM_REFILLS_AUTHORIZED
	,RQ_RX_ORIGIN_CODE
	,RQ_SUBMISSION_CLARIFICATION
	,RQ_OTHER_COVERAGE_CODE
	,RQ_ORIG_RX_PROD_SVC_ID_QUAL
	,RQ_ORIG_RX_PROD_SVC_CODE
	,RQ_ORIG_RX_QTY
	,RQ_SCHEDULED_PRESCRIPTION_ID
	,RQ_UNIT_OF_MEASURE
	,RQ_PRIOR_AUTH_NUM_SUBMITTED
	,RQ_INTERMEDIARY_AUTH_ID
	,RQ_PHARMACY_SERVICE_TYPE
	,RQ_CARDHOLDER_ID
	,RQ_GROUP_ID
	,RQ_PERSON_CODE
	,RQ_PATIENT_RELATIONSHIP_CODE
	,RQ_PRESCRIBER_ID_QUAL
	,RQ_PRESCRIBER_ID
	,RQ_PRESCRIBER_LNAME
	,RQ_PRESCRIBER_FIRST_NAME
	,RQ_PRESCRIBER_STATE_ADDRESS
	,RQ_INGREDIENT_COST_SUBMITTED
	,RQ_DISPENSING_FEE_SUBMITTED
	,RQ_UANDC_CHARGE
	,RQ_GROSS_AMT_DUE
	,RQ_BASIS_OF_COST_DETERMINATION
	,rs_tpmsg_key
	,rs_tp_message_num
	,rs_tp_item_claim_num
	,rs_tp_processor_destinatn_seq
	,rs_tp_processor_num
	,rs_tpmsg_submission_datetime
	,rs_tpmsg_submission_status
	,rs_tpmsg_msg_category
	,rs_tpmsg_comm_failure
	,rs_version_release_num
	,rs_transaction_code
	,rs_transaction_count
	,rs_header_response_status
	,rs_svc_provider_id_qual
	,rs_svc_provider_id
	,rs_date_of_service
	,rs_transaction_rsp_status
	,rs_authorization_num
	,rs_approved_msg_code_count
	,rs_approved_msg_code1
	,rs_approved_msg_code2
	,rs_addtl_message_info
	,rs_rx_svc_ref_num
	,rs_group_id
	,rs_netwrk_reimbursement_id
	,rs_patient_pay_amt
	,rs_ingredient_cost_pd
	,rs_dispensing_fee_pd
	,rs_tax_exempt_ind
	,rs_flat_sales_tax_amt_pd
	,rs_pct_sales_tax_amt_pd
	,rs_pct_sales_tax_rate_pd
	,rs_pct_sales_tax_basis_pd
	,rs_incentive_fee_pd
	,rs_prof_service_fee_pd
	,rs_total_amt_pd
	,rs_basis_of_reimbrsmt_det
	,rs_amt_attr_to_sales_tax
	,rs_accumulated_deduct_amt
	,rs_remaining_deduct_amt
	,rs_remaining_benefit_amt
	,rs_amt_applied_to_per_deduct
	,rs_amt_of_copay
	,rs_amt_attrib_to_prod_select
	,rs_amt_exceed_per_benefit_max
	,rs_basis_of_calc_disp_fee
	,rs_basis_of_calc_copay
	,rs_basis_of_calc_flt_sales_tax
	,rs_basis_of_calc_pct_sales_tax
	,rs_actual_dispense_fee
	,rs_actual_tax_exempt_code
	,rs_actual_flat_sales_tax_amt
	,rs_actual_sales_tax_percent
	,rs_actual_sales_tax_rate
	,rs_actual_sales_tax_basis
	,rs_actual_incentive_amt
	,rs_actual_other_amt
	,rs_actual_sales_tax_amt
	,rs_actual_copay_amt
	,rs_processor_fee_amt
	,rs_patient_sales_tax_amount
	,rs_plan_sales_tax_amount
	,rs_coinsurance_amt
	,ship_num
	,ship_store_num
	,ship_pd_patient_key
	,ship_pad_key
	,ship_date
	,ship_datetime
	,ship_orig_exp_deliv_date
	,ship_orig_exp_deliv_datetime
	,ship_exp_deliv_date
	,ship_exp_deliv_datetime
	,ship_facility_order_number
	,ship_item_count
	,ship_package_count
	,ship_dea_class
	,ship_refrigeration
	,ship_weight
	,ship_weight_units
	,ship_tracking_number
	,ship_handle_fee
	,ship_courier_charge
	,ship_is_complete
	,ship_orig_method
	,ship_method
	,ship_orig_courier_name
	,ship_courier_name
	,ship_delivery_method
	,sig_required
    
    --NEW FIELDS ARE BELOW HERE    
	,is_change_status
    ,item_seq

    ,RXF_DISPENSE_DATE
    ,RXF_DAYS_SUPPLY
    ,RXF_QTY_DISPENSED
    ,RXF_METRIC_QTY_DISPENSED
    ,RXF_INTENDED_QTY_DISPENSED
    ,RXF_INTENDED_DAYS_SUPPLY
    ,RXF_IS_CENTRAL_FILL
    ,RXF_SELECTED_UPC
    ,RXF_RTP_DATE
    ,RXF_READY_DATE
    ,RXF_IS_340B
    ,RXF_IS_CASH
    ,RXF_SKIP_PRE_VER_CODE

    ,ADS_ACTUAL_SHIPPING_METHOD
    ,ADS_ACTUAL_SHIP_CHARGE
    ,ADS_ACTUAL_SHIP_DATE
    ,ADS_ESTIMATED_DELIVERY_DATE
    ,ADS_TOTE_LICENSE_PLATE
    ,ADS_MANIFEST_DELIVERY_DATE
    ,ADS_EXCEPTION_TEXT
    ,ADS_external_order_number
    ,ADS_reject_code
    ,ADS_qty_dispensed
    ,ADS_number_of_vials
    ,ADS_package_size
    ,ADS_date_completed
    ,ADS_date_filled
    ,ADS_ndc_requested
    ,ADS_ndc_dispensed
    ,ADS_qc_initials
    ,ADS_is_easy_open_cap
    ,ADS_status_code
    ,ADS_status_desc
    ,ADS_date_updated
    ,ADS_datestamp
    
    ,MAN_CHECK_OUT_DATESTAMP
    ,MAN_CHECK_IN_DATESTAMP
    ,MAN_DATESTAMP
    ,MAN_IS_PULLED_BACK

    ,Rank() OVER (PARTITION BY
                       FILL_STATE_CHG_TS
                      ,fill_state_code
                      ,fill_state_key
                      ,order_num
                      ,item_seq
                  ORDER BY
                       equal_fill_state_price_num Desc
                      ,equal_fill_state_price_num2 Desc
                      ,equal_pd_patient_key Desc
                 ) AS MalarkeyRank
  FROM (
    SELECT
     AUD__Fill_Fact.FILL_STATE_CHG_TS
    ,To_Number(To_Char(AUD__Fill_Fact.FILL_STATE_CHG_TS,'YYYYMMDD')) AS FILL_STATE_CHG_TS_DATE
    ,Round((AUD__Fill_Fact.FILL_STATE_CHG_TS - To_Date('20350101','YYYYMMDD')),6) AS FILL_STATE_CHG_TS_datetime
    ,To_Number(DW__Facility.FD_FACILITY_ID) AS store_num
    ,AUD__Tp_Response_Message.RS_SVC_PROVIDER_ID AS Store_NPI
    ,DW__Facility.FD_NCPDP_PROVIDER_ID AS Store_NCPDP
    ,DW__Facility_ID.FD_VALUE AS Facility_NPI
    ,To_Number(REGEXP_REPLACE(AUD__Fill_Fact.RX_NUMBER,'[^0-9]+','')) AS rx_num
    ,AUD__Fill_Fact.RX_NUMBER AS rx_num_txt
    ,AUD__Fill_Fact.REFILL_NUM
    ,AUD__Fill_Fact.PARTIAL_FILL_SEQ
    ,AUD__Fill_Fact.BENEFIT_GROUP_ID
    ,To_Number(To_Char(AUD__Fill_Fact.DATE_OF_SERVICE,'YYYYMMDD')) AS DATE_OF_SERVICE
    ,AUD__Fill_Fact.DAW_CODE
    ,AUD__Fill_Fact.DAYS_SUPPLY
    ,AUD__Fill_Fact.dispensed_drug_num
    ,AUD__Fill_Fact.DISPENSED_QTY_AWP
    ,AUD__Fill_Fact.facility_num
    ,AUD__Fill_Fact.fill_state_code
    ,AUD__Fill_Fact.fill_state_key
    ,AUD__Fill_Fact.fill_state_price_num
    ,AUD__Fill_Fact.final_price
    ,AUD__Fill_Fact.INCENTIVE_FEE_AMOUNT
    ,AUD__Fill_Fact.internal_fill_id
    ,AUD__Fill_Fact.is_same_day_reversal
    ,AUD__Fill_Fact.medical_condition_num
    ,AUD__Fill_Fact.ORIGINAL_PRODUCT_QTY
    ,AUD__Fill_Fact.patient_num
    ,AUD__Fill_Fact.PATIENT_PAY_AMOUNT
    ,To_Number(To_Char(AUD__Fill_Fact.PRESCRIBED_DATE,'YYYYMMDD')) AS PRESCRIBED_DATE
    ,AUD__Fill_Fact.prescriber_address_num
    ,AUD__Fill_Fact.PRESCRIBER_ID_DEA
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Fill_Fact.PRESCRIBER_ID_NPI),1,15) AS PRESCRIBER_ID_NPI
    ,AUD__Fill_Fact.prescriber_num
    ,AUD__Fill_Fact.QTY_DISPENSED
    ,AUD__Fill_Fact.REFILLS_AUTHORIZED
    ,AUD__Fill_Fact.REFILLS_REMAINING
    ,AUD__Fill_Fact.rx_fill_seq
    ,AUD__Fill_Fact.RX_ORIGIN_CODE
    ,AUD__Fill_Fact.rx_record_num
    ,AUD__Fill_Fact.short_fill_status
    ,AUD__Fill_Fact.split_billing_code
    ,AUD__Fill_Fact.TLOG_SEQUENCE_NUM
    ,AUD__Fill_Fact.TOTAL_COST
    ,AUD__Fill_Fact.TOTAL_FEE
    ,AUD__Fill_Fact.total_uandc
    ,AUD__Fill_Fact.TOTAL_USER_DEFINED
    ,AUD__Fill_Fact.tp_curr_patient_pay_amount
    ,AUD__Fill_Fact.tp_curr_total_cost
    ,AUD__Fill_Fact.TP_CURR_TOTAL_FEE
    ,AUD__Fill_Fact.tp_item_claim_key
    ,AUD__Fill_Fact.tp_item_claim_num
    ,AUD__Fill_Fact.tp_message_key
    ,AUD__Fill_Fact.tp_patient_num
    ,AUD__Fill_Fact.TP_PLAN_AMOUNT_PAID
    ,AUD__Fill_Fact.tp_plan_num
    ,AUD__Fill_Fact.tp_prior_auth_num
    ,AUD__Fill_Fact.tp_processor_dest_num
    ,AUD__Fill_Fact.transaction_code
    ,AUD__Fill_Fact.IS_BAR
    ,AUD__Fill_State.order_num
    ,Round((AUD__Fill_State.fill_state_chg_ts - To_Date('20350101','YYYYMMDD')),6) AS FS_FILL_STATE_CHG_TS_datetime
    ,AUD__Fill_State.fill_state_code AS fs_fill_state_code
    ,AUD__Fill_State.rx_fill_seq AS fs_rx_fill_seq
    ,AUD__Fill_State.rx_record_num AS fs_rx_record_num
    ,AUD__Fill_State.INGREDIENT_NDC1
    ,AUD__Fill_State.INGREDIENT_NDC2
    ,AUD__Fill_State.INGREDIENT_NDC3
    ,AUD__Fill_State.INGREDIENT_NDC4
    ,AUD__Fill_State.INGREDIENT_NDC5
    ,AUD__Fill_State.INGREDIENT_NDC6
    ,AUD__Fill_State.INGREDIENT_PRICE1
    ,AUD__Fill_State.INGREDIENT_PRICE2
    ,AUD__Fill_State.INGREDIENT_PRICE3
    ,AUD__Fill_State.INGREDIENT_PRICE4
    ,AUD__Fill_State.INGREDIENT_PRICE5
    ,AUD__Fill_State.INGREDIENT_PRICE6
    ,AUD__Fill_State.INGREDIENT_QTY1
    ,AUD__Fill_State.INGREDIENT_QTY2
    ,AUD__Fill_State.INGREDIENT_QTY3
    ,AUD__Fill_State.INGREDIENT_QTY4
    ,AUD__Fill_State.INGREDIENT_QTY5
    ,AUD__Fill_State.INGREDIENT_QTY6
    ,AUD__Tp_Item_Claim.AUTHORIZATION_NUMBER
    ,AUD__Tp_Item_Claim.EMERGENCY_CONDITION_CODE
    ,AUD__Tp_Item_Claim.external_billing_indicator
    ,AUD__Tp_Message.h_level        
    ,AUD__Tp_Message.tp_message_num
    ,AUD__Tp_Processor_Destination.BIN_NUMBER
    ,AUD__Tp_Processor_Destination.PROCESSOR_CONTROL_NUMBER
    ,DW__Drug.drug_gcn_seq
    ,DW__Drug.DRUG_IS_MAINT_DRUG
    ,DW__Drug.DRUG_NDC
    ,DW__Drug.DRUG_PACKAGE_SIZE
    ,Substr(WEGMANS.Clean_String_ForTenUp(DW__Drug.DRUG_STRENGTH),1,19) AS DRUG_STRENGTH
    ,Substr(WEGMANS.Clean_String_ForTenUp(DW__Drug.DRUG_STRENGTH_UNITS),1,15) AS DRUG_STRENGTH_UNITS
    ,DW__Medical_Condition.MCD_FRMTTD_COND_REF_NUM
    ,DW__Prescriber.PRS_PRESCRIBER_KEY
    ,DW__Prescriber.PRS_PRESCRIBER_NUM
    ,DW__Prescriber_Address.PADR_KEY
    ,DW__Product.prd_is_compound
    ,DW__Product.prd_is_generic
    ,DW__Product.PRD_IS_PRICE_MAINTAINED
    ,Substr(WEGMANS.Clean_String_ForTenUp(DW__Product.PRD_NAME),1,30) AS PRD_NAME
    ,DW__Product.prd_package_size
    ,DW__Tp_Patient.PD_PATIENT_KEY
    ,DW__Tp_Patient.TPD_CARDHOLDER_ID
    ,DW__Tp_Patient.tpd_cardholder_patient_num
    ,DW__Tp_Patient.tpd_relationship_num
    ,DW__Tp_Patient.tpd_tp_plan_num
    ,DW__Tp_Plan.tpld_plan_code
    ,DW__Tp_Plan.PCN
    ,AUD__Price_Calc_Result.tp_plan_amount_paid AS pcr_tp_plan_amount_paid
    ,AUD__Price_Calc_Result.CASH_TP_INDICATOR AS pcr_CASH_TP_INDICATOR
    ,AUD__Price_Calc_Result.fill_state_price_num AS pcr_fill_state_price_num
    ,AUD__Price_Calc_Result.total_cost AS pcr_total_cost
    ,AUD__Price_Calc_Result.patient_pay_amount AS pcr_patient_pay_amount
    ,AUD__Price_Calc_Result.total_fee AS pcr_total_fee
    ,AUD__Price_Calc_Result.final_price AS pcr_final_price
    ,AUD__Price_Calc_Result.dispensed_qty_awp AS pcr_dispensed_qty_awp
    ,AUD__Price_Calc_Result.dispensing_fee_amount AS pcr_dispensing_fee_amount
    ,AUD__Price_Calc_Result.total_user_defined AS pcr_total_user_defined
    ,AUD__Price_Calc_Result2.tp_plan_amount_paid AS pcr2_tp_plan_amount_paid
    ,AUD__Item.target_cf_facility_num
    ,Round((AUD__Item.PICKUP_TIME - To_Date('20350101','YYYYMMDD')),6) AS PICKUP_datetime
    ,AUD__Order_Record.FACILITY_ORDER_NUMBER
    ,AUD__Tp_Request_Message.TPMSG_KEY AS rq_tpmsg_key
    ,AUD__Tp_Request_Message.TP_MESSAGE_NUM AS rq_tp_message_num
    ,AUD__Tp_Request_Message.TP_ITEM_CLAIM_NUM AS rq_tp_item_claim_num
    ,AUD__Tp_Request_Message.TP_PROCESSOR_DESTINATION_SEQ AS rq_tp_processor_dest_seq
    ,AUD__Tp_Request_Message.TP_PROCESSOR_NUM AS rq_tp_processor_num
    ,Round((AUD__Tp_Request_Message.TPMSG_SUBMISSION_DATE - To_Date('20350101','YYYYMMDD')),6) AS rq_tpmsg_submission_datetime
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Request_Message.TPMSG_SUBMISSION_STATUS),1,1) AS rq_tpmsg_submission_status
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Request_Message.TPMSG_MSG_CATEGORY),1,1) AS rq_tpmsg_msg_category
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Request_Message.TPMSG_COMM_FAILURE),1,1) AS rq_tpmsg_comm_failure
    ,AUD__Tp_Request_Message.RQ_BIN_NUM
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Request_Message.RQ_VERSION_RELEASE_NUM),1,2) AS RQ_VERSION_RELEASE_NUM
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Request_Message.RQ_TRANSACTION_CODE),1,2) AS RQ_TRANSACTION_CODE
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Request_Message.RQ_PROCESSOR_CONTROL_NUM),1,10) AS RQ_PROCESSOR_CONTROL_NUM
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Request_Message.RQ_TRANSACTION_COUNT),1,1) AS RQ_TRANSACTION_COUNT
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Request_Message.RQ_SERVICE_PROVIDER_ID_QUAL),1,2) AS RQ_SERVICE_PROVIDER_ID_QUAL
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Request_Message.RQ_SERVICE_PROVIDER_ID),1,10) AS RQ_SERVICE_PROVIDER_ID
    ,To_Number(To_Char(AUD__Tp_Request_Message.RQ_DATE_OF_SERVICE,'YYYYMMDD')) AS RQ_DATE_OF_SERVICE    
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Request_Message.RQ_SOFTWARE_CERT_ID),1,10) AS RQ_SOFTWARE_CERT_ID
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Request_Message.RQ_RX_SERVICE_REF_QUAL),1,1) AS RQ_RX_SERVICE_REF_QUAL
    ,AUD__Tp_Request_Message.RQ_RX_SERVICE_REF_NUM
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Request_Message.RQ_PRODUCT_SERVICE_ID_QUAL),1,2) AS RQ_PRODUCT_SERVICE_ID_QUAL
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Request_Message.RQ_PRODUCT_SERVICE_ID),1,11) AS RQ_PRODUCT_SERVICE_ID
    ,AUD__Tp_Request_Message.RQ_QUANTITY_DISPENSED
    ,AUD__Tp_Request_Message.RQ_FILL_NUMBER
    ,AUD__Tp_Request_Message.RQ_DAYS_SUPPLY
    ,AUD__Tp_Request_Message.RQ_COMPOUND_CODE
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Request_Message.RQ_DAW_CODE),1,1) AS RQ_DAW_CODE
    ,To_Number(To_Char(AUD__Tp_Request_Message.RQ_RX_WRITTEN_DATE,'YYYYMMDD')) AS RQ_RX_WRITTEN_DATE    
    ,AUD__Tp_Request_Message.RQ_NUM_REFILLS_AUTHORIZED
    ,AUD__Tp_Request_Message.RQ_RX_ORIGIN_CODE
    ,AUD__Tp_Request_Message.RQ_SUBMISSION_CLARIFICATION
    ,AUD__Tp_Request_Message.RQ_OTHER_COVERAGE_CODE
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Request_Message.RQ_ORIG_RX_PROD_SVC_ID_QUAL),1,2) AS RQ_ORIG_RX_PROD_SVC_ID_QUAL
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Request_Message.RQ_ORIG_RX_PROD_SVC_CODE),1,11) AS RQ_ORIG_RX_PROD_SVC_CODE
    ,AUD__Tp_Request_Message.RQ_ORIG_RX_QTY
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Request_Message.RQ_SCHEDULED_PRESCRIPTION_ID),1,12) AS RQ_SCHEDULED_PRESCRIPTION_ID
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Request_Message.RQ_UNIT_OF_MEASURE),1,2) AS RQ_UNIT_OF_MEASURE
    ,AUD__Tp_Request_Message.RQ_PRIOR_AUTH_NUM_SUBMITTED
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Request_Message.RQ_INTERMEDIARY_AUTH_ID),1,11) AS RQ_INTERMEDIARY_AUTH_ID
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Request_Message.PHARMACY_SERVICE_TYPE),1,1) AS RQ_PHARMACY_SERVICE_TYPE
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Request_Message.RQ_CARDHOLDER_ID),1,20) AS RQ_CARDHOLDER_ID
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Request_Message.RQ_GROUP_ID),1,15) AS RQ_GROUP_ID
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Request_Message.RQ_PERSON_CODE),1,3) AS RQ_PERSON_CODE
    ,AUD__Tp_Request_Message.RQ_PATIENT_RELATIONSHIP_CODE
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Request_Message.RQ_PRESCRIBER_ID_QUAL),1,2) AS RQ_PRESCRIBER_ID_QUAL
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Request_Message.RQ_PRESCRIBER_ID),1,13) AS RQ_PRESCRIBER_ID
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Request_Message.RQ_PRESCRIBER_LNAME),1,15) AS RQ_PRESCRIBER_LNAME
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Request_Message.PRESCRIBER_FIRST_NAME),1,12) AS RQ_PRESCRIBER_FIRST_NAME
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Request_Message.PRESCRIBER_STATE_ADDRESS),1,2) AS RQ_PRESCRIBER_STATE_ADDRESS
    ,AUD__Tp_Request_Message.RQ_INGREDIENT_COST_SUBMITTED
    ,AUD__Tp_Request_Message.RQ_DISPENSING_FEE_SUBMITTED
    ,AUD__Tp_Request_Message.RQ_UANDC_CHARGE
    ,AUD__Tp_Request_Message.RQ_GROSS_AMT_DUE
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Request_Message.RQ_BASIS_OF_COST_DETERMINATION),1,2) AS RQ_BASIS_OF_COST_DETERMINATION
    ,AUD__Tp_Response_Message.TPMSG_KEY AS rs_tpmsg_key
    ,AUD__Tp_Response_Message.TP_MESSAGE_NUM AS rs_tp_message_num
    ,AUD__Tp_Response_Message.TP_ITEM_CLAIM_NUM AS rs_tp_item_claim_num
    ,AUD__Tp_Response_Message.TP_PROCESSOR_DESTINATION_SEQ AS rs_tp_processor_destinatn_seq
    ,AUD__Tp_Response_Message.TP_PROCESSOR_NUM AS rs_tp_processor_num
    ,Round((AUD__Tp_Response_Message.TPMSG_SUBMISSION_DATE - To_Date('20350101','YYYYMMDD')),6) AS rs_tpmsg_submission_datetime    
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Response_Message.TPMSG_SUBMISSION_STATUS),1,1) AS rs_tpmsg_submission_status
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Response_Message.TPMSG_MSG_CATEGORY),1,1) AS rs_tpmsg_msg_category
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Response_Message.TPMSG_COMM_FAILURE),1,1) AS rs_tpmsg_comm_failure
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Response_Message.RS_VERSION_RELEASE_NUM),1,2) AS rs_version_release_num
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Response_Message.RS_TRANSACTION_CODE),1,2) AS rs_transaction_code
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Response_Message.RS_TRANSACTION_COUNT),1,1) AS rs_transaction_count
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Response_Message.RS_HEADER_RESPONSE_STATUS),1,1) AS rs_header_response_status
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Response_Message.RS_SVC_PROVIDER_ID_QUAL),1,2) AS rs_svc_provider_id_qual
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Response_Message.RS_SVC_PROVIDER_ID),1,10) AS rs_svc_provider_id
    ,To_Number(To_Char(AUD__Tp_Response_Message.RS_DATE_OF_SERVICE,'YYYYMMDD')) AS rs_date_of_service    
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Response_Message.RS_TRANSACTION_RSP_STATUS),1,1) AS rs_transaction_rsp_status
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Response_Message.RS_AUTHORIZATION_NUM),1,20) AS rs_authorization_num
    ,AUD__Tp_Response_Message.RS_APPROVED_MSG_CODE_COUNT AS rs_approved_msg_code_count
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Response_Message.RS_APPROVED_MSG_CODE1),1,3) AS rs_approved_msg_code1
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Response_Message.RS_APPROVED_MSG_CODE2),1,3) AS rs_approved_msg_code2
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Response_Message.RS_ADDTL_MESSAGE_INFO),1,100) AS rs_addtl_message_info
    ,AUD__Tp_Response_Message.RS_RX_SVC_REF_NUM AS rs_rx_svc_ref_num
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Response_Message.RS_GROUP_ID),1,15) AS rs_group_id
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Response_Message.RS_NETWRK_REIMBURSEMENT_ID),1,10) AS rs_netwrk_reimbursement_id
    ,AUD__Tp_Response_Message.RS_PATIENT_PAY_AMT AS rs_patient_pay_amt
    ,AUD__Tp_Response_Message.RS_INGREDIENT_COST_PD AS rs_ingredient_cost_pd
    ,AUD__Tp_Response_Message.RS_DISPENSING_FEE_PD AS rs_dispensing_fee_pd
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Response_Message.RS_TAX_EXEMPT_IND),1,1) AS rs_tax_exempt_ind
    ,AUD__Tp_Response_Message.RS_FLAT_SALES_TAX_AMT_PD AS rs_flat_sales_tax_amt_pd
    ,AUD__Tp_Response_Message.RS_PCT_SALES_TAX_AMT_PD AS rs_pct_sales_tax_amt_pd
    ,AUD__Tp_Response_Message.RS_PCT_SALES_TAX_RATE_PD AS rs_pct_sales_tax_rate_pd
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Response_Message.RS_PCT_SALES_TAX_BASIS_PD),1,2) AS rs_pct_sales_tax_basis_pd
    ,AUD__Tp_Response_Message.RS_INCENTIVE_FEE_PD AS rs_incentive_fee_pd
    ,AUD__Tp_Response_Message.RS_PROF_SERVICE_FEE_PD AS rs_prof_service_fee_pd
    ,AUD__Tp_Response_Message.RS_TOTAL_AMT_PD AS rs_total_amt_pd
    ,AUD__Tp_Response_Message.RS_BASIS_OF_REIMBRSMT_DET AS rs_basis_of_reimbrsmt_det
    ,AUD__Tp_Response_Message.RS_AMT_ATTR_TO_SALES_TAX AS rs_amt_attr_to_sales_tax
    ,AUD__Tp_Response_Message.RS_ACCUMULATED_DEDUCT_AMT AS rs_accumulated_deduct_amt
    ,AUD__Tp_Response_Message.RS_REMAINING_DEDUCT_AMT AS rs_remaining_deduct_amt
    ,AUD__Tp_Response_Message.RS_REMAINING_BENEFIT_AMT AS rs_remaining_benefit_amt
    ,AUD__Tp_Response_Message.RS_AMT_APPLIED_TO_PER_DEDUCT AS rs_amt_applied_to_per_deduct
    ,AUD__Tp_Response_Message.RS_AMT_OF_COPAY AS rs_amt_of_copay
    ,AUD__Tp_Response_Message.RS_AMT_ATTRIB_TO_PROD_SELECT AS rs_amt_attrib_to_prod_select
    ,AUD__Tp_Response_Message.RS_AMT_EXCEED_PER_BENEFIT_MAX AS rs_amt_exceed_per_benefit_max
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Response_Message.RS_BASIS_OF_CALC_DISP_FEE),1,2) AS rs_basis_of_calc_disp_fee
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Response_Message.RS_BASIS_OF_CALC_COPAY),1,2) AS rs_basis_of_calc_copay
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Response_Message.RS_BASIS_OF_CALC_FLT_SALES_TAX),1,2) AS rs_basis_of_calc_flt_sales_tax
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Response_Message.RS_BASIS_OF_CALC_PCT_SALES_TAX),1,2) AS rs_basis_of_calc_pct_sales_tax
    ,AUD__Tp_Response_Message.ACTUAL_DISPENSE_FEE AS rs_actual_dispense_fee
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Response_Message.ACTUAL_TAX_EXEMPT_CODE),1,10) AS rs_actual_tax_exempt_code
    ,AUD__Tp_Response_Message.ACTUAL_FLAT_SALES_TAX_AMT AS rs_actual_flat_sales_tax_amt
    ,AUD__Tp_Response_Message.ACTUAL_SALES_TAX_PERCENT AS rs_actual_sales_tax_percent
    ,AUD__Tp_Response_Message.ACTUAL_SALES_TAX_RATE AS rs_actual_sales_tax_rate
    ,Substr(WEGMANS.Clean_String_ForTenUp(AUD__Tp_Response_Message.ACTUAL_SALES_TAX_BASIS),1,10) AS rs_actual_sales_tax_basis
    ,AUD__Tp_Response_Message.ACTUAL_INCENTIVE_AMT AS rs_actual_incentive_amt
    ,AUD__Tp_Response_Message.ACTUAL_OTHER_AMT AS rs_actual_other_amt
    ,AUD__Tp_Response_Message.ACTUAL_SALES_TAX_AMT AS rs_actual_sales_tax_amt
    ,AUD__Tp_Response_Message.ACTUAL_COPAY_AMT AS rs_actual_copay_amt
    ,AUD__Tp_Response_Message.PROCESSOR_FEE_AMT AS rs_processor_fee_amt
    ,AUD__Tp_Response_Message.PATIENT_SALES_TAX_AMOUNT AS rs_patient_sales_tax_amount
    ,AUD__Tp_Response_Message.PLAN_SALES_TAX_AMOUNT AS rs_plan_sales_tax_amount
    ,AUD__Tp_Response_Message.COINSURANCE_AMT AS rs_coinsurance_amt
    ,DW__Shipment_Fact.shipment_num AS ship_num
    ,To_Number(DW__Shipment_Fact_Facility.FD_FACILITY_ID) AS ship_store_num
    ,DW__Shipment_Fact.pd_patient_key AS ship_pd_patient_key
    ,DW__Shipment_Fact.pad_key AS ship_pad_key
    ,(CASE WHEN DW__Shipment_Fact.ship_date_key = 0 THEN NULL ELSE DW__Shipment_Fact.ship_date_key END) AS ship_date
    ,(CASE WHEN DW__Shipment_Fact.ship_date_key = 0 THEN NULL
           ELSE Round(((To_date(To_Char(DW__Shipment_Fact.ship_date_key),'YYYYMMDD') + ((DW__Shipment_Fact.SHIP_TIME_KEY - 2)/86400)) - To_Date('20350101','YYYYMMDD')),6)
      END) AS ship_datetime
    ,(CASE WHEN DW__Shipment_Fact.ORIG_EXPECT_DELIVERY_DATE_KEY = 0 THEN NULL ELSE DW__Shipment_Fact.ORIG_EXPECT_DELIVERY_DATE_KEY END) AS ship_orig_exp_deliv_date
    ,(CASE WHEN DW__Shipment_Fact.ORIG_EXPECT_DELIVERY_DATE_KEY = 0 THEN NULL
           ELSE Round(((To_date(To_Char(DW__Shipment_Fact.ORIG_EXPECT_DELIVERY_DATE_KEY),'YYYYMMDD') + ((DW__Shipment_Fact.ORIG_EXPECT_DELIVERY_TIME_KEY - 2)/86400)) - To_Date('20350101','YYYYMMDD')),6)
      END) AS ship_orig_exp_deliv_datetime
    ,(CASE WHEN DW__Shipment_Fact.EXPECTED_DELIVERY_DATE_KEY = 0 THEN NULL ELSE DW__Shipment_Fact.EXPECTED_DELIVERY_DATE_KEY END) AS ship_exp_deliv_date
    ,(CASE WHEN DW__Shipment_Fact.EXPECTED_DELIVERY_DATE_KEY = 0 THEN NULL
           ELSE Round(((To_date(To_Char(DW__Shipment_Fact.EXPECTED_DELIVERY_DATE_KEY),'YYYYMMDD') + ((DW__Shipment_Fact.EXPECTED_DELIVERY_TIME_KEY - 2)/86400)) - To_Date('20350101','YYYYMMDD')),6)
      END) AS ship_exp_deliv_datetime
    ,DW__Shipment_Fact.facility_order_number AS ship_facility_order_number
    ,DW__Shipment_Fact.ship_item_count
    ,DW__Shipment_Fact.package_count AS ship_package_count
    ,DW__Shipment_Fact.dea_class AS ship_dea_class
    ,DW__Shipment_Fact.refrigeration AS ship_refrigeration
    ,DW__Shipment_Fact.weight AS ship_weight
    ,Substr(WEGMANS.Clean_String_ForTenUp(DW__Shipment_Fact.weight_units),1,10) AS ship_weight_units
    ,Substr(WEGMANS.Clean_String_ForTenUp(DW__Shipment_Fact.tracking_number),1,100) AS ship_tracking_number
    ,DW__Shipment_Fact.ship_handle_fee
    ,DW__Shipment_Fact.courier_ship_charge AS ship_courier_charge
    ,DW__Shipment_Fact.is_shipping_complete AS ship_is_complete
    ,DW__Courier_Shipment_Type_Orig.shipping_method_desc AS ship_orig_method
    ,DW__Courier_Shipment_Type.shipping_method_desc AS ship_method
    ,DW__Courier_Shipment_Type_Orig.courier_name AS ship_orig_courier_name
    ,DW__Courier_Shipment_Type.courier_name AS ship_courier_name
    ,DW__Delivery_Method_Type.dm_delivery_method_desc AS ship_delivery_method
	,AUD__Order_Record.IS_SIGNATURE_REQUIRED AS sig_required
    
    --NEW FIELDS ARE BELOW HERE
	,AUD__Fill_Fact.is_change_status --VARCHAR2(2)
    ,AUD__Fill_State.item_seq --NUMBER(18)

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
                       AUD__Fill_Fact.FILL_STATE_CHG_TS
                      ,AUD__Fill_Fact.fill_state_code
                      ,AUD__Fill_Fact.fill_state_key
                      ,AUD__Item.order_num
                      ,AUD__Item.item_seq
                  ORDER BY
                       AUD__Ads_Response.eff_start_date Desc
                      ,AUD__Ads_Response.rowid Desc
                      ,AUD__Manifest_Item.eff_start_date Desc
                      ,AUD__Manifest_Item.rowid Desc
                 ) AS CentralFillRank

    ,(CASE WHEN AUD__Price_Calc_Result.fill_state_price_num = AUD__Fill_State.fill_state_price_num
           THEN 1 ELSE 0 END) AS equal_fill_state_price_num
    ,(CASE WHEN AUD__Price_Calc_Result2.fill_state_price_num = AUD__Fill_State.fill_state_price_num
           THEN 1 ELSE 0 END) AS equal_fill_state_price_num2
    ,(CASE WHEN DW__Tp_Patient.pd_patient_key = DW__Shipment_fact.pd_patient_key
           THEN 1 ELSE 0 END) AS equal_pd_patient_key

FROM TREXONE_AUD_DATA.Fill_Fact AUD__Fill_Fact
     INNER JOIN TREXONE_DW_DATA.Facility DW__Facility
        ON DW__Facility.fd_facility_num = AUD__Fill_Fact.facility_num
       AND DW__Facility.FD_IS_PPI_ENABLED = 'Y'
     INNER JOIN (
         SELECT FD_VALUE, fd_facility_num FROM (
         SELECT FD_VALUE, fd_facility_num,
             Rank() OVER (PARTITION BY fd_facility_num
                          ORDER BY FD_NUM_UPDATES Desc) AS NPI_Rank
         FROM TREXONE_DW_DATA.Facility_ID
         WHERE fd_type = 'F08'
         ) WHERE NPI_Rank = 1
        ) DW__Facility_ID
        ON DW__Facility_ID.fd_facility_num = AUD__Fill_Fact.facility_num
     INNER JOIN TREXONE_AUD_DATA.Fill_State AUD__Fill_State
        ON AUD__Fill_State.h_level = AUD__Fill_Fact.fill_state_key
       AND AUD__Fill_State.rx_record_num = AUD__Fill_Fact.rx_record_num
       AND AUD__Fill_State.rx_fill_seq = AUD__Fill_Fact.rx_fill_seq
     INNER JOIN TREXONE_DW_DATA.Product DW__Product
        ON DW__Product.prd_product_num = AUD__Fill_Fact.dispensed_prod_num
     INNER JOIN TREXONE_DW_DATA.Drug DW__Drug
        ON DW__Drug.drug_num = AUD__Fill_Fact.dispensed_drug_num
     INNER JOIN TREXONE_DW_DATA.Prescriber DW__Prescriber
        ON DW__Prescriber.prs_prescriber_num = AUD__Fill_Fact.prescriber_num
     LEFT OUTER JOIN TREXONE_DW_DATA.Prescriber_Address DW__Prescriber_Address
        ON DW__Prescriber_Address.padr_prescriber_address_num = AUD__Fill_Fact.prescriber_address_num
     --INNER JOIN TREXONE_AUD_DATA.Patient AUD__Patient
     --   ON AUD__Patient.patient_num = AUD__Fill_Fact.patient_num
     --  AND AUD__Patient.eff_end_date = TO_DATE('12/31/2999', 'MM/DD/YYYY')
     --LEFT OUTER JOIN TREXONE_AUD_DATA.Patient_Address AUD__Patient_Address
     --   ON AUD__Patient_Address.patient_num = AUD__Fill_Fact.patient_num
     LEFT OUTER JOIN TREXONE_DW_DATA.Tp_Patient DW__Tp_Patient
        ON DW__Tp_Patient.tpd_tp_patient_num = AUD__Fill_Fact.tp_patient_num
       AND DW__Tp_Patient.tpd_eff_end_date = TO_DATE('12/31/2999', 'MM/DD/YYYY')
     LEFT OUTER JOIN TREXONE_AUD_DATA.Tp_Message AUD__Tp_Message
        ON AUD__Tp_Message.h_level = AUD__Fill_Fact.tp_message_key
       AND AUD__Tp_Message.tp_item_claim_num = AUD__Fill_Fact.tp_item_claim_num
       AND AUD__Tp_Message.eff_end_date = TO_DATE('12/31/2999', 'MM/DD/YYYY')
     LEFT OUTER JOIN TREXONE_DW_DATA.Medical_Condition DW__Medical_Condition
        ON DW__Medical_Condition.mcd_med_condition_num = AUD__Fill_Fact.medical_condition_num
     LEFT OUTER JOIN TREXONE_DW_DATA.Tp_Plan DW__Tp_Plan
        ON DW__Tp_Plan.tpld_plan_num = AUD__Fill_Fact.tp_plan_num
     LEFT OUTER JOIN TREXONE_AUD_DATA.Tp_Prior_Auth AUD__Tp_Prior_Auth
        ON AUD__Tp_Prior_Auth.tp_prior_auth_num = AUD__Fill_Fact.tp_prior_auth_num
       AND AUD__Tp_Prior_Auth.eff_end_date = TO_DATE('12/31/2999', 'MM/DD/YYYY')
     LEFT OUTER JOIN TREXONE_AUD_DATA.Tp_Processor_Destination AUD__Tp_Processor_Destination
        ON AUD__Tp_Processor_Destination.tp_processor_destination_seq = AUD__Fill_Fact.tp_processor_dest_num
       AND AUD__Tp_Processor_Destination.eff_end_date = TO_DATE('12/31/2999', 'MM/DD/YYYY')
     LEFT OUTER JOIN TREXONE_AUD_DATA.Tp_Item_Claim AUD__Tp_Item_Claim
        ON AUD__Tp_Item_Claim.tp_item_claim_num = AUD__Fill_Fact.tp_item_claim_num
       AND AUD__Tp_Item_Claim.h_level = AUD__Fill_Fact.tp_item_claim_key
       AND AUD__Tp_Item_Claim.eff_end_date = TO_DATE('12/31/2999', 'MM/DD/YYYY')
     LEFT OUTER JOIN TREXONE_AUD_DATA.Tp_Request_Message AUD__Tp_Request_Message
        ON AUD__Tp_Request_Message.tp_message_num = AUD__Tp_Message.tp_message_num
       AND AUD__Tp_Request_Message.h_level = AUD__Tp_Message.h_level
     LEFT OUTER JOIN TREXONE_AUD_DATA.Tp_Response_Message AUD__Tp_Response_Message
        ON AUD__Tp_Response_Message.tp_message_num = AUD__Tp_Message.tp_message_num
       AND AUD__Tp_Response_Message.h_level = AUD__Tp_Message.h_level
     LEFT OUTER JOIN TREXONE_AUD_DATA.Price_Calc_Result AUD__Price_Calc_Result
        ON AUD__Price_Calc_Result.h_level = AUD__Fill_Fact.current_claim_key
     LEFT OUTER JOIN 
        --TREXONE_AUD_DATA.Price_Calc_Result
        (select h_level, tp_plan_amount_paid, fill_state_price_num
         from TREXONE_AUD_DATA.Price_Calc_Result) AUD__Price_Calc_Result2
        ON AUD__Price_Calc_Result2.h_level = AUD__Fill_Fact.current_key
--       AND AUD__Price_Calc_Result2.fill_state_price_num = AUD__Fill_State.fill_state_price_num
     LEFT OUTER JOIN TREXONE_AUD_DATA.Item AUD__Item
        ON AUD__Item.rx_record_num = AUD__Fill_Fact.rx_record_num
       AND AUD__Item.rx_fill_seq = AUD__Fill_Fact.rx_fill_seq
       AND AUD__Item.eff_end_date = TO_DATE('12/31/2999', 'MM/DD/YYYY')
       AND AUD__Item.H_TYPE != 'D'
     LEFT OUTER JOIN TREXONE_AUD_DATA.Order_Record AUD__Order_Record
         ON AUD__Order_Record.order_num = AUD__Item.order_num
        AND AUD__Order_Record.eff_end_date = TO_DATE('12/31/2999', 'MM/DD/YYYY')
     LEFT OUTER JOIN TREXONE_DW_DATA.Patient DW__Patient
         ON DW__Patient.pd_patient_num = AUD__Fill_Fact.patient_num
     LEFT OUTER JOIN TREXONE_DW_DATA.Shipment_Fact DW__Shipment_Fact
         ON DW__Shipment_Fact.order_num = AUD__Item.order_num
        AND DW__Shipment_Fact.pd_patient_key = DW__Patient.pd_patient_key
     LEFT OUTER JOIN TREXONE_DW_DATA.Courier_Shipment_Type DW__Courier_Shipment_Type
         ON DW__Courier_Shipment_Type.courier_shipment_key = DW__Shipment_Fact.courier_shipment_key
     LEFT OUTER JOIN TREXONE_DW_DATA.Courier_Shipment_Type DW__Courier_Shipment_Type_Orig
         ON DW__Courier_Shipment_Type_Orig.courier_shipment_key = DW__Shipment_Fact.orig_courier_shipment_key
     LEFT OUTER JOIN TREXONE_DW_DATA.Delivery_Method_Type DW__Delivery_Method_Type
         ON DW__Delivery_Method_Type.dm_delivery_method_key = DW__Shipment_Fact.dm_delivery_method_key
     LEFT OUTER JOIN TREXONE_DW_DATA.Facility DW__Shipment_Fact_Facility
        ON DW__Shipment_Fact_Facility.fd_facility_key = DW__Shipment_Fact.fd_facility_key
       AND DW__Shipment_Fact_Facility.FD_IS_PPI_ENABLED = 'Y'
     LEFT OUTER JOIN TREXONE_AUD_DATA.Rx_Fill AUD__Rx_Fill
        ON AUD__Rx_Fill.rx_record_num = AUD__Item.rx_record_num
       AND AUD__Rx_Fill.rx_fill_seq = AUD__Item.rx_fill_seq
       AND AUD__Rx_Fill.eff_end_date = To_Date('29991231','YYYYMMDD')
     LEFT OUTER JOIN TREXONE_AUD_DATA.Ads_Response AUD__Ads_Response
        ON AUD__Ads_Response.order_num = AUD__Item.order_num
       AND AUD__Ads_Response.item_seq = AUD__Item.item_seq
       AND AUD__Ads_Response.eff_end_date = To_Date('29991231','YYYYMMDD')
     LEFT OUTER JOIN TREXONE_AUD_DATA.Manifest_Item AUD__Manifest_Item
        ON AUD__Manifest_Item.order_num = AUD__Item.order_num
       AND AUD__Manifest_Item.item_seq = AUD__Item.item_seq
       AND AUD__Manifest_Item.eff_end_date = To_Date('29991231','YYYYMMDD')
    WHERE AUD__Fill_Fact.FILL_STATE_CHG_TS BETWEEN (:RunDate - 1) 
                                               AND (:RunDate - 1 + INTERVAL '23:59:59' HOUR TO SECOND)
  )
  WHERE CentralFillRank = 1
)
WHERE MalarkeyRank = 1
ORDER BY
     store_num
    ,rx_num_txt
    ,REFILL_NUM
    ,PARTIAL_FILL_SEQ
    ,split_billing_code
    ,FILL_STATE_CHG_TS
    ,fill_state_code
    ,rx_record_num
    ,rx_fill_seq
    ,order_num
    ,item_seq