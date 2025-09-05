WITH INSERT_INTO_STAGING AS (
    SELECT 
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
        ,0 AS LOOKUPDAYSSUPPLY
        ,0 AS LOOKUPQTYDISPENSED
        ,0 AS LOOKUPACQUISITIONCOST
        ,'' AS ISSENIOR
        ,ORDER_NUM
    FROM (
                             SELECT  
                                                           AUD__FILL_FACT.BENEFIT_GROUP_ID
                                                          ,AUD__FILL_FACT.DATE_OF_SERVICE
                                                          ,AUD__FILL_FACT.DAW_CODE
                                                          ,AUD__FILL_FACT.DAYS_SUPPLY
                                                         ,AUD__FILL_FACT.DISPENSED_DRUG_NUM
                                                         ,AUD__FILL_FACT.DISPENSED_QTY_AWP
                                                          ,AUD__FILL_FACT.FACILITY_NUM
                                                          ,AUD__FILL_FACT.FILL_STATE_CHG_TS
                                                          ,AUD__FILL_FACT.FILL_STATE_CODE
                                                          ,AUD__FILL_FACT.FILL_STATE_KEY
                                                          ,AUD__FILL_FACT.FILL_STATE_PRICE_NUM
                                                          ,AUD__FILL_FACT.FINAL_PRICE
                                                         ,AUD__FILL_FACT.INCENTIVE_FEE_AMOUNT
                                                          ,AUD__FILL_FACT.INTERNAL_FILL_ID
                                                         ,AUD__FILL_FACT.IS_SAME_DAY_REVERSAL
                                                         ,AUD__FILL_FACT.MEDICAL_CONDITION_NUM
                                                         ,AUD__FILL_FACT.ORIGINAL_PRODUCT_QTY
                                                          ,AUD__FILL_FACT.PARTIAL_FILL_SEQ
                                                          ,AUD__FILL_FACT.PATIENT_NUM
                                                         ,AUD__FILL_FACT.PATIENT_PAY_AMOUNT
                                                          ,AUD__FILL_FACT.PRESCRIBED_DATE
                                                         ,AUD__FILL_FACT.PRESCRIBER_ADDRESS_NUM
                                                         ,AUD__FILL_FACT.PRESCRIBER_ID_DEA
                                                          ,AUD__FILL_FACT.PRESCRIBER_ID_NPI
                                                          ,AUD__FILL_FACT.PRESCRIBER_NUM
                                                          ,AUD__FILL_FACT.PRESCRIBER_STATE
                                                          ,AUD__FILL_FACT.PRESCRIBER_ZIP
                                                          ,AUD__FILL_FACT.QTY_DISPENSED
                                                          ,AUD__FILL_FACT.REFILL_NUM
                                                         ,AUD__FILL_FACT.REFILLS_AUTHORIZED
                                                          ,AUD__FILL_FACT.REFILLS_REMAINING
                                                          ,AUD__FILL_FACT.RX_FILL_SEQ
                                                          ,AUD__FILL_FACT.RX_NUMBER
                                                          ,AUD__FILL_FACT.RX_ORIGIN_CODE
                                                          ,AUD__FILL_FACT.RX_RECORD_NUM
                                                          ,AUD__FILL_FACT.SHORT_FILL_STATUS
                                                          ,AUD__FILL_FACT.SPLIT_BILLING_CODE
                                                         ,AUD__FILL_FACT.TLOG_SEQUENCE_NUM
                                                          ,AUD__FILL_FACT.TOTAL_COST
                                                          ,AUD__FILL_FACT.TOTAL_FEE
                                                          ,AUD__FILL_FACT.TOTAL_UANDC
                                                         ,AUD__FILL_FACT.TOTAL_USER_DEFINED
                                                         ,AUD__FILL_FACT.TP_CURR_PATIENT_PAY_AMOUNT
                                                          ,AUD__FILL_FACT.TP_CURR_TOTAL_COST
                                                         ,AUD__FILL_FACT.TP_CURR_TOTAL_FEE
                                                          ,AUD__FILL_FACT.TP_ITEM_CLAIM_KEY
                                                          ,AUD__FILL_FACT.TP_ITEM_CLAIM_NUM
                                                          ,AUD__FILL_FACT.TP_MESSAGE_KEY
                                                          ,AUD__FILL_FACT.TP_PATIENT_NUM
                                                         ,AUD__FILL_FACT.TP_PLAN_AMOUNT_PAID
                                                          ,AUD__FILL_FACT.TP_PLAN_NUM
                                                          ,AUD__FILL_FACT.TP_PRIOR_AUTH_NUM
                                                         ,AUD__FILL_FACT.TP_PROCESSOR_DEST_NUM
                                                          ,AUD__FILL_FACT.TRANSACTION_CODE
                                                          ,AUD__FILL_STATE.FILL_STATE_CHG_TS AS FS_FILL_STATE_CHG_TS
                                                          ,AUD__FILL_STATE.FILL_STATE_CODE AS FS_FILL_STATE_CODE
                                                          ,AUD__FILL_STATE.RX_FILL_SEQ AS FS_RX_FILL_SEQ
                                                          ,AUD__FILL_STATE.RX_RECORD_NUM AS FS_RX_RECORD_NUM
                                                          ,AUD__FILL_STATE.INGREDIENT_NDC1
                                                          ,AUD__FILL_STATE.INGREDIENT_NDC2
                                                          ,AUD__FILL_STATE.INGREDIENT_NDC3
                                                          ,AUD__FILL_STATE.INGREDIENT_NDC4
                                                          ,AUD__FILL_STATE.INGREDIENT_NDC5
                                                          ,AUD__FILL_STATE.INGREDIENT_NDC6
                                                         ,AUD__FILL_STATE.INGREDIENT_PRICE1
                                                         ,AUD__FILL_STATE.INGREDIENT_PRICE2
                                                         ,AUD__FILL_STATE.INGREDIENT_PRICE3
                                                         ,AUD__FILL_STATE.INGREDIENT_PRICE4
                                                         ,AUD__FILL_STATE.INGREDIENT_PRICE5
                                                         ,AUD__FILL_STATE.INGREDIENT_PRICE6
                                                          ,AUD__FILL_STATE.INGREDIENT_QTY1
                                                          ,AUD__FILL_STATE.INGREDIENT_QTY2
                                                          ,AUD__FILL_STATE.INGREDIENT_QTY3
                                                          ,AUD__FILL_STATE.INGREDIENT_QTY4
                                                          ,AUD__FILL_STATE.INGREDIENT_QTY5
                                                          ,AUD__FILL_STATE.INGREDIENT_QTY6
                                                          ,AUD__PATIENT.BIRTH_DATE
                                                          ,AUD__PATIENT.FIRST_NAME AS P_FIRST_NAME
                                                          ,AUD__PATIENT.GENDER AS P_GENDER
                                                          ,AUD__PATIENT.LAST_NAME AS P_LAST_NAME
                                                          ,AUD__PATIENT.MIDDLE_NAME AS P_MIDDLE_NAME
                                                          ,AUD__PATIENT_ADDRESS.ADDRESS1
                                                          ,AUD__PATIENT_ADDRESS.ADDRESS2
                                                          ,AUD__PATIENT_ADDRESS.CITY
                                                          ,AUD__PATIENT_ADDRESS.FIRST_NAME AS PA_FIRST_NAME
                                                          ,AUD__PATIENT_ADDRESS.GENDER AS PA_GENDER
                                                          ,AUD__PATIENT_ADDRESS.LAST_NAME AS PA_LAST_NAME
                                                         ,AUD__PATIENT_ADDRESS.MIDDLE_NAME AS PA_MIDDLE_NAME
                                                         ,AUD__PATIENT_ADDRESS.PHONE_NUM
                                                          ,AUD__PATIENT_ADDRESS.STATE
                                                          ,AUD__PATIENT_ADDRESS.ZIP
                                                         ,AUD__TP_ITEM_CLAIM.AUTHORIZATION_NUMBER
                                                         ,AUD__TP_ITEM_CLAIM.EMERGENCY_CONDITION_CODE
                                                         ,AUD__TP_ITEM_CLAIM.EXTERNAL_BILLING_INDICATOR
                                                          ,AUD__TP_MESSAGE.H_LEVEL       
                                                          ,AUD__TP_MESSAGE.TP_MESSAGE_NUM
                                                         ,AUD__TP_PROCESSOR_DESTINATION.BIN_NUMBER
                                           ,AUD__TP_PROCESSOR_DESTINATION.PROCESSOR_CONTROL_NUMBER
                                                ,AUD__TP_RESPONSE_MESSAGE.RS_BASIS_OF_REIMBRSMT_DET
                                           ,AUD__TP_RESPONSE_MESSAGE.RS_NETWRK_REIMBURSEMENT_ID
                                                          ,DW__DRUG.DRUG_GCN_SEQ
                                                          ,DW__DRUG.DRUG_IS_MAINT_DRUG
                                                          ,DW__DRUG.DRUG_NDC
                                                          ,DW__DRUG.DRUG_PACKAGE_SIZE
                                                          ,DW__DRUG.DRUG_STRENGTH
                                                          ,DW__DRUG.DRUG_STRENGTH_UNITS
                                                          ,DW__FACILITY.FD_FACILITY_ID
                                                         ,DW__FACILITY.FD_NCPDP_PROVIDER_ID
                                                        ,DW__MEDICAL_CONDITION.MCD_FRMTTD_COND_REF_NUM
                                                          ,DW__PRESCRIBER.PRS_FIRST_NAME
                                                          ,DW__PRESCRIBER.PRS_LAST_NAME
                                                         ,DW__PRESCRIBER.PRS_PRESCRIBER_KEY
                                                         ,DW__PRESCRIBER.PRS_PRESCRIBER_NUM
                                                          ,DW__PRESCRIBER_ADDRESS.PADR_KEY
                                                         ,DW__PRESCRIBER_ADDRESS.PADR_ADDRESS1
                                                         ,DW__PRESCRIBER_ADDRESS.PADR_ADDRESS2
                                                         ,DW__PRESCRIBER_ADDRESS.PADR_CITY
                                                          ,DW__PRODUCT.PRD_IS_COMPOUND
                                                          ,DW__PRODUCT.PRD_IS_GENERIC
                                                         ,DW__PRODUCT.PRD_IS_PRICE_MAINTAINED
                                                          ,DW__PRODUCT.PRD_NAME
                                                          ,DW__PRODUCT.PRD_PACKAGE_SIZE
                                                          ,DW__TP_PATIENT.PD_PATIENT_KEY
                                                         ,DW__TP_PATIENT.TPD_CARDHOLDER_ID
                                                         ,DW__TP_PATIENT.TPD_CARDHOLDER_PATIENT_NUM
                                                         ,DW__TP_PATIENT.TPD_RELATIONSHIP_NUM
                                                          ,DW__TP_PATIENT.TPD_TP_PLAN_NUM
                                                          ,DW__TP_PLAN.TPLD_PLAN_CODE
                                                          ,SUBSTR (TO_CHAR (AUD__TP_MESSAGE.REQUEST_MESSAGE), 1, 2000) AS REQUEST_MESSAGE
                                                          ,AUD__FILL_STATE.EFF_END_DATE AS EE_FILL_STATE
                                                          ,AUD__TP_MESSAGE.EFF_END_DATE AS EE_TP_MESSAGE
                                                          ,AUD__TP_ITEM_CLAIM.EFF_END_DATE AS EE_TP_ITEM_CLAIM
                                                          ,AUD__PATIENT.EFF_END_DATE AS EE_PATIENT
                                                          ,DW__TP_PATIENT.TPD_EFF_END_DATE AS EE_TP_PATIENT
                                                          ,CARDHOLDER.EFF_END_DATE AS EE_CARDHOLDER
                                                          ,AUD__TP_PRIOR_AUTH.EFF_END_DATE AS EE_TP_PRIOR_AUTH
                                                         ,AUD__TP_PROCESSOR_DESTINATION.EFF_END_DATE AS EE_TP_PROCESSOR_DESTINATION
                                                          ,AUD__FILL_STATE.ORDER_NUM

                                           FROM {AUD}.ERXAUD_PLS_ARCHIVE_VIEW.FILL_FACT AUD__FILL_FACT
                                                          INNER JOIN {DW}.ERXDW_PLS_ARCHIVE_VIEW.FACILITY DW__FACILITY
                                                                        ON DW__FACILITY.FD_FACILITY_NUM = AUD__FILL_FACT.FACILITY_NUM
                                                             AND DW__FACILITY.FD_IS_PPI_ENABLED = 'Y'
                                                          --NOTE: DW__FACILITY.FD_NCPDP_PROVIDER_ID IS THE NCPDP, WHICH CAN BE OBTAINED WITH THE FOLLOWING JOIN IF NEEDED.
                                                          --INNER JOIN {DW}.ERXDW_PLS_ARCHIVE_VIEW.FACILITY_ID DW__FACILITY_ID
                                                          --    ON DW__FACILITY_ID.FD_FACILITY_KEY = AUD__FILL_FACT.FACILITY_NUM
                                                          --   AND DW__FACILITY_ID.FD_TYPE = 'F08'
                                                                        --NOTE: DW__FACILITY_ID.FD_VALUE IS THE NPI
                                                          INNER JOIN {AUD}.ERXAUD_PLS_ARCHIVE_VIEW.FILL_STATE AUD__FILL_STATE
                                                                        ON AUD__FILL_STATE.H_LEVEL = AUD__FILL_FACT.FILL_STATE_KEY
                                                             AND AUD__FILL_STATE.RX_RECORD_NUM = AUD__FILL_FACT.RX_RECORD_NUM
                                                             AND AUD__FILL_STATE.RX_FILL_SEQ = AUD__FILL_FACT.RX_FILL_SEQ
                                                          INNER JOIN {DW}.ERXDW_PLS_ARCHIVE_VIEW.PRODUCT DW__PRODUCT
                                                                        ON DW__PRODUCT.PRD_PRODUCT_NUM = AUD__FILL_FACT.DISPENSED_PROD_NUM
                                                          INNER JOIN {DW}.ERXDW_PLS_ARCHIVE_VIEW.DRUG DW__DRUG
                                                                        ON DW__DRUG.DRUG_NUM = AUD__FILL_FACT.DISPENSED_DRUG_NUM
                                                          INNER JOIN {DW}.ERXDW_PLS_ARCHIVE_VIEW.PRESCRIBER DW__PRESCRIBER
                                                                        ON DW__PRESCRIBER.PRS_PRESCRIBER_NUM = AUD__FILL_FACT.PRESCRIBER_NUM
                                                          LEFT OUTER JOIN {DW}.ERXDW_PLS_ARCHIVE_VIEW.PRESCRIBER_ADDRESS DW__PRESCRIBER_ADDRESS
                                                                        ON DW__PRESCRIBER_ADDRESS.PADR_PRESCRIBER_ADDRESS_NUM = AUD__FILL_FACT.PRESCRIBER_ADDRESS_NUM
                                                          INNER JOIN {AUD}.ERXAUD_PLS_ARCHIVE_VIEW.PATIENT AUD__PATIENT
                                                                        ON AUD__PATIENT.PATIENT_NUM = AUD__FILL_FACT.PATIENT_NUM
                                                             AND AUD__PATIENT.EFF_END_DATE = TO_DATE('12/31/2999', 'MM/DD/YYYY')
                                                          LEFT OUTER JOIN {AUD}.ERXAUD_PLS_ARCHIVE_VIEW.PATIENT_ADDRESS AUD__PATIENT_ADDRESS
                                                                        ON AUD__PATIENT_ADDRESS.PATIENT_NUM = AUD__FILL_FACT.PATIENT_NUM
                                                          LEFT OUTER JOIN {DW}.ERXDW_PLS_ARCHIVE_VIEW.TP_PATIENT DW__TP_PATIENT
                                                                        ON DW__TP_PATIENT.TPD_TP_PATIENT_NUM = AUD__FILL_FACT.TP_PATIENT_NUM
                                                             AND DW__TP_PATIENT.TPD_EFF_END_DATE = TO_DATE('12/31/2999', 'MM/DD/YYYY')
                                                          LEFT OUTER JOIN {AUD}.ERXAUD_PLS_ARCHIVE_VIEW.TP_MESSAGE AUD__TP_MESSAGE
                                                                        ON AUD__TP_MESSAGE.H_LEVEL = AUD__FILL_FACT.TP_MESSAGE_KEY
                                                             AND AUD__TP_MESSAGE.TP_ITEM_CLAIM_NUM = AUD__FILL_FACT.TP_ITEM_CLAIM_NUM
                                                             AND AUD__TP_MESSAGE.EFF_END_DATE = TO_DATE('12/31/2999', 'MM/DD/YYYY')
                                                          LEFT OUTER JOIN {DW}.ERXDW_PLS_ARCHIVE_VIEW.MEDICAL_CONDITION DW__MEDICAL_CONDITION
                                                                        ON DW__MEDICAL_CONDITION.MCD_MED_CONDITION_NUM = AUD__FILL_FACT.MEDICAL_CONDITION_NUM
                                                          LEFT OUTER JOIN {DW}.ERXDW_PLS_ARCHIVE_VIEW.TP_PLAN DW__TP_PLAN
                                                                        ON DW__TP_PLAN.TPLD_PLAN_NUM = AUD__FILL_FACT.TP_PLAN_NUM
                                                          LEFT OUTER JOIN {AUD}.ERXAUD_PLS_ARCHIVE_VIEW.PATIENT CARDHOLDER
                                                                        ON CARDHOLDER.PATIENT_NUM = DW__TP_PATIENT.TPD_CARDHOLDER_PATIENT_NUM
                                                             AND CARDHOLDER.EFF_END_DATE = TO_DATE('12/31/2999', 'MM/DD/YYYY')
                                                          LEFT OUTER JOIN {AUD}.ERXAUD_PLS_ARCHIVE_VIEW.TP_PRIOR_AUTH AUD__TP_PRIOR_AUTH
                                                                        ON AUD__TP_PRIOR_AUTH.TP_PRIOR_AUTH_NUM = AUD__FILL_FACT.TP_PRIOR_AUTH_NUM
                                                             AND AUD__TP_PRIOR_AUTH.EFF_END_DATE = TO_DATE('12/31/2999', 'MM/DD/YYYY')
                                                          LEFT OUTER JOIN {AUD}.ERXAUD_PLS_ARCHIVE_VIEW.TP_PROCESSOR_DESTINATION AUD__TP_PROCESSOR_DESTINATION
                                                                        ON AUD__TP_PROCESSOR_DESTINATION.TP_PROCESSOR_DESTINATION_SEQ = AUD__FILL_FACT.TP_PROCESSOR_DEST_NUM
                                                             AND AUD__TP_PROCESSOR_DESTINATION.EFF_END_DATE = TO_DATE('12/31/2999', 'MM/DD/YYYY')
                                                          LEFT OUTER JOIN {AUD}.ERXAUD_PLS_ARCHIVE_VIEW.TP_ITEM_CLAIM AUD__TP_ITEM_CLAIM
                                                                        ON AUD__TP_ITEM_CLAIM.TP_ITEM_CLAIM_NUM = AUD__FILL_FACT.TP_ITEM_CLAIM_NUM
                                                             AND AUD__TP_ITEM_CLAIM.H_LEVEL = AUD__FILL_FACT.TP_ITEM_CLAIM_KEY
                                                             AND AUD__TP_ITEM_CLAIM.EFF_END_DATE = TO_DATE('12/31/2999', 'MM/DD/YYYY')
                                                          LEFT OUTER JOIN {AUD}.ERXAUD_PLS_ARCHIVE_VIEW.TP_RESPONSE_MESSAGE AUD__TP_RESPONSE_MESSAGE
                                                                        ON AUD__TP_RESPONSE_MESSAGE.TP_MESSAGE_NUM = AUD__TP_MESSAGE.TP_MESSAGE_NUM
                                                             AND AUD__TP_RESPONSE_MESSAGE.H_LEVEL = AUD__TP_MESSAGE.H_LEVEL

                   )
    WHERE FILL_STATE_CHG_TS BETWEEN (:RUNDATE - 1) AND (:RUNDATE - 1 + INTERVAL '23 HOURS, 59 MINUTES, 59 SECONDS')
      AND FILL_STATE_CODE IN (3, 4)
      AND IS_SAME_DAY_REVERSAL = 'N'
      --AND ROWNUM < 10
--) SELECT COUNT(*) FROM INSERT_INTO_STAGING
--TIME TO GET ALL [36593] ROWS IN [159] SECONDS
), UPDATE_STAGING_CF_INDICATOR AS (
    SELECT 
            TPCS.*,
            (CASE WHEN I.TARGET_CF_FACILITY_NUM IS NULL THEN 'N' ELSE 'Y' END) AS CF_STORE_INDICATOR
        FROM INSERT_INTO_STAGING TPCS
             LEFT OUTER JOIN {AUD}.ERXAUD_PLS_ARCHIVE_VIEW.ITEM I
                 ON I.RX_RECORD_NUM = TPCS.RX_RECORD_NUM
                AND I.RX_FILL_SEQ = TPCS.RX_FILL_SEQ
                AND I.ORDER_NUM = TPCS.ORDER_NUM
                AND I.EFF_END_DATE = TO_DATE('12/31/2999', 'MM/DD/YYYY')
                AND I.H_TYPE <> 'D'
--) SELECT COUNT(*) FROM UPDATE_STAGING_CF_INDICATOR
--(A) TIME TO GET ALL [36593] ROWS IN [159] SECONDS
--(B) TIME TO GET ALL [36593] ROWS IN [157] SECONDS
), UPDATE_STAGING_ADJUDICATION_DT AS (
        SELECT  
            TPCS.*,
            TEMP_TABLE.FIRST_ADJUDICATED_DATE
        FROM UPDATE_STAGING_CF_INDICATOR TPCS
             LEFT OUTER JOIN
             (SELECT  
                  RX_RECORD_NUM,
                  RX_FILL_SEQ,
                  MIN(FS.FILL_STATE_CHG_TS) AS FIRST_ADJUDICATED_DATE
              FROM {AUD}.ERXAUD_PLS_ARCHIVE_VIEW.FILL_STATE FS
              WHERE FS.FILL_STATE_CODE = 3
              GROUP BY RX_RECORD_NUM, RX_FILL_SEQ
             ) TEMP_TABLE
             ON TEMP_TABLE.RX_RECORD_NUM = TPCS.RX_RECORD_NUM
            AND TEMP_TABLE.RX_FILL_SEQ = TPCS.RX_FILL_SEQ
--) SELECT COUNT(*) FROM UPDATE_STAGING_ADJUDICATION_DT
--(A) TIME TO GET ALL [36593] ROWS IN [159] SECONDS
--(B) TIME TO GET ALL [36593] ROWS IN [157] SECONDS
--(C) TIME TO GET ALL [36593] ROWS IN [155] SECONDS
), UPDATE_STAGING_DISPENSED_DT AS (
        SELECT 
        --WE TRIED IN_RX_07
            TPCS.*,
            MIN(RF.DISPENSE_DATE) AS FIRST_DISPENSE_DATE
        FROM UPDATE_STAGING_ADJUDICATION_DT TPCS
             LEFT OUTER JOIN {AUD}.ERXAUD_PLS_ARCHIVE_VIEW.RX_FILL RF
               ON RF.RX_RECORD_NUM = TPCS.RX_RECORD_NUM


              --AND RF.EFF_START_DATE BETWEEN (TPCS.RUN_DATE -365) AND TPCS.RUN_DATE
              --AND RF.EFF_END_DATE = TO_DATE('12/31/2999', 'MM/DD/YYYY') --REMOVE HERE IF QA FINDS ISSUES WITH FIRST DISP DT
              --TRY THESE NEXT TWO LINES INSTEAD OF ABOVE - 2999
              --AND RF.EFF_START_DATE <= TPCS.FILL_STATE_CHG_TS
              --AND RF.EFF_END_DATE > TPCS.FILL_STATE_CHG_TS
        GROUP BY
        TPCS.BENEFIT_GROUP_ID
        ,TPCS.DATE_OF_SERVICE
        ,TPCS.DAW_CODE
        ,TPCS.DAYS_SUPPLY
        ,TPCS.DISPENSED_DRUG_NUM
        ,TPCS.DISPENSED_QTY_AWP
        ,TPCS.FACILITY_NUM
        ,TPCS.FILL_STATE_CHG_TS
        ,TPCS.FILL_STATE_CODE
        ,TPCS.FILL_STATE_KEY
        ,TPCS.FILL_STATE_PRICE_NUM
        ,TPCS.FINAL_PRICE
        ,TPCS.INCENTIVE_FEE_AMOUNT
        ,TPCS.INTERNAL_FILL_ID
        ,TPCS.IS_SAME_DAY_REVERSAL
        ,TPCS.MEDICAL_CONDITION_NUM
        ,TPCS.ORIGINAL_PRODUCT_QTY
        ,TPCS.PARTIAL_FILL_SEQ
        ,TPCS.PATIENT_NUM
        ,TPCS.PATIENT_PAY_AMOUNT
        ,TPCS.PRESCRIBED_DATE
        ,TPCS.PRESCRIBER_ADDRESS_NUM
        ,TPCS.PRESCRIBER_ID_DEA
        ,TPCS.PRESCRIBER_ID_NPI
        ,TPCS.PRESCRIBER_NUM
        ,TPCS.PRESCRIBER_STATE
        ,TPCS.PRESCRIBER_ZIP
        ,TPCS.QTY_DISPENSED
        ,TPCS.REFILL_NUM
        ,TPCS.REFILLS_AUTHORIZED
        ,TPCS.REFILLS_REMAINING
        ,TPCS.RX_FILL_SEQ
        ,TPCS.RX_NUMBER
        ,TPCS.RX_ORIGIN_CODE
        ,TPCS.RX_RECORD_NUM
        ,TPCS.SHORT_FILL_STATUS
        ,TPCS.SPLIT_BILLING_CODE
        ,TPCS.TLOG_SEQUENCE_NUM
        ,TPCS.TOTAL_COST
        ,TPCS.TOTAL_FEE
        ,TPCS.TOTAL_UANDC
        ,TPCS.TOTAL_USER_DEFINED
        ,TPCS.TP_CURR_PATIENT_PAY_AMOUNT
        ,TPCS.TP_CURR_TOTAL_COST
        ,TPCS.TP_CURR_TOTAL_FEE
        ,TPCS.TP_ITEM_CLAIM_KEY
        ,TPCS.TP_ITEM_CLAIM_NUM
        ,TPCS.TP_MESSAGE_KEY
        ,TPCS.TP_PATIENT_NUM
        ,TPCS.TP_PLAN_AMOUNT_PAID
        ,TPCS.TP_PLAN_NUM
        ,TPCS.TP_PRIOR_AUTH_NUM
        ,TPCS.TP_PROCESSOR_DEST_NUM
        ,TPCS.TRANSACTION_CODE
        ,TPCS.FS_FILL_STATE_CHG_TS
        ,TPCS.FS_FILL_STATE_CODE
        ,TPCS.FS_RX_FILL_SEQ
        ,TPCS.FS_RX_RECORD_NUM
        ,TPCS.INGREDIENT_NDC1
        ,TPCS.INGREDIENT_NDC2
        ,TPCS.INGREDIENT_NDC3
        ,TPCS.INGREDIENT_NDC4
        ,TPCS.INGREDIENT_NDC5
        ,TPCS.INGREDIENT_NDC6
        ,TPCS.INGREDIENT_PRICE1
        ,TPCS.INGREDIENT_PRICE2
        ,TPCS.INGREDIENT_PRICE3
        ,TPCS.INGREDIENT_PRICE4
        ,TPCS.INGREDIENT_PRICE5
        ,TPCS.INGREDIENT_PRICE6
        ,TPCS.INGREDIENT_QTY1
        ,TPCS.INGREDIENT_QTY2
        ,TPCS.INGREDIENT_QTY3
        ,TPCS.INGREDIENT_QTY4
        ,TPCS.INGREDIENT_QTY5
        ,TPCS.INGREDIENT_QTY6
        ,TPCS.BIRTH_DATE
        ,TPCS.P_FIRST_NAME
        ,TPCS.P_GENDER
        ,TPCS.P_LAST_NAME
        ,TPCS.P_MIDDLE_NAME
        ,TPCS.ADDRESS1
        ,TPCS.ADDRESS2
        ,TPCS.CITY
        ,TPCS.PA_FIRST_NAME
        ,TPCS.PA_GENDER
        ,TPCS.PA_LAST_NAME
        ,TPCS.PA_MIDDLE_NAME
        ,TPCS.PHONE_NUM
        ,TPCS.STATE
        ,TPCS.ZIP
        ,TPCS.AUTHORIZATION_NUMBER
        ,TPCS.EMERGENCY_CONDITION_CODE
        ,TPCS.EXTERNAL_BILLING_INDICATOR
        ,TPCS.H_LEVEL
        ,TPCS.TP_MESSAGE_NUM
        ,TPCS.BIN_NUMBER
        ,TPCS.PROCESSOR_CONTROL_NUMBER
        ,TPCS.RS_BASIS_OF_REIMBRSMT_DET
        ,TPCS.RS_NETWRK_REIMBURSEMENT_ID
        ,TPCS.DRUG_GCN_SEQ
        ,TPCS.DRUG_IS_MAINT_DRUG
        ,TPCS.DRUG_NDC
        ,TPCS.DRUG_PACKAGE_SIZE
        ,TPCS.DRUG_STRENGTH
        ,TPCS.DRUG_STRENGTH_UNITS
        ,TPCS.FD_FACILITY_ID
        ,TPCS.FD_NCPDP_PROVIDER_ID
        ,TPCS.MCD_FRMTTD_COND_REF_NUM
        ,TPCS.PRS_FIRST_NAME
        ,TPCS.PRS_LAST_NAME
        ,TPCS.PRS_PRESCRIBER_KEY
        ,TPCS.PRS_PRESCRIBER_NUM
        ,TPCS.PADR_KEY
        ,TPCS.PADR_ADDRESS1
        ,TPCS.PADR_ADDRESS2
        ,TPCS.PADR_CITY
        ,TPCS.PRD_IS_COMPOUND
        ,TPCS.PRD_IS_GENERIC
        ,TPCS.PRD_IS_PRICE_MAINTAINED
        ,TPCS.PRD_NAME
        ,TPCS.PRD_PACKAGE_SIZE
        ,TPCS.PD_PATIENT_KEY
        ,TPCS.TPD_CARDHOLDER_ID
        ,TPCS.TPD_CARDHOLDER_PATIENT_NUM
        ,TPCS.TPD_RELATIONSHIP_NUM
        ,TPCS.TPD_TP_PLAN_NUM
        ,TPCS.TPLD_PLAN_CODE
        ,TPCS.REQUEST_MESSAGE
        ,TPCS.CF_STORE_INDICATOR
        ,TPCS.FIRST_ADJUDICATED_DATE
        ,TPCS.LOOKUPDAYSSUPPLY
        ,TPCS.LOOKUPQTYDISPENSED
        ,TPCS.ISSENIOR
        ,TPCS.ORDER_NUM
--) SELECT COUNT(*) FROM UPDATE_STAGING_DISPENSED_DT
--(A) TIME TO GET ALL [36593] ROWS IN [159] SECONDS
--(B) TIME TO GET ALL [36593] ROWS IN [157] SECONDS
--(C) TIME TO GET ALL [36593] ROWS IN [155] SECONDS
--(D) TIME TO GET ALL [36593] ROWS IN [280] SECONDS
), UPDATE_STAGING_SUBMITTED_DT AS (
    SELECT 
        TPCS.*,
        TEMP_TABLE.TRANSMISSION_DATE
    FROM UPDATE_STAGING_DISPENSED_DT TPCS
        LEFT OUTER JOIN
        (SELECT 
            FS.RX_RECORD_NUM,
            FS.RX_FILL_SEQ,
            MAX(FS.FILL_STATE_CHG_TS) AS TRANSMISSION_DATE
         FROM {AUD}.ERXAUD_PLS_ARCHIVE_VIEW.FILL_STATE FS
         WHERE FS.FILL_STATE_CODE IN (3, 4)
           AND FS.FILL_STATE_CHG_TS BETWEEN (:RUNDATE - 1)
                                        AND (:RUNDATE + INTERVAL '23 HOURS, 59 MINUTES, 59 SECONDS')
         GROUP BY FS.RX_RECORD_NUM, FS.RX_FILL_SEQ
         ) TEMP_TABLE
            ON TEMP_TABLE.RX_RECORD_NUM = TPCS.RX_RECORD_NUM
           AND TEMP_TABLE.RX_FILL_SEQ = TPCS.RX_FILL_SEQ
), UPDATE_STAGING_QTY_SUPPLY AS (
        SELECT 
            TPCS.*,
            SUM(RF.QTY_DISPENSED) AS QTY_DISPENSED,
            SUM(RF.DAYS_SUPPLY) AS DAYS_SUPPLY
        FROM UPDATE_STAGING_SUBMITTED_DT TPCS
             LEFT OUTER JOIN {AUD}.ERXAUD_PLS_ARCHIVE_VIEW.RX_FILL RF
               ON RF.RX_RECORD_NUM = TPCS.RX_RECORD_NUM
              AND RF.INTERNAL_FILL_ID = TPCS.INTERNAL_FILL_ID
              AND RF.EFF_START_DATE <= TPCS.FILL_STATE_CHG_TS
              AND RF.EFF_END_DATE > TPCS.FILL_STATE_CHG_TS
              AND RF.FILL_STATUS_NUM IN (1, 2)               
              ------AND RF.EFF_END_DATE = TO_DATE('12/31/2999', 'MM/DD/YYYY') --REMOVE HERE IF QA FINDS ISSUES WITH FIRST DISP DT
              AND TPCS.SHORT_FILL_STATUS IN ('P','C')
              AND NVL(TPCS.TP_PLAN_NUM,0) != 0
        GROUP BY
        TPCS.BENEFIT_GROUP_ID
        ,TPCS.DATE_OF_SERVICE
        ,TPCS.DAW_CODE
        ,TPCS.DAYS_SUPPLY
        ,TPCS.DISPENSED_DRUG_NUM
        ,TPCS.DISPENSED_QTY_AWP
        ,TPCS.FACILITY_NUM
        ,TPCS.FILL_STATE_CHG_TS
        ,TPCS.FILL_STATE_CODE
        ,TPCS.FILL_STATE_KEY
        ,TPCS.FILL_STATE_PRICE_NUM
        ,TPCS.FINAL_PRICE
        ,TPCS.INCENTIVE_FEE_AMOUNT
        ,TPCS.INTERNAL_FILL_ID
        ,TPCS.IS_SAME_DAY_REVERSAL
        ,TPCS.MEDICAL_CONDITION_NUM
        ,TPCS.ORIGINAL_PRODUCT_QTY
        ,TPCS.PARTIAL_FILL_SEQ
        ,TPCS.PATIENT_NUM
        ,TPCS.PATIENT_PAY_AMOUNT
        ,TPCS.PRESCRIBED_DATE
        ,TPCS.PRESCRIBER_ADDRESS_NUM
        ,TPCS.PRESCRIBER_ID_DEA
        ,TPCS.PRESCRIBER_ID_NPI
        ,TPCS.PRESCRIBER_NUM
        ,TPCS.PRESCRIBER_STATE
        ,TPCS.PRESCRIBER_ZIP
        ,TPCS.QTY_DISPENSED
        ,TPCS.REFILL_NUM
        ,TPCS.REFILLS_AUTHORIZED
        ,TPCS.REFILLS_REMAINING
        ,TPCS.RX_FILL_SEQ
        ,TPCS.RX_NUMBER
        ,TPCS.RX_ORIGIN_CODE
        ,TPCS.RX_RECORD_NUM
        ,TPCS.SHORT_FILL_STATUS
        ,TPCS.SPLIT_BILLING_CODE
        ,TPCS.TLOG_SEQUENCE_NUM
        ,TPCS.TOTAL_COST
        ,TPCS.TOTAL_FEE
        ,TPCS.TOTAL_UANDC
        ,TPCS.TOTAL_USER_DEFINED
        ,TPCS.TP_CURR_PATIENT_PAY_AMOUNT
        ,TPCS.TP_CURR_TOTAL_COST
        ,TPCS.TP_CURR_TOTAL_FEE
        ,TPCS.TP_ITEM_CLAIM_KEY
        ,TPCS.TP_ITEM_CLAIM_NUM
        ,TPCS.TP_MESSAGE_KEY
        ,TPCS.TP_PATIENT_NUM
        ,TPCS.TP_PLAN_AMOUNT_PAID
        ,TPCS.TP_PLAN_NUM
        ,TPCS.TP_PRIOR_AUTH_NUM
        ,TPCS.TP_PROCESSOR_DEST_NUM
        ,TPCS.TRANSACTION_CODE
        ,TPCS.FS_FILL_STATE_CHG_TS
        ,TPCS.FS_FILL_STATE_CODE
        ,TPCS.FS_RX_FILL_SEQ
        ,TPCS.FS_RX_RECORD_NUM
        ,TPCS.INGREDIENT_NDC1
        ,TPCS.INGREDIENT_NDC2
        ,TPCS.INGREDIENT_NDC3
        ,TPCS.INGREDIENT_NDC4
        ,TPCS.INGREDIENT_NDC5
        ,TPCS.INGREDIENT_NDC6
        ,TPCS.INGREDIENT_PRICE1
        ,TPCS.INGREDIENT_PRICE2
        ,TPCS.INGREDIENT_PRICE3
        ,TPCS.INGREDIENT_PRICE4
        ,TPCS.INGREDIENT_PRICE5
        ,TPCS.INGREDIENT_PRICE6
        ,TPCS.INGREDIENT_QTY1
        ,TPCS.INGREDIENT_QTY2
        ,TPCS.INGREDIENT_QTY3
        ,TPCS.INGREDIENT_QTY4
        ,TPCS.INGREDIENT_QTY5
        ,TPCS.INGREDIENT_QTY6
        ,TPCS.BIRTH_DATE
        ,TPCS.P_FIRST_NAME
        ,TPCS.P_GENDER
        ,TPCS.P_LAST_NAME
        ,TPCS.P_MIDDLE_NAME
        ,TPCS.ADDRESS1
        ,TPCS.ADDRESS2
        ,TPCS.CITY
        ,TPCS.PA_FIRST_NAME
        ,TPCS.PA_GENDER
        ,TPCS.PA_LAST_NAME
        ,TPCS.PA_MIDDLE_NAME
        ,TPCS.PHONE_NUM
        ,TPCS.STATE
        ,TPCS.ZIP
        ,TPCS.AUTHORIZATION_NUMBER
        ,TPCS.EMERGENCY_CONDITION_CODE
        ,TPCS.EXTERNAL_BILLING_INDICATOR
        ,TPCS.H_LEVEL
        ,TPCS.TP_MESSAGE_NUM
        ,TPCS.BIN_NUMBER
        ,TPCS.PROCESSOR_CONTROL_NUMBER
        ,TPCS.RS_BASIS_OF_REIMBRSMT_DET
        ,TPCS.RS_NETWRK_REIMBURSEMENT_ID
        ,TPCS.DRUG_GCN_SEQ
        ,TPCS.DRUG_IS_MAINT_DRUG
        ,TPCS.DRUG_NDC
        ,TPCS.DRUG_PACKAGE_SIZE
        ,TPCS.DRUG_STRENGTH
        ,TPCS.DRUG_STRENGTH_UNITS
        ,TPCS.FD_FACILITY_ID
        ,TPCS.FD_NCPDP_PROVIDER_ID
        ,TPCS.MCD_FRMTTD_COND_REF_NUM
        ,TPCS.PRS_FIRST_NAME
        ,TPCS.PRS_LAST_NAME
        ,TPCS.PRS_PRESCRIBER_KEY
        ,TPCS.PRS_PRESCRIBER_NUM
        ,TPCS.PADR_KEY
        ,TPCS.PADR_ADDRESS1
        ,TPCS.PADR_ADDRESS2
        ,TPCS.PADR_CITY
        ,TPCS.PRD_IS_COMPOUND
        ,TPCS.PRD_IS_GENERIC
        ,TPCS.PRD_IS_PRICE_MAINTAINED
        ,TPCS.PRD_NAME
        ,TPCS.PRD_PACKAGE_SIZE
        ,TPCS.PD_PATIENT_KEY
        ,TPCS.TPD_CARDHOLDER_ID
        ,TPCS.TPD_CARDHOLDER_PATIENT_NUM
        ,TPCS.TPD_RELATIONSHIP_NUM
        ,TPCS.TPD_TP_PLAN_NUM
        ,TPCS.TPLD_PLAN_CODE
        ,TPCS.REQUEST_MESSAGE
        ,TPCS.CF_STORE_INDICATOR
        ,TPCS.FIRST_ADJUDICATED_DATE
        ,TPCS.FIRST_DISPENSE_DATE
        ,TPCS.TRANSMISSION_DATE
        ,TPCS.LOOKUPDAYSSUPPLY
        ,TPCS.LOOKUPQTYDISPENSED
        ,TPCS.ISSENIOR
        ,TPCS.ORDER_NUM
)
--SELECT * FROM UPDATE_STAGING_SUBMITTED_DT
SELECT 
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
        ,TRANSMISSION_DATE
        ,FIRST_ADJUDICATED_DATE
        ,FIRST_DISPENSE_DATE
        ,LOOKUPQTYDISPENSED
        ,LOOKUPDAYSSUPPLY
        ,LOOKUPACQUISITIONCOST
        ,CF_STORE_INDICATOR
        ,ISSENIOR
        ,ORDER_NUM
FROM UPDATE_STAGING_SUBMITTED_DT