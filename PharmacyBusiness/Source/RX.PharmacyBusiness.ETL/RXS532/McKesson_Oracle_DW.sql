SELECT
     VAX_EVENT_ID
    ,EXT_TYPE
    ,PPRL_ID
    ,RECIP_ID
    ,RECIP_FIRST_NAME
    ,RECIP_MIDDLE_NAME
    ,RECIP_LAST_NAME
    ,RECIP_DOB
    ,RECIP_SEX
    ,RECIP_ADDRESS_STREET
    ,RECIP_ADDRESS_STREET_2
    ,RECIP_ADDRESS_CITY
    ,RECIP_ADDRESS_COUNTY
    ,RECIP_ADDRESS_STATE
    ,RECIP_ADDRESS_ZIP
    ,RECIP_RACE_1
    ,RECIP_RACE_2
    ,RECIP_RACE_3
    ,RECIP_RACE_4
    ,RECIP_RACE_5
    ,RECIP_RACE_6
    ,RECIP_ETHNICITY
    ,ADMIN_DATE
    ,CVX
    ,NDC
    ,MVX
    ,LOT_NUMBER
    ,VAX_EXPIRATION
    ,VAX_ADMIN_SITE
    ,VAX_ROUTE
    ,DOSE_NUM
    ,VAX_SERIES_COMPLETE
    ,RESPONSIBLE_ORG
    ,ADMIN_NAME
    ,VTRCKS_PROV_PIN
    ,ADMIN_TYPE
    ,ADMIN_ADDRESS_STREET
    ,ADMIN_ADDRESS_STREET_2
    ,ADMIN_ADDRESS_CITY
    ,ADMIN_ADDRESS_COUNTY
    ,ADMIN_ADDRESS_STATE
    ,ADMIN_ADDRESS_ZIP
    ,VAX_REFUSAL
    ,CMORBID_STATUS
    ,SEROLOGY
    ,fd_facility_id
    ,rx_number
    ,refill_num
