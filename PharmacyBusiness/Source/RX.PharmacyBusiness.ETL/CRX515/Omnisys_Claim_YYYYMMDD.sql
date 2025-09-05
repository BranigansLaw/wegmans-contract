WITH fill_facts AS (
    SELECT facts.*
    FROM (
        SELECT /*+ index (AUD__Fill_Fact IN_FILL_FACT_03) */
            DW__Facility_ID.FD_Value AS PHARMACY_NPI,
            AUD__Fill_Fact.Rx_Number AS PRESCRIPTION_NBR,
            AUD__Fill_Fact.Refill_Num AS REFILL_NBR,
            AUD__Fill_Fact.Date_Of_Service AS DATE_OF_SERVICE,
            Trunc(AUD__Fill_Fact.FILL_STATE_CHG_TS) AS FILL_STATE_DATE,
            DW__Drug.Drug_NDC AS NDC_NBR,
            Substr(DW__Tp_Patient.TPD_Cardholder_ID, 1, 10) AS CARDHOLDER_ID,
            AUD__Tp_Item_Claim.AUTHORIZATION_NUMBER AS AUTHORIZATION_NBR,
            AUD__Tp_Processor_Destination.BIN_NUMBER AS BIN_0,
            LEAD(AUD__Tp_Processor_Destination.bin_number,1)
                 OVER(PARTITION BY AUD__Fill_Fact.fill_state_price_num, AUD__Fill_Fact.fill_state_code
                 ORDER BY AUD__Fill_Fact.split_billing_code) AS BIN_1,
            LEAD(AUD__Tp_Processor_Destination.bin_number,2)
                 OVER(PARTITION BY AUD__Fill_Fact.fill_state_price_num, AUD__Fill_Fact.fill_state_code
                 ORDER BY AUD__Fill_Fact.split_billing_code) AS BIN_2,            
            Rank() OVER (PARTITION BY
                              DW__Facility_ID.FD_Value
                             ,AUD__Fill_Fact.Rx_Number
                             ,AUD__Fill_Fact.Refill_Num
                             ,DW__Drug.Drug_NDC
                         ORDER BY DW__Tp_Patient.tpd_eff_end_date Desc
                                 ,AUD__Fill_Fact.FILL_STATE_CHG_TS Desc
                                 ,rownum Desc
                        ) AS ClaimRank,                        
            AUD__Fill_Fact.facility_num,
            AUD__Fill_Fact.Rx_Number,
            AUD__Fill_Fact.Refill_Num,
            AUD__Fill_Fact.dispensed_drug_num, AUD__Fill_Fact.tp_patient_num
        FROM TREXONE_AUD_DATA.Fill_Fact AUD__Fill_Fact
             INNER JOIN TREXONE_DW_DATA.Facility DW__Facility
                     ON DW__Facility.fd_facility_num = AUD__Fill_Fact.facility_num
             INNER JOIN TREXONE_DW_DATA.Facility_ID DW__Facility_ID
                     ON DW__Facility_ID.fd_facility_num = DW__Facility.fd_facility_num
             INNER JOIN TREXONE_DW_DATA.Drug DW__Drug
                     ON DW__Drug.drug_num = AUD__Fill_Fact.dispensed_drug_num	
             LEFT OUTER JOIN TREXONE_DW_DATA.Tp_Patient DW__Tp_Patient
                     ON DW__Tp_Patient.tpd_tp_patient_num = AUD__Fill_Fact.tp_patient_num
             LEFT OUTER JOIN TREXONE_AUD_DATA.Tp_Processor_Destination AUD__Tp_Processor_Destination
                     ON AUD__Tp_Processor_Destination.tp_processor_destination_seq = AUD__Fill_Fact.tp_processor_dest_num
                    AND AUD__Tp_Processor_Destination.eff_end_date BETWEEN (:RunDate - 1) 
                                                                       AND To_Date('29991231','YYYYMMDD')
			 LEFT OUTER JOIN TREXONE_AUD_DATA.Tp_Item_Claim AUD__Tp_Item_Claim
                     ON AUD__Tp_Item_Claim.tp_item_claim_num = AUD__Fill_Fact.tp_item_claim_num
                    AND AUD__Tp_Item_Claim.h_level = AUD__Fill_Fact.tp_item_claim_key
                    AND AUD__Tp_Item_Claim.eff_end_date BETWEEN (:RunDate - 1) 
                                                            AND To_Date('29991231','YYYYMMDD')
        WHERE AUD__Fill_Fact.FILL_STATE_CHG_TS BETWEEN (:RunDate - 1) 
                                                   AND (:RunDate - 1 + INTERVAL '23:59:59' HOUR TO SECOND)
          AND AUD__Fill_Fact.fill_state_code = '14' --Sold
          AND DW__Facility_ID.fd_type = 'F08'
          AND (DW__Tp_Patient.tpd_eff_end_date IS NULL OR 
              DW__Tp_Patient.tpd_eff_end_date BETWEEN (:RunDate - 1) 
                                                  AND To_Date('29991231','YYYYMMDD'))
          AND AUD__Fill_Fact.is_same_day_reversal = 'N'
         ) facts
    WHERE (facts.BIN_0 = '004303' OR facts.BIN_1 = '004303' OR facts.BIN_2 = '004303')
      AND facts.ClaimRank = 1
)
SELECT 
	RPad(NVL(PHARMACY_NPI,' '), 10, '0') AS PHARMACY_NPI,
	LPad(NVL(PRESCRIPTION_NBR,' '), 12, '0') AS PRESCRIPTION_NBR,
	LPad(NVL(To_Char(REFILL_NBR),' '), 3, '0') AS REFILL_NBR,
	To_Char(DATE_OF_SERVICE,'YYYYMMDD') AS DATE_OF_SERVICE,
	To_Char(FIRST_SOLD_DATE_ANY_PLAN,'YYYYMMDD') AS SOLD_DATE,
	RPad(NVL(NDC_NBR,' '), 11, ' ') AS NDC_NBR,
	RPad(NVL(CARDHOLDER_ID,' '), 15, ' ') AS CARDHOLDER_ID,
	RPad(NVL(To_Char(AUTHORIZATION_NBR),' '), 20, ' ') AS AUTHORIZATION_NBR,
	RPad(' ', 8, ' ') AS RESERVED_FOR_FUTURE_USE
