WITH INSERT_INTO_STAGING AS (
    SELECT /*+ materialize */
        BENEFIT_GROUP_ID
        ,DATE_OF_SERVICE
        ,DAW_CODE
        ,DAYS_SUPPLY
        ,DISPENSED_DRUG_NUM
        ,DISPENSED_QTY_AWP
        ,FACILITY_NUM
        ,FILL_STATE_CHG_TS
        ,FILL_STATE_CODE
        ,FILL_STATE_KEY
        ,FILL_STATE_PRICE_NUM
        ,FINAL_PRICE
        ,INCENTIVE_FEE_AMOUNT
        ,INTERNAL_FILL_ID
        ,IS_SAME_DAY_REVERSAL
        ,MEDICAL_CONDITION_NUM
        ,ORIGINAL_PRODUCT_QTY
        ,PARTIAL_FILL_SEQ
        ,PATIENT_NUM
        ,PATIENT_PAY_AMOUNT
        ,PRESCRIBED_DATE
        ,PRESCRIBER_ADDRESS_NUM
        ,PRESCRIBER_ID_DEA
        ,PRESCRIBER_ID_NPI
        ,PRESCRIBER_NUM
        ,PRESCRIBER_STATE
        ,PRESCRIBER_ZIP
        ,QTY_DISPENSED
        ,REFILL_NUM
        ,REFILLS_AUTHORIZED
        ,REFILLS_REMAINING
        ,RX_FILL_SEQ
        ,RX_NUMBER
        ,RX_ORIGIN_CODE
        ,RX_RECORD_NUM
        ,SHORT_FILL_STATUS
        ,SPLIT_BILLING_CODE
        ,TLOG_SEQUENCE_NUM
        ,TOTAL_COST
        ,TOTAL_FEE
        ,TOTAL_UANDC
        ,TOTAL_USER_DEFINED
        ,TP_CURR_PATIENT_PAY_AMOUNT
        ,TP_CURR_TOTAL_COST
        ,TP_CURR_TOTAL_FEE
        ,TP_ITEM_CLAIM_KEY
        ,TP_ITEM_CLAIM_NUM
        ,TP_MESSAGE_KEY
        ,TP_PATIENT_NUM
        ,TP_PLAN_AMOUNT_PAID
        ,TP_PLAN_NUM
        ,TP_PRIOR_AUTH_NUM
        ,TP_PROCESSOR_DEST_NUM
        ,TRANSACTION_CODE
        ,FS_FILL_STATE_CHG_TS
        ,FS_FILL_STATE_CODE
        ,FS_RX_FILL_SEQ
        ,FS_RX_RECORD_NUM
        ,INGREDIENT_NDC1
        ,INGREDIENT_NDC2
        ,INGREDIENT_NDC3
        ,INGREDIENT_NDC4
        ,INGREDIENT_NDC5
        ,INGREDIENT_NDC6
        ,INGREDIENT_PRICE1
        ,INGREDIENT_PRICE2
        ,INGREDIENT_PRICE3
        ,INGREDIENT_PRICE4
        ,INGREDIENT_PRICE5
        ,INGREDIENT_PRICE6
        ,INGREDIENT_QTY1
        ,INGREDIENT_QTY2
        ,INGREDIENT_QTY3
        ,INGREDIENT_QTY4
        ,INGREDIENT_QTY5
        ,INGREDIENT_QTY6
        ,BIRTH_DATE
        ,P_FIRST_NAME
        ,P_GENDER
        ,P_LAST_NAME
        ,P_MIDDLE_NAME
        ,ADDRESS1
        ,ADDRESS2
        ,CITY
        ,PA_FIRST_NAME
        ,PA_GENDER
        ,PA_LAST_NAME
        ,PA_MIDDLE_NAME
        ,PHONE_NUM
        ,STATE
        ,ZIP
        ,AUTHORIZATION_NUMBER
        ,EMERGENCY_CONDITION_CODE
        ,EXTERNAL_BILLING_INDICATOR
        ,H_LEVEL
        ,TP_MESSAGE_NUM
        ,BIN_NUMBER
        ,PROCESSOR_CONTROL_NUMBER
        ,RS_BASIS_OF_REIMBRSMT_DET
        ,RS_NETWRK_REIMBURSEMENT_ID
        ,DRUG_GCN_SEQ
        ,DRUG_IS_MAINT_DRUG
        ,DRUG_NDC
        ,DRUG_PACKAGE_SIZE
        ,DRUG_STRENGTH
        ,DRUG_STRENGTH_UNITS
        ,FD_FACILITY_ID
        ,FD_NCPDP_PROVIDER_ID
        ,MCD_FRMTTD_COND_REF_NUM
        ,PRS_FIRST_NAME
        ,PRS_LAST_NAME
        ,PRS_PRESCRIBER_KEY
        ,PRS_PRESCRIBER_NUM
        ,PADR_KEY
        ,PADR_ADDRESS1
        ,PADR_ADDRESS2
        ,PADR_CITY
        ,PRD_IS_COMPOUND
        ,PRD_IS_GENERIC
        ,PRD_IS_PRICE_MAINTAINED
        ,PRD_NAME
        ,PRD_PACKAGE_SIZE
        ,PD_PATIENT_KEY
        ,TPD_CARDHOLDER_ID
        ,TPD_CARDHOLDER_PATIENT_NUM
        ,TPD_RELATIONSHIP_NUM
        ,TPD_TP_PLAN_NUM
        ,TPLD_PLAN_CODE
        ,REQUEST_MESSAGE
        ,0 as LookUpDaysSupply
        ,0 as LookUpQtyDispensed
        ,0 as LookUpAcquisitionCost
        ,'' as IsSenior
        ,ORDER_NUM
    FROM (
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

                   )
    WHERE fill_state_chg_ts BETWEEN (:RunDate - 1) AND (:RunDate - 1 + INTERVAL '23:59:59' HOUR TO SECOND)
      AND fill_state_code IN (3, 4)
      AND is_same_day_reversal = 'N'
      --and rownum < 10
--) SELECT Count(*) FROM INSERT_INTO_STAGING
--Time to get all [36593] rows in [159] seconds
), UPDATE_STAGING_CF_INDICATOR AS (
    SELECT /*+ materialize index (i IN_RX_FILL_07)  */
            tpcs.*,
            (CASE WHEN i.target_cf_facility_num IS NULL THEN 'N' ELSE 'Y' END) AS CF_STORE_INDICATOR
        FROM INSERT_INTO_STAGING tpcs
             LEFT OUTER JOIN TREXONE_AUD_DATA.Item i
                 ON i.rx_record_num = tpcs.RX_RECORD_NUM
                AND i.rx_fill_seq = tpcs.RX_FILL_SEQ
                AND i.order_num = tpcs.order_num
                AND i.eff_end_date = TO_DATE('12/31/2999', 'MM/DD/YYYY')
                AND i.h_type <> 'D'
--) SELECT Count(*) FROM UPDATE_STAGING_CF_INDICATOR
--(A) Time to get all [36593] rows in [159] seconds
--(B) Time to get all [36593] rows in [157] seconds
), UPDATE_STAGING_ADJUDICATION_DT AS (
        SELECT  /*+ materialize */
            tpcs.*,
            temp_table.first_adjudicated_date
        FROM UPDATE_STAGING_CF_INDICATOR tpcs
             LEFT OUTER JOIN
             (SELECT  /*+ index (fs IN_RX_FILL_06)  */
                  rx_record_num,
                  rx_fill_seq,
                  Min(fs.fill_state_chg_ts) AS first_adjudicated_date
              FROM TREXONE_AUD_DATA.Fill_State fs
              WHERE fs.fill_state_code = 3
              GROUP BY rx_record_num, rx_fill_seq
             ) temp_table
             ON temp_table.rx_record_num = tpcs.RX_RECORD_NUM
            AND temp_table.rx_fill_seq = tpcs.RX_FILL_SEQ
--) SELECT Count(*) FROM UPDATE_STAGING_ADJUDICATION_DT
--(A) Time to get all [36593] rows in [159] seconds
--(B) Time to get all [36593] rows in [157] seconds
--(C) Time to get all [36593] rows in [155] seconds
), UPDATE_STAGING_DISPENSED_DT AS (
        SELECT /*+ materialize index (rf sanjay101) */
        --we tried IN_RX_07
            tpcs.*,
            Min(rf.dispense_date) AS first_dispense_date
        FROM UPDATE_STAGING_ADJUDICATION_DT tpcs
             LEFT OUTER JOIN TREXONE_AUD_DATA.Rx_Fill rf
               ON rf.rx_record_num = tpcs.RX_RECORD_NUM


              --AND rf.EFF_START_DATE BETWEEN (tpcs.RUN_DATE -365) AND tpcs.RUN_DATE
              --AND rf.EFF_END_DATE = TO_DATE('12/31/2999', 'MM/DD/YYYY') --REMOVE HERE IF QA finds issues with First Disp Dt
              --TRY THESE NEXT TWO LINES instead of above - 2999
              --AND rf.eff_start_date <= tpcs.FILL_STATE_CHG_TS
              --AND rf.eff_end_date > tpcs.FILL_STATE_CHG_TS
        GROUP BY
        tpcs.BENEFIT_GROUP_ID
        ,tpcs.DATE_OF_SERVICE
        ,tpcs.DAW_CODE
        ,tpcs.DAYS_SUPPLY
        ,tpcs.DISPENSED_DRUG_NUM
        ,tpcs.DISPENSED_QTY_AWP
        ,tpcs.FACILITY_NUM
        ,tpcs.FILL_STATE_CHG_TS
        ,tpcs.FILL_STATE_CODE
        ,tpcs.FILL_STATE_KEY
        ,tpcs.FILL_STATE_PRICE_NUM
        ,tpcs.FINAL_PRICE
        ,tpcs.INCENTIVE_FEE_AMOUNT
        ,tpcs.INTERNAL_FILL_ID
        ,tpcs.IS_SAME_DAY_REVERSAL
        ,tpcs.MEDICAL_CONDITION_NUM
        ,tpcs.ORIGINAL_PRODUCT_QTY
        ,tpcs.PARTIAL_FILL_SEQ
        ,tpcs.PATIENT_NUM
        ,tpcs.PATIENT_PAY_AMOUNT
        ,tpcs.PRESCRIBED_DATE
        ,tpcs.PRESCRIBER_ADDRESS_NUM
        ,tpcs.PRESCRIBER_ID_DEA
        ,tpcs.PRESCRIBER_ID_NPI
        ,tpcs.PRESCRIBER_NUM
        ,tpcs.PRESCRIBER_STATE
        ,tpcs.PRESCRIBER_ZIP
        ,tpcs.QTY_DISPENSED
        ,tpcs.REFILL_NUM
        ,tpcs.REFILLS_AUTHORIZED
        ,tpcs.REFILLS_REMAINING
        ,tpcs.RX_FILL_SEQ
        ,tpcs.RX_NUMBER
        ,tpcs.RX_ORIGIN_CODE
        ,tpcs.RX_RECORD_NUM
        ,tpcs.SHORT_FILL_STATUS
        ,tpcs.SPLIT_BILLING_CODE
        ,tpcs.TLOG_SEQUENCE_NUM
        ,tpcs.TOTAL_COST
        ,tpcs.TOTAL_FEE
        ,tpcs.TOTAL_UANDC
        ,tpcs.TOTAL_USER_DEFINED
        ,tpcs.TP_CURR_PATIENT_PAY_AMOUNT
        ,tpcs.TP_CURR_TOTAL_COST
        ,tpcs.TP_CURR_TOTAL_FEE
        ,tpcs.TP_ITEM_CLAIM_KEY
        ,tpcs.TP_ITEM_CLAIM_NUM
        ,tpcs.TP_MESSAGE_KEY
        ,tpcs.TP_PATIENT_NUM
        ,tpcs.TP_PLAN_AMOUNT_PAID
        ,tpcs.TP_PLAN_NUM
        ,tpcs.TP_PRIOR_AUTH_NUM
        ,tpcs.TP_PROCESSOR_DEST_NUM
        ,tpcs.TRANSACTION_CODE
        ,tpcs.FS_FILL_STATE_CHG_TS
        ,tpcs.FS_FILL_STATE_CODE
        ,tpcs.FS_RX_FILL_SEQ
        ,tpcs.FS_RX_RECORD_NUM
        ,tpcs.INGREDIENT_NDC1
        ,tpcs.INGREDIENT_NDC2
        ,tpcs.INGREDIENT_NDC3
        ,tpcs.INGREDIENT_NDC4
        ,tpcs.INGREDIENT_NDC5
        ,tpcs.INGREDIENT_NDC6
        ,tpcs.INGREDIENT_PRICE1
        ,tpcs.INGREDIENT_PRICE2
        ,tpcs.INGREDIENT_PRICE3
        ,tpcs.INGREDIENT_PRICE4
        ,tpcs.INGREDIENT_PRICE5
        ,tpcs.INGREDIENT_PRICE6
        ,tpcs.INGREDIENT_QTY1
        ,tpcs.INGREDIENT_QTY2
        ,tpcs.INGREDIENT_QTY3
        ,tpcs.INGREDIENT_QTY4
        ,tpcs.INGREDIENT_QTY5
        ,tpcs.INGREDIENT_QTY6
        ,tpcs.BIRTH_DATE
        ,tpcs.P_FIRST_NAME
        ,tpcs.P_GENDER
        ,tpcs.P_LAST_NAME
        ,tpcs.P_MIDDLE_NAME
        ,tpcs.ADDRESS1
        ,tpcs.ADDRESS2
        ,tpcs.CITY
        ,tpcs.PA_FIRST_NAME
        ,tpcs.PA_GENDER
        ,tpcs.PA_LAST_NAME
        ,tpcs.PA_MIDDLE_NAME
        ,tpcs.PHONE_NUM
        ,tpcs.STATE
        ,tpcs.ZIP
        ,tpcs.AUTHORIZATION_NUMBER
        ,tpcs.EMERGENCY_CONDITION_CODE
        ,tpcs.EXTERNAL_BILLING_INDICATOR
        ,tpcs.H_LEVEL
        ,tpcs.TP_MESSAGE_NUM
        ,tpcs.BIN_NUMBER
        ,tpcs.PROCESSOR_CONTROL_NUMBER
        ,tpcs.RS_BASIS_OF_REIMBRSMT_DET
        ,tpcs.RS_NETWRK_REIMBURSEMENT_ID
        ,tpcs.DRUG_GCN_SEQ
        ,tpcs.DRUG_IS_MAINT_DRUG
        ,tpcs.DRUG_NDC
        ,tpcs.DRUG_PACKAGE_SIZE
        ,tpcs.DRUG_STRENGTH
        ,tpcs.DRUG_STRENGTH_UNITS
        ,tpcs.FD_FACILITY_ID
        ,tpcs.FD_NCPDP_PROVIDER_ID
        ,tpcs.MCD_FRMTTD_COND_REF_NUM
        ,tpcs.PRS_FIRST_NAME
        ,tpcs.PRS_LAST_NAME
        ,tpcs.PRS_PRESCRIBER_KEY
        ,tpcs.PRS_PRESCRIBER_NUM
        ,tpcs.PADR_KEY
        ,tpcs.PADR_ADDRESS1
        ,tpcs.PADR_ADDRESS2
        ,tpcs.PADR_CITY
        ,tpcs.PRD_IS_COMPOUND
        ,tpcs.PRD_IS_GENERIC
        ,tpcs.PRD_IS_PRICE_MAINTAINED
        ,tpcs.PRD_NAME
        ,tpcs.PRD_PACKAGE_SIZE
        ,tpcs.PD_PATIENT_KEY
        ,tpcs.TPD_CARDHOLDER_ID
        ,tpcs.TPD_CARDHOLDER_PATIENT_NUM
        ,tpcs.TPD_RELATIONSHIP_NUM
        ,tpcs.TPD_TP_PLAN_NUM
        ,tpcs.TPLD_PLAN_CODE
        ,tpcs.REQUEST_MESSAGE
        ,tpcs.CF_STORE_INDICATOR
        ,tpcs.first_adjudicated_date
        ,tpcs.LookUpDaysSupply
        ,tpcs.LookUpQtyDispensed
        ,tpcs.IsSenior
        ,tpcs.ORDER_NUM
--) SELECT Count(*) FROM UPDATE_STAGING_DISPENSED_DT
--(A) Time to get all [36593] rows in [159] seconds
--(B) Time to get all [36593] rows in [157] seconds
--(C) Time to get all [36593] rows in [155] seconds
--(D) Time to get all [36593] rows in [280] seconds
), UPDATE_STAGING_SUBMITTED_DT AS (
    SELECT /*+ materialize */
        tpcs.*,
        temp_table.transmission_date
    FROM UPDATE_STAGING_DISPENSED_DT tpcs
        LEFT OUTER JOIN
        (SELECT /*+ index (fs IN_RX_FILL_06)  */
            fs.rx_record_num,
            fs.rx_fill_seq,
            Max(fs.fill_state_chg_ts) AS transmission_date
         FROM TREXONE_AUD_DATA.Fill_State fs
         WHERE fs.fill_state_code IN (3, 4)
           AND fs.fill_state_chg_ts BETWEEN (:RunDate - 1)
                                        AND (:RunDate + INTERVAL '23:59:59' HOUR TO SECOND)
         GROUP BY fs.rx_record_num, fs.rx_fill_seq
         ) temp_table
            ON temp_table.rx_record_num = tpcs.rx_record_num
           AND temp_table.rx_fill_seq = tpcs.RX_FILL_SEQ
), UPDATE_STAGING_QTY_SUPPLY AS (
        SELECT /*+ materialize index (rf sanjay101) */
            tpcs.*,
            Sum(rf.qty_dispensed) AS qty_dispensed,
            Sum(rf.days_supply) AS days_supply
        FROM UPDATE_STAGING_SUBMITTED_DT tpcs
             LEFT OUTER JOIN TREXONE_AUD_DATA.Rx_Fill rf
               ON rf.rx_record_num = tpcs.RX_RECORD_NUM
              AND rf.internal_fill_id = tpcs.INTERNAL_FILL_ID
              AND rf.eff_start_date <= tpcs.FILL_STATE_CHG_TS
              AND rf.eff_end_date > tpcs.FILL_STATE_CHG_TS
              AND rf.fill_status_num IN (1, 2)               
              ------AND rf.EFF_END_DATE = TO_DATE('12/31/2999', 'MM/DD/YYYY') --REMOVE HERE IF QA finds issues with First Disp Dt
              AND tpcs.SHORT_FILL_STATUS IN ('P','C')
              AND NVL(tpcs.TP_PLAN_NUM,0) != 0
        GROUP BY
        tpcs.BENEFIT_GROUP_ID
        ,tpcs.DATE_OF_SERVICE
        ,tpcs.DAW_CODE
        ,tpcs.DAYS_SUPPLY
        ,tpcs.DISPENSED_DRUG_NUM
        ,tpcs.DISPENSED_QTY_AWP
        ,tpcs.FACILITY_NUM
        ,tpcs.FILL_STATE_CHG_TS
        ,tpcs.FILL_STATE_CODE
        ,tpcs.FILL_STATE_KEY
        ,tpcs.FILL_STATE_PRICE_NUM
        ,tpcs.FINAL_PRICE
        ,tpcs.INCENTIVE_FEE_AMOUNT
        ,tpcs.INTERNAL_FILL_ID
        ,tpcs.IS_SAME_DAY_REVERSAL
        ,tpcs.MEDICAL_CONDITION_NUM
        ,tpcs.ORIGINAL_PRODUCT_QTY
        ,tpcs.PARTIAL_FILL_SEQ
        ,tpcs.PATIENT_NUM
        ,tpcs.PATIENT_PAY_AMOUNT
        ,tpcs.PRESCRIBED_DATE
        ,tpcs.PRESCRIBER_ADDRESS_NUM
        ,tpcs.PRESCRIBER_ID_DEA
        ,tpcs.PRESCRIBER_ID_NPI
        ,tpcs.PRESCRIBER_NUM
        ,tpcs.PRESCRIBER_STATE
        ,tpcs.PRESCRIBER_ZIP
        ,tpcs.QTY_DISPENSED
        ,tpcs.REFILL_NUM
        ,tpcs.REFILLS_AUTHORIZED
        ,tpcs.REFILLS_REMAINING
        ,tpcs.RX_FILL_SEQ
        ,tpcs.RX_NUMBER
        ,tpcs.RX_ORIGIN_CODE
        ,tpcs.RX_RECORD_NUM
        ,tpcs.SHORT_FILL_STATUS
        ,tpcs.SPLIT_BILLING_CODE
        ,tpcs.TLOG_SEQUENCE_NUM
        ,tpcs.TOTAL_COST
        ,tpcs.TOTAL_FEE
        ,tpcs.TOTAL_UANDC
        ,tpcs.TOTAL_USER_DEFINED
        ,tpcs.TP_CURR_PATIENT_PAY_AMOUNT
        ,tpcs.TP_CURR_TOTAL_COST
        ,tpcs.TP_CURR_TOTAL_FEE
        ,tpcs.TP_ITEM_CLAIM_KEY
        ,tpcs.TP_ITEM_CLAIM_NUM
        ,tpcs.TP_MESSAGE_KEY
        ,tpcs.TP_PATIENT_NUM
        ,tpcs.TP_PLAN_AMOUNT_PAID
        ,tpcs.TP_PLAN_NUM
        ,tpcs.TP_PRIOR_AUTH_NUM
        ,tpcs.TP_PROCESSOR_DEST_NUM
        ,tpcs.TRANSACTION_CODE
        ,tpcs.FS_FILL_STATE_CHG_TS
        ,tpcs.FS_FILL_STATE_CODE
        ,tpcs.FS_RX_FILL_SEQ
        ,tpcs.FS_RX_RECORD_NUM
        ,tpcs.INGREDIENT_NDC1
        ,tpcs.INGREDIENT_NDC2
        ,tpcs.INGREDIENT_NDC3
        ,tpcs.INGREDIENT_NDC4
        ,tpcs.INGREDIENT_NDC5
        ,tpcs.INGREDIENT_NDC6
        ,tpcs.INGREDIENT_PRICE1
        ,tpcs.INGREDIENT_PRICE2
        ,tpcs.INGREDIENT_PRICE3
        ,tpcs.INGREDIENT_PRICE4
        ,tpcs.INGREDIENT_PRICE5
        ,tpcs.INGREDIENT_PRICE6
        ,tpcs.INGREDIENT_QTY1
        ,tpcs.INGREDIENT_QTY2
        ,tpcs.INGREDIENT_QTY3
        ,tpcs.INGREDIENT_QTY4
        ,tpcs.INGREDIENT_QTY5
        ,tpcs.INGREDIENT_QTY6
        ,tpcs.BIRTH_DATE
        ,tpcs.P_FIRST_NAME
        ,tpcs.P_GENDER
        ,tpcs.P_LAST_NAME
        ,tpcs.P_MIDDLE_NAME
        ,tpcs.ADDRESS1
        ,tpcs.ADDRESS2
        ,tpcs.CITY
        ,tpcs.PA_FIRST_NAME
        ,tpcs.PA_GENDER
        ,tpcs.PA_LAST_NAME
        ,tpcs.PA_MIDDLE_NAME
        ,tpcs.PHONE_NUM
        ,tpcs.STATE
        ,tpcs.ZIP
        ,tpcs.AUTHORIZATION_NUMBER
        ,tpcs.EMERGENCY_CONDITION_CODE
        ,tpcs.EXTERNAL_BILLING_INDICATOR
        ,tpcs.H_LEVEL
        ,tpcs.TP_MESSAGE_NUM
        ,tpcs.BIN_NUMBER
        ,tpcs.PROCESSOR_CONTROL_NUMBER
        ,tpcs.RS_BASIS_OF_REIMBRSMT_DET
        ,tpcs.RS_NETWRK_REIMBURSEMENT_ID
        ,tpcs.DRUG_GCN_SEQ
        ,tpcs.DRUG_IS_MAINT_DRUG
        ,tpcs.DRUG_NDC
        ,tpcs.DRUG_PACKAGE_SIZE
        ,tpcs.DRUG_STRENGTH
        ,tpcs.DRUG_STRENGTH_UNITS
        ,tpcs.FD_FACILITY_ID
        ,tpcs.FD_NCPDP_PROVIDER_ID
        ,tpcs.MCD_FRMTTD_COND_REF_NUM
        ,tpcs.PRS_FIRST_NAME
        ,tpcs.PRS_LAST_NAME
        ,tpcs.PRS_PRESCRIBER_KEY
        ,tpcs.PRS_PRESCRIBER_NUM
        ,tpcs.PADR_KEY
        ,tpcs.PADR_ADDRESS1
        ,tpcs.PADR_ADDRESS2
        ,tpcs.PADR_CITY
        ,tpcs.PRD_IS_COMPOUND
        ,tpcs.PRD_IS_GENERIC
        ,tpcs.PRD_IS_PRICE_MAINTAINED
        ,tpcs.PRD_NAME
        ,tpcs.PRD_PACKAGE_SIZE
        ,tpcs.PD_PATIENT_KEY
        ,tpcs.TPD_CARDHOLDER_ID
        ,tpcs.TPD_CARDHOLDER_PATIENT_NUM
        ,tpcs.TPD_RELATIONSHIP_NUM
        ,tpcs.TPD_TP_PLAN_NUM
        ,tpcs.TPLD_PLAN_CODE
        ,tpcs.REQUEST_MESSAGE
        ,tpcs.CF_STORE_INDICATOR
        ,tpcs.first_adjudicated_date
        ,tpcs.first_dispense_date
        ,tpcs.transmission_date
        ,tpcs.LookUpDaysSupply
        ,tpcs.LookUpQtyDispensed
        ,tpcs.IsSenior
        ,tpcs.ORDER_NUM
)
--select * from UPDATE_STAGING_SUBMITTED_DT
select 
        BENEFIT_GROUP_ID
        ,DATE_OF_SERVICE
        ,DAW_CODE
        ,DAYS_SUPPLY
        ,DISPENSED_DRUG_NUM
        ,DISPENSED_QTY_AWP
        ,FACILITY_NUM
        ,FILL_STATE_CHG_TS
        ,FILL_STATE_CODE
        ,FILL_STATE_KEY
        ,FILL_STATE_PRICE_NUM
        ,FINAL_PRICE
        ,INCENTIVE_FEE_AMOUNT
        ,INTERNAL_FILL_ID
        ,IS_SAME_DAY_REVERSAL
        ,MEDICAL_CONDITION_NUM
        ,ORIGINAL_PRODUCT_QTY
        ,PARTIAL_FILL_SEQ
        ,PATIENT_NUM
        ,PATIENT_PAY_AMOUNT
        ,PRESCRIBED_DATE
        ,PRESCRIBER_ADDRESS_NUM
        ,PRESCRIBER_ID_DEA
        ,PRESCRIBER_ID_NPI
        ,PRESCRIBER_NUM
        ,PRESCRIBER_STATE
        ,PRESCRIBER_ZIP
        ,QTY_DISPENSED
        ,REFILL_NUM
        ,REFILLS_AUTHORIZED
        ,REFILLS_REMAINING
        ,RX_FILL_SEQ
        ,RX_NUMBER
        ,RX_ORIGIN_CODE
        ,RX_RECORD_NUM
        ,SHORT_FILL_STATUS
        ,SPLIT_BILLING_CODE
        ,TLOG_SEQUENCE_NUM
        ,TOTAL_COST
        ,TOTAL_FEE
        ,TOTAL_UANDC
        ,TOTAL_USER_DEFINED
        ,TP_CURR_PATIENT_PAY_AMOUNT
        ,TP_CURR_TOTAL_COST
        ,TP_CURR_TOTAL_FEE
        ,TP_ITEM_CLAIM_KEY
        ,TP_ITEM_CLAIM_NUM
        ,TP_MESSAGE_KEY
        ,TP_PATIENT_NUM
        ,TP_PLAN_AMOUNT_PAID
        ,TP_PLAN_NUM
        ,TP_PRIOR_AUTH_NUM
        ,TP_PROCESSOR_DEST_NUM
        ,TRANSACTION_CODE
        ,FS_FILL_STATE_CHG_TS
        ,FS_FILL_STATE_CODE
        ,FS_RX_FILL_SEQ
        ,FS_RX_RECORD_NUM
        ,INGREDIENT_NDC1
        ,INGREDIENT_NDC2
        ,INGREDIENT_NDC3
        ,INGREDIENT_NDC4
        ,INGREDIENT_NDC5
        ,INGREDIENT_NDC6
        ,INGREDIENT_PRICE1
        ,INGREDIENT_PRICE2
        ,INGREDIENT_PRICE3
        ,INGREDIENT_PRICE4
        ,INGREDIENT_PRICE5
        ,INGREDIENT_PRICE6
        ,INGREDIENT_QTY1
        ,INGREDIENT_QTY2
        ,INGREDIENT_QTY3
        ,INGREDIENT_QTY4
        ,INGREDIENT_QTY5
        ,INGREDIENT_QTY6
        ,BIRTH_DATE
        ,P_FIRST_NAME
        ,P_GENDER
        ,P_LAST_NAME
        ,P_MIDDLE_NAME
        ,ADDRESS1
        ,ADDRESS2
        ,CITY
        ,PA_FIRST_NAME
        ,PA_GENDER
        ,PA_LAST_NAME
        ,PA_MIDDLE_NAME
        ,PHONE_NUM
        ,STATE
        ,ZIP
        ,AUTHORIZATION_NUMBER
        ,EMERGENCY_CONDITION_CODE
        ,EXTERNAL_BILLING_INDICATOR
        ,H_LEVEL
        ,TP_MESSAGE_NUM
        ,BIN_NUMBER
        ,PROCESSOR_CONTROL_NUMBER
        ,RS_BASIS_OF_REIMBRSMT_DET
        ,RS_NETWRK_REIMBURSEMENT_ID
        ,DRUG_GCN_SEQ
        ,DRUG_IS_MAINT_DRUG
        ,DRUG_NDC
        ,DRUG_PACKAGE_SIZE
        ,DRUG_STRENGTH
        ,DRUG_STRENGTH_UNITS
        ,FD_FACILITY_ID
        ,FD_NCPDP_PROVIDER_ID
        ,MCD_FRMTTD_COND_REF_NUM
        ,PRS_FIRST_NAME
        ,PRS_LAST_NAME
        ,PRS_PRESCRIBER_KEY
        ,PRS_PRESCRIBER_NUM
        ,PADR_KEY
        ,PADR_ADDRESS1
        ,PADR_ADDRESS2
        ,PADR_CITY
        ,PRD_IS_COMPOUND
        ,PRD_IS_GENERIC
        ,PRD_IS_PRICE_MAINTAINED
        ,PRD_NAME
        ,PRD_PACKAGE_SIZE
        ,PD_PATIENT_KEY
        ,TPD_CARDHOLDER_ID
        ,TPD_CARDHOLDER_PATIENT_NUM
        ,TPD_RELATIONSHIP_NUM
        ,TPD_TP_PLAN_NUM
        ,TPLD_PLAN_CODE
        ,REQUEST_MESSAGE
        ,transmission_date
        ,first_adjudicated_date
        ,first_dispense_date
        ,LookUpQtyDispensed
        ,LookUpDaysSupply
        ,LookUpAcquisitionCost
        ,CF_STORE_INDICATOR
        ,IsSenior
        ,ORDER_NUM
from UPDATE_STAGING_SUBMITTED_DT
