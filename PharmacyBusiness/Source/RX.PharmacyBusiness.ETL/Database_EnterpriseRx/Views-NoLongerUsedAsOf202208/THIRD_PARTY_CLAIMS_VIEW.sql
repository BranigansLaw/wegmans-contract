
  CREATE OR REPLACE FORCE EDITIONABLE VIEW "WEGMANS"."THIRD_PARTY_CLAIMS_VIEW" ("BENEFIT_GROUP_ID", "DATE_OF_SERVICE", "DAW_CODE", "DAYS_SUPPLY", "DISPENSED_DRUG_NUM", "DISPENSED_QTY_AWP", "FACILITY_NUM", "FILL_STATE_CHG_TS", "FILL_STATE_CODE", "FILL_STATE_KEY", "FILL_STATE_PRICE_NUM", "FINAL_PRICE", "INCENTIVE_FEE_AMOUNT", "INTERNAL_FILL_ID", "IS_SAME_DAY_REVERSAL", "MEDICAL_CONDITION_NUM", "ORIGINAL_PRODUCT_QTY", "PARTIAL_FILL_SEQ", "PATIENT_NUM", "PATIENT_PAY_AMOUNT", "PRESCRIBED_DATE", "PRESCRIBER_ADDRESS_NUM", "PRESCRIBER_ID_DEA", "PRESCRIBER_ID_NPI", "PRESCRIBER_NUM", "PRESCRIBER_STATE", "PRESCRIBER_ZIP", "QTY_DISPENSED", "REFILL_NUM", "REFILLS_AUTHORIZED", "REFILLS_REMAINING", "RX_FILL_SEQ", "RX_NUMBER", "RX_ORIGIN_CODE", "RX_RECORD_NUM", "SHORT_FILL_STATUS", "SPLIT_BILLING_CODE", "TLOG_SEQUENCE_NUM", "TOTAL_COST", "TOTAL_FEE", "TOTAL_UANDC", "TOTAL_USER_DEFINED", "TP_CURR_PATIENT_PAY_AMOUNT", "TP_CURR_TOTAL_COST", "TP_CURR_TOTAL_FEE", "TP_ITEM_CLAIM_KEY", "TP_ITEM_CLAIM_NUM", "TP_MESSAGE_KEY", "TP_PATIENT_NUM", "TP_PLAN_AMOUNT_PAID", "TP_PLAN_NUM", "TP_PRIOR_AUTH_NUM", "TP_PROCESSOR_DEST_NUM", "TRANSACTION_CODE", "FS_FILL_STATE_CHG_TS", "FS_FILL_STATE_CODE", "FS_RX_FILL_SEQ", "FS_RX_RECORD_NUM", "INGREDIENT_NDC1", "INGREDIENT_NDC2", "INGREDIENT_NDC3", "INGREDIENT_NDC4", "INGREDIENT_NDC5", "INGREDIENT_NDC6", "INGREDIENT_PRICE1", "INGREDIENT_PRICE2", "INGREDIENT_PRICE3", "INGREDIENT_PRICE4", "INGREDIENT_PRICE5", "INGREDIENT_PRICE6", "INGREDIENT_QTY1", "INGREDIENT_QTY2", "INGREDIENT_QTY3", "INGREDIENT_QTY4", "INGREDIENT_QTY5", "INGREDIENT_QTY6", "BIRTH_DATE", "P_FIRST_NAME", "P_GENDER", "P_LAST_NAME", "P_MIDDLE_NAME", "ADDRESS1", "ADDRESS2", "CITY", "PA_FIRST_NAME", "PA_GENDER", "PA_LAST_NAME", "PA_MIDDLE_NAME", "PHONE_NUM", "STATE", "ZIP", "AUTHORIZATION_NUMBER", "EMERGENCY_CONDITION_CODE", "EXTERNAL_BILLING_INDICATOR", "H_LEVEL", "TP_MESSAGE_NUM", "BIN_NUMBER", "PROCESSOR_CONTROL_NUMBER", "RS_BASIS_OF_REIMBRSMT_DET", "RS_NETWRK_REIMBURSEMENT_ID", "DRUG_GCN_SEQ", "DRUG_IS_MAINT_DRUG", "DRUG_NDC", "DRUG_PACKAGE_SIZE", "DRUG_STRENGTH", "DRUG_STRENGTH_UNITS", "FD_FACILITY_ID", "FD_NCPDP_PROVIDER_ID", "MCD_FRMTTD_COND_REF_NUM", "PRS_FIRST_NAME", "PRS_LAST_NAME", "PRS_PRESCRIBER_KEY", "PRS_PRESCRIBER_NUM", "PADR_KEY", "PADR_ADDRESS1", "PADR_ADDRESS2", "PADR_CITY", "PRD_IS_COMPOUND", "PRD_IS_GENERIC", "PRD_IS_PRICE_MAINTAINED", "PRD_NAME", "PRD_PACKAGE_SIZE", "PD_PATIENT_KEY", "TPD_CARDHOLDER_ID", "TPD_CARDHOLDER_PATIENT_NUM", "TPD_RELATIONSHIP_NUM", "TPD_TP_PLAN_NUM", "TPLD_PLAN_CODE", "REQUEST_MESSAGE", "EE_FILL_STATE", "EE_TP_MESSAGE", "EE_TP_ITEM_CLAIM", "EE_PATIENT", "EE_TP_PATIENT", "EE_CARDHOLDER", "EE_TP_PRIOR_AUTH", "EE_TP_PROCESSOR_DESTINATION", "ORDER_NUM") AS 
  SELECT  /*+ cardinality(AUD__Fill_Fact,1) leading (AUD__Fill_Fact) index (AUD__Fill_Fact IN_FILL_FACT_03)  */ 
		 AUD__Fill_Fact.BENEFIT_GROUP_ID
		,AUD__Fill_Fact.date_of_service
		,AUD__Fill_Fact.DAW_CODE
		,AUD__Fill_Fact.DAYS_SUPPLY
		,AUD__Fill_Fact.dispensed_drug_num
		,AUD__Fill_Fact.DISPENSED_QTY_AWP
		,AUD__Fill_Fact.facility_num
		,AUD__Fill_Fact.FILL_STATE_CHG_TS
		,AUD__Fill_Fact.fill_state_code
		,AUD__Fill_Fact.fill_state_key
		,AUD__Fill_Fact.fill_state_price_num
		,AUD__Fill_Fact.final_price
		,AUD__Fill_Fact.INCENTIVE_FEE_AMOUNT
		,AUD__Fill_Fact.internal_fill_id
		,AUD__Fill_Fact.is_same_day_reversal
		,AUD__Fill_Fact.medical_condition_num
		,AUD__Fill_Fact.ORIGINAL_PRODUCT_QTY
		,AUD__Fill_Fact.partial_fill_seq
		,AUD__Fill_Fact.patient_num
		,AUD__Fill_Fact.PATIENT_PAY_AMOUNT
		,AUD__Fill_Fact.PRESCRIBED_DATE
		,AUD__Fill_Fact.prescriber_address_num
		,AUD__Fill_Fact.PRESCRIBER_ID_DEA
		,AUD__Fill_Fact.PRESCRIBER_ID_NPI
		,AUD__Fill_Fact.prescriber_num
		,AUD__Fill_Fact.PRESCRIBER_STATE
		,AUD__Fill_Fact.PRESCRIBER_ZIP
		,AUD__Fill_Fact.QTY_DISPENSED
		,AUD__Fill_Fact.REFILL_NUM
		,AUD__Fill_Fact.REFILLS_AUTHORIZED
		,AUD__Fill_Fact.REFILLS_REMAINING
		,AUD__Fill_Fact.rx_fill_seq
		,AUD__Fill_Fact.rx_number
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
		,AUD__Fill_State.fill_state_chg_ts AS fs_fill_state_chg_ts
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
		,AUD__Patient.BIRTH_DATE
		,AUD__Patient.FIRST_NAME AS P_FIRST_NAME
		,AUD__Patient.GENDER AS P_GENDER
		,AUD__Patient.LAST_NAME AS P_LAST_NAME
		,AUD__Patient.MIDDLE_NAME AS P_MIDDLE_NAME
		,AUD__Patient_Address.ADDRESS1
		,AUD__Patient_Address.ADDRESS2
		,AUD__Patient_Address.CITY
		,AUD__Patient_Address.FIRST_NAME AS PA_FIRST_NAME
		,AUD__Patient_Address.GENDER AS PA_GENDER
		,AUD__Patient_Address.LAST_NAME AS PA_LAST_NAME
		,AUD__Patient_Address.MIDDLE_NAME AS PA_MIDDLE_NAME
		,AUD__Patient_Address.PHONE_NUM
		,AUD__Patient_Address.STATE
		,AUD__Patient_Address.ZIP
		,AUD__Tp_Item_Claim.AUTHORIZATION_NUMBER
		,AUD__Tp_Item_Claim.EMERGENCY_CONDITION_CODE
		,AUD__Tp_Item_Claim.external_billing_indicator
		,AUD__Tp_Message.h_level        
		,AUD__Tp_Message.tp_message_num
		,AUD__Tp_Processor_Destination.BIN_NUMBER
		,AUD__Tp_Processor_Destination.PROCESSOR_CONTROL_NUMBER
		,AUD__Tp_Response_Message.RS_BASIS_OF_REIMBRSMT_DET
		,AUD__Tp_Response_Message.RS_NETWRK_REIMBURSEMENT_ID
		,DW__Drug.drug_gcn_seq
		,DW__Drug.DRUG_IS_MAINT_DRUG
		,DW__Drug.DRUG_NDC
		,DW__Drug.DRUG_PACKAGE_SIZE
		,DW__Drug.DRUG_STRENGTH
		,DW__Drug.DRUG_STRENGTH_UNITS
		,DW__Facility.FD_FACILITY_ID
		,DW__Facility.FD_NCPDP_PROVIDER_ID
		,DW__Medical_Condition.MCD_FRMTTD_COND_REF_NUM
		,DW__Prescriber.PRS_FIRST_NAME
		,DW__Prescriber.PRS_LAST_NAME
		,DW__Prescriber.PRS_PRESCRIBER_KEY
		,DW__Prescriber.PRS_PRESCRIBER_NUM
		,DW__Prescriber_Address.PADR_KEY
		,DW__Prescriber_Address.PADR_ADDRESS1
		,DW__Prescriber_Address.PADR_ADDRESS2
		,DW__Prescriber_Address.PADR_CITY
		,DW__Product.prd_is_compound
		,DW__Product.prd_is_generic
		,DW__Product.PRD_IS_PRICE_MAINTAINED
		,DW__Product.PRD_NAME
		,DW__Product.prd_package_size
		,DW__Tp_Patient.PD_PATIENT_KEY
		,DW__Tp_Patient.TPD_CARDHOLDER_ID
		,DW__Tp_Patient.tpd_cardholder_patient_num
		,DW__Tp_Patient.tpd_relationship_num
		,DW__Tp_Patient.tpd_tp_plan_num
		,DW__Tp_Plan.tpld_plan_code
		,SUBSTR (TO_CHAR (AUD__Tp_Message.request_message), 1, 2000) AS request_message
		,AUD__Fill_State.eff_end_date AS ee_Fill_State
		,AUD__Tp_Message.eff_end_date AS ee_Tp_Message
		,AUD__Tp_Item_Claim.eff_end_date AS ee_Tp_Item_Claim
		,AUD__Patient.eff_end_date AS ee_Patient
		,DW__Tp_Patient.tpd_eff_end_date AS ee_Tp_Patient
		,Cardholder.eff_end_date AS ee_Cardholder
		,AUD__Tp_Prior_Auth.eff_end_date AS ee_Tp_Prior_Auth
		,AUD__Tp_Processor_Destination.eff_end_date AS ee_Tp_Processor_Destination
        ,AUD__Fill_State.order_num

    FROM TREXONE_AUD_DATA.Fill_Fact AUD__Fill_Fact
        INNER JOIN TREXONE_DW_DATA.Facility DW__Facility
            ON DW__Facility.fd_facility_num = AUD__Fill_Fact.facility_num
           AND DW__Facility.FD_IS_PPI_ENABLED = 'Y'
		--NOTE: DW__Facility.fd_ncpdp_provider_id is the NCPDP, which can be obtained with the following join if needed.
        --INNER JOIN TREXONE_DW_DATA.Facility_ID DW__Facility_ID
        --    ON DW__Facility_ID.fd_facility_num = AUD__Fill_Fact.facility_num
        --   AND DW__Facility_ID.fd_type = 'F08'
		     --NOTE: DW__Facility_ID.fd_value is the NPI
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
        INNER JOIN TREXONE_AUD_DATA.Patient AUD__Patient
            ON AUD__Patient.patient_num = AUD__Fill_Fact.patient_num
           AND AUD__Patient.eff_end_date = TO_DATE('12/31/2999', 'MM/DD/YYYY')
        LEFT OUTER JOIN TREXONE_AUD_DATA.Patient_Address AUD__Patient_Address
            ON AUD__Patient_Address.patient_num = AUD__Fill_Fact.patient_num
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
        LEFT OUTER JOIN TREXONE_AUD_DATA.Patient Cardholder
            ON Cardholder.patient_num = DW__Tp_Patient.tpd_cardholder_patient_num
           AND Cardholder.eff_end_date = TO_DATE('12/31/2999', 'MM/DD/YYYY')
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
        LEFT OUTER JOIN TREXONE_AUD_DATA.Tp_Response_Message AUD__Tp_Response_Message
            ON AUD__Tp_Response_Message.tp_message_num = AUD__Tp_Message.tp_message_num
           AND AUD__Tp_Response_Message.h_level = AUD__Tp_Message.h_level

WITH READ ONLY;
/