FROM (
      SELECT 
          PHARMACY_NPI,
          PRESCRIPTION_NBR,
          REFILL_NBR,
          DATE_OF_SERVICE,
          FILL_STATE_DATE,
          NDC_NBR,
          CARDHOLDER_ID,
          AUTHORIZATION_NBR,
          (SELECT /*+ index (ff IN_FILL_FACT_04) */ Trunc(Min(ff.FILL_STATE_CHG_TS))
           FROM TREXONE_AUD_DATA.Fill_Fact ff
           WHERE ff.facility_num = fills.facility_num
             AND ff.Rx_Number = fills.Rx_Number
             AND ff.Refill_Num = fills.Refill_Num
             AND ff.Date_Of_Service = fills.DATE_OF_SERVICE
             AND ff.dispensed_drug_num = fills.dispensed_drug_num
             AND ff.fill_state_code = '14' --Sold
             AND ff.is_same_day_reversal = 'N'
             AND ff.FILL_STATE_CHG_TS BETWEEN (:RunDate - 365) 
                                          AND (:RunDate - 1 + INTERVAL '23:59:59' HOUR TO SECOND)
          ) AS FIRST_SOLD_DATE_ANY_PLAN,
          (SELECT /*+ index (ff IN_FILL_FACT_04) */ Trunc(Min(ff.FILL_STATE_CHG_TS))
           FROM TREXONE_AUD_DATA.Fill_Fact ff
                INNER JOIN TREXONE_AUD_DATA.Tp_Processor_Destination tppd
                   ON tppd.tp_processor_destination_seq = ff.tp_processor_dest_num
                  AND tppd.BIN_NUMBER = '004303'
                  AND tppd.eff_end_date BETWEEN (:RunDate - 1) 
                                            AND To_Date('29991231','YYYYMMDD')
           WHERE ff.facility_num = fills.facility_num
             AND ff.Rx_Number = fills.Rx_Number
             AND ff.Refill_Num = fills.Refill_Num
             AND ff.Date_Of_Service = fills.DATE_OF_SERVICE
             AND ff.dispensed_drug_num = fills.dispensed_drug_num
             AND ff.fill_state_code = '14' --Sold
             AND ff.is_same_day_reversal = 'N'
             AND ff.FILL_STATE_CHG_TS BETWEEN (:RunDate - 365) 
                                          AND (:RunDate - 1 + INTERVAL '23:59:59' HOUR TO SECOND)
          ) AS FIRST_MEDICARE_PART_B_SOLD_DATE
      FROM fill_facts fills
     )
WHERE FIRST_MEDICARE_PART_B_SOLD_DATE = FILL_STATE_DATE
ORDER BY
    PHARMACY_NPI,
    PRESCRIPTION_NBR,
    REFILL_NBR,
    DATE_OF_SERVICE,
    FIRST_SOLD_DATE_ANY_PLAN,
    NDC_NBR