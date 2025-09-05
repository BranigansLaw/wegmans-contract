WITH FACTS AS (
            SELECT
                RX_NUMBER, 
                TRANSACTION_DATE,
                FILL_FACT_KEY,
                REFILL_NUM,
                QTY_DISPENSED,
                DAYS_SUPPLY,
                SUBSTR(SIG, 1, 400) AS SIG_TEXT,
                WRITTEN_DATE_KEY,
                DISPENSED_DATE_KEY,
                SOLD_DATE_KEY,
                TOTAL_REFILLS_ALLOWED,
                CANCEL_DATE_KEY,
                PATIENT_PRICE_PAID,
                ERP_ENROLLMENT_CODE_NUM,
                DC_DAW_CODE_KEY,
                FIRST_FILL_DATE_KEY,
                TOTAL_REFILLS_REMAINING,
                RX_EXPIRY_DATE_KEY,
                RX_ORIGIN_CODE,
                READY_DATE_KEY,
                FD_FACILITY_KEY,
                PRS_PRESCRIBER_KEY,
                PADR_KEY,
                PD_PATIENT_KEY,
                FS_FILL_STATUS_KEY,
                DISPENSED_DRUG_KEY,
                PRIMARY_TPLD_PLAN_KEY,
                TPD_KEY,
                IS_BAR,
                RX_RECORD_NUM
            FROM {DW}.ERXDW_PLS_ARCHIVE_VIEW.FILL_FACT
            WHERE IS_BAR = 'N'
              AND (DISPENSED_DATE_KEY BETWEEN TO_NUMBER(TO_CHAR((:RUNDATE - 1)::DATE,'YYYYMMDD')) AND TO_NUMBER(TO_CHAR(:RUNDATE::DATE,'YYYYMMDD'))
                   OR
                   READY_DATE_KEY BETWEEN TO_NUMBER(TO_CHAR((:RUNDATE - 1)::DATE,'YYYYMMDD')) AND TO_NUMBER(TO_CHAR(:RUNDATE::DATE,'YYYYMMDD'))
                   OR
                   SOLD_DATE_KEY BETWEEN TO_NUMBER(TO_CHAR((:RUNDATE - 1)::DATE,'YYYYMMDD')) AND TO_NUMBER(TO_CHAR(:RUNDATE::DATE,'YYYYMMDD'))
                   OR
                   CANCEL_DATE_KEY BETWEEN TO_NUMBER(TO_CHAR((:RUNDATE - 1)::DATE,'YYYYMMDD')) AND TO_NUMBER(TO_CHAR(:RUNDATE::DATE,'YYYYMMDD'))
                  )
        ), STATUSES AS (
            SELECT 
                 RX.RX_NUMBER
                ,RX.EFF_START_DATE
                ,RX.DAYS_SUPPLY
                ,RX.ERP_ENROLLMENT_CODE_NUM
                ,RX.DAW_CODE
                ,RX.ORIGINAL_FILL_DATE
                ,RX.EXPIRY_DATE
                ,RX.FACILITY_NUM
                ,RX.PRESCRIBER_NUM
                ,RX.PRESCRIBER_ADDRESS_NUM
                ,RX.PATIENT_NUM
                ,RX.RX_RECORD_NUM                
                ,RX.PRODUCT_NUM
                ,RX.REFILLS_AUTHORIZED
                ,RX.PRESCRIBED_DATE
                ,RX.LAST_FILL_DATE
            FROM {AUD}.ERXAUD_PLS_ARCHIVE_VIEW.RX RX
                INNER JOIN {AUD}.ERXAUD_PLS_ARCHIVE_VIEW.RX_STATUS RS
                    ON RS.RX_STATUS_NUM = RX.RX_STATUS_NUM
                   AND RS.EFF_END_DATE = TO_DATE('29991231','YYYYMMDD')
                   AND RS.DESCRIPTION IN ('INACTIVE','INACTIVE DUR RX','INACTIVE PROFILED')
            WHERE RX.EFF_START_DATE BETWEEN (:RUNDATE - 1) AND (:RUNDATE + 0)
              AND RX.EFF_END_DATE = TO_DATE('29991231','YYYYMMDD')
              AND RX.DEACTIVATE_COMMENT IS NOT NULL
        )
        SELECT
            PAT_NUM,
            PERSON_CODE,
            PAT_LST_NM,
            PAT_FRSTNM,
            PAT_MIDNM,
            PAT_TITLE,
            PAT_SUFF,
            PAT_DOB,
            GENDER,
            PAT_ADDR1,
            PAT_ADDR2,
            TRIM(PAT_CITY) AS PAT_CITY,
            PAT_STATE,
            PAT_ZIP,
            PAT_PHONE,
            PAT_EMAIL,
            NCPDP,
            NPI,
            PRESCRIBERID_QUALIFIER,
            PRESCRIBERID,
            PRESCRIBER_DEA_NBR,
            PRESCRIBER_STATE_LICENSE,
            PRESCRIBER_SPECIALTY,
            PRS_LAST_NAME,
            PRS_FIRST_NAME,
            PRS_MIDDLE_NAME,
            PRS_TITLE_ABBR,
            PRS_NAME_SUFFIX_ABBR,
            PRESCR_ADDR1,
            TRIM(PRESCR_ADDR2) AS PRESCR_ADDR2,
            PRESCR_CITY,
            PRESCR_STATE,
            PRESCR_ZIP,
            PRESCR_PHONE,
            PRESCR_FAX,
            RX_NUMBER,
            REFILL_NUM,
            PRODUCTID_QUALIFIER,
            DRUG_NUM,
            DRUG_LABEL_NAME,
            QTY_DISPENSED,
            DAYS_SUPPLY,
            SIG_TEXT,
            DOSAGE_ID,
            DATE_WRITTEN,
            DATE_FILLED,
            DATE_SOLD,
            REFILLS_ALLOWED,
            RX_STATUS,
            RX_PAT_PAID_AMT_SUBMITTED,
            CO_PAY,
            BIN,
            PCN,
            PAYER_GROUPID,
            PAYER_MEMBER_ID,
            PAYER_CARDHOLDERID,
            PATALTPHONENBR,
            PD_LANGUAGE_NUM,
            PHARMACY_NUMBER,
            AUTO_FILL,
            DAW_CODE,
            ORIGINALFILLDATE,
            REFILLSREM,
            RXEXPDATE,
            DRUG_NDC,
            TIMEPICKEDUP,
            RXORIGIN,
            SHIPMENT_DATE,
            CARRIERID,
            SHIPTRACKINGNBR,
            EST_DELIVER_DATE,
            PAT_PREFRD_CONTACT_METHOD,
            BRANDGENERIC_IND,
            SHIP_METHOD,
            PROD_COST,
            TOTAL_PRICE,
            PAYMENT_TYPE,
            RXREADYDT,
            CONTACTED_PRESCR_DATE,
            PRESCR_DENIED_DATE,
            PRESCR_APPROVE_DATE,
            LONGTERMCARE_FAC_CD,
            NULL AS UNKNOWN
        FROM (
                SELECT 
                    RANK() OVER (PARTITION BY F.FD_FACILITY_ID, FF.RX_NUMBER, FF.TRANSACTION_DATE, FF.SOLD_DATE_KEY
                                 ORDER BY FF.FILL_FACT_KEY, ROW_NUMBER() OVER (ORDER BY FF.FILL_FACT_KEY)
                    ) AS DUPLICATERANK,
                    PAT.PD_PATIENT_NUM AS PAT_NUM,
                    NULL AS PERSON_CODE,
                    PAT.PD_LAST_NAME AS PAT_LST_NM,
                    PAT.PD_FIRST_NAME AS PAT_FRSTNM,
                    PAT.PD_MIDDLE_NAME AS PAT_MIDNM,
                    PAT.PD_TITLE_ABBR AS PAT_TITLE,
                    PAT.PD_NAME_SUFFIX_ABBR AS PAT_SUFF,
                    TO_CHAR(PAT.PD_BIRTH_DATE::DATE,'YYYYMMDD') AS PAT_DOB,
                    (CASE WHEN DEM.GENDER = 'M' THEN 1
                          WHEN DEM.GENDER = 'F' THEN 2
                          ELSE 0
                     END) AS GENDER,
                    PAT_ADDR.PAD_ADDRESS1 AS PAT_ADDR1,
                    PAT_ADDR.PAD_ADDRESS2 AS PAT_ADDR2,
                    PAT_ADDR.PAD_CITY AS PAT_CITY,
                    PAT_ADDR.PAD_STATE AS PAT_STATE,
                    PAT_ADDR.PAD_ZIP AS PAT_ZIP,
                    (CASE WHEN PREFERRED.PATP_PHONE_NUMBER IS NULL 
                               THEN HOME1.PATP_AREA_CODE || HOME1.PATP_PHONE_NUMBER
                          ELSE PREFERRED.PATP_AREA_CODE || PREFERRED.PATP_PHONE_NUMBER
                     END) AS PAT_PHONE,
                    PAT.PATIENT_EMAIL AS PAT_EMAIL,
                    F.FD_NCPDP_PROVIDER_ID AS NCPDP,
                    FID.FD_VALUE AS NPI,
                    NULL AS PRESCRIBERID_QUALIFIER,
                    NULL AS PRESCRIBERID,
                    NULL AS PRESCRIBER_DEA_NBR,
                    NULL AS PRESCRIBER_STATE_LICENSE,
                    NULL AS PRESCRIBER_SPECIALTY,
                    PRESC.PRS_LAST_NAME,
                    PRESC.PRS_FIRST_NAME,
                    PRESC.PRS_MIDDLE_NAME,
                    PRESC.PRS_TITLE_ABBR,
                    PRESC.PRS_NAME_SUFFIX_ABBR,
                    PRESC_ADDR.PADR_ADDRESS1 AS PRESCR_ADDR1,
                    PRESC_ADDR.PADR_ADDRESS2 AS PRESCR_ADDR2,
                    PRESC_ADDR.PADR_CITY AS PRESCR_CITY,
                    PRESC_ADDR.PADR_STATE_CODE AS PRESCR_STATE,
                    PRESC_ADDR.PADR_ZIPCODE AS PRESCR_ZIP,
                    NULL AS PRESCR_PHONE,
                    NULL AS PRESCR_FAX,
                    FF.RX_NUMBER,
                    FF.REFILL_NUM,
                    '03' AS PRODUCTID_QUALIFIER,
                    DRUG.DRUG_NUM,
                    DRUG.DRUG_LABEL_NAME,
                    FF.QTY_DISPENSED,
                    FF.DAYS_SUPPLY,
                    FF.SIG_TEXT,
                    NULL AS DOSAGE_ID,
                    TO_CHAR(FF.WRITTEN_DATE_KEY) AS DATE_WRITTEN,
                    TO_CHAR(FF.DISPENSED_DATE_KEY) AS DATE_FILLED,
                    (CASE WHEN FF.SOLD_DATE_KEY = 0 THEN NULL ELSE TO_CHAR(FF.SOLD_DATE_KEY) END) AS DATE_SOLD,
                    FF.TOTAL_REFILLS_ALLOWED AS REFILLS_ALLOWED,
                    (CASE WHEN (S_FF.RX_RECORD_NUM IS NOT NULL)
                               THEN '14' --INACTIVE
                          WHEN ( (FF.CANCEL_DATE_KEY > 0) OR
                                 (FST.FS_FILL_STATUS_NUM BETWEEN 3 AND 4) OR
                                 (FST.FS_FILL_STATUS_NUM BETWEEN 6 AND 10)
                               )
                               THEN '1' --CANCELLED
                          WHEN ( (FF.SOLD_DATE_KEY > 0) OR
                                 (FST.FS_FILL_STATUS_NUM = 2)
                               )
                               THEN '3' --SOLD/PICKED UP
                          WHEN (FST.FS_FILL_STATUS_NUM = 1)
                               THEN '2' --MCK=INPROCCESS/ATEB=FILLED
                          WHEN (FST.FS_FILL_STATUS_NUM = 5)
                               THEN '4' --SUSPENDED/WAITING FOR APPROVAL
                          ELSE '0' 
                    END) AS RX_STATUS,
                    NULL AS RX_PAT_PAID_AMT_SUBMITTED,
                    REPLACE(TO_CHAR(FF.PATIENT_PRICE_PAID,'000000.00'),'.') AS CO_PAY,
                    TP.BIN AS BIN,
                    TP.PCN AS PCN,
                    TPPAT.TPD_BENEFIT_GROUP_ID AS PAYER_GROUPID,
                    TPPAT.TPD_CARDHOLDER_ID AS PAYER_MEMBER_ID,
                    TPPAT.TPD_CARDHOLDER_ID AS PAYER_CARDHOLDERID,
                    NULL AS PATALTPHONENBR,
                    PAT.PD_LANGUAGE_NUM,
                    F.FD_FACILITY_ID AS PHARMACY_NUMBER,
                    (CASE WHEN FF.ERP_ENROLLMENT_CODE_NUM = 3 THEN 'Y'
                          ELSE 'N'
                     END) AS AUTO_FILL,
                    (CASE WHEN FF.DC_DAW_CODE_KEY = 501 THEN '1'
                          WHEN FF.DC_DAW_CODE_KEY = 502 THEN '6'
                          WHEN FF.DC_DAW_CODE_KEY = 503 THEN '3'
                          WHEN FF.DC_DAW_CODE_KEY = 504 THEN '5'
                          WHEN FF.DC_DAW_CODE_KEY = 505 THEN '2'
                          WHEN FF.DC_DAW_CODE_KEY = 506 THEN '9'
                          WHEN FF.DC_DAW_CODE_KEY = 507 THEN '4'
                          WHEN FF.DC_DAW_CODE_KEY = 508 THEN '0'
                          WHEN FF.DC_DAW_CODE_KEY = 509 THEN '7'
                          WHEN FF.DC_DAW_CODE_KEY = 510 THEN '8'
                          ELSE '' 
                    END) AS DAW_CODE,
                    TO_CHAR(FF.FIRST_FILL_DATE_KEY) AS ORIGINALFILLDATE,
                    ROUND(FF.TOTAL_REFILLS_REMAINING) AS REFILLSREM,
                    TO_CHAR(FF.RX_EXPIRY_DATE_KEY) AS RXEXPDATE,
                    DRUG.DRUG_NDC,
                    NULL AS TIMEPICKEDUP,
                    FF.RX_ORIGIN_CODE AS RXORIGIN,
                    NULL AS SHIPMENT_DATE,
                    NULL AS CARRIERID,
                    NULL AS SHIPTRACKINGNBR,
                    NULL AS EST_DELIVER_DATE,
                    NULL AS PAT_PREFRD_CONTACT_METHOD,
                    NULL AS BRANDGENERIC_IND,
                    NULL AS SHIP_METHOD,
                    NULL AS PROD_COST,
                    NULL AS TOTAL_PRICE,
                    NULL AS PAYMENT_TYPE,
                    TO_CHAR(FF.READY_DATE_KEY) AS RXREADYDT,
                    NULL AS CONTACTED_PRESCR_DATE,
                    NULL AS PRESCR_DENIED_DATE,
                    NULL AS PRESCR_APPROVE_DATE,
                    NULL AS LONGTERMCARE_FAC_CD,
                    TO_CHAR(FF.TRANSACTION_DATE::DATE,'YYYYMMDD') AS TRANSACTION_DATE
                FROM FACTS FF
                     INNER JOIN {DW}.ERXDW_PLS_ARCHIVE_VIEW.FACILITY F
                        ON F.FD_FACILITY_KEY = FF.FD_FACILITY_KEY
                       AND F.FD_IS_PPI_ENABLED = 'Y'
                     INNER JOIN {DW}.ERXDW_PLS_ARCHIVE_VIEW.FACILITY_ID FID
                        ON FID.FD_FACILITY_KEY = F.FD_FACILITY_NUM
                       AND FID.FD_TYPE = 'F08'
                       AND (
                            (FID.FD_FACILITY_KEY = 1165600 AND FID.FD_VALUE = '1376818336') --BAD DATA IN TABLE...WAITING ON SEV3 TICKET SR 1-1410344725
                             OR 
                            (FID.FD_FACILITY_KEY != 1165600)
                           )
                     INNER JOIN {DW}.ERXDW_PLS_ARCHIVE_VIEW.PRESCRIBER PRESC
                        ON PRESC.PRS_PRESCRIBER_KEY = FF.PRS_PRESCRIBER_KEY
                     INNER JOIN {DW}.ERXDW_PLS_ARCHIVE_VIEW.PRESCRIBER_ADDRESS PRESC_ADDR
                        ON PRESC_ADDR.PADR_KEY = FF.PADR_KEY
                     INNER JOIN {DW}.ERXDW_PLS_ARCHIVE_VIEW.PATIENT PAT
                        ON PAT.PD_PATIENT_KEY = FF.PD_PATIENT_KEY
                     INNER JOIN {DW}.ERXDW_PLS_ARCHIVE_VIEW.PATIENT_ADDRESS PAT_ADDR
                         ON PAT_ADDR.PD_PATIENT_KEY = FF.PD_PATIENT_KEY
                        AND PAT_ADDR.PAD_EFF_END_DATE = TO_DATE('29991231','YYYYMMDD')
                        AND PAT_ADDR.PAD_ADDRESS_USAGE = '1' -- HOME 1 ADDRESS
                        AND PAT_ADDR.PAD_ADDRESS1 IS NOT NULL
                     INNER JOIN {DW}.ERXDW_PLS_ARCHIVE_VIEW.FILL_STATUS_TYPE FST
                         ON FST.FS_FILL_STATUS_KEY = FF.FS_FILL_STATUS_KEY
                     INNER JOIN {DW}.ERXDW_PLS_ARCHIVE_VIEW.PATIENT_DEMOGRAPHIC DEM
                         ON DEM.PATIENT_DEMOGRAPHIC_KEY = PAT.PATIENT_DEMOGRAPHIC_KEY
                     INNER JOIN {DW}.ERXDW_PLS_ARCHIVE_VIEW.DRUG DRUG
                         ON DRUG.DRUG_KEY = FF.DISPENSED_DRUG_KEY
                     INNER JOIN {AUD}.ERXAUD_PLS_ARCHIVE_VIEW.PATIENT_ADDRESS PAT_ADDR_AUD
                         ON PAT_ADDR_AUD.PATIENT_NUM = PAT.PD_PATIENT_NUM
                     LEFT OUTER JOIN {DW}.ERXDW_PLS_ARCHIVE_VIEW.TP_PLAN TP
                         ON TP.TPLD_PLAN_KEY = FF.PRIMARY_TPLD_PLAN_KEY
                     LEFT OUTER JOIN {DW}.ERXDW_PLS_ARCHIVE_VIEW.TP_PATIENT TPPAT
                         ON TPPAT.TPD_KEY = FF.TPD_KEY
                     LEFT OUTER JOIN {DW}.ERXDW_PLS_ARCHIVE_VIEW.PATIENT_PHONE HOME1
                         ON HOME1.PD_PATIENT_KEY = FF.PD_PATIENT_KEY
                        AND HOME1.EFF_END_DATE = TO_DATE('29991231','YYYYMMDD')
                        AND HOME1.PATP_PHONE_USAGE = 1
                        AND HOME1.PATP_USAGE = 1
                     LEFT OUTER JOIN {DW}.ERXDW_PLS_ARCHIVE_VIEW.PATIENT_PHONE PREFERRED
                         ON PREFERRED.PD_PATIENT_KEY = FF.PD_PATIENT_KEY
                        AND PREFERRED.EFF_END_DATE = TO_DATE('29991231','YYYYMMDD')
                        AND PREFERRED.PATP_PHONE_USAGE =
                            (CASE PAT.PD_CONTACT_BY_CODE
                                  WHEN '1' THEN 1    -- HOME 1
                                  WHEN '3' THEN 1    -- HOME 2
                                  WHEN '5' THEN 1    -- WORK
                                  WHEN '7' THEN 1    -- OTHER
                                  WHEN '9' THEN 2    -- FAX (HOME 1)
                                  WHEN '8' THEN 16   -- CELL
                                  WHEN '10' THEN 32  -- ALT 1
                                  WHEN '11' THEN 8   -- ALT 2
                                  WHEN '18' THEN 64  -- ALT 3
                             END) 
                        AND PREFERRED.PATP_USAGE =
                            (CASE PAT.PD_CONTACT_BY_CODE
                                  WHEN '1' THEN 1    -- HOME 1
                                  WHEN '3' THEN 2    -- HOME 2
                                  WHEN '5' THEN 4    -- WORK
                                  WHEN '7' THEN 8    -- OTHER
                                  WHEN '9' THEN 1    -- FAX (HOME 1)
                                  WHEN '8' THEN 0    -- CELL
                                  WHEN '10' THEN 0   -- ALT 1
                                  WHEN '11' THEN 0   -- ALT 2
                                  WHEN '18' THEN 0   -- ALT 3
                             END)
                     LEFT OUTER JOIN STATUSES S_FF
                         ON S_FF.RX_RECORD_NUM = FF.RX_RECORD_NUM

                UNION        

                SELECT 
                    RANK() OVER (PARTITION BY F.FD_FACILITY_ID, S.RX_NUMBER
                                 ORDER BY S.EFF_START_DATE DESC, ROW_NUMBER() OVER (ORDER BY FF.FILL_FACT_KEY)
                    ) AS DUPLICATERANK,
                    PAT.PATIENT_NUM AS PAT_NUM,
                    NULL AS PERSON_CODE,
                    PAT.LAST_NAME AS PAT_LST_NM,
                    PAT.FIRST_NAME AS PAT_FRSTNM,
                    PAT.MIDDLE_NAME AS PAT_MIDNM,
                    PAT.TITLE_ABBR AS PAT_TITLE,
                    NULL AS PAT_SUFF, ---DONT HAVE
                    TO_CHAR(PAT.BIRTH_DATE::DATE,'YYYYMMDD') AS PAT_DOB,
                    (CASE WHEN PAT.GENDER = 'M' THEN 1
                          WHEN PAT.GENDER = 'F' THEN 2
                          ELSE 0
                     END) AS GENDER,
                    PAT_ADDR.ADDRESS1 AS PAT_ADDR1,
                    PAT_ADDR.ADDRESS2 AS PAT_ADDR2,
                    PAT_ADDR.CITY AS PAT_CITY,
                    PAT_ADDR.STATE AS PAT_STATE,
                    PAT_ADDR.ZIP AS PAT_ZIP,
                    PAT_ADDR.PHONE_NUM AS PAT_PHONE,
                    NULL AS PAT_EMAIL, ---DONT HAVE
                    F.FD_NCPDP_PROVIDER_ID AS NCPDP,
                    FID.FD_VALUE AS NPI,
                    NULL AS PRESCRIBERID_QUALIFIER,
                    NULL AS PRESCRIBERID,
                    NULL AS PRESCRIBER_DEA_NBR,
                    NULL AS PRESCRIBER_STATE_LICENSE,
                    NULL AS PRESCRIBER_SPECIALTY,
                    PRESC.PRS_LAST_NAME,
                    PRESC.PRS_FIRST_NAME,
                    PRESC.PRS_MIDDLE_NAME,
                    PRESC.PRS_TITLE_ABBR,
                    PRESC.PRS_NAME_SUFFIX_ABBR,
                    PRESC_ADDR.PADR_ADDRESS1 AS PRESCR_ADDR1,
                    PRESC_ADDR.PADR_ADDRESS2 AS PRESCR_ADDR2,
                    PRESC_ADDR.PADR_CITY AS PRESCR_CITY,
                    PRESC_ADDR.PADR_STATE_CODE AS PRESCR_STATE,
                    PRESC_ADDR.PADR_ZIPCODE AS PRESCR_ZIP,
                    NULL AS PRESCR_PHONE,
                    NULL AS PRESCR_FAX,
                    S.RX_NUMBER,
                    NULL AS REFILL_NUM, ---DONT HAVE
                    '03' AS PRODUCTID_QUALIFIER,
                    DRUG.DRUG_NUM AS DRUG_NUM,
                    DRUG.DRUG_LABEL_NAME AS DRUG_LABEL_NAME,
                    NULL AS QTY_DISPENSED, ---DONT HAVE
                    NVL(S.DAYS_SUPPLY,0) AS DAYS_SUPPLY,
                    NULL AS SIG_TEXT, ---DONT HAVE
                    NULL AS DOSAGE_ID,
                    NULL AS DATE_WRITTEN, ---DONT HAVE
                    TO_CHAR(NVL(S.LAST_FILL_DATE,S.PRESCRIBED_DATE)::DATE,'YYYYMMDD') AS DATE_FILLED,
                    NULL AS DATE_SOLD, ---DONT HAVE
                    NVL(S.REFILLS_AUTHORIZED,0) AS REFILLS_ALLOWED,
                    '14' AS RX_STATUS,
                    NULL AS RX_PAT_PAID_AMT_SUBMITTED, ---DONT HAVE
                    NULL AS CO_PAY, ---DONT HAVE
                    NULL AS BIN, ---DONT HAVE
                    NULL AS PCN, ---DONT HAVE
                    NULL AS PAYER_GROUPID, ---DONT HAVE
                    NULL AS PAYER_MEMBER_ID, ---DONT HAVE
                    NULL AS PAYER_CARDHOLDERID, ---DONT HAVE
                    NULL AS PATALTPHONENBR,
                    PAT.LANGUAGE_NUM AS PD_LANGUAGE_NUM,
                    F.FD_FACILITY_ID AS PHARMACY_NUMBER,
                    (CASE WHEN S.ERP_ENROLLMENT_CODE_NUM = 3 THEN 'Y'
                          ELSE 'N'
                     END) AS AUTO_FILL,
                    TO_CHAR(S.DAW_CODE) AS DAW_CODE,
                    TO_CHAR(S.ORIGINAL_FILL_DATE::DATE,'YYYYMMDD') AS ORIGINALFILLDATE,
                    0 AS REFILLSREM, ---DONT HAVE
                    TO_CHAR(S.EXPIRY_DATE::DATE,'YYYYMMDD') AS RXEXPDATE,
                    DRUG.DRUG_NDC AS DRUG_NDC,
                    NULL AS TIMEPICKEDUP, ---DONT HAVE
                    NULL AS RXORIGIN, ---DONT HAVE
                    NULL AS SHIPMENT_DATE,
                    NULL AS CARRIERID,
                    NULL AS SHIPTRACKINGNBR,
                    NULL AS EST_DELIVER_DATE,
                    NULL AS PAT_PREFRD_CONTACT_METHOD,
                    NULL AS BRANDGENERIC_IND,
                    NULL AS SHIP_METHOD,
                    NULL AS PROD_COST,
                    NULL AS TOTAL_PRICE,
                    NULL AS PAYMENT_TYPE,
                    NULL AS RXREADYDT, --DONT HAVE
                    NULL AS CONTACTED_PRESCR_DATE,
                    NULL AS PRESCR_DENIED_DATE,
                    NULL AS PRESCR_APPROVE_DATE,
                    NULL AS LONGTERMCARE_FAC_CD,
                    NULL AS TRANSACTION_DATE --DONT HAVE
                FROM STATUSES S
                     INNER JOIN {DW}.ERXDW_PLS_ARCHIVE_VIEW.FACILITY F
                         ON F.FD_FACILITY_NUM = S.FACILITY_NUM
                        AND F.FD_IS_PPI_ENABLED = 'Y'
                     INNER JOIN {DW}.ERXDW_PLS_ARCHIVE_VIEW.FACILITY_ID FID
                         ON FID.FD_FACILITY_KEY = F.FD_FACILITY_NUM
                        AND FID.FD_TYPE = 'F08'
                        AND (
                             (FID.FD_FACILITY_KEY = 1165600 AND FID.FD_VALUE = '1376818336') --BAD DATA IN TABLE...WAITING ON SEV3 TICKET SR 1-1410344725
                              OR 
                             (FID.FD_FACILITY_KEY != 1165600)
                            )
                     INNER JOIN {DW}.ERXDW_PLS_ARCHIVE_VIEW.PRESCRIBER PRESC
                         ON PRESC.PRS_PRESCRIBER_NUM = S.PRESCRIBER_NUM
                     INNER JOIN {DW}.ERXDW_PLS_ARCHIVE_VIEW.PRESCRIBER_ADDRESS PRESC_ADDR
                         ON PRESC_ADDR.PADR_PRESCRIBER_ADDRESS_NUM = S.PRESCRIBER_ADDRESS_NUM
                     INNER JOIN {AUD}.ERXAUD_PLS_ARCHIVE_VIEW.PATIENT PAT
                         ON PAT.PATIENT_NUM = S.PATIENT_NUM
                        AND PAT.EFF_END_DATE = TO_DATE('29991231','YYYYMMDD')
                     INNER JOIN {AUD}.ERXAUD_PLS_ARCHIVE_VIEW.PATIENT_ADDRESS PAT_ADDR
                         ON PAT_ADDR.PATIENT_NUM = S.PATIENT_NUM
                     INNER JOIN {DW}.ERXDW_PLS_ARCHIVE_VIEW.PRODUCT PRODUCT
                         ON PRODUCT.PRD_PRODUCT_NUM = S.PRODUCT_NUM
                     INNER JOIN {DW}.ERXDW_PLS_ARCHIVE_VIEW.DRUG DRUG
                         ON DRUG.DRUG_NUM = PRODUCT.PRD_DRUG_NUM
                WHERE NOT EXISTS (
                          SELECT * 
                          FROM FACTS FF2
                          WHERE FF2.RX_RECORD_NUM = S.RX_RECORD_NUM
                      )
        ) 
        WHERE DUPLICATERANK = 1

        ORDER BY PHARMACY_NUMBER, RX_NUMBER, TRANSACTION_DATE, DATE_SOLD NULLS FIRST