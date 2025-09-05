WITH maybe_full_refunds AS (
    SELECT /*+ optimizer_features_enable('12.1.0.2') */ Distinct
         p.payment_num
        ,p.reversal_payment_num
        ,p.payment_amount
        ,p.health_card_designation
        ,p.cancelled_date
        ,p.completion_date
        ,p.payment_type_num
        ,pf.payment_type_key
        ,p.account_member_credit_card_num
        ,pf.facility_order_number
        ,pf.rx_number
        ,pf.refill_num
        ,f.fd_facility_num
        ,f.fd_facility_key
        ,ff.fill_state_chg_ts
        ,ff.fill_state_code
        ,NULL AS is_cob
    FROM TREXONE_AUD_DATA.Payment p
         INNER JOIN TREXONE_DW_DATA.PAYMENT_FACT pf
           ON pf.payment_num = p.payment_num
          AND pf.is_active = 'Y'
         INNER JOIN TREXONE_DW_DATA.Facility f
            ON f.fd_facility_key = pf.fd_facility_key
         INNER JOIN TREXONE_AUD_DATA.Fill_Fact ff
            ON ff.facility_num = f.fd_facility_num
           AND ff.rx_number = pf.rx_number
           AND ff.refill_num = pf.refill_num
           ---------------------------------------AND ff.partial_fill_seq IS NULL
           AND ff.rx_fill_seq = pf.rx_fill_seq
           AND p.completion_date <= ff.fill_state_chg_ts
																																   
           AND ff.fill_state_chg_ts <= (To_Date(:QueryDateStr,'YYYYMMDD') + INTERVAL '23:59:59' HOUR TO SECOND)
           --AND ff.fill_state_chg_ts <= (:QueryDateDt + INTERVAL '23:59:59' HOUR TO SECOND)
    WHERE ff.fill_state_code = '16'
      AND pf.payment_type_key = 7
      AND p.eff_end_date = To_Date('29991231','YYYYMMDD')
																															  
      AND p.completion_date BETWEEN To_Date(:QueryDateStr,'YYYYMMDD') AND (To_Date(:QueryDateStr,'YYYYMMDD') + INTERVAL '23:59:59' HOUR TO SECOND)
      --AND p.completion_date BETWEEN :QueryDateDt AND (:QueryDateDt + INTERVAL '23:59:59' HOUR TO SECOND)

    UNION ALL
    
    SELECT Distinct /*+ cardinality(ff,1) leading (ff) index (ff IN_FILL_FACT_03) optimizer_features_enable('12.1.0.2') */
         NULL AS payment_num
        ,NULL AS reversal_payment_num
        ,NULL AS payment_amount
        ,NULL AS health_card_designation
        ,NULL AS cancelled_date
        ,NULL AS completion_date
        ,NULL AS payment_type_num
        ,NULL AS payment_type_key
        ,NULL AS account_member_credit_card_num
        ,NULL AS facility_order_number
        ,ff.rx_number
        ,ff.refill_num
        ,f.fd_facility_num
        ,f.fd_facility_key
        ,ff.fill_state_chg_ts
        ,ff.fill_state_code
        ,NULL AS is_cob
    FROM TREXONE_AUD_DATA.Fill_Fact ff
         INNER JOIN TREXONE_DW_DATA.Facility f
            ON f.fd_facility_num = ff.facility_num
           AND f.FD_IS_PPI_ENABLED = 'Y'
    --WHERE ff.fill_state_chg_ts BETWEEN To_Date(:QueryDateStr,'YYYYMMDD') AND (To_Date(:QueryDateStr,'YYYYMMDD') + INTERVAL '23:59:59' HOUR TO SECOND)
    WHERE ff.fill_state_chg_ts BETWEEN :QueryDateDt AND (:QueryDateDt + INTERVAL '23:59:59' HOUR TO SECOND)
      AND ff.fill_state_code = '16'
      AND EXISTS (
               SELECT /*+ optimizer_features_enable('12.1.0.2') */ *
               FROM TREXONE_DW_DATA.Fill_Fact dw_ff
               WHERE dw_ff.sold_date_key BETWEEN To_Number(To_Char((:QueryDateDt - 100),'YYYYMMDD'))
                                             AND To_Number(To_Char((:QueryDateDt + 1),'YYYYMMDD'))
                 AND dw_ff.fd_facility_key = f.fd_facility_key
                 AND dw_ff.rx_number = ff.rx_number
                 AND dw_ff.refill_num = ff.refill_num
                 AND (
                       (dw_ff.partial_fill_seq = ff.partial_fill_seq)
                       OR
                       (dw_ff.partial_fill_seq IS NULL 
                        AND
                        ff.partial_fill_seq IS NULL)
                     )
                 AND dw_ff.rx_record_num = ff.rx_record_num
                 AND dw_ff.rx_fill_seq = ff.rx_fill_seq
                 AND dw_ff.is_mail = 'Y'
          )

    UNION ALL
    
    SELECT Distinct /*+ cardinality(ff,1) leading (ff) index (ff IN_FILL_FACT_03) optimizer_features_enable('12.1.0.2') */
         NULL AS payment_num
        ,NULL AS reversal_payment_num
        ,NULL AS payment_amount
        ,NULL AS health_card_designation
        ,NULL AS cancelled_date
        ,NULL AS completion_date
        ,NULL AS payment_type_num
        ,NULL AS payment_type_key
        ,NULL AS account_member_credit_card_num
        ,NULL AS facility_order_number
        ,ff.rx_number
        ,ff.refill_num
        ,f.fd_facility_num
        ,f.fd_facility_key
        ,ff.fill_state_chg_ts
        ,ff.fill_state_code
        ,'Y' AS is_cob
    FROM TREXONE_AUD_DATA.Fill_Fact ff
         INNER JOIN TREXONE_DW_DATA.Facility f
            ON f.fd_facility_num = ff.facility_num
           AND f.FD_IS_PPI_ENABLED = 'Y'
    --WHERE ff.fill_state_chg_ts BETWEEN To_Date(:QueryDateStr,'YYYYMMDD') AND (To_Date(:QueryDateStr,'YYYYMMDD') + INTERVAL '23:59:59' HOUR TO SECOND)
    --NOTE that the query execution explain plan is VERYYYY BAD when using bind variable QueryDateStr, but super fast with QueryDateDt
    WHERE ff.fill_state_chg_ts BETWEEN :QueryDateDt AND (:QueryDateDt + INTERVAL '23:59:59' HOUR TO SECOND)
      AND ff.fill_state_code = '4'
      AND ff.split_billing_code = '0'
      AND ff.has_reversal = 'Y'
      AND (f.fd_facility_key, ff.rx_number, ff.refill_num) IN (
               SELECT /*+ optimizer_features_enable('12.1.0.2') */ dw_ff.fd_facility_key, dw_ff.rx_number, dw_ff.refill_num
               FROM TREXONE_DW_DATA.Fill_Fact dw_ff
               --WHERE dw_ff.sold_date_key = To_Number(To_Char(To_Date(:QueryDateStr,'YYYYMMDD'),'YYYYMMDD'))
               WHERE dw_ff.sold_date_key = To_Number(To_Char(:QueryDateDt,'YYYYMMDD'))
                 AND dw_ff.is_mail = 'Y'
                 AND dw_ff.is_cob = 'Y'
          )
), maybe_partial_refunds AS (
    SELECT /*+ optimizer_features_enable('12.1.0.2') */
         payment_num
        ,reversal_payment_num
        ,payment_amount
        ,health_card_designation
        ,cancelled_date
        ,completion_date
        ,payment_type_num
        ,payment_type_key
        ,account_member_credit_card_num
        ,facility_order_number
        ,rx_number
        ,refill_num
        ,fd_facility_num
        ,fd_facility_key
        ,fill_state_chg_ts
        ,fill_state_code
        ,is_cob
    FROM (
        SELECT /*+ optimizer_features_enable('12.1.0.2') */ Distinct 
             pf.payment_num
            ,pf.reversal_payment_num
            ,pf.payment_amount
            ,pf.health_card_designation
            ,(CASE WHEN pf.cancelled_date_key = 0 THEN NULL ELSE To_Date(To_Char(pf.cancelled_date_key),'YYYYMMDD') END) AS cancelled_date
            ,(CASE WHEN pf.completion_date_key = 0 THEN NULL ELSE To_Date(To_Char(pf.completion_date_key),'YYYYMMDD') END) AS completion_date
            ,NULL AS payment_type_num
            ,pf.payment_type_key
            ,NULL AS account_member_credit_card_num
            ,pf.facility_order_number
            ,pf.rx_number
            ,pf.refill_num
            ,f.fd_facility_num
            ,f.fd_facility_key
            ,ff.fill_state_chg_ts
            ,ff.fill_state_code
            ,NULL AS is_cob
            ,(To_date(To_Char(pf.completion_date_key),'YYYYMMDD') + ((pf.completion_time_key - 2)/86400)) as pf_comp
            ,Rank() OVER (PARTITION BY pf.payment_num
                          ORDER BY ff.fill_state_chg_ts Desc, rownum Desc)
              AS Partial_Ranky
        FROM TREXONE_DW_DATA.PAYMENT_FACT pf
             INNER JOIN TREXONE_DW_DATA.Facility f
                ON f.fd_facility_key = pf.fd_facility_key
             INNER JOIN TREXONE_AUD_DATA.Fill_Fact ff
                ON ff.facility_num = f.fd_facility_num
               AND ff.rx_number = pf.rx_number
               AND ff.refill_num = pf.refill_num
               AND ff.partial_fill_seq IS NULL
               AND ff.rx_fill_seq = pf.rx_fill_seq
               AND ff.fill_state_code = '14' --IN ('3','4','14','16')
               --AND ff.fill_state_chg_ts <= (To_Date(:QueryDateStr,'YYYYMMDD') + INTERVAL '23:59:59' HOUR TO SECOND)
               AND ff.fill_state_chg_ts <= (:QueryDateDt + INTERVAL '23:59:59' HOUR TO SECOND)
        WHERE pf.completion_date_key = To_Number(To_Char(To_Date(:QueryDateStr,'YYYYMMDD'),'YYYYMMDD'))
          AND pf.is_active = 'Y'
    )
    WHERE Partial_Ranky = 1
      AND (fd_facility_key, rx_number, refill_num, fill_state_chg_ts) NOT IN (
              SELECT /*+ optimizer_features_enable('12.1.0.2') */ m.fd_facility_key, m.rx_number, m.refill_num, m.fill_state_chg_ts
              FROM maybe_full_refunds m
          )
      AND (payment_num, fill_state_chg_ts) NOT IN (
              SELECT /*+ optimizer_features_enable('12.1.0.2') */ m.payment_num, m.fill_state_chg_ts
              FROM maybe_full_refunds m
              WHERE m.payment_num IS NOT NULL
          )
), payments AS (
    SELECT /*+ optimizer_features_enable('12.1.0.2') */ * FROM maybe_full_refunds
    UNION ALL
    SELECT /*+ optimizer_features_enable('12.1.0.2') */ * FROM maybe_partial_refunds
)
SELECT /*+ optimizer_features_enable('12.1.0.2') */
     CancelledDate
    ,CompletionDate
    ,DateOfService
    ,FacilityTableFacilityNumber
    ,FillFactTableFacilityKey
    ,FillFactTableFillSequence
    ,FillFactTableKey
    ,FillFactTablePartitionDate
    ,FillFactTablePatientKey
    ,FillFactTablePrimaryPlanKey
    ,FillFactTableRecordNumber
    ,FillFactTableThirdPartyPlanKey
    ,HealthCardDesignation
    ,IsBilledAfterReturn
    ,ItemTableOrderNumber
    ,ItemTablePaymentGroupNumber
    ,ItemTableProductNumber
    ,LastFour
    ,OrderNumber
    ,PartialFillSequence
    ,PatientPricePaid
    ,PaymentAmount
    ,PaymentNumber
    ,PaymentTableAccountNumber
    ,PaymentTablePaymentTypeNumber
    ,PaymentTableReversalNumber
    ,PaymentTypeName
    ,PrescriptionNumber
    ,ProductTableDrugNumber
    ,RefillNumber
    ,ReversalPaymentAmount
    ,ShipHandleFee
    ,SoldDateKey
    ,SoldDateSeconds
    ,StoreNationalDrugProgramsId
    ,StoreNumber
    ,StoreNationalProviderId
    ,TargetCentralFillFacility
    ,TotalPricePaid
    ,TotalUsualAndCustomary
    ,TrackingNumber
    ,DeliveryMethod
    ,CourierName
    ,ShippingMethod
    ,CourierShipCharge
    ,PatientNum
    ,PatientAddressKey
    ,PatientCity
    ,PatientState
    ,PatientZip
    ,CardType
    ,FillStateCode
    ,IsReversalForCob
    ,LatestFillStatusDesc
    ,SoldFillStatusDate
    ,PatientPricePaidForOrder
