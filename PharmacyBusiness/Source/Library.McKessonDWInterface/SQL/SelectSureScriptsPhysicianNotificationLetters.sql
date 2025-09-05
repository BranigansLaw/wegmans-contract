SELECT
     ff.fill_fact_key AS RecordSequenceNumber

    --Patient--
    ,p.pd_patient_num AS ParticipantPatientId
    ,p.pd_last_name AS PatientLastName
    ,p.pd_first_name AS PatientFirstName
    ,p.pd_middle_name AS PatientMiddleName
    ,pp.patp_area_code AS PatientPhoneAreaCode
    ,pp.patp_phone_number AS PatientPhoneNumber
    ,p.pd_birth_date AS PatientDateOfBirth
    ,pd.gender AS PatientGender
    ,pa.pad_address1 AS PatientAddress1
    ,pa.pad_address2 AS PatientAddress2
    ,pa.pad_city AS PatientCity
    ,pa.pad_state AS PatientState
    ,pa.pad_zip AS PatientZipCode

    --Patients Primary Care Provider--
    --NOTE The 'ALL' state filter uses Primary Care Provider (abbreviated PCP) for all prescriber data.
    --NOTE The 'PA' state filter typically uses the Precriber from Transaction (abbreviated ff, short for Fill fact) for all prescriber data.
    --     Also for 'PA', most prescriber data fields are intentionally left blank.
    ,pres_pcp_npi.pid_id_text AS PrimaryCareProviderNpi
    ,pres_pcp.prs_prescriber_num AS InternalPrimaryCareProviderId
    ,pres_ff.prs_last_name AS TransactionPrecriberLastName
    ,pres_pcp.prs_last_name AS PrimaryCareProviderLastName
    ,pres_ff.prs_first_name AS TransactionPrecriberFirstName
    ,pres_pcp.prs_first_name AS PrimaryCareProviderFirstName
    ,pres_pcp_addr.padr_address1 AS PrimaryCareProviderAddress1
    ,pres_pcp_addr.padr_city AS PrimaryCareProviderCity
    ,pres_pcp_addr.padr_state_code AS PrimaryCareProviderState
    ,pres_pcp_addr.padr_zipcode AS PrimaryCareProviderZipCode
    ,pres_pcp_phone.prsp_area_code AS PrimaryCareProviderPhoneAreaCode
    ,pres_pcp_phone.prsp_phone_number AS PrimaryCareProviderPhoneNumber

    --NOTE The 'PA' fax number is hard coded because there is a dedicated fax number not listed in the prescriber table.
    ,pres_pcp_fax.prsp_area_code AS PrimaryCareProviderFaxAreaCode
    ,pres_pcp_fax.prsp_phone_number AS PrimaryCareProviderFaxNumber

    --Vaccine Information--
    ,pr.cvx_code AS VaccineCvxCode
    ,d.drug_labeler AS VaccineManufacturerName
    ,d.drug_label_name AS VaccineName
    ,fpf.dispensed_item_lot_number AS VaccineLotNumber
    ,fpf.dispensed_item_expiration_date AS VaccineExpirationDate
    ,d.drug_gc3 AS VaccineInformationStatementName
    ,ff.rtp_date_key AS AdministeredDate

    --Administration Details--
    ,ff.qty_dispensed AS AdministeredAmount
    ,d.drug_package_units AS AdministeredUnits
    ,d.drug_route AS RouteOfAdministrationDescription

    --Care Facility Where the vaccine was given--
    ,fid.fd_value AS FacilityNpi
    ,f.fd_facility_name AS OtherFacilityIdentification
    ,f.fd_address1 AS FacilityAddress1
    ,f.fd_address2 AS FacilityAddress2
    ,f.fd_city AS FacilityCity
    ,f.fd_state_code AS FacilityState
    ,f.fd_zipcode AS FacilityZipCode
    ,f.fd_phone1 AS FacilityPhoneNumber

    --Processing Information--
    ,fpf.counseling_accepted AS ProfessionalConsultationComplete

    --Fields needed for business logic but not in data extract--
	,d.drug_ndc AS Logic_DrugNdc
    ,ff.fill_fact_key AS Logic_FillFactKey
    ,ff.pd_patient_key AS Logic_FillFactPatientKey
    ,fid.fd_facility_num AS Logic_FacilityIdNum
    ,fid.fd_datestamp AS Logic_FacilityIdDatestamp
    ,pa.pad_address_usage AS Logic_PatientAddrUsage
    ,pa.pad_create_date AS Logic_PatientAddrCreateDate
    ,pres_pcp_phone.padr_key AS Logic_PresPhoneKey
    ,pres_pcp_phone.prsp_status AS Logic_PresPhoneStatus
    ,pres_pcp_phone.prsp_source_code AS Logic_PresPhoneSourceCode
    ,pres_pcp_phone.h_level AS Logic_PresPhoneHLevel
    ,pres_pcp_fax.padr_key AS Logic_PresFaxKey
    ,pres_pcp_fax.prsp_status AS Logic_PresFaxStatus
    ,pres_pcp_fax.prsp_source_code AS Logic_PresFaxSourceCode
    ,pres_pcp_fax.h_level AS Logic_PresFaxHLevel
    ,pres_pcp_npi.prs_prescriber_key AS Logic_PresNpiPresKey
    ,pres_pcp_npi.h_level AS Logic_PresNpiHLevel
    ,fpf.order_num AS Logic_FpfOrderNum
    ,fpf.responsible_party_key AS Logic_FpfRespPartyKey