FROM (
    SELECT
         To_Char(ff.fill_fact_key) AS VAX_EVENT_ID
        ,'I' AS EXT_TYPE
        ,NULL AS PPRL_ID
        ,p.pd_patient_num AS RECIP_ID
        ,SURESCRIPTS_PKG.Format_For_SureScripts(p.pd_first_name, 35) AS RECIP_FIRST_NAME
        ,SURESCRIPTS_PKG.Format_For_SureScripts(p.pd_middle_name, 35) AS RECIP_MIDDLE_NAME
        ,SURESCRIPTS_PKG.Format_For_SureScripts(p.pd_last_name, 35) AS RECIP_LAST_NAME
        ,To_Char(p.pd_birth_date,'YYYY-MM-DD') AS RECIP_DOB
        ,NVL(SURESCRIPTS_PKG.Format_For_SureScripts(pd.gender, 1),'U') AS RECIP_SEX
        ,SURESCRIPTS_PKG.Format_For_SureScripts(pa.pad_address1, 35) AS RECIP_ADDRESS_STREET
        ,SURESCRIPTS_PKG.Format_For_SureScripts(pa.pad_address2, 35) AS RECIP_ADDRESS_STREET_2
        ,SURESCRIPTS_PKG.Format_For_SureScripts(pa.pad_city, 35) AS RECIP_ADDRESS_CITY
        --,SURESCRIPTS_PKG.Format_For_SureScripts(pa.pad_county, 30) AS RECIP_ADDRESS_COUNTY
        ,NULL AS RECIP_ADDRESS_COUNTY --Made NULL because too few patients have this data element.
        ,SURESCRIPTS_PKG.Format_For_SureScripts(pa.pad_state, 2) AS RECIP_ADDRESS_STATE
        ,SURESCRIPTS_PKG.Format_For_SureScripts(REGEXP_REPLACE(pa.pad_zip,'[^0-9]',''), 9) AS RECIP_ADDRESS_ZIP
        ,NULL AS RECIP_RACE_1
        ,NULL AS RECIP_RACE_2
        ,NULL AS RECIP_RACE_3
        ,NULL AS RECIP_RACE_4
        ,NULL AS RECIP_RACE_5
        ,NULL AS RECIP_RACE_6
        ,NULL AS RECIP_ETHNICITY
        ,To_Char(To_Date(To_Char(ff.sold_date_key),'YYYYMMDD'),'YYYY-MM-DD') AS ADMIN_DATE
        ,(CASE WHEN p.cvx_code is not null THEN p.cvx_code
               WHEN d.drug_labeler LIKE '%MODERNA%' THEN '207'
               WHEN d.drug_labeler LIKE '%PFIZER%' THEN '208'
               WHEN d.drug_labeler LIKE '%ASTRAZENECA%' THEN '210'
               WHEN d.drug_labeler LIKE '%JOHNSON%' OR
                    d.drug_labeler LIKE '%JANSSEN%' THEN '212'
               ELSE NULL
          END) AS CVX
        ,d.drug_ndc AS NDC
        ,(CASE WHEN p.mvx_code is not null THEN p.mvx_code
               WHEN d.drug_labeler LIKE '%MODERNA%' THEN 'MOD'
               WHEN d.drug_labeler LIKE '%PFIZER%' THEN 'PFR'
               WHEN d.drug_labeler LIKE '%ASTRAZENECA%' THEN 'ASZ'
               WHEN d.drug_labeler LIKE '%JOHNSON%' OR
                    d.drug_labeler LIKE '%JANSSEN%' THEN 'JSN'
               ELSE NULL
          END) AS MVX

        ,SURESCRIPTS_PKG.Format_For_SureScripts(fpf.dispensed_item_lot_number, 20) AS LOT_NUMBER
        ,To_Char(fpf.dispensed_item_expiration_date,'YYYY-MM-DD') AS VAX_EXPIRATION
        ,NULL AS VAX_ADMIN_SITE
        ,'C28161' AS VAX_ROUTE
        ,NULL AS DOSE_NUM
        ,NULL AS VAX_SERIES_COMPLETE
        ,'TopCo' AS RESPONSIBLE_ORG
        ,f.fd_facility_name AS ADMIN_NAME
        ,v.loc_store_no AS VTRCKS_PROV_PIN
        ,'17' AS ADMIN_TYPE
        ,f.fd_address1 AS ADMIN_ADDRESS_STREET
        ,f.fd_address2 AS ADMIN_ADDRESS_STREET_2
        ,f.fd_city AS ADMIN_ADDRESS_CITY
        ,v.loc_ship_county AS ADMIN_ADDRESS_COUNTY
        ,f.fd_state_code AS ADMIN_ADDRESS_STATE
        ,f.fd_zipcode AS ADMIN_ADDRESS_ZIP
        ,'NO' AS VAX_REFUSAL
        ,NULL AS CMORBID_STATUS
        ,'UNK' AS SEROLOGY

        ,f.fd_facility_id
        ,ff.rx_number
        ,ff.refill_num

        ,Rank() OVER (PARTITION BY ff.fill_fact_key, pa.pd_patient_key
                      ORDER BY pa.pad_address_usage, pa.pad_create_date Desc, pa.rowid
         ) AS PatientAddressRank
        ,Rank() OVER (PARTITION BY ff.fill_fact_key, fpf.order_num
                      ORDER BY
                           fpf.responsible_party_key
                          ,fpf.rowid
         ) AS FillPricingFactRank
		 
        ,Rank() OVER (PARTITION BY ff.rx_record_num
                      ORDER BY
                           ff.refill_num Desc
                          ,ff.rowid
         ) AS FillFactRank

    FROM TREXONE_DW_DATA.Fill_Fact ff
         INNER JOIN TREXONE_DW_DATA.Facility f
            ON f.fd_facility_key = ff.fd_facility_key
           AND f.FD_IS_PPI_ENABLED = 'Y' --Licensed stores
            
         LEFT OUTER JOIN TREXONE_DW_DATA.Tp_Patient tp_pat
            ON tp_pat.PD_PATIENT_KEY = ff.PD_PATIENT_KEY
           AND tp_pat.TPD_KEY = ff.TPD_KEY
           AND tp_pat.TPD_EFF_END_DATE = TO_DATE('29991231', 'YYYYMMDD')

         LEFT OUTER JOIN TREXONE_DW_DATA.Patient p
            ON p.pd_patient_key = ff.pd_patient_key
         LEFT OUTER JOIN TREXONE_DW_DATA.Patient_Address pa
            ON pa.pd_patient_key = ff.pd_patient_key
           AND pa.pad_end_date = TO_DATE('29991231','YYYYMMDD')        
         LEFT OUTER JOIN TREXONE_DW_DATA.Patient_Demographic pd
            ON pd.patient_demographic_key = ff.patient_demographic_key

         INNER JOIN TREXONE_DW_DATA.Drug d
             ON d.drug_key = ff.dispensed_drug_key
         LEFT OUTER JOIN TREXONE_DW_DATA.Fill_Pricing_Fact fpf
             ON fpf.SOLD_DATE_KEY = ff.SOLD_DATE_KEY
            AND fpf.rx_fill_seq = ff.rx_fill_seq

         INNER JOIN TREXONE_DW_DATA.PRODUCT p
             ON p.PRD_DRUG_NUM = d.DRUG_NUM

         INNER JOIN wegmans.VTRACKS v
             ON v.store_num = f.fd_facility_id
    WHERE ff.SOLD_DATE_KEY = To_Number(To_Char((:RunDate - 1),'YYYYMMDD'))
      AND ff.CANCEL_DATE_KEY = 0
      AND d.DRUG_GC3 = 'COVID-19 VACCINES'
)
WHERE PatientAddressRank = 1
  AND FillPricingFactRank = 1
  AND FillFactRank = 1