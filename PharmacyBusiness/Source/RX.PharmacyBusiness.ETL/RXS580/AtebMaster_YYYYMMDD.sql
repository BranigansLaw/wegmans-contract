       WITH facts AS (
            SELECT
                rx_number, 
                transaction_date,
                fill_fact_key,
                REFILL_NUM,
                QTY_DISPENSED,
                DAYS_SUPPLY,
                Substr(SIG, 1, 400) AS SIG_Text,
                WRITTEN_DATE_KEY,
                DISPENSED_DATE_KEY,
                SOLD_DATE_KEY,
                TOTAL_REFILLS_ALLOWED,
                cancel_date_key,
                patient_price_paid,
                ERP_ENROLLMENT_CODE_NUM,
                DC_DAW_CODE_KEY,
                FIRST_FILL_DATE_KEY,
                TOTAL_REFILLS_REMAINING,
                RX_EXPIRY_DATE_KEY,
                RX_ORIGIN_CODE,
                READY_DATE_KEY,
                fd_facility_key,
                PRS_PRESCRIBER_KEY,
                PADR_KEY,
                PD_PATIENT_KEY,
                FS_FILL_STATUS_KEY,
                DISPENSED_DRUG_KEY,
                primary_tpld_plan_key,
                tpd_key,
                IS_BAR,
                rx_record_num
            FROM TREXONE_DW_DATA.Fill_Fact
            WHERE IS_BAR = 'N'
              AND (DISPENSED_DATE_KEY BETWEEN To_Number(To_Char((:RunDate - 1),'YYYYMMDD')) AND To_Number(To_Char(:RunDate,'YYYYMMDD'))
                   OR
                   READY_DATE_KEY BETWEEN To_Number(To_Char((:RunDate - 1),'YYYYMMDD')) AND To_Number(To_Char(:RunDate,'YYYYMMDD'))
                   OR
                   SOLD_DATE_KEY BETWEEN To_Number(To_Char((:RunDate - 1),'YYYYMMDD')) AND To_Number(To_Char(:RunDate,'YYYYMMDD'))
                   OR
                   CANCEL_DATE_KEY BETWEEN To_Number(To_Char((:RunDate - 1),'YYYYMMDD')) AND To_Number(To_Char(:RunDate,'YYYYMMDD'))
                  )
        ), statuses AS (
            SELECT 
                 rx.rx_number
                ,rx.EFF_START_DATE
                ,rx.DAYS_SUPPLY
                ,rx.ERP_ENROLLMENT_CODE_NUM
                ,rx.DAW_Code
                ,rx.ORIGINAL_FILL_DATE
                ,rx.EXPIRY_DATE
                ,rx.facility_num
                ,rx.prescriber_num
                ,rx.prescriber_address_num
                ,rx.patient_num
                ,rx.rx_record_num                
                ,rx.PRODUCT_NUM
                ,rx.REFILLS_AUTHORIZED
                ,rx.PRESCRIBED_DATE
                ,rx.LAST_FILL_DATE
            FROM TREXONE_AUD_DATA.Rx rx
                INNER JOIN TREXONE_AUD_DATA.Rx_Status rs
                    ON rs.rx_status_num = rx.rx_status_num
                   AND rs.eff_end_date = To_Date('29991231','YYYYMMDD')
                   AND rs.description IN ('Inactive','Inactive DUR Rx','Inactive Profiled')
            WHERE rx.eff_start_date BETWEEN (:RunDate - 1) AND (:RunDate + 0)
              AND rx.eff_end_date = To_Date('29991231','YYYYMMDD')
              AND rx.deactivate_comment IS NOT NULL
        )
        SELECT
            Pat_Num,
            Person_Code,
            Pat_Lst_Nm,
            Pat_FrstNm,
            Pat_MidNm,
            Pat_Title,
            Pat_Suff,
            Pat_DOB,
            GENDER,
            Pat_Addr1,
            Pat_Addr2,
            TRIM(Pat_City) as Pat_City,
            Pat_State,
            Pat_Zip,
            Pat_Phone,
            Pat_Email,
            NCPDP,
            NPI,
            PrescriberID_Qualifier,
            PrescriberID,
            Prescriber_DEA_Nbr,
            Prescriber_State_License,
            Prescriber_Specialty,
            PRS_LAST_NAME,
            PRS_FIRST_NAME,
            PRS_MIDDLE_NAME,
            PRS_TITLE_ABBR,
            PRS_NAME_SUFFIX_ABBR,
            Prescr_Addr1,
            TRIM(Prescr_Addr2) as Prescr_Addr2,
            Prescr_City,
            Prescr_State,
            Prescr_Zip,
            Prescr_Phone,
            Prescr_Fax,
            rx_number,
            REFILL_NUM,
            ProductID_Qualifier,
            DRUG_NUM,
            DRUG_LABEL_NAME,
            QTY_DISPENSED,
            DAYS_SUPPLY,
            SIG_Text,
            Dosage_ID,
            Date_Written,
            Date_Filled,
            Date_Sold,
            Refills_Allowed,
            Rx_Status,
            Rx_Pat_Paid_Amt_Submitted,
            Co_Pay,
            BIN,
            PCN,
            Payer_GroupID,
            Payer_Member_ID,
            Payer_CardholderID,
            PatAltPhoneNbr,
            PD_LANGUAGE_NUM,
            Pharmacy_Number,
            AUTO_FILL,
            DAW_Code,
            OriginalFillDate,
            RefillsRem,
            RxEXPDate,
            DRUG_NDC,
            TimePickedUp,
            RxOrigin,
            Shipment_Date,
            CarrierID,
            ShipTrackingNbr,
            Est_Deliver_Date,
            Pat_Prefrd_Contact_Method,
            BrandGeneric_Ind,
            Ship_Method,
            Prod_Cost,
            Total_Price,
            Payment_Type,
            RxReadyDt,
            Contacted_Prescr_Date,
            Prescr_Denied_Date,
            Prescr_Approve_Date,
            LongTermCare_Fac_Cd,
            null as unknown
        FROM (
                SELECT 
                    Rank() OVER (PARTITION BY f.fd_facility_id, ff.rx_number, ff.transaction_date, ff.SOLD_DATE_KEY
                                 ORDER BY ff.fill_fact_key, rownum
                    ) AS DuplicateRank,
                    pat.PD_PATIENT_NUM AS Pat_Num,
                    NULL AS Person_Code,
                    pat.PD_LAST_NAME AS Pat_Lst_Nm,
                    pat.PD_FIRST_NAME AS Pat_FrstNm,
                    pat.PD_MIDDLE_NAME AS Pat_MidNm,
                    pat.PD_TITLE_ABBR AS Pat_Title,
                    pat.PD_NAME_SUFFIX_ABBR AS Pat_Suff,
                    To_Char(pat.PD_BIRTH_DATE,'YYYYMMDD') AS Pat_DOB,
                    (CASE WHEN dem.GENDER = 'M' THEN 1
                          WHEN dem.GENDER = 'F' THEN 2
                          ELSE 0
                     END) AS GENDER,
                    pat_addr.PAD_ADDRESS1 AS Pat_Addr1,
                    pat_addr.PAD_ADDRESS2 AS Pat_Addr2,
                    pat_addr.PAD_CITY AS Pat_City,
                    pat_addr.PAD_STATE AS Pat_State,
                    pat_addr.PAD_ZIP AS Pat_Zip,
                    (CASE WHEN preferred.patp_phone_number IS NULL 
                               THEN home1.patp_area_code || home1.patp_phone_number
                          ELSE preferred.patp_area_code || preferred.patp_phone_number
                     END) AS Pat_Phone,
                    pat.PATIENT_EMAIL AS Pat_Email,
                    f.FD_NCPDP_PROVIDER_ID AS NCPDP,
                    fid.FD_VALUE AS NPI,
                    NULL AS PrescriberID_Qualifier,
                    NULL AS PrescriberID,
                    NULL AS Prescriber_DEA_Nbr,
                    NULL AS Prescriber_State_License,
                    NULL AS Prescriber_Specialty,
                    presc.PRS_LAST_NAME,
                    presc.PRS_FIRST_NAME,
                    presc.PRS_MIDDLE_NAME,
                    presc.PRS_TITLE_ABBR,
                    presc.PRS_NAME_SUFFIX_ABBR,
                    presc_addr.PADR_ADDRESS1 AS Prescr_Addr1,
                    presc_addr.PADR_ADDRESS2 AS Prescr_Addr2,
                    presc_addr.PADR_CITY AS Prescr_City,
                    presc_addr.PADR_STATE_CODE AS Prescr_State,
                    presc_addr.PADR_ZIPCODE AS Prescr_Zip,
                    NULL AS Prescr_Phone,
                    NULL AS Prescr_Fax,
                    ff.rx_number,
                    ff.REFILL_NUM,
                    '03' AS ProductID_Qualifier,
                    drug.DRUG_NUM,
                    drug.DRUG_LABEL_NAME,
                    ff.QTY_DISPENSED,
                    ff.DAYS_SUPPLY,
                    ff.SIG_Text,
                    NULL AS Dosage_ID,
                    To_Char(ff.WRITTEN_DATE_KEY) AS Date_Written,
                    To_Char(ff.DISPENSED_DATE_KEY) AS Date_Filled,
                    (CASE WHEN ff.SOLD_DATE_KEY = 0 THEN NULL ELSE To_Char(ff.SOLD_DATE_KEY) END) AS Date_Sold,
                    ff.TOTAL_REFILLS_ALLOWED AS Refills_Allowed,
                    (CASE WHEN (s_ff.rx_record_num IS NOT NULL)
                               THEN '14' --Inactive
                          WHEN ( (ff.cancel_date_key > 0) OR
                                 (fst.fs_fill_status_num BETWEEN 3 AND 4) OR
                                 (fst.fs_fill_status_num BETWEEN 6 AND 10)
                               )
                               THEN '1' --Cancelled
                          WHEN ( (ff.sold_date_key > 0) OR
                                 (fst.fs_fill_status_num = 2)
                               )
                               THEN '3' --Sold/picked up
                          WHEN (fst.fs_fill_status_num = 1)
                               THEN '2' --Mck=InProccess/ATEB=filled
                          WHEN (fst.fs_fill_status_num = 5)
                               THEN '4' --Suspended/waiting for approval
                          ELSE '0' 
                    END) AS Rx_Status,
                    NULL AS Rx_Pat_Paid_Amt_Submitted,
                    Replace(To_Char(ff.patient_price_paid,'000000.00'),'.') AS Co_Pay,
                    tp.bin AS BIN,
                    tp.pcn AS PCN,
                    tppat.tpd_benefit_group_id AS Payer_GroupID,
                    tppat.tpd_cardholder_id AS Payer_Member_ID,
                    tppat.tpd_cardholder_id AS Payer_CardholderID,
                    NULL AS PatAltPhoneNbr,
                    pat.PD_LANGUAGE_NUM,
                    f.fd_facility_id AS Pharmacy_Number,
                    (CASE WHEN ff.ERP_ENROLLMENT_CODE_NUM = 3 THEN 'Y'
                          ELSE 'N'
                     END) AS AUTO_FILL,
                    (CASE WHEN ff.DC_DAW_CODE_KEY = 501 THEN '1'
                          WHEN ff.DC_DAW_CODE_KEY = 502 THEN '6'
                          WHEN ff.DC_DAW_CODE_KEY = 503 THEN '3'
                          WHEN ff.DC_DAW_CODE_KEY = 504 THEN '5'
                          WHEN ff.DC_DAW_CODE_KEY = 505 THEN '2'
                          WHEN ff.DC_DAW_CODE_KEY = 506 THEN '9'
                          WHEN ff.DC_DAW_CODE_KEY = 507 THEN '4'
                          WHEN ff.DC_DAW_CODE_KEY = 508 THEN '0'
                          WHEN ff.DC_DAW_CODE_KEY = 509 THEN '7'
                          WHEN ff.DC_DAW_CODE_KEY = 510 THEN '8'
                          ELSE '' 
                    END) AS DAW_Code,
                    To_Char(ff.FIRST_FILL_DATE_KEY) AS OriginalFillDate,
                    Round(ff.TOTAL_REFILLS_REMAINING) AS RefillsRem,
                    To_Char(ff.RX_EXPIRY_DATE_KEY) AS RxEXPDate,
                    drug.DRUG_NDC,
                    NULL AS TimePickedUp,
                    ff.RX_ORIGIN_CODE AS RxOrigin,
                    NULL AS Shipment_Date,
                    NULL AS CarrierID,
                    NULL AS ShipTrackingNbr,
                    NULL AS Est_Deliver_Date,
                    NULL AS Pat_Prefrd_Contact_Method,
                    NULL AS BrandGeneric_Ind,
                    NULL AS Ship_Method,
                    NULL AS Prod_Cost,
                    NULL AS Total_Price,
                    NULL AS Payment_Type,
                    To_Char(ff.READY_DATE_KEY) AS RxReadyDt,
                    NULL AS Contacted_Prescr_Date,
                    NULL AS Prescr_Denied_Date,
                    NULL AS Prescr_Approve_Date,
                    NULL AS LongTermCare_Fac_Cd,
                    To_Char(ff.transaction_date,'YYYYMMDD') AS transaction_date
                FROM facts ff
                     INNER JOIN TREXONE_DW_DATA.Facility f
                        ON f.fd_facility_key = ff.fd_facility_key
                       AND f.FD_IS_PPI_ENABLED = 'Y'
                     INNER JOIN TREXONE_DW_DATA.Facility_ID fid
                        ON fid.fd_facility_num = f.fd_facility_num
                       AND fid.fd_type = 'F08'
                       AND (
                            (fid.fd_facility_num = 1165600 AND fid.fd_value = '1376818336') --bad data in table...waiting ON SEV3 ticket SR 1-1410344725
                             OR 
                            (fid.fd_facility_num != 1165600)
                           )
                     INNER JOIN TREXONE_DW_DATA.Prescriber presc
                        ON presc.PRS_PRESCRIBER_KEY = ff.PRS_PRESCRIBER_KEY
                     INNER JOIN TREXONE_DW_DATA.Prescriber_Address presc_addr
                        ON presc_addr.PADR_KEY = ff.PADR_KEY
                     INNER JOIN TREXONE_DW_DATA.Patient pat
                        ON pat.PD_PATIENT_KEY = ff.PD_PATIENT_KEY
                     INNER JOIN TREXONE_DW_DATA.Patient_Address pat_addr
                         ON pat_addr.PD_PATIENT_KEY = ff.PD_PATIENT_KEY
                        AND pat_addr.PAD_END_DATE = To_Date('29991231','YYYYMMDD')
                        AND pat_addr.PAD_ADDRESS_USAGE = '1' -- Home 1 Address
                        AND pat_addr.PAD_ADDRESS1 IS NOT NULL
                     INNER JOIN TREXONE_DW_DATA.Fill_Status_Type fst
                         ON fst.FS_FILL_STATUS_KEY = ff.FS_FILL_STATUS_KEY
                     INNER JOIN TREXONE_DW_DATA.Patient_Demographic dem
                         ON dem.PATIENT_DEMOGRAPHIC_KEY = pat.PATIENT_DEMOGRAPHIC_KEY
                     INNER JOIN TREXONE_DW_DATA.Drug drug
                         ON drug.DRUG_KEY = ff.DISPENSED_DRUG_KEY
                     INNER JOIN TREXONE_AUD_DATA.Patient_Address pat_addr_aud
                         ON pat_addr_aud.PATIENT_NUM = pat.PD_PATIENT_NUM
                     LEFT OUTER JOIN TREXONE_DW_DATA.Tp_Plan tp
                         ON tp.tpld_plan_key = ff.primary_tpld_plan_key
                     LEFT OUTER JOIN TREXONE_DW_DATA.Tp_Patient tppat
                         ON tppat.tpd_key = ff.tpd_key
                     LEFT OUTER JOIN TREXONE_DW_DATA.Patient_Phone home1
                         ON home1.pd_patient_key = ff.pd_patient_key
                        AND home1.eff_end_date = To_Date('29991231','YYYYMMDD')
                        AND home1.patp_phone_usage = 1
                        AND home1.patp_usage = 1
                     LEFT OUTER JOIN TREXONE_DW_DATA.Patient_Phone preferred
                         ON preferred.pd_patient_key = ff.pd_patient_key
                        AND preferred.eff_end_date = To_Date('29991231','YYYYMMDD')
                        AND preferred.patp_phone_usage =
                            (CASE pat.pd_contact_by_code
                                  WHEN '1' then 1    -- Home 1
                                  WHEN '3' then 1    -- Home 2
                                  WHEN '5' then 1    -- Work
                                  WHEN '7' then 1    -- Other
                                  WHEN '9' then 2    -- Fax (Home 1)
                                  WHEN '8' then 16   -- Cell
                                  WHEN '10' then 32  -- Alt 1
                                  WHEN '11' then 8   -- Alt 2
                                  WHEN '18' then 64  -- Alt 3
                             END) 
                        AND preferred.patp_usage =
                            (CASE pat.pd_contact_by_code
                                  WHEN '1' then 1    -- Home 1
                                  WHEN '3' then 2    -- Home 2
                                  WHEN '5' then 4    -- Work
                                  WHEN '7' then 8    -- Other
                                  WHEN '9' then 1    -- Fax (Home 1)
                                  WHEN '8' then 0    -- Cell
                                  WHEN '10' then 0   -- Alt 1
                                  WHEN '11' then 0   -- Alt 2
                                  WHEN '18' then 0   -- Alt 3
                             END)
                     LEFT OUTER JOIN statuses s_ff
                         ON s_ff.rx_record_num = ff.rx_record_num

                UNION        

                SELECT 
                    Rank() OVER (PARTITION BY f.fd_facility_id, s.rx_number
                                 ORDER BY s.EFF_START_DATE Desc, rownum
                    ) AS DuplicateRank,
                    pat.PATIENT_NUM AS Pat_Num,
                    NULL AS Person_Code,
                    pat.LAST_NAME AS Pat_Lst_Nm,
                    pat.FIRST_NAME AS Pat_FrstNm,
                    pat.MIDDLE_NAME AS Pat_MidNm,
                    pat.TITLE_ABBR AS Pat_Title,
                    NULL AS Pat_Suff, ---dont have
                    To_Char(pat.BIRTH_DATE,'YYYYMMDD') AS Pat_DOB,
                    (CASE WHEN pat.GENDER = 'M' THEN 1
                          WHEN pat.GENDER = 'F' THEN 2
                          ELSE 0
                     END) AS GENDER,
                    pat_addr.ADDRESS1 AS Pat_Addr1,
                    pat_addr.ADDRESS2 AS Pat_Addr2,
                    pat_addr.CITY AS Pat_City,
                    pat_addr.STATE AS Pat_State,
                    pat_addr.ZIP AS Pat_Zip,
                    pat_addr.PHONE_NUM AS Pat_Phone,
                    NULL AS Pat_Email, ---dont have
                    f.FD_NCPDP_PROVIDER_ID AS NCPDP,
                    fid.FD_VALUE AS NPI,
                    NULL AS PrescriberID_Qualifier,
                    NULL AS PrescriberID,
                    NULL AS Prescriber_DEA_Nbr,
                    NULL AS Prescriber_State_License,
                    NULL AS Prescriber_Specialty,
                    presc.PRS_LAST_NAME,
                    presc.PRS_FIRST_NAME,
                    presc.PRS_MIDDLE_NAME,
                    presc.PRS_TITLE_ABBR,
                    presc.PRS_NAME_SUFFIX_ABBR,
                    presc_addr.PADR_ADDRESS1 AS Prescr_Addr1,
                    presc_addr.PADR_ADDRESS2 AS Prescr_Addr2,
                    presc_addr.PADR_CITY AS Prescr_City,
                    presc_addr.PADR_STATE_CODE AS Prescr_State,
                    presc_addr.PADR_ZIPCODE AS Prescr_Zip,
                    NULL AS Prescr_Phone,
                    NULL AS Prescr_Fax,
                    s.rx_number,
                    NULL AS REFILL_NUM, ---dont have
                    '03' AS ProductID_Qualifier,
                    drug.DRUG_NUM AS DRUG_NUM,
                    drug.DRUG_LABEL_NAME AS DRUG_LABEL_NAME,
                    NULL AS QTY_DISPENSED, ---dont have
                    NVL(s.DAYS_SUPPLY,0) AS DAYS_SUPPLY,
                    NULL AS SIG_Text, ---dont have
                    NULL AS Dosage_ID,
                    NULL AS Date_Written, ---dont have
                    To_Char(NVL(s.LAST_FILL_DATE,s.PRESCRIBED_DATE),'YYYYMMDD') AS Date_Filled,
                    NULL AS Date_Sold, ---dont have
                    NVL(s.REFILLS_AUTHORIZED,0) AS Refills_Allowed,
                    '14' AS Rx_Status,
                    NULL AS Rx_Pat_Paid_Amt_Submitted, ---dont have
                    NULL AS Co_Pay, ---dont have
                    NULL AS BIN, ---dont have
                    NULL AS PCN, ---dont have
                    NULL AS Payer_GroupID, ---dont have
                    NULL AS Payer_Member_ID, ---dont have
                    NULL AS Payer_CardholderID, ---dont have
                    NULL AS PatAltPhoneNbr,
                    pat.LANGUAGE_NUM AS PD_LANGUAGE_NUM,
                    f.fd_facility_id AS Pharmacy_Number,
                    (CASE WHEN s.ERP_ENROLLMENT_CODE_NUM = 3 THEN 'Y'
                          ELSE 'N'
                     END) AS AUTO_FILL,
                    To_Char(s.DAW_Code) AS DAW_Code,
                    To_Char(s.ORIGINAL_FILL_DATE,'YYYYMMDD') AS OriginalFillDate,
                    0 AS RefillsRem, ---dont have
                    To_Char(s.EXPIRY_DATE,'YYYYMMDD') AS RxEXPDate,
                    drug.DRUG_NDC AS DRUG_NDC,
                    NULL AS TimePickedUp, ---dont have
                    NULL AS RxOrigin, ---dont have
                    NULL AS Shipment_Date,
                    NULL AS CarrierID,
                    NULL AS ShipTrackingNbr,
                    NULL AS Est_Deliver_Date,
                    NULL AS Pat_Prefrd_Contact_Method,
                    NULL AS BrandGeneric_Ind,
                    NULL AS Ship_Method,
                    NULL AS Prod_Cost,
                    NULL AS Total_Price,
                    NULL AS Payment_Type,
                    NULL AS RxReadyDt, --dont have
                    NULL AS Contacted_Prescr_Date,
                    NULL AS Prescr_Denied_Date,
                    NULL AS Prescr_Approve_Date,
                    NULL AS LongTermCare_Fac_Cd,
                    NULL AS transaction_date --dont have
                FROM statuses s
                     INNER JOIN TREXONE_DW_DATA.Facility f
                         ON f.fd_facility_num = s.facility_num
                        AND f.FD_IS_PPI_ENABLED = 'Y'
                     INNER JOIN TREXONE_DW_DATA.Facility_ID fid
                         ON fid.fd_facility_num = f.fd_facility_num
                        AND fid.fd_type = 'F08'
                        AND (
                             (fid.fd_facility_num = 1165600 AND fid.fd_value = '1376818336') --bad data in table...waiting ON SEV3 ticket SR 1-1410344725
                              OR 
                             (fid.fd_facility_num != 1165600)
                            )
                     INNER JOIN TREXONE_DW_DATA.Prescriber presc
                         ON presc.prs_prescriber_num = s.prescriber_num
                     INNER JOIN TREXONE_DW_DATA.Prescriber_Address presc_addr
                         ON presc_addr.padr_prescriber_address_num = s.prescriber_address_num
                     INNER JOIN TREXONE_AUD_DATA.Patient pat
                         ON pat.patient_num = s.patient_num
                        AND pat.eff_end_date = To_Date('29991231','YYYYMMDD')
                     INNER JOIN TREXONE_AUD_DATA.Patient_Address pat_addr
                         ON pat_addr.patient_num = s.patient_num
                     INNER JOIN TREXONE_DW_DATA.Product product
                         ON product.PRD_PRODUCT_NUM = s.PRODUCT_NUM
                     INNER JOIN TREXONE_DW_DATA.Drug drug
                         ON drug.DRUG_NUM = product.PRD_DRUG_NUM
                WHERE NOT EXISTS (
                          SELECT * 
                          FROM facts ff2
                          WHERE ff2.rx_record_num = s.rx_record_num
                      )
        ) 
        WHERE DuplicateRank = 1

        ORDER BY Pharmacy_Number, rx_number, transaction_date, Date_Sold Nulls First