FROM TREXONE_DW_DATA.Fill_Fact ff
     INNER JOIN TREXONE_DW_DATA.Facility f
        ON f.fd_facility_key = ff.fd_facility_key  
     INNER JOIN TREXONE_DW_DATA.Facility_ID fid
        ON fid.fd_facility_num = f.fd_facility_num
       AND fid.fd_type = 'F08'
     LEFT OUTER JOIN TREXONE_DW_DATA.Patient p
        ON p.pd_patient_key = ff.pd_patient_key
     LEFT OUTER JOIN TREXONE_DW_DATA.Patient_Phone pp
        ON pp.pd_patient_key = ff.pd_patient_key
       AND pp.eff_end_date = TO_DATE('29991231','YYYYMMDD')
     LEFT OUTER JOIN TREXONE_DW_DATA.Patient_Address pa
        ON pa.pd_patient_key = ff.pd_patient_key
       AND pa.pad_end_date = TO_DATE('29991231','YYYYMMDD')
     LEFT OUTER JOIN TREXONE_DW_DATA.Patient_Demographic pd
        ON pd.patient_demographic_key = ff.patient_demographic_key
     LEFT OUTER JOIN TREXONE_DW_DATA.Prescriber pres_pcp
        ON pres_pcp.prs_prescriber_num = p.pd_prescriber_num
     LEFT OUTER JOIN TREXONE_DW_DATA.Prescriber pres_ff
        ON pres_ff.prs_prescriber_key = ff.prs_prescriber_key
     LEFT OUTER JOIN TREXONE_DW_DATA.Prescriber_Address pres_pcp_addr
        ON pres_pcp_addr.prs_prescriber_key = pres_pcp.prs_prescriber_key
       AND pres_pcp_addr.padr_is_default = 'Y'
     LEFT OUTER JOIN TREXONE_DW_DATA.Prescriber_Phone pres_pcp_phone
        ON pres_pcp_phone.padr_key = pres_pcp_addr.padr_key
       AND pres_pcp_phone.prsp_phone_address_type = 'PRIM'
     LEFT OUTER JOIN TREXONE_DW_DATA.Prescriber_Phone pres_pcp_fax
        ON pres_pcp_fax.padr_key = pres_pcp_addr.padr_key
       AND pres_pcp_fax.prsp_phone_address_type = 'FAX'
     LEFT OUTER JOIN TREXONE_DW_DATA.Prescriber_Ids pres_pcp_npi
        ON pres_pcp_npi.prs_prescriber_key = pres_pcp.prs_prescriber_key
       AND pres_pcp_npi.pit_key = 509 --See TREXONE_DW_DATA.Prescriber_Id_Type.pit_id_description = 'National Provider Identifier (NPI)'
       AND pres_pcp_npi.pid_status = 'A'
     INNER JOIN TREXONE_DW_DATA.Drug d
         ON d.drug_key = ff.dispensed_drug_key
        AND d.drug_route != 'Oral'
     INNER JOIN TREXONE_DW_DATA.Product pr
         ON pr.PRD_DRUG_NUM = d.DRUG_NUM
        AND pr.PRD_DEACTIVATE_DATE IS NULL
     LEFT OUTER JOIN TREXONE_DW_DATA.Product_Identifier pi
         ON pi.PRD_PRODUCT_KEY = pr.PRD_PRODUCT_KEY
        AND pi.IDENTIFIER = 'VAX'
        AND pi.IS_ACTIVE = 'Y'
     LEFT OUTER JOIN TREXONE_DW_DATA.Fill_Pricing_Fact fpf
         ON fpf.SOLD_DATE_KEY = ff.SOLD_DATE_KEY
        AND fpf.rx_fill_seq = ff.rx_fill_seq
        AND fpf.PARTITION_DATE BETWEEN (:RunDate - 60) AND (:RunDate + 1)