FROM (
    SELECT /*+ optimizer_features_enable('12.1.0.2') */
         CANCELLED_DATE                    CancelledDate
        ,COMPLETION_DATE                   CompletionDate
        ,DATE_OF_SERVICE                   DateOfService
        ,FD_FACILITY_NUM                   FacilityTableFacilityNumber
        ,FD_FACILITY_KEY                   FillFactTableFacilityKey
        ,RX_FILL_SEQ                       FillFactTableFillSequence
        ,FILL_FACT_KEY                     FillFactTableKey
        ,PARTITION_DATE                    FillFactTablePartitionDate
        ,PD_PATIENT_KEY                    FillFactTablePatientKey
        ,PRIMARY_TPLD_PLAN_KEY             FillFactTablePrimaryPlanKey
        ,RX_RECORD_NUM                     FillFactTableRecordNumber
        ,TPD_KEY                           FillFactTableThirdPartyPlanKey
        ,HEALTH_CARD_DESIGNATION           HealthCardDesignation
        ,IS_BAR                            IsBilledAfterReturn
        ,ORDER_NUM                         ItemTableOrderNumber
        ,PAYMENT_GROUP_NUM                 ItemTablePaymentGroupNumber
        ,PRODUCT_NUM                       ItemTableProductNumber
        ,LAST4_PAN                         LastFour
        ,FACILITY_ORDER_NUMBER             OrderNumber
        ,PARTIAL_FILL_SEQ                  PartialFillSequence
        ,PATIENT_PRICE_PAID                PatientPricePaid
        ,PAYMENT_AMOUNT                    PaymentAmount
        ,PAYMENT_NUM                       PaymentNumber
        ,ACCOUNT_MEMBER_CREDIT_CARD_NUM    PaymentTableAccountNumber
        ,PAYMENT_TYPE_NUM                  PaymentTablePaymentTypeNumber
        ,REVERSAL_PAYMENT_NUM              PaymentTableReversalNumber
        ,PAYMENT_TYPE_NAME                 PaymentTypeName
        ,RX_NUMBER                         PrescriptionNumber
        ,PRD_DRUG_NUM                      ProductTableDrugNumber
        ,REFILL_NUM                        RefillNumber
        ,REVERSAL_PAYMENT_AMOUNT           ReversalPaymentAmount
        ,SHIP_HANDLE_FEE                   ShipHandleFee
        ,SOLD_DATE_KEY                     SoldDateKey
        ,SOLD_DATE_SECONDS                 SoldDateSeconds
        ,FD_NCPDP_PROVIDER_ID              StoreNationalDrugProgramsId
        ,FD_FACILITY_ID                    StoreNumber
        ,FD_VALUE                          StoreNationalProviderId
        ,TARGET_CF_FACILITY_NUM            TargetCentralFillFacility
        ,TOTAL_PRICE_PAID                  TotalPricePaid
        ,TOTAL_UNC                         TotalUsualAndCustomary
        ,TRACKING_NUMBER                   TrackingNumber
        ,dm_delivery_method_desc           DeliveryMethod
        ,courier_name                      CourierName
        ,shipping_method_desc              ShippingMethod
        ,courier_ship_charge               CourierShipCharge
        ,pd_patient_num                    PatientNum
        ,pad_key                           PatientAddressKey
        ,pad_city                          PatientCity
        ,pad_state                         PatientState
        ,pad_zip                           PatientZip
        ,card_type_desc                    CardType
        ,fill_state_code                   FillStateCode
        ,is_reversal_for_cob               IsReversalForCob
        ,LatestFillStatusDesc
        ,SoldFillStatusDate
        ,Sum((CASE WHEN Order_Rx_Rank = 1 THEN PATIENT_PRICE_PAID ELSE 0 END)) OVER 
            (PARTITION BY FACILITY_ORDER_NUMBER, PAYMENT_NUM)
            AS PatientPricePaidForOrder
        ,(CASE WHEN fill_state_code = '16' AND is_bar = 'Y'
               THEN Rank() OVER (PARTITION BY
                                      FACILITY_ORDER_NUMBER
                                     ,RX_NUMBER
                                     ,refill_num
                                     ,partial_fill_seq
                                     ,fill_state_code
                                     ,is_bar
                                     ,fill_state_chg_ts
                                 ORDER BY
                                      fill_state_chg_ts Desc
                                     ,completion_date Desc Nulls Last
                                     ,rownum Desc)
               WHEN LatestFillStatusDesc = 'Cancelled'
               THEN Rank() OVER (PARTITION BY
                                      PARTITION_DATE
                                     ,FACILITY_ORDER_NUMBER
                                     ,RX_NUMBER
                                     ,refill_num
                                     ,partial_fill_seq
                                 ORDER BY
                                      fill_state_chg_ts Desc
                                     ,completion_date Asc Nulls First
                                     ,rownum Desc)
               ELSE Rank() OVER (PARTITION BY
                                      FACILITY_ORDER_NUMBER
                                     ,RX_NUMBER
                                     ,refill_num
                                     ,partial_fill_seq
                                     ,fill_state_chg_ts
                                 ORDER BY
                                      fill_state_chg_ts Desc
                                     ,completion_date Desc Nulls Last
                                     ,rownum Desc)
         END) AS Avoid_Duplicates_Rank
    FROM (
          SELECT /*+ optimizer_features_enable('12.1.0.2') */
             DW__Facility.fd_facility_id --Store Number
            ,DW__Facility.fd_ncpdp_provider_id --Store NCPDP
            ,DW__Facility_ID.fd_value --Store NPI
            ,To_Number(To_Char(AUD__Fill_Fact.fill_state_chg_ts,'YYYYMMDD')) AS sold_date_key
            ,Round(((AUD__Fill_Fact.fill_state_chg_ts - Trunc(AUD__Fill_Fact.fill_state_chg_ts)) * 86400),0) AS sold_date_seconds
            ,AUD__Payment.cancelled_date
            ,AUD__Payment.completion_date
            ,AUD__Order_Record.facility_order_number
            ,AUD__Payment.payment_amount
            ,AUD__PaymentRev.payment_amount AS reversal_payment_amount --need to rename due to duplicate field name
            ,AUD__Fill_Fact.final_price AS total_price_paid
            ,AUD__Fill_Fact.patient_pay_amount AS patient_price_paid
            ,AUD__Fill_Fact.total_uandc AS total_unc
            ,AUD__Payment.payment_num
            ,AUD__Payment.health_card_designation
            ,DW__Payment_Type.payment_type_name
            ,AUD__Item.target_cf_facility_num
            ,AUD__Item.date_of_service
            ,AUD__Fill_Fact.rx_number
            ,AUD__Fill_Fact.refill_num
            ,AUD__Fill_Fact.partial_fill_seq
            ,DW__Shipment_Fact.courier_ship_charge
            ,DW__Patient_Address.pad_city
            ,DW__Patient_Address.pad_state
            ,DW__Patient_Address.pad_zip
            ,AUD__Credit_Card_Type.card_type_desc
            ,DW__Delivery_Method_Type.dm_delivery_method_desc
            ,DW__Courier_Shipment_Type.courier_name
            ,DW__Courier_Shipment_Type.shipping_method_desc
            ,DW__Shipment_Fact.tracking_number
            ,DW__Shipment_Fact.ship_handle_fee
            ,AUD__Account_Member_CC.last4_pan
            ,NVL(AUD__Fill_Fact.is_bar,'N') AS is_bar
            ,AUD__Fill_Fact.fill_state_chg_ts AS partition_date
            ,0 AS fill_fact_key
            ,DW__Facility.fd_facility_key
            ,AUD__Fill_Fact.rx_fill_seq
            ,AUD__Fill_Fact.rx_record_num
            ,DW__Facility.fd_facility_num
            ,AUD__Item.order_num
            ,AUD__Item.product_num
            ,AUD__Item.payment_group_num
            ,AUD__Payment.reversal_payment_num
            ,AUD__Payment.payment_type_num
            ,AUD__Payment.account_member_credit_card_num
            ,AUD__Fill_Fact.dispensed_prod_num AS prd_drug_num
            ,DW__Patient_Address.pd_patient_key
            ,DW__Tp_Patient.tpd_key
            ,DW__Tp_Plan.tpld_plan_key AS primary_tpld_plan_key
            ,NVL(AUD__Fill_Fact.patient_num,0) AS pd_patient_num
            ,NVL(DW__Shipment_Fact.pad_key,0) AS pad_key
            ,AUD__Fill_Fact.fill_state_chg_ts
            ,AUD__Fill_Fact.fill_state_code
            
            ,(CASE WHEN AUD__Fill_Fact.fill_state_code = '4' AND
                        AUD__Fill_Fact.split_billing_code = '0' AND
                        AUD__Fill_Fact.has_reversal = 'Y' AND            
                        (AUD__Fill_Fact.fill_state_chg_ts, AUD__Fill_Fact.facility_num, AUD__Fill_Fact.rx_number, AUD__Fill_Fact.refill_num, AUD__Fill_Fact.fill_state_code, 'Y') IN (
                            SELECT /*+ optimizer_features_enable('12.1.0.2') */ fill_state_chg_ts, fd_facility_num, rx_number, refill_num, fill_state_code, is_cob
                            FROM payments
                        ) AND            
                        (AUD__Fill_Fact.facility_num, AUD__Fill_Fact.rx_number, AUD__Fill_Fact.refill_num, '16') NOT IN (
                            SELECT /*+ optimizer_features_enable('12.1.0.2') */ fd_facility_num, rx_number, refill_num, fill_state_code
                            FROM payments
                        )
                        THEN 'Y'
                   ELSE 'N'
              END) AS is_reversal_for_cob
    
            ,(SELECT /*+ optimizer_features_enable('12.1.0.2') */ FS_FILL_STATUS_DESC
              FROM (SELECT /*+ optimizer_features_enable('12.1.0.2') */
                        fst.FS_FILL_STATUS_DESC,
                        Rank() OVER (ORDER BY fwf.FWFS_DT_OUT Desc, rownum Desc) AS Fill_Status_Rank
                    FROM TREXONE_DW_DATA.FILL_WORKFLOW_FACT fwf
                         INNER JOIN TREXONE_DW_DATA.FILL_STATUS_TYPE fst
                            ON fst.FS_FILL_STATUS_KEY = fwf.FS_FILL_STATUS_KEY
                    WHERE fwf.FWFS_RX_RECORD_NUM = AUD__Fill_Fact.RX_RECORD_NUM
                      AND fwf.FWFS_RX_FILL_SEQ = AUD__Fill_Fact.RX_FILL_SEQ
                      AND fwf.FWFS_DT_OUT < (To_Date(:QueryDateStr,'YYYYMMDD') + 1)
                      --AND fwf.FWFS_DT_OUT < (:QueryDateDt + 1)
                   )
              WHERE Fill_Status_Rank = 1
             ) AS LatestFillStatusDesc
    
            ,(SELECT /*+ optimizer_features_enable('12.1.0.2') */ FWFS_DT_OUT
              FROM (SELECT /*+ optimizer_features_enable('12.1.0.2') */
                        fwf.FWFS_DT_OUT,
                        Rank() OVER (ORDER BY fwf.FWFS_DT_OUT Asc, rownum Asc) AS Fill_Status_Rank
                    FROM TREXONE_DW_DATA.FILL_WORKFLOW_FACT fwf
                         INNER JOIN TREXONE_DW_DATA.FILL_STATUS_TYPE fst
                            ON fst.FS_FILL_STATUS_KEY = fwf.FS_FILL_STATUS_KEY
                           AND fst.FS_FILL_STATUS_DESC = 'Sold'
                    WHERE fwf.FWFS_RX_RECORD_NUM = AUD__Fill_Fact.RX_RECORD_NUM
                      AND fwf.FWFS_RX_FILL_SEQ = AUD__Fill_Fact.RX_FILL_SEQ
                      AND fwf.FWFS_DT_OUT < (To_Date(:QueryDateStr,'YYYYMMDD') + 1)
                      --AND fwf.FWFS_DT_OUT < (:QueryDateDt + 1)
                   )
              WHERE Fill_Status_Rank = 1
             ) AS SoldFillStatusDate
    
            ,Rank() OVER (PARTITION BY
                               AUD__Order_Record.facility_order_number
                              ,AUD__Fill_Fact.rx_number
                              ,AUD__Fill_Fact.refill_num
                          ORDER BY
                               AUD__Fill_Fact.fill_state_chg_ts Desc
                              ,rownum Desc
                          ) AS Order_Rx_Rank
        
        FROM TREXONE_AUD_DATA.Fill_Fact AUD__Fill_Fact
         INNER JOIN TREXONE_DW_DATA.Facility DW__Facility
            ON DW__Facility.fd_facility_num = AUD__Fill_Fact.facility_num
           AND DW__Facility.FD_IS_PPI_ENABLED = 'Y'
         INNER JOIN TREXONE_DW_DATA.Facility_ID DW__Facility_ID
            ON DW__Facility_ID.fd_facility_num = DW__Facility.fd_facility_num
           AND DW__Facility_ID.fd_type = 'F08'
         INNER JOIN TREXONE_DW_DATA.Drug DW__Drug
            ON DW__Drug.drug_num = AUD__Fill_Fact.dispensed_drug_num
         LEFT OUTER JOIN TREXONE_DW_DATA.Tp_Patient DW__Tp_Patient
            ON DW__Tp_Patient.tpd_tp_patient_num = AUD__Fill_Fact.tp_patient_num
           AND DW__Tp_Patient.tpd_eff_end_date = To_Date('29991231','YYYYMMDD')
         LEFT OUTER JOIN TREXONE_DW_DATA.Tp_Plan DW__Tp_Plan
            ON DW__Tp_Plan.tpld_plan_num = AUD__Fill_Fact.tp_plan_num
         LEFT OUTER JOIN TREXONE_AUD_DATA.Item AUD__Item
            ON AUD__Item.rx_record_num = AUD__Fill_Fact.rx_record_num
           AND AUD__Item.rx_fill_seq = AUD__Fill_Fact.rx_fill_seq
           AND AUD__Item.eff_end_date = To_Date('29991231','YYYYMMDD')
           AND AUD__Item.H_TYPE != 'D'
         LEFT OUTER JOIN TREXONE_AUD_DATA.Order_Record AUD__Order_Record
             ON AUD__Order_Record.order_num = AUD__Item.order_num
            AND AUD__Order_Record.eff_end_date = To_Date('29991231','YYYYMMDD')
         LEFT OUTER JOIN TREXONE_DW_DATA.Patient DW__Patient
             ON DW__Patient.pd_patient_num = AUD__Fill_Fact.patient_num
         LEFT OUTER JOIN TREXONE_DW_DATA.Shipment_Fact DW__Shipment_Fact
             ON DW__Shipment_Fact.order_num = AUD__Item.order_num
            AND DW__Shipment_Fact.pd_patient_key = DW__Patient.pd_patient_key
         LEFT OUTER JOIN TREXONE_DW_DATA.Patient_Address DW__Patient_Address
            ON DW__Patient_Address.pad_key = DW__Shipment_Fact.pad_key
         LEFT OUTER JOIN TREXONE_AUD_DATA.Payment_Group_List AUD__Payment_Group_List
            ON AUD__Payment_Group_List.payment_group_num = AUD__Item.payment_group_num
           AND AUD__Payment_Group_List.eff_end_date = To_Date('29991231','YYYYMMDD')  
         LEFT OUTER JOIN payments AUD__Payment
            on AUD__Payment.payment_num = AUD__Payment_Group_List.payment_num
         LEFT OUTER JOIN TREXONE_AUD_DATA.Payment AUD__PaymentRev
            ON AUD__PaymentRev.payment_num = AUD__Payment.reversal_payment_num
           AND AUD__PaymentRev.eff_end_date = TO_DATE('12/31/2999', 'MM/DD/YYYY')
         LEFT OUTER JOIN TREXONE_DW_DATA.Payment_Type DW__Payment_Type
            ON (DW__Payment_Type.payment_type_num = AUD__Payment.payment_type_num OR
                DW__Payment_Type.payment_type_key = AUD__Payment.payment_type_key)
         LEFT OUTER JOIN TREXONE_DW_DATA.Courier_Shipment_Type DW__Courier_Shipment_Type
            ON DW__Courier_Shipment_Type.courier_shipment_key = DW__Shipment_Fact.courier_shipment_key
         LEFT OUTER JOIN TREXONE_DW_DATA.Delivery_Method_Type DW__Delivery_Method_Type
            ON DW__Delivery_Method_Type.dm_delivery_method_key = DW__Shipment_Fact.dm_delivery_method_key
         LEFT OUTER JOIN TREXONE_AUD_DATA.Account_Member_Credit_Card AUD__Account_Member_CC
            ON AUD__Account_Member_CC.account_member_credit_card_num = AUD__Payment.account_member_credit_card_num
           AND AUD__Account_Member_CC.eff_end_date = To_Date('29991231','YYYYMMDD')
         LEFT OUTER JOIN TREXONE_AUD_DATA.Credit_Card_Type AUD__Credit_Card_Type
            ON AUD__Credit_Card_Type.credit_card_type_num = AUD__Account_Member_CC.credit_card_type_num
           AND AUD__Credit_Card_Type.eff_end_date = To_Date('29991231','YYYYMMDD')
        WHERE AUD__Payment.cancelled_date IS NULL
          AND AUD__Fill_Fact.partial_fill_seq IS NULL
          AND (AUD__Fill_Fact.fill_state_chg_ts, AUD__Fill_Fact.facility_num, AUD__Fill_Fact.rx_number, AUD__Fill_Fact.refill_num, AUD__Fill_Fact.fill_state_code) in (
                  SELECT /*+ optimizer_features_enable('12.1.0.2') */ fill_state_chg_ts, fd_facility_num, rx_number, refill_num, fill_state_code
                  FROM payments
              )
    )
)
WHERE Avoid_Duplicates_Rank = 1
  AND SoldFillStatusDate IS NOT NULL
  AND FillFactTablePartitionDate >= SoldFillStatusDate  -- refunds must be dated after sale