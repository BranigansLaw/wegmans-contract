SELECT
	 ff.fill_fact_key AS RecordSequenceNumber

	--Patient--
	,p.pd_patient_num AS ParticipantPatientId
	,p.pd_last_name AS PatientLastName
	,p.pd_first_name AS PatientFirstName
	,p.pd_middle_name AS PatientMiddleName
	,p.pd_title_abbr AS PatientPrefix
	,p.pd_name_suffix_abbr AS PatientSufix
	,p.pd_birth_date AS PatientDateOfBirth
	,pd.gender AS PatientGender
	,pa.pad_address1 AS PatientAddress1
	,pa.pad_city AS PatientCity
	,pa.pad_state AS PatientState
	,pa.pad_zip AS PatientZipCode

	--Pharmacy--
	,f.fd_ncpdp_provider_id AS NcpdpId
	,f.fd_facility_id AS ChainSiteId
	,f.fd_facility_name AS PharmacyName
	,f.fd_address1 AS FacilityAddress1
	,f.fd_city AS FacilityCity
	,f.fd_state_code AS FacilityState
	,f.fd_zipcode AS FacilityZipCode
	,f.fd_phone1 AS FacilityPhoneNumber

	--Prescriber--
	,pres.prs_last_name AS FPrimaryCareProviderLastName
	,pres.prs_first_name AS PrimaryCareProviderFirstName
	,pres_addr.padr_address1 AS PrimaryCareProviderAddress1
	,pres_addr.padr_city AS PrimaryCareProviderCity
	,pres_addr.padr_state_code AS PrimaryCareProviderState
	,pres_addr.padr_zipcode AS PrimaryCareProviderZipCode
	,pres_phone.prsp_area_code AS PrimaryCareProviderAreaCode
	,pres_phone.prsp_phone_number AS PrimaryCareProviderPhoneNumber

	--Medication--
	,ff.rx_number AS PrescriptionNumber
	,ff.refill_num AS FillNumber
	,d.drug_ndc AS NdcNumberDispensed
	,d.drug_label_name AS MedicationName
	,ff.intended_qty_dispensed AS QuantityPrescribed
	,ff.qty_dispensed AS QuantityDispensed
	,ff.days_supply AS DaysSupply
	,ff.sig AS SigText
	,ff.written_date_key AS DateWritten
	,ff.ready_date_key AS DateFilled
	,ff.sold_date_key AS DatePickedUpDispensed
	,ff.total_refills_allowed AS RefillsOriginallyAuthorized
	,ff.total_refills_remaining AS RefillsRemaining

	--Fields needed for business logic but not in data extract--
	,ff.fill_fact_key AS Logic_FillFactKey
	,ff.pd_patient_key AS Logic_PdPatientKey
	,pa.pad_address_usage AS Logic_PatientAddressUsage
	,pa.pad_create_date AS Logic_PatientAddressCreateDate
	,pres_phone.padr_key AS Logic_PresPhoneKey
	,pres_phone.prsp_status AS Logic_PresPhoneStatus
	,pres_phone.prsp_source_code AS Logic_PresPhoneSourceCode
	,pres_phone.h_level AS Logic_PresPhoneHLevel
FROM TREXONE_DW_DATA.Fill_Fact ff
     INNER JOIN TREXONE_DW_DATA.Facility f
        ON f.fd_facility_key = ff.fd_facility_key
     LEFT OUTER JOIN TREXONE_DW_DATA.Patient p
        ON p.pd_patient_key = ff.pd_patient_key
     LEFT OUTER JOIN TREXONE_DW_DATA.Patient_Address pa
        ON pa.pd_patient_key = ff.pd_patient_key
       AND pa.pad_end_date = To_Date('29991231','YYYYMMDD')
     LEFT OUTER JOIN TREXONE_DW_DATA.Patient_Demographic pd
        ON pd.patient_demographic_key = ff.patient_demographic_key
     LEFT OUTER JOIN TREXONE_DW_DATA.Prescriber pres
        ON pres.prs_prescriber_key = ff.prs_prescriber_key
     LEFT OUTER JOIN TREXONE_DW_DATA.Prescriber_Address pres_addr
        ON pres_addr.prs_prescriber_key = pres.prs_prescriber_key
       AND pres_addr.padr_is_default = 'Y'        
     LEFT OUTER JOIN TREXONE_DW_DATA.Prescriber_Phone pres_phone
        ON pres_phone.padr_key = pres_addr.padr_key
       AND pres_phone.prsp_phone_address_type = 'PRIM'
     INNER JOIN TREXONE_DW_DATA.Drug d
        ON d.drug_key = ff.dispensed_drug_key
     INNER JOIN TREXONE_DW_DATA.Product pr
        ON pr.PRD_DRUG_NUM = d.DRUG_NUM
       AND pr.PRD_DEACTIVATE_DATE IS NULL
WHERE ff.PARTITION_DATE BETWEEN (:RunDate - 60) AND (:RunDate + 1)
  AND ff.SOLD_DATE_KEY = To_Number(To_Char((:RunDate - 1),'YYYYMMDD'))
  AND ff.CANCEL_DATE_KEY = 0