WHERE ff.PARTITION_DATE BETWEEN (:RunDate - 60) AND (:RunDate + 1)
  AND ff.SOLD_DATE_KEY = To_Number(To_Char((:RunDate - 1),'YYYYMMDD'))
  AND ff.CANCEL_DATE_KEY = 0
  --Exclude previously sold prescriptions (some updated records to exclude may appear like a new sold record because it was BARd).
  AND NOT EXISTS (
        SELECT /*+ index (ff IN_FILL_FACT_03) */ *
        FROM TREXONE_AUD_DATA.Fill_Fact prior_sales
        WHERE prior_sales.facility_num = f.fd_facility_num
          AND prior_sales.Rx_Number = ff.rx_number
          AND prior_sales.Refill_Num = ff.refill_num
          AND prior_sales.dispensed_drug_num = d.drug_num
          AND prior_sales.fill_state_code = '14' --Sold
          AND prior_sales.is_same_day_reversal = 'N'
		  --Presciption numbers are only valid for up to one year.
          AND prior_sales.fill_state_chg_ts BETWEEN (:RunDate - 365) AND (:RunDate - 2 + INTERVAL '23:59:59' HOUR TO SECOND)		  
      )
  AND (
        (pi.IDENTIFIER = 'VAX' AND pi.IS_ACTIVE = 'Y')
        OR
        (d.DRUG_GC3 IN (
           'COVID-19 VACCINES'
          ,'ENTERIC VIRUS VACCINES'
          ,'GRAM NEGATIVE COCCI VACCINES'
          ,'GRAM POSITIVE COCCI VACCINES'
          ,'INFLUENZA VIRUS VACCINES'
          ,'NEUROTOXIC VIRUS VACCINES'
          ,'TOXIN-PRODUCING BACILLI VACCINES/TOXOIDS'
          ,'VACCINE/TOXOID PREPARATIONS,COMBINATIONS'
          ,'GRAM (-) BACILLI (NON-ENTERIC) VACCINES'
          ,'VIRAL/TUMORIGENIC VACCINES'
          ,'ANTINEOPLASTICS,MISCELLANEOUS'
          )
        )
	    OR
        --New Flu shots for fall 2018, per SR 208272
        (d.drug_ndc IN (
           '00000000010'
          ,'00000000042'
          ,'00000000057'
          ,'00000000043'
          ,'00000000056'
          ,'00000000045'
          ,'00000000046'
          ,'00000000048'
          ,'00000000049'
          ,'00000000050'
          ,'00000000051'
          ,'00000000052'
          ,'00000000053'
          ,'00000000054'
          ,'00000000055'
          ,'00000000047'
          ,'00000000044'
          ,'33332041810'
          ,'33332031801'
          ,'33332011810'
          ,'58160089841'
          ,'58160089852'
          ,'49281071888'
          ,'49281071810'
          ,'70461031803'
          ,'19515090952'
          ,'19515090941'
          ,'19515090011'
          ,'19515090001'
          ,'66019030510'
          ,'49281040365'
          ,'49281040388'
          ,'49281041888'
          ,'49281041850'
          ,'49281062978'
          ,'49281062915'
          ,'49281041810'
          ,'49281041858'
          ,'49281051800'
          ,'49281051825'
          ,'00000000037'
          ,'00000000038'
          ,'00000000039'
          ,'00000000040'
          )
        )
      )
